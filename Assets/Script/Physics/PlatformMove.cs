using System;
using System.Collections.Generic;
using UnityEngine;


public interface IPlatformDirection{
    public Vector2 UpdateDirection(Vector2 currentPos);
}

[Serializable]
public sealed class PlatformMove : Move, IPlatformDirection
{
    private List<Vector2> l; 
    private int index = 1;
    private float speed;

    public PlatformMove(List<Vector2> list){
        l = list;
        _direction = (l[index] - l[(l.Count + (index - 1)) % l.Count]).normalized;
    }



    public override Vector2 MoveHorizontalFixedUpdate(ref PhysicsStats playerPhysicsStats, ref InputState playerInputState){
        speed = playerPhysicsStats.HorizontalSpeed;
        _horizontalVelocity = new Vector2(_direction.x,0) * speed * Time.fixedDeltaTime;
        return _horizontalVelocity;
    }

    public override Vector2 MoveVerticalFixedUpdate(ref PhysicsStats playerPhysicsStats, ref InputState playerInputState){
        speed = playerPhysicsStats.HorizontalSpeed;
        _verticalVelocity = new Vector2(0, _direction.y) * speed * Time.fixedDeltaTime;
        return _verticalVelocity;
    }

    public override Vector2 MoveBaseHorizontalVelocity() => _horizontalVelocity;

    public override Vector2 MoveBaseVerticalVelocity() => _verticalVelocity;

    public Vector2 UpdateDirection(Vector2 currentPos){
        float magnitude = (l[index] - currentPos).magnitude;
        if (magnitude < 0.2f) {
            Vector2 delta = l[index] - currentPos;
            index = ++index % l.Count;
            _direction = (l[index] - l[(l.Count + (index - 1)) % l.Count]).normalized;

            _horizontalVelocity = new Vector2(_direction.x,0) * speed * Time.fixedDeltaTime;
            _verticalVelocity = new Vector2(0, _direction.y) * speed * Time.fixedDeltaTime;
            
            return delta;
        }
        return Vector2.zero;
    
    }

}