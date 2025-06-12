using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3[] cameraOffsets3D;
    public Vector3[] cameraOffsets2D;
    public int currentViewIndex = 0;
    public float smoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 lookVelocity = Vector3.zero;
    public bool is2D = false;

    private Vector3 lookTarget;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 offset = is2D ? cameraOffsets2D[currentViewIndex] : cameraOffsets3D[currentViewIndex];
        Vector3 desiredPosition = target.position + offset;

        // Smooth camera position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

        // Smooth look-at target (prevents jitter from flip movement)
        lookTarget = Vector3.SmoothDamp(lookTarget, target.position, ref lookVelocity, smoothTime);
        transform.LookAt(lookTarget);
    }

    public void SetViewIndex(int index)
    {
        int max = is2D ? cameraOffsets2D.Length : cameraOffsets3D.Length;
        if (index >= 0 && index < max)
        {
            currentViewIndex = index;
        }
    }
}