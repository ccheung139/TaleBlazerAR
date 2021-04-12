using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMeGestureScript : MonoBehaviour {
    public bool CheckFollowMe (List<Vector3> points, List<float> shakeTimes) {
        float minHeight = Mathf.Infinity;
        float maxHeight = -Mathf.Infinity;

        foreach (Vector3 point in points) {
            if (point.y > maxHeight) {
                maxHeight = point.y;
            }
            if (point.y < minHeight) {
                minHeight = point.y;
            }

            if (GetFlatDistance(points[0], point) > 0.5f) {
                return false;
            }
        }

        float difference = Mathf.Abs (maxHeight - minHeight);
        return difference < 0.3f && shakeTimes.Count >= 3;
    }

    private float GetFlatDistance(Vector3 point1, Vector3 point2)
    {
        Vector3 flatP2 = point2;
        flatP2.y = point1.y;
        return Vector3.Distance(point1, flatP2);
    }
}