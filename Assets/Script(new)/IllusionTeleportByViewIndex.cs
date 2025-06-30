// =============================
// IllusionTeleportByViewIndex.cs (Teleport Triggers On Specific Direction)
// =============================
using UnityEngine;
using System.Collections;

public class IllusionTeleportByViewIndex : MonoBehaviour
{
    [System.Serializable]
    public class ViewTeleport
    {
        public int viewIndex;
        public Transform targetPosition;
        public Vector3 direction; // Direction player must move to trigger
    }

    [Header("Teleport Settings")]
    public ViewTeleport[] viewTeleports;
    public string playerTag = "Player";
    public bool onlyOnce = true;

    private bool hasTeleported = false;
    private bool isPlayerInZone = false;
    private Transform cachedPlayer;
    private Vector3 requiredDirection;
    private Vector3 teleportTarget;
    private bool teleportReady = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (onlyOnce && hasTeleported) return;

        int currentView = GetCurrentViewIndex();
        if (currentView == -1) return;

        foreach (var teleport in viewTeleports)
        {
            if (teleport.viewIndex == currentView && teleport.targetPosition != null)
            {
                isPlayerInZone = true;
                cachedPlayer = other.transform;
                requiredDirection = teleport.direction.normalized;
                teleportTarget = teleport.targetPosition.position;
                teleportReady = true;
                return;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            ResetTeleportState();
        }
    }

    public bool ShouldBlockDirection(Vector3 moveDir)
    {
        if (!teleportReady) return false;
        return Vector3.Dot(moveDir.normalized, requiredDirection) > 0.9f;
    }

    public bool TryTriggerTeleport(Vector3 moveDir)
    {
        if (!teleportReady) return false;

        if (Vector3.Dot(moveDir.normalized, requiredDirection) > 0.9f)
        {
            cachedPlayer.position = teleportTarget;
            teleportReady = false;
            isPlayerInZone = false;
            hasTeleported = true;
            return true;
        }

        return false;
    }

    private void ResetTeleportState()
    {
        isPlayerInZone = false;
        teleportReady = false;
        cachedPlayer = null;
    }

    private int GetCurrentViewIndex()
    {
        CameraFollow camFollow = Camera.main?.GetComponent<CameraFollow>();
        return camFollow != null ? camFollow.currentViewIndex : -1;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (var t in viewTeleports)
        {
            if (t.targetPosition != null)
            {
                Gizmos.DrawLine(transform.position, t.targetPosition.position);
                Gizmos.DrawSphere(t.targetPosition.position, 0.2f);
            }
        }
    }
#endif
}
