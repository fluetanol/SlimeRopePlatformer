using System.Collections.Generic;
using UnityEngine;

public class PlatformCollision : ISeperateCollision
{
    private BoxCollider2D collider;
    private Vector2 size;

    public PlatformCollision(BoxCollider2D collider, Vector2 size)
    {
        this.collider = collider;
        this.size = size;
    }

    public Vector2 HorizontalCollision(Vector2 currentPosition, Vector2 moveDelta)
    {
        RaycastHit2D hit = Physics2D.BoxCast(currentPosition, size, 0, moveDelta.normalized, moveDelta.magnitude, LayerMask.GetMask("Player"));

        if (hit.collider != null)
        {
            Vector2 horizontal = moveDelta;
            hit.transform.GetComponent<PlayerKinematicMove>().IsetMoveVelocity.SetBaseHorizontalVelocity(horizontal);
        }
        return moveDelta;
    }

    public Vector2 VerticalCollision(Vector2 currentPosition, Vector2 moveDelta)
    {
        RaycastHit2D hit = Physics2D.BoxCast(currentPosition, size, 0, moveDelta.normalized, moveDelta.magnitude, LayerMask.GetMask("Player"));
        if (hit.collider != null)
        {
            Vector2 vertical = moveDelta;
            hit.transform.GetComponent<PlayerKinematicMove>().IsetMoveVelocity.SetBaseVerticalVelocity(vertical);
        }

        return moveDelta;
    }
}



public interface IPlatformVelocity{
    Vector2 GetHorizontalPlatformVelocity();
    Vector2 GetVerticalPlatformVelocity();
}


public class PlatformKinematicMove : KinematicPhysics, IPlatformVelocity
{
    [SerializeField] private List<Transform> l_pathTransform;
    [SerializeField] private PhysicsStats _PlatformPhysicsStats;
    [SerializeField] private InputState _PlatformInputState;
    [SerializeField] private PlayerComponent _PlatformComponent;
    
    public Vector2 horizontal, vertical;
    Vector2 delta = Vector2.zero;

    protected override void ComponentInitialize(){
        _PlatformComponent.Collider2D = GetComponent<BoxCollider2D>();
        _PlatformComponent.Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    protected override void SettingInitialize()
    {
        List<Vector2> l_path = new();
        foreach (Transform t in l_pathTransform) l_path.Add(t.position);
        if (l_path.Count > 0) _PlatformComponent.Rigidbody2D.position = l_path[0];

        PlatformMove platformMove = new PlatformMove(l_path);
        Move = platformMove;
        IplatformDirection = platformMove;

        BoxCollider2D collider = _PlatformComponent.Collider2D as BoxCollider2D;
        Vector2 size = collider.size * transform.localScale;
        ISeperateCollision = new PlatformCollision(collider, size);

    }


    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 currentPosition = _PlatformComponent.Rigidbody2D.position;

        if(delta == Vector2.zero){
           horizontal = Move.MoveHorizontalFixedUpdate(ref _PlatformPhysicsStats, ref _PlatformInputState);
           vertical = Move.MoveVerticalFixedUpdate(ref _PlatformPhysicsStats, ref _PlatformInputState);
           delta += ISeperateCollision.VerticalCollision(currentPosition, vertical);
           delta += ISeperateCollision.HorizontalCollision(currentPosition + delta, horizontal);
        }
        
        _PlatformComponent.Rigidbody2D.MovePosition(currentPosition + delta);
        delta = IplatformDirection.UpdateDirection(_PlatformComponent.Rigidbody2D.position);

        if(delta!=Vector2.zero){
            print("delta : " + delta + " next frame direction change!");
        }
    }

    public Vector2 GetHorizontalPlatformVelocity() => Move.MoveBaseHorizontalVelocity();
    
    public Vector2 GetVerticalPlatformVelocity() => Move.MoveBaseVerticalVelocity();

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
