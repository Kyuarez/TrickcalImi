using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITransitionLoading : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private Image heroImage;
    [SerializeField] private TextMeshProUGUI heroText;

    private Coroutine loadingCoroutine;

    public void SetActivePanel(bool active)
    {
        if(panel.activeSelf == !active)
        {
            panel.SetActive(active);
        }

        loadingSlider.value = 0f;
    }

    public void OnTransitionLoading(float duration)
    {
        UIManager.Transition.OnEndTransitionAction += ResetTransitionLoading;
        //TODO : ���� ������ �ִ� ĳ���� �߿��� �ϳ� ���� �̾Ƽ� UI ����
        SetActivePanel(true);

        //�ð� �ʸ�ŭ Loading
        if(loadingCoroutine != null)
        {
            StopCoroutine(loadingCoroutine);
            loadingCoroutine = null;
        }

        loadingCoroutine = StartCoroutine(LoadingCo(duration));
    }

    public void ResetTransitionLoading()
    {
        SetActivePanel(false);
        loadingSlider.value = 0f;
        loadingCoroutine = null;
        heroImage.sprite = null;
        heroText.text = string.Empty;

        UIManager.Transition.OnEndTransitionAction -= ResetTransitionLoading;
    }

    public IEnumerator LoadingCo(float duration)
    {
        float elapsedTime = 0f;
        while(elapsedTime < duration)
        {
            loadingSlider.value = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        UIManager.Transition.OnEndTransition();
    }

}
