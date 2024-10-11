using System.Collections.Generic;

public abstract class State
{
    public abstract void Start();
    public abstract void Execute();
    public abstract void Exit();
}

public abstract class StateFactory<T> where T : System.Enum{
    public abstract State CreateState(T state);
}


public class GroundStateFactory : StateFactory<EPlayerLandState>
{
    public Dictionary<EPlayerLandState, State> _groundState = new Dictionary<EPlayerLandState, State>();

    public GroundStateFactory(ISetMoveState IsetMoveState, IGetPlayerData _playerData, ISetDirection IsetDirection)
    {
        _groundState.Add(EPlayerLandState.Land, new LandState(IsetMoveState, _playerData));
        _groundState.Add(EPlayerLandState.Air, new AirState(IsetDirection));
    }

    public override State CreateState(EPlayerLandState state){
        return _groundState[state];
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


public class BehaviourStateFactory : StateFactory<EPlayerBehaviourState>
{
    public Dictionary<EPlayerBehaviourState, State> _behaviorState = new Dictionary<EPlayerBehaviourState, State>();

    public BehaviourStateFactory(IAttackAction attackAction, ISetState IsetState, ISetMoveState IsetMoveState, ISetJumpValue IsetJumpValue, IGetPlayerData IplayerData)
    {
        _behaviorState.Add(EPlayerBehaviourState.Normal, new NormalState(IsetMoveState, IsetJumpValue, IplayerData));
        _behaviorState.Add(EPlayerBehaviourState.Attack, new AttackState(attackAction, IsetState, IplayerData));
        _behaviorState.Add(EPlayerBehaviourState.Dead, new DeadState());
    }

    public override State CreateState(EPlayerBehaviourState state)
    {
        return _behaviorState[state];
    }
}



