using UnityEngine;

public static class Define 
{
    public static readonly string Name_Pool_Enemy = "@Enemy";
    public static readonly string Name_Pool_FX = "@FX";

    public static readonly int OrderLayer_titleUI = 10;
    public static readonly int OrderLayer_baseUI = 9;
    public static readonly int OrderLayer_ingameUI = 8;

    public static readonly int OrderLayer_background = -1;
    public static readonly int OrderLayer_HeroShadow = 0;
    public static readonly int OrderLayer_HeroFirst = 1;
    public static readonly int OrderLayer_HeroSecond = 2;
    public static readonly int OrderLayer_HeroThird = 3;

}

#region EnumType
public enum IngameModeType
{
    None,
    Setup,
    Combat,
    Success,
    Failure,
}

public enum HeroState
{
    None,
    Idle,
    Walk,
    Chase,
    Attack,
    Hit,
    Die,
}

public enum EnemyState 
{
    None,
    Idle,
    Walk,
    Chase,
    Attack,
    Hit,
    Die,
}
#endregion
