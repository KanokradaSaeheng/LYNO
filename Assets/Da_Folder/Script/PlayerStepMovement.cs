using UnityEngine;
using System.Collections;

public class PlayerStepMovement : MonoBehaviour
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

    [Header("Footstep Sound")]
    public AudioClip stepSound; // 🎵 assign in inspector
    private AudioSource audioSource;

    private bool isMoving = false;
    private Animator animator;

    private bool isLeftFootNext = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (isMoving) return;

        if (Input.touchCount == 0 || Input.GetTouch(0).phase != TouchPhase.Began)
            return;

        Vector3 inputDir;
        GetTouchDirection(out inputDir);

        if (inputDir == Vector3.zero) return;

        if (IsBlocked(inputDir))
        {
            Debug.Log("⛔ Blocked! Movement cancelled.");
            return;
        }

        Vector3 nextPos = transform.position + inputDir * moveDistance;
        StartCoroutine(MoveToPosition(nextPos, inputDir));
    }

    private bool IsBlocked(Vector3 direction)
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 halfExtents = Vector3.one * checkBoxSize * 0.5f;

        return Physics.BoxCast(origin, halfExtents, direction.normalized, Quaternion.identity, checkDistance, obstacleLayer);
    }

    void GetTouchDirection(out Vector3 direction)
    {
        direction = Vector3.zero;

        Vector2 touchPos = Input.GetTouch(0).position;
        float w = Screen.width;
        float h = Screen.height;

        bool isLeft = touchPos.x < w * 0.25f;
        bool isRight = touchPos.x > w * 0.75f;
        bool isTop = touchPos.y > h * 0.75f;
        bool isBottom = touchPos.y < h * 0.25f;

        if (is2DMode)
        {
            switch (active2DOffsetIndex)
            {
                case 0: direction = isLeft ? Vector3.back : isRight ? Vector3.forward : Vector3.zero; break;
                case 1: direction = isLeft ? Vector3.left : isRight ? Vector3.right : Vector3.zero; break;
                case 2: direction = isLeft ? Vector3.forward : isRight ? Vector3.back : Vector3.zero; break;
                case 3: direction = isLeft ? Vector3.right : isRight ? Vector3.left : Vector3.zero; break;
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

        // 👣 Play left or right step animation
        if (animator != null)
        {
            string animationName = isLeftFootNext ? "StepLeft" : "StepRight";
            animator.Play(animationName);
            isLeftFootNext = !isLeftFootNext;
        }

        // 🎵 Play step sound
        if (audioSource != null && stepSound != null)
        {
            audioSource.PlayOneShot(stepSound);
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

        if (animator != null)
            animator.SetBool("IsTurning", true);

        while (Quaternion.Angle(transform.rotation, targetRot) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotateSpeed);
            yield return null;
        }

        transform.rotation = targetRot;

        if (animator != null)
            animator.SetBool("IsTurning", false);
    }
}
