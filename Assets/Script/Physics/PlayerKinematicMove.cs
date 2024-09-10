using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerKinematicMove
{
    private Vector2 Collision(Vector2 currentPosition, in Vector2 moveHorizontal, in Vector2 moveVertical, in Vector2 baseVector)
    {
        Vector2 moveDelta = VerticalCollision(currentPosition, moveVertical);
        moveDelta += HorizontalCollision(currentPosition + moveDelta, moveHorizontal);
        moveDelta += baseVector;
        print(baseVector);
        return moveDelta;
    }


    protected override Vector2 VerticalCollision(Vector2 currentPosition, Vector2 moveDelta)
    {
        Vector2 size = _playerComponent.CapsuleCollider2D.size;
        CapsuleDirection2D colliderDirection = _playerComponent.CapsuleCollider2D.direction;
        Physics2D.queriesStartInColliders = false;

        RaycastHit2D hit = Physics2D.CapsuleCast(currentPosition, size, colliderDirection, 0, moveDelta, moveDelta.magnitude + 0.01f);
        if (hit.collider != null)
        {
            _basicMove.SetSlopeDirection(hit.normal);
            if (hit.normal == Vector2.down){
                _basicMove.SetVerticalVelocity(Vector2.zero);
                _playerInputState.isJump = false;
            }
            else {
                _playerInputState.isGrounded = true;
                _playerInputState.isJump = false;
                if(hit.transform.TryGetComponent(out ObstacleKinematicMove obstacle))  _basicMove.SetBaseHorizontalVelocity(obstacle.GetObstacleVelocity());
                moveDelta = moveDelta.normalized * (hit.distance - 0.01f);
            }
        }
        else
        {
            _basicMove.SetBaseHorizontalVelocity(Vector2.zero);
            _playerInputState.isGrounded = false;
            _basicMove.SetSlopeDirection(Vector2.up);
        }

        return moveDelta;
    }

    protected override Vector2 HorizontalCollision(Vector2 currentPosition, Vector2 moveDelta)
    {
        Vector2 size = _playerComponent.CapsuleCollider2D.size;
        CapsuleDirection2D colliderDirection = _playerComponent.CapsuleCollider2D.direction;
        int collisionCount = _playerPhysicsStats.collisionCount;
        Physics2D.queriesStartInColliders = false;

        Vector2 delta = Vector2.zero;
        
        for (int i = 0; i < collisionCount; i++)
        {
            RaycastHit2D hit = Physics2D.CapsuleCast(currentPosition, size, colliderDirection, 0, moveDelta, moveDelta.magnitude + 0.01f);

            if (hit.collider != null)
            {
                if(hit.normal == Vector2.left || hit.normal == Vector2.right){
                    _basicMove.SetSlopeDirection(hit.normal);
                    break;
                }

                _playerInputState.isGrounded = true;
                _playerInputState.isJump = false;
                _basicMove.SetSlopeDirection(hit.normal);
                moveDelta = Vector3.ProjectOnPlane(moveDelta, hit.normal).normalized * moveDelta.magnitude;
                delta += moveDelta.normalized * (hit.distance - 0.01f);
                moveDelta -= delta;
                if (moveDelta.magnitude < 0.01f) break;
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


public partial class PlayerKinematicMove : KinematicPhysics, IInputMove
{
    public PhysicsStats _playerPhysicsStats;
    public InputState _playerInputState = new InputState()
    {
        GravityDirection = Vector2.down,
        isGrounded = false,
    };
    public PlayerComponent _playerComponent;


    //이동 방식 정의
    public Move _basicMove;

    new void Awake()
    {
        base.Awake();
        _basicMove = new AccelMove(_playerPhysicsStats.acceltime, _playerPhysicsStats.stoptime);
        _playerInputState.GravityDirection = Vector2.down;
    }

    void FixedUpdate()
    {
        Vector2 currentPosition = _playerComponent.Rigidbody2D.position;
        Vector2 moveHorizontal = _basicMove.MoveHorizontalFixedUpdate(ref _playerPhysicsStats, ref _playerInputState);
        Vector2 moveVertical = _basicMove.MoveVerticalFixedUpdate(ref _playerPhysicsStats, ref _playerInputState);
        Vector2 baseVector = _basicMove.MoveBaseHorizontalVelocity();

        Vector2 moveDelta = Collision(currentPosition, moveHorizontal, moveVertical, baseVector);
        _playerComponent.Rigidbody2D.MovePosition(currentPosition + moveDelta);
    }


    protected override void PlayerComponentInitialize()
    {
        _playerComponent.CapsuleCollider2D =
        _playerComponent.CapsuleCollider2D == null ? GetComponent<CapsuleCollider2D>() : _playerComponent.CapsuleCollider2D;

        _playerComponent.Rigidbody2D =
        _playerComponent.Rigidbody2D == null ? GetComponent<Rigidbody2D>() : _playerComponent.Rigidbody2D;
    }

    protected override void SetInputAction()
    {
        PlayerInputManager.Instance.SetJumpAction(OnJump);
        PlayerInputManager.Instance.SetMoveAction(OnMove);
    }

    //움직일 시
    public void OnMove(InputAction.CallbackContext ctx)
    {
        _playerInputState.MoveDirection = ctx.ReadValue<Vector2>();
    }

    //점프할 시
    public void OnJump(InputAction.CallbackContext ctx)
    {
        if(_playerInputState.isGrounded) _playerInputState.isJump = ctx.ReadValue<float>() == 1;
    }
}
