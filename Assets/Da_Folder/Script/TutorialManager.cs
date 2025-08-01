using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialMove;
    public GameObject tutorialAngle;
    public GameObject tutorialView;
    public GameObject tutorialGoodluck;

    private int tutorialStep = 0;
    private Vector2 lastTouchPosition;
    private float doubleTapTime = 0.3f;
    private float lastTapTime = 0f;

    // For improved swipe detection
    private Vector2 swipeStartPos;
    private bool swipeInProgress = false;

    private void Start()
    {
        ShowStep(0);
    }

    private void Update()
    {
        switch (tutorialStep)
        {
            case 0: // Detect directional tap for movement
                if (DetectDirectionalTap())
                {
                    NextTutorial();
                }
                break;

            case 1: // Detect swipe on left or right half only
                if (DetectSwipeOnSides())
                {
                    NextTutorial();
                }
                break;

            case 2: // Detect double tap only in middle area
                if (DetectDoubleTapInMiddle())
                {
                    NextTutorial();
                }
                break;

            case 3: // Tap anywhere to dismiss good luck
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    tutorialGoodluck.SetActive(false);
                    tutorialStep++; // Tutorial finished
                }
                break;
        }
    }

    private bool DetectDirectionalTap()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 tapPosition = Input.GetTouch(0).position;
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            // Define tap zones for directional movement: top, bottom, left, right
            if (tapPosition.y > screenHeight * 0.75f)      // Top 25%
                return true;
            if (tapPosition.y < screenHeight * 0.25f)      // Bottom 25%
                return true;
            if (tapPosition.x < screenWidth * 0.25f)       // Left 25%
                return true;
            if (tapPosition.x > screenWidth * 0.75f)       // Right 25%
                return true;
        }
        return false;
    }

    private bool DetectSwipeOnSides()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            float screenWidth = Screen.width;

            // Only consider touches starting on left or right half
            if (touch.phase == TouchPhase.Began)
            {
                if (touch.position.x < screenWidth * 0.5f || touch.position.x > screenWidth * 0.5f)
                {
                    swipeStartPos = touch.position;
                    swipeInProgress = true;
                }
            }
            else if (touch.phase == TouchPhase.Ended && swipeInProgress)
            {
                Vector2 swipeEndPos = touch.position;
                Vector2 delta = swipeEndPos - swipeStartPos;

                swipeInProgress = false;

                // Check horizontal swipe: distance over threshold & mostly horizontal
                if (Mathf.Abs(delta.x) > 50f && Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    return true;
                }
            }
            else if (touch.phase == TouchPhase.Canceled)
            {
                swipeInProgress = false;
            }
        }
        return false;
    }

    private bool DetectDoubleTapInMiddle()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 tapPosition = Input.GetTouch(0).position;
            float screenWidth = Screen.width;

            // Define middle 50% horizontally
            float leftBoundary = screenWidth * 0.25f;
            float rightBoundary = screenWidth * 0.75f;

            if (tapPosition.x > leftBoundary && tapPosition.x < rightBoundary)
            {
                float timeSinceLastTap = Time.time - lastTapTime;
                lastTapTime = Time.time;

                if (timeSinceLastTap < doubleTapTime)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void NextTutorial()
    {
        tutorialStep++;
        ShowStep(tutorialStep);
    }

    private void ShowStep(int step)
    {
        tutorialMove.SetActive(false);
        tutorialAngle.SetActive(false);
        tutorialView.SetActive(false);
        tutorialGoodluck.SetActive(false);

        switch (step)
        {
            case 0:
                tutorialMove.SetActive(true);
                break;
            case 1:
                tutorialAngle.SetActive(true);
                break;
            case 2:
                tutorialView.SetActive(true);
                break;
            case 3:
                tutorialGoodluck.SetActive(true);
                break;
        }
    }
}
