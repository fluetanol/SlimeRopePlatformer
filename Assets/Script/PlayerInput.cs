using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour, InputInterface{

    public float MaxSpeed;
    public float NormalAccelTime;
    public float StopAccelTime;
    public float Gravity;
    public float Resistence;

    

    private InterfaceFactory _interfaceFactory;
    private NormalMoveInterface _normalMoveInterface;
    private PlayerControls _playerControls;
    private Rigidbody2D _playerRigidbody;
    
    private Vector2 _normalMove;
    private Vector2 _gravityMove;

    void Awake(){
        PlayerInputInitialize();
        ComponentInitialize(); 
        InterfaceInitialize();
    }
    void OnEnable() => _playerControls.Enable();
    void OnDisable() => _playerControls.Disable();

    void FixedUpdate(){
         _normalMove = _normalMoveInterface.NormalMovingVector();

         Vector2 GravityAccel = Vector2.down * Gravity * Time.fixedDeltaTime;
         _gravityMove += GravityAccel * Time.fixedDeltaTime;
         print(_gravityMove + " " +GravityAccel);

        _playerRigidbody.MovePosition(_playerRigidbody.position + _normalMove + _gravityMove);
    }

    //initialize GetComponent
    private void ComponentInitialize(){
        _playerRigidbody = GetComponent<Rigidbody2D>();
    }

    private void InterfaceInitialize(){
        _normalMoveInterface = new PlayerAccelNormalMove(MaxSpeed, NormalAccelTime, StopAccelTime);
    }
    //initialize Player Input
    private void PlayerInputInitialize(){
        _playerControls = new();
        _playerControls.Enable();
        _playerControls.Locomotion.Move.started += OnMove;
        _playerControls.Locomotion.Move.canceled += OnMove;
    }

    public void OnMove(InputAction.CallbackContext ctx){
        Vector2 _direction = ctx.ReadValue<Vector2>();
        _normalMoveInterface.SetDirectionVector(_direction);
    }
}


