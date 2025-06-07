using UnityEngine;

public class DimensionManager : MonoBehaviour
{
    public CameraFollow cameraFollow;
    public Transform cameraTransform;
    public Camera mainCamera;

    public Quaternion rotation3D = Quaternion.Euler(30f, 45f, 0f);
    public Quaternion rotation2D = Quaternion.Euler(90f, 0f, 0f);
    public float rotationDuration = 1f;

    public bool is2D = false;
    public Vector3 offset2D;
    public bool isRotating = false;
    public Movement movementScript;

    private int cameraPOVIndex = 0;
    public static System.Action<bool> OnDimensionChange; // True = 2D, False = 3D
    public static bool Is2DModeStatic { get; private set; }

    private Vector2 swipeStart;
    private float swipeThreshold = 50f;
    private float doubleTapTime = 0.3f;
    private float lastTapTime = 0f;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        mainCamera.orthographic = true;
        cameraTransform.rotation = is2D ? rotation2D : rotation3D;
    }

    void Update()
    {
#if UNITY_EDITOR
        // For editor testing
        if (Input.GetKeyDown(KeyCode.Space) && !isRotating)
        {
            ToggleDimension();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //ChangePOV();
        }
#endif

        HandleMobileInput();
    }

    void HandleMobileInput()
    {
        if (Input.touchCount == 0 || isRotating) return;

        Touch touch = Input.GetTouch(0);
        Vector2 touchPos = touch.position;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        bool isInMiddleZone = touchPos.y > screenHeight * 0.25f && touchPos.y < screenHeight * 0.75f;

        switch (touch.phase)
        {
            case TouchPhase.Began:
                swipeStart = touchPos;

                // Detect double tap
                if (Time.time - lastTapTime < doubleTapTime && isInMiddleZone)
                {
                    ToggleDimension();
                    lastTapTime = 0; // Reset to avoid triple tap
                }
                else
                {
                    lastTapTime = Time.time;
                }
                break;

            case TouchPhase.Ended:
                Vector2 swipeDelta = touchPos - swipeStart;

                if (isInMiddleZone &&
                    Mathf.Abs(swipeDelta.x) > swipeThreshold &&
                    Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                {
                    if (swipeDelta.x > 0)
                    {
                        Debug.Log("Swipe RIGHT → Next POV");
                        NextPOV();
                    }
                    else
                    {
                        Debug.Log("Swipe LEFT → Previous POV");
                        PreviousPOV();
                    }
                }
                break;

        }
    }

    void ToggleDimension()
    {
        is2D = !is2D;
        Is2DModeStatic = is2D;
        cameraFollow.is2D = is2D;
        movementScript.is2DMode = is2D;

        StartCoroutine(SmoothRotate(is2D ? rotation2D : rotation3D));
        OnDimensionChange?.Invoke(is2D);
    }

    void NextPOV()
    {
        int max = cameraFollow.is2D ? cameraFollow.cameraOffsets2D.Length : cameraFollow.cameraOffsets3D.Length;
        cameraPOVIndex = (cameraPOVIndex + 1) % max;
        cameraFollow.SetViewIndex(cameraPOVIndex);
    }

    void PreviousPOV()
    {
        int max = cameraFollow.is2D ? cameraFollow.cameraOffsets2D.Length : cameraFollow.cameraOffsets3D.Length;
        cameraPOVIndex = (cameraPOVIndex - 1 + max) % max; // make it wrap around correctly
        cameraFollow.SetViewIndex(cameraPOVIndex);
    }


    System.Collections.IEnumerator SmoothRotate(Quaternion targetRotation)
    {
        isRotating = true;
        Quaternion startRotation = cameraTransform.rotation;
        float elapsed = 0f;

        while (elapsed < rotationDuration)
        {
            cameraTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / rotationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraTransform.rotation = targetRotation;
        isRotating = false;
    }
}
