using System;
using UnityEngine;


//플레이어 이동 상태
public enum EPlayerMoveState{
    ForceStop = -1,  //ForceStop은 이동을 하면 안되는 상태로 정의
    Idle = 0,
    Run = 1,
    Jump= 2,
}

//플레이어 공중/지상 상태 여부
public enum EPlayerLandState{
    Land,
    Koyote,//공중에 떠도 잠깐 Land처럼 인식하는 상태
    Air
}

public enum EPlayerBehaviourState{
    Normal, //아무것도 안한 일반 상태
    Attack, //우클릭 누를 시
    Dead,   //죽었을 때 (아무것도 못하게 막아야 함)
}

[Serializable]
public class PlayerStateMachine{
    public EPlayerMoveState _playerMoveState;
    public EPlayerLandState _playerLandState;
    public EPlayerBehaviourState _playerBehaviourState;
    public EPlayerElementalType _playerElementalType;

}



//플레이어와 관련된 모든 데이터를 보유하고 있는 클래스 (보유 관리. getter, setter 제공)

public interface IGetPlayerData{
    public ref PhysicsStats GetPlayerPhysicsStats();
    public ref InputState GetPlayerInputState();
    public ref PlayerComponent GetPlayerComponent();
}

public interface IGetPlayerStateData{
    public ref PlayerStateMachine GetPlayerStateMachine();
}


public class PlayerData : MonoBehaviour, IGetPlayerData, IGetPlayerStateData
{
    [SerializeField] private PhysicsStats _playerPhysicsStats;
    [SerializeField] private InputState _playerInputState;
    [SerializeField] private PlayerComponent _playerComponent;
    [SerializeField] private PlayerStateMachine _stateMachine;

    void Awake(){
        SettingInitialize();
        ComponentInitialize();
    }

    private void SettingInitialize()
    {
        _playerInputState = new InputState(){
            GravityDirection = Vector2.down,
        };
        _playerInputState.GravityDirection = Vector2.down;
    }

    private void ComponentInitialize()
    {
        _playerComponent.CapsuleCollider2D =
        _playerComponent.CapsuleCollider2D == null ? GetComponent<CapsuleCollider2D>() : _playerComponent.CapsuleCollider2D;

        _playerComponent.Rigidbody2D =
        _playerComponent.Rigidbody2D == null ? GetComponent<Rigidbody2D>() : _playerComponent.Rigidbody2D;

        _playerComponent.SpriteRenderer =
        _playerComponent.SpriteRenderer == null ? GetComponentInChildren<SpriteRenderer>(true) : _playerComponent.SpriteRenderer;

        _playerComponent.Animator = 
        _playerComponent.Animator == null ? GetComponentInChildren<Animator>(true) : _playerComponent.Animator;
    


    }

    public ref PhysicsStats GetPlayerPhysicsStats() => ref _playerPhysicsStats;

    public ref InputState GetPlayerInputState() => ref _playerInputState;

    public ref PlayerComponent GetPlayerComponent() => ref _playerComponent;

    public ref PlayerStateMachine GetPlayerStateMachine() => ref _stateMachine;

}
