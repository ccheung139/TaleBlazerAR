using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawSpaceScript : MonoBehaviour {
    public Button drawSpaceButton;
    public Button drawFinishButton;
    public Button moveGrassButton;
    public Button finishPlacingButton;
    public Button placeGrassButton;

    public Camera arCamera;
    public GameObject line1;
    public BarnPlacementScript barnPlacementScript;
    public GameObject barnBottom;
    public GameObject grass;
    public Image placeGameBounds;
    public GameObject fencePrefab;

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

    private bool isDrawing = false;
    private bool movingGrass = false;
    private Vector3 firstPoint;
    private Vector3 possiblePoint;
    private Vector3 endPoint;

    private float lineWidth = 0.03f;
    private int cornerVertices = 5;
    private int endCapVertices = 5;
    private LineRenderer lineRenderer1;
    private LineRenderer lineRenderer2;
    private LineRenderer lineRenderer3;
    private LineRenderer lineRenderer4;

    // Start is called before the first frame update
    void Start () {
        drawSpaceButton.onClick.AddListener (DrawPressed);
        drawFinishButton.onClick.AddListener (DrawFinished);
        moveGrassButton.onClick.AddListener (MoveGrassPressed);
        finishPlacingButton.onClick.AddListener (FinishPlacingPressed);
        placeGrassButton.onClick.AddListener (PlaceGrassPressed);
    }

    void Update () {
        if (isDrawing) {
            ClearLines ();
            possiblePoint = arCamera.transform.position + (arCamera.transform.forward * 0.5f);
            CalcPositons (possiblePoint);
            DrawBox ();
        } else if (movingGrass) {
            grass.transform.rotation = Quaternion.Euler (90, arCamera.transform.eulerAngles.y, 0);
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
        SaveSpace.SavePlayerSpace (this);

        // SizeGrass ();
        // ClearLines ();
        DrawFences (v3FrontBottomLeft, v3FrontBottomRight);
        DrawFences (v3FrontBottomRight, v3BackBottomRight);
        DrawFences (v3BackBottomRight, v3BackBottomLeft);
        DrawFences (v3BackBottomLeft, v3FrontBottomLeft);
        barnPlacementScript.PlaceBarnPressed (v3Center, v3Extents);
    }

    private void SizeGrass () {
        grass.SetActive (true);
        Stretch (grass, v3FrontBottomLeft, v3FrontBottomRight, v3BackBottomLeft, true);
        grass.transform.position = new Vector3 (v3Center.x, -0.8f, v3Center.z);
    }

    private void CalcPositons (Vector3 endPoint) {
        Vector3 scale = firstPoint - endPoint;
        scale.x = Mathf.Abs (scale.x);
        scale.y = Mathf.Abs (scale.y);
        scale.z = Mathf.Abs (scale.z);

        // float yPoint = barnBottom.transform.position.y;
        // float yPoint = -0.3f;
        v3Center = (firstPoint + endPoint) * 0.5f;
        v3Extents = scale * 0.5f;

        PositionFinding ();
    }

    public void Stretch (GameObject _sprite, Vector3 _initialPosition, Vector3 secondPos, Vector3 _finalPosition, bool _mirrorZ) {
        float width = _sprite.GetComponent<SpriteRenderer> ().bounds.size.x;
        Vector3 scale = new Vector3 (1, 1, 1);
        scale.x = Vector3.Distance (_initialPosition, secondPos) / width;
        scale.y = Vector3.Distance (_initialPosition, _finalPosition) / width;
        _sprite.transform.localScale = scale;
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
        SpaceData data = SaveSpace.LoadPlayerSpace ();
        Vector3 center;
        center.x = data.center[0];
        center.y = data.center[1];
        center.z = data.center[2];
        v3Center = center;

        Vector3 extents;
        extents.x = data.extents[0];
        extents.y = data.extents[1];
        extents.z = data.extents[2];
        v3Extents = extents;

        PositionFinding ();
        // SizeGrass ();
        // moveGrassButton.gameObject.SetActive (true);
        // finishPlacingButton.gameObject.SetActive (true);
    }

    private void MoveGrassPressed () {
        movingGrass = true;
        grass.transform.parent = arCamera.transform;
        moveGrassButton.gameObject.SetActive (false);
        placeGrassButton.gameObject.SetActive (true);
    }

    private void PlaceGrassPressed () {
        movingGrass = false;
        grass.transform.parent = null;
        // grass.transform.rotation = Quaternion.Euler (90, 0, 0);
        moveGrassButton.gameObject.SetActive (true);
        placeGrassButton.gameObject.SetActive (false);
    }

    private void FinishPlacingPressed () {
        PlaceGrassPressed ();
        moveGrassButton.gameObject.SetActive (false);
        placeGrassButton.gameObject.SetActive (false);
        finishPlacingButton.gameObject.SetActive (false);
        placeGameBounds.gameObject.SetActive (false);

        Vector3 newCenter = grass.transform.position;
        barnPlacementScript.PlaceBarnPressed (newCenter, v3Extents);
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
        float fenceLength = fencePrefab.GetComponent<MeshRenderer> ().bounds.size.x;
        float distance = Vector3.Distance (point2, point1);
        float counter = 0;
        Vector3 pointToPlace = point1;
        while (counter < distance) {
            Instantiate (fencePrefab, pointToPlace, Quaternion.LookRotation (direction, Vector3.up) * Quaternion.Euler (0, 90, 0));
            counter += fenceLength;
            pointToPlace = point1 + (direction * counter);
        }
    }

}