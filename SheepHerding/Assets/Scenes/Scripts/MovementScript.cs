using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour {
    public Camera arCamera;

    private bool rotateOn = false;

    void Update () {
        HandleTouch ();
    }

    private void HandleTouch () {
#if UNITY_EDITOR

        KeyBoardMovement ();

#else
#endif

    }

    private void KeyBoardMovement () {
        if (Input.GetKey ("r")) {
            rotateOn = !rotateOn;
            return;
        }

        // if (rotateOn) {
        //     Rotate ();
        // } else {
        //     Movement ();
        // }
        Rotate ();
        Movement ();
    }

    private void Movement () {
        float speed = 3.0f;

        Vector3 pos = arCamera.transform.position;
        if (Input.GetKey ("w")) {
            // pos.z += speed * Time.deltaTime;
            pos += arCamera.transform.forward * 0.05f;
        }
        if (Input.GetKey ("s")) {
            pos += arCamera.transform.forward * -0.05f;
        }
        if (Input.GetKey ("d")) {
            pos += arCamera.transform.right * 0.05f;
        }
        if (Input.GetKey ("a")) {
            pos += arCamera.transform.right * -0.05f;
        }
        if (Input.GetKey ("e")) {
            pos += arCamera.transform.up * 0.05f;
        }
        if (Input.GetKey ("q")) {
            pos += arCamera.transform.up * -0.05f;
        }
        arCamera.transform.position = pos;
    }

    private void Rotate () {
        float rotateSpeed = 60.0f;

        if (Input.GetKey ("up")) {
            arCamera.transform.Rotate (-Vector3.right, rotateSpeed * Time.deltaTime);
        }
        if (Input.GetKey ("down")) {
            arCamera.transform.Rotate (Vector3.right, rotateSpeed * Time.deltaTime);
        }
        if (Input.GetKey ("right")) {
            arCamera.transform.Rotate (Vector3.up, rotateSpeed * Time.deltaTime);
        }
        if (Input.GetKey ("left")) {
            arCamera.transform.Rotate (-Vector3.up, rotateSpeed * Time.deltaTime);
        }
    }
}