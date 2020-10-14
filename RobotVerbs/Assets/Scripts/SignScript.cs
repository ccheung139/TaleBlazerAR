using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignScript : MonoBehaviour {
    public Camera arCamera;
    public Text resultText;
    public bool isImposter;
    public GestureButton gestureButtonScript;
    public GameObject selectedSphere;
    public GameObject letterPrefab;
    public GameObject penPrefab;

    private bool startSign = false;
    private GameObject spawnedLetter;
    private GameObject spawnedPen;
    private int shakeCounter = 0;

    private Vector3 originOffset;

    void Start () {
        Accelerometer.Instance.LighterShake += ShakeOccured;
    }

    void Update () {
        if (!SpawnCharacters.signOn || !selectedSphere.activeSelf) {
            return;
        }

        if (!gestureButtonScript.holdingDown && gestureButtonScript.failedGesture && resultText.text == "") {
            resultText.text = "You didn't sign!";
            gestureButtonScript.failedGesture = false;

            if (spawnedLetter != null) {
                Destroy (spawnedLetter);
                Destroy (spawnedPen);
            }
            spawnedLetter = null;
            spawnedPen = null;
            startSign = false;
            shakeCounter = 0;
            return;
        }

        if (gestureButtonScript.holdingDown && !startSign && spawnedLetter == null && resultText.text == "") {
            Vector3 newPosition = (transform.position + arCamera.transform.position) / 2f;
            spawnedLetter = Instantiate (letterPrefab, newPosition, Quaternion.identity);

            Vector3 cameraOrigin = arCamera.transform.position + arCamera.transform.forward * 0.2f;
            originOffset = cameraOrigin + arCamera.transform.right * 0.04f - new Vector3 (0, 0.1f, 0);

            Vector3 inFront = arCamera.transform.forward * 0.5f;
            Quaternion orientation = Quaternion.LookRotation (inFront, Vector3.up);
            spawnedPen = Instantiate (penPrefab, originOffset, orientation, arCamera.transform);
        }
    }

    private void ShakeOccured () {
        if (!gestureButtonScript.holdingDown || !selectedSphere.activeSelf || !SpawnCharacters.signOn || spawnedLetter == null) {
            return;
        }
        float distance = Vector3.Distance (spawnedLetter.transform.position, spawnedPen.transform.position);

        if (distance < 0.3f) {
            if (shakeCounter >= 3) {
                resultText.text = "You signed!";
                Destroy (spawnedLetter);
                Destroy (spawnedPen);
                spawnedLetter = null;
                spawnedPen = null;
                startSign = false;
                shakeCounter = 0;
                gestureButtonScript.FinishGesture ();
            } else {
                shakeCounter += 1;
            }
        }
    }
}