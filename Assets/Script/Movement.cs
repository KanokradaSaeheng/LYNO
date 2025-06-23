using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDistance = 1f;
    public float moveSpeed = 5f;

    [Header("Camera + Illusion")]
    public CameraFollow cameraFollow;
    public IllusionWalkSystem illusionSystem;
    public bool allowIllusionMovement = true;
    public bool is2DMode = false;
    public int active2DOffsetIndex;

    [Header("Debug + Player Ref")]
    public bool drawDebug = true;
    public Transform player;

    private bool isMoving = false;
    private Vector3 targetPos;

    void Start()
    {
        targetPos = transform.position;
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        if (isMoving) return;

        Vector3 inputDir = GetTouchDirection();
        if (inputDir == Vector3.zero) return;

        if (CanMoveTo(inputDir, out Vector3 finalTarget))
        {
            StartCoroutine(MoveToPosition(finalTarget));
        }
    }

    bool CanMoveTo(Vector3 direction, out Vector3 finalTarget)
    {
        finalTarget = transform.position + direction * moveDistance;

        if (!IsBlocked(finalTarget))
        {
            if (drawDebug) Debug.Log("✔ Real platform found.");
            return true;
        }

        // Illusion logic
        if (allowIllusionMovement && illusionSystem.TryGetIllusionTarget(direction, out Vector3 illusionTarget))
        {
            if (drawDebug) Debug.Log("✨ Illusion move allowed to " + illusionTarget);
            finalTarget = illusionTarget;
            return true;
        }

        if (drawDebug) Debug.Log("❌ Move blocked.");
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
                if (drawDebug) Debug.DrawLine(rayOrigin, hit.point, Color.green, 1f);
                return false;
            }
        }

        if (drawDebug) Debug.DrawLine(rayOrigin, rayOrigin + rayDirection * rayLength, Color.red, 1f);
        return true;
    }

    Vector3 GetTouchDirection()
    {
        if (Input.touchCount == 0) return Vector3.zero;

        Vector2 touchPos = Input.GetTouch(0).position;
        float w = Screen.width;
        float h = Screen.height;

        bool isTop = touchPos.y > h * 0.75f;
        bool isBottom = touchPos.y < h * 0.25f;
        bool isLeft = touchPos.x < w * 0.25f;
        bool isRight = touchPos.x > w * 0.75f;

        if (is2DMode)
        {
            return active2DOffsetIndex switch
            {
                0 => isLeft ? Vector3.back : isRight ? Vector3.forward : Vector3.zero,
                1 => isLeft ? Vector3.left : isRight ? Vector3.right : Vector3.zero,
                2 => isLeft ? Vector3.forward : isRight ? Vector3.back : Vector3.zero,
                3 => isLeft ? Vector3.right : isRight ? Vector3.left : Vector3.zero,
                _ => Vector3.zero
            };
        }

        float yRot = Camera.main.transform.eulerAngles.y;
        float[] angles = { 45f, 135f, 225f, 315f };
        float closest = angles[0];
        float minDiff = Mathf.Infinity;

        foreach (float angle in angles)
        {
            float diff = Mathf.Abs(Mathf.DeltaAngle(yRot, angle));
            if (diff < minDiff)
            {
                closest = angle;
                minDiff = diff;
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
        if (isRight)
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
        if (isTop)
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
        if (isBottom)
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
