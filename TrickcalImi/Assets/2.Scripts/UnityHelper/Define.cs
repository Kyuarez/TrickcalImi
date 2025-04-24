using System.IO;
using UnityEngine;

public static class Define 
{
    public static readonly string FilePath_LocalData = Path.Combine(Application.persistentDataPath, "JsonLocalUserData.json");

    public static readonly string Name_Pool_Hero = "@Hero";
    public static readonly string Name_Pool_Enemy = "@Enemy";
    public static readonly string Name_Pool_FX = "@FX";
    public static readonly string Name_Pool_UI = "@UI";
    public static readonly string Name_Hero_Weapon = "@Weapon";


    public static readonly int OrderLayer_titleUI = 10;
    public static readonly int OrderLayer_baseUI = 9;
    public static readonly int OrderLayer_ingameUI = 8;

    public static readonly float Default_Enemy_NormalDamage = 5.0f;

    #region Res
    public static readonly string Res_UI_Pool = "Prefabs/UI/Pool";
    public static readonly string Res_UI_LobbyBackground = "Sprites/UI/Lobby/BackgroundImage";
    public static readonly string Res_UI_LobbyStageSlot = "Prefabs/UI/Pool/Lobby/UIStageSlot";
    public static readonly string Res_UI_Icon_Hero = "Sprites/UI/Icon/Hero/";
    public static readonly string Res_UI_Icon_Enemy = "Sprites/UI/Icon/Enemy/";
    public static readonly string Res_Sound_BGM = "Sounds/BGM/";
    public static readonly string Res_Sound_SFX = "Sounds/SFX/";
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

public enum UITransitionType
{
    Loading, //로딩바
    FadeInout, //검은색 Fade
    StarCover, //별모양 커버
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

public enum AttackType
{
    Melee,
    Ranged,
}

public enum FXType
{
    Click,
    Heal_HP,
    Heal_MP,
    Hit_Red,
    Hit_Yellow,
    Hit_Ice,
    Hit_Fire,
}

public enum IngameCardType
{
    PickHero,
    Buff,
    Item, //@tk 일단 구현 x
}

public enum BuffType
{

}


public enum EnvironmentType
{
    ForestRiver,
    TwilightShadowwood,
    SilentSoilGrove,
    EmeraldGlade,
    DuskmistHollow,
    FloatingIsles,
    RuinCastle,
    CursedTemple,
}

public enum SoundType
{
    BGM,
    SFX,
}
#endregion
