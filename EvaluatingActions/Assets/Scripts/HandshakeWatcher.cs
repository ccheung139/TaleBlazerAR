using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandshakeWatcher : MonoBehaviour {
    public GameObject robot;

    public bool CheckHandshake (List<Vector3> points, List<float> pointsInTime, List<float> shakeTimes) {
        int shakeCounts = 0;
        float timeExtended = CheckExtension (points, pointsInTime);
        if (timeExtended == -1.0f) {
            return false;
        }

        foreach (float shakeTime in shakeTimes) {
            if (shakeTime < timeExtended) {
                return false;
            } else {
                shakeCounts += 1;
                if (shakeCounts > 2) {
                    return true;
                }
            }
        }
        return false;

    }

    public float CheckExtension (List<Vector3> points, List<float> pointsInTime) {

        float maxDistance = 0;
        Vector3 maxDistancePoint = new Vector3 (0, 0, 0);

        float minDistance = 100000;
        Vector3 minDistancePoint = new Vector3 (0, 0, 0);

        for (int i = 0; i < points.Count; i++) {
            Vector3 point = points[i];
            float pointTime = pointsInTime[i];

            float distanceToRobot = Vector3.Distance (robot.transform.position, point);
            if (distanceToRobot >= maxDistance) {
                maxDistance = distanceToRobot;
                maxDistancePoint = point;
            }

            if (distanceToRobot <= minDistance) {
                minDistance = distanceToRobot;
                minDistancePoint = point;
            }

            if (Vector3.Distance (maxDistancePoint, minDistancePoint) > 0.3f) {
                return pointTime;
                // return true;
            }
        }
        return -1.0f;
    }
}