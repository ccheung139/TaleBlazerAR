using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveScript : MonoBehaviour {
    public Camera arCamera;
    public Text resultText;
    public bool isImposter;
    public GestureButton gestureButtonScript;
    public GameObject selectedSphere;

    private bool leftRotate = false;
    private bool rightRotate = false;

    private float leftTime;
    private float rightTime;

    void Update () {
        if (!SpawnCharacters.waveOn || !selectedSphere.activeSelf) {
            return;
        }
        if (!gestureButtonScript.holdingDown && gestureButtonScript.failedGesture && resultText.text == "") {
            resultText.text = "You didn't wave!";
            gestureButtonScript.failedGesture = false;
            return;
        }

        CheckWave ();
    }

    private void CheckWave () {
        if (!gestureButtonScript.holdingDown) {
            return;
        }

        float rotation = Input.acceleration.x;
        if (rotation < -0.8f) {
            leftRotate = true;
            leftTime = Time.time;
        } else if (rotation > 0.8f) {
            rightRotate = true;
            rightTime = Time.time;
        }

        float timeDifference = Math.Abs (leftTime - rightTime);
        if (leftRotate && rightRotate && timeDifference <= 0.4f) {
            gestureButtonScript.FinishGesture ();
            resultText.text = "You waved!";

            leftRotate = false;
            rightRotate = false;
        }
    }
}