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

    private float duration = 1.0f;

    private void OnEnable()
    {

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

        damageText.text = damage.ToString();
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

        Color bgColor = backgroundImage.color;
        backgroundImage.color = new Color(bgColor.r, bgColor.g, bgColor.b, 1f);

        Color textColor = damageText.color;
        damageText.color = new Color(textColor.r, textColor.g, textColor.b, 1f);

        transform.position = Vector3.zero;
        transform.localScale = Vector3.one;
    }

    IEnumerator OnDamagePopupCo()
    {
        float elapsedTime = 0.0f;

        Vector3 startPos = transform.position;
        while (elapsedTime < 0.3f)
        {
            transform.position = Vector3.Lerp(startPos, startPos + Vector3.up, elapsedTime / 0.3f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        //아래로 떨어지면서 fade out
        // Fall & Fade Out (0.3 ~ duration)
        Color bgColor = backgroundImage.color;
        Color textColor = damageText.color;
        Vector3 midPos = transform.position;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(midPos, midPos + Vector3.down + Vector3.right, t);
            backgroundImage.color = Color.Lerp(bgColor, new Color(bgColor.r, bgColor.g, bgColor.b, 0), t);
            damageText.color = Color.Lerp(textColor, new Color(textColor.r, textColor.g, textColor.b, 0), t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        PoolManager.Instance.DespawnObject("UIDamagePopup", gameObject);
    }
}
