using System;
using UnityEngine;

public class UITransition : MonoBehaviour
{
    [SerializeField] private GameObject coverImage; //Ŭ�� ������
    [SerializeField] private UITransitionLoading loading;

    public Action OnEndTransitionAction;


    public void OnTransition(UITransitionType type)
    {
        coverImage.SetActive(true);

        switch (type)
        {
            case UITransitionType.Loading:
                loading.OnTransitionLoading(1.0f);
                break;
            case UITransitionType.FadeInout:
                break;
            case UITransitionType.StarCover:
                break;
        }

    }

    public void OnEndTransition()
    {
        coverImage.SetActive(false);
        OnEndTransitionAction?.Invoke();
    }
}
