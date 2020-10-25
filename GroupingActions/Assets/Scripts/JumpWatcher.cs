using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpWatcher : MonoBehaviour {
    public Camera arCamera;
    public GestureButton gestureButtonScript;
    public Text resultText;

    private float peakPoint;
    private float peakTime;

    private bool isJumping = false;

    void Update () {

        if (!gestureButtonScript.holdingDown) {
            Restart ();
            return;
        }
        CheckJump ();
    }

    private void CheckJump () {

        if (!isJumping) {
            peakPoint = gestureButtonScript.startPosition.y;
            isJumping = true;
            return;
        }
        if (arCamera.transform.position.y > peakPoint) {
            peakPoint = arCamera.transform.position.y;
            peakTime = Time.time;
        }

        float peakHeightDifference = peakPoint - gestureButtonScript.startPosition.y;
        float currentHeightDifference = arCamera.transform.position.y - gestureButtonScript.startPosition.y;
        float timeBackToBottom = Time.time - peakTime;

        // Vector3 endPosition = new Vector3 (arCamera.transform.position.x, 0, arCamera.transform.position.z);
        // Vector3 startPosition = new Vector3 (gestureButtonScript.startPosition.x, 0, gestureButtonScript.startPosition.z);
        // float distanceNoHeight = Vector3.Distance (endPosition, startPosition);

        if (peakHeightDifference >= .2f && currentHeightDifference <= .1f && timeBackToBottom <= .5f) {
            gestureButtonScript.FinishGesture ();
            isJumping = false;
            resultText.text += "you jumped! ";
            Restart ();
        }
    }

    private void Restart () {
        isJumping = false;
    }
}