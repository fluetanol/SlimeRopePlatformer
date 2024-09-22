using UnityEngine;

public class SpriteAnimationController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    [SerializeField] private Transform _handleObject;
    [SerializeField] private PlayerKinematicMove playerKinematicMove;

    void Awake(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update(){
        JumpState();
        GroundState();
        flip();
    }


    void JumpState(){
        if (PlayerKinematicMove._playerInputState.isJump) animator.SetBool("isJump", true);
        else animator.SetBool("isJump", false);
    }

    void GroundState(){
        if (PlayerKinematicMove._playerInputState.isGrounded) animator.SetBool("isGrounded", true);
        else animator.SetBool("isGrounded", false);
    }

    void flip(){
        if(PlayerKinematicMove._playerInputState.MoveDirection == Vector2.left) {
            spriteRenderer.flipX = true;
            _handleObject.localPosition = new Vector3(-1f, 0, 0);
            _handleObject.up = Vector2.left;

        }
        else if(PlayerKinematicMove._playerInputState.MoveDirection == Vector2.right){
            spriteRenderer.flipX = false;
            _handleObject.localPosition = new Vector3(1f, 0, 0);
            _handleObject.up = Vector2.right;


        }
    }




}
