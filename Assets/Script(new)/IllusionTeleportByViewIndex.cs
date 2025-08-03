using UnityEngine;
using System.Collections;

public class IllusionTeleportByViewIndex : MonoBehaviour
{
    [System.Serializable]
    public class ViewTeleportEntry
    {
        public bool is2DEntry = false;
        public int viewIndex;
        public string controlSide;
        public Transform targetPosition;
    }

    [Header("Teleport Config")]
    public ViewTeleportEntry[] teleportEntries;
    public string playerTag = "Player";
    public bool onlyOnce = true;

    private bool isPlayerInZone = false;
    private bool hasTeleported = false;
    private Transform cachedPlayer;

    private string queuedControl = null;
    private int queuedViewIndex = -1;

    private void Start() => IllusionTeleportManager.Instance?.RegisterZone(this);
    private void OnDestroy() => IllusionTeleportManager.Instance?.UnregisterZone(this);

    private void Update()
    {
        if (!isPlayerInZone || cachedPlayer == null || string.IsNullOrEmpty(queuedControl)) return;

        if (IsGroundedByTag(cachedPlayer))
        {
            if (MatchesTeleportCondition(queuedViewIndex, queuedControl))
            {
                TryTeleport(queuedViewIndex, queuedControl);
                queuedControl = null;
                queuedViewIndex = -1;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag) || (onlyOnce && hasTeleported)) return;

        cachedPlayer = other.transform;
        isPlayerInZone = true;
        Debug.Log($"🟡 ENTERED teleport zone: {gameObject.name}");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        isPlayerInZone = false;
        cachedPlayer = null;
        queuedControl = null;
        queuedViewIndex = -1;

        Debug.Log($"⚪ EXITED teleport zone: {gameObject.name}");
    }

    public bool ShouldBlockControl(int viewIndex, string controlSide)
    {
        if (!isPlayerInZone || (onlyOnce && hasTeleported)) return false;

        if (cachedPlayer != null)
        {
            if (!IsGroundedByTag(cachedPlayer))
            {
                // queue teleport input
                queuedControl = controlSide;
                queuedViewIndex = viewIndex;
                Debug.Log("🕳️ Falling into zone, queued teleport...");
                return false;
            }
        }

        return MatchesTeleportCondition(viewIndex, controlSide);
    }

    private bool MatchesTeleportCondition(int viewIndex, string controlSide)
    {
        bool is2D = DimensionManager.Is2DModeStatic;
        int currentIndex = is2D ? GetCurrent2DIndex() : GetCurrentViewIndex();

        foreach (var entry in teleportEntries)
        {
            if (entry.is2DEntry == is2D && entry.viewIndex == currentIndex && entry.controlSide == controlSide)
            {
                Debug.Log($"🟢 Teleport Match: [{(is2D ? "2D" : "3D")}] Index={currentIndex} Side={controlSide}");
                return true;
            }
        }

        return false;
    }

    public void TryTeleport(int viewIndex, string controlSide)
    {
        if (!isPlayerInZone || cachedPlayer == null) return;

        bool is2D = DimensionManager.Is2DModeStatic;
        int currentIndex = is2D ? GetCurrent2DIndex() : viewIndex;

        foreach (var entry in teleportEntries)
        {
            if (entry.is2DEntry == is2D && entry.viewIndex == currentIndex && entry.controlSide == controlSide)
            {
                Debug.Log($"🚀 Teleporting to: {entry.targetPosition.position}");

                if (cachedPlayer.TryGetComponent<Movement>(out var moveScript))
                {
                    moveScript.TeleportTo(entry.targetPosition.position);
                }
                else
                {
                    cachedPlayer.position = entry.targetPosition.position;
                }

                if (onlyOnce)
                {
                    hasTeleported = true;
                    isPlayerInZone = false;
                    cachedPlayer = null;
                }

                StartCoroutine(ForceRecheckZone());
                return;
            }
        }

        Debug.LogWarning($"❌ No teleport match found for index={currentIndex}, side={controlSide}");
    }

    private IEnumerator ForceRecheckZone()
    {
        yield return null;
        if (cachedPlayer == null)
            cachedPlayer = GameObject.FindGameObjectWithTag(playerTag)?.transform;

        if (cachedPlayer != null)
        {
            Collider[] hits = Physics.OverlapSphere(cachedPlayer.position, 0.1f);
            foreach (var col in hits)
            {
                if (col.gameObject == gameObject)
                {
                    OnTriggerEnter(col);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Check if the player is touching a ground-tagged collider below them.
    /// </summary>
    private bool IsGroundedByTag(Transform player)
    {
        Vector3 checkPosition = player.position + Vector3.down * 0.1f;
        float radius = 0.2f;

        Collider[] hits = Physics.OverlapSphere(checkPosition, radius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Ground") && hit.gameObject != gameObject)
                return true;
        }

        return false;
    }

    public bool IsPlayerInZone() => isPlayerInZone;

    private int GetCurrentViewIndex()
    {
        CameraFollow cam = Camera.main?.GetComponent<CameraFollow>();
        return cam != null ? cam.currentViewIndex : -1;
    }

    private int GetCurrent2DIndex()
    {
        Movement move = GameObject.FindGameObjectWithTag(playerTag)?.GetComponent<Movement>();
        return move != null ? move.active2DOffsetIndex : -1;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (var entry in teleportEntries)
        {
            if (entry.targetPosition != null)
            {
                Gizmos.DrawLine(transform.position, entry.targetPosition.position);
                Gizmos.DrawSphere(entry.targetPosition.position, 0.2f);
            }
        }
    }
#endif
}
