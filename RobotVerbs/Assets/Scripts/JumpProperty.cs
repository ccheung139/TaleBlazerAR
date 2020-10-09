using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpProperty : MonoBehaviour {
    public Camera arCamera;
    public Text resultText;
    public bool isImposter;
    public GestureButton gestureButtonScript;
    public GameObject selectedSphere;

    private float peakPoint;
    private float peakTime;

    private bool isJumping = false;

    private bool robotJump = false;
    private float robotTimer = 0f;
    private float robotTimerTotal = 1.0f;

    void Update () {
        CheckRobotJump ();
        if (!SpawnCharacters.jumpOn || !selectedSphere.activeSelf) {
            return;
        }

        if (!gestureButtonScript.holdingDown && gestureButtonScript.failedGesture && resultText.text == "") {
            resultText.text = "You didn't jump!";
            isJumping = false;
            gestureButtonScript.failedGesture = false;
        }
        CheckJump ();
    }

    private void CheckJump () {
        if (!gestureButtonScript.holdingDown) {
            return;
        }

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

        if (peakHeightDifference >= .2f && currentHeightDifference <= .1f && timeBackToBottom <= .3f) {
            gestureButtonScript.FinishGesture ();
            resultText.text = "You jumped!";
            isJumping = false;

            ThrustRobot ();
        }
    }

    private void ThrustRobot () {
        Vector3 offset = isImposter ? new Vector3 (0, 3f, 0) : new Vector3 (0, 1f, 0);
        transform.position = transform.position + offset;
        robotJump = true;
    }

    private void CheckRobotJump () {
        if (!robotJump) {
            return;
        }
        if (robotTimer >= robotTimerTotal) {
            robotTimer = 0f;
            robotJump = false;
            Vector3 offset = isImposter ? new Vector3 (0, 3f, 0) : new Vector3 (0, 1f, 0);
            transform.position = transform.position - offset;
        } else {
            robotTimer += Time.deltaTime * 1.0f;
        }
    }
}