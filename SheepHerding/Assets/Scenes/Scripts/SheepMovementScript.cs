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
    public LoadSpaceScript loadSpaceScript;
    public Vector3 pivot;
    public Vector3 relative;

    public Bounds room1Bounds;
    public Bounds room2Bounds;
    public List<Bounds> connectingRoomBounds;

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
        List<Bounds> allBounds = new List<Bounds> (connectingRoomBounds);
        allBounds.Add (room1Bounds);
        allBounds.Add (room2Bounds);

        // Vector3 rotatedPosition = loadSpaceScript.RotatePointAroundPivot (position, pivot, relative);
        foreach (Bounds bound in allBounds) {
            if (bound.Contains (position)) {
                return true;
            }
        }
        return false;
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
        Vector3 rotatedPos = loadSpaceScript.RotatePointAroundPivot (newPosition, pivot, relative);
        return rotatedPos;
    }

    private Vector3 RandomPosition () {
        Bounds currentBounds = FindWhichBounds ();
        Vector3 newCenter = currentBounds.center;
        Vector3 newExtents = currentBounds.extents;
        float randX = (newCenter.x) + (float) (rand.NextDouble () * newExtents.x * (rand.Next (2) == 1 ? 1 : -1));
        float randZ = (newCenter.z) + (float) (rand.NextDouble () * newExtents.z * (rand.Next (2) == 1 ? 1 : -1));
        Vector3 newPosition = new Vector3 (randX, -0.8f, randZ);
        return newPosition;
    }

    private Bounds FindWhichBounds () {
        List<Bounds> allBounds = new List<Bounds> (connectingRoomBounds);
        allBounds.Add (room1Bounds);
        allBounds.Add (room2Bounds);
        foreach (Bounds bound in allBounds) {
            Vector3 rotatedPos = loadSpaceScript.RotatePointAroundPivot (transform.position, pivot, -relative);
            if (bound.Contains (rotatedPos)) {
                return bound;
            }
        }
        return allBounds[0];
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
        Vector3 rotatedPos = loadSpaceScript.RotatePointAroundPivot (herdPosition, pivot, -relative);
        if (!CheckInBounds (rotatedPos)) {
            isHerding = false;
            isMoving = false;
        }
        return herdPosition;
    }
}