using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour {
    public Camera arCamera;

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
        float speed = 1.0f;
        float rotateSpeed = 30.0f;
        Vector3 pos = arCamera.transform.position;
        if (Input.GetKey ("w")) {
            pos.z += speed * Time.deltaTime;
        }
        if (Input.GetKey ("s")) {
            pos.z -= speed * Time.deltaTime;
        }
        if (Input.GetKey ("d")) {
            pos.x += speed * Time.deltaTime;
        }
        if (Input.GetKey ("a")) {
            pos.x -= speed * Time.deltaTime;
        }
        if (Input.GetKey ("r")) {
            pos.y += speed * Time.deltaTime;
        }
        if (Input.GetKey ("f")) {
            pos.y -= speed * Time.deltaTime;
        }
        arCamera.transform.position = pos;

        if (Input.GetKey ("e")) {
            arCamera.transform.Rotate (Vector3.right, rotateSpeed * Time.deltaTime);
        }
        if (Input.GetKey ("q")) {
            arCamera.transform.Rotate (-Vector3.right, rotateSpeed * Time.deltaTime);
        }
    }
}