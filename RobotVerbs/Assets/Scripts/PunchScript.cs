using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PunchScript : MonoBehaviour {
    public Camera arCamera;
    public Text resultText;
    public bool isImposter;
    public GestureButton gestureButtonScript;
    public GameObject selectedSphere;

    private Queue<float> lastFiveSpeeds = new Queue<float> ();
    private int sizeOfQueue = 0;
    private float lastDistance = 0;
    private float cumulativeSpeed = 0;

    void Update () {
        if (!SpawnCharacters.punchOn || !selectedSphere.activeSelf) {
            return;
        }
        if (!gestureButtonScript.holdingDown && gestureButtonScript.failedGesture && resultText.text == "") {
            resultText.text = "You didn't punch!";
            gestureButtonScript.failedGesture = false;
            return;
        }

        float distance = Vector3.Distance (arCamera.transform.position, transform.position);
        float distanceTraveled = lastDistance - distance;
        lastDistance = distance;
        HandleSpeedQueue (distanceTraveled);

        if (sizeOfQueue < 5) {
            return;
        }
        float avgSpeed = cumulativeSpeed / sizeOfQueue;
        CheckPunch (avgSpeed, distance);
    }

    private void CheckPunch (float avgSpeed, float distance) {
        if (distance < 2f && avgSpeed >= .25f && gestureButtonScript.holdingDown) {
            transform.rotation = isImposter ? Quaternion.Euler (270, 270, 270) : Quaternion.Euler (90, 90, 90);
            gestureButtonScript.FinishGesture ();
            resultText.text = "You punched a robot!";
        }
    }

    private void HandleSpeedQueue (float distanceTraveled) {
        float speed = distanceTraveled / (Time.deltaTime);
        if (sizeOfQueue == 5) {
            float dequeuedSpeed = lastFiveSpeeds.Dequeue ();
            sizeOfQueue -= 1;
            cumulativeSpeed -= dequeuedSpeed;
        } else {
            lastFiveSpeeds.Enqueue (speed);
            sizeOfQueue += 1;
            cumulativeSpeed = speed;
        }
    }
}