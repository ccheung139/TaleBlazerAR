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

[RequireComponent (typeof (ARRaycastManager))]
public class AttacherScript : MonoBehaviour {
    public Camera arCamera;
    public Button placeCubeButton;
    public Button placeSphereButton;
    public Button placeCylinderButton;
    public Button inventoryButton;
    public GameObject cubePrefab;
    public GameObject spherePrefab;
    public GameObject cylinderPrefab;
    public Text resultText;

    private ARRaycastManager arRaycastManager;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit> ();
    private List<List<GameObject>> shapeSets = new List<List<GameObject>> ();

    private bool placingCube = false;
    private bool placingSphere = false;
    private bool placingCylinder = false;
    private bool inventoryCheck = false;
    private Vector3 normalOfHit;

    private float textTimer = 0.0f;
    private float textTimerTotal = 3.0f;

    void Start () {
        placeCubeButton.onClick.AddListener (PressedPlaceCube);
        placeSphereButton.onClick.AddListener (PressedPlaceSphere);
        placeCylinderButton.onClick.AddListener (PressedPlaceCylinder);
        inventoryButton.onClick.AddListener (TurnOnInventory);
        arRaycastManager = GetComponent<ARRaycastManager> ();
    }

    void Update () {
        HandleTouch ();
        HandleTextTimer ();
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
            if (inventoryCheck) {
                FindItemsInSet (obj);
                inventoryCheck = false;
            } else {
                PlaceSecondaryObject (touch, obj, hitObject);
            }
        } else {
            PlaceInitialObject (touch);
        }
    }

    private void PlaceSecondaryObject (Touch touch, GameObject obj, RaycastHit hit) {
        if (arRaycastManager.Raycast (touch.position, hits, TrackableType.FeaturePoint)) {
            Pose hitPose = hits[0].pose;
            Vector3 hitPosition = hitPose.position;

            if (placingCylinder) {
                if (obj.name == "Cylinder") {
                    return;
                }
                GameObject newObject = Instantiate (cylinderPrefab, hit.transform.position, Quaternion.identity);
                newObject.transform.rotation = Quaternion.FromToRotation (Vector3.up, hit.normal);
                AddToFormerSet (obj, newObject.transform.Find ("Cylinder").gameObject);

            } else if (placingCube) {
                InstantiateBlock (obj, cubePrefab, hit);
            } else if (placingSphere) {
                InstantiateBlock (obj, spherePrefab, hit);
            }

            TurnOffPlacements ();
        }
    }

    private void InstantiateBlock (GameObject obj, GameObject prefab, RaycastHit hit) {
        if (obj.name != "Cylinder") {
            return;
        }
        Transform topOfCylinder = obj.transform.Find ("TopOfCylinder");
        GameObject newObject = Instantiate (prefab, topOfCylinder.position, Quaternion.identity);
        newObject.transform.rotation = obj.transform.rotation;

        if (placingCube) {
            newObject.GetComponent<BoxCollider> ().isTrigger = true;
        } else if (placingSphere) {
            newObject.GetComponent<SphereCollider> ().isTrigger = true;
        }

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

    private void FindItemsInSet (GameObject obj) {
        foreach (List<GameObject> potentialList in shapeSets) {
            if (potentialList.Contains (obj)) {
                resultText.text = ParseItemSet (potentialList);
                return;
            }
        }
    }

    private string ParseItemSet (List<GameObject> items) {
        int cubes = 0;
        int spheres = 0;
        int cylinders = 0;
        foreach (GameObject item in items) {
            if (item.name.Contains ("Cube")) {
                cubes += 1;
            } else if (item.name.Contains ("Sphere")) {
                spheres += 1;
            } else if (item.name.Contains ("Cylinder")) {
                cylinders += 1;
            }
        }
        return cubes + " cube(s), " + spheres + " sphere(s), and " + cylinders + " cylinder(s).";
    }

    private void PlaceInitialObject (Touch touch) {
        if (arRaycastManager.Raycast (touch.position, hits, TrackableType.FeaturePoint)) {
            Pose hitPose = hits[0].pose;
            Vector3 hitPosition = hitPose.position;
            // place initial object
            GameObject newObject = new GameObject ("Cool GameObject made from Code");
            if (placingCube) {
                newObject = Instantiate (cubePrefab, hitPosition, Quaternion.identity);
                placingCube = false;
            } else if (placingSphere) {
                newObject = Instantiate (spherePrefab, hitPosition, Quaternion.identity);
                placingSphere = false;
            } else {
                return;
            }
            List<GameObject> newShapeSet = new List<GameObject> ();
            newShapeSet.Add (newObject);
            shapeSets.Add (newShapeSet);
        }
    }

    private void PressedPlaceCube () {
        TurnOffPlacements ();
        placingCube = true;
    }

    private void PressedPlaceSphere () {
        TurnOffPlacements ();
        placingSphere = true;
    }

    private void PressedPlaceCylinder () {
        TurnOffPlacements ();
        placingCylinder = true;
    }

    private void TurnOffPlacements () {
        placingCube = false;
        placingSphere = false;
        placingCylinder = false;
        inventoryCheck = false;
    }

    private void TurnOnInventory () {
        inventoryCheck = true;
    }

    private void HandleTextTimer () {
        if (resultText.text == "") {
            return;
        }
        if (textTimer >= textTimerTotal) {
            resultText.text = "";
            textTimer = 0;
        } else {
            textTimer += Time.deltaTime * 1.0f;
        }
    }
}