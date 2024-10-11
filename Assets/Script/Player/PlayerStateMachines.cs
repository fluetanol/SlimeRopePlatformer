using UnityEngine;

public interface ISetState{
    void SetMoveState(EPlayerMoveState stateFlag);
    void SetGroundState(EPlayerLandState stateFlag);
    void SetBehaviourState(EPlayerBehaviourState stateFlag);
}

public interface IGetState{
    EPlayerMoveState GetMoveState();
    EPlayerLandState GetGroundState();
    EPlayerBehaviourState GetBehaviourState();
}

[RequireComponent(typeof(PlayerData))]
public class PlayerStateMachines : MonoBehaviour, ISetState, IGetState
{
    // Need Interface
    private IGetPlayerData IplayerData;
    private ISetMoveState IsetMoveState;
    private ISetDirection IsetDirection;
    private ISetJumpValue IsetJumpValue;
    private IAttackAction IattackAction;

    private GroundStateFactory _groundStateFactory;
    private MoveStateFactory _moveStateFactory;
    private BehaviourStateFactory _behaviourStateFactory;


    State _currentMoveState;
    public EPlayerMoveState _eCurrentMoveState;

    State _currentGroundState;
    public EPlayerLandState _eCurrentGroundState;

    State _currentBehaviourState;
    public EPlayerBehaviourState _eCurrentBehaviourState;


    void Awake(){
        InterfaceServiceLocator.Register<ISetState>(this);

        PlayerKinematicMove playerKinematicMove = GetComponent<PlayerKinematicMove>();
        IsetMoveState = playerKinematicMove.IsetMoveState;
        IsetDirection = playerKinematicMove.IsetDirection;
        IsetJumpValue = playerKinematicMove.IsetJumpValue;
        IattackAction = playerKinematicMove.IattackAction;
        IplayerData = GetComponent<PlayerData>();
    }

    void Start()
    {
        _groundStateFactory = new GroundStateFactory(IsetMoveState, IplayerData, IsetDirection);
        _moveStateFactory = new MoveStateFactory(IsetMoveState, IplayerData);
        _behaviourStateFactory = new BehaviourStateFactory(IattackAction, this, IsetMoveState, IsetJumpValue, IplayerData);

        _eCurrentMoveState = EPlayerMoveState.Idle;
        _eCurrentGroundState = EPlayerLandState.Air;
        _eCurrentBehaviourState = EPlayerBehaviourState.Normal;
        
        _currentMoveState = _moveStateFactory.CreateState(_eCurrentMoveState);
        _currentGroundState = _groundStateFactory.CreateState(_eCurrentGroundState);
        _currentBehaviourState = _behaviourStateFactory.CreateState(_eCurrentBehaviourState);
    }

    public void SetMoveState(EPlayerMoveState state){
        if(_eCurrentMoveState == state) {
            _currentMoveState.Execute();
            return;
        }
        _eCurrentMoveState = state;
        _currentMoveState.Exit();
        _currentMoveState = _moveStateFactory.CreateState(state);
        _currentMoveState.Start();
        _currentMoveState.Execute();
    }

    public void SetGroundState(EPlayerLandState state){
        if(_eCurrentGroundState == state) {
            _currentGroundState.Execute();
            return;
        }
        _eCurrentGroundState = state;
        _currentGroundState.Exit();
        _currentGroundState = _groundStateFactory.CreateState(state);
        _currentGroundState.Execute();
    }
    
    public void SetBehaviourState(EPlayerBehaviourState state){
        if(_eCurrentBehaviourState == state) {
            _currentBehaviourState.Execute();
            return;
        }
        _eCurrentBehaviourState = state;
        _currentBehaviourState.Exit();
        _currentBehaviourState = _behaviourStateFactory.CreateState(state);
        _currentBehaviourState.Execute();
    }

    public EPlayerMoveState GetMoveState() => _eCurrentMoveState;
    

    public EPlayerLandState GetGroundState() => _eCurrentGroundState;
    

    public EPlayerBehaviourState GetBehaviourState() => _eCurrentBehaviourState;
    




}

