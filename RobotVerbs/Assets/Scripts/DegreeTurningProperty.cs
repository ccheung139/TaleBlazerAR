using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class DegreeTurningProperty : MonoBehaviour {
    public Camera arCamera;
    public bool isImposter;
    public GameObject selectedSphere;

    private System.Random rand = new System.Random ();
    private bool isTurning = false;
    private bool turnedAlready = false;
    private Quaternion targetRotation;
    private List<Quaternion> normalAngles = new List<Quaternion> () {
        Quaternion.Euler (0, 0, 0),
        Quaternion.Euler (0, 90, 0),
        Quaternion.Euler (0, 180, 0),
        Quaternion.Euler (0, 270, 0),
    };
    private List<Quaternion> imposterAngles = new List<Quaternion> () {
        Quaternion.Euler (0, 45, 0),
        Quaternion.Euler (0, 135, 0),
        Quaternion.Euler (0, 225, 0),
        Quaternion.Euler (0, 315, 0),
    };

    void Update () {
        // float distance = Vector3.Distance (arCamera.transform.position, transform.position);
        // if (isTurning) {
        //     RotateRobot ();
        // } else {
        //     CheckDistance (distance);
        // }
    }

    private void CheckDistance (float distance) {
        if (distance < .3f && !turnedAlready) {
            isTurning = true;
            turnedAlready = true;
            if (isImposter) {
                ChooseTargetRotationImposter ();
            } else {
                ChooseTargetRotation ();
            }
        } else if (distance > 1f && turnedAlready) {
            turnedAlready = false;
        }
    }

    private void RotateRobot () {
        if (transform.rotation == targetRotation) {
            isTurning = false;
            return;
        }

        float movementSpeed = 50f;
        var step = movementSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRotation, step);
    }

    private void ChooseTargetRotation () {
        int index = rand.Next (normalAngles.Count);
        Quaternion newRotation = normalAngles[index];
        while (newRotation == targetRotation) {
            index = rand.Next (normalAngles.Count);
            newRotation = normalAngles[index];
        }
        targetRotation = newRotation;
    }

    private void ChooseTargetRotationImposter () {
        int index = rand.Next (imposterAngles.Count);
        Quaternion newRotation = imposterAngles[index];
        while (newRotation == targetRotation) {
            index = rand.Next (normalAngles.Count);
            newRotation = imposterAngles[index];
        }
        targetRotation = newRotation;
    }
}