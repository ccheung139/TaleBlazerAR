using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchGestureScript : MonoBehaviour
{
    public bool CheckPunch(List<Vector3> points, List<float> pointTimes, Vector3 forwardVector)
    {

        float firstYPos = points[0].y;
        float earliestTime = pointTimes[0];

        bool beenAssigned = false;
        Vector3 firstSpeedPoint = new Vector3(0, 0, 0);
        float firstSpeedTime = 0;

        bool punchMade = false;
        for (int i = 1; i < points.Count; i++)
        {
            Vector3 point = points[i];
            float pointTime = pointTimes[i];

            Vector3 direction = point - points[0];
            float angle = GetFlatAngle(forwardVector, direction);

            // float GetFlatDistance;
            float flatDiff = GetFlatDistance(points[i - 1], points[i]);
            float verticalTotalDiff = Math.Abs(point.y - firstYPos);
            float timeDiff = pointTimes[i] - pointTimes[i - 1];

            float speed = flatDiff / (timeDiff);

            if (verticalTotalDiff > 0.5f)
            {
                return false;
            }

            if (beenAssigned)
            {
                float punchLength = GetFlatDistance(firstSpeedPoint, point);
                float punchTime = pointTime - firstSpeedTime;
                if (punchLength > 0.3f && punchTime < 1f && angle < 15f)
                {
                    punchMade = true;
                }
            }
            else
            {
                // Debug.Log(speed);
                if (speed > 1.5f)
                {
                    beenAssigned = true;
                    firstSpeedPoint = point;
                    firstSpeedTime = pointTime;
                }
            }
        }
        return punchMade;
    }

    private float GetFlatAngle(Vector3 first, Vector3 second) {
        Vector3 secondFlat = second;
        secondFlat.y = first.y;

        return Vector3.Angle(Vector3.Normalize(first), Vector3.Normalize(secondFlat));
    }

    private float GetFlatDistance(Vector3 first, Vector3 second)
    {
        Vector3 secondFlat = second;
        secondFlat.y = first.y;

        return Vector3.Distance(first, secondFlat);
    }
}
