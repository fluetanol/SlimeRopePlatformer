using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachines : MonoBehaviour
{
    private IGetPlayerStateData _playerStateData;

    //Physics interface
    private ISetMoveState IsetMoveState;

    private MoveState _currentState;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update(){
        _currentState.Execute();
    }


    //물리 계산 시작 전 플레이어 상태에 따른 물리적 상태 업데이트
    private void PlayerPhysicsStateUpdate()
    {
        if (_playerStateData.GetPlayerStateMachine()._playerMoveState == EPlayerMoveState.Jump)
        {
            IsetMoveState.SetGravityState(true);
            IsetMoveState.SetGroundState(false);
            IsetMoveState.SetJumpState(true);
        }
        else
        {
            IsetMoveState.SetJumpState(false);
        }

        if (_playerStateData.GetPlayerStateMachine()._playerLandState == EPlayerLandState.Land)
            IsetMoveState.SetGroundState(true);

        else if (_playerStateData.GetPlayerStateMachine()._playerLandState == EPlayerLandState.Air)
            IsetMoveState.SetGroundState(false);
    }
}


public abstract class MoveState{
    public abstract void Execute();
    public abstract void Exit();
}

public class JumpState : MoveState{
    private ISetMoveState IsetMoveState;
    public override void Execute()
    {
        IsetMoveState.SetGravityState(true);
        IsetMoveState.SetGroundState(false);
        IsetMoveState.SetJumpState(true);
    }
    public override void Exit()
    {
        IsetMoveState.SetJumpState(false);
    }
}