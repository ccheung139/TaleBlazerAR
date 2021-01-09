using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadSpaceScript : MonoBehaviour {
    public DrawSpaceScript drawSpaceScript;
    public Image foundFrontGateText;
    public Image foundBackGateText;
    public Button foundFrontGateButton;
    public Button foundBackGateButton;
    public GameObject roomFencePrefab;
    public GameObject connectorFencePrefab;
    public Camera arCamera;
    public SheepSpawnScript sheepSpawnScript;

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

    public Vector3 frontGatePosition;
    public Vector3 backGatePosition;

    public Bounds room1Bounds;
    public Bounds room2Bounds;
    public List<Bounds> connectingRooms = new List<Bounds> ();

    public Vector3 pivot;
    public Vector3 relative;

    void Start () {
        foundFrontGateButton.onClick.AddListener (FoundFrontGatePressed);
        foundBackGateButton.onClick.AddListener (FoundBackGatePressed);
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
        sheepSpawnScript.StartSheepHerd (room1Bounds, room2Bounds, connectingRooms, pivot, relative);
    }

    private Vector3 TranslateAndRotateVector (Vector3 oldVector, Vector3 translation, Quaternion relative) {
        Vector3 newVector = oldVector;
        newVector += translation;
        return relative * newVector;
    }

    private void LoadCenterAndExtents (out Bounds bound, float[] centerData, float[] extentsData, Vector3 translation, int status) {
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

        Vector3 rotatedCenter = RotatePointAroundPivot (center, pivot, relative);
        topRight = RotatePointAroundPivot (topRight, pivot, relative);
        topLeft = RotatePointAroundPivot (topLeft, pivot, relative);
        bottomRight = RotatePointAroundPivot (bottomRight, pivot, relative);
        bottomLeft = RotatePointAroundPivot (bottomLeft, pivot, relative);
        drawSpaceScript.DrawFences (topRight, topLeft, status);
        drawSpaceScript.DrawFences (topLeft, bottomLeft, status);
        drawSpaceScript.DrawFences (bottomLeft, bottomRight, status);
        drawSpaceScript.DrawFences (bottomRight, topRight, status);

        bound = new Bounds (center, extents);
    }

    public Vector3 RotatePointAroundPivot (Vector3 point, Vector3 pivot, Vector3 angles) {
        Vector3 dir = point - pivot;
        dir = Quaternion.Euler (angles) * dir;
        point = dir + pivot;
        return point;
    }

    public void LoadSpace () {
        foundFrontGateButton.gameObject.SetActive (true);
        foundFrontGateText.gameObject.SetActive (true);
    }

    private void TranslateAndRotateLoad () {
        SpaceData data = SaveSpace.LoadPlayerSpace ();

        Vector3 oldFront = FloatToVector (data.frontGatePosition);
        Vector3 oldBack = FloatToVector (data.backGatePosition);
        Vector3 oldDirection = oldBack - oldFront;
        oldDirection.y = 0;

        Vector3 newDirection = backGatePosition - frontGatePosition;
        newDirection.y = 0;
        float angles = Vector3.SignedAngle (oldDirection, newDirection, Vector3.up);
        relative = new Vector3 (0, angles, 0);
        pivot = frontGatePosition;

        Vector3 translation = oldFront - frontGatePosition;
        translation.y = 0.8f;

        LoadCenterAndExtents (out room1Bounds, data.room1Center, data.room1Extents, translation, 0);
        // LoadFences (room1Bounds);

        LoadCenterAndExtents (out room2Bounds, data.room2Center, data.room2Extents, translation, 2);
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
            LoadCenterAndExtents (out connectBounds, connectCenter, connectExtent, translation, 1);
            // LoadFences (connectBounds);
            connectingRooms.Add (connectBounds);
        }

        TakeOutOverlapingFences ();
    }

    private Vector3 FloatToVector (float[] f) {
        Vector3 ret;
        ret.x = f[0];
        ret.y = f[1];
        ret.z = f[2];
        return ret;
    }

    private void PositionFinding () {
        float yPoint = -0.8f;
        v3FrontBottomLeft = new Vector3 (v3Center.x - v3Extents.x, yPoint, v3Center.z - v3Extents.z); // Front bottom left corner
        v3FrontBottomRight = new Vector3 (v3Center.x + v3Extents.x, yPoint, v3Center.z - v3Extents.z); // Front bottom right corner
        v3BackBottomLeft = new Vector3 (v3Center.x - v3Extents.x, yPoint, v3Center.z + v3Extents.z); // Back bottom left corner
        v3BackBottomRight = new Vector3 (v3Center.x + v3Extents.x, yPoint, v3Center.z + v3Extents.z); // Back bottom right corner
    }

    public void TakeOutOverlapingFences () {
        List<Bounds> allBounds = new List<Bounds> (connectingRooms);
        allBounds.Add (room1Bounds);
        allBounds.Add (room2Bounds);

        List<GameObject> fencesToRemove = new List<GameObject> ();
        foreach (GameObject fence in GameObject.FindGameObjectsWithTag ("Fence")) {
            Vector3 fencePos = fence.transform.position;
            Vector3 rotatedFencePos = RotatePointAroundPivot (fencePos, pivot, -relative);
            int count = 0;
            foreach (Bounds potentialBound in allBounds) {
                if (potentialBound.Contains (rotatedFencePos)) {
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
}