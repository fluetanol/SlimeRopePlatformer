using System;
using UnityEngine;

public interface ISetMoveVelocity{
    public void SetBaseHorizontalVelocity(Vector2 baseHorizontalVelocity);
    public void SetBaseVerticalVelocity(Vector2 baseVerticalVelocity);
    public void SetHorizontalVelocity(Vector2 horizontalVelocity);
    public void SetVerticalVelocity(Vector2 verticalVelocity);
}

public interface ISetDirection{
    public void SetSlopeDirection(Vector2 slopeNormal);
    public void SetJumpDirection(Vector2 jumpDirection);
}

public interface ISetMoveState{
    public void SetGravityState(bool isGravity);
    public void SetAccelState(bool isAccel);
    public void SetGroundState(bool isGround);
    public void SetJumpState(bool isJump);
}

[Serializable]
public abstract class Move {
    protected Vector2 _baseHorizontalVeclocity = Vector2.zero; //V0
    protected Vector2 _baseVerticalVeclocity = Vector2.zero; //V0
    protected Vector2 _direction = Vector2.zero; //Move direction
    protected Vector2 _horizontalVelocity;
    protected Vector2 _verticalVelocity;

    public abstract Vector2 MoveHorizontalFixedUpdate(ref PhysicsStats playerPhysicsStats, ref InputState playerInputState);
    public abstract Vector2 MoveVerticalFixedUpdate(ref PhysicsStats playerPhysicsStats, ref InputState playerInputState);
    public abstract Vector2 MoveBaseHorizontalVelocity();
    public abstract Vector2 MoveBaseVerticalVelocity();
}
