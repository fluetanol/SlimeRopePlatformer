using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerKinematicCollision : IOverlapCollision, ISeperateCollision, IStepCollision
{
    private CapsuleCollider2D collider;
    private ISetMoveVelocity IsetMoveVelocity;
    private ISetSlopeDirection IsetSlopeDirection;
    private IPhysicsInfo iPhysicsInfo;
    private float allowAngle = 5;
    private bool isOverlap = false;

    public PlayerKinematicCollision(CapsuleCollider2D collider, ISetSlopeDirection isetSlopeDirection, ISetMoveVelocity isetMoveVelocity, IPhysicsInfo iphyicsInfo)
    {
        this.collider = collider;
        this.IsetSlopeDirection = isetSlopeDirection;
        this.IsetMoveVelocity = isetMoveVelocity;
        this.iPhysicsInfo = iphyicsInfo;
    }

    public Vector2 OverlapCollision(Vector2 currentPosition)
    {
        Physics2D.queriesStartInColliders = false;
        Collider2D[] hit = new Collider2D[1];

        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(LayerMask.GetMask("Platform"));

        Vector2 delta = Vector2.zero;
        int k = collider.OverlapCollider(contactFilter, hit);

        if (k > 0)
        {
            Debug.Log("collision?");
            Vector2 mydirection = (currentPosition - (Vector2)hit[0].bounds.center).normalized;

            float platformAngle = Vector2.Angle(Vector2.zero + (Vector2)hit[0].bounds.extents, Vector2.right) + allowAngle;
            float currentAngle = Vector2.Angle(mydirection, Vector2.right);

            if (platformAngle >= currentAngle){
                delta = Vector2.right * Mathf.Abs((hit[0].bounds.center + hit[0].bounds.extents).x - (currentPosition.x - collider.size.x / 2));
            }
            else if (180 - platformAngle <= currentAngle){
                delta = Vector2.left * Mathf.Abs((hit[0].bounds.center - hit[0].bounds.extents).x - (currentPosition.x + collider.size.x / 2));
            }
            else if (platformAngle < currentAngle && 180 - platformAngle > currentAngle)
            {   
                isOverlap = true;
                IsetMoveVelocity.SetBaseVerticalVelocity(hit[0].GetComponent<PlatformKinematicMove>().GetVerticalPlatformVelocity());
                IsetMoveVelocity.SetBaseHorizontalVelocity(hit[0].GetComponent<PlatformKinematicMove>().GetPlatformVelocity());
                float distance = ((Vector2)hit[0].bounds.center + (Vector2)hit[0].bounds.extents).y - (currentPosition.y - collider.size.y / 2);
                delta = Vector2.up * distance;
            }

        }
        else{
            isOverlap = false;
            IsetMoveVelocity.SetBaseHorizontalVelocity(Vector2.zero);
            IsetMoveVelocity.SetBaseVerticalVelocity(Vector2.zero);
        }
        return delta;
    }

    public Vector2 VerticalCollision(Vector2 currentPosition, Vector2 moveDelta)
    {
        Vector2 size = collider.size;
        CapsuleDirection2D colliderDirection = collider.direction;
        Physics2D.queriesStartInColliders = false;

        RaycastHit2D hit = Physics2D.CapsuleCast(currentPosition, size, colliderDirection, 0, moveDelta, moveDelta.magnitude + 0.01f);
        if (hit.collider != null)
        {
            Debug.Log("!!!");
            IsetSlopeDirection.SetSlopeDirection(hit.normal);
            float angle = Vector2.Angle(hit.normal, Vector2.up);
            if (angle > 90) IsetMoveVelocity.SetVerticalVelocity(Vector2.zero);
            else{
                iPhysicsInfo.SetGroundState(true);
                moveDelta = moveDelta.normalized * (hit.distance - 0.01f);
                if(hit.collider.TryGetComponent(out PlatformKinematicMove platform) && !isOverlap){
                    moveDelta += platform.GetVerticalPlatformVelocity();
                }
            }
            iPhysicsInfo.SetJumpState(false);
        }
        else
        {
            iPhysicsInfo.SetGroundState(false);
            IsetSlopeDirection.SetSlopeDirection(Vector2.up);
        }

        return moveDelta;
    }

    public Vector2 HorizontalCollision(Vector2 currentPosition, Vector2 moveDelta)
    {
        Vector2 size = collider.size;
        CapsuleDirection2D colliderDirection = collider.direction;
        Physics2D.queriesStartInColliders = false;
        Vector2 delta = Vector2.zero;
        int collisionCount = iPhysicsInfo.GetCollisionCount();


        for (int i = 0; i < collisionCount; i++)
        {
            RaycastHit2D hit = Physics2D.CapsuleCast(currentPosition, size, colliderDirection, 0, moveDelta, moveDelta.magnitude + 0.01f);

            if (hit.collider != null)
            {
                float angle = Vector2.Angle(hit.normal, Vector2.up);

                if (angle > 50 && angle <= 90)
                {
                    IsetMoveVelocity.SetHorizontalVelocity(Vector2.zero);
                    delta += moveDelta.normalized * (hit.distance - 0.01f);
                    break;
                }

                else if (angle <= 50)
                {
                    iPhysicsInfo.SetGroundState(true);
                    iPhysicsInfo.SetJumpState(false);
                    IsetSlopeDirection.SetSlopeDirection(hit.normal);
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

    public Vector2 StepdownRaycast(Vector2 currentPosition, float maxDistance)
    {
        Physics2D.queriesStartInColliders = false;
        RaycastHit2D hit = Physics2D.Raycast(currentPosition, Vector2.down, collider.size.y/2 + maxDistance, LayerMask.GetMask("Platform"));

        if(hit.collider != null){
            float distance = hit.distance - collider.size.y / 2;
            IsetMoveVelocity.SetBaseHorizontalVelocity(hit.collider.GetComponent<PlatformKinematicMove>().GetPlatformVelocity());
            IsetMoveVelocity.SetBaseVerticalVelocity(hit.collider.GetComponent<PlatformKinematicMove>().GetVerticalPlatformVelocity());
            return Vector2.down * distance;
        }else{
            IsetMoveVelocity.SetBaseHorizontalVelocity(Vector2.zero);
            IsetMoveVelocity.SetBaseVerticalVelocity(Vector2.zero);
        }
        return Vector2.zero;
    }

    public Vector2 StepupRaycast(Vector2 currentPosition, float horizontalDistance, float verticalDistance)
    {
        throw new System.NotImplementedException();
    }
}
