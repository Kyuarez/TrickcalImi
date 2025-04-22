using UnityEngine;

public static class Define 
{
    public static readonly string Name_Pool_Hero = "@Hero";
    public static readonly string Name_Pool_Enemy = "@Enemy";
    public static readonly string Name_Pool_FX = "@FX";
    public static readonly string Name_Pool_UI = "@UI";

    public static readonly int OrderLayer_titleUI = 10;
    public static readonly int OrderLayer_baseUI = 9;
    public static readonly int OrderLayer_ingameUI = 8;

    public static readonly int OrderLayer_background = -1;
    public static readonly int OrderLayer_HeroShadow = 0;
    public static readonly int OrderLayer_HeroFirst = 1;
    public static readonly int OrderLayer_HeroSecond = 2;
    public static readonly int OrderLayer_HeroThird = 3;

    public static readonly float Default_Enemy_NormalDamage = 5.0f;

    #region Res
    public static readonly string Res_UI_Pool = "Prefabs/UI/Pool";
    public static readonly string Res_UI_LobbyBackground = "Sprites/UI/Lobby/BackgroundImage";
    public static readonly string Res_UI_LobbyStageSlot = "Prefabs/UI/Pool/Lobby/UIStageSlot";
    #endregion

    #region Color
    public static Color Color_UI_HP_Hero = new Color(0f, 1f, 0f);
    public static Color Color_UI_HP_Monster = new Color(128f/255f, 0f, 128f/255f);
    public static Color Color_UI_MP = new Color(0f, 0f, 1f);

    #endregion
}

#region EnumType
public enum GameSceneType
{
    Title,
    Lobby,
    Ingame,
}

public enum LobbyType
{
    LobbyMain,
    LobbyAdventure,
    LobbySelectStage,
}

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
    Idle,
    Walk,
    Chase,
    Attack,
    Hit,
    Dead,
    None = 99,
}

public enum EnemyState 
{
    Idle,
    Walk,
    Chase,
    Attack,
    Hit,
    Dead,
    None = 99,
}

public enum HealthType
{
    HP,
    MP,
}

public enum FXType
{
    Click,
    Hit_Red,
    Hit_Yellow,
    Hit_Ice,
    Hit,Fire,
}
#endregion
