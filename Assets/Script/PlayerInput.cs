
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour, InputInterface{

    public float MaxSpeed;
    public float NormalAccelTime;
    public float StopAccelTime;
    public float JumpHeight;
    public float Gravity;
    public float Resistence;

    private NormalMoveInterface _normalMoveInterface;
    private FallingInterface _fallingInterface;
    
    private PlayerControls _playerControls;
    private Rigidbody2D _playerRigidbody;
    
    private Vector2 _normalMove;
    private Vector2 _gravityMove;
    private Vector2 _finalMove;

    private Vector2 _jumpMove;

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
        //print(_gravityMove);
        
        Vector2 currentPosition = _playerRigidbody.position;
        Vector2 expectPosition = _playerRigidbody.position + _normalMove + _gravityMove + _jumpMove;
        Collision(currentPosition, expectPosition);

        _playerRigidbody.MovePosition(_finalMove);
    }


    private void Collision(Vector2 currentPosition, Vector2 expectPosition){
        RaycastHit2D[] hits = new RaycastHit2D[10];
        int hitCount = 0;
        hitCount = CollisionCast(currentPosition, expectPosition,ref hits);
        Physics2D.queriesStartInColliders = false;

        //print(currentPosition +" "+expectPosition);
        if (hitCount==0){
            RaycastHit2D hit = Physics2D.Raycast(currentPosition, Vector2.down, 1.02f);
            //if(hit.collider == null) 
            _fallingInterface.EnableFalling();
            //else {
               // float angle = Vector2.SignedAngle(Vector2.up, hit.normal);
                //print("hit!: "+ hit.point);
              //  if (angle < 45 && angle > -45) 
              //  SlopeDirection(hit.normal, ref currentPosition, ref expectPosition, ref hit);
           // }
           print("?");
        }
        else {
            print("!");
            CollsionHitCheck(hits,hitCount,ref currentPosition, ref expectPosition);
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

    private void CollsionHitCheck(RaycastHit2D[] hits, int hitCount,ref Vector2 currentPosition, ref Vector2 expectPosition){
        for (int i = 0; i < hitCount; i++){
            RaycastHit2D hit = hits[i];
            Vector2 normal = hit.normal;
            float angle = Vector2.SignedAngle(Vector2.up, normal);
            
            if (angle < 45 && angle > -45){
                SlopeDirection(normal, ref currentPosition, ref expectPosition, ref hit);
               _fallingInterface.DisableFalling();
                break;
            }
        }
    }

    private void SlopeDirection(Vector2 normal, ref Vector2 currentPosition, ref Vector2 expectPosition, ref RaycastHit2D hit){
        Vector2 projectVector = Vector3.ProjectOnPlane(_normalMove, normal);
        Vector2 slopeDirection = projectVector - _normalMove;
        CapsuleCollider2D collider = GetComponent<CapsuleCollider2D>();
        expectPosition.y = hit.point.y + collider.size.y/2;
        //expectPosition -= _gravityMove;
        
        expectPosition += slopeDirection;
        print(expectPosition);
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
        _playerControls.Locomotion.Jump.started += OnJump;

    }

    public void OnMove(InputAction.CallbackContext ctx){
        Vector2 _direction = ctx.ReadValue<Vector2>();
        _normalMoveInterface.SetDirectionVector(_direction);
    }

    public void OnJump(InputAction.CallbackContext ctx){
        print("?");
        _jumpMove = Vector2.up * JumpHeight;
    }

}

