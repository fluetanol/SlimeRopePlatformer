
using System;
using UnityEngine;

//enum -> ~Type
//불변 데이터 -> ~Stats
//변하는 데이터 -> ~State


public enum EPlayerColliderType{
    Capsule,
    Box,
    Circle
}

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

//Player Physics Stats And Fixed Value
[Serializable]
public struct PhysicsStats{
    public float HorizontalSpeed;   //수평 속도
    public float acceltime;         //가속 시간
    public float stoptime;          //정지 시간
    public float Gravity;           //중력
    public float JumpForce;         //점프력
    public float AttackForce;       //공격력
    public float FallingClamp;      //낙하 속도 제한
    public int jumpCount;           //점프 횟수
    public int collisionCount;      //collide and slide 충돌 횟수
}


//Player input state And Insertable Value
[Serializable]
public struct InputState{
    public Vector2 PlayerPosition;
    public Vector2 MoveDirection;
    public Vector2 GravityDirection;
    public Vector2 CursorPosition;
    public bool Isjump;
}

[Serializable]
public struct AttackData
{
    public Vector2 attackDirection;
    public Vector2 attackPosition;
    public float attackRange;
    public float attackSpeed;
}


public struct PlayerState
{
    public EPlayerElementalType ElementalType;
    public int Health;
}
