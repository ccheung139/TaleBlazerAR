using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScaleScript : MonoBehaviour {
    public SelectObjectsScript selectObjectsScript;
    public Button scaleButton;
    public Camera arCamera;
    public GameObject colliderObject;
    public GameObject xScaleSphere;
    public GameObject yScaleSphere;
    public GameObject zScaleSphere;

    public bool scaleOn = false;

    private float previousValue = 0;
    private bool colliderTouched = false;
    private GameObject colliderTouchedObj;

    void Start () {
        scaleButton.onClick.AddListener (PressedScale);
    }

    void Update () {
        if (scaleOn) {
            if (colliderTouched) {
                HandleScale ();
            } else {
                HandleTouch ();
            }
        }
    }

    private void HandleTouch () {
#if UNITY_EDITOR
        bool touching = Input.GetMouseButtonDown (0);
#else
        bool touching = Input.touchCount > 0;
#endif

        if (touching) {
            Ray ray = arCamera.ScreenPointToRay (Input.mousePosition);
            RaycastHit hitObject;
            if (Physics.Raycast (ray, out hitObject)) {
                GameObject obj = hitObject.transform.gameObject;
                if (obj.name.Contains ("ScaleSphere")) {
                    colliderTouchedObj = obj;
                    colliderTouched = true;
                    CalculateStartingPosition ();
                }
            } else {
                colliderTouched = false;
            }

        }

    }

    private void CalculateStartingPosition () {
        previousValue = 0;
        if (colliderTouchedObj.name == "YScaleSphere") {
            previousValue = arCamera.transform.position.y;
        } else if (colliderTouchedObj.name == "XScaleSphere") {
            previousValue = arCamera.transform.position.x;
        } else if (colliderTouchedObj.name == "ZScaleSphere") {
            previousValue = arCamera.transform.position.z;
        }
    }

    private void HandleScale () {
#if UNITY_EDITOR
        bool released = Input.GetMouseButtonUp (0);
#else
        bool released = Input.touchCount == 0;
#endif
        if (released) {
            colliderTouched = false;
            return;
        }
        GameObject selected = selectObjectsScript.selectedShapes[0];
        Vector3 cameraPosition = arCamera.transform.position;

        Vector3 scaleChange = new Vector3 (0, 0, 0);
        float newValue = 0;
        if (colliderTouchedObj.name == "YScaleSphere") {
            newValue = cameraPosition.y;
            float delta = newValue - previousValue;
            float value = yScaleSphere.transform.position.y - selected.transform.position.y;
            if (value < 0) {
                delta = -delta;
            }
            scaleChange = new Vector3 (0, delta, 0);
        } else if (colliderTouchedObj.name == "XScaleSphere") {
            newValue = cameraPosition.x;
            float delta = newValue - previousValue;
            float value = xScaleSphere.transform.position.x - selected.transform.position.x;
            if (value < 0) {
                delta = -delta;
            }
            scaleChange = new Vector3 (delta, 0, 0);
        } else {
            newValue = cameraPosition.z;
            float delta = newValue - previousValue;
            float value = zScaleSphere.transform.position.z - selected.transform.position.z;
            if (value < 0) {
                delta = -delta;
            }
            scaleChange = new Vector3 (0, 0, delta);
        }

        if (selected.name == "Cylinder") {
            GameObject parent = selected.transform.parent.gameObject;
            Vector3 scaleCopy = parent.transform.localScale + scaleChange * 3.0f;
            if (scaleCopy.x > 0 && scaleCopy.y > 0 && scaleCopy.z > 0) {
                parent.transform.localScale = scaleCopy;
            }

        } else {
            Vector3 scaleCopy = selected.transform.localScale + scaleChange;
            if (scaleCopy.x > 0 && scaleCopy.y > 0 && scaleCopy.z > 0) {
                selected.transform.localScale = scaleCopy;
            }
        }
        previousValue = newValue;
        UpdateScaleSpheres (selected);
    }

    private void UpdateScaleSpheres (GameObject obj) {
        xScaleSphere.transform.position = obj.transform.Find ("XPoint").position;
        yScaleSphere.transform.position = obj.transform.Find ("YPoint").position;
        zScaleSphere.transform.position = obj.transform.Find ("ZPoint").position;
    }

    private void PressedScale () {
        scaleOn = !scaleOn;
        if (scaleOn) {
            EnableCapsules ();
        } else {
            DisableCapsules ();
        }
    }

    private void EnableCapsules () {
        if (selectObjectsScript.selectedShapes.Count == 0) {
            return;
        }
        GameObject selected = selectObjectsScript.selectedShapes[0];
        UpdateScaleSpheres (selected);

        xScaleSphere.SetActive (true);
        yScaleSphere.SetActive (true);
        zScaleSphere.SetActive (true);
    }

    private void DisableCapsules () {
        xScaleSphere.SetActive (false);
        yScaleSphere.SetActive (false);
        zScaleSphere.SetActive (false);
    }
}