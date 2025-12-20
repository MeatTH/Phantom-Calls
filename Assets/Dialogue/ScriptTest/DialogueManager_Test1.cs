using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public class DialogueManager_Test1 : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Speaker UI")]
    [SerializeField] private GameObject speakerNamePanel;      // กล่อง / พื้นหลังชื่อ (ไม่บังคับ)
    [SerializeField] private TextMeshProUGUI speakerNameText;  // ข้อความชื่อคนพูด

    [Header("Choice UI")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    [Header("Navigation UI")]
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject backButton; // ✅ ปุ่ม Back (ผูกใน Inspector)

    [Header("Ink JSON")]
    [SerializeField] private TextAsset[] inkJSON;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("Back / History Settings")]
    [SerializeField] private bool enableBack = true;      // เปิด/ปิดระบบ Back
    [SerializeField] private int maxHistory = 200;        // จำกัดจำนวนสเตทที่เก็บ (กันเมมบวม)

    // เก็บ state ของ story ก่อนจะกด Continue แต่ละครั้ง
    private List<string> stateHistory = new List<string>();
    private bool isRestoringFromHistory = false;          // flag กันไม่ให้เซฟซ้อนตอนย้อนกลับ

    // เก็บข้อความเต็มของบรรทัดปัจจุบัน (ไว้ใช้ตอนกด Back ขณะยังพิมพ์ไม่จบ)
    private string currentLineRaw = "";

    [System.Serializable]
    public class NamedPanel
    {
        public string name;
        public GameObject panel;
    }

    [Header("Custom UI Panels")]
    [SerializeField] private List<NamedPanel> customPanels;

    private Dictionary<string, GameObject> panelDict;
    private string pendingInkToLoad = null;

    private Story currentStory;
    private bool waitingForChatToFinish = false;

    private bool dialogueIsPlaying;

    private static DialogueManager_Test1 instance;

    public string CurrentStoryBaseName { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("DialogueManager_Test1: multiple instances found.");
        }
        instance = this;
    }

    public static DialogueManager_Test1 GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        // เตรียม choice text
        choicePanel.SetActive(false);

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            choice.SetActive(false);
            index++;
        }

        // เตรียม dictionary ของ custom panels
        panelDict = new Dictionary<string, GameObject>();
        foreach (NamedPanel p in customPanels)
        {
            if (!panelDict.ContainsKey(p.name))
                panelDict.Add(p.name, p.panel);
        }

        // ซ่อนชื่อคนพูดตอนเริ่ม (ถ้ามี object)
        ResetSpeakerName();

        // auto start ที่ inkJSON[0] ถ้ามี
        if (inkJSON != null && inkJSON.Length > 0)
        {
            EnterDialogueMode(inkJSON[0]);
        }

        // ซ่อนปุ่ม back ตอนเริ่ม (ยังไม่มี history)
        UpdateBackButtonVisibility();
    }

    private void Update()
    {
        // ตอนนี้การกดต่อใช้ปุ่ม/ระบบอื่น
    }

    public string CurrentStoryName { get; private set; }

    // ========== ENTER / EXIT DIALOGUE ==========

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        
        // Parse Story Name (e.g. "Story1_Scene 1-1" -> "Story1")
        CurrentStoryName = ParseStoryName(inkJSON.name);
        Debug.Log("Current Story: " + CurrentStoryName);

        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);
        Debug.Log("dialoguePanel.SetActive(true)");

        stateHistory.Clear();
        isRestoringFromHistory = false;

        CurrentStoryBaseName = inkJSON.name;

        ContinueStory();
    }

    private string ParseStoryName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return "";

        // Try Regex first: Matches "Story1", "Story2" at the start
        var match = Regex.Match(fileName, @"^(Story\d+)", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            string result = match.Groups[1].Value;
            // Ensure first letter is uppercase (Story1)
            return char.ToUpper(result[0]) + result.Substring(1);
        }
        
        // Fallback
        string[] parts = fileName.Split('_');
        string splitResult = parts.Length > 0 ? parts[0] : fileName;
        Debug.Log($"ParseStoryName: Input='{fileName}' -> Output='{splitResult}'");
        return splitResult;
    }

    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        ResetSpeakerName();

        stateHistory.Clear();
        isRestoringFromHistory = false;

        UpdateBackButtonVisibility();
    }

    // ========== MAIN FLOW (CONTINUE / BACK) ==========

    /// <summary>
    /// เรียกตอนกดปุ่ม Continue / แตะไปต่อ
    /// </summary>
    public void ContinueStory()
    {
        if (waitingForChatToFinish || isTyping) return;

        if (currentStory == null)
        {
            Debug.LogWarning("ContinueStory called but currentStory is null.");
            return;
        }

        if (currentStory.canContinue)
        {
            // ✅ เก็บ state ก่อนจะไปบรรทัดถัดไป (เฉพาะตอนเดินหน้า ไม่ใช่ตอนย้อน)
            if (!isRestoringFromHistory && enableBack)
            {
                SaveCurrentStateSnapshot();
            }

            ShowNextLine();
        }
        else
        {
            ExitDialogueMode();
        }
    }

    /// <summary>
    /// ดึงบรรทัดถัดไปจาก Ink และแสดง (ใช้ทั้งตอนเดินหน้าและตอนย้อนกลับ)
    /// </summary>
    private void ShowNextLine()
    {
        // ดึงข้อความถัดไป
        string nextLine = currentStory.Continue().Trim();
        Debug.Log("Ink line: " + nextLine);

        // ล้างชื่อคนพูดก่อนทุกบรรทัด
        ResetSpeakerName();

        // จัดการ tag ทั้งหมดของบรรทัดนี้
        foreach (string tag in currentStory.currentTags)
        {
            HandleTag(tag);
            if (SoundManager_Test1.instance != null)
                SoundManager_Test1.instance.HandleSoundTag(tag, CurrentStoryName);
        }

        // เตรียมข้อความสำหรับ typing + back
        currentLineRaw = nextLine;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText(nextLine));
    }

    /// <summary>
    /// เรียกจากปุ่ม Back (ใน Inspector: OnClick → DialogueManager_Test1.OnBackButtonPressed)
    /// </summary>
    public void OnBackButtonPressed()
    {
        if (!enableBack) return;

        // 1) ถ้ากำลังพิมพ์ทีละตัวอยู่ → ให้ skip ให้จบก่อน (ไม่ถอย)
        if (isTyping)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            isTyping = false;
            // dialogueText.text = currentLineRaw; // Text is already set in TypeText
            dialogueText.maxVisibleCharacters = int.MaxValue; // Show all

            // แสดง choices ตามปกติของบรรทัดนี้
            DisplayChoices();

            // ยังไม่เปลี่ยน state Story
            return;
        }

        // 2) ถ้าไม่มี history ให้ถอยแล้ว → ไม่ทำอะไร
        if (stateHistory.Count == 0 || currentStory == null)
        {
            Debug.Log("Back: no history to go back to.");
            return;
        }

        // 3) Calculate logic to go back
        string json = "";

        if (stateHistory.Count > 1)
        {
            // If we have more than 1 history, it means we advanced at least once.
            // The last entry is the state *before* the current line.
            // But we want to go to the *previous* line.
            // So we discard the last entry (current state) and load the one before it.
            stateHistory.RemoveAt(stateHistory.Count - 1);
            json = stateHistory[stateHistory.Count - 1]; 
            Debug.Log($"[Back] Removing last state. New Count: {stateHistory.Count}. Loading index: {stateHistory.Count - 1}");
        }
        else // Count == 1
        {
            // If only 1 history (the start state), we just replay it.
            // Do not remove it.
            json = stateHistory[0];
            Debug.Log("[Back] Replaying start state (Count=1).");
        }

        try
        {
            Debug.Log($"[Back] Loading JSON snapshot...");
            isRestoringFromHistory = true;
            currentStory.state.LoadJson(json);
            // ตอนนี้ currentStory อยู่ในสถานะ "ก่อนบรรทัดปัจจุบันหนึ่งก้าว"
            // ดังนั้นเรียก ShowNextLine() เพื่อแสดงบรรทัดนั้นอีกครั้ง
            ShowNextLine();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Back failed: " + e);
        }
        finally
        {
            isRestoringFromHistory = false;
        }

        UpdateBackButtonVisibility();
    }

    /// <summary>
    /// เซฟ state ปัจจุบันของ Ink Story เป็น JSON ไว้ใน history
    /// </summary>
    private void SaveCurrentStateSnapshot()
    {
        if (currentStory == null) return;

        string json = currentStory.state.ToJson();
        stateHistory.Add(json);
        Debug.Log($"[SaveSnapshot] Saved state. History Count: {stateHistory.Count}");

        // จำกัดขนาด history กันเมมบวม
        if (stateHistory.Count > maxHistory)
        {
            stateHistory.RemoveAt(0);
        }

        UpdateBackButtonVisibility();
    }

    private void UpdateBackButtonVisibility()
    {
        if (backButton == null) return;

        // ถ้าอยากให้ปุ่ม Back แสดงตลอดก็เปลี่ยน logic ตรงนี้ได้
        bool canGoBack = enableBack && stateHistory.Count > 0 && dialogueIsPlaying;
        backButton.SetActive(canGoBack);
    }

    // ========== CHOICES ==========

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if (currentChoices.Count > 0)
        {
            choicePanel.SetActive(true);
            continueButton.SetActive(false);
        }
        else
        {
            choicePanel.SetActive(false);
            continueButton.SetActive(true);
            return;
        }

        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("Num of Choice: " + currentChoices.Count);
        }

        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }
        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);

        foreach (GameObject choice in choices)
        {
            choice.SetActive(false);
        }
        choicePanel.SetActive(false);

        StartCoroutine(ContinueAfterFrame());
    }

    private IEnumerator ContinueAfterFrame()
    {
        yield return null;
        while (!currentStory.canContinue && currentStory.currentChoices.Count == 0)
        {
            yield return null;
        }

        ContinueStory();
    }

    // ========== PANEL TRANSITION ==========

    private void SetPanelStateWithTransition(GameObject panel, bool shouldShow, string transitionType)
    {
        PanelTransitioner pt = panel.GetComponent<PanelTransitioner>();

        if (pt != null)
        {
            if (shouldShow)
                pt.ShowPanel(transitionType);
            else
                pt.HidePanel(transitionType);
        }
        else
        {
            panel.SetActive(shouldShow);
        }
    }

    // ========== TAG HANDLING ==========

    private void HandleTag(string tag)
    {
        // 1) ชื่อคนพูด: speaker:ชื่อ
        if (tag.StartsWith("speaker:"))
        {
            string speaker = tag.Substring("speaker:".Length).Trim();
            ApplySpeakerName(speaker);
            return;
        }

        // 2) โหลด Ink ใหม่
        if (tag.StartsWith("load_ink:"))
        {
            string inkName = tag.Substring("load_ink:".Length).Trim();
            if (pendingInkToLoad != inkName)
            {
                pendingInkToLoad = inkName;
                Debug.Log("Opened Scene (pending load): " + inkName);
            }
            return;
        }

        // 3) show_panel:PanelName[:transitionType]
        if (tag.StartsWith("show_panel:"))
        {
            string fullParam = tag.Substring("show_panel:".Length);
            string[] parts = fullParam.Split(':');
            string panelName = parts[0].Trim();
            string transitionType = parts.Length > 1 ? parts[1].Trim() : "instant";

            if (panelDict.TryGetValue(panelName, out GameObject panel))
            {
                SetPanelStateWithTransition(panel, true, transitionType);
                Debug.Log($"Opened panel: {panelName} with transition: {transitionType}");

                if (panelName.StartsWith("Chat"))
                {
                    waitingForChatToFinish = true;
                }
            }
            else
            {
                Debug.LogWarning("No panel found: " + panelName);
            }
            return;
        }

        // 4) hide_panel:PanelName[:transitionType]
        if (tag.StartsWith("hide_panel:"))
        {
            string fullParam = tag.Substring("hide_panel:".Length);
            string[] parts = fullParam.Split(':');
            string panelName = parts[0].Trim();
            string transitionType = parts.Length > 1 ? parts[1].Trim() : "instant";

            if (panelDict.TryGetValue(panelName, out GameObject panel))
            {
                SetPanelStateWithTransition(panel, false, transitionType);
                Debug.Log($"Closed panel: {panelName} with transition: {transitionType}");
            }
            else
            {
                Debug.LogWarning("No panel found: " + panelName);
            }
            return;
        }

        // 5) แท็กอื่น ๆ → ให้ SoundManager ลองจัดการ (REMOVED: ShowNextLine handles this globally)
        // if (SoundManager_Test1.instance != null)
        // {
        //     SoundManager_Test1.instance.HandleSoundTag(tag, CurrentStoryBaseName);
        // }

        Debug.Log("Unhandled tag: " + tag);
    }

    // ========== SPEAKER NAME HELPERS ==========

    private void ApplySpeakerName(string speaker)
    {
        if (speakerNameText != null)
        {
            speakerNameText.text = speaker;
        }

        if (speakerNamePanel != null)
        {
            speakerNamePanel.SetActive(!string.IsNullOrEmpty(speaker));
        }
    }

    private void ResetSpeakerName()
    {
        if (speakerNameText != null)
        {
            speakerNameText.text = "";
        }
        if (speakerNamePanel != null)
        {
            speakerNamePanel.SetActive(false);
        }
    }

    // ========== CHAT / INK CONTROL (สำหรับ ChatFlow) ==========

    public void OnChatFinished()
    {
        waitingForChatToFinish = false;
        ContinueStory();
    }

    public void LoadNewInkStory(string inkName)
    {
        Debug.Log("LoadNewInkStory CALLED: " + inkName);
        TextAsset selectedInk = null;

        foreach (TextAsset ink in inkJSON)
        {
            if (ink.name == inkName)
            {
                selectedInk = ink;
                break;
            }
        }

        if (selectedInk == null)
        {
            Debug.LogError("Ink file not found: " + inkName);
            return;
        }
        currentStory = new Story(selectedInk.text);
        
        // Update Story Name
        CurrentStoryName = ParseStoryName(selectedInk.name);
        Debug.Log("Current Story: " + CurrentStoryName);

        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        stateHistory.Clear();
        isRestoringFromHistory = false;
        UpdateBackButtonVisibility();

        waitingForChatToFinish = false;
        ContinueStory();
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = text;
        dialogueText.maxVisibleCharacters = 0;

        // Iterate through length of text (mimics original timing)
        // Note: text.Length might be larger than visible chars if there are tags, but ensures completion.
        for (int i = 0; i <= text.Length; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        // Ensure fully visible at the end
        dialogueText.maxVisibleCharacters = int.MaxValue;

        isTyping = false;
        DisplayChoices();

        if (!string.IsNullOrEmpty(pendingInkToLoad))
        {
            string inkToLoad = pendingInkToLoad;
            pendingInkToLoad = null;
            LoadNewInkStory(inkToLoad);
        }
    }

    // ===== ฟังก์ชันที่ใช้กับ ChatFlow / ภายนอก =====

    public void EnsureOpen()
    {
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);
    }

    public void JumpToKnot(string knotName)
    {
        if (currentStory == null || string.IsNullOrEmpty(knotName)) return;

        waitingForChatToFinish = false;
        try
        {
            currentStory.ChoosePathString(knotName);
            ContinueStory();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JumpToKnot failed: {knotName}\n{e}");
        }
    }

    public void LoadNewInkAndJump(string inkName, string knotName = null)
    {
        TextAsset selectedInk = null;
        foreach (TextAsset ink in inkJSON)
        {
            if (ink.name == inkName)
            {
                selectedInk = ink;
                break;
            }
        }
        if (selectedInk == null)
        {
            Debug.LogError("Ink file not found: " + inkName);
            return;
        }

        currentStory = new Ink.Runtime.Story(selectedInk.text);
        
        // Update Story Name
        CurrentStoryName = ParseStoryName(selectedInk.name);
        Debug.Log("Current Story: " + CurrentStoryName);

        EnsureOpen();

        stateHistory.Clear();
        isRestoringFromHistory = false;
        UpdateBackButtonVisibility();

        CurrentStoryBaseName = inkName;
        waitingForChatToFinish = false;

        if (!string.IsNullOrEmpty(knotName))
        {
            JumpToKnot(knotName);
        }
        else
        {
            ContinueStory();
        }
    }
}
