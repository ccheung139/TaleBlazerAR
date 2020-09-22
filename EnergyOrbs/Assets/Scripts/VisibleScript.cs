using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class VisibleScript : MonoBehaviour {

    public Camera arCamera;
    public Text text;
    public Button collectButton;

    public bool isSeen = false;

    private Queue<float> lastFiveSpeeds = new Queue<float> ();
    private int sizeOfQueue = 0;
    private float lastDistance = 0;
    private float cumulativeSpeed = 0;

    private float maxSpeed = 0;
    private float generateAfterSeconds = 5.0f;
    private float placementTimer = 0;

    private bool orbMoving = false;
    private Vector3 newOrbPosition = new Vector3 (0, 0, 0);

    void Start () {
        collectButton.onClick.AddListener (CollectOrb);
    }

    void Update () {
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer> ();
        if (orbMoving) {
            foreach (Material material in meshRenderer.materials) {
                material.color = Color.gray;
            }
            MoveOrb (newOrbPosition);
        }

        if (placementTimer < generateAfterSeconds) {
            placementTimer += Time.deltaTime * 1.0f;
            return;
        }

        float distance = Vector3.Distance (arCamera.transform.position, transform.position);
        float distanceTraveled = lastDistance - distance;
        lastDistance = distance;

        DetermineCollection (distance, meshRenderer);
        HandleSpeedQueue (distanceTraveled);

        if (sizeOfQueue < 5) {
            return;
        }
        float avgSpeed = cumulativeSpeed / sizeOfQueue;

        ScareOrbsAway (cumulativeSpeed, distance);
    }

    private void HandleSpeedQueue (float distanceTraveled) {
        float speed = distanceTraveled / (Time.deltaTime);
        if (sizeOfQueue == 5) {
            float dequeuedSpeed = lastFiveSpeeds.Dequeue ();
            sizeOfQueue -= 1;
            cumulativeSpeed -= dequeuedSpeed;
        } else {
            lastFiveSpeeds.Enqueue (speed);
            sizeOfQueue += 1;
            cumulativeSpeed = speed;
        }
    }

    private void ScareOrbsAway (float cumulativeSpeed, float distance) {
        if (cumulativeSpeed >.5f && distance < 2f) {
            orbMoving = true;
            newOrbPosition = findNewPosition ();

            if (OrbCollectorScript.potentialOrb == gameObject) {
                OrbCollectorScript.potentialOrb = null;
                OrbCollectorScript.foundObject = false;
                collectButton.gameObject.SetActive (false);
            }
        }
    }

    private void DetermineCollection (float distance, MeshRenderer meshRenderer) {
        if (distance < 1f && !OrbCollectorScript.foundObject && !orbMoving) {
            OrbCollectorScript.foundObject = true;
            OrbCollectorScript.potentialOrb = gameObject;

            foreach (Material material in meshRenderer.materials) {
                material.color = Color.red;
            }
            collectButton.gameObject.SetActive (true);
        }
    }

    private void CollectOrb () {
        if (OrbCollectorScript.potentialOrb != gameObject) {
            return;
        }
        OrbCollectorScript.collectedOrbs += 1;
        OrbCollectorScript.potentialOrb = null;
        OrbCollectorScript.foundObject = false;
        collectButton.gameObject.SetActive (false);
        Destroy (gameObject);
    }

    private Vector3 findNewPosition () {
        System.Random rand = new System.Random ();
        float randX = (float) ((rand.NextDouble () * 4) - 2.0);
        float randZ = (float) ((rand.NextDouble () * 4) - 2.0);

        Vector3 newPosition = transform.position + new Vector3 (randX, 0, randZ);
        return newPosition;
    }

    private void MoveOrb (Vector3 targetPosition) {
        OrbCollectorScript.foundObject = false;
        if (transform.position == targetPosition) {
            orbMoving = false;
            return;
        }
        float movementSpeed = 1f;
        float step = movementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards (transform.position, targetPosition, step);
    }
}