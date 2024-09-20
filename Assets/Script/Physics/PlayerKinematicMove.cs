using UnityEngine;
using UnityEngine.InputSystem;
/*
    1. 플레이어의 충돌 방식         -> 수평, 수직 충돌 + 트리거 특수 판정
    2. 움직이는 플랫폼의 충돌 방식  -> 수평, 수직 충돌
    3. 특수 오브젝트의 충돌 방식    -> 트리거 또는 수평 수직이 의미없는 충돌
*/


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
    [SerializeField] private Vector2 moveHorizontal, moveVertical, basehorizontal, baseVertical, baseVector, moveDelta;

    protected override void InterfaceInitialize()
    {
        PlayerAccelMove accelMove = new PlayerAccelMove(_playerPhysicsStats.acceltime, _playerPhysicsStats.stoptime);
        Move = accelMove;
        IsetMoveVelocity = accelMove;
        IsetSlopeDirection = accelMove;
        IsetMoveBoolean = accelMove;

        PlayerKinematicCollision playerCollision = new PlayerKinematicCollision(_playerComponent.CapsuleCollider2D, IsetSlopeDirection, IsetMoveVelocity, this);
        IOverlapCollision = playerCollision;
        ISeperateCollision = playerCollision;
        IStepRaycast = playerCollision;
    }

    protected override void SettingInitialize()
    {
        _playerInputState.GravityDirection = Vector2.down;
    }


    void FixedUpdate() {
        Vector2 currentPosition = _playerComponent.Rigidbody2D.position;
        moveDelta = IOverlapCollision.OverlapCollision(currentPosition);

        VelocityFixedUpdate(ref moveHorizontal, ref moveVertical, ref basehorizontal, ref baseVertical);
        moveDelta += ISeperateCollision.VerticalCollision(currentPosition + moveDelta, moveVertical);
        moveDelta += ISeperateCollision.HorizontalCollision(currentPosition + moveDelta, moveHorizontal);
        baseVector = basehorizontal + baseVertical;
        moveDelta += baseVector;

        _playerComponent.Rigidbody2D.MovePosition(currentPosition + moveDelta);
    }

    private void VelocityFixedUpdate(ref Vector2 moveHorizontal, ref Vector2 moveVertical, ref Vector2 basehorizontal, ref Vector2 baseVertical){
        moveHorizontal = Move.MoveHorizontalFixedUpdate(ref _playerPhysicsStats, ref _playerInputState);
        moveVertical = Move.MoveVerticalFixedUpdate(ref _playerPhysicsStats, ref _playerInputState);
        basehorizontal = Move.MoveBaseHorizontalVelocity();
        baseVertical = Move.MoveBaseVerticalVelocity(); 

    }

    protected override void ComponentInitialize() {
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
        if(_playerInputState.isGrounded) {
            _playerInputState.isJump = ctx.ReadValue<float>() == 1;
            IsetMoveBoolean.SetGravityState(true);
        }
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
