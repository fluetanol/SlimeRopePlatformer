using System;
using UnityEngine;

//Moving by acceleration
[Serializable]
public sealed class PlayerAccelMove : Move, ISetMoveVelocity, ISetDirection, ISetMoveState
{
    public bool isAccelerating = true;
    public bool isGravity = true;
    public bool isGrounded = false;
    public bool isJumping = false;
    public float _normalAccelTime = 1; //Acclereation time
    public float _stopAccelTime = 0.5f;   //Stop acceleration time
    private float accelMagnitde = 1;
    private Vector2 _velocity = Vector2.zero; //Velocity vector
    private Vector2 _jumpdirection = Vector2.up; //Jump direction
    private Vector2 _acceleration = Vector2.zero; //Acceleration
    private Vector2 _jumpVelocity = Vector2.zero; //Jump velocity vector
    private Vector2 _gravityVelocity = Vector2.zero; //Gravity
    private Vector2 _slopeNormal = Vector2.up;  //Land normal vector    

    // Parent Override Method //

    public override Vector2 MoveHorizontalFixedUpdate(ref PhysicsStats playerPhysicsStats, ref InputState playerInputState)
    {
        _stopAccelTime = playerPhysicsStats.stoptime;
        _normalAccelTime = playerPhysicsStats.acceltime;
        float HorizontalSpeed = playerPhysicsStats.HorizontalSpeed;
        Vector2 inputDirection = playerInputState.MoveDirection;

        CalculateAccelVector(in HorizontalSpeed, in inputDirection);
        CalculateHorizontalVelocityVector(in HorizontalSpeed);
        return _horizontalVelocity;
    }
    
    public override Vector2 MoveVerticalFixedUpdate(ref PhysicsStats playerPhysicsStats, ref InputState playerInputState){
        float jumpForce = playerPhysicsStats.JumpForce;
        float gravity = playerPhysicsStats.Gravity;
        float fallClamp = playerPhysicsStats.FallingClamp;
        Vector2 gravityDirection = playerInputState.GravityDirection;

        CalculateJumpVelocity(jumpForce);
        if(isGravity) CalculateVerticalVector(gravity, gravityDirection, 0.1f);
        return _verticalVelocity;
    }

    public override Vector2 MoveBaseHorizontalVelocity() => _baseHorizontalVeclocity;

    public override Vector2 MoveBaseVerticalVelocity() => _baseVerticalVeclocity;


    //수평 가속도 벡터 계산
    private void CalculateAccelVector(in float HorizontalSpeed, in Vector2 inputDirection){
        _direction = Vector3.ProjectOnPlane(inputDirection, _slopeNormal).normalized;
        if (_direction.magnitude == 0) _acceleration = -_horizontalVelocity.normalized * (HorizontalSpeed * Time.fixedDeltaTime / _stopAccelTime);
        else _acceleration = _direction * (HorizontalSpeed * Time.fixedDeltaTime / _normalAccelTime);
    }

    //수평 속도 벡터 계산
    private void CalculateHorizontalVelocityVector(in float HorizontalSpeed){
        _velocity = Vector3.ProjectOnPlane(_velocity, _slopeNormal).normalized * _velocity.magnitude;
        _velocity += _acceleration;

        if (_direction.magnitude == 0 && _velocity.magnitude < _acceleration.magnitude) _velocity = Vector2.zero;
        else _velocity = Vector2.ClampMagnitude(_velocity, HorizontalSpeed);

        _horizontalVelocity = _velocity * Time.fixedDeltaTime;
    }

    //점프 속도 벡터 계산
    private void CalculateJumpVelocity(in float jumpForce){
        
        if (isJumping) _jumpVelocity = _jumpdirection * jumpForce * Time.fixedDeltaTime;
        else _jumpVelocity = Vector2.zero;
    }

    //수직 속도 벡터 계산
    private void CalculateVerticalVector(in float gravity, in Vector2 gravityDirection, in float fallClamp){
        if (isGrounded){
            _gravityVelocity = Vector2.zero;
            accelMagnitde = 1;
        }
        Vector2 GravityAccel = accelMagnitde * gravityDirection * gravity * Time.fixedDeltaTime;
        accelMagnitde += 0.1f;
        _gravityVelocity += GravityAccel * Time.fixedDeltaTime;
        _verticalVelocity = _jumpVelocity + _gravityVelocity;
    }


    // interface implementation //

    public void SetBaseHorizontalVelocity(Vector2 baseHorizontalVelocity) => _baseHorizontalVeclocity = baseHorizontalVelocity;

    public void SetHorizontalVelocity(Vector2 horizontalVelocity) => this._velocity = horizontalVelocity;

    public void SetBaseVerticalVelocity(Vector2 baseVerticalVelocity) => this._baseVerticalVeclocity = baseVerticalVelocity;

    public void SetVerticalVelocity(Vector2 verticalVelocity) => this._verticalVelocity = verticalVelocity;

    public void SetSlopeDirection(Vector2 slopeNormal) => this._slopeNormal = slopeNormal;

    public void SetJumpDirection(Vector2 jumpDirection) => this._jumpdirection = jumpDirection;

    public void SetGravityState(bool isGravity) {
        this.isGravity = isGravity;
        if(!isGravity)  {
            _gravityVelocity = Vector2.zero;
            _verticalVelocity = Vector2.zero;
            accelMagnitde = 1;
            _jumpVelocity = Vector2.zero;
        }
    }
    
    public void SetAccelState(bool isAccel) => this.isAccelerating = isAccel;

    public void SetGroundState(bool isGround) {
        this.isGrounded = isGround;
    } 

    public void SetJumpState(bool isJump) => this.isJumping = isJump;

}