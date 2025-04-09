using System;
using System.Collections.Generic;
using UnityEngine;

/* [25.04.09]
 구글 드라이브 내부에 있는 구글 스프레드 시트 데이터들 
해당하는 데이터 클래스로 파싱 후에 딕셔너리에 저장.
 
 
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
