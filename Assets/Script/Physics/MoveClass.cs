using System;
using UnityEngine;


public abstract class Move {
    protected Vector2 _baseVeclocity = Vector2.zero; //V0
    protected Vector2 _slopeNormal = Vector2.up;  //Land normal vector    
    protected Vector2 _direction = Vector2.zero; //Move direction
    [SerializeField] protected Vector2 _horizontalVelocity;
    [SerializeField] protected Vector2 _verticalVelocity;

    public abstract Vector2 MoveHorizontalFixedUpdate(ref float HorizontalSpeed, ref Vector2 direction);
    public abstract Vector2 MoveVerticalFixedUpdate(ref float gravity, ref Vector2 gravityDirection, ref bool isGrounded);
    public virtual void SetSlopeDirection(Vector2 slopeNormal){}
}


//Moving by acceleration
[Serializable]
public class AccelMove : Move{ 
    public bool isAccelerating = true;
    public float _normalAccelTime = 1; //Acclereation time
    public float _stopAccelTime = 0.5f;   //Stop acceleration time
    public Vector2 _velocity = Vector2.zero; //Velocity vector
    public Vector2 _acceleration = Vector2.zero; //Acceleration

    public AccelMove(float normalAccelTime, float stopAccelTime){
        _normalAccelTime = normalAccelTime;
        _stopAccelTime = stopAccelTime;
    }

    public override Vector2 MoveHorizontalFixedUpdate(ref float HorizontalSpeed, ref Vector2 inputDirection){
        CalculateAccelVector(in HorizontalSpeed, in inputDirection);
        CalculateVelocityVector(in HorizontalSpeed);
        CalculateHorizontalVelocityVector();
        return _horizontalVelocity;
    }

    public override Vector2 MoveVerticalFixedUpdate(ref float gravity, ref Vector2 gravityDirection, ref bool isGrounded){
        return CalculateGravityVector(gravity, gravityDirection, isGrounded);
    }

    public override void SetSlopeDirection(Vector2 slopeNormal){
        _slopeNormal = slopeNormal;
    }


    private void CalculateAccelVector(in float HorizontalSpeed, in Vector2 inputDirection){
        _direction = Vector3.ProjectOnPlane(inputDirection, _slopeNormal).normalized;

        //정지
        if (_direction.magnitude == 0) _acceleration = -_horizontalVelocity.normalized * (HorizontalSpeed * Time.fixedDeltaTime / _stopAccelTime);
        //이동
        else _acceleration = _direction * (HorizontalSpeed * Time.fixedDeltaTime / _normalAccelTime);
    }

    private void CalculateVelocityVector(in float HorizontalSpeed)
    {
        _velocity = Vector3.ProjectOnPlane(_velocity, _slopeNormal).normalized * _velocity.magnitude;
        _velocity += _acceleration;
       
        if (_direction.magnitude == 0 && _velocity.magnitude < _acceleration.magnitude) {
            _velocity = Vector2.zero;
        }
        else _velocity = Vector2.ClampMagnitude(_velocity, HorizontalSpeed);

    }

    private void CalculateHorizontalVelocityVector(){
        _horizontalVelocity = _baseVeclocity + _velocity * Time.fixedDeltaTime;
    }


    public Vector2 CalculateGravityVector(in float gravity, in Vector2 gravityDirection, in bool isGrounded){
        if(isGrounded) {
            _verticalVelocity = Vector2.zero;
        }
        Vector2 GravityAccel = gravityDirection * gravity * Time.fixedDeltaTime;
        _verticalVelocity += GravityAccel * Time.fixedDeltaTime;

        return _verticalVelocity;
    }
}