using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class DrawSpaceScript : MonoBehaviour {
    public Button drawSpaceButton;
    public Button drawFinishButton;
    public Button drawConnectorButton;
    public Button drawRoom2Button;
    public Button placeFrontGateButton;
    public Button placeBackGateButton;
    public Button foundFrontGateButton;
    public Button foundBackGateButton;
    public Image placeFrontGateText;
    public Image placeBackGateText;
    public Image foundFrontGateText;
    public Image foundBackGateText;

    public Camera arCamera;
    public GameObject line1;
    public SheepSpawnScript sheepSpawnScript;
    public BarnPlacementScript barnPlacementScript;
    public GameObject barnBottom;
    public GameObject grass;
    public GameObject connectorFencePrefab;
    public GameObject roomFencePrefab;
    public ARSessionOrigin origin;

    private Vector3 v3FrontTopLeft;
    private Vector3 v3FrontTopRight;
    private Vector3 v3FrontBottomLeft;
    private Vector3 v3FrontBottomRight;
    private Vector3 v3BackTopLeft;
    private Vector3 v3BackTopRight;
    private Vector3 v3BackBottomLeft;
    private Vector3 v3BackBottomRight;

    public Vector3 v3Center;
    public Vector3 v3Extents;

    public Bounds room1Bounds;
    public Bounds room2Bounds;
    public List<Bounds> connectingRooms = new List<Bounds> ();

    public Vector3 frontGatePosition;
    public Vector3 backGatePosition;

    private int createRoomStatus = 0;

    private bool isDrawing = false;
    private Vector3 firstPoint;
    private Vector3 possiblePoint;
    private Vector3 endPoint;

    private float lineWidth = 0.03f;
    private int cornerVertices = 5;
    private int endCapVertices = 5;
    private LineRenderer lineRenderer1;

    void Start () {
        drawSpaceButton.onClick.AddListener (DrawPressed);
        drawFinishButton.onClick.AddListener (DrawFinished);
        drawConnectorButton.onClick.AddListener (DrawConnectorPressed);
        drawRoom2Button.onClick.AddListener (DrawRoom2Pressed);
        placeFrontGateButton.onClick.AddListener (PlaceFrontGatePressed);
        placeBackGateButton.onClick.AddListener (PlaceBackGatePressed);
        foundFrontGateButton.onClick.AddListener (FoundFrontGatePressed);
        foundBackGateButton.onClick.AddListener (FoundBackGatePressed);
    }

    void Update () {
        if (isDrawing) {
            ClearLines ();
            possiblePoint = arCamera.transform.position + (arCamera.transform.forward * 0.5f);
            CalcPositons (possiblePoint);
            DrawBox ();
        }
    }

    private void DrawPressed () {
        // line1.gameObject.SetActive (true);
        ClearLines ();
        firstPoint = arCamera.transform.position + (arCamera.transform.forward * 0.5f);
        isDrawing = true;
        drawSpaceButton.gameObject.SetActive (false);
        drawFinishButton.gameObject.SetActive (true);
    }

    private void DrawFinished () {
        isDrawing = false;
        ClearLines ();
        endPoint = arCamera.transform.position + (arCamera.transform.forward * 0.5f);
        CalcPositons (endPoint);
        DrawBox ();
        drawFinishButton.gameObject.SetActive (false);

        DrawFences (v3FrontBottomLeft, v3FrontBottomRight);
        DrawFences (v3FrontBottomRight, v3BackBottomRight);
        DrawFences (v3BackBottomRight, v3BackBottomLeft);
        DrawFences (v3BackBottomLeft, v3FrontBottomLeft);
        ClearLines ();

        if (createRoomStatus == 0) {
            room1Bounds = BoundsHelper ();
            drawConnectorButton.gameObject.SetActive (true);
            drawRoom2Button.gameObject.SetActive (true);
        } else if (createRoomStatus == 1) {
            connectingRooms.Add (BoundsHelper ());
            drawConnectorButton.gameObject.SetActive (true);
            drawRoom2Button.gameObject.SetActive (true);
        } else {
            room2Bounds = BoundsHelper ();
            TakeOutOverlapingFences ();
            // sheepSpawnScript.StartSheepHerd (room1Bounds, room2Bounds, connectingRooms);
            placeFrontGateButton.gameObject.SetActive (true);
            placeFrontGateText.gameObject.SetActive (true);

        }
    }

    private Bounds BoundsHelper () {
        return new Bounds (v3Center, new Vector3 (v3Extents.x * 2, 100f, v3Extents.z * 2));
    }

    private void CalcPositons (Vector3 endPoint) {
        Vector3 scale = firstPoint - endPoint;
        scale.x = Mathf.Abs (scale.x);
        scale.y = Mathf.Abs (scale.y);
        scale.z = Mathf.Abs (scale.z);

        v3Center = (firstPoint + endPoint) * 0.5f;
        v3Extents = scale * 0.5f;

        PositionFinding ();
    }

    private void PositionFinding () {
        float yPoint = -0.8f;
        v3FrontTopLeft = new Vector3 (v3Center.x - v3Extents.x, yPoint, v3Center.z - v3Extents.z); // Front top left corner
        v3FrontTopRight = new Vector3 (v3Center.x + v3Extents.x, yPoint, v3Center.z - v3Extents.z); // Front top right corner
        v3FrontBottomLeft = new Vector3 (v3Center.x - v3Extents.x, yPoint, v3Center.z - v3Extents.z); // Front bottom left corner
        v3FrontBottomRight = new Vector3 (v3Center.x + v3Extents.x, yPoint, v3Center.z - v3Extents.z); // Front bottom right corner
        v3BackTopLeft = new Vector3 (v3Center.x - v3Extents.x, yPoint, v3Center.z + v3Extents.z); // Back top left corner
        v3BackTopRight = new Vector3 (v3Center.x + v3Extents.x, yPoint, v3Center.z + v3Extents.z); // Back top right corner
        v3BackBottomLeft = new Vector3 (v3Center.x - v3Extents.x, yPoint, v3Center.z + v3Extents.z); // Back bottom left corner
        v3BackBottomRight = new Vector3 (v3Center.x + v3Extents.x, yPoint, v3Center.z + v3Extents.z); // Back bottom right corner
    }

    private void SetLineSettings (LineRenderer currentLineRenderer) {
        currentLineRenderer.startWidth = lineWidth;
        currentLineRenderer.endWidth = lineWidth;
        currentLineRenderer.numCornerVertices = cornerVertices;
        currentLineRenderer.numCapVertices = endCapVertices;
    }

    void DrawBox () {
        lineRenderer1 = Instantiate (line1, v3FrontBottomLeft, Quaternion.Euler (90, 0, 0)).GetComponent<LineRenderer> ();
        SetLineSettings (lineRenderer1);
        lineRenderer1.positionCount = 5;
        lineRenderer1.SetPosition (0, v3FrontBottomLeft);
        lineRenderer1.SetPosition (1, v3FrontBottomRight);
        lineRenderer1.SetPosition (2, v3BackBottomRight);
        lineRenderer1.SetPosition (3, v3BackBottomLeft);
        lineRenderer1.SetPosition (4, v3FrontBottomLeft);
    }

    private GameObject[] GetAllLinesInScene () {
        return GameObject.FindGameObjectsWithTag ("Line");
    }

    private void ClearLines () {
        GameObject[] lines = GetAllLinesInScene ();
        foreach (GameObject currentLine in lines) {
            DestroyImmediate (currentLine);
        }
    }

    public void LoadSpace () {
        foundFrontGateButton.gameObject.SetActive (true);
        foundFrontGateText.gameObject.SetActive (true);
    }

    private void TranslateAndRotateLoad () {
        SpaceData data = SaveSpace.LoadPlayerSpace ();

        Vector3 oldFront = FloatToVector (data.frontGatePosition);
        Vector3 oldBack = FloatToVector (data.backGatePosition);
        float angles = Vector3.SignedAngle (oldBack - oldFront, backGatePosition - frontGatePosition, Vector3.up);
        Vector3 relative = new Vector3 (0, angles, 0);
        Vector3 translation = oldFront - frontGatePosition;
        translation.y = 0.8f;

        LoadCenterAndExtents (out room1Bounds, data.room1Center, data.room1Extents, translation, relative, frontGatePosition);
        // LoadFences (room1Bounds);

        LoadCenterAndExtents (out room2Bounds, data.room2Center, data.room2Extents, translation, relative, frontGatePosition);
        // LoadFences (room2Bounds);

        for (int i = 0; i < data.connectingRoomCenters.GetLength (0); i++) {
            Bounds connectBounds;
            float[] connectCenter = new float[3];
            connectCenter[0] = data.connectingRoomCenters[i, 0];
            connectCenter[1] = data.connectingRoomCenters[i, 1];
            connectCenter[2] = data.connectingRoomCenters[i, 2];

            float[] connectExtent = new float[3];
            connectExtent[0] = data.connectingRoomExtents[i, 0];
            connectExtent[1] = data.connectingRoomExtents[i, 1];
            connectExtent[2] = data.connectingRoomExtents[i, 2];
            LoadCenterAndExtents (out connectBounds, connectCenter, connectExtent, translation, relative, frontGatePosition);
            // LoadFences (connectBounds);
            connectingRooms.Add (connectBounds);
        }

        // TakeOutOverlapingFences ();
    }

    private Vector3 TranslateAndRotateVector (Vector3 oldVector, Vector3 translation, Quaternion relative) {
        Vector3 newVector = oldVector;
        newVector += translation;
        return relative * newVector;
    }

    private Vector3 FloatToVector (float[] f) {
        Vector3 ret;
        ret.x = f[0];
        ret.y = f[1];
        ret.z = f[2];
        return ret;
    }

    private void LoadFences (Bounds bounds) {
        v3Center = bounds.center;
        v3Extents = bounds.extents;
        PositionFinding ();
        DrawFences (v3FrontBottomLeft, v3FrontBottomRight);
        DrawFences (v3FrontBottomRight, v3BackBottomRight);
        DrawFences (v3BackBottomRight, v3BackBottomLeft);
        DrawFences (v3BackBottomLeft, v3FrontBottomLeft);
    }

    private void LoadCenterAndExtents (out Bounds bound, float[] centerData, float[] extentsData, Vector3 translation, Vector3 relative, Vector3 ppp) {
        Vector3 center;
        center.x = centerData[0];
        center.y = centerData[1];
        center.z = centerData[2];

        Vector3 extents;
        extents.x = extentsData[0];
        extents.y = 100f;
        extents.z = extentsData[2];

        Vector3 topRight = center;
        topRight.x += extents.x / 2f;
        topRight.z += extents.z / 2f;
        Vector3 topLeft = center;
        topLeft.x -= extents.x / 2f;
        topLeft.z += extents.z / 2f;
        Vector3 bottomRight = center;
        bottomRight.x += extents.x / 2f;
        bottomRight.z -= extents.z / 2f;
        Vector3 bottomLeft = center;
        bottomLeft.x -= extents.x / 2f;
        bottomLeft.z -= extents.z / 2f;

        center -= translation;
        topRight -= translation;
        topLeft -= translation;
        bottomRight -= translation;
        bottomLeft -= translation;

        center = RotatePointAroundPivot (center, center, relative);
        topRight = RotatePointAroundPivot (topRight, ppp, relative);
        topLeft = RotatePointAroundPivot (topLeft, ppp, relative);
        bottomRight = RotatePointAroundPivot (bottomRight, ppp, relative);
        bottomLeft = RotatePointAroundPivot (bottomLeft, ppp, relative);
        DrawFences (topRight, topLeft);
        DrawFences (topLeft, bottomLeft);
        DrawFences (bottomLeft, bottomRight);
        DrawFences (bottomRight, topRight);

        bound = new Bounds (center, extents);
    }

    private Vector3 RotatePointAroundPivot (Vector3 point, Vector3 pivot, Vector3 angles) {
        Vector3 dir = point - pivot;
        dir = Quaternion.Euler (angles) * dir;
        point = dir + pivot;
        return point;
    }

    private Rect CreateRect () {
        float width = v3BackBottomRight.x - v3FrontBottomLeft.x;
        float x = width > 0 ? v3FrontBottomLeft.x : v3BackBottomRight.x;

        float height = v3BackBottomRight.z - v3FrontBottomLeft.z;
        float z = height > 0 ? v3FrontBottomLeft.z : v3BackBottomRight.z;

        return new Rect (x, z, width, height);
    }

    private void DrawFences (Vector3 point1, Vector3 point2) {
        Vector3 direction = Vector3.Normalize (point2 - point1);

        float distance = Vector3.Distance (point2, point1);
        float counter = 0;
        Vector3 pointToPlace = point1;

        GameObject typeOfFence;
        if (createRoomStatus == 0 || createRoomStatus == 2) {
            typeOfFence = roomFencePrefab;
        } else {
            typeOfFence = connectorFencePrefab;
        }
        float fenceLength = typeOfFence.GetComponent<MeshRenderer> ().bounds.size.x;
        while (counter < distance) {
            Instantiate (typeOfFence, pointToPlace, Quaternion.LookRotation (direction, Vector3.up) * Quaternion.Euler (0, 90, 0));
            counter += fenceLength;
            pointToPlace = point1 + (direction * counter);
        }
    }

    private void TakeOutOverlapingFences () {
        List<Bounds> allBounds = new List<Bounds> (connectingRooms);
        allBounds.Add (room1Bounds);
        allBounds.Add (room2Bounds);

        List<GameObject> fencesToRemove = new List<GameObject> ();
        foreach (GameObject fence in GameObject.FindGameObjectsWithTag ("Fence")) {
            Vector3 fencePos = fence.transform.position;
            int count = 0;
            foreach (Bounds potentialBound in allBounds) {
                if (potentialBound.Contains (fencePos)) {
                    count += 1;
                }
            }
            if (count > 1) {
                fencesToRemove.Add (fence);
            }
        }
        foreach (GameObject fenceToRemove in fencesToRemove) {
            Destroy (fenceToRemove);
        }
    }

    private void DrawConnectorPressed () {
        drawSpaceButton.gameObject.SetActive (true);
        drawConnectorButton.gameObject.SetActive (false);
        drawRoom2Button.gameObject.SetActive (false);
        createRoomStatus = 1;
    }

    private void DrawRoom2Pressed () {
        drawSpaceButton.gameObject.SetActive (true);
        drawConnectorButton.gameObject.SetActive (false);
        drawRoom2Button.gameObject.SetActive (false);
        createRoomStatus = 2;
    }

    private void PlaceFrontGatePressed () {
        frontGatePosition = arCamera.transform.position;
        placeBackGateButton.gameObject.SetActive (true);
        placeFrontGateButton.gameObject.SetActive (false);
        placeFrontGateText.gameObject.SetActive (false);
        placeBackGateText.gameObject.SetActive (true);
    }

    private void PlaceBackGatePressed () {
        backGatePosition = arCamera.transform.position;
        SaveSpace.SavePlayerSpace (this);
        placeBackGateButton.gameObject.SetActive (false);
        placeBackGateText.gameObject.SetActive (false);
        sheepSpawnScript.StartSheepHerd (room1Bounds, room2Bounds, connectingRooms);
    }

    private void FoundFrontGatePressed () {
        frontGatePosition = arCamera.transform.position;
        foundBackGateButton.gameObject.SetActive (true);
        foundFrontGateButton.gameObject.SetActive (false);
        foundFrontGateText.gameObject.SetActive (false);
        foundBackGateText.gameObject.SetActive (true);
    }

    private void FoundBackGatePressed () {
        backGatePosition = arCamera.transform.position;
        foundBackGateButton.gameObject.SetActive (false);
        foundBackGateText.gameObject.SetActive (false);
        TranslateAndRotateLoad ();
        // sheepSpawnScript.StartSheepHerd (room1Bounds, room2Bounds, connectingRooms);
    }

}