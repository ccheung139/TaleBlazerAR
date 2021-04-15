using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaxOnGestureScript : MonoBehaviour
{
    public GameObject marker;

    public bool CheckWaxOn(List<Vector3> points, List<float> pointTimes, Vector3 upVector, Vector3 rightVector)
    {
        float radius = 0.1f;


        bool highestPointAssigned = false;
        Vector3 highestPoint = new Vector3(0, 0, 0);
        float highestY = 0;

        Vector3 midPoint = new Vector3(0, 0, 0);
        Vector3 leftPoint = new Vector3(0, 0, 0);
        Vector3 bottomPoint = new Vector3(0, 0, 0);
        Vector3 rightPoint = new Vector3(0, 0, 0);

        bool leftReached = false;
        bool bottomReached = false;
        bool rightReached = false;

        bool waxOnMade = false;

        for (int i = 1; i < points.Count; i++)
        {
            Vector3 point = points[i];
            float pointTime = pointTimes[i];



            if (!highestPointAssigned)
            {
                highestPoint = point;
                highestY = point.y;
                midPoint = highestPoint - upVector * radius;
                leftPoint = midPoint - rightVector * radius;
                bottomPoint = midPoint - upVector * radius;
                rightPoint = midPoint + rightVector * radius;
                highestPointAssigned = true;

                // Instantiate(marker, highestPoint, Quaternion.identity);
                // Instantiate(marker, leftPoint, Quaternion.identity);
                // Instantiate(marker, bottomPoint, Quaternion.identity);
                // Instantiate(marker, rightPoint, Quaternion.identity);
            }
            else
            {

                if (Vector3.Distance(point, highestPoint) > 0.8f)
                {
                    return false;
                }
                if (!leftReached)
                {
                    if (point.y > highestY)
                    {
                        highestPoint = point;
                        highestY = point.y;
                        midPoint = highestPoint - upVector * radius;
                        leftPoint = midPoint - rightVector * radius;
                        bottomPoint = midPoint - upVector * radius;
                        rightPoint = midPoint + rightVector * radius;
                    }
                    if (CheckAtLeastAsFar(point, highestPoint, -rightVector) > 0 && CheckAtLeastAsFar(point, highestPoint, -upVector) > radius)
                    {
                        leftReached = true;
                    }
                }
                else
                {
                    if (!bottomReached)
                    {
                        if (CheckAtLeastAsFar(point, highestPoint, -rightVector) < radius
                            && CheckAtLeastAsFar(point, highestPoint, -upVector) > radius * 2)
                        {
                            bottomReached = true;
                        }
                    }
                    else
                    {
                        if (!rightReached)
                        {
                            if (CheckAtLeastAsFar(point, highestPoint, rightVector) > radius && CheckAtLeastAsFar(point, highestPoint, -upVector) > radius)
                            {
                                rightReached = true;
                            }
                        }
                        else
                        {
                            if (CheckAtLeastAsFar(point, highestPoint, -upVector) < radius / 2)
                            {
                                waxOnMade = true;

                            }
                        }
                    }
                }

            }

        }
        return waxOnMade;
    }

    private float CheckAtLeastAsFar(Vector3 point, Vector3 refPoint, Vector3 direction)
    {
        Vector3 difference = point - refPoint;
        float projection = Vector3.Dot(difference, direction) / (direction.magnitude);
        return projection;
    }
}
