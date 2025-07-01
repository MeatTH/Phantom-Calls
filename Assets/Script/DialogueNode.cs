using UnityEngine;

[System.Serializable]
public class DialogueNode
{
    public string message;
    public bool hasChoices;
    public string choice1Text;
    public int choice1NextIndex;
    public string choice2Text;
    public int choice2NextIndex;
    public bool isGoodEnding;
}
