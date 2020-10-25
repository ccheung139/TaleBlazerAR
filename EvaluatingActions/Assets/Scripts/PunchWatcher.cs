using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PunchWatcher : MonoBehaviour {

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

            if (Vector3.Distance (maxDistancePoint, minDistancePoint) > 0.3f &&  heightDifference < 0.2f) {
                return true;
            }
        }
        return false;
    }

    public bool CheckPunch (List<Vector3> points, List<float> pointsInTime) {
        Vector3 lastPoint = new Vector3 (0, 0, 0);
        float lastPointTime = 0;
        bool passedSpeedPoint = false;

        Vector3 punchStartPoint = new Vector3 (0, 0, 0);
        float punchStartTime = 0;

        for (int i = 0; i < points.Count; i++) {
            Vector3 point = points[i];
            float pointTime = pointsInTime[i];

            if (i == 0) {
                lastPoint = point;
                lastPointTime = pointTime;
                continue;
            }

            float distanceTraveled = Vector3.Distance (new Vector3 (point.x, 0, point.z), new Vector3 (lastPoint.x, 0, lastPoint.z));
            float timeTraveled = pointTime - lastPointTime;
            float speed = distanceTraveled / timeTraveled;

            if (passedSpeedPoint) {
                if (speed < 0.25f) {
                    float totalDistance = Vector3.Distance (new Vector3 (point.x, 0, point.z), new Vector3 (punchStartPoint.x, 0, punchStartPoint.z));
                    float totalTime = pointTime - punchStartTime;
                    float totalSpeed = totalDistance / totalTime;

                    if (totalDistance >= 0.2f && totalTime <= 0.7f) {
                        return true;
                    }
                }
            } else {
                float startDistanceToRobot = Vector3.Distance (robot.transform.position, points[0]);
                float distanceToRobot = Vector3.Distance (robot.transform.position, point);
                if ((distanceToRobot - startDistanceToRobot) < 0 && speed >= 0.25f) {
                    punchStartPoint = point;
                    punchStartTime = pointTime;
                    passedSpeedPoint = true;
                }
            }

            lastPoint = point;
            lastPointTime = pointTime;
        }
        return false;
    }
}