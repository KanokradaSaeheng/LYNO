using UnityEngine;
using TMPro;
using System.Collections;

public class DialogSystem : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI dialogText; // Assign your TMP text component
    public string[] dialogLines;       // List of dialog sentences

    [Header("Typewriter Settings")]
    public float typingSpeed = 0.05f;

    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool lineFinished = false;

    void Start()
    {
        StartCoroutine(TypeLine(dialogLines[currentLineIndex]));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogText.text = dialogLines[currentLineIndex];
                isTyping = false;
                lineFinished = true;
            }
            else if (lineFinished)
            {
                currentLineIndex++;
                if (currentLineIndex < dialogLines.Length)
                {
                    StartCoroutine(TypeLine(dialogLines[currentLineIndex]));
                }
                else
                {
                    dialogText.text = ""; // End of dialog
                }
            }
        }
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        lineFinished = false;
        dialogText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        lineFinished = true;
    }
}
