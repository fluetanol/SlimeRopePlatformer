using UnityEngine;
using UnityEngine.InputSystem;


public abstract class KinematicPhysics : MonoBehaviour
{
    protected Vector2 _horizontalDirection;
    protected Vector2 _verticalDirection;
    protected Vector2 _jumpDirection;
    
    protected void Awake(){
        PlayerComponentInitialize();
    }

    void OnEnable(){
        SetInputAction();
    }

    protected abstract Vector2 VerticalCollision(Vector2 currentPosition, Vector2 moveDelta);
    protected abstract Vector2 HorizontalCollision(Vector2 currentPosition, Vector2 moveDelta);
    
    protected virtual Vector2 Collision(Vector2 currentPosition, Vector2 moveDelta){return moveDelta;}
    protected virtual void SetInputAction(){}
    protected virtual void PlayerComponentInitialize(){}
}


public class PlayerKinematicMove : KinematicPhysics, IInputMove
{
    public PlayerPhysicsStats _playerPhysicsStats;
    public PlayerInputState _playerInputState = new PlayerInputState(){
        GravityDirection = Vector2.down,
        isGrounded = false,
    };
    public PlayerComponent _playerComponent;
    

    //이동 방식 정의
    public Move _basicMove;

    new void Awake(){
        base.Awake();
        _basicMove = new AccelMove(_playerPhysicsStats.acceltime, _playerPhysicsStats.stoptime);
        _playerInputState.GravityDirection = Vector2.down;
    }

    void FixedUpdate(){
        Vector2 currentPosition = _playerComponent.Rigidbody2D.position;

        Vector2 moveHorizontal = _basicMove.MoveHorizontalFixedUpdate(ref _playerPhysicsStats.HorizontalSpeed , ref _playerInputState.MoveDirection);
        Vector2 moveVertical = _basicMove.MoveVerticalFixedUpdate(ref _playerPhysicsStats.Gravity, ref _playerInputState.GravityDirection, ref _playerInputState.isGrounded);
        Vector2 moveDelta = VerticalCollision(currentPosition, moveVertical);

         moveDelta += HorizontalCollision(currentPosition + moveDelta, moveHorizontal);
        _playerComponent.Rigidbody2D.MovePosition(currentPosition + moveDelta);
    }


    protected override Vector2 VerticalCollision(Vector2 currentPosition, Vector2 moveDelta){
        Vector2 size = _playerComponent.CapsuleCollider2D.size;
        CapsuleDirection2D colliderDirection = _playerComponent.CapsuleCollider2D.direction;
        Physics2D.queriesStartInColliders = false;

        RaycastHit2D hit = Physics2D.CapsuleCast(currentPosition, size, colliderDirection,0, moveDelta, moveDelta.magnitude + 0.01f);
        if(hit.collider != null) {
            print(hit.normal);
            _playerInputState.isGrounded = true;
            _basicMove.SetSlopeDirection(hit.normal);
            moveDelta = moveDelta.normalized * (hit.distance - 0.01f);
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
        int collisionCount = _playerPhysicsStats.collisionCount;
        Physics2D.queriesStartInColliders = false;

        Vector2 delta = Vector2.zero;

        for(int i=0; i<collisionCount; i++){
            RaycastHit2D hit = Physics2D.CapsuleCast(currentPosition, size, colliderDirection, 0, moveDelta, moveDelta.magnitude + 0.01f);

            if(hit.collider != null){
                _playerInputState.isGrounded = true;
                _basicMove.SetSlopeDirection(hit.normal);
                moveDelta = Vector3.ProjectOnPlane(moveDelta, hit.normal).normalized * moveDelta.magnitude;
                delta += moveDelta.normalized * (hit.distance - 0.01f);
                moveDelta -= delta;
                if(moveDelta.magnitude < 0.01f) break;
            }
            else{
                delta += moveDelta;
                break;
            }
        }

        return delta;
    }

    protected override void PlayerComponentInitialize(){
        _playerComponent.CapsuleCollider2D = 
        _playerComponent.CapsuleCollider2D == null ? GetComponent<CapsuleCollider2D>(): _playerComponent.CapsuleCollider2D;

        _playerComponent.Rigidbody2D =
        _playerComponent.Rigidbody2D == null ? GetComponent<Rigidbody2D>() : _playerComponent.Rigidbody2D;
    }

    protected override void SetInputAction(){
        PlayerInputManager.Instance.SetJumpAction(OnJump);
        PlayerInputManager.Instance.SetMoveAction(OnMove);
    }
    
    //움직일 시
    public void OnMove(InputAction.CallbackContext ctx){
         _playerInputState.MoveDirection = ctx.ReadValue<Vector2>();
    }

    //점프할 시
    public void OnJump(InputAction.CallbackContext ctx){
        _playerInputState.Jump = ctx.ReadValue<float>() == 1;
    }
}
