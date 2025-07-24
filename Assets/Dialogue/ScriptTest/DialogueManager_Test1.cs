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
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    [Header("Continue UI")]
    [SerializeField] private GameObject continueButton;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset[] inkJSON;

    [Header("Custom UI Panels")]
    [SerializeField] private GameObject chatPanel;



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

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
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
            string nextLine = currentStory.Continue();
            Debug.Log("Ink line: " + nextLine); 
            dialogueText.text = nextLine;
            foreach (string tag in currentStory.currentTags)
            {
                HandleTag(tag);
            }

            DisplayChoices();
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
            continueButton.SetActive(false);
        }
        else
        {
            continueButton.SetActive(true);
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
        ContinueStory();
    }

    private void HandleTag(string tag)
    {
        if (tag.StartsWith("load_ink:"))
        {
            string inkName = tag.Substring("load_ink:".Length);
            LoadNewInkStory(inkName);
            return;
        }
        switch (tag)
        {
            case "Show_Chat2-1":
                chatPanel.SetActive(true);
                waitingForChatToFinish = true;
                break;

            case "hide_chat":
                chatPanel.SetActive(false);
                break;

            default:
                Debug.Log("Unhandled tag: " + tag);
                chatPanel.SetActive(false);
                break;
        }
    }
    public void OnChatFinished()
    {
        waitingForChatToFinish = false;
        ContinueStory(); // ✨ ไปต่อเนื้อเรื่องทันที
    }

    public void LoadNewInkStory(string inkName)
    {
        TextAsset selectedInk = null;

        foreach (TextAsset ink in inkJSON)
        {
            if (ink.name == inkName) // ต้องตั้งชื่อไฟล์ให้ตรง เช่น Scene2-1
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


}
