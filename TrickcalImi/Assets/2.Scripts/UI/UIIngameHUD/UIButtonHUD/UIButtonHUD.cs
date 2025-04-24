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

        //@tk TODO ���� ��� ���� �������� ���ͷ��ͺ� ����
        btn_AutoBattle.interactable = false;
        btn_IngameSpeed.interactable = false;
    }

    public void OnClickAutoBattle()
    {
        SoundManager.Instance.PlaySFX(10000);
        //TODO ī�� ���� ���� �˾Ƽ� �ڵ� 
    }

    public void OnClickIngameSpeed()
    {
        SoundManager.Instance.PlaySFX(10000);
        //TODO �ΰ��� Ÿ�� ������ �ø��簡 �ؼ� �ӵ� ������
    }

    public void OnClickPause()
    {
        SoundManager.Instance.PlaySFX(10000);
        //TODO ���� é�� ��Ȳ �����ְ�, ������ �˾� �����
    }
}
