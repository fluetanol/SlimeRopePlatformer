using System.Collections.Generic;
using UnityEngine;


public class StageManager : SingletonMonobehavior<StageManager>
{
    [SerializeField] private List<Transform> XBoundaries = new();
    [SerializeField] private List<Transform> YBoundaries = new();
    [SerializeField] private int XclusterNum = 0;
    [SerializeField] private int YclusterNum = 0;

    StageDatabase stageDatabase;
    int stageNum = 0;

    new void Awake(){
        base.Awake();
    
    }

    public int AddXcluster() => XclusterNum++;
    public int AddYcluster() => YclusterNum++;
    public int MinusXcluster() => XclusterNum--;
    public int MinusYcluster() => YclusterNum--;

    public (Vector2, Vector2) GetCurrentXBoundaries()  {
        return (XBoundaries[XclusterNum].position, XBoundaries[XclusterNum + 1].position);
    }
    public (Vector2, Vector2) GetCurrentYBoundaries(){
        return (YBoundaries[YclusterNum].position, YBoundaries[YclusterNum + 1].position);
    }
    public bool IsEndOfXBoundary {
        get{
            return XclusterNum == XBoundaries.Count - 2;
        }  
        private set{}
    
    }
    public bool IsEndOfYBoundary {
        get{
            return YclusterNum == YBoundaries.Count - 2;
        }    
        private set{}
    
    }

    public bool IsStartOfXBoundary{
        get{
            return XclusterNum == 0;
        }
        private set{}
    }
    public bool IsStartOfYBoundary{
        get{
            return YclusterNum == 0;
        }
        private set{}
    }


}