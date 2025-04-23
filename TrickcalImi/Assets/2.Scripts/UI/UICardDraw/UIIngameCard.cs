using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIIngameCard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Image cardContentsImage;
    [SerializeField] private TextMeshProUGUI cardHeaderText;
    [SerializeField] private TextMeshProUGUI costCountText;

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void SetUIIngameCard(/*JsonIngameCard*/)
    {

    }
}
