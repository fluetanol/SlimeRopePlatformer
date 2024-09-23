using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public enum ECollisionType{
    Air,
    Wall,
    Ground
}

public enum EOverlapType{
    Overlap,
    Seperate
}

public interface ICollisionResult{
    public EOverlapType OverlapCollisionType();
    public ECollisionType VerticalCollisionType();
    public ECollisionType HorizontalCollisionType();

    public ref Collider2D GetOverlapHit();
    public ref Vector2 GetVerticalNormal();
    public ref Vector2 GetHorizontalNormal();
}

public class PlayerKinematicCollision : IOverlapCollision, ISeperateCollision, ICollisionResult // IStepCollision
{
    private CapsuleCollider2D collider;

    private ECollisionType collisionVerticalType;
    private ECollisionType collisionHorizontalType;
    private EOverlapType overlapType;

    private Collider2D overlapHit;
    private Vector2 verticalNormal;
    private Vector2 horizontalNormal;

    //이건 오버랩시 플레이어 위치 조정을 위해 플랫폼 각도를 측정하는 과정에서, 그 각도의 추가 여유를 주기 위한 변수입니다.
    //추가 여유라는 건 플랫폼 지면에 올라가기 위한 추가 여유 각도
    private float allowAngle = 1;
    private bool isOverlap = false;

    public PlayerKinematicCollision(CapsuleCollider2D collider)
    {
        this.collider = collider;
    }

    public Vector2 OverlapCollision(Vector2 currentPosition)
    {
        overlapType = EOverlapType.Seperate;
        overlapHit = null;
        Physics2D.queriesStartInColliders = false;
        Collider2D[] hit = new Collider2D[1];

        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(LayerMask.GetMask("Platform"));

        Vector2 delta = Vector2.zero;
        int k = collider.OverlapCollider(contactFilter, hit);

        if (k > 0)
        {
            Vector2 mydirection = (currentPosition - (Vector2)hit[0].bounds.center).normalized;
            float platformAngle = Vector2.SignedAngle(Vector2.right, (Vector2)hit[0].bounds.extents) + allowAngle;
            float currentAngle = Vector2.SignedAngle(Vector2.right, mydirection);

            //Debug.Log(platformAngle + " " + currentAngle);
            //Debug.DrawRay((Vector2)hit[0].bounds.center, (Vector2)hit[0].bounds.extents, Color.red, 1f);
            
            //우
            if (platformAngle >= currentAngle && -platformAngle <= currentAngle){
                delta = Vector2.right * Mathf.Abs((hit[0].bounds.center + hit[0].bounds.extents).x - (currentPosition.x - collider.size.x / 2));
            }
            //좌
            else if (180 - platformAngle <= currentAngle && -(180-platformAngle) <= currentAngle){
                delta = Vector2.left * Mathf.Abs((hit[0].bounds.center - hit[0].bounds.extents).x - (currentPosition.x + collider.size.x / 2));
            }
            //위
            else if (platformAngle < currentAngle && 180 - platformAngle > currentAngle)
            {   
                isOverlap = true;

                overlapHit = hit[0];
                overlapType = EOverlapType.Overlap;
                //IPlatformVelocity platformVelocity = hit[0].GetComponent<IPlatformVelocity>();
                //SetPlatformVelocity(platformVelocity.GetHorizontalPlatformVelocity(), platformVelocity.GetVerticalPlatformVelocity());

                float distance = ((Vector2)hit[0].bounds.center + 
                (Vector2)hit[0].bounds.extents).y - (currentPosition.y - collider.size.y / 2);
                
                delta = Vector2.up * distance;
            }
            //아래
        }
        else{
            isOverlap = false;
        }
        return delta;
    }


    public Vector2 VerticalCollision(Vector2 currentPosition, Vector2 moveDelta)
    {
        collisionVerticalType = ECollisionType.Air;
        verticalNormal = Vector2.zero;

        Vector2 size = collider.size;
        CapsuleDirection2D colliderDirection = collider.direction;
        Physics2D.queriesStartInColliders = false;

        RaycastHit2D hit = Physics2D.CapsuleCast(currentPosition, size, colliderDirection, 0, moveDelta, moveDelta.magnitude + 0.01f);
        if (hit.collider != null)
        {
            verticalNormal = hit.normal;
            float angle = Vector2.Angle(hit.normal, Vector2.up);
            if (angle > 90) {
                moveDelta = moveDelta.normalized * (hit.distance - 0.01f);
                collisionVerticalType = ECollisionType.Wall;
            }
            else{
                collisionVerticalType = ECollisionType.Ground;
               // iPhysicsInfo.SetGroundState(true);
                moveDelta = moveDelta.normalized * (hit.distance - 0.01f);
                if(hit.collider.TryGetComponent(out PlatformKinematicMove platform) && !isOverlap){
                    moveDelta += platform.GetVerticalPlatformVelocity();
                }
            }
        }
        else{
            collisionVerticalType = ECollisionType.Air;
        }

        return moveDelta;
    }



    public Vector2 HorizontalCollision(Vector2 currentPosition, Vector2 moveDelta)
    {
        collisionHorizontalType = ECollisionType.Air;
        horizontalNormal = Vector2.zero;
        Vector2 size = collider.size;
        CapsuleDirection2D colliderDirection = collider.direction;
        Physics2D.queriesStartInColliders = false;
        Vector2 delta = Vector2.zero;
        int collisionCount = 3;
        //iPhysicsInfo.GetCollisionCount();


        for (int i = 0; i < collisionCount; i++)
        {
            RaycastHit2D hit = Physics2D.CapsuleCast(currentPosition, size, colliderDirection, 0, moveDelta, moveDelta.magnitude + 0.01f);

            if (hit.collider != null)
            {
                float angle = Vector2.Angle(hit.normal, Vector2.up);

                if (angle > 50 && angle <= 90)
                {
                    collisionHorizontalType = ECollisionType.Wall;
                    delta += moveDelta.normalized * (hit.distance - 0.01f);
                    break;
                }

                else if (angle <= 50)
                {
                    collisionHorizontalType = ECollisionType.Ground;
                    horizontalNormal = hit.normal;
                    moveDelta = Vector3.ProjectOnPlane(moveDelta, hit.normal).normalized * moveDelta.magnitude;
                    delta += moveDelta.normalized * (hit.distance - 0.01f);
                    moveDelta -= delta;
                    if (moveDelta.magnitude < 0.01f) break;
                }
            }
            else
            {
                delta += moveDelta;
                break;
            }
        }
        return delta;
    }

/*
    public Vector2 StepdownRaycast(Vector2 currentPosition, float maxDistance)
    {
        Physics2D.queriesStartInColliders = false;
        RaycastHit2D hit = Physics2D.Raycast(currentPosition, Vector2.down, collider.size.y/2 + maxDistance, LayerMask.GetMask("Platform"));

        if(hit.collider != null){
            IPlatformVelocity platformVelocity = hit.collider.GetComponent<IPlatformVelocity>();
            SetPlatformVelocity(platformVelocity.GetHorizontalPlatformVelocity(), platformVelocity.GetVerticalPlatformVelocity());
            float distance = hit.distance - collider.size.y / 2;
            return Vector2.down * distance;
        }else{
            SetPlatformVelocity(Vector2.zero, Vector2.zero);
        }
        return Vector2.zero;
    }
*/

    public Vector2 StepupRaycast(Vector2 currentPosition, float horizontalDistance, float verticalDistance)
    {
        throw new System.NotImplementedException();
    }


    public EOverlapType OverlapCollisionType() => overlapType;
    public ECollisionType VerticalCollisionType() =>collisionVerticalType;
    public ECollisionType HorizontalCollisionType() => collisionHorizontalType;

    public ref Collider2D GetOverlapHit() => ref overlapHit;
    public ref Vector2 GetVerticalNormal() => ref verticalNormal;
    public ref Vector2 GetHorizontalNormal() => ref horizontalNormal;

    

    //private void SetPlatformVelocity(Vector2 horizontalVelocity, Vector2 verticalVelocity){
    //IsetMoveVelocity.SetBaseHorizontalVelocity(horizontalVelocity);
    //IsetMoveVelocity.SetBaseVerticalVelocity(verticalVelocity);
    //}
}
