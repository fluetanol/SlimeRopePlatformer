using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//플레이어 이동 상태
public enum EPlayerMoveState
{
    ForceStop = -1,  //ForceStop은 이동을 하면 안되는 상태로 정의
    Idle = 0,
    Run = 1,
    Jump = 2,
}
//플레이어 공중/ 지상 상태 여부
public enum EPlayerLandState
{
    Land,
    Koyote,//공중에 떠도 잠깐 Land처럼 인식하는 상태
    Air
}

//플레이어의 행동 상태 (사실 애니메이션, 시퀀스 제어를 위한 상태에 가까움)
public enum EPlayerBehaviourState
{
    Normal, //아무것도 안한 일반 상태(공격이 가능 함)
    Attack, //우클릭 누를 시
    AttackWait, //공격 후 대기 상태(공격이 종료되긴 했지만, 쿨타임 등으로 인해 공격을 할 수 없음)
    Dead,   //죽었을 때 (아무것도 못하게 막아야 함)
}