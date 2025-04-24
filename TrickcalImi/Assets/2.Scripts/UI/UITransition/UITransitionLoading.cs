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
        SetLoadingUIByRandom();
        SetActivePanel(true);

        //시간 초만큼 Loading
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

    public void SetLoadingUIByRandom()
    {
        int heroID = LocalDataManager.Instance.GetHeroIDByLocalData();
        JsonUIIngameObject json = TableManager.Instance.FindTableData<JsonUIIngameObject>(heroID);

        if (json == null)
        {
            return;
        }

        heroText.text = json.UIName;
        //@tk : 이거 나중에 애니메이션 (렌더 텍스쳐)로 변경 필요
        Sprite heroIcon = Resources.Load<Sprite>(Define.Res_UI_Icon_Hero + json.IconImagePath);
        if (heroText != null)
        {
            heroImage.sprite = heroIcon;
        }
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
