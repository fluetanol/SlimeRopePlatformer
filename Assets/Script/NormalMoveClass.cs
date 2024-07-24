using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;


public class PlayerAccelNormalMove : NormalMoveInterface
{
    private float MaxSpeed;
    private float NormalAccelTime;
    private float StopAccelTime;

    private Vector2 _normalMove;
    private Vector2 _direction;
    private Vector2 _velocity;
    private Vector2 _acceleration;
    private Vector2 _slopeDirection;

    public PlayerAccelNormalMove(float maxSpeed, float NormalAccelTime, float StopAccelTime)
    {
        SetBasicValue(maxSpeed, NormalAccelTime, StopAccelTime);
        _slopeDirection =Vector2.zero;
    }

    public Vector2 NormalMovingVector()
    {
        NormalMoveAccel();
        NormalMoveVelocity();
        NormalMoveVector();
        return _normalMove;
    }

    public void SetDirectionVector(Vector2 direction){
        _direction = direction;
    }

    public void SetBasicValue(float maxSpeed, float NormalAccelTime, float StopAccelTime)
    {
        this.MaxSpeed = maxSpeed;
        this.NormalAccelTime = NormalAccelTime;
        this.StopAccelTime = StopAccelTime;
    }

    private void NormalMoveAccel()
    {
        if (_direction.magnitude == 0) _acceleration = -_velocity.normalized * (MaxSpeed * Time.fixedDeltaTime / StopAccelTime);
        else _acceleration = (_direction) * (MaxSpeed * Time.fixedDeltaTime / NormalAccelTime);
    
    }

    private void NormalMoveVelocity()
    {
        _velocity += _acceleration;
        _velocity = Vector2.ClampMagnitude(_velocity, MaxSpeed);
    }

    private void NormalMoveVector()
    {
        _normalMove = (_velocity + _slopeDirection) * Time.fixedDeltaTime;
    }

    public void SetSlopeDirection(Vector2 slopeDirection)
    {
        _slopeDirection = slopeDirection;
    }

    public Vector2 GetSlopeDirection(){
        return _slopeDirection;
    }

}