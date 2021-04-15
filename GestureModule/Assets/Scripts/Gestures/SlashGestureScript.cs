using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashGestureScript : MonoBehaviour
{
    public bool CheckSlash(List<Vector3> points, List<float> pointTimes, Vector3 upVector, Vector3 forwardVector, Vector3 rightVector)
    {

        bool beenAssigned = false;
        Vector3 highestPoint = new Vector3(0, 0, 0);
        float highestY = 0;
        float highestTime = 0;

        bool slashMade = false;
        Vector3 slashPoint = new Vector3(0, 0, 0);


        for (int i = 1; i < points.Count; i++)
        {
            Vector3 point = points[i];
            float pointTime = pointTimes[i];

            if (slashMade)
            {
                float leftDistance = GetForwardDistance(slashPoint, point, -rightVector);
                if (leftDistance > 0.2f)
                {
                    return false;
                }
            }

            if (beenAssigned)
            {
                if (point.y > highestY)
                {
                    highestPoint = point;
                    highestY = point.y;
                    highestTime = pointTime;
                }

                Vector3 slashDirection = point - highestPoint;
                float angle = Vector3.Angle(Vector3.Normalize(upVector), Vector3.Normalize(slashDirection));
                float slashTime = pointTime - highestTime;
                float slashDistance = Vector3.Distance(point, highestPoint);

                float difference = Vector3.Distance(points[i - 1], point);
                float timeDiff = pointTimes[i] - pointTimes[i - 1];
                float speed = difference / (timeDiff);

                float forwardDistance = GetForwardDistance(points[0], point, forwardVector);
                float rightDistance = GetForwardDistance(highestPoint, point, rightVector);

                if (angle > 90 && angle < 180 && slashTime < 1.0f && slashDistance > 0.2f && speed > 1.0f && forwardDistance < 0.2f && rightDistance > 0.2f)
                {
                    slashMade = true;
                    slashPoint = point;
                }

                if (slashMade)
                {
                    if (angle < 90 || angle > 180)
                    {
                        return false;
                    }
                }
            }
            else
            {
                highestPoint = point;
                highestY = point.y;
                highestTime = pointTime;
                beenAssigned = true;
            }
        }

        return slashMade;
    }

    private float GetForwardDistance(Vector3 point1, Vector3 point2, Vector3 forwardVector)
    {
        Vector3 difference = point2 - point1;

        float projection = Vector3.Dot(difference, forwardVector) / (forwardVector.magnitude);
        return projection;
    }
}
