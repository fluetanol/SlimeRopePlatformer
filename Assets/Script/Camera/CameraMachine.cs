using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

[ExecuteInEditMode]
public class CameraMachine : MonoBehaviour
{
    public enum ECameraState{
        Follow,
        Fixed,
    }
    public enum ECameraMove
    {
        Linear,
        Smooth,
    }

    public List<Transform> CameraStartBoundaries = new();
    public List<Transform> CameraEndBoundaries = new();

    public ECameraState _cameraState = ECameraState.Follow;
    public ECameraMove _cameraMove = ECameraMove.Smooth;
    public GameObject Target;
    public GameObject EndPoints;
    public float yOffset = 1;
    public float CameraLerpSpeed = 0.5f;
    public int clusterNum = 0;

    // Update is called once per frame
    void Update()
    {
        if(_cameraState == ECameraState.Follow) CameraMove();
    }

    void CameraMove(){
        Vector2 nextPosition = Vector2.zero;
        if(_cameraMove == ECameraMove.Linear) nextPosition = Target.transform.position + new Vector3(0, yOffset, -10);
        else if(_cameraMove == ECameraMove.Smooth) nextPosition = Vector3.Lerp(this.transform.position, Target.transform.position + new Vector3(0, yOffset, -10), CameraLerpSpeed);
        
        float Width = Camera.main.orthographicSize * 2 * Camera.main.aspect;

        if(CameraStartBoundaries[clusterNum].position.x + Width / 2 > nextPosition.x){
            nextPosition.x = transform.position.x;
        }
        else if(CameraEndBoundaries[clusterNum].position.x - Width / 2 < nextPosition.x){
            nextPosition.x = transform.position.x;
        }

        transform.position = new Vector3(nextPosition.x, nextPosition.y, -10);
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Target.transform.position, 1);
    }


}
