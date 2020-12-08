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
    public RotateScript rotateScript;
    public ScaleScript scaleScript;

    public Material blueMaterial;
    public Material grayMaterial;
    public Material yellowMaterial;

    public GameObject preliminaryObject;
    private GameObject objectPlacingOn;
    public List<GameObject> allSelects = new List<GameObject> ();
    private ARRaycastManager arRaycastManager;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit> ();
    private float closenessFactor = 0.04995f;

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
        if (rotateScript.rotateOn || scaleScript.scaleOn) {
            return;
        }
        if (Input.GetMouseButtonDown (0)) {
            if (EventSystem.current.currentSelectedGameObject != null) {
                return;
            }
            TouchOccurred (Input.mousePosition);
        }

#else
        if (rotateScript.rotateOn || scaleScript.scaleOn) {
            return;
        }
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
                selectObjectsScript.DeselectShapes ();
            }
        }
    }

    private void PressedPlaceCube () {
        cancelScript.DisableAllPlacers ();
        cubePlacer.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
        cubePlacer.SetActive (true);
        placeButton.gameObject.SetActive (true);
    }

    private void PressedPlaceSphere () {
        cancelScript.DisableAllPlacers ();
        spherePlacer.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
        spherePlacer.SetActive (true);
        placeButton.gameObject.SetActive (true);
    }

    private void PressedPlaceCylinder () {
        cancelScript.DisableAllPlacers ();
        cylinderPlacer.transform.Find ("CylinderPlaced").gameObject.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
        cylinderPlacer.SetActive (true);
        placeButton.gameObject.SetActive (true);
    }

    private void WholePressed () {
        if (selectObjectsScript.selectedShapes.Count == 1) {
            TurnAssociatedShapesYellow (selectObjectsScript.selectedShapes[0]);
            foreach (GameObject obj in allSelects) {
                
                if (selectObjectsScript.selectedShapes[0].name == "Cylinder") {
                    obj.transform.parent = selectObjectsScript.selectedShapes[0].transform.parent;
                } else {
                    obj.transform.parent = selectObjectsScript.selectedShapes[0].transform;
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
                // return;
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
            bool validPlacement = CheckPreliminaryPlacement (preliminaryObject);
            if (!validPlacement) {
                cancelScript.DisableAllPlacers (false);
                return;
            }
            preliminaryObject.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
            objectPlacingOn.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
            preliminaryObject = null;
            objectPlacingOn = null;
            if (allSelects.Count != 0) {
                cancelScript.DeselectAssociatedShapes ();
            } else {
                attachScript.attachOn = false;
            }
        }
        cancelScript.DisableAllPlacers (true);
    }

    private bool CheckPreliminaryPlacement (GameObject newObject) {
        List<GameObject> newObjectColliding = GetCollidingObjects (newObject);
        List<GameObject> objectPlacingColliding = GetCollidingObjects (objectPlacingOn);

        return newObjectColliding.Contains (objectPlacingOn) && objectPlacingColliding.Contains (newObject);
    }

    private List<GameObject> GetCollidingObjects (GameObject obj) {
        if (obj.name == "Cylinder") {
            return obj.GetComponent<CylinderColliderScript> ().collidingObjects;
        } else {
            return obj.GetComponent<ColliderScript> ().collidingObjects;
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
        GameObject newObject;
        if (obj.name == "Cylinder") {
            CylinderColliderScript cs = obj.GetComponent<CylinderColliderScript> ();
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
        } else {
            Vector3 newPoint = hit.point + (closenessFactor * hit.normal);
            newObject = Instantiate (prefab, newPoint, Quaternion.identity);
        }

        newObject.transform.rotation = obj.transform.rotation;
        preliminaryObject = newObject;
        setGovernScript.AddToFormerSet (obj, newObject);
    }

}