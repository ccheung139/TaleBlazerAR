using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BirdAttraction : MonoBehaviour {
    public InventoryScript invScript;
    public ToastThrownScript toastThrownScript;
    public HouseScript houseScript;
    public Camera arCamera;
    public GameObject breadOwnedPrefab;
    public Button cancelButton;

    private bool isMoving = false;
    private bool isMovingToast = false;
    private Vector3 targetPosition;
    private Vector3 toastPosition;
    private float movementSpeed = 0.5f;
    private GameObject placedLureBread;
    private GameObject minToast;
    private GameObject toastBeingEaten;
    private Quaternion targetRotation;

    private bool breadMoved = false;
    private bool toastAttracted = false;
    private bool foundToast = false;
    private bool completed = false;

    void Update () {
        if (completed) {
            return;
        }
        if (isMoving) {
            gameObject.SendMessage ("Scared");
            MoveBirdFlee ();
            return;
        } else if (isMovingToast) {
            gameObject.SendMessage ("Scared");
            MoveBirdToast ();
            return;
        }

        HandleMovements ();
        HandleThrownBread ();
    }

    private void HandleThrownBread () {
        if (foundToast) {
            RotateBirdToast ();
            gameObject.SendMessage ("Peck");
            return;
        } else if (toastThrownScript.allToasts.Count == 0 || toastAttracted) {
            return;
        }
        float minDistance = 10000;
        minToast = toastThrownScript.allToasts[0];
        foreach (GameObject toast in toastThrownScript.allToasts) {
            float distance = Vector3.Distance (toast.transform.position, transform.position);
            if (distance < minDistance) {
                minDistance = distance;
                minToast = toast;
            }
        }
        if (minDistance <= .5f) {
            toastPosition = minToast.transform.position;
            isMovingToast = true;
            toastAttracted = true;
            toastBeingEaten = minToast;
        }
    }

    private void HandleMovements () {
        float distance = Vector3.Distance (gameObject.transform.position, arCamera.transform.position);
        if (distance <= 2f && !breadMoved && !toastAttracted) {
            targetRotation = Quaternion.LookRotation (arCamera.transform.position);
            RotateBird ();
        }
        if (distance <= 1f) {
            if (!toastAttracted) {
                if (invScript.breadInHand.activeSelf) {
                    if (distance <= .7f) {
                        MoveBread ();
                    }
                } else if (!breadMoved) {
                    isMoving = true;
                    GenerateTargetPosition ();
                    gameObject.SendMessage ("Scared");
                }
            }
            if (distance <= 0.2f) {
                if (placedLureBread != null) {
                    Destroy (placedLureBread);
                }
                else if (toastBeingEaten != null) {
                    toastThrownScript.RemoveAllToasts();
                }
                gameObject.SetActive (false);
                houseScript.MoveBirdToHouse (gameObject);
                completed = true;
            }
        }
        if (breadMoved) {
            gameObject.SendMessage ("Peck");
        }
    }

    private void MoveBirdToast () {
        if (Vector3.Distance (transform.position, toastPosition) <= 0.15f) {
            isMovingToast = false;
            foundToast = true;
            return;
        }

        float step = movementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards (transform.position, toastPosition, step);
        RotateBirdToast ();
    }

    private void MoveBirdFlee () {
        if (Vector3.Distance (transform.position, targetPosition) <= 0.03f) {
            isMoving = false;
            return;
        }

        float step = movementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards (transform.position, targetPosition, step);
    }

    private void RotateBird () {
        var direction = arCamera.transform.position - transform.position;
        direction.y = 0;
        var newRotation = Quaternion.LookRotation (direction) * Quaternion.Euler (0, 0, 0);
        transform.rotation = Quaternion.Slerp (transform.rotation, newRotation, Time.deltaTime * 1f);
    }

    private void RotateBirdToast () {
        var direction = toastPosition - transform.position;
        direction.y = 0;
        var newRotation = Quaternion.LookRotation (direction) * Quaternion.Euler (0, 0, 0);
        transform.rotation = Quaternion.Slerp (transform.rotation, newRotation, Time.deltaTime * 3f);
    }

    private void MoveBread () {
        if (breadMoved) {
            return;
        }
        breadMoved = true;
        Vector3 inFrontOfBird = transform.position + transform.forward * 0.1f;

        Vector3 inFrontVec = transform.forward * 0.5f;
        Quaternion orientation = Quaternion.LookRotation (inFrontVec, Vector3.up);
        placedLureBread = Instantiate (breadOwnedPrefab, inFrontOfBird, orientation);
        invScript.breadInHand.SetActive (false);
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