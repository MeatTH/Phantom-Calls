using UnityEngine;

public class GameManager : MonoBehaviour
{
    public DialogueManager dialogueManager;
    void Start()
    {
        int storyIndex = StoryLoader.selectedStoryIndex;
        int selectedIndex = StoryLoader.selectedStoryIndex;
        DialogueNode[] story = DialogueLoader.LoadStory(selectedIndex);
        dialogueManager.StartDialogue(story);

        switch (storyIndex)
        {
            case 0:
                Debug.Log("เล่น Story 1");
                // เรียกโหลด dialogue ของ Story 1
                break;
            case 1:
                Debug.Log("เล่น Story 2");
                // เรียกโหลด dialogue ของ Story 2
                break;
            case 2:
                Debug.Log("เล่น Story 3");
                // เรียกโหลด dialogue ของ Story 3
                break;
            default:
                Debug.LogWarning("ยังไม่ได้เลือกเรื่อง!");
                break;
        }
    }

}
