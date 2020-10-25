using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchingHandScript : MonoBehaviour {

    public Text resultText;
    public Camera arCamera;
    public GameObject sphere;

    private int handTouches = 0;

    void Update () {
        HandleTouch ();
    }

    void Start () {
        resultText.text = "Hand touches: " + handTouches;
        sphere.SetActive (false);
    }

    private void HandleTouch () {
        if (Input.touchCount == 0) {
            return;
        }
        Touch touch = Input.GetTouch (0);

        if (EventSystem.current.currentSelectedGameObject != null || touch.phase != TouchPhase.Began) {
            return;
        }

        TouchOccurred (touch);
    }

    private void TouchOccurred (Touch touch) {
        Ray ray = arCamera.ScreenPointToRay (touch.position);
        RaycastHit hitObject;
        if (Physics.Raycast (ray, out hitObject)) {
            GameObject obj = hitObject.transform.gameObject;
            handTouches += 1;
            resultText.text = "Hand touches: " + handTouches;
            sphere.SetActive (!sphere.activeSelf);
        }
    }
}