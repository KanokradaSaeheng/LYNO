using UnityEngine;

public class IllusionWalkSystem : MonoBehaviour
{
    public Transform player;

    public bool TryGetIllusionTarget(Vector3 moveDir, out Vector3 illusionTarget)
    {
        illusionTarget = Vector3.zero;
        Vector3 start = player.position;

        PathPiece current = FindClosestPiece(start);
        if (current == null) return false;

        foreach (PathPiece next in current.illusionLinkedPieces)
        {
            if (current.IsAlignedWith(next, Camera.main))
            {
                Vector3 dirToNext = (next.transform.position - current.transform.position).normalized;
                if (Vector3.Dot(dirToNext, moveDir.normalized) > 0.9f)
                {
                    illusionTarget = next.transform.position;
                    return true;
                }
            }
        }

        return false;
    }

    private PathPiece FindClosestPiece(Vector3 pos)
    {
        PathPiece closest = null;
        float minDist = float.MaxValue;

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Platform"))
        {
            PathPiece piece = obj.GetComponent<PathPiece>();
            if (!piece) continue;

            float dist = Vector3.Distance(pos, piece.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = piece;
            }
        }

        return closest;
    }
}