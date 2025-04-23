using System.Collections;
using UnityEngine;

public class InGameCameraController : MonoBehaviour
{
    private Camera cam;

    private Coroutine zoomCoroutine;

    private void Awake()
    {
        cam = gameObject.GetComponent<Camera>();
    }

    private void Start()
    {
        StageManager.Instance.OnSetupAction += OnSetupAction;
        StageManager.Instance.OnCombatAction += OnCombatAction;
        StageManager.Instance.OnResetAction += OnResetAction;
    }

    public void OnSetupAction()
    {
        if (zoomCoroutine != null) 
        {
            StopCoroutine(zoomCoroutine);
            zoomCoroutine = null;
        }

        zoomCoroutine = StartCoroutine(IngameZoomInCo());
    }

    public void OnCombatAction()
    {
        if (zoomCoroutine != null)
        {
            StopCoroutine(zoomCoroutine);
            zoomCoroutine = null;
        }

        zoomCoroutine = StartCoroutine(IngameZoomOutCo());
    }

    public void OnResetAction()
    {
        //camera °ª ¸®¼Â
    }

    private IEnumerator IngameZoomInCo()
    {
        yield return null;
    }
    private IEnumerator IngameZoomOutCo()
    {
        yield return null;
    }
}
