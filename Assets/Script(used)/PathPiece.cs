using UnityEngine;
using System.Collections.Generic;

public class PathPiece : MonoBehaviour
{
    public List<PathPiece> illusionLinkedPieces = new List<PathPiece>();
    public float alignmentTolerance = 50f;
    public float depthTolerance = 2f;
    public bool enableDebug = true;

    public float alignmentAngleTolerance = 2f;

    public bool IsAlignedWith(PathPiece other, Camera cam)
    {
        Vector3 toOther = other.transform.position - transform.position;
        Vector3 camForward = cam.transform.forward;

        // Project both vectors onto the screen plane
        Vector3 projectedToOther = Vector3.ProjectOnPlane(toOther, cam.transform.forward).normalized;
        Vector3 projectedRight = Vector3.ProjectOnPlane(cam.transform.right, cam.transform.forward).normalized;

        float angle = Vector3.Angle(projectedToOther, projectedRight);

        if (enableDebug)
        {
            Debug.Log($"🟡 Angle between [{name}] and [{other.name}] = {angle}");
        }

        return angle < alignmentAngleTolerance;
    }

}