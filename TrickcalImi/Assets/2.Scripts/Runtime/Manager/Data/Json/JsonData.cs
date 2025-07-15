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
    public string PoolPath;
    public string Description;
    public int HP;
    public int MP;
    public AttackType AttackType;
    public float MoveSpeed;
    public float ChaseSpeed;
    public float TrackingRange;
    public int NormalDamage;
    public float NormalAttackDelay;
    public float NormalAttackRange;
    public List<int> SkillList;
    public int WeaponSoundID;
}

public class JsonUIIngameObject
{
    public int IngameObjectID;
    public string UIName;
    public string Description;
    public string IconImagePath;
    public string UIAnimPath;
}
public class JsonChapter
{
    public int ChapterID;
    public string ChapterHeader;
    public List<int> StageIDList;
    public int SoundID;
}

public class JsonStage
{
    public int StageID;
    public int ChapterNumber;
    public string StageName;
    public int SetupCost;
    public int ChargeCost;
    public List<int> WaveList;
}

public class JsonWave
{
    public int WaveID;
    public List<int> MonsterList;
    public List<int> SpawnIndexList;
}

public class JsonSound
{
    public int SoundID;
    public SoundType SoundType;
    public string SoundResPath;
}