using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterWithHTMLStrings : MonoBehaviour
{
    private TextMeshProUGUI dialogueText;
    private string fullText;
    public float typingSpeed = 0.05f;

    private void Awake()
    {
        dialogueText = GetComponent<TextMeshProUGUI>();
        fullText = dialogueText.text;
    }

    private void Start()
    {
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        dialogueText.text = "";  // เคลียร์ข้อความก่อน
        foreach (char letter in fullText.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
