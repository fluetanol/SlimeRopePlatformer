using System;
using System.Collections.Generic;

[Serializable]
public class PlayerFactory
{
    Dictionary<string, Move> _moveDictionary = new Dictionary<string, Move>();
    Dictionary<string, AttackData> _attackDataDictionary = new Dictionary<string, AttackData>();

    public PlayerFactory(){
        _moveDictionary.Add("AccelMove", new PlayerAccelMove());

        _attackDataDictionary.Add("AttackData", new AttackData());
    }

    public void Create(ref Move move, ref AttackData attackData,EPlayerElementalType type){
        //if(_moveDictionary.ContainsKey(key)){
         //   move = _moveDictionary[key];
        //}
        //if(_attackDataDictionary.ContainsKey(key)){
         //   attackData = _attackDataDictionary[key];
        //}

    }


    public Move GetMove(string key){
        if(_moveDictionary.ContainsKey(key)){
            return _moveDictionary[key];
        }
        return null;
    }

    public AttackData GetAttackData(string key){
        if(_attackDataDictionary.ContainsKey(key)){
            return _attackDataDictionary[key];
        }
        return new AttackData();
    }

}
