using UnityEngine;
using System.Collections;

public class PanelTransitioner : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.3f;

    private Animator animator;
    private CanvasGroup canvasGroup;
    private Coroutine currentFade;

    // ชื่อ Parameter ใน Animator (ถ้าใช้ Animator)
    private const string VISIBILITY_PARAM = "IsVisible";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        canvasGroup = GetComponent<CanvasGroup>();

        // ถ้ามี CanvasGroup และเริ่มมาปิดอยู่ ให้เซ็ต Alpha เป็น 0
        if (canvasGroup != null)
        {
            if (!gameObject.activeSelf) canvasGroup.alpha = 0f;
        }
    }

    // [แก้ไข] เพิ่มพารามิเตอร์ transitionType พร้อมค่าเริ่มต้น
    public void ShowPanel(string transitionType = "instant")
    {
        // 1. เปิด GameObject
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        // 2. ตัดสินใจตาม transitionType หรือ Component ที่มี

        // กรณี A: ใช้ Fade (ถ้ามี CanvasGroup และไม่ได้บังคับ instant)
        if (canvasGroup != null && transitionType != "instant")
        {
            if (currentFade != null) StopCoroutine(currentFade);
            currentFade = StartCoroutine(Fade(1f));
        }
        // กรณี B: ใช้ Animator (ถ้ามี Animator และไม่ได้บังคับ instant)
        else if (animator != null && transitionType != "instant")
        {
            // ตัวอย่าง: ถ้า Ink ส่งมาว่า "slide" ก็สั่ง SlideIn
            if (transitionType == "slide")
            {
                animator.Play("SlideIn");
            }
            else
            {
                // ค่า Default ของ Animator
                animator.SetBool(VISIBILITY_PARAM, true);
            }
        }
        // กรณี C: Instant (ไม่มี Effect หรือ Ink สั่ง instant)
        else
        {
            // เปิดไปแล้วที่ข้อ 1, ไม่ต้องทำอะไรเพิ่ม
        }
    }

    // [แก้ไข] เพิ่มพารามิเตอร์ transitionType พร้อมค่าเริ่มต้น
    public void HidePanel(string transitionType = "instant")
    {
        // กรณี A: ใช้ Fade Out
        if (canvasGroup != null && transitionType != "instant")
        {
            if (currentFade != null) StopCoroutine(currentFade);
            currentFade = StartCoroutine(Fade(0f, true)); // true = ปิด GameObject เมื่อจบ
        }
        // กรณี B: ใช้ Animator
        else if (animator != null && transitionType != "instant")
        {
            if (transitionType == "slide")
            {
                animator.Play("SlideOut");
            }
            else
            {
                animator.SetBool(VISIBILITY_PARAM, false);
            }
            // หมายเหตุ: สำหรับ Animator คุณต้องไปตั้ง Event ใน Animation ให้ปิด GameObject ด้วยตัวเอง
        }
        // กรณี C: Instant Close
        else
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator Fade(float targetAlpha, bool disableOnComplete = false)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0;

        canvasGroup.blocksRaycasts = true; // ป้องกันการคลิกซ้อน

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        if (disableOnComplete && targetAlpha == 0f)
        {
            gameObject.SetActive(false);
            canvasGroup.blocksRaycasts = false;
        }
        else if (targetAlpha == 1f)
        {
            canvasGroup.blocksRaycasts = true; // เปิดให้คลิกได้ปกติ
        }
        else
        {
            canvasGroup.blocksRaycasts = false;
        }
    }
}