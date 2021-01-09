using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleCalculations : MonoBehaviour {

    public List<Vector3> ConvertBoundsToFourPoints (Bounds bounds) {
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        Vector3 topRight = center;
        topRight.x += extents.x;
        topRight.z += extents.z;
        Vector3 topLeft = center;
        topLeft.x -= extents.x;
        topLeft.z += extents.z;
        Vector3 bottomRight = center;
        bottomRight.x += extents.x;
        bottomRight.z -= extents.z;
        Vector3 bottomLeft = center;
        bottomLeft.x -= extents.x;
        bottomLeft.z -= extents.z;

        // Debug.Log ("here:");
        // Debug.Log ("center: " + center);
        // Debug.Log ("extents: " + extents);
        // Debug.Log (topRight);
        // Debug.Log (topLeft);
        // Debug.Log (bottomRight);
        // Debug.Log (bottomLeft);

        return new List<Vector3> { topRight, topLeft, bottomRight, bottomLeft };
    }

    public bool PointInVector3s (Vector3 m, Vector3 a, Vector3 b, Vector3 c, Vector3 d) {
        return PointInRectangle (FlattenVector3 (m), FlattenVector3 (a), FlattenVector3 (b), FlattenVector3 (c), FlattenVector3 (d));
    }

    private Vector2 FlattenVector3 (Vector3 vec) {
        return new Vector2 (vec.x, vec.z);
    }

    public bool PointInRectangle (Vector2 m, Vector2 a, Vector2 b, Vector2 c, Vector2 d) {
        Vector2 am = m - a;
        Vector2 ab = b - a;
        Vector2 ad = d - a;

        float amab = Vector2.Dot (am, ab);
        float abab = Vector2.Dot (ab, ab);
        float amad = Vector2.Dot (am, ad);
        float adad = Vector2.Dot (ad, ad);

        bool left = 0 < amab && amab < abab;
        bool right = 0 < amad && amad < adad;

        Debug.Log ("point: " + m);
        Debug.Log ("left: " + left);
        Debug.Log ("right: " + right);
        return left && right;
    }
}