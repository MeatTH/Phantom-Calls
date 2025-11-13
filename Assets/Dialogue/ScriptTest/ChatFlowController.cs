using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ChatFlowController : MonoBehaviour
{
    [Header("เริ่มทำงานเมื่อพาเนลเปิด (ไม่รีเซ็ต)")]
    public GameObject chatPanel;
    private bool flowStarted = false;
    private bool panelWasActive = false;
    private bool isPaused = false;

    [System.Serializable]
    public class OnSpawnTrigger
    {
        public GameObject whenThisPrefab;
        public bool closeChatPanel = true;

        [Header("เมื่อบับเบิลนี้สแปวน")]
        public bool showNextButton = false;   // ให้โชว์ปุ่ม Next ณ จุดนี้ไหม

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

        // ใหม่: ช้อยนี้จบตอนเลยไหม และอยากโชว์ปุ่ม Next ให้ผู้เล่นยืนยันปิดไหม
        public bool finishOnPick = false;
        public bool showNextButtonOnPick = true;
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

    [Header("พรีวิวกลางล่าง + ปุ่มส่ง")]
    public GameObject previewContainer;
    public TMP_Text previewText;
    public float typeCharInterval = 0.02f;
    public Button sendButton;

    [Header("ยก/ลดแถบพิมพ์")]
    public RectTransform inputDock;
    public Vector2 dockPosNormal;
    public Vector2 dockPosRaised;
    public float dockAnimDuration = 0.2f;
    public AnimationCurve dockCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("ปุ่มไปต่อ หลังทุกสล็อตจบ (ถ้าต้องการ)")]
    public Button nextButtonAfterAll;

    [Header("ปุ่ม Next เฉพาะจุด (ตัวเดียวใช้ร่วมกัน)")]
    public Button nextButton;

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

    // หยุดการไหลเมื่อมีปุ่ม Next เฉพาะจุด
    private bool flowHaltedByNextButton = false; // กำลังหยุดรอ Next
    private Slot _haltedSlot = null;             // สล็อตที่หยุดคาไว้ (จะกลับมาโชว์ช้อยส์ของสล็อตนี้)

    private enum HaltResumeMode { None, ToChoicesOfSlot, ToNextSlot, FinishAll }
    private HaltResumeMode _resumeMode = HaltResumeMode.None;
    private int _resumeNextStep = -1;

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
            nextButtonAfterAll.onClick.AddListener(OnAllFinished);
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
        while (isPaused) yield return null;

        if (slot.npcMessagePrefabs != null && slot.npcMessagePrefabs.Count > 0)
            RaiseInputDock(false);

        if (slot.npcMessagePrefabs != null && slot.npcMessagePrefabs.Count > 0)
        {
            yield return WaitWithPause(slot.npcStartDelay);
            for (int i = 0; i < slot.npcMessagePrefabs.Count; i++)
            {
                SpawnBubble(slot.npcMessagePrefabs[i]);
                yield return WaitWithPause(slot.npcInterval);

                // ถ้าถูกสั่งหยุดด้วยปุ่ม Next เฉพาะจุด ให้หยุดทันทีและรอผู้เล่น
                if (flowHaltedByNextButton)
                {
                    _haltedSlot = slot;  // จะกลับมาโชว์ช้อยส์ของสล็อตนี้เมื่อกด Next
                    yield break;         // ไม่ไป TryShowChoices ที่ท้ายฟังก์ชัน
                }
            }
        }

        if (flowHaltedByNextButton) yield break;

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
        StartCoroutine(SendFlowCo());
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

        // สแปวนบับเบิลของผู้เล่น
        SpawnBubble(_pendingChoice.playerMessagePrefab);

        // ถ้าช้อยนี้จบตอนทันที
        if (_pendingChoice.finishOnPick)
        {
            // ซ่อนสิ่งที่กดได้อื่น ๆ
            HideAllChoiceButtons();
            if (choicesPanel) choicesPanel.SetActive(false);

            if (_pendingChoice.showNextButtonOnPick && nextButton != null)
            {
                // โชว์ปุ่ม Next ให้ผู้เล่นยืนยันปิด
                flowHaltedByNextButton = true;
                _resumeMode = HaltResumeMode.FinishAll; // แตะ Next แล้วจบตอน
                nextButton.gameObject.SetActive(true);

                _pendingChoice = null;
                isPreviewBusy = false;
                yield break; // อย่าเดินต่อ
            }
            else
            {
                // ไม่ต้องการปุ่ม Next → จบทันที
                _pendingChoice = null;
                isPreviewBusy = false;
                OnAllFinished();
                yield break;
            }
        }

        // เคสปกติ: เดินต่อด้วย branch
        int branch = next;
        _pendingChoice = null;
        isPreviewBusy = false;

        yield return WaitWithPause(0.35f);
        GoToNextStep(branch);
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
            if (trg.whenThisPrefab == spawnedPrefabRef)
            {
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

                // โชว์ปุ่ม NEXT เฉพาะจุด และหยุดการไหลต่อ
                if (trg.showNextButton && nextButton != null)
                {
                    HideAllChoiceButtons();
                    if (choicesPanel) choicesPanel.SetActive(false);
                    if (sendButton) sendButton.gameObject.SetActive(false);

                    flowHaltedByNextButton = true;
                    _resumeMode = HaltResumeMode.ToChoicesOfSlot; // กลับไปโชว์ช้อยส์ของสล็อตนี้
                    nextButton.gameObject.SetActive(true);
                }
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
        if (nextButtonAfterAll) nextButtonAfterAll.gameObject.SetActive(false);
        if (chatPanel != null) chatPanel.SetActive(false);

        var mgr = DialogueManager_Test1.GetInstance();
        if (mgr != null) mgr.OnChatFinished();
    }

    private void OnNextResumeClicked()
    {
        if (nextButton != null) nextButton.gameObject.SetActive(false);
        flowHaltedByNextButton = false;

        switch (_resumeMode)
        {
            case HaltResumeMode.ToChoicesOfSlot:
                if (_haltedSlot != null) TryShowChoices(_haltedSlot);
                _haltedSlot = null;
                break;

            case HaltResumeMode.ToNextSlot:
                GoToNextStep(_resumeNextStep); // -1 = ไปสล็อตถัดไปตามลำดับ
                break;

            case HaltResumeMode.FinishAll:
                OnAllFinished(); // ปิดพาเนล/จบตอน
                break;
        }

        _resumeMode = HaltResumeMode.None;
        _resumeNextStep = -1;
    }

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
