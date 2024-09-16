using UnityEngine;

public abstract class KinematicPhysics : MonoBehaviour
{
    protected Vector2 _horizontalDirection;
    protected Vector2 _verticalDirection;
    protected Vector2 _jumpDirection;
    
    protected void Awake(){
        PlayerComponentInitialize();
    }

    void OnEnable(){
        SetInputAction();
    }

    protected virtual Vector2 VerticalCollision(Vector2 currentPosition, Vector2 moveDelta){return moveDelta;}
    protected virtual Vector2 HorizontalCollision(Vector2 currentPosition, Vector2 moveDelta){return moveDelta;}
    protected virtual Vector2 Collision(Vector2 currentPosition, Vector2 moveDelta){return moveDelta;}
    protected virtual void SetInputAction(){}
    protected virtual void PlayerComponentInitialize(){}
}


