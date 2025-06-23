using UnityEngine;
using System.Collections.Generic;

public class PathPiece : MonoBehaviour
{
    public List<PathPiece> illusionLinkedPieces = new List<PathPiece>();
    public float alignmentTolerance = 50f;
    public float depthTolerance = 2f;
    public bool enableDebug = true;

    public bool IsAlignedWith(PathPiece other, Camera cam)
    {
        Vector3 screenPos1 = cam.WorldToScreenPoint(transform.position);
        Vector3 screenPos2 = cam.WorldToScreenPoint(other.transform.position);

        float screenDist = Vector2.Distance(new Vector2(screenPos1.x, screenPos1.y), new Vector2(screenPos2.x, screenPos2.y));
        float zDiff = Mathf.Abs(screenPos1.z - screenPos2.z);

        if (enableDebug)
        {
            Debug.Log($"🟡 [{name}] vs [{other.name}] | ZDiff: {zDiff}, ScreenDist: {screenDist}");
        }

        if (zDiff > depthTolerance || screenDist > alignmentTolerance)
        {
            if (enableDebug)
                Debug.Log("❌ Not aligned with: " + other.name);
            return false;
        }

        return true;
    }
}