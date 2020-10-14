using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefendScript : MonoBehaviour {
    public Camera arCamera;
    public Text resultText;
    public bool isImposter;
    public GestureButton gestureButtonScript;
    public GameObject selectedSphere;
    public GameObject missilePrefab;

    private bool shotMissile = false;
    private GameObject spawnedMissile;
    private LineRenderer beam;
    private Vector3 cameraOrigin;
    private Vector3 originOffset;
    private Vector3 endPoint;

    void Start () {
        beam = arCamera.gameObject.GetComponent<LineRenderer> ();
        beam.startWidth = 0.1f;
        beam.endWidth = 0.1f;
        beam.enabled = false;
    }

    void Update () {
        ConfigureLaser ();
        if (!SpawnCharacters.defendOn || !selectedSphere.activeSelf) {
            return;
        }
        if (!gestureButtonScript.holdingDown && gestureButtonScript.failedGesture && resultText.text == "") {
            resultText.text = "You didn't defend yourself";
            gestureButtonScript.failedGesture = false;
            shotMissile = false;
            beam.enabled = false;
            Destroy (spawnedMissile);
            return;
        }

        CheckDefended ();
    }

    private void ConfigureLaser () {
        cameraOrigin = arCamera.transform.position + arCamera.transform.forward * 0.2f;
        originOffset = cameraOrigin + arCamera.transform.right * 0.1f - new Vector3 (0, 0.1f, 0);
        endPoint = cameraOrigin + arCamera.transform.forward * 1f;

        beam.SetPosition (0, originOffset);
        beam.SetPosition (1, endPoint);
    }

    private void CheckDefended () {
        if (!gestureButtonScript.holdingDown) {
            beam.enabled = false;
            return;
        }

        beam.enabled = true;

        if (!shotMissile) {
            ShootMissile ();
            shotMissile = true;
            return;
        } else {
            MoveMissile (arCamera.transform.position, 0.6f);
        }

        CheckHit ();
    }

    private void ShootMissile () {
        spawnedMissile = Instantiate (missilePrefab, transform.position + new Vector3 (0, 0.5f, 0), Quaternion.Euler (-90, 0, 0));
    }

    private void MoveMissile (Vector3 targetPosition, float movementSpeed) {
        float step = movementSpeed * Time.deltaTime;
        spawnedMissile.transform.position = Vector3.MoveTowards (spawnedMissile.transform.position, targetPosition, step);
    }

    void CheckHit () {
        RaycastHit hit;
        Vector3 direction = endPoint - originOffset;
        if (Physics.Raycast (originOffset, direction, out hit, Vector3.Distance (endPoint, originOffset))) {
            if (hit.collider.gameObject == spawnedMissile) {
                shotMissile = false;
                gestureButtonScript.FinishGesture ();
                resultText.text = "You defended yourself!";
                beam.enabled = false;
                Destroy (spawnedMissile);
            }
        } else if (Vector3.Distance (spawnedMissile.transform.position, arCamera.transform.position) < .2f) {
            shotMissile = false;
            gestureButtonScript.FinishGesture ();
            resultText.text = "You didn't defend yourself!";
            beam.enabled = false;
            Destroy (spawnedMissile);
        }
    }
}