using System.Collections.Generic;
using UnityEngine;

public class ObstacleKinematicMove : KinematicPhysics
{
    [SerializeField] private List<Transform> l_pathTransform;
    [SerializeField] private PhysicsStats _obstaclePhysicsStats;
    [SerializeField] private InputState _obstacleInputState;
    [SerializeField] private PlayerComponent _obstacleComponent;
    
    
    private ObstacleMove _obstacleMove;

    protected override void PlayerComponentInitialize()
    {
        List<Vector2> l_path = new();
        foreach (Transform t in l_pathTransform) l_path.Add(t.position);
        _obstacleMove = new ObstacleMove(l_path);
        _obstacleComponent.Collider2D = GetComponent<Collider2D>();
        _obstacleComponent.Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 currentPosition = _obstacleComponent.Rigidbody2D.position;  
        _obstacleMove.UpdateDirection(currentPosition);
        Vector2 delta = _obstacleMove.MoveHorizontalFixedUpdate(ref _obstaclePhysicsStats, ref _obstacleInputState);
        Vector2 k = Collision(currentPosition, delta);

        _obstacleComponent.Rigidbody2D.MovePosition(currentPosition + delta);
    }

    public Vector2 GetObstacleVelocity(){
        return _obstacleMove.MoveBaseHorizontalVelocity();
    }

}
