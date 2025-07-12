// =============================
// Movement.cs (One touch = one move)
// =============================
using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDistance = 1f;
    public float moveSpeed = 5f;

    [Header("Camera & View Settings")]
    public CameraFollow cameraFollow;
    public bool is2DMode = false;
    public int active2DOffsetIndex;

    private bool isMoving = false;
    private IllusionTeleportByViewIndex illusionSystem;

    void Start()
    {
        illusionSystem = FindObjectOfType<IllusionTeleportByViewIndex>();
    }

    private void Update()
    {
        if (isMoving) return;

        if (Input.touchCount == 0 || Input.GetTouch(0).phase != TouchPhase.Began)
            return;

        Vector3 inputDir;
        string controlSide;
        GetTouchDirection(out inputDir, out controlSide);

        if (inputDir == Vector3.zero || string.IsNullOrEmpty(controlSide)) return;

        int currentView = cameraFollow != null ? cameraFollow.currentViewIndex : -1;

        var activeZone = IllusionTeleportManager.Instance?.GetActiveTeleportZone();
        if (activeZone != null && activeZone.ShouldBlockControl(currentView, controlSide))
        {
            activeZone.TryTeleport(currentView, controlSide);
            return;
        }

        Vector3 nextPos = transform.position + inputDir * moveDistance;
        StartCoroutine(MoveToPosition(nextPos));
    }


    void GetTouchDirection(out Vector3 direction, out string controlSide)
    {
        direction = Vector3.zero;
        controlSide = null;

        Vector2 touchPos = Input.GetTouch(0).position;
        float w = Screen.width;
        float h = Screen.height;

        bool isTop = touchPos.y > h * 0.75f;
        bool isBottom = touchPos.y < h * 0.25f;
        bool isLeft = touchPos.x < w * 0.25f;
        bool isRight = touchPos.x > w * 0.75f;

        if (is2DMode)
        {
            switch (active2DOffsetIndex)
            {
                case 0:
                    if (isLeft) { direction = Vector3.back; controlSide = "Left"; }
                    else if (isRight) { direction = Vector3.forward; controlSide = "Right"; }
                    break;
                case 1:
                    if (isLeft) { direction = Vector3.left; controlSide = "Left"; }
                    else if (isRight) { direction = Vector3.right; controlSide = "Right"; }
                    break;
                case 2:
                    if (isLeft) { direction = Vector3.forward; controlSide = "Left"; }
                    else if (isRight) { direction = Vector3.back; controlSide = "Right"; }
                    break;
                case 3:
                    if (isLeft) { direction = Vector3.right; controlSide = "Left"; }
                    else if (isRight) { direction = Vector3.left; controlSide = "Right"; }
                    break;
            }
            return;
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
            controlSide = "Left";
            direction = closest switch
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
            controlSide = "Right";
            direction = closest switch
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
            controlSide = "Top";
            direction = closest switch
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
            controlSide = "Bottom";
            direction = closest switch
            {
                45f => Vector3.left,
                135f => Vector3.forward,
                225f => Vector3.right,
                315f => Vector3.back,
                _ => Vector3.zero
            };
        }
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
