using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;

public class DialogueManager_Test1 : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Choice UI")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    [Header("Continue UI")]
    [SerializeField] private GameObject continueButton;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset[] inkJSON;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    [SerializeField] private float typingSpeed = 0.05f;

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
    //private bool dialogueIsPlaying { get; private set; }

    private static DialogueManager_Test1 instance;
 
    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("...");
        }
        instance = this;
    }

    public static DialogueManager_Test1 GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        //ContinueStory();
        //dialogueIsPlaying = true;
        //dialogueIsPlaying = false;
        //dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            choice.SetActive(false);
            index++;
        }

        panelDict = new Dictionary<string, GameObject>();
        foreach (NamedPanel p in customPanels)
        {
            if (!panelDict.ContainsKey(p.name))
                panelDict.Add(p.name, p.panel);
        }

        if (inkJSON != null && inkJSON.Length > 0)
        {
            EnterDialogueMode(inkJSON[0]);
        }


    }



    private void Update()
    {
        /*if (!dialogueIsPlaying)
        {
            return;
        }*/
        /*
        if (DialogueTrigger_Test1())
        {
            ContinueStory();
        }*/
        /*if (InputManager.GetInstance().GetSubmitPressed())
        {
            ContinueStory();
        }*/
    }
    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);
        Debug.Log("dialoguePanel.SetActive(true)");

        ContinueStory();
    }
    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";

        //ContinueStory();
    }

    public void ContinueStory()
    {
        if (waitingForChatToFinish) return;

        if (currentStory.canContinue)
        {
            //dialogueText.text = currentStory.Continue();
            string nextLine = currentStory.Continue().Trim();
            Debug.Log("Ink line: " + nextLine);

            foreach (string tag in currentStory.currentTags)
            {
                HandleTag(tag);
                SoundManager_Test1.instance.HandleSoundTag(tag);
            }
            
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeText(nextLine));

            //DisplayChoices();
        }
        else
        {
            ExitDialogueMode();
        }
    }

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
        foreach(Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }
        for (int i = index; i < choices.Length; i++)
        {
            //choicePanel.SetActive(false);
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


    private void HandleTag(string tag)
    {
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

        if (tag.StartsWith("show_panel:"))
        {
            string panelName = tag.Substring("show_panel:".Length);
            if (panelDict.TryGetValue(panelName, out GameObject panel))
            {
                panel.SetActive(true);
                Debug.Log("Opened panel: " + panelName);

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

        if (tag.StartsWith("hide_panel:"))
        {
            string panelName = tag.Substring("hide_panel:".Length);
            if (panelDict.TryGetValue(panelName, out GameObject panel))
            {
                panel.SetActive(false);
                Debug.Log("Closed panel: " + panelName);
            }
            return;
        }
        if (SoundManager_Test1.instance != null)
        {
            SoundManager_Test1.instance.HandleSoundTag(tag);
        }

        Debug.Log("Unhandled tag: " + tag);
    }

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
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);
        ContinueStory();
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        DisplayChoices();

        if (!string.IsNullOrEmpty(pendingInkToLoad))
        {
            string inkToLoad = pendingInkToLoad;
            pendingInkToLoad = null;
            yield return null; 
            LoadNewInkStory(inkToLoad);
        }
    }

}
