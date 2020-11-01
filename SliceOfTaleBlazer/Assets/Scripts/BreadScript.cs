using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreadScript : MonoBehaviour {
    public Camera arCamera;
    public InventoryScript invScript;
    public bool owned;
    public Text searchingText;
    public GameObject mouse;

    private float timer = 0;
    private float timerTotal = 10.0f;
    private bool timeComplete = false;
    private bool scurry = false;
    private Vector3 scurryPosition;

    private float movementSpeed = 0.02f;
    private float breadSpeed = 0.1f;
    private float scurrySpeed = 1f;

    // Update is called once per frame
    void Update () {
        float distance = Vector3.Distance (transform.position, arCamera.transform.position);
        if (distance <= 0.5f && !owned) {
            searchingText.text = "You found toast! Construct a baguette with 3 toasts.";
            invScript.AddToast ();
            Destroy (gameObject);
            Destroy (mouse);
        }

        if (invScript.magnet.activeSelf && distance <= 2f && GetComponent<Renderer> ().IsVisibleFrom (arCamera)) {
            MoveBread ();
        }
        HandleMouse ();
    }

    private void HandleMouse () {
        if (scurry) {
            RotateMouse (scurryPosition);
            ScurryMouse ();
            return;
        }
        if (timer >= timerTotal) {
            timeComplete = true;
        } else {
            timer += Time.deltaTime * 1.0f;
        }
        MouseCreeping ();
    }

    private void MouseCreeping () {
        if (timeComplete) {
            if (!mouse.activeSelf) {
                mouse.transform.position = ChooseNewPosition ();
                if (Vector3.Distance (arCamera.transform.position, transform.position) > 1.5f) {
                    mouse.SetActive (true);
                }
            }
            if (Vector3.Distance (arCamera.transform.position, mouse.transform.position) <= 1f &&
                mouse.GetComponent<Renderer> ().IsVisibleFrom (arCamera)) {
                scurry = true;
                scurryPosition = ChooseNewPosition ();
                timer = 0;
                timeComplete = false;
            } else {
                RotateMouse (transform.position);
                MoveMouse ();
            }
        }
    }

    private Vector3 ChooseNewPosition () {
        Vector3 toastPosition = transform.position;
        Vector3 size = new Vector3 (2f, 0, 2f);
        return toastPosition + new Vector3 (
            (Random.value - 0.5f) * size.x,
            toastPosition.y,
            (Random.value - 0.5f) * size.z
        );
    }

    private void ScurryMouse () {
        if (mouse.transform.position == scurryPosition) {
            scurry = false;
            mouse.SetActive (false);
            return;
        }
        float step = scurrySpeed * Time.deltaTime;
        mouse.transform.position = Vector3.MoveTowards (mouse.transform.position, scurryPosition, step);
    }

    private void MoveMouse () {
        if (Vector3.Distance (mouse.transform.position, transform.position) <= 0.1f) {
            Destroy (mouse);
            Destroy (gameObject);
        }
        float step = movementSpeed * Time.deltaTime;
        mouse.transform.position = Vector3.MoveTowards (mouse.transform.position, transform.position, step);
    }

    private void RotateMouse (Vector3 newPosition) {
        var direction = mouse.transform.position - newPosition;
        direction.y = 0;
        var newRotation = Quaternion.LookRotation (direction) * Quaternion.Euler (0, 0, 0);
        mouse.transform.rotation = Quaternion.Slerp (mouse.transform.rotation, newRotation, Time.deltaTime * 6f);
    }

    private void MoveBread () {
        float step = breadSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards (transform.position, arCamera.transform.position, step);
    }
}