using System;
using UnityEngine;

//Moving by acceleration
[Serializable]
public sealed class PlayerAccelMove : Move, ISetMoveVelocity, ISetSlopeDirection
{
    public bool isAccelerating = true;
    public float _normalAccelTime = 1; //Acclereation time
    public float _stopAccelTime = 0.5f;   //Stop acceleration time
    private float accelMagnitde = 1;
    private Vector2 _velocity = Vector2.zero; //Velocity vector
    private Vector2 _slopeNormal = Vector2.up;  //Land normal vector    
    private Vector2 _acceleration = Vector2.zero; //Acceleration
    private Vector2 _jumpVelocity = Vector2.zero; //Jump velocity vector
    private Vector2 _gravityVector = Vector2.zero; //Gravity

    public PlayerAccelMove(float normalAccelTime, float stopAccelTime){
        _normalAccelTime = normalAccelTime;
        _stopAccelTime = stopAccelTime;
    }

    public override Vector2 MoveHorizontalFixedUpdate(ref PhysicsStats playerPhysicsStats, ref InputState playerInputState)
    {
        float HorizontalSpeed = playerPhysicsStats.HorizontalSpeed;
        Vector2 inputDirection = playerInputState.MoveDirection;
        CalculateAccelVector(in HorizontalSpeed, in inputDirection);
        CalculateHorizontalVelocityVector(in HorizontalSpeed);
        return _horizontalVelocity;
    }

    public override Vector2 MoveBaseHorizontalVelocity() => _baseHorizontalVeclocity;

    public override Vector2 MoveBaseVerticalVelocity() => _baseVerticalVeclocity;
    

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


    private void CalculateAccelVector(in float HorizontalSpeed, in Vector2 inputDirection){
        _direction = Vector3.ProjectOnPlane(inputDirection, _slopeNormal).normalized;
        Debug.Log(_direction);
        //정지
        if (_direction.magnitude == 0) _acceleration = -_horizontalVelocity.normalized * (HorizontalSpeed * Time.fixedDeltaTime / _stopAccelTime);
        //이동
        else _acceleration = _direction * (HorizontalSpeed * Time.fixedDeltaTime / _normalAccelTime);
    }

    private void CalculateHorizontalVelocityVector(in float HorizontalSpeed){
        _velocity = Vector3.ProjectOnPlane(_velocity, _slopeNormal).normalized * _velocity.magnitude;
        _velocity += _acceleration;

        if (_direction.magnitude == 0 && _velocity.magnitude < _acceleration.magnitude) _velocity = Vector2.zero;
        else _velocity = Vector2.ClampMagnitude(_velocity, HorizontalSpeed);

        _horizontalVelocity = _velocity * Time.fixedDeltaTime;
    }

    private void CalculateJumpVelocity(in float jumpForce, in bool isJumping){
        if (isJumping) _jumpVelocity = Vector2.up * jumpForce * Time.fixedDeltaTime;
        else _jumpVelocity = Vector2.zero;
    }

    private void CalculateVerticalVector(in float gravity, in Vector2 gravityDirection, in bool isGrounded, in float fallClamp){
        if (isGrounded){
            _gravityVector = Vector2.zero;
            accelMagnitde = 1;
        }
        Vector2 GravityAccel = accelMagnitde * gravityDirection * gravity * Time.fixedDeltaTime;
        accelMagnitde += 0.1f;
        _gravityVector += GravityAccel * Time.fixedDeltaTime;
        _verticalVelocity = _jumpVelocity + _gravityVector;
    }


    public void SetBaseHorizontalVelocity(Vector2 baseHorizontalVelocity) => _baseHorizontalVeclocity = baseHorizontalVelocity;

    public void SetHorizontalVelocity(Vector2 horizontalVelocity) => _velocity = horizontalVelocity;

    public void SetBaseVerticalVelocity(Vector2 baseVerticalVelocity) => _baseVerticalVeclocity = baseVerticalVelocity;

    public void SetVerticalVelocity(Vector2 verticalVelocity) => _verticalVelocity = verticalVelocity;

    public void SetSlopeDirection(Vector2 slopeNormal) => _slopeNormal = slopeNormal;

}