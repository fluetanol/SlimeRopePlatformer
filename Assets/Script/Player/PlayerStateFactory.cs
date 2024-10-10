using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public abstract class StateFactory<T> where T : System.Enum{
    public abstract State CreateState(T state);
}


public class GroundStateFactory : StateFactory<EPlayerLandState>
{
    public Dictionary<EPlayerLandState, State> _groundState = new Dictionary<EPlayerLandState, State>();

    public GroundStateFactory(ISetMoveState IsetMoveState, IGetPlayerData _playerData, ISetDirection IsetDirection)
    {
        _groundState.Add(EPlayerLandState.Land, new LandState(IsetMoveState, _playerData));
        _groundState.Add(EPlayerLandState.Air, new AirState(IsetDirection, IsetMoveState, _playerData));
    }

    public override State CreateState(EPlayerLandState state)
    {
        Debug.Log("?!!: " + state + " " + _groundState[state]);
        return _groundState[state];
    }
}

public class LandState : State
{
    private ISetMoveState IsetMoveState;
    private IGetPlayerData _IplayerData;

    public LandState(ISetMoveState IsetMoveState, IGetPlayerData _playerData)
    {
        this.IsetMoveState = IsetMoveState;
        this._IplayerData = _playerData;
    }

    public override void Execute()
    {
        IsetMoveState.SetGroundState(true);
        _IplayerData.GetPlayerComponent().Animator.SetBool("isGrounded", true);
    }

    public override void ExecuteInFixedUpdate()
    {

    }

    public override void Exit()
    {
        IsetMoveState.SetGroundState(false);
        _IplayerData.GetPlayerComponent().Animator.SetBool("isGrounded", false);
    }
}


public class AirState : State
{
    private ISetDirection IsetDirection;
    private ISetMoveState IsetMoveState;
    private IGetPlayerData _IplayerData;

    public AirState(ISetDirection IsetDirection, ISetMoveState IsetMoveState, IGetPlayerData _playerData)
    {
        this._IplayerData = _playerData;
        this.IsetMoveState = IsetMoveState;
        this.IsetDirection = IsetDirection;
    }
    public override void ExecuteInFixedUpdate()
    {

    }
    public override void Execute()
    {
        IsetDirection.SetSlopeDirection(Vector2.up);
    }
    public override void Exit()
    {
    }
}


public class MoveStateFactory : StateFactory<EPlayerMoveState>
{
    public Dictionary<EPlayerMoveState, State> _moveState = new Dictionary<EPlayerMoveState, State>();

    public MoveStateFactory(ISetMoveState IsetMoveState, IGetPlayerData _playerData)
    {
        _moveState.Add(EPlayerMoveState.Idle, new IdleState());
        _moveState.Add(EPlayerMoveState.Run, new RunState());
        _moveState.Add(EPlayerMoveState.Jump, new JumpState(IsetMoveState, _playerData));
    }

    public override State CreateState(EPlayerMoveState state)
    {
        return _moveState[state];
    }
}


public abstract class State
{
    public abstract void Execute();
    public abstract void ExecuteInFixedUpdate();
    public abstract void Exit();
}


public class JumpState : State
{
    private ISetMoveState _IsetMoveState;
    private IGetPlayerData _IplayerData;

    public JumpState(ISetMoveState IsetMoveState, IGetPlayerData _playerData)
    {
        this._IsetMoveState = IsetMoveState;
        this._IplayerData = _playerData;
    }

    public override void Execute()
    {
        _IsetMoveState.SetGravityState(true);
        _IsetMoveState.SetGroundState(false);
        _IsetMoveState.SetJumpState(true);
        _IplayerData.GetPlayerComponent().Animator.SetBool("isJump", true);
    }
    public override void ExecuteInFixedUpdate()
    {

    }
    public override void Exit()
    {
        _IsetMoveState.SetJumpState(false);
        _IplayerData.GetPlayerComponent().Animator.SetBool("isJump", false);
    }
}

public class RunState : State
{
    private ISetMoveState IsetMoveState;
    public override void Execute()
    {

    }
    public override void ExecuteInFixedUpdate()
    {

    }
    public override void Exit()
    {

    }
}

public class IdleState : State
{
    private ISetMoveState IsetMoveState;
    public override void Execute()
    {

    }
    public override void ExecuteInFixedUpdate()
    {

    }
    public override void Exit()
    {

    }
}