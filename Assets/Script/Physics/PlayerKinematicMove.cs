using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerKinematicMove
{
    private Vector2 Collision(Vector2 currentPosition, in Vector2 moveHorizontal, in Vector2 moveVertical, in Vector2 baseVector)
    {
        Vector2 moveDelta  = OverlapPlatformCollision(currentPosition);
        moveDelta += VerticalCollision(currentPosition + moveDelta, moveVertical);
        moveDelta += HorizontalCollision(currentPosition + moveDelta, moveHorizontal);
        moveDelta += baseVector;

        return moveDelta;
    }

    public Vector2 OverlapPlatformCollision(Vector2 currentPosition, float allowAngle = 5){
        Physics2D.queriesStartInColliders = false;
        CapsuleCollider2D collider = _playerComponent.CapsuleCollider2D;
        Collider2D[] hit = new Collider2D[1];

        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(LayerMask.GetMask("Platform"));

        Vector2 delta = Vector2.zero;
        int k = collider.OverlapCollider(contactFilter, hit);

        if(k>0){
            Vector2 mydirection  = (currentPosition - (Vector2)hit[0].bounds.center).normalized;

            float platformAngle = Vector2.Angle(Vector2.zero + (Vector2)hit[0].bounds.extents, Vector2.right) + allowAngle;
            float currentAngle = Vector2.Angle(mydirection, Vector2.right);

            if(platformAngle >= currentAngle) {
                delta = Vector2.right * Math.Abs((hit[0].bounds.center+hit[0].bounds.extents).x - currentPosition.x);
            }
            else if(platformAngle <= currentAngle && 180 - platformAngle >= currentAngle){
                _basicMove.SetBaseHorizontalVelocity(hit[0].GetComponent<PlatformKinematicMove>().GetPlatformVelocity());
            }
            else if(180 - platformAngle <= currentAngle){
                delta = Vector2.left * Math.Abs((hit[0].bounds.center - hit[0].bounds.extents).x - currentPosition.x);
            }
        }else{
            _basicMove.SetBaseHorizontalVelocity(Vector2.zero);
        }
        return delta;
    
    }


    protected override Vector2 VerticalCollision(Vector2 currentPosition, Vector2 moveDelta){
        Vector2 size = _playerComponent.CapsuleCollider2D.size;
        CapsuleDirection2D colliderDirection = _playerComponent.CapsuleCollider2D.direction;
        Physics2D.queriesStartInColliders = false;

        RaycastHit2D hit = Physics2D.CapsuleCast(currentPosition, size, colliderDirection, 0, moveDelta, moveDelta.magnitude + 0.01f);
        if (hit.collider != null){
            _basicMove.SetSlopeDirection(hit.normal);
            float angle = Vector2.Angle(hit.normal, Vector2.up);
            if (angle > 90)  _basicMove.SetVerticalVelocity(Vector2.down);
            else{
                _playerInputState.isGrounded = true;
                moveDelta = moveDelta.normalized * (hit.distance - 0.01f);
            }
            _playerInputState.isJump = false;
        }
        else{
            _playerInputState.isGrounded = false;
            _basicMove.SetSlopeDirection(Vector2.up);
        }

        return moveDelta;
    }

    protected override Vector2 HorizontalCollision(Vector2 currentPosition, Vector2 moveDelta){
        Vector2 size = _playerComponent.CapsuleCollider2D.size;
        CapsuleDirection2D colliderDirection = _playerComponent.CapsuleCollider2D.direction;
        Physics2D.queriesStartInColliders = false;

        int collisionCount = _playerPhysicsStats.collisionCount;

        Vector2 delta = Vector2.zero;
        
        for (int i = 0; i < collisionCount; i++)
        {
            RaycastHit2D hit = Physics2D.CapsuleCast(currentPosition, size, colliderDirection, 0, moveDelta, moveDelta.magnitude + 0.01f);

            if (hit.collider != null){
                float angle = Vector2.Angle(hit.normal, Vector2.up);

                if(angle > 50 && angle <= 90){
                    _basicMove.SetHorizontalVelocity(Vector2.zero);
                    delta += moveDelta.normalized * (hit.distance - 0.01f);
                    break;
                }
                
                else if(angle <= 50){
                    _playerInputState.isGrounded = true;
                    _playerInputState.isJump = false;
                    _basicMove.SetSlopeDirection(hit.normal);
                    moveDelta = Vector3.ProjectOnPlane(moveDelta, hit.normal).normalized * moveDelta.magnitude;
                    delta += moveDelta.normalized * (hit.distance - 0.01f);
                    moveDelta -= delta;
                    if (moveDelta.magnitude < 0.01f) break;
                }
            }
            else {
                delta += moveDelta;
                break;
            }
        }

        return delta;
    }
}


public partial class PlayerKinematicMove : KinematicPhysics, IInputMove, IInputMouse
{
    public PhysicsStats _playerPhysicsStats;
    public InputState _playerInputState = new InputState(){
        GravityDirection = Vector2.down,
        isGrounded = false,
    };
    public PlayerComponent _playerComponent;


    //이동 방식 정의
    public AccelMove _basicMove;

    new void Awake() {
        base.Awake();
        _basicMove = new AccelMove(_playerPhysicsStats.acceltime, _playerPhysicsStats.stoptime);
        _playerInputState.GravityDirection = Vector2.down;
    }

    void FixedUpdate() {
        Vector2 currentPosition = _playerComponent.Rigidbody2D.position;
        Vector2 moveHorizontal = _basicMove.MoveHorizontalFixedUpdate(ref _playerPhysicsStats, ref _playerInputState);
        Vector2 moveVertical = _basicMove.MoveVerticalFixedUpdate(ref _playerPhysicsStats, ref _playerInputState);
        Vector2 horizontalVector = _basicMove.MoveBaseHorizontalVelocity();
        Vector2 VerticalVector = _basicMove.MoveBaseVerticalVelocity(); 
        Vector2 baseVector = horizontalVector + VerticalVector;
        Vector2 moveDelta = Collision(currentPosition, moveHorizontal, moveVertical, baseVector);
        _playerComponent.Rigidbody2D.MovePosition(currentPosition + moveDelta);
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

}
