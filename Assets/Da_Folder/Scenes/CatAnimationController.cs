using UnityEngine;

public class CatAnimationController : MonoBehaviour
{
    public Animator catAnimator; // Drag the CatModel's Animator here

    [Tooltip("How far from center is considered 'left' or 'right' tap")]
    public float screenSplit = 0.5f; // 0.5 = exact half of the screen

    private bool isWaitingToPlay = false;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && !isWaitingToPlay)
            {
                float screenX = touch.position.x / Screen.width;

                if (screenX < screenSplit || screenX > screenSplit)
                {
                    // Left or Right tap detected
                    catAnimator.SetTrigger("Walk");
                    isWaitingToPlay = true; // Prevent multi-trigger from holding
                }
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isWaitingToPlay = false;
            }
        }
    }
}
