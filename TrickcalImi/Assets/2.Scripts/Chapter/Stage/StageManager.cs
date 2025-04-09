using System;
using UnityEngine;

public class StageManager : MonoSingleton<StageManager>
{

    //@tk (25.04.09) 지금은 프로토타입 제작 때문에, 데이터를 강제로 주입했지만 이후로는 챕터로 부터 받아오기
    #region Test
    [Header("Test")]
    public int testEnemyCount = 10;
    #endregion

    private IngameModeType currentMode;

    public Action OnCombatAction;

    public void OnCombatMode()
    {
        if(currentMode == IngameModeType.Combat)
        {
            return;
        }

        currentMode = IngameModeType.Combat;

        OnCombatAction?.Invoke();
        //알아서 영웅도 전투 모드로

        //알아서 몬스터 생성?
    }

    
}
