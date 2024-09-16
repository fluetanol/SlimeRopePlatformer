using System;
using System.Collections.Generic;
using UnityEngine;

public interface ISetMoveVelocity{
    public void SetBaseHorizontalVelocity(Vector2 baseHorizontalVelocity);
    public void SetBaseVerticalVelocity(Vector2 baseVerticalVelocity);
    public void SetHorizontalVelocity(Vector2 horizontalVelocity);
    public void SetVerticalVelocity(Vector2 verticalVelocity);
}

public interface ISetSlopeDirection{
    public void SetSlopeDirection(Vector2 slopeNormal);
}

public abstract class Move {
    protected Vector2 _baseHorizontalVeclocity = Vector2.zero; //V0
    protected Vector2 _baseVerticalVeclocity = Vector2.zero; //V0
    protected Vector2 _slopeNormal = Vector2.up;  //Land normal vector    
    protected Vector2 _direction = Vector2.zero; //Move direction
    
    [SerializeField] protected Vector2 _horizontalVelocity;
    [SerializeField] protected Vector2 _verticalVelocity;

    public abstract Vector2 MoveHorizontalFixedUpdate(ref PhysicsStats playerPhysicsStats, ref InputState playerInputState);
    public abstract Vector2 MoveVerticalFixedUpdate(ref PhysicsStats playerPhysicsStats, ref InputState playerInputState);
    public abstract Vector2 MoveBaseHorizontalVelocity();
    public abstract Vector2 MoveBaseVerticalVelocity();
}
