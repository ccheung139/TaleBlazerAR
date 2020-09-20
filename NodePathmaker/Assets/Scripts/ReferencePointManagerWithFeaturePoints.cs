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
[RequireComponent (typeof (ARReferencePointManager))]
public class ReferencePointManagerWithFeaturePoints : MonoBehaviour {
    [Header ("For reference point balls")]
    [SerializeField]
    private Text debugLog;

    [SerializeField]
    private Text referencePointCount;

    [SerializeField]
    private Button clearReferencePointsButton;

    [SerializeField]
    private Button removeBallButton;

    [SerializeField]
    private Button yesSisterButton;

    [SerializeField]
    private Button noSisterButton;

    [SerializeField]
    private Text sisterText;

    [SerializeField]
    private Camera arCamera;

    [Header ("For line rendering")]
    [SerializeField]
    private float distanceFromCamera = 1f;

    [SerializeField]
    private GameObject line;

    [SerializeField]
    private GameObject cube;

    [SerializeField]
    private int cornerVertices = 5;

    [SerializeField]
    private int endCapVertices = 5;

    [Header ("For simplifying lines")]

    [SerializeField]
    private float tolerance = 2f;

    [SerializeField]
    private float deltaBetweenPoints = 0.03f;

    [Header ("For detecting button press")]
    [SerializeField]
    private GraphicRaycaster GraphicRaycaster;

    [SerializeField]
    private EventSystem eventSystem;

    private LineRenderer lineRenderer;

    private Color randomStartColor = Color.white;
    private Color randomEndColor = Color.white;

    [SerializeField]
    private float lineWidth = 0.0493f;

    [SerializeField, Tooltip ("To specify a min constraint when drawing the next position")]
    private float minPointPositionDistance = 1.0f;

    // to show a point when user just draws a point, so index = 2
    int index = 2;

    int sortingOrder = 2;

    private ARRaycastManager arRaycastManager;
    private ARReferencePointManager arReferencePointManager;
    private List<ARReferencePoint> referencePoints = new List<ARReferencePoint> ();

    private GameObject selectedBall = null;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit> ();
    private float generateAfterSeconds = 5.0f;
    private float placementTimer = 0;
    private int numNodes = 0;
    private bool pathMaking = false;

    void Awake () {
        removeBallButton.gameObject.SetActive (false);
        yesSisterButton.gameObject.SetActive (false);
        noSisterButton.gameObject.SetActive (false);
        sisterText.gameObject.SetActive (false);

        arRaycastManager = GetComponent<ARRaycastManager> ();
        arReferencePointManager = GetComponent<ARReferencePointManager> ();
        clearReferencePointsButton.onClick.AddListener (ClearReferencePoints);

        removeBallButton.onClick.AddListener (RemoveSelectedBall);
        yesSisterButton.onClick.AddListener (YesSister);
        noSisterButton.onClick.AddListener (NoSister);
    }

    void Update () {
        if (placementTimer >= generateAfterSeconds) {
            placementTimer = 0;
            GroupBalls ();
        } else {
            placementTimer += Time.deltaTime * 1.0f;
        }
        if (lineRenderer != null) {
            Vector3 newPos = arCamera.transform.position;
            if (PositionChanged (newPos)) {
                if (index >= lineRenderer.positionCount) {
                    lineRenderer.positionCount++;
                }
                lineRenderer.SetPosition (index++, newPos);
            }
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
            GroupBalls ();

            GameObject obj = hitObject.transform.gameObject;
            selectedBall = obj;

            ChangeColor (Color.red);
        } else {
            if (selectedBall != null) {
                ChangeColor (Color.gray);
                GroupBalls ();
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

    // FOR LINE RENDERING ---------------------------------------------------------------------------------------------------------------------------

    private void SetLineSettings (LineRenderer currentLineRenderer) {
        currentLineRenderer.startWidth = lineWidth;
        currentLineRenderer.endWidth = lineWidth;
        currentLineRenderer.numCornerVertices = cornerVertices;
        currentLineRenderer.numCapVertices = endCapVertices;
        currentLineRenderer.Simplify (tolerance);
        currentLineRenderer.startColor = randomStartColor;
        currentLineRenderer.endColor = randomEndColor;
    }

    bool PositionChanged (Vector3 newPos) {
        Vector3 pos = lineRenderer.GetPosition (index - 1);
        float diff = Vector3.Distance (newPos, pos);

        if (diff >= deltaBetweenPoints) {
            return true;
        }
        return false;
    }

    GameObject[] GetAllLinesInScene () {
        return GameObject.FindGameObjectsWithTag ("Line");
    }

    public void Undo () {
        Debug.Log ("Undo button pressed");
        GameObject[] linesInScene = GetAllLinesInScene ();

        if (linesInScene.Length > 0) {
            Destroy (linesInScene[linesInScene.Length - 1]);
        }
    }

    private void ClearLines () {
        GameObject[] lines = GetAllLinesInScene ();
        foreach (GameObject currentLine in lines) {
            DestroyImmediate (currentLine);
        }
    }

    private void StartLine () {
        lineRenderer = Instantiate (line, arCamera.transform.position, Quaternion.identity).GetComponent<LineRenderer> ();
        SetLineSettings (lineRenderer);
        lineRenderer.SetPosition (0, arCamera.transform.position);
        lineRenderer.SetPosition (1, arCamera.transform.position);
    }

    private void StopLine () {
        index = 2;
        int numVertices = lineRenderer.positionCount;
        Debug.Log (numVertices);
        Vector3 middlePosition = lineRenderer.GetPosition (numVertices / 2);
        Instantiate (cube, middlePosition, Quaternion.identity);
        lineRenderer = null;
    }

    // FOR BALL PLACEMENT ---------------------------------------------------------------------------------------------------------------------------

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
        var result = FindColliders ();

        List<Collider[]> colliders = result.Item1;
        List<GameObject> allBalls = result.Item2;
        List<Collider[]> sortedColliders = colliders.OrderByDescending (collider => collider.Length).Where (collider => collider.Length >= 3).ToList ();

        List<List<GameObject>> nodes = FindNodes (sortedColliders);

        HandleLineSearch (nodes);

        numNodes = nodes.Count;
        ChangeNodesToGreen (nodes);
        ChangeNonNodestoGray (nodes, allBalls);
    }

    private void HandleLineSearch (List<List<GameObject>> nodes) {
        if (!pathMaking) {
            if (nodes.Count > numNodes) {
                yesSisterButton.gameObject.SetActive (true);
                noSisterButton.gameObject.SetActive (true);
                sisterText.gameObject.SetActive (true);

                debugLog.text = "JUST ADDED";
            } else {
                debugLog.text = "not added";
            }
        } else {
            if (nodes.Count > numNodes) {
                debugLog.text = "FINISHED LINE";
                pathMaking = false;
                StopLine ();
            } else if (nodes.Count < numNodes) {
                debugLog.text = "You removed something";
                pathMaking = false;
                StopLine ();
            } else {
                debugLog.text = "line still going";
            }
        }
    }

    private void YesSister () {
        yesSisterButton.gameObject.SetActive (false);
        noSisterButton.gameObject.SetActive (false);
        sisterText.gameObject.SetActive (false);
        pathMaking = true;
        StartLine ();
    }

    private void NoSister () {
        yesSisterButton.gameObject.SetActive (false);
        noSisterButton.gameObject.SetActive (false);
        sisterText.gameObject.SetActive (false);
        pathMaking = false;
    }

    private (List<Collider[]>, List<GameObject>) FindColliders () {
        List<Collider[]> colliders = new List<Collider[]> ();
        List<GameObject> allBalls = new List<GameObject> ();
        foreach (ARReferencePoint referencePoint in referencePoints) {
            GameObject ball = referencePoint.gameObject;
            allBalls.Add (ball);
            Vector3 ballPoint = ball.transform.position;

            // layerMask to only look for balls
            int layerMask = 1 << 8;
            Collider[] hitColliders = Physics.OverlapSphere (ballPoint, .5f, layerMask);

            colliders.Add (hitColliders);
        }
        return (colliders, allBalls);
    }

    private List<List<GameObject>> FindNodes (List<Collider[]> sortedColliders) {
        List<List<GameObject>> nodes = new List<List<GameObject>> ();
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
        return nodes;
    }

    private void ChangeNodesToGreen (List<List<GameObject>> nodes) {
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
    }

    private void ChangeNonNodestoGray (List<List<GameObject>> nodes, List<GameObject> allBalls) {
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