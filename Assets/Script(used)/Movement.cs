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
    private Animator animator;

    private struct QueuedTeleport
    {
        public int viewIndex;
        public string controlSide;
        public IllusionTeleportByViewIndex zone;
    }
    private QueuedTeleport? pendingTeleport = null;

    void Start()
    {
        illusionSystem = FindObjectOfType<IllusionTeleportByViewIndex>();
        animator = GetComponent<Animator>();
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
            pendingTeleport = new QueuedTeleport
            {
                viewIndex = currentView,
                controlSide = controlSide,
                zone = activeZone
            };
            // DO NOT RETURN — continue to rotate and process teleport after
        }

        Vector3 nextPos = transform.position + inputDir * moveDistance;
        StartCoroutine(MoveToPosition(nextPos, inputDir));
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
                case 0: if (isLeft) { direction = Vector3.back; controlSide = "Left"; } else if (isRight) { direction = Vector3.forward; controlSide = "Right"; } break;
                case 1: if (isLeft) { direction = Vector3.left; controlSide = "Left"; } else if (isRight) { direction = Vector3.right; controlSide = "Right"; } break;
                case 2: if (isLeft) { direction = Vector3.forward; controlSide = "Left"; } else if (isRight) { direction = Vector3.back; controlSide = "Right"; } break;
                case 3: if (isLeft) { direction = Vector3.right; controlSide = "Left"; } else if (isRight) { direction = Vector3.left; controlSide = "Right"; } break;
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
            if (diff < minDiff) { closest = angle; minDiff = diff; }
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

    IEnumerator MoveToPosition(Vector3 destination, Vector3 moveDirection)
    {
        isMoving = true;

        if (moveDirection != Vector3.zero)
        {
            Vector3 flatDirection = new Vector3(moveDirection.x, 0f, moveDirection.z);
            yield return StartCoroutine(RotateToDirection(flatDirection));
        }

        if (pendingTeleport != null)
        {
            var data = pendingTeleport.Value;
            pendingTeleport = null;
            data.zone.TryTeleport(data.viewIndex, data.controlSide);
            isMoving = false;
            yield break;
        }

        while (Vector3.Distance(transform.position, destination) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = destination;
        isMoving = false;
    }

    IEnumerator RotateToDirection(Vector3 moveDir)
    {
        Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
        float rotateSpeed = 10f;

        if (animator != null) animator.SetBool("IsTurning", true);

        while (Quaternion.Angle(transform.rotation, targetRot) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotateSpeed);
            yield return null;
        }

        transform.rotation = targetRot;

        if (animator != null) animator.SetBool("IsTurning", false);
    }

    public void TeleportTo(Vector3 destination)
    {
        StopAllCoroutines(); // force override
        StartCoroutine(TeleportAsSmoothMove(destination));
    }

    private IEnumerator TeleportAsSmoothMove(Vector3 destination)
    {
        isMoving = true;

        Vector3 moveDir = destination - transform.position;
        moveDir.y = 0;

        if (moveDir != Vector3.zero)
            yield return StartCoroutine(RotateToDirection(moveDir));

        while (Vector3.Distance(transform.position, destination) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = destination;
        isMoving = false;
    }
}
