using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayPutGestureScript : MonoBehaviour {
    public bool CheckStayPut (List<Vector3> points, List<float> pointTimes, List<float> rotations) {
        bool loweredArm = false;

        float firstYPos = points[0].y;
        float earliestTime = pointTimes[0];
        float maxTime = 0;

        for (int i = 0; i < points.Count; i++) {
            Vector3 point = points[i];
            float rotation = rotations[i];
            float pointTime = pointTimes[i];

            float currentYPos = point.y;
            float difference = firstYPos - currentYPos;
            if (!loweredArm && difference > 0.3f && rotation < 0) {
                loweredArm = true;
                earliestTime = pointTime;
            }
            if (loweredArm) {
                if (difference < 0.3f || rotation > 0) {
                    loweredArm = false;
                    maxTime = CalculateTime (pointTime, earliestTime, maxTime);
                }
            }
        }

        if (loweredArm) {
            maxTime = CalculateTime (pointTimes[pointTimes.Count - 1], earliestTime, maxTime);
        }

        return maxTime > 0.5f;
    }

    private float CalculateTime (float pointTime, float earliestTime, float maxTime) {
        float time = pointTime - earliestTime;
        if (time > maxTime) {
            maxTime = time;
        }
        return maxTime;
    }
}