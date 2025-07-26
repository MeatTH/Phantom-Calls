using System.Collections;
using UnityEngine;

public class CallController : MonoBehaviour
{
    [SerializeField] private GameObject callPanel;
    [SerializeField] private float callRepeatDelay = 1f; 

    private DialogueManager_Test1 dialogueManager;

    private void Start()
    {
        callPanel.SetActive(false);
        dialogueManager = DialogueManager_Test1.GetInstance();
        //ShowIncomingCall();
    }

    public void ShowIncomingCall()
    {
        callPanel.SetActive(true);
    }

    public void OnAcceptCall()
    {
        callPanel.SetActive(false);
        dialogueManager.ContinueStory();
    }

    public void OnDeclineCall()
    {
        callPanel.SetActive(false);
        StartCoroutine(CallAgain());
    }

    private IEnumerator CallAgain()
    {
        yield return new WaitForSeconds(callRepeatDelay);
        ShowIncomingCall();
    }
}
