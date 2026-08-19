using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverEffect : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    // =========================================================
    // SCALE SETTINGS
    // =========================================================

    [Header("Scale Settings")]

    // Mouse hover par button kitna bada hoga
    // 1.02 = 2% bigger
    [SerializeField] private float hoverScale = 1.02f;

    // Mouse press par button kitna chhota hoga
    // 0.98 = 2% smaller
    [SerializeField] private float pressedScale = 0.98f;

    // Button kitni smoothly target scale tak jayega
    [SerializeField] private float scaleSpeed = 12f;


    // =========================================================
    // OUTLINE SETTINGS
    // =========================================================

    [Header("Outline Settings")]

    // Button ke Outline component ka reference
    [SerializeField] private Outline buttonOutline;

    // Normal state mein outline ka color
    [SerializeField]
    private Color normalOutlineColor =
        new Color32(53, 217, 255, 255);

    // Hover state mein outline ka brighter color
    [SerializeField]
    private Color hoverOutlineColor =
        new Color32(125, 244, 255, 255);


    // =========================================================
    // RUNTIME DATA
    // =========================================================

    // Button ki original starting scale
    private Vector3 normalScale;

    // Button kis scale ki taraf smoothly move karega
    private Vector3 targetScale;


    // =========================================================
    // INITIALIZATION
    // =========================================================

    private void Awake()
    {
        // Button ki original scale save karna
        normalScale = transform.localScale;

        // Starting target bhi normal scale rakhna
        targetScale = normalScale;


        // Agar Inspector se Outline assign nahi kiya
        // to isi GameObject se automatically lena
        if (buttonOutline == null)
        {
            buttonOutline = GetComponent<Outline>();
        }


        // Starting outline color set karna
        if (buttonOutline != null)
        {
            buttonOutline.effectColor =
                normalOutlineColor;
        }
    }


    // =========================================================
    // SMOOTH SCALE ANIMATION
    // =========================================================

    private void Update()
    {
        // Current scale ko smoothly
        // targetScale ki taraf move karna
        transform.localScale =
            Vector3.Lerp(
                transform.localScale,
                targetScale,
                scaleSpeed * Time.unscaledDeltaTime
            );
    }


    // =========================================================
    // MOUSE ENTER / HOVER
    // =========================================================

    public void OnPointerEnter(
        PointerEventData eventData
    )
    {
        // Hover par button thora bada karna
        targetScale =
            normalScale * hoverScale;


        // Outline ko brighter cyan karna
        if (buttonOutline != null)
        {
            buttonOutline.effectColor =
                hoverOutlineColor;
        }
    }


    // =========================================================
    // MOUSE EXIT
    // =========================================================

    public void OnPointerExit(
        PointerEventData eventData
    )
    {
        // Button ko original scale par lana
        targetScale = normalScale;


        // Outline ko normal color par lana
        if (buttonOutline != null)
        {
            buttonOutline.effectColor =
                normalOutlineColor;
        }
    }


    // =========================================================
    // MOUSE BUTTON PRESS
    // =========================================================

    public void OnPointerDown(
        PointerEventData eventData
    )
    {
        // Press karne par button thora chhota
        // taake physical press feel aaye
        targetScale =
            normalScale * pressedScale;
    }


    // =========================================================
    // MOUSE BUTTON RELEASE
    // =========================================================

    public void OnPointerUp(
        PointerEventData eventData
    )
    {
        // Mouse abhi button ke upar hota hai,
        // isliye hover scale par wapas lana
        targetScale =
            normalScale * hoverScale;
    }
}