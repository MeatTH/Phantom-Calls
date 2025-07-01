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
                Debug.Log("��� Story 1");
                // ���¡��Ŵ dialogue �ͧ Story 1
                break;
            case 1:
                Debug.Log("��� Story 2");
                // ���¡��Ŵ dialogue �ͧ Story 2
                break;
            case 2:
                Debug.Log("��� Story 3");
                // ���¡��Ŵ dialogue �ͧ Story 3
                break;
            default:
                Debug.LogWarning("�ѧ��������͡����ͧ!");
                break;
        }
    }

}
