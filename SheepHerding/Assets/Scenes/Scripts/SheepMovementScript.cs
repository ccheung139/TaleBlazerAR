using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepMovementScript : MonoBehaviour {
    public Camera arCamera;
    public GameObject barn;
    public System.Random rand;
    public BarnAndSheepScript barnAndSheepScript;
    public Vector3 v3Center;
    public Vector3 v3Extents;

    private bool isMoving = false;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private bool isHerding = false;
    private Vector3 herdPosition;
    private Quaternion herdRotation;

    private float stillTimer = 0;
    private float moveAfterSeconds = 5.0f;

    void Update () {
        if (isHerding) {
            HandleHerdMovements ();
        } else {
            float distance = Vector3.Distance (arCamera.transform.position, transform.position);
            if (distance < 1.2f) {
                isHerding = true;
                isMoving = false;
                stillTimer = 0;
                herdPosition = GenerateHerdPosition ();
                herdRotation = Quaternion.LookRotation (herdPosition - transform.position);
                return;
            }
            HandleNormalMovements ();
        }

    }

    private void HandleNormalMovements () {
        if (isMoving) {
            MoveSheep (targetPosition, .5f);
            RotateSheep (targetRotation);
        } else {
            if (stillTimer >= moveAfterSeconds) {
                isMoving = true;
                targetPosition = GenerateTargetPosition ();
                stillTimer = 0;
                moveAfterSeconds = (float) ((rand.NextDouble () * 8.0) + 3.0);
                targetRotation = Quaternion.LookRotation (targetPosition - transform.position);
            } else {
                stillTimer += Time.deltaTime * 1.0f;
            }
        }
    }

    private void MoveSheep (Vector3 newPosition, float movementSpeed) {

        if (transform.position == newPosition || barnAndSheepScript.HittingSidesOfBarn (newPosition)) {
            isMoving = false;
            isHerding = false;
            stillTimer = 0;
            return;
        }

        float step = movementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards (transform.position, newPosition, step);

        if (barnAndSheepScript.InsideBarn (newPosition)) {

            Destroy (gameObject);
        }
    }

    private bool CheckInBounds (Vector3 position) {
        Vector3 v3CenterFlat = new Vector3 (v3Center.x, position.y, v3Center.z);
        Vector3 v3ExtentExpanded = new Vector3 (v3Extents.x * 2f, 100f, v3Extents.z * 2f);
        Bounds bounds = new Bounds (v3CenterFlat, v3ExtentExpanded);
        return bounds.Contains (position);
    }

    private void RotateSheep (Quaternion newRotation) {
        float movementSpeed = 200f;
        var step = movementSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards (transform.rotation, newRotation, step);
    }

    private Vector3 GenerateTargetPosition () {
        Vector3 newPosition = RandomPosition ();
        while (!CheckInBounds (newPosition)) {
            newPosition = RandomPosition ();
        }
        return newPosition;
    }

    private Vector3 RandomPosition () {
        Vector3 currentPosition = transform.position;
        float randX = (float) ((rand.NextDouble () * 10.0) - 5.0);
        float randZ = (float) ((rand.NextDouble () * 10.0) - 5.0);
        Vector3 newPosition = currentPosition + new Vector3 ((rand.Next (1) == 0) ? randX : -randX, 0, (rand.Next (1) == 0) ? randZ : -randZ);
        return newPosition;
    }

    private void HandleHerdMovements () {
        MoveSheep (herdPosition, .8f);
        RotateSheep (herdRotation);
    }

    private Vector3 GenerateHerdPosition () {
        Vector3 sheepFlatVector = new Vector3 (transform.position.x, 0, transform.position.z);
        Vector3 cameraFlatVector = new Vector3 (arCamera.transform.position.x, 0, arCamera.transform.position.z);
        Vector3 direction = sheepFlatVector - cameraFlatVector;

        herdPosition = cameraFlatVector + (direction * 1.3f);
        herdPosition.y = transform.position.y;
        if (!CheckInBounds (herdPosition)) {
            isHerding = false;
            isMoving = false;
        }
        return herdPosition;
    }
}