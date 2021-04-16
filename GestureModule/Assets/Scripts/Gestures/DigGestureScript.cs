using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigGestureScript : MonoBehaviour
{
    public bool CheckDig(List<Vector3> points, List<float> pointTimes, Vector3 upVector, Vector3 rightVector)
    {
        bool digPerformed = false;

        bool firstHighest = false;
        Vector3 highestPoint = new Vector3(0, 0, 0);

        bool lowestFound = false;
        Vector3 lowestPoint = new Vector3(0, 0, 0);

        float digRadius = 0.3f;

        for (int i = 1; i < points.Count; i++)
        {
            Vector3 point = points[i];
            float pointTime = pointTimes[i];

            if (GetRightDistance(points[0], point, rightVector) > 0.4f || GetRightDistance(points[0], point, -rightVector) > 0.4f) {
                return false;
            }

            if (firstHighest)
            {

                if (lowestFound)
                {
                    if (point.y - lowestPoint.y > digRadius * 0.8f)
                    {
                        digPerformed = true;
                    }
                }
                else
                {
                    if (point.y > highestPoint.y)
                    {
                        highestPoint = point;
                        continue;
                    }

                    if (highestPoint.y - point.y > digRadius)
                    {
                        lowestFound = true;
                        lowestPoint = point;
                    }

                }
            }
            else
            {
                highestPoint = point;
                firstHighest = true;
            }

        }
        return digPerformed;
    }

    private float GetRightDistance(Vector3 point1, Vector3 point2, Vector3 rightVector)
    {
        Vector3 difference = point2 - point1;

        float projection = Vector3.Dot(difference, rightVector) / (rightVector.magnitude);
        return projection;
    }
}
