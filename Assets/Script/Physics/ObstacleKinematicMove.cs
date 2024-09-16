using System.Collections.Generic;
using UnityEngine;

public class PlatformKinematicMove : KinematicPhysics
{
    [SerializeField] private List<Transform> l_pathTransform;
    [SerializeField] private PhysicsStats _PlatformPhysicsStats;
    [SerializeField] private InputState _PlatformInputState;
    [SerializeField] private PlayerComponent _PlatformComponent;
    
    private PlatformMove _PlatformMove;

    protected override void PlayerComponentInitialize()
    {
        List<Vector2> l_path = new();
        foreach (Transform t in l_pathTransform) l_path.Add(t.position);
        _PlatformMove = new PlatformMove(l_path);
        _PlatformComponent.Collider2D = GetComponent<Collider2D>();
        _PlatformComponent.Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 currentPosition = _PlatformComponent.Rigidbody2D.position;  
        _PlatformMove.UpdateDirection(currentPosition);
        Vector2 delta = _PlatformMove.MoveHorizontalFixedUpdate(ref _PlatformPhysicsStats, ref _PlatformInputState);
        HorizontalCollision(currentPosition, delta);
        _PlatformComponent.Rigidbody2D.MovePosition(currentPosition + delta);
    }


    protected override Vector2 HorizontalCollision(Vector2 currentPosition, Vector2 moveDelta)
    {
        BoxCollider2D collider = _PlatformComponent.Collider2D as BoxCollider2D;
        Vector2 size = collider.size;
        size = new Vector2(size.x * transform.localScale.x + 0.1f, size.y * transform.localScale.y + 0.1f);

        Physics2D.queriesStartInColliders = false;
        RaycastHit2D hit = Physics2D.BoxCast(currentPosition, size, 0, moveDelta.normalized, moveDelta.magnitude, LayerMask.GetMask("Player"));
        
        if(hit.collider != null){
            Vector2 horizontal = new Vector2(moveDelta.x,0);
            Vector2 vertical = new Vector2(0, moveDelta.y);
            hit.transform.GetComponent<PlayerKinematicMove>()._basicMove.SetBaseHorizontalVelocity(horizontal);
            hit.transform.GetComponent<PlayerKinematicMove>()._basicMove.SetBaseVerticalVelocity(vertical);
        }

        return base.HorizontalCollision(currentPosition, moveDelta);
    }



    public Vector2 GetPlatformVelocity(){
        return _PlatformMove.MoveBaseHorizontalVelocity();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        foreach (Transform t in l_pathTransform){
            Gizmos.DrawSphere(t.position, 0.5f);
        }
        for(int i = 0; i < l_pathTransform.Count - 1; i++){
            Gizmos.DrawLine(l_pathTransform[i].position, l_pathTransform[i + 1].position);
        }
    }

}
