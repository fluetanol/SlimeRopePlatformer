using UnityEngine;

interface ISetState{
    void SetMoveState(EPlayerMoveState stateFlag);
    void SetGroundState(EPlayerLandState stateFlag);
}

interface IGetState{
    EPlayerMoveState GetMoveState();
    EPlayerLandState GetGroundState();
}

[RequireComponent(typeof(PlayerData))]
public class PlayerStateMachines : MonoBehaviour, ISetState, IGetState
{
    private IGetPlayerData IplayerData;
    private ISetMoveState IsetMoveState;
    private ISetDirection IsetDirection;

    private GroundStateFactory _groundStateFactory;
    private MoveStateFactory _moveStateFactory;


    State _currentMoveState;
    public EPlayerMoveState _eCurrentMoveState;

    State _currentGroundState;
    public EPlayerLandState _eCurrentGroundState;

    void Awake(){
        IplayerData = GetComponent<PlayerData>();
        IsetMoveState = GetComponent<PlayerKinematicMove>().IsetMoveState;
        IsetDirection = GetComponent<PlayerKinematicMove>().IsetDirection;
    }

    // Start is called before the first frame update
    void Start()
    {
        _groundStateFactory = new GroundStateFactory(IsetMoveState, IplayerData, IsetDirection);
        _moveStateFactory = new MoveStateFactory(IsetMoveState, IplayerData);

        _eCurrentMoveState = EPlayerMoveState.Idle;
        _eCurrentGroundState = EPlayerLandState.Air;
        
        _currentMoveState = _moveStateFactory.CreateState(_eCurrentMoveState);
        _currentGroundState = _groundStateFactory.CreateState(_eCurrentGroundState);
    }

    public void SetMoveState(EPlayerMoveState state){
        if(_eCurrentMoveState == state) {
            _currentMoveState.Execute();
            return;
        }
        _currentMoveState.Exit();
        _eCurrentMoveState = state;
        _currentMoveState = _moveStateFactory.CreateState(state);
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
    

    public EPlayerMoveState GetMoveState(){
        return _eCurrentMoveState;
    }

    public EPlayerLandState GetGroundState(){
        return _eCurrentGroundState;
    }
}

