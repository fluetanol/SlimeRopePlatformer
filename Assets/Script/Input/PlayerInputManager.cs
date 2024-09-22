using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : SingletonMonobehavior<PlayerInputManager>, IInputMove, IInputMouse{

    public static PlayerControls _playerControls;
    private IGetPlayerData _playerData;
    private IGetPlayerStateData _playerStateData;

    new void Awake(){
        base.Awake();
        PlayerInputInitialize();
        ComponentInitialize();
    }

    void OnEnable() => _playerControls.Enable();
    void OnDisable() => _playerControls.Disable();

    //initialize Player Input
    private void PlayerInputInitialize(){
        _playerControls = new();
        _playerControls.Enable();
    }

    private void ComponentInitialize(){
        _playerData = GetComponent<PlayerData>();
        _playerStateData = GetComponent<PlayerData>();
        SetClickAction(this);
        SetMoveAction(this);
    }


    public static void SetMoveAction(IInputMove inputMove){
        _playerControls.Locomotion.Move.started += inputMove.OnMove;
        _playerControls.Locomotion.Move.canceled += inputMove.OnMove;
        _playerControls.Locomotion.Jump.started += inputMove.OnJump;
    }   

    public static void SetClickAction(IInputMouse inputMouse){
        _playerControls.Locomotion.Click.started += inputMouse.OnClick;
    }

    //움직일 시
    public void OnMove(InputAction.CallbackContext ctx)
    {
        print("OnMove");
        PlayerStateMachine stateMachine = _playerStateData.GetPlayerStateMachine();
        _playerData.GetPlayerInputState().MoveDirection = ctx.ReadValue<Vector2>();
        if (stateMachine._playerMoveState != EPlayerMoveState.Jump)
        {
            stateMachine._playerMoveState = EPlayerMoveState.Run;
        }
    }

    //점프할 시
    public void OnJump(InputAction.CallbackContext ctx)
    {
        PlayerStateMachine stateMachine = _playerStateData.GetPlayerStateMachine();
        if (stateMachine._playerLandState == EPlayerLandState.Land)
        {
            if (ctx.ReadValue<float>() == 1)
            {
                stateMachine._playerMoveState = EPlayerMoveState.Jump;
            }
        }
    }

    public void OnClick(InputAction.CallbackContext ctx)
    {
        print("OnClick");
    }
}
