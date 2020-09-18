using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent (typeof (ARRaycastManager))]
[RequireComponent (typeof (ARReferencePointManager))]
public class ReferencePointManagerWithFeaturePoints : MonoBehaviour {
    [SerializeField]
    private Text debugLog;

    [SerializeField]
    private Text referencePointCount;

    [SerializeField]
    private Button clearReferencePointsButton;

    [SerializeField]
    private Button removeBallButton;

    [SerializeField]
    private Camera arCamera;

    private ARRaycastManager arRaycastManager;
    private ARReferencePointManager arReferencePointManager;
    private List<ARReferencePoint> referencePoints = new List<ARReferencePoint> ();

    private GameObject selectedBall = null;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit> ();
    private float generateAfterSeconds = 5.0f;
    private float placementTimer = 0;

    void Awake () {
        removeBallButton.gameObject.SetActive (false);
        arRaycastManager = GetComponent<ARRaycastManager> ();
        arReferencePointManager = GetComponent<ARReferencePointManager> ();
        clearReferencePointsButton.onClick.AddListener (ClearReferencePoints);
        removeBallButton.onClick.AddListener (RemoveSelectedBall);
    }

    void Update () {
        if (placementTimer >= generateAfterSeconds) {
            placementTimer = 0;
            GroupBalls ();
        } else {
            placementTimer += Time.deltaTime * 1.0f;
        }

        if (Input.touchCount == 0) {
            return;
        }
        Touch touch = Input.GetTouch (0);

        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject (touch.fingerId)) {
            return;
        }

        if (touch.phase != TouchPhase.Began) {
            return;
        }

        Ray ray = arCamera.ScreenPointToRay (touch.position);
        RaycastHit hitObject;
        if (Physics.Raycast (ray, out hitObject)) {
            debugLog.gameObject.SetActive (true);
            removeBallButton.gameObject.SetActive (true);
            ChangeColor (Color.gray);
            GroupBalls();

            GameObject obj = hitObject.transform.gameObject;
            selectedBall = obj;

            ChangeColor (Color.red);
        } else {
            if (selectedBall != null) {
                ChangeColor (Color.gray);
                GroupBalls();
                selectedBall = null;
                removeBallButton.gameObject.SetActive (false);
                return;
            }

            if (arRaycastManager.Raycast (touch.position, hits, TrackableType.FeaturePoint)) {
                Pose hitPose = hits[0].pose;
                ARReferencePoint referencePoint = arReferencePointManager.AddReferencePoint (hitPose);

                if (referencePoint == null) {
                    debugLog.gameObject.SetActive (true);
                    string errorEntry = "There was an error creating a reference point\n";
                    Debug.Log (errorEntry);
                    debugLog.text += errorEntry;
                } else {
                    referencePoints.Add (referencePoint);
                    referencePointCount.text = $"Reference Point Count: {referencePoints.Count}";
                }
            }
        }
    }

    private void ChangeColor (Color color) {
        if (selectedBall != null) {
            MeshRenderer meshRenderer = selectedBall.GetComponent<MeshRenderer> ();
            foreach (Material material in meshRenderer.materials) {
                material.color = color;
            }
        }
    }

    private void ClearReferencePoints () {
        foreach (ARReferencePoint referencePoint in referencePoints) {
            arReferencePointManager.RemoveReferencePoint (referencePoint);
        }
        referencePoints.Clear ();
        referencePointCount.text = $"Reference Point Count: {referencePoints.Count}";
    }

    private void RemoveSelectedBall () {
        ARReferencePoint point = selectedBall.GetComponent<ARReferencePoint> ();
        referencePoints.Remove (point);
        arReferencePointManager.RemoveReferencePoint (point);
        selectedBall = null;
        removeBallButton.gameObject.SetActive (false);
        referencePointCount.text = $"Reference Point Count: {referencePoints.Count}";
    }

    private void GroupBalls () {
        List<Collider[]> colliders = new List<Collider[]> ();
        List<List<GameObject>> nodes = new List<List<GameObject>> ();
        List<GameObject> allBalls = new List<GameObject> ();

        foreach (ARReferencePoint referencePoint in referencePoints) {
            GameObject ball = referencePoint.gameObject;
            allBalls.Add (ball);
            Vector3 ballPoint = ball.transform.position;
            Collider[] hitColliders = Physics.OverlapSphere (ballPoint, .5f);

            colliders.Add (hitColliders);
        }
        List<Collider[]> sortedColliders = colliders.OrderByDescending (collider => collider.Length).Where (collider => collider.Length >= 3).ToList ();

        foreach (Collider[] foundColliders in sortedColliders) {
            List<GameObject> foundBalls = foundColliders.Select (col => col.gameObject).ToList ();

            if (!nodes.Any ()) {
                nodes.Add (foundBalls);
            }
            foreach (List<GameObject> existingNode in nodes) {
                if (ListsAreDistinct (existingNode, foundBalls)) {
                    nodes.Add (foundBalls);
                    break;
                }
            }
        }

        foreach (List<GameObject> foundNode in nodes) {
            foreach (GameObject groupBall in foundNode) {
                if (groupBall == selectedBall) {
                    break;
                }
                MeshRenderer meshRenderer = groupBall.GetComponent<MeshRenderer> ();
                foreach (Material material in meshRenderer.materials) {
                    material.color = Color.green;
                }
            }
        }

        List<GameObject> allGroupedBalls = nodes.SelectMany (ball => ball).ToList ();
        foreach (GameObject ball in allBalls) {
            if (!allGroupedBalls.Contains (ball)) {
                MeshRenderer meshRenderer = ball.GetComponent<MeshRenderer> ();
                foreach (Material material in meshRenderer.materials) {
                    material.color = Color.gray;
                }
            }
        }
    }

    private bool ListsAreDistinct (List<GameObject> existingNode, List<GameObject> newNode) {
        foreach (GameObject newObj in newNode) {
            if (existingNode.Contains (newObj)) {
                return false;
            }
        }
        return true;
    }
}