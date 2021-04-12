using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour {
    public Camera arCamera;

    private float movementSpeed = .02f;

    void Update () {
        HandleTouch ();
    }

    private void HandleTouch () {
#if UNITY_EDITOR

        KeyBoardMovement ();
        // if (rotateScript.rotateOn || scaleScript.scaleOn) {
        //     return;
        // }
        // if (Input.GetMouseButtonDown (0)) {
        //     if (EventSystem.current.currentSelectedGameObject != null) {
        //         return;
        //     }
        //     TouchOccurred (Input.mousePosition);
        // }

#else
        // if (rotateScript.rotateOn || scaleScript.scaleOn) {
        //     return;
        // }
        // if (Input.touchCount == 0) {
        //     return;
        // }
        // Touch touch = Input.GetTouch (0);

        // if (EventSystem.current.currentSelectedGameObject != null || touch.phase != TouchPhase.Began) {
        //     return;
        // }
        // TouchOccurred (touch.position);
#endif

    }

    private void KeyBoardMovement () {
        Rotate ();
        Movement ();
    }

    private void Movement () {
        float speed = 3.0f;

        Vector3 pos = transform.position;
        if (Input.GetKey ("w")) {
            pos += transform.forward * movementSpeed;
        }
        if (Input.GetKey ("s")) {
            pos += transform.forward * -movementSpeed;
        }
        if (Input.GetKey ("d")) {
            pos += transform.right * movementSpeed;
        }
        if (Input.GetKey ("a")) {
            pos += transform.right * -movementSpeed;
        }
        if (Input.GetKey ("e")) {
            pos += transform.up * movementSpeed;
        }
        if (Input.GetKey ("q")) {
            pos += transform.up * -movementSpeed;
        }
        transform.position = pos;
    }

    private void Rotate () {
        float rotateSpeed = 60.0f;

        if (Input.GetKey ("up")) {
            transform.Rotate (-Vector3.right, rotateSpeed * Time.deltaTime);
        }
        if (Input.GetKey ("down")) {
            transform.Rotate (Vector3.right, rotateSpeed * Time.deltaTime);
        }
        if (Input.GetKey ("right")) {
            transform.Rotate (Vector3.up, rotateSpeed * Time.deltaTime);
        }
        if (Input.GetKey ("left")) {
            transform.Rotate (-Vector3.up, rotateSpeed * Time.deltaTime);
        }
    }
}