using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDamagePopup : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI damageText;

    private Coroutine coroutine;

    private void OnEnable()
    {
        //TODO
    }

    private void OnDisable()
    {
        ResetDamagePopup();
    }

    public void SetActivePanel(bool active)
    {
        if(panel.activeSelf == !active)
        {
            panel.SetActive(active);
        }
    }

    public void OnDamagePopup(float damage)
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        coroutine = StartCoroutine(OnDamagePopupCo());
    }

    private void ResetDamagePopup()
    {

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        damageText.text = string.Empty;
    }

    IEnumerator OnDamagePopupCo()
    {
        yield return null;
    }
}
