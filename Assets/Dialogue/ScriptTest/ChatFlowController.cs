using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChatFlowController : MonoBehaviour
{
    [Header("เริ่มทำงานเมื่อพาเนลเปิด (ไม่รีเซ็ต)")]
    public GameObject chatPanel;
    private bool flowStarted = false;
    private bool panelWasActive = false;
    private bool isPaused = false;

    // ===== Integrate with Ink / DialogueManager =====
    [Header("หลังจบแชทให้สั่ง Inkle เดินต่อหรือไม่")]
    public bool continueInkWhenClosed = true;     // เปิดไว้จะ ContinueStory() อัตโนมัติ
    [Tooltip("ถ้าต้องการ Jump ไป knot ใดโดยเฉพาะ (ปล่อยว่างเพื่อ Continue ปกติ)")]
    public string continueInkKnot = "";           // ถ้าใส่ จะ EnsureOpen()+JumpToKnot ก่อน ไม่ใส่จะ ContinueStory()

    // ===== Event ให้ภายนอกรับว่าจบแล้ว =====
    public event Action<ChatFlowController> OnChatFinishedEvent;

    [System.Serializable]
    public class OnSpawnTrigger
    {
        public GameObject whenThisPrefab;
        public bool closeChatPanel = true;

        [Header("เมื่อบับเบิลนี้สแปวน")]
        public bool showNextButton = false;      // แสดงปุ่ม Next เฉพาะจุดและหยุดคิวต่อไปชั่วคราว
        public bool endChatHere = false;         // ขึ้นบับเบิลนี้แล้ว 'จบแชททันที'
        public bool useInk = true;
        public string inkNameToLoad;
        public string jumpToKnot;
        public float smallDelay = 0.15f;
    }

    [Header("ทริกเกอร์เมื่อบับเบิลบางอันปรากฏ")]
    public List<OnSpawnTrigger> spawnTriggers = new List<OnSpawnTrigger>();

    [System.Serializable]
    public class Slot
    {
        public string name;
        public int step = 0;
        public List<GameObject> npcMessagePrefabs = new List<GameObject>();
        public float npcStartDelay = 0.4f;
        public float npcInterval = 0.8f;
        public ChoiceSet choice;
    }

    public enum DockPos { Pos0 = 0, Pos1 = 1, Pos2 = 2 }

    [System.Serializable]
    public class ChoiceSet
    {
        public List<ChoiceOption> options = new List<ChoiceOption>();
        [Tooltip("เลือกแล้วปิดปุ่มอื่น (จะถูกเพิกเฉยถ้า allowChangeBeforeSend=true)")]
        public bool destroyOthersOnPick = true;
    }

    [System.Serializable]
    public class ChoiceOption
    {
        public string buttonText;
        [TextArea] public string previewTextOverride;
        public GameObject playerMessagePrefab;
        public int nextStep = -1;
        public DockPos dockOverride = DockPos.Pos0;

        [Header("การจบบทจากช้อยส์นี้")]
        public bool finishOnPick = false;        // เลือกช้อยส์แล้ว 'จบแชททันที'
        public bool showTapToContinue = true;    // ถ้า finishOnPick=true: ให้แสดงปุ่ม 'แตะเพื่อไปต่อ/ปิด' มั้ย
    }

    [Header("คอนเทนต์ของแชท (ScrollView > Content)")]
    public Transform chatContentRoot;

    [Header("สกรอลลงล่างสุดอัตโนมัติ")]
    public ScrollRect scrollRect;

    [Header("เสียงเอฟเฟกต์ (ถ้ามี)")]
    public string sfxPopName = "chat_pop";

    [Header("กลุ่มปุ่มช้อยส์ (วางไว้ในฉากแล้ว)")]
    public GameObject choicesPanel;
    public Button[] choiceDockButtons = new Button[3];

    [Header("พรีวิวกลางล่าง + ปุ่มส่ง (New Text)")]
    public GameObject previewContainer;
    public TMP_Text previewText;
    public float typeCharInterval = 0.02f;
    public Button sendButton;

    [Header("ยก/ลดแถบพิมพ์ (New Text)")]
    public RectTransform inputDock;
    public Vector2 dockPosNormal;
    public Vector2 dockPosRaised;
    public float dockAnimDuration = 0.2f;
    public AnimationCurve dockCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("ปุ่มไปต่อ หลังทุกสล็อตจบ (ปุ่มสรุป)")]
    public Button nextButtonAfterAll;    // ปุ่มปิด chat และสั่ง ink เดินต่อ

    [Header("ปุ่ม Next เฉพาะจุด (หยุดคิวชั่วคราว)")]
    public Button nextButton;            // ปุ่มหยุด flow ชั่วคราว (เช่น “แตะเพื่อไปต่อ” กลางไทม์ไลน์)

    [Header("ไทม์ไลน์ทั้งหมด (เรียง step จากน้อยไปมาก)")]
    public List<Slot> timeline = new List<Slot>();

    [Header("อนุญาตให้เปลี่ยนช้อยได้ก่อนกดส่ง")]
    public bool allowChangeBeforeSend = true;

    // ====== State ======
    private int currentSlotIndex = 0;
    private bool isPreviewBusy = false;
    private ChoiceOption _pendingChoice = null;
    private Coroutine _dockAnimCo;
    private Coroutine _typingCo;

    // หยุด flow ชั่วคราวเพราะโชว์ nextButton เฉพาะจุด
    private bool flowHaltedByNextButton = false;
    private Slot _haltedSlot = null;

    // หยุด flow แบบ 'จบแชทแล้ว' (ไม่ควรไปต่ออีก)
    private bool flowTerminated = false;

    private void Start()
    {
        panelWasActive = (chatPanel != null && chatPanel.activeInHierarchy);
        if (panelWasActive && !flowStarted) BeginFlow();

        foreach (var s in timeline)
        {
            foreach (var npc in s.npcMessagePrefabs) if (npc) npc.SetActive(false);
            if (s.choice != null)
                foreach (var o in s.choice.options)
                    if (o.playerMessagePrefab) o.playerMessagePrefab.SetActive(false);
        }

        HideAllChoiceButtons();
        if (choicesPanel) choicesPanel.SetActive(false);

        if (inputDock) inputDock.anchoredPosition = dockPosNormal;

        if (previewContainer) previewContainer.SetActive(true);
        if (previewText) previewText.text = "";
        if (sendButton)
        {
            sendButton.gameObject.SetActive(false);
            sendButton.onClick.RemoveAllListeners();
            sendButton.onClick.AddListener(OnSendClicked);
        }

        if (nextButtonAfterAll)
        {
            nextButtonAfterAll.gameObject.SetActive(false);
            nextButtonAfterAll.onClick.RemoveAllListeners();
            nextButtonAfterAll.onClick.AddListener(HandleCloseAndContinue); // << ปรับให้ปิด+สั่ง ink
        }

        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(OnNextResumeClicked);
        }
    }

    private void Update()
    {
        bool panelIsActive = (chatPanel != null && chatPanel.activeInHierarchy);

        if (!panelWasActive && panelIsActive)
        {
            if (!flowStarted) BeginFlow();
            isPaused = false;
        }
        if (panelWasActive && !panelIsActive)
        {
            isPaused = true;
        }
        panelWasActive = panelIsActive;
    }

    private void BeginFlow()
    {
        flowStarted = true;
        RunCurrentSlot();
    }

    private void RunCurrentSlot()
    {
        if (flowTerminated) return;

        if (currentSlotIndex >= timeline.Count)
        {
            // จบ timeline -> โชว์ปุ่มสรุป (กดแล้วปิด & Inkle ต่อ)
            ShowEndButton();
            return;
        }
        var slot = timeline[currentSlotIndex];
        StartCoroutine(RevealNpcAuto(slot));
    }

    private IEnumerator RevealNpcAuto(Slot slot)
    {
        while (isPaused) yield return null;
        if (flowTerminated) yield break;

        // ถ้ามีข้อความ NPC ในสล็อตนี้ → ลดแถบพิมพ์ลง
        if (slot.npcMessagePrefabs != null && slot.npcMessagePrefabs.Count > 0)
            RaiseInputDock(false);

        if (slot.npcMessagePrefabs != null && slot.npcMessagePrefabs.Count > 0)
        {
            yield return WaitWithPause(slot.npcStartDelay);
            for (int i = 0; i < slot.npcMessagePrefabs.Count; i++)
            {
                if (flowTerminated) yield break;

                SpawnBubble(slot.npcMessagePrefabs[i]);
                yield return WaitWithPause(slot.npcInterval);

                // ถ้าถูกสั่งหยุดเพราะ nextButton เฉพาะจุด
                if (flowHaltedByNextButton)
                {
                    _haltedSlot = slot;
                    yield break;
                }
                // ถ้าถูกสั่ง 'จบแชท' จากบับเบิลนี้
                if (flowTerminated)
                    yield break;
            }
        }

        if (flowTerminated || flowHaltedByNextButton) yield break;

        TryShowChoices(slot);
    }

    private IEnumerator WaitWithPause(float seconds)
    {
        float t = 0f;
        while (t < seconds)
        {
            while (isPaused) yield return null;
            t += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    private void TryShowChoices(Slot slot)
    {
        HideAllChoiceButtons();

        if (flowTerminated) return;

        if (slot.choice == null || slot.choice.options == null || slot.choice.options.Count == 0)
        {
            GoToNextStep(-1);
            return;
        }

        if (choicesPanel) choicesPanel.SetActive(true);
        RaiseInputDock(true);

        for (int i = 0; i < choiceDockButtons.Length; i++)
            if (choiceDockButtons[i] != null)
                choiceDockButtons[i].onClick.RemoveAllListeners();

        foreach (var opt in slot.choice.options)
        {
            int dock = (int)opt.dockOverride;
            if (dock < 0 || dock >= choiceDockButtons.Length) continue;
            var btn = choiceDockButtons[dock];
            if (btn == null) continue;

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
        if (opt == null || opt.playerMessagePrefab == null) return;
        if (!allowChangeBeforeSend)
        {
            if (isPreviewBusy) return;
            if (set.destroyOthersOnPick) HideAllChoiceButtons();
            else SetAllChoiceButtonsInteractable(false);
        }
        else
        {
            SetAllChoiceButtonsInteractable(true);
        }

        isPreviewBusy = true;

        if (_typingCo != null) StopCoroutine(_typingCo);

        string txt =
            !string.IsNullOrWhiteSpace(opt.previewTextOverride) ? opt.previewTextOverride.Trim() :
            (!string.IsNullOrWhiteSpace(opt.buttonText) ? opt.buttonText : "");

        _typingCo = StartCoroutine(PreviewTypingThenEnableSend(txt, opt));
    }

    private IEnumerator PreviewTypingThenEnableSend(string textToType, ChoiceOption opt)
    {
        if (previewText) previewText.text = "";
        if (sendButton) sendButton.gameObject.SetActive(false);

        foreach (char c in textToType)
        {
            while (isPaused) yield return null;
            if (previewText) previewText.text += c;
            yield return WaitWithPause(typeCharInterval);
        }

        _pendingChoice = opt;
        if (sendButton) sendButton.gameObject.SetActive(true);
        _typingCo = null;
    }

    private void OnSendClicked()
    {
        if (previewText) previewText.text = "";
        if (!isPreviewBusy || _pendingChoice == null) return;

        // ถ้าเลือกช้อยส์นี้แล้วจบเลย
        if (_pendingChoice.finishOnPick)
        {
            StartCoroutine(FinishByChoiceCo(_pendingChoice));
            return;
        }

        StartCoroutine(SendFlowCo());
    }

    private IEnumerator FinishByChoiceCo(ChoiceOption opt)
    {
        HideAllChoiceButtons();
        if (choicesPanel) choicesPanel.SetActive(false);

        // ส่งบับเบิลผู้เล่นก่อน
        if (sendButton) sendButton.gameObject.SetActive(false);
        SpawnBubble(opt.playerMessagePrefab);

        _pendingChoice = null;
        isPreviewBusy = false;

        // แสดงปุ่มแตะเพื่อไปต่อ/ปิด ตามต้องการ
        if (opt.showTapToContinue && nextButtonAfterAll != null)
        {
            yield return WaitWithPause(0.35f);
            ShowEndButton(); // ใช้ปุ่มสรุปปิด + สั่ง ink ต่อ
        }
        else
        {
            // ปิดพาเนลและจบแชททันที
            HandleCloseAndContinue();
        }
    }

    private IEnumerator SendFlowCo()
    {
        HideAllChoiceButtons();
        if (choicesPanel) choicesPanel.SetActive(false);

        int next = _pendingChoice.nextStep;

        bool nextIsNpc = NextSlotStartsWithNpc(next);
        if (nextIsNpc)
        {
            RaiseInputDock(false);
            yield return WaitWithPause(dockAnimDuration);
        }
        else
        {
            yield return null;
        }

        if (sendButton) sendButton.gameObject.SetActive(false);

        SpawnBubble(_pendingChoice.playerMessagePrefab);

        int branch = next;
        _pendingChoice = null;
        isPreviewBusy = false;

        yield return WaitWithPause(0.35f);
        GoToNextStep(branch);
    }

    private void GoToNextStep(int branchNextStep)
    {
        if (flowTerminated) return;

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

    // ===== Utilities =====
    private void SpawnBubble(GameObject prefab)
    {
        if (prefab == null || chatContentRoot == null) return;

        var go = Instantiate(prefab, chatContentRoot);
        go.SetActive(true);

        if (!string.IsNullOrEmpty(sfxPopName) && SoundManager_Test1.instance != null)
            SoundManager_Test1.instance.PlaySFX(sfxPopName);

        if (scrollRect != null)
            StartCoroutine(ScrollToBottomStabilized());

        StartCoroutine(CheckSpawnTriggers(prefab));
    }

    private IEnumerator CheckSpawnTriggers(GameObject spawnedPrefabRef)
    {
        yield return null;

        foreach (var trg in spawnTriggers)
        {
            if (trg == null || trg.whenThisPrefab == null) continue;
            if (trg.whenThisPrefab != spawnedPrefabRef) continue;

            if (trg.closeChatPanel && chatPanel != null)
                chatPanel.SetActive(false);

            if (trg.smallDelay > 0f) yield return new WaitForSeconds(trg.smallDelay);

            if (trg.useInk)
            {
                var dlg = DialogueManager_Test1.GetInstance();
                if (dlg != null)
                {
                    bool willLoadNew = !string.IsNullOrEmpty(trg.inkNameToLoad);
                    if (willLoadNew) dlg.LoadNewInkAndJump(trg.inkNameToLoad, trg.jumpToKnot);
                    else
                    {
                        dlg.EnsureOpen();
                        if (!string.IsNullOrWhiteSpace(trg.jumpToKnot)) dlg.JumpToKnot(trg.jumpToKnot);
                        else dlg.ContinueStory();
                    }
                }
            }

            // จบแชทด้วยบับเบิลนี้
            if (trg.endChatHere)
            {
                HideAllChoiceButtons();
                if (choicesPanel) choicesPanel.SetActive(false);
                if (sendButton) sendButton.gameObject.SetActive(false);

                flowTerminated = true;
                ShowEndButton(); // ใช้ปุ่มสรุปปิด + สั่ง ink ต่อ
                yield break;
            }

            // โชว์ปุ่ม NEXT เฉพาะจุด (หยุด flow ชั่วคราว)
            if (trg.showNextButton && nextButton != null)
            {
                HideAllChoiceButtons();
                if (choicesPanel) choicesPanel.SetActive(false);
                if (sendButton) sendButton.gameObject.SetActive(false);

                flowHaltedByNextButton = true;
                nextButton.gameObject.SetActive(true);
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

    // ====== ปุ่มสรุป / ปิดแชท + สั่ง Inkle ======
    private void ShowEndButton()
    {
        flowTerminated = true; // ถือว่าจบแล้ว รอผู้เล่นกดปุ่มปิด
        if (nextButtonAfterAll == null)
        {
            // ถ้าไม่มีปุ่ม ให้ปิดทันที
            HandleCloseAndContinue();
            return;
        }

        nextButtonAfterAll.gameObject.SetActive(true);
        // ให้ชัวร์ว่า listener ถูกต้อง (ป้องกันซ้ำจาก Start)
        nextButtonAfterAll.onClick.RemoveAllListeners();
        nextButtonAfterAll.onClick.AddListener(HandleCloseAndContinue);
    }

    private void HandleCloseAndContinue()
    {
        if (nextButtonAfterAll) nextButtonAfterAll.gameObject.SetActive(false);
        TerminateChatAndClose(); // ปิด panel + แจ้ง event + ink ต่อ (ดูในฟังก์ชัน)
    }

    private void OnAllFinished() // (ไม่ถูกใช้แล้ว แต่เก็บไว้เผื่อเรียกจากภายนอก)
    {
        HandleCloseAndContinue();
    }

    private void OnNextResumeClicked()
    {
        if (nextButton != null) nextButton.gameObject.SetActive(false);

        // ถ้าถูกสั่งให้จบแชทไว้แล้ว ให้ปิดเลย
        if (flowTerminated)
        {
            HandleCloseAndContinue();
            return;
        }

        // ปลดสถานะพัก และไปต่อที่ช้อยส์ของสล็อตที่ค้างไว้
        flowHaltedByNextButton = false;

        if (_haltedSlot != null && !flowTerminated)
        {
            TryShowChoices(_haltedSlot);
            _haltedSlot = null;
        }
    }

    private void TerminateChatAndClose()
    {
        flowTerminated = true;
        if (chatPanel != null) chatPanel.SetActive(false);

        // แจ้งผู้ฟังภายนอก (เช่น ChatManager)
        OnChatFinishedEvent?.Invoke(this);

        // ควบคุม Inkle ต่อ
        if (continueInkWhenClosed)
        {
            var mgr = DialogueManager_Test1.GetInstance();
            if (mgr != null)
            {
                mgr.EnsureOpen();
                if (!string.IsNullOrWhiteSpace(continueInkKnot))
                    mgr.JumpToKnot(continueInkKnot);
                else
                    mgr.ContinueStory();
            }
        }
        else
        {
            var mgr = DialogueManager_Test1.GetInstance();
            if (mgr != null) mgr.OnChatFinished(); // แจ้งว่าแชทจบ แต่ไม่สั่งเดินเรื่อง
        }
    }

    // ===== Scroll / UI Motion =====
    private IEnumerator ScrollToBottomStabilized(float smoothDuration = 0.12f, int settleFrames = 2)
    {
        while (isPaused) yield return null;
        yield return null;
        yield return new WaitForEndOfFrame();

        var content = scrollRect ? scrollRect.content : null;
        if (content == null) yield break;

        float lastHeight = -1f;
        int stableCount = 0;

        for (int guard = 0; guard < 30; guard++)
        {
            while (isPaused) yield return null;
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            float h = content.rect.height;

            if (Mathf.Approximately(h, lastHeight)) stableCount++;
            else stableCount = 0;

            lastHeight = h;
            if (stableCount >= settleFrames) break;
            yield return null;
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

    private void RaiseInputDock(bool raised)
    {
        if (inputDock == null) return;
        if (_dockAnimCo != null) StopCoroutine(_dockAnimCo);
        _dockAnimCo = StartCoroutine(AnimateDock(raised ? dockPosRaised : dockPosNormal));
    }

    private IEnumerator AnimateDock(Vector2 targetPos)
    {
        Vector2 start = inputDock.anchoredPosition;
        float t = 0f;
        while (t < dockAnimDuration)
        {
            while (isPaused) yield return null;
            t += Time.unscaledDeltaTime;
            float k = dockCurve.Evaluate(Mathf.Clamp01(t / dockAnimDuration));
            inputDock.anchoredPosition = Vector2.LerpUnclamped(start, targetPos, k);
            yield return null;
        }
        inputDock.anchoredPosition = targetPos;
    }

    private bool NextSlotHasPlayerChoices(int nextStep)
    {
        int idx = (nextStep >= 0)
            ? timeline.FindIndex(s => s.step == nextStep)
            : currentSlotIndex + 1;

        if (idx < 0 || idx >= timeline.Count) return false;

        var ch = timeline[idx].choice;
        return ch != null && ch.options != null && ch.options.Count > 0;
    }

    private bool NextSlotStartsWithNpc(int nextStep)
    {
        int idx = (nextStep >= 0)
            ? timeline.FindIndex(s => s.step == nextStep)
            : currentSlotIndex + 1;

        if (idx < 0 || idx >= timeline.Count) return false;

        var s = timeline[idx];
        return s.npcMessagePrefabs != null && s.npcMessagePrefabs.Count > 0;
    }
}
