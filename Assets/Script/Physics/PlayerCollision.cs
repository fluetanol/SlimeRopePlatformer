using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public interface IOverlapCollision
{
    public Vector2 OverlapCollision(Vector2 currentPosition);
}

public interface ISeperateCollision
{
    public Vector2 VerticalCollision(Vector2 currentPosition, Vector2 moveDelta);
    public Vector2 HorizontalCollision(Vector2 currentPosition, Vector2 moveDelta);
}

public interface IMixCollision
{
    public Vector2 Collision(Vector2 currentPosition, Vector2 moveDelta);
}

public interface IStepRaycast{
    public Vector2 StepdownRaycast(Vector2 currentPosition, float maxDistance);
    public Vector2 StepupRaycast(Vector2 currentPosition, float horizontalDistance, float verticalDistance);
}


public class PlayerKinematicCollision : IOverlapCollision, ISeperateCollision, IStepRaycast
{
    private CapsuleCollider2D collider;
    private ISetMoveVelocity IsetMoveVelocity;
    private ISetSlopeDirection IsetSlopeDirection;
    private ISetMoveBoolean IsetMoveBoolean;
    private IPhysicsInfo iPhysicsInfo;
    private float allowAngle = 5;


    public PlayerKinematicCollision(CapsuleCollider2D collider, ISetSlopeDirection isetSlopeDirection, ISetMoveVelocity isetMoveVelocity, IPhysicsInfo iphyicsInfo, ISetMoveBoolean iSetMoveBoolean)
    {
        this.collider = collider;
        this.IsetSlopeDirection = isetSlopeDirection;
        this.IsetMoveVelocity = isetMoveVelocity;
        this.iPhysicsInfo = iphyicsInfo;
        this.IsetMoveBoolean = iSetMoveBoolean;
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
            Vector2 mydirection = (currentPosition - (Vector2)hit[0].bounds.center).normalized;

            float platformAngle = Vector2.Angle(Vector2.zero + (Vector2)hit[0].bounds.extents, Vector2.right) + allowAngle;
            float currentAngle = Vector2.Angle(mydirection, Vector2.right);

            if (platformAngle >= currentAngle)
            {
                delta = Vector2.right * Mathf.Abs((hit[0].bounds.center + hit[0].bounds.extents).x - (currentPosition.x - collider.size.x / 2));
            }
            else if (platformAngle < currentAngle && 180 - platformAngle > currentAngle)
            {   
                IsetMoveVelocity.SetBaseHorizontalVelocity(hit[0].GetComponent<PlatformKinematicMove>().GetPlatformVelocity());
                float distance = (hit[0].bounds.center + hit[0].bounds.extents).y - (currentPosition.y - collider.size.y / 2);
                delta = Vector2.up * distance;
            }
            else if (180 - platformAngle <= currentAngle)
            {
                delta = Vector2.left * Mathf.Abs((hit[0].bounds.center - hit[0].bounds.extents).x - (currentPosition.x + collider.size.x / 2));
            }
        }
        else{
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
            IsetSlopeDirection.SetSlopeDirection(hit.normal);
            float angle = Vector2.Angle(hit.normal, Vector2.up);
            if (angle > 90) IsetMoveVelocity.SetVerticalVelocity(Vector2.zero);
            else
            {
                iPhysicsInfo.SetGroundState(true);
                moveDelta = moveDelta.normalized * (hit.distance - 0.01f);
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
        throw new System.NotImplementedException();
    }

    public Vector2 StepupRaycast(Vector2 currentPosition, float horizontalDistance, float verticalDistance)
    {
        throw new System.NotImplementedException();
    }
}
