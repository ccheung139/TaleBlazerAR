using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComeHereGestureScript : MonoBehaviour
{

    public bool CheckComeHere(List<Vector3> points, List<float> pointTimes, List<float> shakeTimes)
    {
        bool raisedArm = false;

        float firstYPos = points[0].y;
        float earliestTime = pointTimes[0];
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 point = points[i];
            float pointTime = pointTimes[i];

            if (GetFlatDistance(points[0], point) > 0.3f)
            {
                return false;
            }

            float currentYPos = point.y;
            float difference = currentYPos - firstYPos;
            if (!raisedArm && difference > 0.5f)
            {
                raisedArm = true;
                earliestTime = pointTime;
            }
            if (raisedArm)
            {
                if (difference < 0.3f)
                {
                    raisedArm = false;
                }
            }
        }

        if (raisedArm)
        {
            int raisedShakeCount = 0;
            foreach (float shakeTime in shakeTimes)
            {
                if (shakeTime >= earliestTime)
                {
                    raisedShakeCount += 1;
                }
            }
            if (raisedShakeCount >= 3)
            {
                return true;
            }
        }
        return false;
    }

    private float GetFlatDistance(Vector3 point1, Vector3 point2)
    {
        Vector3 flatP2 = point2;
        flatP2.y = point1.y;
        return Vector3.Distance(point1, flatP2);
    }

}