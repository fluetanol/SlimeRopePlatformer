using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableDatabase", menuName = "ScriptableObject/PlayerPhysicsDatabase")]
public class PlayerDatabase : ScriptableObject
{
    public PhysicsStats playerStats;

    public PlayerComponent playerComponent;

    public void SetPlayerComponent(Transform Object){
        playerComponent.Collider2D = Object.GetComponent<BoxCollider2D>();
        playerComponent.Rigidbody2D = Object.GetComponent<Rigidbody2D>();
    }
}


/*
    데이터 종류 
    1. 변하는 데이터 <- input
    2. 변하지 않는 데이터 <- ~ Stats

*/