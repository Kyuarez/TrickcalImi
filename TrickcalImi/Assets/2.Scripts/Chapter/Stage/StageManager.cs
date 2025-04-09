using System;
using UnityEngine;

public class StageManager : MonoSingleton<StageManager>
{

    //@tk (25.04.09) ������ ������Ÿ�� ���� ������, �����͸� ������ ���������� ���ķδ� é�ͷ� ���� �޾ƿ���
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
        //�˾Ƽ� ������ ���� ����

        //�˾Ƽ� ���� ����?
    }

    
}
