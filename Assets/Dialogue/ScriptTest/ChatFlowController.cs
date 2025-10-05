using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ChatFlowController : MonoBehaviour
{
    [Header("เริ่มทำงานเมื่อพาเนลเปิด (ไม่รีเซ็ต)")]
    public GameObject chatPanel;
    private bool flowStarted = false;   // เริ่ม flow แล้วหรือยัง (เริ่มครั้งเดียว)
    private bool panelWasActive = false;
    private bool isPaused = false;      // พักระหว่าง panel ปิด

    // ===== เพิ่มไว้ด้านบนภายในคลาส ChatFlowController =====
    [System.Serializable]
    public class OnSpawnTrigger
    {
        [Header("ถ้าบับเบิล prefab นี้ถูก Spawn แล้วให้ทำ...")]
        public GameObject whenThisPrefab;     // ระบุ prefab ที่เป็น "ตัวเหตุ"

        [Header("ปิด Chat Panel ทันทีไหม?")]
        public bool closeChatPanel = true;

        [Header("สั่ง Ink หลังปิดแชท")]
        public bool useInk = true;

        [Tooltip("ถ้าใส่ชื่อ Ink จะโหลดไฟล์นั้น (ต้องตรงกับ TextAsset.name ใน DialogueManager)")]
        public string inkNameToLoad;    // เว้นว่าง = ไม่โหลดไฟล์ใหม่ (ใช้ story ปัจจุบัน)

        [Tooltip("ถ้าระบุจะ Jump ไป knot นี้ (ไฟล์ใหม่/ไฟล์เดิมตามข้างบน)")]
        public string jumpToKnot;       // เว้นว่าง = ไม่ jump

        [Header("หน่วงเวลานิดก่อนสลับ (ป้องกันตัดเอฟเฟกต์)")]
        public float smallDelay = 0.15f;
    }

    [Header("ทริกเกอร์เมื่อบับเบิลบางอันปรากฏ")]
    public List<OnSpawnTrigger> spawnTriggers = new List<OnSpawnTrigger>();


    // ====== โครงสร้างข้อมูลไทม์ไลน์ ======
    [System.Serializable]
    public class Slot
    {
        public string name;
        public int step = 0;

        [Header("NPC (ซ้าย) : ภาพที่จะโชว์ตามลำดับ")]
        public List<GameObject> npcMessagePrefabs = new List<GameObject>();

        [Header("ตั้งเวลาการโชว์อัตโนมัติของ NPC")]
        public float npcStartDelay = 0.4f;
        public float npcInterval = 0.8f;

        [Header("Player Choice (เว้นว่างได้ถ้าไม่มีช้อยส์)")]
        public ChoiceSet choice;
    }

    public enum DockPos { Pos0 = 0, Pos1 = 1, Pos2 = 2 }

    [System.Serializable]
    public class ChoiceSet
    {
        [Tooltip("ช้อยส์ 1–3 ตัว; แต่ละตัวเลือกกำหนดปุ่มตำแหน่ง (dockOverride) ได้")]
        public List<ChoiceOption> options = new List<ChoiceOption>();

        [Tooltip("เลือกแล้ว ปิดปุ่มที่เหลือ (แนะนำให้เปิดเพื่อกันกดซ้ำ)")]
        public bool destroyOthersOnPick = true;
    }

    [System.Serializable]
    public class ChoiceOption
    {
        [Header("ป้ายบนปุ่มช้อยส์")]
        public string buttonText;

        [Header("ข้อความพรีวิว (ว่าง = ใช้ข้อความบนปุ่ม)")]
        [TextArea] public string previewTextOverride;

        [Header("รูปบับเบิลผู้เล่น (ขวา) ที่จะขึ้นหลังผู้เล่นกดส่ง")]
        public GameObject playerMessagePrefab;

        [Header("แตกแขนงไปสล็อตใด (−1 = ไปสล็อตถัดไปตามลำดับ)")]
        public int nextStep = -1;

        [Header("ให้ช้อยส์นี้ไปแสดงที่ปุ่มตำแหน่งใด")]
        public DockPos dockOverride = DockPos.Pos0;   // ← กำหนดปุ่มปลายทาง
    }

    // ====== การตั้งค่า UI หลัก ======
    [Header("คอนเทนต์ของแชท (ScrollView > Content)")]
    public Transform chatContentRoot;

    [Header("สกรอลลงล่างสุดอัตโนมัติ")]
    public ScrollRect scrollRect;

    [Header("เสียงเอฟเฟกต์ (ถ้ามี)")]
    public string sfxPopName = "chat_pop";

    [Header("ปุ่มช้อยส์ 3 ตำแหน่ง (วางไว้ในฉากแล้ว)")]
    public Button[] choiceDockButtons = new Button[3];  // ใส่ปุ่ม Pos0/Pos1/Pos2 ตามลำดับ

    [Header("พรีวิวกลางล่าง + ปุ่มส่ง")]
    public GameObject previewContainer;
    public TMP_Text previewText;
    public float typeCharInterval = 0.02f;
    public Button sendButton;

    [Header("ปุ่มไปต่อ หลังทุกสล็อตจบ (ถ้าต้องการ)")]
    public Button nextButtonAfterAll;

    [Header("ไทม์ไลน์ทั้งหมด (เรียง step จากน้อยไปมาก)")]
    public List<Slot> timeline = new List<Slot>();

    // ====== สถานะภายใน ======
    private int currentSlotIndex = 0;
    private bool isPreviewBusy = false;
    private ChoiceOption _pendingChoice = null;

    private void Start()
    {
        // เก็บสถานะพาเนลตอนเริ่ม
        panelWasActive = (chatPanel != null && chatPanel.activeInHierarchy);
        if (panelWasActive && !flowStarted)
            BeginFlow(); // เริ่มได้เลยถ้าพาเนลเปิดอยู่ตั้งแต่ต้น

        // ปิด prefab ทั้งหมดเผื่อมีเปิดค้างในฉาก
        foreach (var s in timeline)
        {
            foreach (var npc in s.npcMessagePrefabs) if (npc) npc.SetActive(false);
            if (s.choice != null)
                foreach (var o in s.choice.options)
                    if (o.playerMessagePrefab) o.playerMessagePrefab.SetActive(false);
        }

        // ซ่อนปุ่มช้อยส์ทั้งหมดก่อน
        HideAllChoiceButtons();

        // พรีวิว/ส่ง
        if (previewContainer) previewContainer.SetActive(false);
        if (sendButton)
        {
            sendButton.gameObject.SetActive(false);
            sendButton.onClick.RemoveAllListeners();
            sendButton.onClick.AddListener(OnSendClicked);
        }

        // ปุ่มต่อท้ายทั้งหมด
        if (nextButtonAfterAll)
        {
            nextButtonAfterAll.gameObject.SetActive(false);
            nextButtonAfterAll.onClick.RemoveAllListeners();
            nextButtonAfterAll.onClick.AddListener(OnAllFinished);
        }

        // **สำคัญ**: ไม่เรียก RunCurrentSlot() ที่นี่ — ให้เริ่มเฉพาะเมื่อพาเนลเปิดครั้งแรกเท่านั้น
    }

    private void Update()
    {
        bool panelIsActive = (chatPanel != null && chatPanel.activeInHierarchy);

        // ปิด -> เปิด
        if (!panelWasActive && panelIsActive)
        {
            if (!flowStarted)
                BeginFlow();   // เริ่มครั้งแรก
            isPaused = false;  // resume ต่อจากเดิม
        }

        // เปิด -> ปิด
        if (panelWasActive && !panelIsActive)
        {
            isPaused = true;   // พักไว้ (ไม่รีเซ็ต)
        }

        panelWasActive = panelIsActive;
    }

    private void BeginFlow()
    {
        flowStarted = true;
        RunCurrentSlot();
    }

    // ====== Flow ต่อสล็อต ======
    private void RunCurrentSlot()
    {
        if (currentSlotIndex >= timeline.Count)
        {
            if (nextButtonAfterAll) nextButtonAfterAll.gameObject.SetActive(true);
            return;
        }

        var slot = timeline[currentSlotIndex];
        StartCoroutine(RevealNpcAuto(slot));
    }

    private IEnumerator RevealNpcAuto(Slot slot)
    {
        // เคารพ pause
        while (isPaused) yield return null;

        // โชว์ NPC อัตโนมัติ
        if (slot.npcMessagePrefabs != null && slot.npcMessagePrefabs.Count > 0)
        {
            yield return WaitWithPause(slot.npcStartDelay);
            for (int i = 0; i < slot.npcMessagePrefabs.Count; i++)
            {
                SpawnBubble(slot.npcMessagePrefabs[i]);
                yield return WaitWithPause(slot.npcInterval);
            }
        }

        // ถึงคิวผู้เล่น → โชว์ช้อยส์บนปุ่มที่จัดวางไว้แล้ว
        TryShowChoices(slot);
    }

    // รอเวลาแบบเคารพ pause
    private IEnumerator WaitWithPause(float seconds)
    {
        float t = 0f;
        while (t < seconds)
        {
            while (isPaused) yield return null;  // หยุดนับเวลาระหว่าง pause
            t += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    private void TryShowChoices(Slot slot)
    {
        HideAllChoiceButtons();

        if (slot.choice == null || slot.choice.options == null || slot.choice.options.Count == 0)
        {
            // ไม่มีช้อยส์ → ไปสล็อตถัดไปอัตโนมัติ
            GoToNextStep(-1);
            return;
        }

        // 1) เคลียร์ onClick เดิมทั้งหมด
        for (int i = 0; i < choiceDockButtons.Length; i++)
            if (choiceDockButtons[i] != null)
                choiceDockButtons[i].onClick.RemoveAllListeners();

        // 2) จัดลงปุ่มตาม dockOverride
        foreach (var opt in slot.choice.options)
        {
            int dock = (int)opt.dockOverride;
            if (dock < 0 || dock >= choiceDockButtons.Length) continue;

            var btn = choiceDockButtons[dock];
            if (btn == null) continue;

            // อัปเดตป้ายปุ่ม
            var label = btn.GetComponentInChildren<TMP_Text>();
            if (label) label.text =
                !string.IsNullOrWhiteSpace(opt.buttonText) ? opt.buttonText :
                (!string.IsNullOrWhiteSpace(opt.previewTextOverride) ? opt.previewTextOverride : "เลือก");

            btn.gameObject.SetActive(true);

            var capturedSet = slot.choice;
            var capturedOpt = opt;
            btn.onClick.AddListener(() => OnChoiceClicked(capturedSet, capturedOpt));
        }
    }

    private void OnChoiceClicked(ChoiceSet set, ChoiceOption opt)
    {
        if (isPreviewBusy) return;
        if (opt == null || opt.playerMessagePrefab == null) return;

        isPreviewBusy = true;

        // ปิด/แช่แข็งปุ่มอื่น
        if (set.destroyOthersOnPick) HideAllChoiceButtons();
        else SetAllChoiceButtonsInteractable(false);

        string txt =
            !string.IsNullOrWhiteSpace(opt.previewTextOverride) ? opt.previewTextOverride.Trim() :
            (!string.IsNullOrWhiteSpace(opt.buttonText) ? opt.buttonText : "");

        StartCoroutine(PreviewTypingThenEnableSend(txt, opt));
    }

    private IEnumerator PreviewTypingThenEnableSend(string textToType, ChoiceOption opt)
    {
        if (previewContainer) previewContainer.SetActive(true);
        if (sendButton) sendButton.gameObject.SetActive(false);
        if (previewText) previewText.text = "";

        foreach (char c in textToType)
        {
            while (isPaused) yield return null;        // เคารพ pause ระหว่างพิมพ์
            if (previewText) previewText.text += c;
            yield return WaitWithPause(typeCharInterval);
        }

        _pendingChoice = opt;
        if (sendButton) sendButton.gameObject.SetActive(true);
    }

    private void OnSendClicked()
    {
        if (!isPreviewBusy || _pendingChoice == null) return;

        if (sendButton) sendButton.gameObject.SetActive(false);
        if (previewContainer) previewContainer.SetActive(false);

        SpawnBubble(_pendingChoice.playerMessagePrefab);

        int next = _pendingChoice.nextStep;
        _pendingChoice = null;
        isPreviewBusy = false;

        StartCoroutine(GoNextWithDelay(next, 0.35f));
    }

    private IEnumerator GoNextWithDelay(int nextStep, float delay)
    {
        yield return WaitWithPause(delay);
        GoToNextStep(nextStep);
    }

    private void GoToNextStep(int branchNextStep)
    {
        HideAllChoiceButtons();

        if (branchNextStep >= 0)
        {
            int idx = timeline.FindIndex(s => s.step == branchNextStep);
            currentSlotIndex = (idx >= 0) ? idx : currentSlotIndex + 1;
        }
        else
        {
            currentSlotIndex++;
        }

        RunCurrentSlot();
    }

    // ====== Utilities ======
    // ===== แทนที่เมธอด SpawnBubble เดิม =====
    private void SpawnBubble(GameObject prefab)
    {
        if (prefab == null || chatContentRoot == null) return;

        var go = Instantiate(prefab, chatContentRoot);
        go.SetActive(true);

        if (!string.IsNullOrEmpty(sfxPopName) && SoundManager_Test1.instance != null)
            SoundManager_Test1.instance.PlaySFX(sfxPopName);

        if (scrollRect != null)
            //StartCoroutine(ScrollToBottomDeferred());
            StartCoroutine(ScrollToBottomStabilized());

        // ★ ตรวจทริกเกอร์หลัง Spawn
        StartCoroutine(CheckSpawnTriggers(prefab));
    }

    private IEnumerator CheckSpawnTriggers(GameObject spawnedPrefabRef)
    {
        // รอ 1 เฟรมให้เลย์เอาต์/เสียงทำงานก่อน (กันตัดเอฟเฟกต์)
        yield return null;

        // หา trigger ที่ match กับ prefab ที่เพิ่งสแปวน
        foreach (var trg in spawnTriggers)
        {
            if (trg == null || trg.whenThisPrefab == null) continue;

            // เทียบจาก "ตัวอย่าง prefab" ต้นทาง (อ้างจาก reference prefab ที่สั่ง Spawn)
            if (trg.whenThisPrefab == spawnedPrefabRef)
            {
                // ปิดแชทก่อน (ถ้าต้องการ)
                if (trg.closeChatPanel && chatPanel != null)
                    chatPanel.SetActive(false);

                // หน่วงนิดหน่อย (กันภาพหายปุบปับ/เด้ง)
                if (trg.smallDelay > 0f) yield return new WaitForSeconds(trg.smallDelay);

                // เรียก Ink ต่อ
                if (trg.useInk)
                {
                    var dlg = DialogueManager_Test1.GetInstance();
                    if (dlg != null)
                    {
                        bool willLoadNew = !string.IsNullOrEmpty(trg.inkNameToLoad);
                        if (willLoadNew)
                        {
                            dlg.LoadNewInkAndJump(trg.inkNameToLoad, trg.jumpToKnot);
                        }
                        else
                        {
                            dlg.EnsureOpen();
                            if (!string.IsNullOrEmpty(trg.jumpToKnot))
                                dlg.JumpToKnot(trg.jumpToKnot);
                            else
                                dlg.ContinueStory(); // ไม่ระบุ knot ก็เดินต่อ
                        }
                    }
                }

                // พบแล้ว 1 รายการก็พอ (ถ้าต้องอนุญาตหลาย trigger ซ้อน ให้เอา break ออก)
                break;
            }
        }
    }


    private void HideAllChoiceButtons()
    {
        for (int i = 0; i < choiceDockButtons.Length; i++)
        {
            if (choiceDockButtons[i] == null) continue;
            choiceDockButtons[i].onClick.RemoveAllListeners();
            choiceDockButtons[i].gameObject.SetActive(false);
            choiceDockButtons[i].interactable = true;
        }
    }

    private void SetAllChoiceButtonsInteractable(bool interactable)
    {
        for (int i = 0; i < choiceDockButtons.Length; i++)
            if (choiceDockButtons[i] != null)
                choiceDockButtons[i].interactable = interactable;
    }

    private void OnAllFinished()
    {
        if (nextButtonAfterAll != null) nextButtonAfterAll.gameObject.SetActive(false);
        if (chatPanel != null) chatPanel.SetActive(false);
        //DialogueManager_Test1.GetInstance()?.OnChatFinished();


        var mgr = DialogueManager_Test1.GetInstance();
        if (mgr != null) mgr.OnChatFinished();
    }

    private IEnumerator ScrollToBottomDeferred(bool smooth = true, float duration = 0.12f)
    {
        // เคารพ pause
        while (isPaused) yield return null;

        // รอให้ Unity จบการวางเลย์เอาต์รอบนี้ก่อน
        yield return null;                 // 1 เฟรม
        yield return new WaitForEndOfFrame();

        // ระหว่างรอก็เคารพ pause
        while (isPaused) yield return null;

        // บังคับคำนวณเลย์เอาต์
        if (scrollRect != null && scrollRect.content != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);

        if (scrollRect == null) yield break;

        if (!smooth)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
            yield break;
        }

        float t = 0f;
        float start = scrollRect.verticalNormalizedPosition;
        const float target = 0f; // ล่างสุด
        while (t < duration)
        {
            while (isPaused) yield return null;

            t += Time.unscaledDeltaTime;
            float k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / duration));
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(start, target, k);
            yield return null;
        }
        scrollRect.verticalNormalizedPosition = target;
    }
    private IEnumerator ScrollToBottomStabilized(float smoothDuration = 0.12f, int settleFrames = 2)
    {
        // เคารพ pause
        while (isPaused) yield return null;

        // รอเฟรมเพื่อให้ Instantiate + Layout ทำงานรอบแรก
        yield return null;
        yield return new WaitForEndOfFrame();

        var content = scrollRect ? scrollRect.content : null;
        if (content == null) yield break;

        // เฝ้ารอจนความสูง Content “นิ่ง” ติดต่อกัน N เฟรม
        float lastHeight = -1f;
        int stableCount = 0;

        for (int guard = 0; guard < 30; guard++) // กันลูปไม่จบ (30 รอบพอ)
        {
            while (isPaused) yield return null;

            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            float h = content.rect.height;

            if (Mathf.Approximately(h, lastHeight))
                stableCount++;
            else
                stableCount = 0;

            lastHeight = h;

            if (stableCount >= settleFrames) break; // นิ่งแล้ว
            yield return null; // รอต่ออีกเฟรม
        }

        // ถึงตรงนี้ค่อยเลื่อนลงล่างสุด
        if (smoothDuration <= 0f)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
            yield break;
        }

        float t = 0f;
        float start = scrollRect.verticalNormalizedPosition;
        const float target = 0f;
        while (t < smoothDuration)
        {
            while (isPaused) yield return null;
            t += Time.unscaledDeltaTime;
            float k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / smoothDuration));
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(start, target, k);
            yield return null;
        }
        scrollRect.verticalNormalizedPosition = target;
    }

}
