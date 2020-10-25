using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveWatcher : MonoBehaviour {
    public bool CheckWave (List<float> pointsInTime, List<float> rotationTimes) {
        bool leftRotate = false;
        bool rightRotate = false;
        float leftTime = 0;
        float rightTime = 0;

        for (int i = 0; i < rotationTimes.Count; i++) {
            float rotation = rotationTimes[i];
            if (rotation < -0.8f) {
                leftRotate = true;
                leftTime = pointsInTime[i];
            } else if (rotation > 0.8f) {
                rightRotgate = true;
                rightTime = pointsInTime[i];
            }

            float timeDifference = Math.Abs (leftTime - rightTime);
            if (leftRotate && rightRotate && timeDifference <= 0.4f) {
                return true;
            }
        }
        return false;
    }
}