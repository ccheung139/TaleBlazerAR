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
    public Button placeButton;
    public Button wholeButton;
    
    public GameObject cubePrefab;
    public GameObject spherePrefab;
    public GameObject cylinderPrefab;
    public GameObject cubePlacer;
    public GameObject spherePlacer;
    public GameObject cylinderPlacer;

    public SelectObjectsScript selectObjectsScript;
    public CancelScript cancelScript;
    public SetGovernScript setGovernScript;
    public AttachScript attachScript;

    public Material blueMaterial;
    public Material grayMaterial;
    public Material yellowMaterial;
    
    public GameObject preliminaryObject;
    private GameObject objectPlacingOn;
    public List<GameObject> allSelects = new List<GameObject> ();
    private ARRaycastManager arRaycastManager;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit> ();

    void Start () {
        placeCubeButton.onClick.AddListener (PressedPlaceCube);
        placeSphereButton.onClick.AddListener (PressedPlaceSphere);
        placeCylinderButton.onClick.AddListener (PressedPlaceCylinder);

        placeButton.onClick.AddListener (PlaceObject);
        wholeButton.onClick.AddListener (WholePressed);
        placeButton.gameObject.SetActive (false);
        arRaycastManager = GetComponent<ARRaycastManager> ();
    }

    void Update () {
        HandleTouch ();
    }

    private void HandleTouch () {
#if UNITY_EDITOR

        KeyBoardMovement ();
        if (Input.GetMouseButtonDown (0)) {
            if (EventSystem.current.currentSelectedGameObject != null) {
                return;
            }
            TouchOccurred (Input.mousePosition);
        }

#else
        if (Input.touchCount == 0) {
            return;
        }
        Touch touch = Input.GetTouch (0);

        if (EventSystem.current.currentSelectedGameObject != null || touch.phase != TouchPhase.Began) {
            return;
        }
        TouchOccurred (touch.position);
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

    private void TouchOccurred (Vector2 touchPosition) {
        Ray ray = arCamera.ScreenPointToRay (touchPosition);
        RaycastHit hitObject;

        if (cubePlacer.activeSelf || spherePlacer.activeSelf || cylinderPlacer.activeSelf || attachScript.attachOn) {
            if (Physics.Raycast (ray, out hitObject)) {
                GameObject obj = hitObject.transform.gameObject;
                PlaceSecondaryObject (touchPosition, obj, hitObject);
            }
        } else {
            if (Physics.Raycast (ray, out hitObject)) {
                GameObject obj = hitObject.transform.gameObject;
                selectObjectsScript.SelectShape (obj);
            } else {
                selectObjectsScript.DeselectShape ();
            }
        }
    }

    private void PressedPlaceCube () {
        cancelScript.DisableAllPlacers ();
        cubePlacer.SetActive (true);
        placeButton.gameObject.SetActive (true);
    }

    private void PressedPlaceSphere () {
        cancelScript.DisableAllPlacers ();
        spherePlacer.SetActive (true);
        placeButton.gameObject.SetActive (true);
    }

    private void PressedPlaceCylinder () {
        cancelScript.DisableAllPlacers ();
        cylinderPlacer.SetActive (true);
        placeButton.gameObject.SetActive (true);
    }

    private void WholePressed () {
        if (selectObjectsScript.selectedShape) {
            TurnAssociatedShapesYellow (selectObjectsScript.selectedShape);
            foreach (GameObject obj in allSelects) {
                if (selectObjectsScript.selectedShape.name == "Cylinder") {
                    obj.transform.parent = selectObjectsScript.selectedShape.transform.parent;
                } else {
                    obj.transform.parent = selectObjectsScript.selectedShape.transform;
                }
            }
        }
    }

    private void TurnAssociatedShapesYellow (GameObject obj) {
        foreach (List<GameObject> potentialList in setGovernScript.shapeSets) {
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

    private void PlaceObject () {
        List<GameObject> newShapeSet = new List<GameObject> ();
        if (cubePlacer.activeSelf) {
            GameObject newObject = Instantiate (cubePrefab, cubePlacer.transform.position, cubePlacer.transform.rotation);
            newShapeSet.Add (newObject);
            setGovernScript.shapeSets.Add (newShapeSet);
        } else if (spherePlacer.activeSelf) {
            GameObject newObject = Instantiate (spherePrefab, spherePlacer.transform.position, spherePlacer.transform.rotation);
            newShapeSet.Add (newObject);
            setGovernScript.shapeSets.Add (newShapeSet);
        } else if (preliminaryObject != null) {
            bool validPlacement = CheckPreliminaryPlacement ();
            if (!validPlacement) {
                cancelScript.DisableAllPlacers (false);
                return;
            }
            preliminaryObject.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
            preliminaryObject = null;
            if (allSelects.Count != 0) {
                cancelScript.DeselectAssociatedShapes ();
            } else {
                attachScript.attachOn = false;
            }
        }
        cancelScript.DisableAllPlacers (true);
    }

    private bool CheckPreliminaryPlacement () {
        CylinderColliderScript ccs;
        ColliderScript cs;
        if (objectPlacingOn.name == "Cylinder") {
            ccs = objectPlacingOn.GetComponent<CylinderColliderScript> ();
            cs = preliminaryObject.GetComponent<ColliderScript> ();
            return (cs.collidingObjects.Contains (objectPlacingOn) && ccs.collidingObjects.Contains (preliminaryObject));
        } else {
            ccs = preliminaryObject.GetComponent<CylinderColliderScript> ();
            cs = objectPlacingOn.GetComponent<ColliderScript> ();
            return (ccs.collidingObjects.Contains (objectPlacingOn) && cs.collidingObjects.Contains (preliminaryObject));
        }
    }

    private void PlaceSecondaryObject (Vector2 touchPosition, GameObject obj, RaycastHit hit) {

#if UNITY_EDITOR
        Vector3 hitPosition = hit.point;
#else
        if (arRaycastManager.Raycast (touchPosition, hits, TrackableType.FeaturePoint)) {
            Pose hitPose = hits[0].pose;
            Vector3 hitPosition = hitPose.position;
        }
#endif
        objectPlacingOn = obj;
        if (attachScript.attachOn) {
            attachScript.HandleAttach (obj, hit);
        } else {
            if (cylinderPlacer.activeSelf) {
                if (obj.name == "Cylinder") {
                    return;
                }
                GameObject newObject = Instantiate (cylinderPrefab, hit.transform.position, Quaternion.identity);
                newObject.transform.rotation = Quaternion.FromToRotation (Vector3.up, hit.normal);

                //possibly change this stuff with touch.position
                float maxScaleValue = FindMaxObjectScaleValue (obj);
                preliminaryObject = newObject.transform.Find ("Cylinder").gameObject;
                float xVal = preliminaryObject.transform.localScale.x;
                float zVal = preliminaryObject.transform.localScale.z;
                preliminaryObject.transform.localScale = new Vector3 (xVal, maxScaleValue, zVal);
                setGovernScript.AddToFormerSet (obj, preliminaryObject);
            } else if (cubePlacer.activeSelf) {
                InstantiateBlock (obj, cubePrefab, hit);
            } else if (spherePlacer.activeSelf) {
                InstantiateBlock (obj, spherePrefab, hit);
            }

            if (preliminaryObject != null) {
                preliminaryObject.GetComponent<Renderer> ().sharedMaterial = yellowMaterial;
                cancelScript.DisableAllPlacers (true);
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

    
    private void InstantiateBlock (GameObject obj, GameObject prefab, RaycastHit hit) {
        if (obj.name != "Cylinder") {
            return;
        }
        CylinderColliderScript cs = obj.GetComponent<CylinderColliderScript> ();

        GameObject newObject;
        if (cs.topNeighbor == null) {
            Transform topOfCylinder = obj.transform.Find ("TopOfCylinder");
            newObject = Instantiate (prefab, topOfCylinder.position, Quaternion.identity);
        } else if (cs.bottomNeighbor == null) {
            Transform bottomOfCylinder = obj.transform.Find ("BottomOfCylinder");
            newObject = Instantiate (prefab, bottomOfCylinder.position, Quaternion.identity);
        } else {
            Debug.Log ("all spaces taken up!");
            return;
        }

        newObject.transform.rotation = obj.transform.rotation;
        preliminaryObject = newObject;
        setGovernScript.AddToFormerSet (obj, newObject);
    }

}