using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UICardDraw : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    [Header("CardDraw Panel")]
    [SerializeField] private Transform cardDeck;

    [Header("OnCombatButton")]
    [SerializeField] private Button btn_OnCombat;

    private void Awake()
    {
        btn_OnCombat.onClick.AddListener(OnClickOnCombat);
    }

    public void OnSetupAction()
    {
        SetCardDeck();
        SetActivePanel(true);
    }

    public void OnCombatAction()
    {
        ResetCardDeck();
        SetActivePanel(false);
    }
    public void OnResetAction()
    {
        ResetCardDeck();
        SetActivePanel(false);
    }

    public void SetActivePanel(bool active)
    {
        if(panel.activeSelf == !active)
        {
            panel.SetActive(active);
        }
    }

    private void SetCardDeck()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject cardObj = PoolManager.Instance.SpawnObject("UIIngameCard");
            if(cardObj != null)
            {
                cardObj.transform.SetParent(cardDeck);

            }
        }
    }

    private void ResetCardDeck()
    {
        for (int i = 0; i < cardDeck.childCount; i++)
        {
            PoolManager.Instance.DespawnObject("UIIngameCard", cardDeck.GetChild(i).gameObject);
        }
    }

    #region OnClick
    public void OnClickOnCombat()
    {
        StageManager.Instance.OnCombatMode();
    }

    #endregion
}
