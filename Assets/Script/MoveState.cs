public class JumpState : State
{
    private ISetMoveState _IsetMoveState;
    private IGetPlayerData _IplayerData;

    public JumpState(ISetMoveState IsetMoveState, IGetPlayerData _playerData)
    {
        this._IsetMoveState = IsetMoveState;
        this._IplayerData = _playerData;
    }

    
    public override void Start()
    {
        // throw new System.NotImplementedException();
    }


    public override void Execute()
    {
        _IsetMoveState.SetGravityState(true);
        _IsetMoveState.SetGroundState(false);
        _IsetMoveState.SetJumpState(true);
        _IplayerData.GetPlayerComponent().Animator.SetBool("isJump", true);
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

    public override void Start()
    {
        // throw new System.NotImplementedException();
    }


    public override void Execute()
    {

    }

    public override void Exit()
    {

    }
}

public class IdleState : State
{
    private ISetMoveState IsetMoveState;

    public override void Start()
    {
    }
    public override void Execute()
    {
    }
    public override void Exit()
    {

    }
}



