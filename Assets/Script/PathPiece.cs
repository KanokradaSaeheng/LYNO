using UnityEngine;

public class PathPiece : MonoBehaviour
{
    public PathPiece nextPiece;
    public float alignmentTolerance = 10f;

    public bool IsVisuallyAligned(Camera cam, float depthTolerance = 2f)
    {
        if (nextPiece == null) return false;

        Vector3 screenPos1 = cam.WorldToScreenPoint(transform.position);
        Vector3 screenPos2 = cam.WorldToScreenPoint(nextPiece.transform.position);

        if (Mathf.Abs(screenPos1.z - screenPos2.z) > depthTolerance)
            return false;

        return Vector2.Distance(new Vector2(screenPos1.x, screenPos1.y),
            new Vector2(screenPos2.x, screenPos2.y)) <= alignmentTolerance;
    }
}