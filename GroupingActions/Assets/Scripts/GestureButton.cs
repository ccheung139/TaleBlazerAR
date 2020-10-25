using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestureButton : MonoBehaviour {
    public Camera arCamera;
    public Text resultText;
    public Button startButton;

    public bool holdingDown = false;
    public bool failedGesture = false;
    public Vector3 startPosition;
    public float timer = 0.0f;
    public float timerTotal = 10.0f;

    void Update () {
        // if (timer >= timerTotal && holdingDown) {
        //     ReleasedButton ();
        // } else {
        //     timer += Time.deltaTime * 1.0f;
        // }
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
        resultText.text = "";
    }

    public void FinishGesture () {
        holdingDown = false;
        gameObject.SetActive (false);
        startButton.gameObject.SetActive (true);
        timer = 0;
    }
}