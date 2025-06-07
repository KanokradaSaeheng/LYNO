    using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
    public float moveDistance = 1f;
    public float moveSpeed = 5f;
    public CameraFollow cameraFollow;

    private bool isMoving = false;
    public bool is2DMode = false;
    private Vector3 targetPos;

    void Start()
    {
        targetPos = transform.position;
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        if (isMoving) return;


        Vector3 direction = GetTouchDirection();    // For Android build

    
        if (direction != Vector3.zero)
        {
            Debug.Log("start moving");
            targetPos = transform.position + direction * moveDistance;
            StartCoroutine(MoveToPosition(targetPos));
        }
        if (Input.touchCount > 0)
        {
            Vector2 touchPos = Input.GetTouch(0).position;
            Debug.Log("TOUCH DETECTED at: " + touchPos);
        }

    }

   Vector3 GetTouchDirection()
{
    if (Input.touchCount == 0) return Vector3.zero;

    Vector2 touchPos = Input.GetTouch(0).position;
    float screenWidth = Screen.width;
    float screenHeight = Screen.height;

    // Zones
    bool isTop = touchPos.y > screenHeight * 0.75f;
    bool isBottom = touchPos.y < screenHeight * 0.25f;
    bool isLeft = touchPos.x < screenWidth * 0.25f && touchPos.y >= screenHeight * 0.25f && touchPos.y <= screenHeight * 0.75f;
    bool isRight = touchPos.x > screenWidth * 0.75f && touchPos.y >= screenHeight * 0.25f && touchPos.y <= screenHeight * 0.75f;

    if (is2DMode && cameraFollow != null)   
    {
        int viewIndex = cameraFollow.currentViewIndex;

        if (isLeft)
        {
            return viewIndex switch
            {
                0 => Vector3.right,
                1 => Vector3.forward,
                2 => Vector3.left,
                3 => Vector3.back,
                _ => Vector3.zero
            };
        }
        else if (isRight)
        {
            return viewIndex switch
            {
                0 => Vector3.left,
                1 => Vector3.back,
                2 => Vector3.right,
                3 => Vector3.forward,
                _ => Vector3.zero
            };
        }
    
       
        return Vector3.zero;
    }

    else
    {
        float yRot = Camera.main.transform.eulerAngles.y;
        float[] angles = { 45f, 135f, 225f, 315f };
        float closest = 0f;
        float minDiff = float.MaxValue;

        foreach (float angle in angles)
        {
            float diff = Mathf.Abs(Mathf.DeltaAngle(yRot, angle));
            if (diff < minDiff)
            {
                minDiff = diff;
                closest = angle;
            }
        }

        if (isLeft)
        {
            return closest switch
            {
                45f => Vector3.forward,
                135f => Vector3.right,
                225f => Vector3.back,
                315f => Vector3.left,
                _ => Vector3.zero
            };
        }
        else if (isRight)
        {
            return closest switch
            {
                45f => Vector3.back,
                135f => Vector3.left,
                225f => Vector3.forward,
                315f => Vector3.right,
                _ => Vector3.zero
            };
        }
        else if (isTop)
        {
            return closest switch
            {
                45f => Vector3.right,
                135f => Vector3.back,
                225f => Vector3.left,
                315f => Vector3.forward,
                _ => Vector3.zero
            };
        }
        else if (isBottom)
        {
            return closest switch
            {
                45f => Vector3.left,
                135f => Vector3.forward,
                225f => Vector3.right,
                315f => Vector3.back,
                _ => Vector3.zero
            };
        }
    }

    return Vector3.zero;
}

    


    

    IEnumerator MoveToPosition(Vector3 destination)
    {
        isMoving = true;
        Debug.Log("isMoving = true");
        while (Vector3.Distance(transform.position, destination) > 0.01f)
        {
            Debug.Log("isMoving...");
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
            yield return null;
        }
        Debug.Log("isMoving = false");
        transform.position = destination;
        isMoving = false;
    }
}
