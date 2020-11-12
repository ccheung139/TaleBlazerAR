using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent (typeof (ARRaycastManager))]
public class PlacementScript : MonoBehaviour {
    public Camera arCamera;
    public Button placeCubeButton;
    public Button placeSphereButton;
    public Button placeCylinderButton;
    public Button cancelButton;
    public Button placeButton;
    public Button wholeButton;
    public Button attachButton;
    public GameObject cubePrefab;
    public GameObject spherePrefab;
    public GameObject cylinderPrefab;
    public GameObject cubePlacer;
    public GameObject spherePlacer;
    public GameObject cylinderPlacer;
    public Text resultText;
    public Material blueMaterial;
    public Material grayMaterial;
    public Material yellowMaterial;

    public Slider xSlider;
    public Slider ySlider;
    public Slider zSlider;
    public Slider xSliderScale;
    public Slider ySliderScale;
    public Slider zSliderScale;

    private float previousXValue;
    private float previousYValue;
    private float previousZValue;
    private float previousXValueScale;
    private float previousYValueScale;
    private float previousZValueScale;
    private bool attachOn = false;

    private GameObject selectedShape;
    private GameObject preliminaryObject;
    private List<GameObject> allSelects = new List<GameObject> ();
    private ARRaycastManager arRaycastManager;
    private List<List<GameObject>> shapeSets = new List<List<GameObject>> ();
    private static List<ARRaycastHit> hits = new List<ARRaycastHit> ();
    private Vector3 hitAngle;

    private Vector3 previousAttachPosition;
    private Quaternion previousAttachRotation;

    void Start () {
        placeCubeButton.onClick.AddListener (PressedPlaceCube);
        placeSphereButton.onClick.AddListener (PressedPlaceSphere);
        placeCylinderButton.onClick.AddListener (PressedPlaceCylinder);
        cancelButton.onClick.AddListener (CancelActions);
        placeButton.onClick.AddListener (PlaceObject);
        wholeButton.onClick.AddListener (WholePressed);
        placeButton.gameObject.SetActive (false);
        arRaycastManager = GetComponent<ARRaycastManager> ();

        xSlider.onValueChanged.AddListener (OnXSliderChanged);
        ySlider.onValueChanged.AddListener (OnYSliderChanged);
        zSlider.onValueChanged.AddListener (OnZSliderChanged);
        xSliderScale.onValueChanged.AddListener (OnXSliderChangedScale);
        ySliderScale.onValueChanged.AddListener (OnYSliderChangedScale);
        zSliderScale.onValueChanged.AddListener (OnZSliderChangedScale);
        attachButton.onClick.AddListener (AttachObject);
    }

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

    private void SliderChanger (float previousValue, Vector3 newAngle, float value) {
        if (preliminaryObject == null || hitAngle == null) {
            return;
        }
        float delta = value - previousValue;
        if (preliminaryObject.name == "Cylinder") {
            GameObject parent = preliminaryObject.transform.parent.gameObject;
            if (allSelects.Count == 0) {
                parent.transform.RotateAround (parent.transform.position, newAngle, delta * 360);
            } else {
                Transform topOfCylinder = preliminaryObject.transform.Find ("TopOfCylinder");
                parent.transform.RotateAround (topOfCylinder.position, newAngle, delta * 360);
            }
        } else {
            preliminaryObject.transform.RotateAround (preliminaryObject.transform.position, newAngle, delta * 360);
        }
    }

    void OnXSliderChanged (float value) {
        Vector3 forwardVector = preliminaryObject.transform.forward;
        SliderChanger (previousXValue, forwardVector, value);
        previousXValue = value;
    }

    void OnYSliderChanged (float value) {
        Vector3 upVector = preliminaryObject.transform.up;
        SliderChanger (previousYValue, upVector, value);
        previousYValue = value;
    }

    void OnZSliderChanged (float value) {
        Vector3 rightVector = preliminaryObject.transform.right;
        SliderChanger (previousZValue, rightVector, value);
        previousZValue = value;
    }

    private void SizeSliderChanger (float delta, Vector3 scaleChange) {
        GameObject objectToChange;
        if (preliminaryObject != null) {
            objectToChange = preliminaryObject;
        } else if (selectedShape != null) {
            objectToChange = selectedShape;
        } else {
            return;
        }
        if (objectToChange.name == "Cylinder") {
            GameObject parent = objectToChange.transform.parent.gameObject;
            parent.transform.localScale += scaleChange;
        } else {
            objectToChange.transform.localScale += scaleChange;
        }
    }

    void OnXSliderChangedScale (float value) {
        float delta = value - previousXValueScale;
        Vector3 scaleChange = new Vector3 (delta, 0, 0);
        SizeSliderChanger (delta, scaleChange);
        previousXValueScale = value;
    }
    void OnYSliderChangedScale (float value) {
        float delta = value - previousYValueScale;
        Vector3 scaleChange = new Vector3 (0, delta, 0);
        SizeSliderChanger (delta, scaleChange);
        previousYValueScale = value;
    }
    void OnZSliderChangedScale (float value) {
        float delta = value - previousZValueScale;
        Vector3 scaleChange = new Vector3 (0, 0, delta);
        SizeSliderChanger (delta, scaleChange);
        previousZValueScale = value;
    }

    private void TouchOccurred (Touch touch) {
        Ray ray = arCamera.ScreenPointToRay (touch.position);
        RaycastHit hitObject;

        if (cubePlacer.activeSelf || spherePlacer.activeSelf || cylinderPlacer.activeSelf || attachOn) {
            if (Physics.Raycast (ray, out hitObject)) {
                GameObject obj = hitObject.transform.gameObject;
                PlaceSecondaryObject (touch, obj, hitObject);
            }
        } else {
            if (Physics.Raycast (ray, out hitObject)) {
                GameObject obj = hitObject.transform.gameObject;
                SelectShape (obj);
            } else {
                DeselectShape ();
            }
        }
    }

    private void PressedPlaceCube () {
        DisableAllPlacers ();
        cubePlacer.SetActive (true);
        placeButton.gameObject.SetActive (true);
    }

    private void PressedPlaceSphere () {
        DisableAllPlacers ();
        spherePlacer.SetActive (true);
        placeButton.gameObject.SetActive (true);
    }

    private void PressedPlaceCylinder () {
        DisableAllPlacers ();
        cylinderPlacer.SetActive (true);
        placeButton.gameObject.SetActive (true);
    }

    private void WholePressed () {
        if (selectedShape) {
            TurnAssociatedShapesYellow (selectedShape);
            foreach (GameObject obj in allSelects) {
                if (selectedShape.name == "Cylinder") {
                    obj.transform.parent = selectedShape.transform.parent;
                } else {
                    obj.transform.parent = selectedShape.transform;
                }

            }
        }
    }

    private void TurnAssociatedShapesYellow (GameObject obj) {
        foreach (List<GameObject> potentialList in shapeSets) {
            if (potentialList.Contains (obj)) {
                foreach (GameObject associatedObj in potentialList) {
                    if (associatedObj != obj) {
                        associatedObj.GetComponent<Renderer> ().sharedMaterial = yellowMaterial;
                        if (associatedObj.name == "Cylinder") {
                            allSelects.Add (associatedObj.transform.parent.gameObject);
                        } else {
                            allSelects.Add (associatedObj);
                        }
                    }
                }
                return;
            }
        }
    }

    private void AttachObject () {
        attachOn = true;
        previousAttachPosition = selectedShape.transform.position;
        previousAttachRotation = selectedShape.transform.rotation;
    }

    private void PlaceObject () {
        List<GameObject> newShapeSet = new List<GameObject> ();
        if (cubePlacer.activeSelf) {
            GameObject newObject = Instantiate (cubePrefab, cubePlacer.transform.position, cubePlacer.transform.rotation);
            newShapeSet.Add (newObject);
            shapeSets.Add (newShapeSet);
        } else if (spherePlacer.activeSelf) {
            GameObject newObject = Instantiate (spherePrefab, spherePlacer.transform.position, spherePlacer.transform.rotation);
            newShapeSet.Add (newObject);
            shapeSets.Add (newShapeSet);
        } else if (preliminaryObject != null) {

            preliminaryObject.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
            preliminaryObject = null;
            if (allSelects.Count != 0) {
                DeselectAssociatedShapes ();
            } else {
                attachOn = false;
            }
        }
        DisableAllPlacers (true);
    }

    private void PlaceSecondaryObject (Touch touch, GameObject obj, RaycastHit hit) {
        if (arRaycastManager.Raycast (touch.position, hits, TrackableType.FeaturePoint)) {
            Pose hitPose = hits[0].pose;
            Vector3 hitPosition = hitPose.position;

            if (attachOn) {
                HandleAttach (obj, hit);
            } else {
                if (cylinderPlacer.activeSelf) {
                    if (obj.name == "Cylinder") {
                        return;
                    }
                    GameObject newObject = Instantiate (cylinderPrefab, hit.transform.position, Quaternion.identity);
                    hitAngle = hit.normal;
                    newObject.transform.rotation = Quaternion.FromToRotation (Vector3.up, hit.normal);

                    //possibly change this stuff with touch.position
                    float maxScaleValue = FindMaxObjectScaleValue (obj);
                    preliminaryObject = newObject.transform.Find ("Cylinder").gameObject;
                    float xVal = preliminaryObject.transform.localScale.x;
                    float zVal = preliminaryObject.transform.localScale.z;
                    preliminaryObject.transform.localScale = new Vector3 (xVal, maxScaleValue, zVal);
                    AddToFormerSet (obj, preliminaryObject);
                } else if (cubePlacer.activeSelf) {
                    InstantiateBlock (obj, cubePrefab, hit);
                } else if (spherePlacer.activeSelf) {
                    InstantiateBlock (obj, spherePrefab, hit);
                }
                preliminaryObject.GetComponent<Renderer> ().sharedMaterial = yellowMaterial;
                DisableAllPlacers (true);
                placeButton.gameObject.SetActive (true);
            }
        }
    }

    private float FindMaxObjectScaleValue (GameObject obj) {
        float maxVal = 0;
        Vector3 scale = obj.transform.localScale;
        maxVal = Math.Max (maxVal, scale.x);
        maxVal = Math.Max (maxVal, scale.y);
        maxVal = Math.Max (maxVal, scale.z);
        return maxVal;
    }

    private void HandleAttach (GameObject obj, RaycastHit hit) {
        if (selectedShape.name.Contains ("Cube") || selectedShape.name.Contains ("Sphere")) {
            Transform topOfCylinder = obj.transform.Find ("TopOfCylinder");
            selectedShape.transform.position = topOfCylinder.position;
            selectedShape.transform.rotation = obj.transform.rotation;

        } else {
            GameObject parentCylinder = selectedShape.transform.parent.gameObject;
            parentCylinder.transform.position = hit.transform.position;
            parentCylinder.transform.rotation = Quaternion.FromToRotation (Vector3.up, hit.normal);

            Vector3 upVector = parentCylinder.transform.forward;
            parentCylinder.transform.RotateAround (selectedShape.transform.position, upVector, 180);
        }
        preliminaryObject = selectedShape;
        DeselectShape ();
        preliminaryObject.GetComponent<Renderer> ().sharedMaterial = yellowMaterial;
        DisableAllPlacers (true);
        placeButton.gameObject.SetActive (true);

        CombineSets (obj, preliminaryObject);
    }

    private void InstantiateBlock (GameObject obj, GameObject prefab, RaycastHit hit) {
        if (obj.name != "Cylinder") {
            return;
        }
        Transform topOfCylinder = obj.transform.Find ("TopOfCylinder");
        GameObject newObject = Instantiate (prefab, topOfCylinder.position, Quaternion.identity);
        newObject.transform.rotation = obj.transform.rotation;
        preliminaryObject = newObject;
        AddToFormerSet (obj, newObject);
    }

    private void AddToFormerSet (GameObject previousObject, GameObject newObject) {
        foreach (List<GameObject> potentialList in shapeSets) {
            if (potentialList.Contains (previousObject)) {
                potentialList.Add (newObject);
                return;
            }
        }
    }

    private void CombineSets (GameObject firstObject, GameObject secondObject) {
        int firstIndex = -1;
        int secondIndex = -1;
        for (int i = 0; i < shapeSets.Count; i++) {
            List<GameObject> potentialList = shapeSets[i];
            if (potentialList.Contains (firstObject)) {
                firstIndex = i;
            } else if (potentialList.Contains (secondObject)) {
                secondIndex = i;
            }
        }
        List<GameObject> firstList = shapeSets[firstIndex];
        List<GameObject> secondList = shapeSets[secondIndex];

        int maxIndex;
        int minIndex;
        if (firstIndex > secondIndex) {
            maxIndex = firstIndex;
            minIndex = secondIndex;
        } else {
            minIndex = firstIndex;
            maxIndex = secondIndex;
        }
        shapeSets.RemoveAt (maxIndex);
        shapeSets.RemoveAt (minIndex);

        firstList.AddRange (secondList);
        shapeSets.Add (firstList);
    }

    private void CancelActions () {
        DisableAllPlacers (false);
    }

    private void DisableAllPlacers (bool preliminary = false) {
        DeselectShape ();
        spherePlacer.SetActive (false);
        cubePlacer.SetActive (false);
        cylinderPlacer.SetActive (false);
        placeButton.gameObject.SetActive (false);
        if (!preliminary && preliminaryObject != null && !attachOn) {
            Destroy (preliminaryObject);
            preliminaryObject = null;
        }
        if (attachOn) {
            if (!preliminary) {
                if (preliminaryObject.name == "Cylinder") {
                    preliminaryObject.transform.parent.transform.position = previousAttachPosition;
                    preliminaryObject.transform.parent.transform.rotation = previousAttachRotation;
                } else {
                    preliminaryObject.transform.position = previousAttachPosition;
                    preliminaryObject.transform.rotation = previousAttachRotation;
                }

                DeselectAssociatedShapes ();
            }
        }
    }

    private void DeselectAssociatedShapes () {
        foreach (GameObject obj in allSelects) {
            if (obj.name.Contains ("CylinderParent")) {
                obj.transform.Find ("Cylinder").GetComponent<Renderer> ().sharedMaterial = grayMaterial;
            } else {
                obj.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
            }
        }
        allSelects.Clear ();
        attachOn = false;

        if (preliminaryObject != null) {
            preliminaryObject.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
            preliminaryObject = null;
        }
    }

    private void SelectShape (GameObject obj) {
        if (cubePlacer.activeSelf || spherePlacer.activeSelf || cylinderPlacer.activeSelf || preliminaryObject != null) {
            return;
        }
        DeselectShape ();

        Renderer renderer = obj.GetComponent<Renderer> ();
        if (obj.GetComponent<Renderer> ().sharedMaterial == grayMaterial) {
            obj.GetComponent<Renderer> ().sharedMaterial = blueMaterial;
            selectedShape = obj;
        } else {
            obj.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
            selectedShape = null;
        }
    }

    private void DeselectShape () {
        if (selectedShape != null) {
            selectedShape.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
            selectedShape = null;
        }
    }
}