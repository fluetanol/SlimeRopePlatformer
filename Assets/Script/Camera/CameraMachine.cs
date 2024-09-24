using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


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

    [SerializeField][Range(10, -10)] private float yOffset = 2;
    [SerializeField] [Range(0, 15)] private float cameraOrthoGrahicSize = 10;
    [SerializeField] private float _cameraLerpSpeed = 0.5f;
    //[SerializeField] private float _cameraSpeed = 0.5f;
    private float screenWidth, screenHeight;
    private IGetPlayerStateData _playerStateData;


    void Awake(){
        screenWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;        //알아 두면 좋을 스크린 식
        screenHeight = Camera.main.orthographicSize * 2;
    }

    void Update(){
        if(_cameraState == ECameraState.Follow) {
            CameraMove(_cameraMove);
            CameraPlayerOver();
        }
        Camera.main.orthographicSize = cameraOrthoGrahicSize;
    }


    void CameraMove(in ECameraMove cameraMoveMode){
        Vector2 nextPosition = Vector2.zero;

        if(cameraMoveMode == ECameraMove.Linear)  nextPosition = Target.position + new Vector3(0, yOffset, -10);
        else if(cameraMoveMode == ECameraMove.Smooth) nextPosition = Vector3.Lerp(this.transform.position, Target.transform.position + new Vector3(0, yOffset, -10), _cameraLerpSpeed/10);
        
        if(IsCameraXBoundaries(screenWidth, nextPosition)) nextPosition.x = transform.position.x;
        if(IsCameraYBoundaries(screenHeight, nextPosition)) nextPosition.y = transform.position.y;
        transform.position = new Vector3(nextPosition.x, nextPosition.y, -10);
    }


    void CameraPlayerOver(){
        (Vector2, Vector2) xposArrange = StageManager.Instance.GetCurrentXBoundaries();
        (Vector2, Vector2) yposArrange = StageManager.Instance.GetCurrentYBoundaries();


            if (Target.position.x <= xposArrange.Item1.x && !StageManager.Instance.IsStartOfXBoundary)
            {
                StageManager.Instance.MinusXcluster();
                Vector3 Startpos = transform.position;
                Vector3 Endpos = Startpos - new Vector3(screenWidth, 0, 0);
                StartCoroutine(CameraClusterMoving(0.2f, Startpos, Endpos));
            }
            else if (Target.position.x >= xposArrange.Item2.x && !StageManager.Instance.IsEndOfXBoundary){
                StageManager.Instance.AddXcluster();
                Vector3 Startpos = transform.position;
                Vector3 Endpos = Startpos + new Vector3(screenWidth, 0, 0);
                StartCoroutine(CameraClusterMoving(0.2f, Startpos, Endpos));
            }


            if (Target.position.y <= yposArrange.Item1.y && !StageManager.Instance.IsStartOfYBoundary)
            {
                StageManager.Instance.MinusYcluster();
                Vector3 Startpos = transform.position;
                Vector3 Endpos = Startpos - new Vector3(0, screenHeight, 0);
                StartCoroutine(CameraClusterMoving(0.2f, Startpos, Endpos));
            }
            else if (Target.position.y >= yposArrange.Item2.y && !StageManager.Instance.IsEndOfYBoundary){
                StageManager.Instance.AddYcluster();
                Vector3 Startpos = transform.position;
                Vector3 Endpos = Startpos + new Vector3(0, screenHeight, 0);
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
    private bool IsCameraXBoundaries(float CameraWidth, Vector2 expectPosition){
        (Vector2, Vector2) xposRange = StageManager.Instance.GetCurrentXBoundaries();

        return xposRange.Item1.x + CameraWidth / 2 > expectPosition.x ||
                xposRange.Item2.x - CameraWidth / 2 < expectPosition.x;
    }

    //어떤 바운더리를 넘어가는지 아닌지를 확인 하는 함수. 넘어가면 더 이상 카메라가 움직이지 않게 함 
    private bool IsCameraYBoundaries(float CameraHeight, Vector2 expectPosition)
    {
        (Vector2,Vector2) yposRange = StageManager.Instance.GetCurrentYBoundaries();

        return yposRange.Item1.y + CameraHeight / 2 > expectPosition.y ||
                yposRange.Item2.y - CameraHeight / 2 < expectPosition.y;
    
    }


}
