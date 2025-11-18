using UnityEngine;
using UnityEngine.EventSystems;

public class CloseChatPanelTrigger : MonoBehaviour, IPointerClickHandler
{
    [Header("Chat Panel ที่ต้องการปิด")]
    public GameObject chatPanelToClose;

    [Header("การกระโดด Ink (Load New Ink Story หรือ Jump Knot)")]
    [Tooltip("ถ้าเว้นว่าง Ink Name จะ Jump ไป Knot ใน Story เดิม")]
    public string inkNameToLoad = "";
    [Tooltip("ชื่อ Knot หรือ Stitch ที่ต้องการ Jump ไป")]
    public string jumpToKnot = "";


    public void OnPointerClick(PointerEventData eventData)
    {
        // 1. ปิด Chat Panel
        if (chatPanelToClose != null)
        {
            chatPanelToClose.SetActive(false);
        }

        var mgr = DialogueManager_Test1.GetInstance();
        if (mgr == null) return;

        // 2. สั่งให้ Ink Story ดำเนินการต่อ/กระโดด Knot
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
        // (ถึงแม้จะปิด Panel แล้วก็ควรสั่งให้จบสถานะรอไว้ก่อน)
        var chatFlow = chatPanelToClose.GetComponent<ChatFlowController>();
        if (chatFlow != null)
        {
            // สมมติว่า ChatFlowController มีฟังก์ชันสำหรับ reset สถานะรอ (แม้จะปิด Panel ไปแล้ว)
            // แต่เนื่องจากเราไม่รู้ว่า ChatFlowController เก็บ reference ของตัวเองใน DialogueManager หรือไม่
            // เราจะอาศัยการสั่ง OnChatFinished() ที่เรียกโดยตรงในกรณีไม่มี Jump แทน

            // ในกรณีที่มีการ Jump (willJump = true) เราจะถือว่า Ink ได้ถูกควบคุมการไหลแล้ว 
            // จึงไม่จำเป็นต้องเรียก OnChatFinished()
        }
    }
}