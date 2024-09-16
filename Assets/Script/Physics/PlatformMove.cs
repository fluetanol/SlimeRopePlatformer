using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class PlatformMove : Move
{
    private List<Vector2> l;
    private int index = 1;

    public PlatformMove(List<Vector2> list)
    {
        l = list;
    }

    public override Vector2 MoveHorizontalFixedUpdate(ref PhysicsStats playerPhysicsStats, ref InputState playerInputState)
    {
        float speed = playerPhysicsStats.HorizontalSpeed;
        _direction = l[index] - l[(l.Count + (index - 1)) % l.Count];
        _horizontalVelocity = _direction.normalized * speed * Time.fixedDeltaTime;
        return _horizontalVelocity;
    }

    public override Vector2 MoveVerticalFixedUpdate(ref PhysicsStats playerPhysicsStats, ref InputState playerInputState)
    {
        return Vector2.zero;
    }
    public override Vector2 MoveBaseHorizontalVelocity() => _horizontalVelocity;

    public override Vector2 MoveBaseVerticalVelocity() => Vector2.zero;

    public void UpdateDirection(Vector2 currentPos)
    {
        _direction = l[index] - currentPos;
        if (_direction.magnitude < 0.1f) index = ++index % l.Count;
    }

}