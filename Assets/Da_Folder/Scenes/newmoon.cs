using UnityEngine;
using System.Collections;

public class newmoon : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDistance = 1f;
    public float moveSpeed = 5f;

    [Header("Teleport Settings")]
    public float teleportSpeed = 10f;

    [Header("Camera & View Settings")]
    public CameraFollow cameraFollow;
    public bool is2DMode = false;
    public int active2DOffsetIndex;

    [Header("Collision Check")]
    public LayerMask obstacleLayer;
    public float checkDistance = 1f;
    public float checkBoxSize = 0.4f;

    [Header("Animation")]
    public Animator catAnimator; // Drag your cat's Animator here

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
        // Use Unity 2023+ API
        illusionSystem = FindFirstObjectByType<IllusionTeleportByViewIndex>();
        animator = GetComponent<Animator>();

        if (catAnimator == null)
            catAnimator = GetComponentInChildren<Animator>();

        if (catAnimator == null)
            Debug.LogError("Cat Animator not found! Make sure the cat model has an Animator component.");
    }

    void Update()
    {
        if (isMoving) return;

        if (Input.touchCount == 0 || Input.GetTouch(0).phase != TouchPhase.Began)
            return;

        Vector3 inputDir;
        string controlSide;
        GetTouchDirection(out inputDir, out controlSide);

        if (inputDir == Vector3.zero) return;

        int currentView = cameraFollow != null ? cameraFollow.currentViewIndex : -1;
        var zone = IllusionTeleportManager.Instance?.GetActiveTeleportZone();
        if (zone != null && zone.ShouldBlockControl(currentView, controlSide))
        {
            pendingTeleport = new QueuedTeleport
            {
                viewIndex = currentView,
                controlSide = controlSide,
                zone = zone
            };
        }

        if (IsBlocked(inputDir)) return;

        Vector3 nextPos = transform.position + inputDir * moveDistance;
        StartCoroutine(MoveToPosition(nextPos, inputDir));
    }

    private bool IsBlocked(Vector3 dir)
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 half = Vector3.one * checkBoxSize * 0.5f;
        return Physics.BoxCast(origin, half, dir.normalized, Quaternion.identity, checkDistance, obstacleLayer);
    }

    void GetTouchDirection(out Vector3 direction, out string controlSide)
    {
        direction = Vector3.zero;
        controlSide = null;
        Vector2 touch = Input.GetTouch(0).position;
        float w = Screen.width, h = Screen.height;
        bool top = touch.y > h * .75f, bottom = touch.y < h * .25f;
        bool left = touch.x < w * .25f, right = touch.x > w * .75f;

        if (is2DMode)
        {
            // ... same as before ...
            return;
        }

        float yRot = Camera.main.transform.eulerAngles.y;
        float[] angles = { 45, 135, 225, 315 };
        // ... same angle-closest logic ...
    }

    IEnumerator MoveToPosition(Vector3 dest, Vector3 dir)
    {
        isMoving = true;

        // 1) Rotate
        if (dir != Vector3.zero)
        {
            Vector3 flat = new Vector3(dir.x, 0, dir.z);
            yield return StartCoroutine(RotateToDirection(flat));
        }

        // 2) Trigger Walk Animation
        if (catAnimator != null)
            catAnimator.SetBool("IsMoving", true);

        // 3) Handle Teleport if queued
        if (pendingTeleport != null)
        {
            var d = pendingTeleport.Value;
            pendingTeleport = null;
            d.zone?.TryTeleport(d.viewIndex, d.controlSide);
            isMoving = false;
            yield break;
        }

        // 4) Move
        while (Vector3.Distance(transform.position, dest) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, dest, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = dest;

        // 5) Stop Walk Animation
        if (catAnimator != null)
            catAnimator.SetBool("IsMoving", false);

        isMoving = false;
    }

    IEnumerator RotateToDirection(Vector3 moveDir)
    {
        Quaternion target = Quaternion.LookRotation(moveDir, Vector3.up);
        if (animator != null) animator.SetBool("IsTurning", true);

        while (Quaternion.Angle(transform.rotation, target) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 10f);
            yield return null;
        }
        transform.rotation = target;

        if (animator != null) animator.SetBool("IsTurning", false);
    }

    public void TeleportTo(Vector3 dest)
    {
        StopAllCoroutines();
        StartCoroutine(TeleportAsSmoothMove(dest));
    }

    IEnumerator TeleportAsSmoothMove(Vector3 dest)
    {
        isMoving = true;
        Vector3 dir = dest - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
            yield return StartCoroutine(RotateToDirection(dir));

        while (Vector3.Distance(transform.position, dest) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, dest, teleportSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = dest;
        isMoving = false;
    }
}
