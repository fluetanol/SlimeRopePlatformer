using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

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
    
    public virtual void SetSlopeDirection(Vector2 slopeNormal) => _slopeNormal = slopeNormal;
    public virtual void SetBaseHorizontalVelocity(Vector2 baseHorizontalVelocity)=> _baseHorizontalVeclocity = baseHorizontalVelocity;
    public virtual void SetBaseVerticalVelocity(Vector2 baseVerticalVelocity) => _baseVerticalVeclocity = baseVerticalVelocity;
    public virtual void SetHorizontalVelocity(Vector2 horizontalVelocity)=> _horizontalVelocity = horizontalVelocity;
    public virtual void SetVerticalVelocity(Vector2 verticalVelocity) => _verticalVelocity = verticalVelocity;
    
}

[Serializable]
public sealed class PlatformMove : Move
{
    private List<Vector2> l;
    private int index = 1;

    public PlatformMove(List<Vector2> list){
        l = list;
    }

    public override Vector2 MoveHorizontalFixedUpdate(ref PhysicsStats playerPhysicsStats, ref InputState playerInputState)
    {
        float speed = playerPhysicsStats.HorizontalSpeed;
        _direction = l[index] - l[(l.Count + (index - 1)) % l.Count];
        _horizontalVelocity = _direction.normalized * speed * Time.fixedDeltaTime;
        return _horizontalVelocity;
    
    }

    public override Vector2 MoveVerticalFixedUpdate(ref PhysicsStats playerPhysicsStats, ref InputState playerInputState){
        return Vector2.zero;
    }

    public override Vector2 MoveBaseHorizontalVelocity() => _horizontalVelocity;

    public override Vector2 MoveBaseVerticalVelocity() => Vector2.zero;

    public void UpdateDirection(Vector2 currentPos){
        _direction = l[index] - currentPos;
        if(_direction.magnitude < 0.1f) index = ++index % l.Count;
    }


}



//Moving by acceleration
[Serializable]
public sealed class AccelMove : Move{ 
    public bool isAccelerating = true;
    public float _normalAccelTime = 1; //Acclereation time
    public float _stopAccelTime = 0.5f;   //Stop acceleration time
    public Vector2 _velocity = Vector2.zero; //Velocity vector
    public Vector2 _acceleration = Vector2.zero; //Acceleration


    public Vector2 _jumpVelocity = Vector2.zero; //Jump velocity vector
    public Vector2 _gravityVector = Vector2.zero; //Gravity
    public float accelMagnitde = 1;



    public AccelMove(float normalAccelTime, float stopAccelTime){
        _normalAccelTime = normalAccelTime;
        _stopAccelTime = stopAccelTime;
    }

    public override Vector2 MoveHorizontalFixedUpdate(ref PhysicsStats playerPhysicsStats, ref InputState playerInputState){
        float HorizontalSpeed = playerPhysicsStats.HorizontalSpeed;
        Vector2 inputDirection = playerInputState.MoveDirection;
        CalculateAccelVector(in HorizontalSpeed, in inputDirection);
        CalculateHorizontalVelocityVector(in HorizontalSpeed);
        CalculateHorizontalVelocityVector();
        return _horizontalVelocity;
    }

    public override Vector2 MoveBaseHorizontalVelocity(){
        return _baseHorizontalVeclocity;
    }

    public override Vector2 MoveBaseVerticalVelocity(){
        return _baseVerticalVeclocity;
    }


    public override Vector2 MoveVerticalFixedUpdate(ref PhysicsStats playerPhysicsStats, ref InputState playerInputState){
        float jumpForce = playerPhysicsStats.JumpForce;
        float gravity = playerPhysicsStats.Gravity;
        Vector2 gravityDirection = playerInputState.GravityDirection;
        bool isGrounded = playerInputState.isGrounded;
        bool isJumping = playerInputState.isJump;
       
        CalculateJumpVelocity(jumpForce, isJumping);
        CalculateVerticalVector(gravity, gravityDirection, isGrounded, 0.1f);
        return _verticalVelocity;
    }

    public override void SetSlopeDirection(Vector2 slopeNormal){
        _slopeNormal = slopeNormal;
    }


    private void CalculateAccelVector(in float HorizontalSpeed, in Vector2 inputDirection){
        _direction = Vector3.ProjectOnPlane(inputDirection, _slopeNormal).normalized;
        Debug.Log(_direction);
        //정지
        if (_direction.magnitude == 0) _acceleration = -_horizontalVelocity.normalized * (HorizontalSpeed * Time.fixedDeltaTime / _stopAccelTime);
        //이동
        else _acceleration = _direction * (HorizontalSpeed * Time.fixedDeltaTime / _normalAccelTime);
    }

    private void CalculateHorizontalVelocityVector(in float HorizontalSpeed)
    {
        _velocity = Vector3.ProjectOnPlane(_velocity, _slopeNormal).normalized * _velocity.magnitude;
        _velocity += _acceleration;
       
        if (_direction.magnitude == 0 && _velocity.magnitude < _acceleration.magnitude) {
            _velocity = Vector2.zero;
        }
        else _velocity = Vector2.ClampMagnitude(_velocity, HorizontalSpeed);
    }

    private void CalculateHorizontalVelocityVector(){
        _horizontalVelocity = _velocity * Time.fixedDeltaTime;
    }

    public override void SetHorizontalVelocity(Vector2 horizontalVelocity){
        _velocity = horizontalVelocity;
    }


    private void CalculateJumpVelocity(in float jumpForce, in bool isJumping){
        if(isJumping) _jumpVelocity = Vector2.up * jumpForce * Time.fixedDeltaTime;
        else _jumpVelocity = Vector2.zero;
    }
    
    private void CalculateVerticalVector(in float gravity, in Vector2 gravityDirection, in bool isGrounded, in float fallClamp){
        if(isGrounded) {
            _gravityVector = Vector2.zero;
            accelMagnitde = 1;
        }
        Vector2 GravityAccel = accelMagnitde * gravityDirection * gravity * Time.fixedDeltaTime;
        accelMagnitde += 0.1f;
        _gravityVector  += GravityAccel * Time.fixedDeltaTime;
        _verticalVelocity = _jumpVelocity + _gravityVector;
    }
}