using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerInput : MonoBehaviour, InputInterface{

    public float MaxSpeed;
    public float NormalAccelTime;
    public float StopAccelTime;
    public float Gravity;
    public float Resistence;

    private NormalMoveInterface _normalMoveInterface;
    private FallingInterface _fallingInterface;
    
    private PlayerControls _playerControls;
    private Rigidbody2D _playerRigidbody;
    
    private Vector2 _normalMove;
    private Vector2 _gravityMove;
    private Vector2 _finalMove;

    void Awake(){
        PlayerInputInitialize();
        ComponentInitialize(); 
        InterfaceInitialize();
    }
    void OnEnable() => _playerControls.Enable();
    void OnDisable() => _playerControls.Disable();

    void FixedUpdate(){
         _normalMove = _normalMoveInterface.NormalMovingVector();
        _gravityMove = _fallingInterface.FallingVector();
        
        Vector2 currentPosition = _playerRigidbody.position;
        Vector2 expectPosition = _playerRigidbody.position + _normalMove + _gravityMove;
        Collision(currentPosition, expectPosition);

        _playerRigidbody.MovePosition(_finalMove);
    }


    private void Collision(Vector2 currentPosition, Vector2 expectPosition){
        RaycastHit2D[] hits = new RaycastHit2D[10];
        int hitCount = 0;
        hitCount = CollisionCast(currentPosition, expectPosition,ref hits);
        Physics2D.queriesStartInColliders = false;

        if (hitCount==0){
            RaycastHit2D hit = Physics2D.Raycast(currentPosition, Vector2.down, 1.02f);
            if(hit.collider == null) _fallingInterface.EnableFalling();
            else {
                float angle = Vector2.SignedAngle(Vector2.up, hit.normal);
                if (angle < 45 && angle > -45) SlopeDirection(hit.normal, ref expectPosition);
            }
        }
        else {
            
            CollsionHitCheck(hits,hitCount,ref expectPosition);
        }
        _finalMove = expectPosition;
    }

    private int CollisionCast(Vector2 currentPosition, Vector2 expectPosition, ref RaycastHit2D[] hits){
        CapsuleCollider2D collider = GetComponent<CapsuleCollider2D>();
        Vector2 CastDirection = expectPosition - currentPosition;
        float CastDistance = Vector2.Distance(expectPosition, currentPosition);
        int k = collider.Cast(CastDirection, hits, CastDistance, true);
        return k;
    }

    private void CollsionHitCheck(RaycastHit2D[] hits, int hitCount,ref Vector2 expectPosition){
        for (int i = 0; i < hitCount; i++){
            RaycastHit2D hit = hits[i];
            Vector2 normal = hit.normal;
            float angle = Vector2.SignedAngle(Vector2.up, normal);
            
            if (angle < 45 && angle > -45){
                SlopeDirection(normal, ref expectPosition);
                _fallingInterface.DisableFalling();
                break;
            }
        }
    }

    private void SlopeDirection(Vector2 normal, ref Vector2 expectPosition){
        Vector2 projectVector = Vector3.ProjectOnPlane(_normalMove, normal);
        Vector2 slopeDirection = projectVector - _normalMove;
        expectPosition -= _gravityMove;
        expectPosition += slopeDirection;
        _normalMoveInterface.SetSlopeDirection(projectVector);
    }

    //initialize GetComponent
    private void ComponentInitialize(){
        _playerRigidbody = GetComponent<Rigidbody2D>();
    }

    private void InterfaceInitialize(){
        _normalMoveInterface = new PlayerAccelNormalMove(MaxSpeed, NormalAccelTime, StopAccelTime);
        _fallingInterface = new FallingGravityMoving(Gravity);
    }
    //initialize Player Input
    private void PlayerInputInitialize(){
        _playerControls = new();
        _playerControls.Enable();
        _playerControls.Locomotion.Move.started += OnMove;
        _playerControls.Locomotion.Move.canceled += OnMove;
    }

    public void OnMove(InputAction.CallbackContext ctx){
        Vector2 _direction = ctx.ReadValue<Vector2>();
        _normalMoveInterface.SetDirectionVector(_direction);
    }
}

