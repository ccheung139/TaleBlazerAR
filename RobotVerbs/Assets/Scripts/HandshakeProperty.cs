using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandshakeProperty : MonoBehaviour {
    public Camera arCamera;
    public Text resultText;
    public bool isImposter;
    public GestureButton gestureButtonScript;
    public GameObject selectedSphere;

    void Start () {
        Accelerometer.Instance.OnShake += ShakeOccured;
    }

    void Update () {
        if (!SpawnCharacters.handshakeOn) {
            return;
        }

        if (!gestureButtonScript.holdingDown && gestureButtonScript.failedGesture && resultText.text == "") {
            resultText.text = "You didn't shake the hand!";
            gestureButtonScript.failedGesture = false;
        }
    }

    private void ShakeOccured () {
        if (!gestureButtonScript.holdingDown || !selectedSphere.activeSelf || !SpawnCharacters.handshakeOn) {
            return;
        }

        Vector3 newPosition = arCamera.transform.position;
        float distance = Vector3.Distance (gestureButtonScript.startPosition, newPosition);
        if (distance > 0.4f) {
            resultText.text = "You shook a hand!";

            Rotate ();
            gestureButtonScript.FinishGesture ();
        }
    }

    private void Rotate () {
        transform.position = transform.position + new Vector3 (0, 0, .5f);
        if (isImposter) {
            transform.rotation = Quaternion.Euler (0, 0, 180);
        }
    }
}