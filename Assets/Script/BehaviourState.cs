using UnityEngine;


public class AttackState : State{
    private IAttackAction IAttackAction;
    private IGetPlayerData IplayerData;
    private ISetState IsetState;

    public AttackState(IAttackAction IAttackAction, ISetState IsetState, IGetPlayerData IplayerData)
    {
        this.IAttackAction = IAttackAction;
        this.IsetState = IsetState;
        this.IplayerData = IplayerData;
    }
    
    public override void Start()
    {
        // throw new System.NotImplementedException();
    }

    public override void Execute()
    {
        IAttackAction.Attack();
        IsetState.SetMoveState(EPlayerMoveState.Jump);
        IplayerData.GetPlayerPhysicsStats().JumpForce = 30;
    }

    public override void Exit()
    {

    }
}


public class NormalState :State{
    ISetMoveState IsetMoveState;
    ISetJumpValue IsetJumpValue;
    IGetPlayerData IplayerData;


    public NormalState(ISetMoveState IsetMoveState, ISetJumpValue IsetJumpValue, IGetPlayerData IplayerData)
    {
        this.IsetMoveState = IsetMoveState;
        this.IplayerData = IplayerData;
        this.IsetJumpValue = IsetJumpValue;
    }

    public override void Start()
    {
       // throw new System.NotImplementedException();
    }

    public override void Execute()
    {
        IplayerData.GetPlayerPhysicsStats().JumpForce = 9.8f;
        if (IplayerData.GetPlayerInputState().MoveDirection != Vector2.zero)
            IsetJumpValue.SetJumpDirection(Vector2.up);
        IsetMoveState.SetGravityState(true);
        IsetMoveState.SetMoveState(true);
    }

    public override void Exit()
    {
        
    }
}

public class DeadState :State{



    public DeadState()
    {

    }

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