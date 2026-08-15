using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverEffect : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Header("Scale Settings")]
    [SerializeField] private float hoverScale = 1.02f;
    [SerializeField] private float pressedScale = 0.98f;
    [SerializeField] private float scaleSpeed = 12f;

    [Header("Outline Settings")]
    [SerializeField] private Outline buttonOutline;
    [SerializeField] private Color normalOutlineColor = new Color32(53, 217, 255, 255);
    [SerializeField] private Color hoverOutlineColor = new Color32(125, 244, 255, 255);

    private Vector3 normalScale;
    private Vector3 targetScale;

    private void Awake()
    {
        normalScale = transform.localScale;
        targetScale = normalScale;

        if (buttonOutline == null)
        {
            buttonOutline = GetComponent<Outline>();
        }

        if (buttonOutline != null)
        {
            buttonOutline.effectColor = normalOutlineColor;
        }
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            scaleSpeed * Time.unscaledDeltaTime
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = normalScale * hoverScale;

        if (buttonOutline != null)
        {
            buttonOutline.effectColor = hoverOutlineColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = normalScale;

        if (buttonOutline != null)
        {
            buttonOutline.effectColor = normalOutlineColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = normalScale * pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetScale = normalScale * hoverScale;
    }
}