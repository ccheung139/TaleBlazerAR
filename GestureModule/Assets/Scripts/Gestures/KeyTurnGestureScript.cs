using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyTurnGestureScript : MonoBehaviour
{
    public bool CheckKeyTurn(List<Vector3> eulerAngles, List<Vector3> points)
    {

        bool keyTurned = false;
        bool flat = false;

        for (int i = 1; i < eulerAngles.Count; i++)
        {
            Vector3 eulerAngle = eulerAngles[i];
            Vector3 point = points[i];

            if (Vector3.Distance(point, points[0]) > 0.5f)
            {
                return false;
            }

            if (flat)
            {
                if (eulerAngle.z > 85 && eulerAngle.z < 110)
                {
                    keyTurned = true;
                }
            }
            else
            {
                if (eulerAngle.x > 80)
                {
                    flat = true;
                }
            }
        }

        return keyTurned;
    }
}
