using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger_Test1 : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private GameObject visual;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    private static DialogueTrigger_Test1 instance;
    public static DialogueTrigger_Test1 GetInstance()
    {
        return instance;
    }
    private void Awake()
    {
        //visual.SetActive(false);
    }
    private void Update()
    {

    }
    public void TriggerDialogue()
    {
        DialogueManager_Test1.GetInstance().EnterDialogueMode(inkJSON);
    }
}
