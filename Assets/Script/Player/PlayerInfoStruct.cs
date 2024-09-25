
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
    public Collider2D Collider2D;
    public Rigidbody2D Rigidbody2D;
    public SpriteRenderer SpriteRenderer;
    public Animator Animator;
}


public struct PlayerState{
    public EPlayerElementalType ElementalType;
    public int Health;
}


//Player Physics Stats And Fixed Value
[Serializable]
public struct PhysicsStats{
    public float HorizontalSpeed; 
    public float Gravity;
    public float JumpForce;
    public float AttackForce;
    public float FallingClamp;
    public int jumpCount;
    public int collisionCount;
    public float acceltime;
    public float stoptime;
}


//Player input state And Insertable Value
[Serializable]
public struct InputState{
    public Vector2 PlayerPosition;
    public Vector2 MoveDirection;
    public Vector2 GravityDirection;
    public Vector2 CursorPosition;
    public bool IsJump;
}

[Serializable]
public struct AttackData
{
    public Vector2 attackDirection;
    public Vector2 attackPosition;
    public float attackSpeed;
}
