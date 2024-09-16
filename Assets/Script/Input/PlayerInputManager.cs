using System;
using UnityEngine.InputSystem;


public class PlayerInputManager : SingletonMonobehavior<PlayerInputManager>{

    public static PlayerControls _playerControls;

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

    public static void SetMoveAction(IInputMove inputMove){
        _playerControls.Locomotion.Move.started += inputMove.OnMove;
        _playerControls.Locomotion.Move.canceled += inputMove.OnMove;
        _playerControls.Locomotion.Jump.started += inputMove.OnJump;
    }   

    public static void SetClickAction(IInputMouse inputMouse){
        _playerControls.Locomotion.Click.started += inputMouse.OnClick;
    
    }
}
