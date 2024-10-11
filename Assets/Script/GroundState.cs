using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LandState : State
{
    private ISetMoveState IsetMoveState;
    private IGetPlayerData _IplayerData;

    public LandState(ISetMoveState IsetMoveState, IGetPlayerData _playerData)
    {
        this.IsetMoveState = IsetMoveState;
        this._IplayerData = _playerData;
    }

    public override void Start()
    {
        // throw new System.NotImplementedException();
    }

    public override void Execute()
    {
        IsetMoveState.SetGroundState(true);
        _IplayerData.GetPlayerComponent().Animator.SetBool("isGrounded", true);
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

    public override void Start()
    {
        // throw new System.NotImplementedException();
    }

    public AirState(ISetDirection IsetDirection)
    {
        this.IsetDirection = IsetDirection;
    }

    public override void Execute()
    {
        IsetDirection.SetSlopeDirection(Vector2.up);
    }
    public override void Exit()
    {
    }
}

