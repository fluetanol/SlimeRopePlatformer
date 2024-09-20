using UnityEngine;

public abstract class KinematicPhysics : MonoBehaviour
{
    public Move Move;
    public IOverlapCollision IOverlapCollision;
    public ISeperateCollision ISeperateCollision;
    public ISetMoveVelocity IsetMoveVelocity;
    public ISetMoveBoolean IsetMoveBoolean;
    public ISetSlopeDirection IsetSlopeDirection;
    public IStepCollision IStepRaycast;
    public IPlatformDirection IplatformDirection;
    
    protected Vector2 _horizontalDirection;
    protected Vector2 _verticalDirection;
    protected Vector2 _jumpDirection;
    
    protected void Awake(){
        ComponentInitialize();
        SettingInitialize();
        InterfaceInitialize();
    
    }

    void OnEnable(){
        SetInputAction();
    }

    protected virtual void SetInputAction(){}
    protected virtual void ComponentInitialize(){}
    protected virtual void SettingInitialize(){}
    protected virtual void InterfaceInitialize(){}

}


