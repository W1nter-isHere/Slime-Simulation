using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private CanvasGroup _canvasGroup;

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0.05f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _canvasGroup.DOFade(1, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _canvasGroup.DOFade(0.05f, 0.2f);
    }
}