using UnityEngine;


public class SpriteAnimationController : MonoBehaviour
{
    private IGetPlayerData _playerData;
    private IGetPlayerStateData _playerStateData;

    [SerializeField] private Transform _handleObject;

    void Awake(){
        _playerData = PlayerData.IPlayerData;
        _playerStateData = PlayerData.IPlayerStateData;
    
    }

    void Update(){
        JumpState();
        GroundState();
        flip();
    }

    void JumpState(){
        Animator animator = _playerData.GetPlayerComponent().Animator;
        if (_playerStateData.GetPlayerStateMachine()._playerMoveState == EPlayerMoveState.Jump) animator.SetBool("IsJump", true);
        else animator.SetBool("IsJump", false);
    }

    void GroundState(){
        Animator animator = _playerData.GetPlayerComponent().Animator;
        if (_playerStateData.GetPlayerStateMachine()._playerLandState == EPlayerLandState.Land) animator.SetBool("IsGrounded", true);
        else animator.SetBool("IsGrounded", false);
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
