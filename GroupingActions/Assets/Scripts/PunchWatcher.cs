using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PunchWatcher : MonoBehaviour {
    public Camera arCamera;
    public GestureButton gestureButtonScript;
    public Text resultText;

    private Queue<float> lastFiveSpeeds = new Queue<float> ();
    private int sizeOfQueue = 0;
    private float lastDistance = 0;
    private float cumulativeSpeed = 0;
    private Vector3 startPunchPosition;

    void Update () {
        if (!gestureButtonScript.holdingDown) {
            Restart ();
            return;
        }

        Vector3 newPosition = new Vector3 (arCamera.transform.position.x, 0, arCamera.transform.position.z);
        Vector3 endPosition = new Vector3 (transform.position.x, 0, transform.position.z);

        float distance = Vector3.Distance (newPosition, endPosition);
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
        float verticalDistance = arCamera.transform.position.y - gestureButtonScript.startPosition.y;
        if (distance > 1f && avgSpeed >= .25f && gestureButtonScript.holdingDown && verticalDistance < .1f) {
            // Debug.Log ("avgSpeed : " + avgSpeed);
            // Debug.Log ("You punched!");
            gestureButtonScript.FinishGesture ();
            resultText.text += "you punched! ";
            Restart ();
        }
    }

    private void Restart () {
        lastFiveSpeeds = new Queue<float> ();
        sizeOfQueue = 0;
        lastDistance = 0;
        cumulativeSpeed = 0;
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