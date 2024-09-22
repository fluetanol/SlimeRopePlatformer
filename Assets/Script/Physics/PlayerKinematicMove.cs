using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
/*
    1. 플레이어의 충돌 방식         -> 수평, 수직 충돌 + 트리거 특수 판정
    2. 움직이는 플랫폼의 충돌 방식  -> 수평, 수직 충돌
    3. 특수 오브젝트의 충돌 방식    -> 트리거 또는 수평 수직이 의미없는 충돌
*/


//플레이어의 물리적 움직임과 관련된 컴포넌트들을 관리하고 결합시키고 제어하는 시스템
public class PlayerKinematicMove : KinematicPhysics
{
    private IGetPlayerData _playerData;
    private IGetPlayerStateData _playerStateData;
    private ICollisionResult IcollisionResult;

    public event Action OnStateChange;

    //just for debugging....
    [SerializeField] private Vector2 moveHorizontal, moveVertical, basehorizontal, baseVertical, baseVector, moveDelta, slopeDirection;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isJump;
    
    protected override void InterfaceInitialize()
    {
        _playerData = GetComponent<PlayerData>();
        _playerStateData = GetComponent<PlayerData>();

        CapsuleCollider2D CapsuleCollider2D = _playerData.GetPlayerComponent().CapsuleCollider2D;

        PlayerAccelMove accelMove = new PlayerAccelMove();
        Move = accelMove;
        IsetMoveVelocity = accelMove;
        IsetSlopeDirection = accelMove;
        IsetMoveState = accelMove;

        PlayerKinematicCollision playerCollision = new PlayerKinematicCollision(CapsuleCollider2D);
        IOverlapCollision = playerCollision;
        ISeperateCollision = playerCollision;
        IcollisionResult = playerCollision;
        
        //IStepRaycast = playerCollision;
    }


    void FixedUpdate() {
        PlayerPhysicsStateUpdate();

        Vector2 currentPosition = _playerData.GetPlayerComponent().Rigidbody2D.position;

        PreCollisionFixedUpdate(currentPosition);
        PreVelocityFixedUpdate(ref moveHorizontal, ref moveVertical, ref basehorizontal, ref baseVertical);
        PostCollisionFixedUpdate(currentPosition);
        PostVelocityFixedUpdate();

        _playerData.GetPlayerComponent().Rigidbody2D.MovePosition(currentPosition + moveDelta);
    }

    private void PlayerPhysicsStateUpdate(){
        if (_playerStateData.GetPlayerStateMachine()._playerMoveState == EPlayerMoveState.Jump){
            IsetMoveState.SetGravityState(true);
            IsetMoveState.SetGroundState(false);
            IsetMoveState.SetJumpState(true);
        }
        else{
            IsetMoveState.SetJumpState(false);
        }

        if (_playerStateData.GetPlayerStateMachine()._playerLandState == EPlayerLandState.Land)
            IsetMoveState.SetGroundState(true);

        else if (_playerStateData.GetPlayerStateMachine()._playerLandState == EPlayerLandState.Air)
            IsetMoveState.SetGroundState(false);
    }


    private void PreCollisionFixedUpdate(Vector2 currentPosition){
        moveDelta = IOverlapCollision.OverlapCollision(currentPosition);
        EOverlapType overlapType = IcollisionResult.OverlapCollisionType();
        Collider2D overlapHit = IcollisionResult.GetOverlapHit();

        switch(overlapType){
            case EOverlapType.Overlap:
                IPlatformVelocity iPlatformVelocity = overlapHit.GetComponent<IPlatformVelocity>();
                Vector2 horizontalVelocity = iPlatformVelocity.GetHorizontalPlatformVelocity();
                Vector2 verticalVelocity = iPlatformVelocity.GetVerticalPlatformVelocity();
                IsetMoveVelocity.SetBaseHorizontalVelocity(horizontalVelocity);
                IsetMoveVelocity.SetBaseVerticalVelocity(verticalVelocity);
                break;

            case EOverlapType.Seperate:
                IsetMoveVelocity.SetBaseHorizontalVelocity(Vector2.zero);
                IsetMoveVelocity.SetBaseVerticalVelocity(Vector2.zero);
                break;
        }
    }

    private void PostCollisionFixedUpdate(Vector2 currentPosition){
        moveDelta += ISeperateCollision.VerticalCollision(currentPosition + moveDelta, moveVertical);
        moveDelta += ISeperateCollision.HorizontalCollision(currentPosition + moveDelta, moveHorizontal);

        Vector2 verticalNormal = IcollisionResult.GetVerticalNormal();
        Vector2 horizontalNormal = IcollisionResult.GetHorizontalNormal();

        if(verticalNormal != horizontalNormal && horizontalNormal != Vector2.zero){
            
            moveDelta += ISeperateCollision.VerticalCollision(currentPosition + moveDelta, Vector2.down);

        }
        print(verticalNormal+" "+horizontalNormal);
        

        ECollisionType verticalCollisionType = IcollisionResult.VerticalCollisionType();
        ECollisionType horizontalCollisionType = IcollisionResult.HorizontalCollisionType();

        verticalCollisionAction(verticalCollisionType);
        horizontalCollisionAction(horizontalCollisionType);
    }

    private void verticalCollisionAction(ECollisionType verticalCollisionType){
        switch (verticalCollisionType)
        {
            case ECollisionType.Ground:
                _playerStateData.GetPlayerStateMachine()._playerLandState = EPlayerLandState.Land;
                _playerStateData.GetPlayerStateMachine()._playerMoveState = EPlayerMoveState.Idle;
                IsetSlopeDirection.SetSlopeDirection(IcollisionResult.GetVerticalNormal());
                break;
            case ECollisionType.Wall:
                _playerStateData.GetPlayerStateMachine()._playerMoveState = EPlayerMoveState.Idle;
                IsetSlopeDirection.SetSlopeDirection(IcollisionResult.GetVerticalNormal());
                IsetMoveVelocity.SetVerticalVelocity(Vector2.zero);
                break;

            case ECollisionType.Air:
                _playerStateData.GetPlayerStateMachine()._playerLandState = EPlayerLandState.Air;
                IsetSlopeDirection.SetSlopeDirection(Vector2.up);
                break;
        }
    }

    private void horizontalCollisionAction(ECollisionType horizontalCollisionType){
        switch (horizontalCollisionType)
        {
            case ECollisionType.Ground:
                _playerStateData.GetPlayerStateMachine()._playerLandState = EPlayerLandState.Land;
                _playerStateData.GetPlayerStateMachine()._playerMoveState = EPlayerMoveState.Idle;
                IsetSlopeDirection.SetSlopeDirection(IcollisionResult.GetHorizontalNormal());
                break;
            case ECollisionType.Wall:
                IsetMoveVelocity.SetHorizontalVelocity(Vector2.zero);
                break;
            case ECollisionType.Air:
                break;
        }
    }

    
    private void PostVelocityFixedUpdate(){
        baseVector = basehorizontal + baseVertical;
        moveDelta += baseVector;
    }


    private void PreVelocityFixedUpdate(ref Vector2 moveHorizontal, ref Vector2 moveVertical, ref Vector2 basehorizontal, ref Vector2 baseVertical){
        ref PhysicsStats _playerPhysicsStats = ref _playerData.GetPlayerPhysicsStats();
        ref InputState _playerInputState = ref _playerData.GetPlayerInputState();

        moveHorizontal = Move.MoveHorizontalFixedUpdate(ref _playerPhysicsStats, ref _playerInputState);
        moveVertical = Move.MoveVerticalFixedUpdate(ref _playerPhysicsStats, ref _playerInputState);
        basehorizontal = Move.MoveBaseHorizontalVelocity();
        baseVertical = Move.MoveBaseVerticalVelocity(); 
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.7f);
    }
}
