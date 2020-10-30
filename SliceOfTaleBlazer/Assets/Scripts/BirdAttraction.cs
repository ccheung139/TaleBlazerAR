using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BirdAttraction : MonoBehaviour {
    public InventoryScript invScript;
    public Camera arCamera;
    public GameObject breadOwnedPrefab;
    public Button cancelButton;

    private bool isMoving = false;
    private Vector3 targetPosition;
    private float movementSpeed = 0.5f;
    private GameObject placedLureBread;
    private Quaternion targetRotation;

    private bool breadMoved = false;

    void Update () {
        if (isMoving) {
            gameObject.SendMessage ("Scared");
            MoveBird ();
            return;
        }

        float distance = Vector3.Distance (gameObject.transform.position, arCamera.transform.position);
        if (distance <= 2f && !breadMoved) {
            targetRotation = Quaternion.LookRotation (arCamera.transform.position);
            RotateBird ();
        }
        if (distance <= 1f) {

            if (invScript.holdingBread) {
                if (distance <= .7f) {
                    MoveBread ();
                }
            } else if (!breadMoved) {
                isMoving = true;
                GenerateTargetPosition ();
                gameObject.SendMessage ("Scared");
            }

            if (distance <= 0.2f) {
                Destroy (placedLureBread);
                Destroy (gameObject);
            }
        }
        if (breadMoved) {
            gameObject.SendMessage ("Peck");
        }
    }

    private void MoveBird () {
        if (transform.position == targetPosition) {
            isMoving = false;
            return;
        }

        float step = movementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards (transform.position, targetPosition, step);
    }

    private void RotateBird () {
        var n = arCamera.transform.position - transform.position;
        n.y = 0;
        var newRotation = Quaternion.LookRotation (n) * Quaternion.Euler (0, 0, 0);

        transform.rotation = Quaternion.Slerp (transform.rotation, newRotation, Time.deltaTime * 1f);
    }

    private void MoveBread () {
        if (breadMoved) {
            return;
        }
        Vector3 inFrontOfBird = transform.position + transform.forward * 0.1f;

        Vector3 inFrontVec = transform.forward * 0.5f;
        Quaternion orientation = Quaternion.LookRotation (inFrontVec, Vector3.up);
        placedLureBread = Instantiate (breadOwnedPrefab, inFrontOfBird, orientation);
        breadMoved = true;
        invScript.holdingBread = false;
        Destroy (invScript.activeBread);

        cancelButton.gameObject.SetActive (false);
    }

    private void GenerateTargetPosition () {
        Vector3 birdFlatVector = new Vector3 (transform.position.x, 0, transform.position.z);
        Vector3 cameraFlatVector = new Vector3 (arCamera.transform.position.x, 0, arCamera.transform.position.z);
        Vector3 direction = birdFlatVector - cameraFlatVector;

        targetPosition = cameraFlatVector + (direction * 1.1f);
        targetPosition.y = transform.position.y;
    }
}