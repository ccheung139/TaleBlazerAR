using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestureButton : MonoBehaviour {
    public Camera arCamera;
    public Text resultText;
    public GameObject selectedRobot;

    public bool holdingDown = false;
    public bool failedGesture = false;
    public Vector3 startPosition;
    public float timer = 0.0f;
    public float timerTotal = 3.0f;

    void Update () {
        if (SpawnCharacters.helpUpOn) {
            timerTotal = 10.0f;
        } else {
            timerTotal = 3.0f;
        }

        if (timer >= timerTotal && holdingDown) {
            ReleasedButton ();
        } else {
            timer += Time.deltaTime * 1.0f;
        }
    }

    public void HoldingDownButton () {
        if (!holdingDown) {
            startPosition = arCamera.transform.position;
            timer = 0;
            failedGesture = false;
        }
        holdingDown = true;
    }

    public void ReleasedButton () {
        FinishGesture ();
        failedGesture = true;
    }

    public void FinishGesture () {
        holdingDown = false;
        gameObject.SetActive (false);
        timer = 0;
    }
}