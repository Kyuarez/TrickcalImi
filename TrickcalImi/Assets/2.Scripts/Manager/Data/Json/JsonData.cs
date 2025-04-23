using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class Wrapper<T>
{
    public List<T> Items;
}


public class JsonIngameObject
{
    public int ObjectID;
    public string CodeName;
    public string Description;
    public int HP;
    public int MP;
    public AttackType AttackType;
    public int NormalDamage;
    public float NormalAttackDelay;
    public List<int> SkillList;
}

public class JsonChapter
{
    public int ChapterID;
    public string ChapterHeader;
    public List<int> StageIDList;
}

public class JsonStage
{
    public int StageID;
    public int ChapterNumber;
    public string StageName;
    public int WaveID;
    public int SetupCost;
    public int ChargeCost;
    public List<int> EnemyList;
}