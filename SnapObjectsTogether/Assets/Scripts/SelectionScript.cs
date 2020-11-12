using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionScript : MonoBehaviour {
    public Camera arCamera;
    public Material blueMaterial;
    public Material grayMaterial;
    public AttacherScript attacherScript;
    private GameObject selectedShape;

    // Start is called before the first frame update
    void Start () {

    }

    // Update is called once per frame
    void Update () {
        HandleTouch ();
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
        if (attacherScript.placingSphere || attacherScript.placingCube || attacherScript.placingCylinder) {
            return;
        }
        Ray ray = arCamera.ScreenPointToRay (touch.position);
        RaycastHit hitObject;
        if (Physics.Raycast (ray, out hitObject)) {
            GameObject obj = hitObject.transform.gameObject;
            if (obj.transform.childCount == 0) {
                SelectShape (obj);
            }
        }
    }

    private void SelectShape (GameObject obj) {
        if (selectedShape != null) {
            selectedShape.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
        }

        Renderer renderer = obj.GetComponent<Renderer> ();
        if (obj.GetComponent<Renderer> ().sharedMaterial == grayMaterial) {
            obj.GetComponent<Renderer> ().sharedMaterial = blueMaterial;
            selectedShape = obj;
        } else {
            obj.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
            selectedShape = null;
        }
    }
}