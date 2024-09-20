using UnityEngine;

//오버랩 상황에 대한 위치 조정 충돌 처리
public interface IOverlapCollision
{
    public Vector2 OverlapCollision(Vector2 currentPosition);
}

//수평, 수직 충돌을 분리하여 처리
public interface ISeperateCollision
{
    public Vector2 VerticalCollision(Vector2 currentPosition, Vector2 moveDelta);
    public Vector2 HorizontalCollision(Vector2 currentPosition, Vector2 moveDelta);
}

public interface IMixCollision
{
    public Vector2 Collision(Vector2 currentPosition, Vector2 moveDelta);
}

//계단 관련 collision처리
public interface IStepCollision
{
    public Vector2 StepdownRaycast(Vector2 currentPosition, float maxDistance);
    public Vector2 StepupRaycast(Vector2 currentPosition, float horizontalDistance, float verticalDistance);
}