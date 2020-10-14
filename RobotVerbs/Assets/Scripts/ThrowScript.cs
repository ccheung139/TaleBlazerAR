using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrowScript : MonoBehaviour {
    public Camera arCamera;
    public Text resultText;
    public bool isImposter;
    public GestureButton gestureButtonScript;
    public GameObject selectedSphere;
    public GameObject throwBallPrefab;

    private bool isThrowing = false;
    private float lastDistance = 0;
    private float speed = 0;
    private Vector3 startPos;
    private Vector3 finalPos;
    private float maxSpeed = 0;
    private GameObject spawnedBall;

    private float timer = 0;
    private float timerTotal = 3.0f;

    void Update () {
        MoveBall ();
        CheckKillBall ();

        if (!SpawnCharacters.throwOn || !selectedSphere.activeSelf || spawnedBall != null) {
            return;
        }

        if (!gestureButtonScript.holdingDown && gestureButtonScript.failedGesture && resultText.text == "") {
            resultText.text = "You didn't throw";
            gestureButtonScript.failedGesture = false;

            isThrowing = false;
            maxSpeed = 0;
            lastDistance = 0;
            return;
        }

        float distance = Vector3.Distance (arCamera.transform.position, transform.position);
        float distanceTraveled = lastDistance - distance;
        lastDistance = distance;
        speed = distanceTraveled / (Time.deltaTime);
        HandleSpeed (speed);
    }

    private void HandleSpeed (float speed) {
        if (speed > 2.0f) {
            if (!isThrowing && resultText.text == "") {
                isThrowing = true;
                startPos = arCamera.transform.position;
            }
            maxSpeed = Math.Max (speed, maxSpeed);
        } else {
            if (isThrowing) {
                finalPos = arCamera.transform.position;
                Debug.Log (Vector3.Distance (finalPos, startPos));
                if (Vector3.Distance (finalPos, startPos) >= .3f) {
                    ThrowOccured ();
                }
            }
            isThrowing = false;
        }
    }

    private void ThrowOccured () {
        resultText.text = "You threw!";
        lastDistance = 0;
        gestureButtonScript.FinishGesture ();
        spawnedBall = Instantiate (throwBallPrefab, finalPos, Quaternion.identity);
    }

    private void MoveBall () {
        if (spawnedBall == null || startPos == null || finalPos == null) {
            return;
        }
        float step = maxSpeed * Time.deltaTime;
        spawnedBall.transform.position += (finalPos - startPos) * step;
    }

    private void CheckKillBall () {
        if (spawnedBall == null) {
            return;
        }
        if (timer >= timerTotal) {
            timer = 0;
            Destroy (spawnedBall);
            spawnedBall = null;
        } else {
            timer += Time.deltaTime * 1.0f;
        }
    }

}