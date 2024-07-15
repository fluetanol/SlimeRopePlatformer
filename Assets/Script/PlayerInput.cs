using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInput : MonoBehaviour
{
    public float _speed;
    public float _accelTime;

    private PlayerControls _playerControls;
    private Rigidbody2D _playerRigidbody;
    private Vector2 _move;
    private Vector2 _direction;

    void Awake(){
        PlayerInputInitialize();
        ComponentInitialize();   
    }
    void OnEnable() => _playerControls.Enable();
    void OnDisable() => _playerControls.Disable();

    void FixedUpdate(){
        if(Time.fixedTime % 2 == 0){  print(_playerRigidbody.position); }


        _move = _direction * _speed * Time.fixedDeltaTime;
        _playerRigidbody.MovePosition(_playerRigidbody.position + _move);
        
    }


    //initialize GetComponent
    private void ComponentInitialize(){
        _playerRigidbody = GetComponent<Rigidbody2D>();
    }

    //initialize Player Input
    private void PlayerInputInitialize(){
        _playerControls = new();
        _playerControls.Enable();
        _playerControls.Locomotion.Move.started += OnMove;
        _playerControls.Locomotion.Move.canceled += OnMove;
    }

    void OnMove(InputAction.CallbackContext ctx){
        _direction = ctx.ReadValue<Vector2>();
    }
}
