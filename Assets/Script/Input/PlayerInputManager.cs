using System;
using UnityEngine.InputSystem;


public class PlayerInputManager : SingletonMonobehavior<PlayerInputManager>{

    public PlayerControls _playerControls;

    new void Awake(){
        base.Awake();
        PlayerInputInitialize();
    }
    
    void OnEnable() => _playerControls.Enable();
    void OnDisable() => _playerControls.Disable();

    //initialize Player Input
    private void PlayerInputInitialize(){
        _playerControls = new();
        _playerControls.Enable();
    }

    public void SetMoveAction(Action<InputAction.CallbackContext> action){
        _playerControls.Locomotion.Move.started += action;
        _playerControls.Locomotion.Move.canceled += action;
    }   

    public void SetJumpAction(Action<InputAction.CallbackContext> action){
        _playerControls.Locomotion.Jump.started += action;
    }
}
