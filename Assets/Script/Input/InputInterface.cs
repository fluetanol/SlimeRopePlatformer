using UnityEngine.InputSystem;
using UnityEngine;

public interface IInputMove
{
    public void OnMove(InputAction.CallbackContext ctx);
    public void OnJump(InputAction.CallbackContext ctx);
}

public interface IInputMouse{
    public void OnClick(InputAction.CallbackContext ctx);
}