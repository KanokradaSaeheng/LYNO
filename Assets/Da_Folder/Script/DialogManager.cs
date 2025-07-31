using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogManager : MonoBehaviour
{
    [System.Serializable]
    public class DialogLine
    {
        public string speakerName;
        public TextMeshProUGUI dialogBox;
        public string sentence;
    }

    public List<DialogLine> conversation = new List<DialogLine>();
    public float typingSpeed = 0.05f;

    private int currentIndex = 0;
    private bool isTyping = false;
    private bool lineFinished = false;

    private TextMeshProUGUI currentBox;

    public void StartConversation()
    {
        currentIndex = 0;
        ShowNextLine();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && currentBox != null)
        {
            if (isTyping)
            {
                StopAllCoroutines();
                currentBox.text = conversation[currentIndex].sentence;
                isTyping = false;
                lineFinished = true;
            }
            else if (lineFinished)
            {
                currentIndex++;
                if (currentIndex < conversation.Count)
                {
                    ShowNextLine();
                }
                else
                {
                    EndConversation();
                }
            }
        }
    }

    void ShowNextLine()
    {
        // Hide previous box if switching
        if (currentBox != null && conversation[currentIndex].dialogBox != currentBox)
            currentBox.transform.parent.gameObject.SetActive(false); // Assumes text is inside a box

        currentBox = conversation[currentIndex].dialogBox;
        currentBox.transform.parent.gameObject.SetActive(true); // Show new box

        StartCoroutine(TypeLine(conversation[currentIndex].sentence));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        lineFinished = false;
        currentBox.text = "";

        foreach (char c in line.ToCharArray())
        {
            currentBox.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        lineFinished = true;
    }

    void EndConversation()
    {
        if (currentBox != null)
            currentBox.transform.parent.gameObject.SetActive(false);
        currentBox = null;
    }
}
