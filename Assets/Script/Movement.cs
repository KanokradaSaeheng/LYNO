using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
    public float moveDistance = 1f;
    public float moveSpeed = 5f;
    public CameraFollow cameraFollow;
    public int active2DOffsetIndex;
    public float illusionScreenDistance = 100f;
    public float illusionTolerance = 15f;
    public Transform player;

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

        Vector3 direction = GetTouchDirection();

        if (direction != Vector3.zero)
        {
            if (CanMoveTo(direction))
            {
                targetPos = transform.position + direction * moveDistance;
                StartCoroutine(MoveToPosition(targetPos));
            }
        }
    }

    bool CanMoveTo(Vector3 direction)
    {
        Vector3 nextPos = transform.position + direction * moveDistance;

        if (!IsBlocked(nextPos))
        {
            Debug.Log("✔ Real platform found.");
            return true;
        }
        else if (is2DMode && CanMoveByIllusion(direction))
        {
            Debug.Log("✨ Illusion platform allowed.");
            return true;
        }

        Debug.Log("❌ Move blocked.");
        return false;
    }

    bool IsBlocked(Vector3 destination)
    {
        Vector3 rayOrigin = destination + Vector3.up * 0.5f;
        Vector3 rayDirection = Vector3.down;
        float rayLength = 2f;

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rayLength))
        {
            if (hit.collider.CompareTag("Platform"))
            {
                Debug.DrawLine(rayOrigin, hit.point, Color.green, 1f);
                return false;
            }
        }

        Debug.DrawLine(rayOrigin, rayOrigin + rayDirection * rayLength, Color.red, 1f);
        return true;
    }

    bool CanMoveByIllusion(Vector3 moveDirection)
    {
        Vector3 playerScreen = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 0.5f);
        Vector3 targetWorld = transform.position + moveDirection * moveDistance;
        Vector3 targetScreen = Camera.main.WorldToScreenPoint(targetWorld);
        Vector2 screenDirection = (targetScreen - playerScreen).normalized;

        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");

        foreach (GameObject platform in platforms)
        {
            Vector3 platformScreen = Camera.main.WorldToScreenPoint(platform.transform.position);
            Vector2 expectedScreen = new Vector2(playerScreen.x, playerScreen.y) + screenDirection * illusionScreenDistance;

            if (Vector2.Distance(platformScreen, expectedScreen) < illusionTolerance)
            {
                Debug.DrawLine(playerScreen, platformScreen, Color.green, 1f);
                return true;
            }

            Debug.DrawLine(playerScreen, platformScreen, Color.red, 0.2f);
        }

        return false;
    }

    Vector3 GetTouchDirection()
    {
        if (Input.touchCount == 0) return Vector3.zero;

        Vector2 touchPos = Input.GetTouch(0).position;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        bool isTop = touchPos.y > screenHeight * 0.75f;
        bool isBottom = touchPos.y < screenHeight * 0.25f;
        bool isLeft = touchPos.x < screenWidth * 0.25f;
        bool isRight = touchPos.x > screenWidth * 0.75f;

        if (is2DMode)
        {
            switch (active2DOffsetIndex)
            {
                case 0: if (isLeft) return Vector3.back; if (isRight) return Vector3.forward; break;
                case 1: if (isLeft) return Vector3.left; if (isRight) return Vector3.right; break;
                case 2: if (isLeft) return Vector3.forward; if (isRight) return Vector3.back; break;
                case 3: if (isLeft) return Vector3.right; if (isRight) return Vector3.left; break;
            }
            return Vector3.zero;
        }

        float yRot = Camera.main.transform.eulerAngles.y;
        float[] angles = { 45f, 135f, 225f, 315f };
        float closest = 0f;
        float minDiff = float.MaxValue;

        foreach (float angle in angles)
        {
            float diff = Mathf.Abs(Mathf.DeltaAngle(yRot, angle));
            if (diff < minDiff) { minDiff = diff; closest = angle; }
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

        return Vector3.zero;
    }

    IEnumerator MoveToPosition(Vector3 destination)
    {
        isMoving = true;
        while (Vector3.Distance(transform.position, destination) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = destination;
        isMoving = false;
    }
}
