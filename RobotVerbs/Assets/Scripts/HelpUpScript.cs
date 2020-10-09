using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpUpScript : MonoBehaviour {
    public Camera arCamera;
    public Text resultText;
    public bool isImposter;
    public GestureButton gestureButtonScript;
    public GameObject selectedSphere;

    private bool initialGrab = false;
    private float offset = 0.5f;
    private Vector3 startPosition;
    private Quaternion startRotation;

    void Update () {
        if (!SpawnCharacters.helpUpOn || !selectedSphere.activeSelf) {
            return;
        }
        if (!gestureButtonScript.holdingDown && gestureButtonScript.failedGesture && resultText.text == "") {
            resultText.text = "You didn't help the robot up!";
            gestureButtonScript.failedGesture = false;

            if (initialGrab) {
                transform.position = startPosition;
                transform.rotation = startRotation;
                initialGrab = false;
            }
            return;
        }
        CheckHelpUp ();
    }

    private void CheckHelpUp () {
        if (!gestureButtonScript.holdingDown) {
            return;
        }

        float distance = Vector3.Distance (arCamera.transform.position, selectedSphere.transform.position);
        if (!initialGrab) {
            if (distance <= .5f) {
                initialGrab = true;
                startPosition = transform.position;
                startRotation = transform.rotation;
            }
        } else {
            transform.position = (arCamera.transform.position + arCamera.transform.forward * offset) - new Vector3 (0, 1f, 0);
            transform.rotation = new Quaternion (0.0f, arCamera.transform.rotation.y, 0.0f, arCamera.transform.rotation.w);

            if (arCamera.transform.position.y >= 0.4f) {

                transform.position = new Vector3 (transform.position.x, -0.8f, transform.position.z);
                gestureButtonScript.FinishGesture ();
                resultText.text = "You helped the robot up!";
                initialGrab = false;
            }
        }
    }
}