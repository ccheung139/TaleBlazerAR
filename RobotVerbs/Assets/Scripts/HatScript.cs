using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HatScript : MonoBehaviour {
    public Camera arCamera;
    public Text resultText;
    public bool isImposter;
    public GestureButton gestureButtonScript;
    public GameObject selectedSphere;
    public GameObject hatPrefab;

    private GameObject newHat;
    private bool hatExists;
    private float offset = 0.5f;
    private bool robotHasHat = false;

    void Update () {
        if (!SpawnCharacters.hatOn || !selectedSphere.activeSelf) {
            return;
        }
        if (robotHasHat) {
            gestureButtonScript.FinishGesture ();
            resultText.text = "This robot already has a hat!";
            return;
        }
        if (!gestureButtonScript.holdingDown && gestureButtonScript.failedGesture && resultText.text == "") {
            resultText.text = "You didn't place a hat!";
            gestureButtonScript.failedGesture = false;
            hatExists = false;
            Destroy (newHat);
        }
        CheckHatPlaced ();
    }

    private void CheckHatPlaced () {
        if (!gestureButtonScript.holdingDown) {
            return;
        }

        if (!hatExists) {
            newHat = Instantiate (hatPrefab, transform.position + new Vector3 (0, 0.8f, 0), Quaternion.identity, transform);
            hatExists = true;
        }

        newHat.transform.position = (arCamera.transform.position + arCamera.transform.forward * offset);
        newHat.transform.rotation = new Quaternion (0.0f, arCamera.transform.rotation.y, 0.0f, arCamera.transform.rotation.w);

        float distance = Vector3.Distance (newHat.transform.position, selectedSphere.transform.position);
        if (distance < .2f) {
            newHat.transform.position = selectedSphere.transform.position - new Vector3 (0, .09f, 0);
            gestureButtonScript.FinishGesture ();
            resultText.text = "You placed the hat!";
            hatExists = false;
            robotHasHat = true;
        }
    }
}