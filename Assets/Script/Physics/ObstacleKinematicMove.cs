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

        Vector2 horizontal = _PlatformMove.MoveHorizontalFixedUpdate(ref _PlatformPhysicsStats, ref _PlatformInputState);
        Vector2 vertical = _PlatformMove.MoveVerticalFixedUpdate(ref _PlatformPhysicsStats, ref _PlatformInputState);

        Vector2 delta = VerticalCollision(currentPosition, vertical);
        delta += HorizontalCollision(currentPosition + delta, horizontal);
        _PlatformComponent.Rigidbody2D.MovePosition(currentPosition + delta);
    }


    protected override Vector2 HorizontalCollision(Vector2 currentPosition, Vector2 moveDelta)
    {
        MakeCollisionInfo(out BoxCollider2D collider, out Vector2 size);
        RaycastHit2D hit = Physics2D.BoxCast(currentPosition, size, 0, moveDelta.normalized, moveDelta.magnitude, LayerMask.GetMask("Player"));
        
        if(hit.collider != null){
            Vector2 horizontal = moveDelta;
            hit.transform.GetComponent<PlayerKinematicMove>().IsetMoveVelocity.SetBaseHorizontalVelocity(horizontal);
        }

        return base.HorizontalCollision(currentPosition, moveDelta);
    }

    protected override Vector2 VerticalCollision(Vector2 currentPosition, Vector2 moveDelta)
    {
        MakeCollisionInfo(out BoxCollider2D collider, out Vector2 size);
        RaycastHit2D hit = Physics2D.BoxCast(currentPosition, size, 0, moveDelta.normalized, moveDelta.magnitude, LayerMask.GetMask("Player"));
        if (hit.collider != null)
        {
            Vector2 vertical = moveDelta;
            hit.transform.GetComponent<PlayerKinematicMove>().IsetMoveVelocity.SetBaseVerticalVelocity(vertical);
        }

        return base.VerticalCollision(currentPosition, moveDelta);
    }

    private void MakeCollisionInfo(out BoxCollider2D collider, out Vector2 size){
        collider = _PlatformComponent.Collider2D as BoxCollider2D;
        size = collider.size;
        size = new Vector2(size.x * transform.localScale.x + 0.1f, size.y * transform.localScale.y + 0.1f);
        Physics2D.queriesStartInColliders = false;
    }


    public Vector2 GetPlatformVelocity(){
        return _PlatformMove.MoveBaseHorizontalVelocity();
    }

    public Vector2 GetVerticalPlatformVelocity()
    {
        return _PlatformMove.MoveBaseVerticalVelocity();
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
