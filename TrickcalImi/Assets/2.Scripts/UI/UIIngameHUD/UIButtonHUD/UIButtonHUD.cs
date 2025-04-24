using UnityEngine;
using UnityEngine.UI;

public class UIButtonHUD : MonoBehaviour
{
    [SerializeField] private Button btn_AutoBattle;
    [SerializeField] private Button btn_IngameSpeed;
    [SerializeField] private Button btn_Pause;

    private void Awake()
    {
        btn_AutoBattle.onClick.AddListener(OnClickAutoBattle);
        btn_IngameSpeed.onClick.AddListener(OnClickIngameSpeed);
        btn_Pause.onClick.AddListener(OnClickPause);

        //@tk TODO 차후 기능 구현 전까지는 인터렉터블 막기
        btn_AutoBattle.interactable = false;
        btn_IngameSpeed.interactable = false;
    }

    public void OnClickAutoBattle()
    {
        SoundManager.Instance.PlaySFX(10000);
        //TODO 카드 선택 까지 알아서 자동 
    }

    public void OnClickIngameSpeed()
    {
        SoundManager.Instance.PlaySFX(10000);
        //TODO 인게임 타임 스케일 올리든가 해서 속도 놉히기
    }

    public void OnClickPause()
    {
        SoundManager.Instance.PlaySFX(10000);
        //TODO 현재 챕터 상황 보여주고, 나가기 팝업 만들기
    }
}
