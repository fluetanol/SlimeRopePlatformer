using UnityEngine;


public class SpriteAnimationController : MonoBehaviour
{
    [SerializeField] PlayerData _playerData;
    private IGetPlayerData _IplayerData;
    private IGetPlayerStateData _IplayerStateData;

    [SerializeField] private Transform _handleObject;

    void Awake(){
        _IplayerData = _playerData;
        _IplayerStateData = _playerData;
    
    }

    void Update(){
        JumpState();
        GroundState();
        flip();
    }

    void JumpState(){
        Animator animator = _playerData.GetPlayerComponent().Animator;
        Animator animator2 = this.GetComponent<Animator>();
        if (_IplayerStateData.GetPlayerStateMachine()._playerMoveState == EPlayerMoveState.Jump) {
            animator.SetBool("isJump", true);
            animator2.SetBool("isJump", true);
        
        }
        else {
            animator.SetBool("isJump", false);
            animator2.SetBool("isJump", false);
        }
    }

    void GroundState(){
        Animator animator = _playerData.GetPlayerComponent().Animator;
        Animator animator2 = this.GetComponent<Animator>();
        if (_IplayerStateData.GetPlayerStateMachine()._playerLandState == EPlayerLandState.Land) {
            animator.SetBool("isGrounded", true);
            animator2.SetBool("isGrounded", true);
        }
        else {
            animator.SetBool("isGrounded", false);
            animator2.SetBool("isGrounded", false);
        }
    }

    void flip(){
        SpriteRenderer spriteRenderer = _playerData.GetPlayerComponent().SpriteRenderer;
        ref InputState playerInputState = ref _playerData.GetPlayerInputState();

        if(playerInputState.MoveDirection == Vector2.left) {
            spriteRenderer.flipX = true;
            _handleObject.localPosition = new Vector3(-1f, 0, 0);
            _handleObject.up = Vector2.left;

        }
        else if(playerInputState.MoveDirection == Vector2.right){
            spriteRenderer.flipX = false;
            _handleObject.localPosition = new Vector3(1f, 0, 0);
            _handleObject.up = Vector2.right;
        }
    }




}
