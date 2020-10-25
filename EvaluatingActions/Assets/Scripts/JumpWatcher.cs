using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpWatcher : MonoBehaviour {
    public Text resultText;

    public bool CheckJump (List<Vector3> points, List<float> pointsInTime) {
        Vector3 maxHeightPoint = new Vector3 (0, 0, 0);
        float maxHeightTime = 0;
        for (int i = 0; i < points.Count; i++) {
            Vector3 point = points[i];
            float pointTime = pointsInTime[i];
            if (point.y >= maxHeightPoint.y) {
                maxHeightPoint = point;
                maxHeightTime = pointTime;
            }

            float peakHeightDifference = maxHeightPoint.y - point.y;
            float currentHeightDifference = point.y - points[0].y;
            float peakToCurrentTime = pointTime - maxHeightTime;

            if (peakHeightDifference >= 0.4f && currentHeightDifference <= .2f && peakToCurrentTime <= .5f) {
                return true;
            }
        }
        return false;
    }
}