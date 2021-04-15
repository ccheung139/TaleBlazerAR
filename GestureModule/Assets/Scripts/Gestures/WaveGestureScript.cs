using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveGestureScript : MonoBehaviour
{
    public bool CheckWave(List<Vector3> eulerAngles, List<Vector3> points, List<float> pointsInTime)
    {
        bool waved = false;

        bool leftRotate = false;
        bool rightRotate = false;
        float leftTime = 0;
        float rightTime = 0;

        for (int i = 1; i < eulerAngles.Count; i++)
        {
            Vector3 eulerAngle = eulerAngles[i];
            Vector3 point = points[i];

            if (eulerAngle.z > 45 && eulerAngle.z < 90)
            {
                leftRotate = true;
                leftTime = pointsInTime[i];
                // Debug.Log("left");
            }
            else if (eulerAngle.z > 270 && eulerAngle.z < 315)
            {
                rightRotate = true;
                rightTime = pointsInTime[i];
                // Debug.Log("right");
            }

            if (leftRotate && rightRotate)
            {
                Debug.Log("both");
                Debug.Log(Math.Abs(leftTime - rightTime));
                if (Math.Abs(leftTime - rightTime) < 0.5f)
                {
                    waved = true;
                }

            }
        }

        return waved;
    }
}
