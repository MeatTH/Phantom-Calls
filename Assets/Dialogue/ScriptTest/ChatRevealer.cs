using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ChatRevealer : MonoBehaviour
{
    public GameObject[] chatTexts;
    public GameObject chatPanel;
    private int currentIndex = 0;
    [SerializeField] private ScrollRect scrollRect;

    void Start()
    {
        foreach (var text in chatTexts)
        {
            text.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        
        if (chatPanel.activeInHierarchy && Input.GetMouseButtonDown(0)) // หรือจะใช้ Input.touch ก็ได้
        {
            ShowNextLine();
        }
    }

    void ShowNextLine()
    {
        if (currentIndex < chatTexts.Length)
        {
            chatTexts[currentIndex].gameObject.SetActive(true);
            SoundManager_Test1.instance.PlaySFX("chat_pop");
            currentIndex++;
        }
        else
        {
            Debug.Log("Chat finished, calling DialogueManager...");
            chatPanel.SetActive(false);
            DialogueManager_Test1.GetInstance().OnChatFinished();
            //DialogueManager_Test1.GetInstance().ContinueStory();
        }
    }
   

}
