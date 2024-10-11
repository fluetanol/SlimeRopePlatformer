
using System;
using Unity.VisualScripting;
using UnityEngine;

/*
    1. 플레이어의 충돌 방식         -> 수평, 수직 충돌 + 트리거 특수 판정
    2. 움직이는 플랫폼의 충돌 방식  -> 수평, 수직 충돌
    3. 특수 오브젝트의 충돌 방식    -> 트리거 또는 수평 수직이 의미없는 충돌
*/

//플레이어의 물리적 움직임과 관련된 컴포넌트들을 관리하고 결합시키고 제어하는 시스템
[RequireComponent(typeof(PlayerData))]
[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerKinematicMove : KinematicPhysics
{
    public ISetJumpValue IsetJumpValue{
        get; private set;
    }
    public IAttackAction IattackAction{
        get; private set;
    }

    private IGetPlayerData _playerData;
    private IGetPlayerStateData _playerStateData;
    private ISetState ISetState;

    private ICollisionResult IcollisionResult;
    private IRopeResult IRopeResult;

    private float RopeForce;
    private float JumpForce;


    //just for debugging....
    [SerializeField] private Vector2 moveHorizontal, moveVertical, basehorizontal, baseVertical, baseVector, moveDelta, slopeDirection;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isJump;
    
    protected override void InterfaceInitialize()
    {
        ISetState = GetComponent<PlayerStateMachines>();
        _playerData = GetComponent<PlayerData>();
        _playerStateData = GetComponent<PlayerData>();

        CapsuleCollider2D CapsuleCollider2D = _playerData.GetPlayerComponent().CapsuleCollider2D;

        PlayerAccelMove accelMove = new PlayerAccelMove();
        Move = accelMove;
        IsetMoveVelocity = accelMove;
        IsetDirection = accelMove;
        IsetMoveState = accelMove;
        IsetJumpValue = accelMove;

        PlayerKinematicCollision playerCollision = new PlayerKinematicCollision(CapsuleCollider2D);
        IOverlapCollision = playerCollision;
        ISeperateCollision = playerCollision;
        IcollisionResult = playerCollision;

        RopeAction ropeAction =  new RopeAction(IsetJumpValue, IsetMoveState, _playerData);
        IRopeResult = ropeAction;
        IattackAction = ropeAction;
    }

    protected override void SettingInitialize()
    {
        JumpForce = _playerData.GetPlayerPhysicsStats().JumpForce;
        RopeForce = _playerData.GetPlayerPhysicsStats().AttackForce;
    }

    void FixedUpdate() {
        Vector2 currentPosition = _playerData.GetPlayerComponent().Rigidbody2D.position;
        //PlayerPhysicsStateUpdate();

        PreCollisionFixedUpdate(currentPosition);
        AttackStateUpdate(currentPosition);

        VelocityFixedUpdate();
        PostCollisionFixedUpdate(currentPosition);
        PostVelocityFixedUpdate(currentPosition);

        _playerData.GetPlayerComponent().Rigidbody2D.MovePosition(currentPosition + moveDelta);
    }


    //물리 계산 시작 전 플레이어 상태에 따른 물리적 상태 업데이트
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

    //공격 상태일 때의 업데이트 (이건 인자 때문에 구조적으로 일단 빼야했음)
    private void AttackStateUpdate(Vector2 currentPosition){
        if (IRopeResult.IsFinish(currentPosition))
        {
            ISetState.SetBehaviourState(EPlayerBehaviourState.Normal);
        }
    }

    //충돌 전 현재 위치에 따라 올바른 충돌 계산을 위한 전처리
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

    //충돌 후 전처리 된 위치에 따른 충돌 계산
    private void PostCollisionFixedUpdate(Vector2 currentPosition){
        moveDelta += ISeperateCollision.VerticalCollision(currentPosition + moveDelta, moveVertical);
        moveDelta += ISeperateCollision.HorizontalCollision(currentPosition + moveDelta, moveHorizontal);

        Vector2 verticalNormal = IcollisionResult.GetVerticalNormal();
        Vector2 horizontalNormal = IcollisionResult.GetHorizontalNormal();

        if(verticalNormal != horizontalNormal && horizontalNormal != Vector2.zero){
            moveDelta += ISeperateCollision.VerticalCollision(currentPosition + moveDelta, Vector2.down);
        }

        ECollisionType verticalCollisionType = IcollisionResult.VerticalCollisionType();
        ECollisionType horizontalCollisionType = IcollisionResult.HorizontalCollisionType();

        verticalCollisionAction(verticalCollisionType);
        horizontalCollisionAction(horizontalCollisionType);
    }


    private void verticalCollisionAction(ECollisionType verticalCollisionType){
        switch (verticalCollisionType)
        {
            case ECollisionType.Ground:
                ISetState.SetGroundState(EPlayerLandState.Land);
                ISetState.SetMoveState(EPlayerMoveState.Idle);
                ISetState.SetBehaviourState(EPlayerBehaviourState.Normal);

                IsetDirection.SetSlopeDirection(IcollisionResult.GetVerticalNormal());
                IsetJumpValue.SetJumpDirection(Vector2.up);
                break;

            case ECollisionType.Wall:
                ISetState.SetMoveState(EPlayerMoveState.Idle);
                ISetState.SetBehaviourState(EPlayerBehaviourState.Normal);    
                IsetDirection.SetSlopeDirection(IcollisionResult.GetVerticalNormal());
                IsetMoveVelocity.SetVerticalVelocity(Vector2.zero);
                IsetJumpValue.SetJumpDirection(Vector2.up);
                break;

            case ECollisionType.Air:
                ISetState.SetGroundState(EPlayerLandState.Air);
                IsetDirection.SetSlopeDirection(Vector2.up);
                break;
        }
    }

    private void horizontalCollisionAction(ECollisionType horizontalCollisionType){
        switch (horizontalCollisionType)
        {
            case ECollisionType.Ground:
                ISetState.SetGroundState(EPlayerLandState.Land);
                ISetState.SetMoveState(EPlayerMoveState.Idle);
                ISetState.SetBehaviourState(EPlayerBehaviourState.Normal);

                IsetDirection.SetSlopeDirection(IcollisionResult.GetHorizontalNormal());
                IsetJumpValue.SetJumpDirection(Vector2.up);
                break;
            case ECollisionType.Wall:
                ISetState.SetBehaviourState(EPlayerBehaviourState.Normal);
                IsetMoveVelocity.SetHorizontalVelocity(Vector2.zero);
                IsetJumpValue.SetJumpDirection(Vector2.up);
                break;
            case ECollisionType.Air:
                break;
        }
    }

    /// <summary>
    /// 충돌 전, 현재 상태에 따른 캐릭터의 물리적 움직임 계산
    /// </summary>
    private void VelocityFixedUpdate()
    {
        ref PhysicsStats _playerPhysicsStats = ref _playerData.GetPlayerPhysicsStats();
        ref InputState _playerInputState = ref _playerData.GetPlayerInputState();

        moveHorizontal = Move.MoveHorizontalFixedUpdate(ref _playerPhysicsStats, ref _playerInputState);
        moveVertical = Move.MoveVerticalFixedUpdate(ref _playerPhysicsStats, ref _playerInputState);
        basehorizontal = Move.MoveBaseHorizontalVelocity();
        baseVertical = Move.MoveBaseVerticalVelocity();
    }


    //모든 충돌 처리 후, 강제로 시행되어야 할 특정 물리적 움직임에 대한 처리 (특히 플랫폼 관련 이동 때문에 어쩔수가 없음)
    private void PostVelocityFixedUpdate(Vector2 currentPosition){
            moveDelta += baseVertical + basehorizontal;
    }



    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.7f);
    }
}
