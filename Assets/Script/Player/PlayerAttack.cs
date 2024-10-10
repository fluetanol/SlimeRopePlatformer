using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackAction
{
    void Attack();
}
public interface IRopeResult
{
    public bool IsFinish(Vector2 position);
}

public class RopeAction : IAttackAction, IRopeResult
{
    private float _ropeVelcity = 40f;
    private ISetMoveState _IsetMoveState;
    private IGetPlayerData _playerData;
    private ISetJumpValue _IsetJumpValue;

    public RopeAction(ISetJumpValue IsetJumpValue, ISetMoveState IsetMoveState, IGetPlayerData playerData)
    {
        this._IsetJumpValue = IsetJumpValue;
        this._IsetMoveState = IsetMoveState;
        this._playerData = playerData;

    }

    public void Attack()
    {
        Vector2 dir = _playerData.GetAttackData().attackDirection;
        _IsetMoveState.SetGravityState(false);
        _IsetMoveState.SetJumpState(true);
        _IsetMoveState.SetMoveState(false);
        _IsetJumpValue.SetJumpDirection(dir);
    }

    public bool IsFinish(Vector2 position)
    {
        Vector2 attackPosition = _playerData.GetAttackData().attackPosition;
        Vector2 dir = _playerData.GetAttackData().attackDirection;

        if (dir.x > 0 && position.x > attackPosition.x)
            return true;
        else if (dir.x < 0 && position.x < attackPosition.x)
            return true;
        else if (dir.y > 0 && position.y > attackPosition.y)
            return true;
        else if (dir.y < 0 && position.y < attackPosition.y)
            return true;


        return false;
    }
}
