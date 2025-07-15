using System;
using System.Collections.Generic;

public class DataListWrapper<T>
{
    public List<T> items;
}

[System.Serializable]
public class Sheet_StageData
{
    public int StageID;
    public string StageName;
}

[System.Serializable]
public class Sheet_UnitData
{
    public int ObjectID;
    public string CodeName;
    public int HP;
    public int MP;
    public int Damage;
}


