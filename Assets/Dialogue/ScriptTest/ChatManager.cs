/*using UnityEngine;
using System.Collections.Generic;

public class ChatManager : MonoBehaviour
{
    [Tooltip("ใส่ ChatFlowController ทั้งหมดที่อยู่ในฉากนี้")]
    public List<ChatFlowController> chats = new List<ChatFlowController>();

    private void Awake()
    {
        foreach (var c in chats)
        {
            if (c == null) continue;
            // สมัคร event ตอนเริ่ม
            c.OnChatFinishedEvent += OnChatFinishedHandler;
        }
    }

    private void OnDestroy()
    {
        foreach (var c in chats)
        {
            if (c == null) continue;
            c.OnChatFinishedEvent -= OnChatFinishedHandler;
        }
    }

    private void OnChatFinishedHandler(ChatFlowController who)
    {
        // ปิดแชทตัวที่จบ (กันพลาด)
        if (who != null && who.chatPanel != null)
            who.chatPanel.SetActive(false);

        // ให้ DialogueManager ไปต่อ (กันพลาดอีกชั้น)
        var dlg = DialogueManager_Test1.GetInstance();
        if (dlg != null) dlg.ContinueStory();
    }

    // ถ้าอยากเปิดแชทตาม id/ชื่อ (เรียกจาก Inkle EXTERNAL ก็ได้)
    public void OpenChat(GameObject panelToOpen)
    {
        if (panelToOpen != null) panelToOpen.SetActive(true);
    }
}
*/