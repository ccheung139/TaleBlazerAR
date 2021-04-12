using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGestureScript : MonoBehaviour
{
    public bool CheckThrow(List<Vector3> points, List<float> pointTimes, Vector3 forwardVector)
    {

        float firstYPos = points[0].y;
        float earliestTime = pointTimes[0];

        bool speedAssigned = false;
        Vector3 firstSpeedPoint = new Vector3(0, 0, 0);
        float firstSpeedTime = 0;

        bool broughtBack = false;
        bool throwMade = false;

        for (int i = 1; i < points.Count; i++)
        {
            Vector3 point = points[i];
            float pointTime = pointTimes[i];

            float distance = Vector3.Distance(points[i - 1], point);
            float timeDiff = pointTimes[i] - pointTimes[i - 1];

            float speed = distance / timeDiff;

            Vector3 direction = point - points[0];
            float angle = Vector3.Angle(forwardVector, direction);
            float distanceFromStart = Vector3.Distance(point, points[0]);

            float heightFromStart = point.y - points[0].y;

            if (!broughtBack)
            {
                if (angle <= 250 && angle >= 90 && distanceFromStart > 0.2f)
                {
                    broughtBack = true;
                }
            }
            else
            {
                if (speedAssigned)
                {
                    float distanceFromFirstSpeed = Vector3.Distance(firstSpeedPoint, point);
                    float timeFromFirstSpeed = pointTime - firstSpeedTime;
                    if (angle < 90 && angle > 10 && distanceFromFirstSpeed > 0.3f && timeFromFirstSpeed < 1.0f)
                    {
                        throwMade = true;
                    }
                }
                else
                {
                    if (speed > 1.5f)
                    {
                        speedAssigned = true;
                        firstSpeedPoint = point;
                        firstSpeedTime = pointTime;
                    }
                }

            }


        }

        return throwMade;


        // bool beenAssigned = false;
        // Vector3 firstSpeedPoint = new Vector3(0, 0, 0);
        // float firstSpeedTime = 0;

        // bool punchMade = false;
        // for (int i = 1; i < points.Count; i++)
        // {
        //     Vector3 point = points[i];
        //     float pointTime = pointTimes[i];

        //     Vector3 direction = point - firstSpeedPoint;
        //     float angle = GetFlatAngle(forwardVector, direction);

        //     // float GetFlatDistance;
        //     float flatDiff = GetFlatDistance(points[i - 1], points[i]);
        //     float verticalTotalDiff = point.y - firstYPos;
        //     float timeDiff = pointTimes[i] - pointTimes[i - 1];

        //     float speed = flatDiff / (timeDiff);

        //     if (verticalTotalDiff > 0.5f)
        //     {
        //         return false;
        //     }

        //     if (beenAssigned)
        //     {
        //         float punchLength = GetFlatDistance(firstSpeedPoint, point);
        //         float punchTime = pointTime - firstSpeedTime;
        //         if (punchLength > 0.3f && punchTime < 1f && angle < 15f)
        //         {
        //             punchMade = true;
        //         }
        //     }
        //     else
        //     {
        //         // Debug.Log(speed);
        //         if (speed > 1.5f)
        //         {
        //             beenAssigned = true;
        //             firstSpeedPoint = point;
        //             firstSpeedTime = pointTime;
        //         }
        //     }
        // }
        return false;
    }
}
