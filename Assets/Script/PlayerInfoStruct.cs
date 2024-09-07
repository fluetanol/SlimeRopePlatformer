
using System;
using UnityEngine;

public enum EPlayerElementalType{
    Dendro = 0b00000001, //basic type
    Pyro = 0b00000010,
    Hydro = 0b00000100,
    Geo = 0b00001000,
}

[Serializable]
public struct PlayerComponent{
    public CapsuleCollider2D CapsuleCollider2D;
    public Rigidbody2D Rigidbody2D;
}


public struct PlayerState{
    public EPlayerElementalType ElementalType;
    public int Health;
}


//Player Physics Stats And Fixed Value
[Serializable]
public struct PlayerPhysicsStats{
    public float HorizontalSpeed; 
    public float Gravity;
    public float JumpForce;
    public float FallingClamp;
    public int jumpCount;
    public int collisionCount;
}


//Player input state And Insertable Value
[Serializable]
public struct PlayerInputState{
    public Vector2 MoveDirection;
    public Vector2 GravityDirection;
    public bool Jump;
    public bool isGrounded;
}