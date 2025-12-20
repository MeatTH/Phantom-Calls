using UnityEngine;
using UnityEngine.EventSystems;

public class CloseChatPanelTrigger : MonoBehaviour, IPointerClickHandler
{
    [Header("Sound Settings")]
    [Tooltip("ชื่อไฟล์เสียง SFX ที่ต้องการเล่นเมื่อกดปุ่ม")]
    public string clickSoundName = "ui_click"; // ตั้งค่าชื่อ default หรือเปลี่ยนใน Inspector

    [Header("Chat Panel ที่ต้องการปิด")]
    public GameObject chatPanelToClose;

    [Header("การกระโดด Ink (Load New Ink Story หรือ Jump Knot)")]
    [Tooltip("ถ้าเว้นว่าง Ink Name จะ Jump ไป Knot ใน Story เดิม")]
    public string inkNameToLoad = "";
    [Tooltip("ชื่อ Knot หรือ Stitch ที่ต้องการ Jump ไป")]
    public string jumpToKnot = "";


    public void OnPointerClick(PointerEventData eventData)
    {
        // ✅ 1. เล่นเสียงก่อน (เรียกผ่าน SoundManager)
        if (!string.IsNullOrEmpty(clickSoundName) && SoundManager_Test1.instance != null)
        {
            SoundManager_Test1.instance.PlaySFX(clickSoundName);
        }

        // 2. ปิด Chat Panel
        if (chatPanelToClose != null)
        {
            chatPanelToClose.SetActive(false);
        }

        var mgr = DialogueManager_Test1.GetInstance();
        if (mgr == null) return;

        // 3. สั่งให้ Ink Story ดำเนินการต่อ/กระโดด Knot
        bool willJump = !string.IsNullOrEmpty(jumpToKnot) || !string.IsNullOrEmpty(inkNameToLoad);

        if (willJump)
        {
            // ใช้ฟังก์ชัน LoadNewInkAndJump เพื่อจัดการทั้งการโหลด Ink ใหม่และการ Jump Knot
            if (!string.IsNullOrEmpty(inkNameToLoad))
            {
                // ถ้ามีชื่อ Ink ใหม่: โหลด Ink ใหม่และ Jump (ถ้ามี Knot)
                mgr.LoadNewInkAndJump(inkNameToLoad, jumpToKnot);
            }
            else
            {
                // ถ้าไม่มีชื่อ Ink ใหม่: Jump Knot ใน Story เดิม
                mgr.EnsureOpen(); // ทำให้ Dialogue Panel เปิดเสมอเผื่อจำเป็น
                mgr.JumpToKnot(jumpToKnot);
            }
        }
        else
        {
            // ถ้าไม่มีการระบุ Knot/Story ให้จบ Chat ตามปกติ (Continue Story)
            mgr.OnChatFinished();
        }

        // ปิดสถานะรอ Chat จบของ ChatFlowController
        var chatFlow = chatPanelToClose.GetComponent<ChatFlowController>();
        if (chatFlow != null)
        {
            // Logic เพิ่มเติมถ้าจำเป็น
        }
    }
}