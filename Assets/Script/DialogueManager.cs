using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public GameObject choicePanel;
    public TextMeshProUGUI dialogueText;
    public Button choice1Button;
    public Button choice2Button;
    public TextMeshProUGUI choice1Text;
    public TextMeshProUGUI choice2Text;

    private DialogueNode[] dialogues;
    private int currentIndex = 0;

    public void StartDialogue(DialogueNode[] data)
    {
        dialogues = data;
        currentIndex = 0;
        ShowDialogue(currentIndex);
    }

    void ShowDialogue(int index)
    {
        if (index >= dialogues.Length)
        {
            Debug.Log("จบบทสนทนา");
            return;
        }

        DialogueNode node = dialogues[index];
        dialogueText.text = node.message;

        if (node.hasChoices)
        {
            dialoguePanel.SetActive(false);
            choicePanel.SetActive(true);

            choice1Text.text = node.choice1Text;
            choice2Text.text = node.choice2Text;

            choice1Button.onClick.RemoveAllListeners();
            choice2Button.onClick.RemoveAllListeners();

            choice1Button.onClick.AddListener(() => OnChoiceSelected(node.choice1NextIndex));
            choice2Button.onClick.AddListener(() => OnChoiceSelected(node.choice2NextIndex));
        }
        else
        {
            dialoguePanel.SetActive(true);
            choicePanel.SetActive(false);

            // ถ้าเป็น ending
            if (node.isGoodEnding)
            {
                Debug.Log("Good Ending!");
                // เรียก Ending UI ดี
            }
            else if (index == dialogues.Length - 1)
            {
                Debug.Log("Bad Ending or default end");
                // เรียก Ending UI ไม่ดี
            }
        }
    }

    public void OnTapToContinue()
    {
        currentIndex++;
        ShowDialogue(currentIndex);
    }

    void OnChoiceSelected(int nextIndex)
    {
        currentIndex = nextIndex;
        ShowDialogue(currentIndex);
    }
}
