using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICard : MonoBehaviour
{
    [SerializeField] private Image cardImage;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI cardNameText;

    //@tk : Pool�� ��������
    public void OnSetUICard(Image cardImage, string costName, int cost)
    {

    }

    public void DeActiveUICard()
    {

    }
}
