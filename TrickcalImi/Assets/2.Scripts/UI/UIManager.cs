using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    private Canvas canvas;

    protected override void Awake()
    {
        canvas = GetComponent<Canvas>();

        canvas.sortingOrder = Define.OrderLayer_baseUI;
    }
}
