using UnityEngine;

public class FixedAspectRatio : MonoBehaviour
{
    // กำหนดขนาดอ้างอิงตามที่คุณต้องการ
    public float targetWidth = 800f;
    public float targetHeight = 1340f;

    void Update()
    {
        // คำนวณทุกเฟรมเพื่อให้รองรับตอนผู้เล่นลากย่อ-ขยายหน้าต่าง
        UpdateCameraRatio();
    }

    void UpdateCameraRatio()
    {
        // 1. คำนวณอัตราส่วนเป้าหมาย (800 / 1340 = 0.597)
        float targetAspect = targetWidth / targetHeight;

        // 2. คำนวณอัตราส่วนหน้าต่างปัจจุบันของคนเล่น
        float windowAspect = (float)Screen.width / (float)Screen.height;

        // 3. หาความต่างของสัดส่วน
        float scaleHeight = windowAspect / targetAspect;

        Camera camera = GetComponent<Camera>();

        // 4. ปรับ Viewport ของกล้อง (Letterboxing)
        if (scaleHeight < 1.0f)
        {
            // กรณี: หน้าต่าง "แคบ" หรือ "สูง" กว่าปกติ (จะมีขอบดำ บน-ล่าง)
            Rect rect = camera.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f; // จัดกึ่งกลางแนวตั้ง
            camera.rect = rect;
        }
        else
        {
            // กรณี: หน้าต่าง "กว้าง" กว่าปกติ (จะมีขอบดำ ซ้าย-ขวา) --> เคสของคุณจะเป็นอันนี้บ่อยสุด
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = camera.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f; // จัดกึ่งกลางแนวนอน
            rect.y = 0;
            camera.rect = rect;
        }
    }
}