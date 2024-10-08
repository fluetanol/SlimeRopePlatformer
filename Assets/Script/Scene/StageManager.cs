using System;
using System.Collections.Generic;
using UnityEngine;

//위와 같은 오브젝트 이름을 지을 것
public struct StageObjectName{
    public const string MapLines = "MapLines";  //맵 경계선
    public const string XPoint = "XPoint";      //맵 경계선의 x축
    public const string YPoint = "YPoint";      //맵 경계선의 y축
    public const string StartPoint = "StartPoint";  //게임 시작점
}


[Serializable]
public class StageInfo{
    public GameObject PlayerObject;
    public List<Transform> XBoundaries = new();
    public List<Transform> YBoundaries = new();
    public Vector2 StartPoint;
}

public class StageManager : SingletonMonobehavior<StageManager>
{
    [SerializeField] private List<Transform> _stageTransforms = new();
    [SerializeField] private int stageNum = 0;

    [SerializeField] private StageInfo StageInfo = new StageInfo();
    [SerializeField] private int xClusterNum = 0;
    [SerializeField] private int YclusterNum = 0;

    public int AddXcluster() => xClusterNum++;
    public int AddYcluster() => YclusterNum++;
    public int MinusXcluster() => xClusterNum--;
    public int MinusYcluster() => YclusterNum--;


    new void Awake() {
        base.Awake();
        var (xLines, yLines, startPoint) = FindStageObject();
        MakeStageInfo(xLines, yLines, startPoint);
        MakePlayer();
    }


    private (Transform[], Transform[], Transform) FindStageObject(){
        var mapLines = _stageTransforms[stageNum].Find(StageObjectName.MapLines);
        var xLines = mapLines.Find(StageObjectName.XPoint).GetComponentsInChildren<Transform>();
        var yLines = mapLines.Find(StageObjectName.YPoint).GetComponentsInChildren<Transform>();
        var startPoint = _stageTransforms[stageNum].Find(StageObjectName.StartPoint);
        return (xLines, yLines, startPoint);
    }


    private void MakeStageInfo(Transform[] Xboundaries, Transform[] Yboundaries, Transform startPoint){
        foreach(var k in Xboundaries){
            StageInfo.XBoundaries.Add(k);
        }
        foreach (var k in Yboundaries){
            StageInfo.YBoundaries.Add(k);
        }
        StageInfo.StartPoint = startPoint.position;
    }

    private void MakePlayer(){
        Instantiate(StageInfo.PlayerObject, StageInfo.StartPoint, Quaternion.identity);
    }


    public (Vector2, Vector2) GetCurrentXBoundaries()  {
        return (StageInfo.XBoundaries[xClusterNum].position, StageInfo.XBoundaries[xClusterNum + 1].position);
    }
    public (Vector2, Vector2) GetCurrentYBoundaries(){
        return (StageInfo.YBoundaries[YclusterNum].position, StageInfo.YBoundaries[YclusterNum + 1].position);
    }
    public bool IsEndOfXBoundary {
        get{
            return xClusterNum == StageInfo.XBoundaries.Count - 2;
        }  
        private set{}
    
    }
    public bool IsEndOfYBoundary {
        get{
            return YclusterNum == StageInfo.YBoundaries.Count - 2;
        }    
        private set{}
    }

    public bool IsStartOfXBoundary{
        get{
            return xClusterNum == 0;
        }
        private set{}
    }
    public bool IsStartOfYBoundary{
        get{
            return YclusterNum == 0;
        }
        private set{}
    }


    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        foreach(var k in StageInfo.XBoundaries){
            Gizmos.DrawLine(k.position, k.position + Vector3.up * 100);
        }
        Gizmos.color = Color.blue;
        foreach(var k in StageInfo.YBoundaries){
            Gizmos.DrawLine(k.position, k.position + Vector3.right * 100);
        }
    }

}