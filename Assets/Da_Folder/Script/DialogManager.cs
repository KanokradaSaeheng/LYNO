using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogManager : MonoBehaviour
{
    [System.Serializable]
    public class DialogLine
    {
        public RectTransform boxTransform;      // RectTransform of the box (UI Panel)
        public TextMeshProUGUI dialogText;      // TMP text inside the box
        public string sentence;                 // The dialog sentence
    }

    public List<DialogLine> conversation = new List<DialogLine>();
    public float typingSpeed = 0.04f;
    public float slideSpeed = 1000f;           // pixels/second

    private int currentIndex = 0;
    private bool isTyping = false;
    private bool lineFinished = false;

    private RectTransform currentBox = null;
    private RectTransform previousBox = null;

    private bool conversationActive = false;

    public Vector2 onScreenPosition = new Vector2(0, -200);    // Where the box should appear
    public Vector2 offScreenLeft = new Vector2(-1500, -200);   // Off-screen left
    public Vector2 offScreenRight = new Vector2(1500, -200);   // Off-screen right

    public void StartConversation()
    {
        currentIndex = 0;
        conversationActive = true;

        // Move all boxes off-screen at start
        foreach (var line in conversation)
        {
            line.boxTransform.anchoredPosition = offScreenRight;
        }

        ShowNextLine();
    }

    void Update()
    {
        if (!conversationActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                conversation[currentIndex].dialogText.text = conversation[currentIndex].sentence;
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
        DialogLine line = conversation[currentIndex];
        previousBox = currentBox;
        currentBox = line.boxTransform;

        // Slide out previous box if speaker changed
        if (previousBox != null && previousBox != currentBox)
        {
            StartCoroutine(SlideOut(previousBox));
        }

        // Slide in new box
        StartCoroutine(SlideIn(currentBox));

        // Type the text
        StartCoroutine(TypeLine(line));
    }

    IEnumerator TypeLine(DialogLine line)
    {
        isTyping = true;
        lineFinished = false;
        line.dialogText.text = "";

        foreach (char c in line.sentence.ToCharArray())
        {
            line.dialogText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        lineFinished = true;
    }

    IEnumerator SlideIn(RectTransform box)
    {
        while (Vector2.Distance(box.anchoredPosition, onScreenPosition) > 1f)
        {
            box.anchoredPosition = Vector2.MoveTowards(box.anchoredPosition, onScreenPosition, slideSpeed * Time.deltaTime);
            yield return null;
        }
        box.anchoredPosition = onScreenPosition;
    }

    IEnumerator SlideOut(RectTransform box)
    {
        while (Vector2.Distance(box.anchoredPosition, offScreenLeft) > 1f)
        {
            box.anchoredPosition = Vector2.MoveTowards(box.anchoredPosition, offScreenLeft, slideSpeed * Time.deltaTime);
            yield return null;
        }
        box.anchoredPosition = offScreenLeft;
    }

    void EndConversation()
    {
        if (currentBox != null)
        {
            StartCoroutine(SlideOut(currentBox));
        }

        currentBox = null;
        previousBox = null;
        conversationActive = false;
    }
}
