using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrowWatcher : MonoBehaviour {

    public GameObject robot;

    public bool CheckThrow (List<Vector3> points, List<float> pointsInTime) {

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

            float heightDifference = minDistancePoint.y - points[0].y;
            float backDistance = maxDistance - Vector3.Distance (points[0], robot.transform.position);

            if (Vector3.Distance (maxDistancePoint, minDistancePoint) > 0.3f && backDistance >= 0.15f && heightDifference >= 0.1f) {
                return true;
            }
        }
        return false;
    }
}