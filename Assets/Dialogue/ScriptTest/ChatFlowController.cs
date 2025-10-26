﻿using UnityEngine;
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

    [Header("ปุ่มไปต่อ หลังทุกสล็อตจบ (ถ้าต้องการ)")]
    public Button nextButtonAfterAll;

    [Header("ไทม์ไลน์ทั้งหมด (เรียง step จากน้อยไปมาก)")]
    public List<Slot> timeline = new List<Slot>();

    [Header("อนุญาตให้เปลี่ยนช้อยได้ก่อนกดส่ง")]
    public bool allowChangeBeforeSend = true;

    // ====== State ======
    private int currentSlotIndex = 0;
    private bool isPreviewBusy = false;          // อยู่ในโหมดพรีวิว/รอส่ง
    private ChoiceOption _pendingChoice = null;  /// ตัวเลือก “ล่าสุด” ที่จะถูกส่ง
    private Coroutine _dockAnimCo;
    private Coroutine _typingCo;                 // ควบคุม coroutine พิมพ์พรีวิว

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

        // ถ้าสล็อตนี้มีข้อความ NPC → ลดแถบทันที (กันเคสเข้ามาสล็อตนี้โดยตรง)
        if (slot.npcMessagePrefabs != null && slot.npcMessagePrefabs.Count > 0)
            RaiseInputDock(false);

        if (slot.npcMessagePrefabs != null && slot.npcMessagePrefabs.Count > 0)
        {
            yield return WaitWithPause(slot.npcStartDelay);
            for (int i = 0; i < slot.npcMessagePrefabs.Count; i++)
            {
                SpawnBubble(slot.npcMessagePrefabs[i]);
                yield return WaitWithPause(slot.npcInterval);
            }
        }

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

        // อนุญาตให้เปลี่ยนช้อยได้เสมอ: ไม่ล็อกปุ่ม, ไม่ซ่อนปุ่มที่เหลือ
        if (!allowChangeBeforeSend)
        {
            if (isPreviewBusy)
                return; // โหมดเดิม: ไม่ให้เปลี่ยนถ้ายังไม่ส่ง
            if (set.destroyOthersOnPick) HideAllChoiceButtons();
            else SetAllChoiceButtonsInteractable(false);
        }
        else
        {
            // โหมดใหม่: ปุ่มอื่นยังกดได้อยู่
            SetAllChoiceButtonsInteractable(true);
        }

        // เข้าสู่โหมดพรีวิว (ถ้ายังไม่เข้า)
        isPreviewBusy = true;

        // หยุดพิมพ์เดิม (ถ้ามี), เริ่มพิมพ์ใหม่ตามช้อย “ล่าสุด”
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
        _typingCo = null; // พิมพ์เสร็จ
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

        // ถ้าสล็อตถัดไปเริ่มด้วย NPC → ลดแถบลง, ไม่งั้นคงแถบไว้ (เพราะจะเป็นเทิร์นผู้เล่นอีก)
        bool nextIsNpc = NextSlotStartsWithNpc(next);
        if (nextIsNpc)
        {
            RaiseInputDock(false);
            yield return WaitWithPause(dockAnimDuration);
        }
        else
        {
            yield return null; // คงแถบไว้
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
                            if (!string.IsNullOrEmpty(trg.jumpToKnot)) dlg.JumpToKnot(trg.jumpToKnot);
                            else dlg.ContinueStory();
                        }
                    }
                }
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
        if (nextButtonAfterAll) nextButtonAfterAll.gameObject.SetActive(false);
        if (chatPanel != null) chatPanel.SetActive(false);

        var mgr = DialogueManager_Test1.GetInstance();
        if (mgr != null) mgr.OnChatFinished();
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
        // หา index ของสล็อตถัดไปจริง ๆ
        int idx = (nextStep >= 0)
            ? timeline.FindIndex(s => s.step == nextStep)
            : currentSlotIndex + 1;

        if (idx < 0 || idx >= timeline.Count) return false;

        var ch = timeline[idx].choice;
        return ch != null && ch.options != null && ch.options.Count > 0; // ≥ 1 ตัวเลือก
    }
    private bool NextSlotStartsWithNpc(int nextStep)
    {
        // คำนวณ index ของสล็อตถัดไปจริง ๆ
        int idx = (nextStep >= 0)
            ? timeline.FindIndex(s => s.step == nextStep)
            : currentSlotIndex + 1;

        if (idx < 0 || idx >= timeline.Count) return false;

        var s = timeline[idx];
        return s.npcMessagePrefabs != null && s.npcMessagePrefabs.Count > 0; // NPC จะขึ้นก่อนเสมอ
    }


}
