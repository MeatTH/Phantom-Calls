using UnityEngine;

public class DialogueLoader
{
    public static DialogueNode[] LoadStory(int storyIndex)
    {
        string path = "Dialogues/Story" + (storyIndex + 1); // �� Dialogues/Story1.json
        TextAsset jsonFile = Resources.Load<TextAsset>(path);

        if (jsonFile != null)
        {
            return JsonHelper.FromJson<DialogueNode>(jsonFile.text);
        }
        else
        {
            Debug.LogError("��辺��� JSON: " + path);
            return new DialogueNode[0];
        }
    }
}
