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
    public GameObject parentHolder;

    public GameObject audioPlayer;
    public Material grayLineMaterial;
    public Material redLineMaterial;
    public Material blueLineMaterial;
    public Material greenLineMaterial;
    public Material grayMaterial;
    public Material blueMaterial;
    public Material yellowMaterial;
    public Material transparentBlue;
    public Material transparentYellow;

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
                    Handheld.Vibrate ();
                    audioPlayer.GetComponent<AudioSource> ().Play ();
                    HandleLineChanges (colliderTouchedObj);
                }
            } else {
                colliderTouched = false;
            }

        }

    }

    private void HandleLineChanges (GameObject colliderTouchedObj) {
        GameObject scaleSphere;
        if (colliderTouchedObj.name == "XScaleSphere") {
            scaleSphere = xScaleSphere;
            yScaleSphere.GetComponent<Renderer> ().sharedMaterial = grayLineMaterial;
            zScaleSphere.GetComponent<Renderer> ().sharedMaterial = grayLineMaterial;
        } else if (colliderTouchedObj.name == "YScaleSphere") {
            scaleSphere = yScaleSphere;
            xScaleSphere.GetComponent<Renderer> ().sharedMaterial = grayLineMaterial;
            zScaleSphere.GetComponent<Renderer> ().sharedMaterial = grayLineMaterial;
        } else {
            scaleSphere = zScaleSphere;
            xScaleSphere.GetComponent<Renderer> ().sharedMaterial = grayLineMaterial;
            yScaleSphere.GetComponent<Renderer> ().sharedMaterial = grayLineMaterial;
        }
        scaleSphere.transform.localScale = new Vector3 (0.06f, 0.06f, 0.06f);
    }

    private void ResetCapsules () {
        ResetCapsuleOption (xScaleSphere, redLineMaterial);
        ResetCapsuleOption (yScaleSphere, greenLineMaterial);
        ResetCapsuleOption (zScaleSphere, blueLineMaterial);
    }

    private void ResetCapsuleOption (GameObject capsule, Material material) {
        capsule.GetComponent<Renderer> ().sharedMaterial = material;
        capsule.transform.localScale = new Vector3 (0.03f, 0.03f, 0.03f);
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
            ResetCapsules ();
            return;
        }

        Vector3 cameraPosition = arCamera.transform.position;
        Vector3 scaleChange = new Vector3 (0, 0, 0);
        float newValue = 0;
        if (colliderTouchedObj.name == "YScaleSphere") {
            newValue = cameraPosition.y;
            float delta = newValue - previousValue;
            float value = yScaleSphere.transform.position.y - parentHolder.transform.position.y;
            if (value < 0) {
                delta = -delta;
            }
            scaleChange = new Vector3 (0, delta, 0);
        } else if (colliderTouchedObj.name == "XScaleSphere") {
            newValue = cameraPosition.x;
            float delta = newValue - previousValue;
            float value = xScaleSphere.transform.position.x - parentHolder.transform.position.x;
            if (value < 0) {
                delta = -delta;
            }
            scaleChange = new Vector3 (delta, 0, 0);
        } else {
            newValue = cameraPosition.z;
            float delta = newValue - previousValue;
            float value = zScaleSphere.transform.position.z - parentHolder.transform.position.z;
            if (value < 0) {
                delta = -delta;
            }
            scaleChange = new Vector3 (0, 0, delta);
        }

        Vector3 scaleCopy = parentHolder.transform.localScale + scaleChange * 3f;
        if (scaleCopy.x > 0 && scaleCopy.y > 0 && scaleCopy.z > 0) {
            parentHolder.transform.localScale = scaleCopy;
        }

        previousValue = newValue;
        GameObject selected = selectObjectsScript.selectedShapes[0];
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
            TransparencyHandler (true);
        } else {
            DisableCapsules ();
            TransparencyHandler (false);
        }
    }

    private void TransparencyHandler (bool isTransparent) {
        foreach (Transform objTransform in parentHolder.transform) {
            GameObject selectedObj = objTransform.gameObject;
            if (isTransparent) {
                TransparencyHelper (selectedObj);
            } else {
                OpaqueHelper (selectedObj);
            }
            foreach (Transform childTransform in selectedObj.transform) {
                GameObject child = childTransform.gameObject;
                if (child.name == "Cylinder" || child.name == "Cube(Clone)" || child.name == "Sphere(Clone)") {
                    if (isTransparent) {
                        TransparencyHelper (child);
                    } else {
                        OpaqueHelper (child);
                    }
                }
            }
        }
    }

    private void TransparencyHelper (GameObject obj) {
        Material mat = obj.GetComponent<Renderer> ().sharedMaterial;
        if (mat.name == "BlueMaterial") {
            obj.GetComponent<Renderer> ().sharedMaterial = transparentBlue;
        } else if (mat.name == "YellowMaterial") {
            obj.GetComponent<Renderer> ().sharedMaterial = transparentYellow;
        }
    }

    private void OpaqueHelper (GameObject obj) {
        Material mat = obj.GetComponent<Renderer> ().sharedMaterial;
        if (mat.name == "TransparentBlue") {
            obj.GetComponent<Renderer> ().sharedMaterial = blueMaterial;
        } else if (mat.name == "TransparentYellow") {
            obj.GetComponent<Renderer> ().sharedMaterial = yellowMaterial;
        }
    }

    private void EnableCapsules () {
        if (selectObjectsScript.selectedShapes.Count == 0) {
            return;
        }
        GameObject selected = selectObjectsScript.selectedShapes[0];
        // GameObject selected = parentHolder;
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