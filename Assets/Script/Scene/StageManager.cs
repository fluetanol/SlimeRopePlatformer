using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageManager : SingletonMonobehavior<StageManager>
{
    [SerializeField] private List<Transform> StartBoundaries = new();
    [SerializeField] private List<Transform> EndBoundaries = new();
    [SerializeField] private int clusterNum = 0;

    StageDatabase stageDatabase;
    int stageNum = 0;

    new void Awake(){
        base.Awake();
    
    }

    public int Addcluster() => clusterNum++;
    public int Minuscluster() => clusterNum--;
    public Transform GetCurrentStartBoundaries() => StartBoundaries[clusterNum];
    public Transform GetCurentEndBoundaries() => EndBoundaries[clusterNum];
}