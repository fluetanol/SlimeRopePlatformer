using System;
using System.Collections;
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

    public ECameraState _cameraState = ECameraState.Follow;
    public ECameraMove _cameraMove = ECameraMove.Smooth;
    public Transform Target;

    [SerializeField] private float yOffset = 2;
    [SerializeField] private float _cameraLerpSpeed = 0.5f;
    [SerializeField] private float _cameraSpeed = 0.5f;
    private float screenWidth;

    void Awake(){
        screenWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;

    }

    void Update(){
        if(_cameraState == ECameraState.Follow) CameraMove(_cameraMove);

        CameraPlayerOver();
    }



    void CameraMove(in ECameraMove cameraMoveMode){
        Vector2 nextPosition = Vector2.zero;

        if(cameraMoveMode == ECameraMove.Linear)  nextPosition = Target.position + new Vector3(0, yOffset, -10);
        else if(cameraMoveMode == ECameraMove.Smooth) nextPosition = Vector3.Lerp(this.transform.position, Target.transform.position + new Vector3(0, yOffset, -10), _cameraLerpSpeed/10);
        
        if(IsCameraBoundaries(screenWidth, nextPosition)) nextPosition.x = transform.position.x;
        transform.position = new Vector3(nextPosition.x, nextPosition.y, -10);
    }


    void CameraPlayerOver(){
        Vector2 pos = StageManager.Instance.GetCurentEndBoundaries().position;
        Vector2 pos2 = StageManager.Instance.GetCurrentStartBoundaries().position;

        if (Target.position.x > pos.x){
            StageManager.Instance.Addcluster();
            Vector3 Startpos = transform.position;
            Vector3 Endpos = Startpos + new Vector3(screenWidth, 0, 0);
            StartCoroutine(CameraClusterMoving(0.2f, Startpos, Endpos));
        }
        
        else if(Target.position.x < pos2.x){
            StageManager.Instance.Minuscluster();
            Vector3 Startpos = transform.position;
            Vector3 Endpos = Startpos - new Vector3(screenWidth, 0, 0);
            StartCoroutine(CameraClusterMoving(0.2f, Startpos, Endpos));
        }
    }

    IEnumerator CameraClusterMoving(float duration, Vector3 Startpos, Vector3 Endpos){
        _cameraState = ECameraState.Fixed;
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration){
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(Startpos, Endpos, elapsed / duration);
            yield return null;
        }
        _cameraState = ECameraState.Follow;
        yield return null;
    }




    //어떤 바운더리를 넘어가는지 아닌지를 확인 하는 함수. 넘어가면 더 이상 카메라가 움직이지 않게 함 
    //*stage manager 싱글톤 사용 의존성이 조금 있긴 한데, 특정 함수에만 존재하는 의존성이라 무시해도 된다고 생각함
    //대신 클래스 응집도는 높혔음
    bool IsCameraBoundaries(float CameraWidth, Vector2 expectPosition){
        Vector2 Startpos = StageManager.Instance.GetCurrentStartBoundaries().position;
        Vector2 Endpos = StageManager.Instance.GetCurentEndBoundaries().position;

        return Startpos.x + CameraWidth / 2 > expectPosition.x || 
                Endpos.x - CameraWidth / 2 < expectPosition.x;
    }


    void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Target.transform.position, 1);
    }


}
