
using UnityEngine;


public class FallingGravityMoving : FallingInterface
{
    private bool _isFall = true;
    private float _gravity;
    private Vector2 _gravityMove;

    public FallingGravityMoving(float gravity) => _gravity = gravity;

    public void EnableFalling()
    {
        _isFall = true;
    }
    public void DisableFalling()
    {
        _gravityMove = Vector2.zero;
        FallingVector();
    }

    public Vector2 FallingVector()
    {
        Vector2 GravityAccel = Vector2.down * _gravity * Time.fixedDeltaTime;
        _gravityMove += GravityAccel * Time.fixedDeltaTime;
        return _gravityMove;
    }
}

public class FallingResistenceMoving : FallingInterface
{
    public void DisableFalling()
    {
        throw new System.NotImplementedException();
    }

    public void EnableFalling()
    {
        throw new System.NotImplementedException();
    }

    public Vector2 FallingVector()
    {
        return Vector2.down;
    }
}


public class FallingClampMoving : FallingInterface
{
    public void DisableFalling()
    {
        throw new System.NotImplementedException();
    }

    public void EnableFalling()
    {
        throw new System.NotImplementedException();
    }

    public Vector2 FallingVector()
    {
        /*
        Vector2 resistVelocity = Vector2.down * Resistence;
        _gravityMove += resistVelocity * (float)Math.Tanh(Gravity * Time.fixedDeltaTime / Resistence);
        print(_gravityMove); */

        return Vector2.down;
    }
}

