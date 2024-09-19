using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public sealed class PlatformMove : Move
{
    private List<Vector2> l; 
    private int index = 1;

    public PlatformMove(List<Vector2> list){
        l = list;
        _direction = (l[index] - l[(l.Count + (index - 1)) % l.Count]).normalized;
    }

    public override Vector2 MoveHorizontalFixedUpdate(ref PhysicsStats playerPhysicsStats, ref InputState playerInputState){
        float speed = playerPhysicsStats.HorizontalSpeed;
        _horizontalVelocity = new Vector2(_direction.x,0) * speed * Time.fixedDeltaTime;
        return _horizontalVelocity;
    }

    public override Vector2 MoveVerticalFixedUpdate(ref PhysicsStats playerPhysicsStats, ref InputState playerInputState){
        float speed = playerPhysicsStats.HorizontalSpeed;
        _verticalVelocity = new Vector2(0, _direction.y) * speed * Time.fixedDeltaTime;
        return _verticalVelocity;
    }


    public override Vector2 MoveBaseHorizontalVelocity() => _horizontalVelocity;

    public override Vector2 MoveBaseVerticalVelocity() => _verticalVelocity;

    public void UpdateDirection(Vector2 currentPos){
        float magnitude = (l[index] - currentPos).magnitude;
        if (magnitude < 0.1f) {
            index = ++index % l.Count;
            _direction = (l[index] - l[(l.Count + (index - 1)) % l.Count]).normalized;
        }
    }

}