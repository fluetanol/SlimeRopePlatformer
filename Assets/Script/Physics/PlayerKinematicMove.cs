using System;
using UnityEngine;
using UnityEngine.InputSystem;

/*
    1. 플레이어의 충돌 방식         -> 수평, 수직 충돌 + 트리거 특수 판정
    2. 움직이는 플랫폼의 충돌 방식  -> 수평, 수직 충돌
    3. 특수 오브젝트의 충돌 방식    -> 트리거 또는 수평 수직이 의미없는 충돌
*/


public interface IOverlapCollision{
    public Vector2 OverlapCollision(Vector2 currentPosition);
}

public interface ISeperateCollision{
    public Vector2 VerticalCollision(Vector2 currentPosition, Vector2 moveDelta);
    public Vector2 HorizontalCollision(Vector2 currentPosition, Vector2 moveDelta);
}

public interface IMixCollision{
    public Vector2 Collision(Vector2 currentPosition, Vector2 moveDelta);
}



public class PlayerKinematicCollision : IOverlapCollision, ISeperateCollision
{
    private CapsuleCollider2D collider;
    private ISetMoveVelocity IsetMoveVelocity;
    private ISetSlopeDirection IsetSlopeDirection;
    private IPhysicsInfo iPhysicsInfo;
    private float allowAngle = 5;

    public PlayerKinematicCollision(CapsuleCollider2D collider, ISetSlopeDirection isetSlopeDirection, ISetMoveVelocity isetMoveVelocity, IPhysicsInfo iphyicsInfo){
        
        this.collider = collider;
        this.IsetSlopeDirection = isetSlopeDirection;
        this.IsetMoveVelocity = isetMoveVelocity;
        this.iPhysicsInfo = iphyicsInfo;
    
    }

    public Vector2 OverlapCollision(Vector2 currentPosition){
        Physics2D.queriesStartInColliders = false;
        Collider2D[] hit = new Collider2D[1];

        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(LayerMask.GetMask("Platform"));

        Vector2 delta = Vector2.zero;
        int k = collider.OverlapCollider(contactFilter, hit);

        if (k > 0){
            Vector2 mydirection = (currentPosition - (Vector2)hit[0].bounds.center).normalized;

            float platformAngle = Vector2.Angle(Vector2.zero + (Vector2)hit[0].bounds.extents, Vector2.right) + allowAngle;
            float currentAngle = Vector2.Angle(mydirection, Vector2.right);

            if (platformAngle >= currentAngle){
                delta = Vector2.right * Math.Abs((hit[0].bounds.center + hit[0].bounds.extents).x - currentPosition.x);
            }
            else if (platformAngle <= currentAngle && 180 - platformAngle >= currentAngle){
                IsetMoveVelocity.SetBaseHorizontalVelocity(hit[0].GetComponent<PlatformKinematicMove>().GetPlatformVelocity());
            }
            else if (180 - platformAngle <= currentAngle)
            {
                delta = Vector2.left * Math.Abs((hit[0].bounds.center - hit[0].bounds.extents).x - currentPosition.x);
            }
        }
        else
        {
            IsetMoveVelocity.SetBaseHorizontalVelocity(Vector2.zero);
        }
        return delta;
    }

    public Vector2 VerticalCollision(Vector2 currentPosition, Vector2 moveDelta){
        Vector2 size = collider.size;
        CapsuleDirection2D colliderDirection = collider.direction;
        Physics2D.queriesStartInColliders = false;

        RaycastHit2D hit = Physics2D.CapsuleCast(currentPosition, size, colliderDirection, 0, moveDelta, moveDelta.magnitude + 0.01f);
        if (hit.collider != null)
        {
            IsetSlopeDirection.SetSlopeDirection(hit.normal);
            float angle = Vector2.Angle(hit.normal, Vector2.up);
            if (angle > 90) IsetMoveVelocity.SetVerticalVelocity(Vector2.zero);
            else{
                iPhysicsInfo.SetGroundState(true);
                moveDelta = moveDelta.normalized * (hit.distance - 0.01f);
            }
            iPhysicsInfo.SetJumpState(false);
        }
        else{
            iPhysicsInfo.SetGroundState(false);
            IsetSlopeDirection.SetSlopeDirection(Vector2.up);
        }

        return moveDelta;
    }

    public Vector2 HorizontalCollision(Vector2 currentPosition, Vector2 moveDelta){
        Vector2 size = collider.size;
        CapsuleDirection2D colliderDirection = collider.direction;
        Physics2D.queriesStartInColliders = false;
        Vector2 delta = Vector2.zero;
        int collisionCount = iPhysicsInfo.GetCollisionCount();


        for (int i = 0; i < collisionCount; i++)
        {
            RaycastHit2D hit = Physics2D.CapsuleCast(currentPosition, size, colliderDirection, 0, moveDelta, moveDelta.magnitude + 0.01f);

            if (hit.collider != null)
            {
                float angle = Vector2.Angle(hit.normal, Vector2.up);

                if (angle > 50 && angle <= 90)
                {
                    IsetMoveVelocity.SetHorizontalVelocity(Vector2.zero);
                    delta += moveDelta.normalized * (hit.distance - 0.01f);
                    break;
                }

                else if (angle <= 50)
                {
                    iPhysicsInfo.SetGroundState(true);
                    iPhysicsInfo.SetJumpState(false);
                    IsetSlopeDirection.SetSlopeDirection(hit.normal);
                    moveDelta = Vector3.ProjectOnPlane(moveDelta, hit.normal).normalized * moveDelta.magnitude;
                    delta += moveDelta.normalized * (hit.distance - 0.01f);
                    moveDelta -= delta;
                    if (moveDelta.magnitude < 0.01f) break;
                }
            }
            else
            {
                delta += moveDelta;
                break;
            }
        }

        return delta;
    }

}

public interface IPhysicsInfo{
    public bool IsGrounded();
    public bool IsJump();
    public void SetGroundState(bool isGrounded);
    public void SetJumpState(bool isJump);
    public int GetCollisionCount();
}


//플레이어의 물리적 움직임과 관련된 모든 데이터들이 담겨있는 곳
public class PlayerKinematicMove : KinematicPhysics, IInputMove, IInputMouse, IPhysicsInfo
{
    //플레이어 물리적 움직임에 필요한 데이터 구조체
    public PhysicsStats _playerPhysicsStats;
    public InputState _playerInputState = new InputState(){
        GravityDirection = Vector2.down,
        isGrounded = false,
    };
    public PlayerComponent _playerComponent;

    //이동 방식 정의
    public Move Move;
    public IOverlapCollision IOverlapCollision;
    public ISeperateCollision ISeperateCollision;
    public ISetMoveVelocity IsetMoveVelocity;
    public ISetSlopeDirection IsetSlopeDirection;

    [SerializeField] private Vector2 moveHorizontal, moveVertical, basehorizontal, baseVertical, baseVector, moveDelta;


    new void Awake() {
        base.Awake();
        PlayerAccelMove accelMove = new PlayerAccelMove(_playerPhysicsStats.acceltime, _playerPhysicsStats.stoptime);
        Move = accelMove;
        IsetMoveVelocity = accelMove;
        IsetSlopeDirection = accelMove;

        PlayerKinematicCollision playerCollision = new PlayerKinematicCollision(_playerComponent.CapsuleCollider2D, IsetSlopeDirection, IsetMoveVelocity, this);
        IOverlapCollision = playerCollision;
        ISeperateCollision = playerCollision;
        _playerInputState.GravityDirection = Vector2.down;
    }

    void FixedUpdate() {
        Vector2 currentPosition = _playerComponent.Rigidbody2D.position;
        VelocityFixedUpdate(ref moveHorizontal, ref moveVertical, ref basehorizontal, ref baseVertical);
        baseVector = basehorizontal + baseVertical;

        moveDelta = IOverlapCollision.OverlapCollision(currentPosition);
        moveDelta += ISeperateCollision.VerticalCollision(currentPosition + moveDelta, moveVertical);
        moveDelta += ISeperateCollision.HorizontalCollision(currentPosition + moveDelta, moveHorizontal);
        moveDelta += baseVector;
       
        _playerComponent.Rigidbody2D.MovePosition(currentPosition + moveDelta);
    }

    private void VelocityFixedUpdate(ref Vector2 moveHorizontal, ref Vector2 moveVertical, ref Vector2 basehorizontal, ref Vector2 baseVertical){
        moveHorizontal = Move.MoveHorizontalFixedUpdate(ref _playerPhysicsStats, ref _playerInputState);
        moveVertical = Move.MoveVerticalFixedUpdate(ref _playerPhysicsStats, ref _playerInputState);
        basehorizontal = Move.MoveBaseHorizontalVelocity();
        baseVertical = Move.MoveBaseVerticalVelocity(); 

    }


    protected override void PlayerComponentInitialize() {
        _playerComponent.CapsuleCollider2D =
        _playerComponent.CapsuleCollider2D == null ? GetComponent<CapsuleCollider2D>() : _playerComponent.CapsuleCollider2D;

        _playerComponent.Rigidbody2D =
        _playerComponent.Rigidbody2D == null ? GetComponent<Rigidbody2D>() : _playerComponent.Rigidbody2D;
    }

    protected override void SetInputAction(){
        PlayerInputManager.SetClickAction(this);
        PlayerInputManager.SetMoveAction(this);
    }

    //움직일 시
    public void OnMove(InputAction.CallbackContext ctx){
        _playerInputState.MoveDirection = ctx.ReadValue<Vector2>();
    }

    //점프할 시
    public void OnJump(InputAction.CallbackContext ctx){
        if(_playerInputState.isGrounded) _playerInputState.isJump = ctx.ReadValue<float>() == 1;
    }

    public void OnClick(InputAction.CallbackContext ctx){
        Debug.Log("Click");
    }

    public bool IsGrounded() => _playerInputState.isGrounded;

    public bool IsJump() => _playerInputState.isJump;

    public void SetGroundState(bool isGrounded) => _playerInputState.isGrounded = isGrounded;

    public void SetJumpState(bool isJump) => _playerInputState.isJump = isJump;

    public int GetCollisionCount() => _playerPhysicsStats.collisionCount;


}
