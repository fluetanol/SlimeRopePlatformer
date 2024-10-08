using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct StageData{
    public int stageNum;
    public int partNum;
    public string sceneName;
}


[CreateAssetMenu(fileName = "ScriptableDatabase", menuName = "ScriptableObject/StageDatabase")]
public class StageDatabase : ScriptableObject{
    public List<StageData> stageDatabase = new List<StageData>();
}
