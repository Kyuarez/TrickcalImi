using System;
using System.Collections.Generic;
using UnityEngine;

/* [25.04.09]
 ���� ����̺� ���ο� �ִ� ���� �������� ��Ʈ �����͵� 
�ش��ϴ� ������ Ŭ������ �Ľ� �Ŀ� ��ųʸ��� ����.
 
 
 */
public class SheetManager
{
    private static SheetManager instance;
    public static SheetManager Instance
    {
        get 
        {
            if(instance == null)
            {
                Debug.Assert(false, "SheetManager didn't Initialize");
                return null;
            }

            return instance; 
        }
    }

    public void OnLoadSheetData()
    {

    }

    
}
