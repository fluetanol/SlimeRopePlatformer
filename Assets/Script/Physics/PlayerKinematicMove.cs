using System;
using Unity.VisualScripting;
using UnityEngine;

/*
    1. 플레이어의 충돌 방식         -> 수평, 수직 충돌 + 트리거 특수 판정
    2. 움직이는 플랫폼의 충돌 방식  -> 수평, 수직 충돌
    3. 특수 오브젝트의 충돌 방식    -> 트리거 또는 수평 수직이 의미없는 충돌
*/

public interface IAttackAction{
    void Attack();
}

public interface IRopeResult{
    public bool IsFinish(Vector2 position);
}


public class RopeAction : IAttackAction, IRopeResult{
    private float _ropeVelcity = 20f;
    private ISetMoveVelocity _IsetMoveVelocity;
    private ISetMoveState _IsetMoveState;
    private IGetPlayerData _playerData;

    public RopeAction(ISetMoveVelocity IsetMoveVelocity, ISetMoveState IsetMoveState, IGetPlayerData playerData){
        _IsetMoveVelocity = IsetMoveVelocity;
        _IsetMoveState = IsetMoveState;
        _playerData = playerData;
    
    }

    public void Attack(){
        Vector2 dir = _playerData.GetAttackData().attackDirection;
        _IsetMoveState.SetGravityState(false);
    
         Vector2 ropeVelocity = dir * _ropeVelcity * Time.fixedDeltaTime;
         Vector2 horizontalVelocity = new Vector2(ropeVelocity.x, 0);
         Vector2 verticalVelocity = new Vector2(0, ropeVelocity.y);
         
        _IsetMoveVelocity.SetBaseHorizontalVelocity(horizontalVelocity);
        _IsetMoveVelocity.SetBaseVerticalVelocity(verticalVelocity);
    }

    public bool IsFinish(Vector2 position){
        Vector2 attackPosition = _playerData.GetAttackData().attackPosition;
        Vector2 dir = _playerData.GetAttackData().attackDirection;

        if(dir.x > 0 && position.x > attackPosition.x)
            return true;
        else if(dir.x < 0 && position.x < attackPosition.x)
            return true;
        else if(dir.y > 0 && position.y > attackPosition.y)
            return true;
        else if(dir.y < 0 && position.y < attackPosition.y)
            return true;
            
        
        return false;
    }
}






//플레이어의 물리적 움직임과 관련된 컴포넌트들을 관리하고 결합시키고 제어하는 시스템
public class PlayerKinematicMove : KinematicPhysics
{
    private IGetPlayerData _playerData;
    private IGetPlayerStateData _playerStateData;
    private ICollisionResult IcollisionResult;

    private IAttackAction IAttackAction;
    private IRopeResult IRopeResult;

    //public event Action OnStateChange;

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
        IsetDirection = accelMove;
        IsetMoveState = accelMove;

        PlayerKinematicCollision playerCollision = new PlayerKinematicCollision(CapsuleCollider2D);
        IOverlapCollision = playerCollision;
        ISeperateCollision = playerCollision;
        IcollisionResult = playerCollision;

        RopeAction ropeAction =  new RopeAction(IsetMoveVelocity, IsetMoveState, _playerData);
        IRopeResult = ropeAction;
        IAttackAction = ropeAction;

    
    }


    void FixedUpdate() {
        PlayerPhysicsStateUpdate();
        Vector2 currentPosition = _playerData.GetPlayerComponent().Rigidbody2D.position;

        PreCollisionFixedUpdate(currentPosition);
        AttackStateUpdate(currentPosition);

        PreVelocityFixedUpdate(ref moveHorizontal, ref moveVertical, ref basehorizontal, ref baseVertical);
        PostCollisionFixedUpdate(currentPosition);
        PostVelocityFixedUpdate(currentPosition);

//        print(basehorizontal+" "+baseVertical);
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


    private void AttackStateUpdate(Vector2 currentPosition){

        if (_playerStateData.GetPlayerStateMachine()._playerBehaviourState == EPlayerBehaviourState.Attack){
            IAttackAction.Attack();

            if (IRopeResult.IsFinish(currentPosition))
            {
                print("?!");
                _playerStateData.GetPlayerStateMachine()._playerBehaviourState = EPlayerBehaviourState.Normal;
                _playerStateData.GetPlayerStateMachine()._playerMoveState = EPlayerMoveState.Jump;
                IsetMoveState.SetGravityState(true);
                IsetMoveState.SetJumpState(true);
                
                basehorizontal = Move.MoveBaseHorizontalVelocity();
                baseVertical = Move.MoveBaseVerticalVelocity();
            
                IsetDirection.SetJumpDirection((basehorizontal + baseVertical).normalized);
            }
        }
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
                IsetDirection.SetSlopeDirection(IcollisionResult.GetVerticalNormal());
                IsetDirection.SetJumpDirection(Vector2.up);
                break;
            case ECollisionType.Wall:
                _playerStateData.GetPlayerStateMachine()._playerMoveState = EPlayerMoveState.Idle;
                IsetDirection.SetSlopeDirection(IcollisionResult.GetVerticalNormal());
                IsetMoveVelocity.SetVerticalVelocity(Vector2.zero);
                break;

            case ECollisionType.Air:
                _playerStateData.GetPlayerStateMachine()._playerLandState = EPlayerLandState.Air;
                IsetDirection.SetSlopeDirection(Vector2.up);
                break;
        }
    }

    private void horizontalCollisionAction(ECollisionType horizontalCollisionType){
        switch (horizontalCollisionType)
        {
            case ECollisionType.Ground:
                _playerStateData.GetPlayerStateMachine()._playerLandState = EPlayerLandState.Land;
                _playerStateData.GetPlayerStateMachine()._playerMoveState = EPlayerMoveState.Idle;
                IsetDirection.SetSlopeDirection(IcollisionResult.GetHorizontalNormal());
                IsetDirection.SetJumpDirection(Vector2.up);
                break;
            case ECollisionType.Wall:
                IsetMoveVelocity.SetHorizontalVelocity(Vector2.zero);
                break;
            case ECollisionType.Air:
                break;
        }
    }

    
    private void PostVelocityFixedUpdate(Vector2 currentPosition){
        moveDelta += ISeperateCollision.VerticalCollision(currentPosition + moveDelta, baseVertical);
        moveDelta += ISeperateCollision.HorizontalCollision(currentPosition+moveDelta, basehorizontal);
        
        print(IcollisionResult.GetVerticalNormal());
       
        
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
