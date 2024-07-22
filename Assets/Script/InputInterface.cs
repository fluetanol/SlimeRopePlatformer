using UnityEngine.InputSystem;
using UnityEngine;

public interface InputInterface
{
    public void OnMove(InputAction.CallbackContext ctx);
}

public interface NormalMoveInterface
{
    public Vector2 NormalMovingVector();
    public void SetDirectionVector(Vector2 direction);
}

public interface FallingInterface
{
    public Vector2 FallingVector();
    public void EnableFalling();
    public void DisableFalling();
}

