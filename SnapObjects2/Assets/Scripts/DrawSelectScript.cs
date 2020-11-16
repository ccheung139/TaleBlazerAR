using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawSelectScript : MonoBehaviour {
    public SelectObjectsScript selectObjectsScript;

    public Button drawSelectButton;
    public Button drawFinishButton;
    public Camera arCamera;
    public GameObject line1;
    public GameObject line2;
    public GameObject line3;
    public GameObject line4;

    private Vector3 v3FrontTopLeft;
    private Vector3 v3FrontTopRight;
    private Vector3 v3FrontBottomLeft;
    private Vector3 v3FrontBottomRight;
    private Vector3 v3BackTopLeft;
    private Vector3 v3BackTopRight;
    private Vector3 v3BackBottomLeft;
    private Vector3 v3BackBottomRight;

    private Vector3 v3Center;
    private Vector3 v3Extents;

    private bool isDrawing = false;
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
        drawSelectButton.onClick.AddListener (DrawPressed);
        drawFinishButton.onClick.AddListener (DrawFinished);
    }

    void Update () {
        if (isDrawing) {
            // lineRenderer = null;
            ClearLines ();
            possiblePoint = arCamera.transform.position + (arCamera.transform.forward * 0.5f);
            CalcPositons (possiblePoint);
            DrawBox ();
            // isDrawing = false;
            selectObjectsScript.SelectInBox (v3Center, v3Extents, false);
        }
    }

    private void DrawPressed () {
        ClearLines ();
        selectObjectsScript.DeselectGroup ();
        firstPoint = arCamera.transform.position + (arCamera.transform.forward * 0.5f);
        isDrawing = true;
    }

    private void DrawFinished () {
        isDrawing = false;
        ClearLines ();
        endPoint = arCamera.transform.position + (arCamera.transform.forward * 0.5f);
        CalcPositons (endPoint);
        DrawBox ();
        selectObjectsScript.SelectInBox (v3Center, v3Extents, true);
    }

    void CalcPositons (Vector3 endPoint) {
        Vector3 scale = firstPoint - endPoint;
        scale.x = Mathf.Abs (scale.x);
        scale.y = Mathf.Abs (scale.y);
        scale.z = Mathf.Abs (scale.z);

        v3Center = (firstPoint + endPoint) * 0.5f;
        v3Extents = scale * 0.5f;

        v3FrontTopLeft = new Vector3 (v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z); // Front top left corner
        v3FrontTopRight = new Vector3 (v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z); // Front top right corner
        v3FrontBottomLeft = new Vector3 (v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z); // Front bottom left corner
        v3FrontBottomRight = new Vector3 (v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z); // Front bottom right corner
        v3BackTopLeft = new Vector3 (v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z); // Back top left corner
        v3BackTopRight = new Vector3 (v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z); // Back top right corner
        v3BackBottomLeft = new Vector3 (v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z); // Back bottom left corner
        v3BackBottomRight = new Vector3 (v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z); // Back bottom right corner
    }

    private void SetLineSettings (LineRenderer currentLineRenderer) {
        currentLineRenderer.startWidth = lineWidth;
        currentLineRenderer.endWidth = lineWidth;
        currentLineRenderer.numCornerVertices = cornerVertices;
        currentLineRenderer.numCapVertices = endCapVertices;
    }

    void DrawBox () {
        lineRenderer1 = Instantiate (line1, v3FrontTopLeft, Quaternion.identity).GetComponent<LineRenderer> ();
        SetLineSettings (lineRenderer1);
        lineRenderer1.positionCount = 5;
        lineRenderer1.SetPosition (0, v3FrontTopLeft);
        lineRenderer1.SetPosition (1, v3FrontTopRight);
        lineRenderer1.SetPosition (2, v3FrontBottomRight);
        lineRenderer1.SetPosition (3, v3FrontBottomLeft);
        lineRenderer1.SetPosition (4, v3FrontTopLeft);

        lineRenderer2 = Instantiate (line2, v3FrontTopLeft, Quaternion.identity).GetComponent<LineRenderer> ();
        SetLineSettings (lineRenderer2);
        lineRenderer2.positionCount = 5;
        lineRenderer2.SetPosition (0, v3BackTopLeft);
        lineRenderer2.SetPosition (1, v3BackTopRight);
        lineRenderer2.SetPosition (2, v3BackBottomRight);
        lineRenderer2.SetPosition (3, v3BackBottomLeft);
        lineRenderer2.SetPosition (4, v3BackTopLeft);

        lineRenderer3 = Instantiate (line3, v3FrontTopLeft, Quaternion.identity).GetComponent<LineRenderer> ();
        SetLineSettings (lineRenderer3);
        lineRenderer3.positionCount = 5;
        lineRenderer3.SetPosition (0, v3FrontTopLeft);
        lineRenderer3.SetPosition (1, v3BackTopLeft);
        lineRenderer3.SetPosition (2, v3BackBottomLeft);
        lineRenderer3.SetPosition (3, v3FrontBottomLeft);
        lineRenderer3.SetPosition (4, v3FrontTopLeft);

        lineRenderer4 = Instantiate (line4, v3FrontTopLeft, Quaternion.identity).GetComponent<LineRenderer> ();
        SetLineSettings (lineRenderer4);
        lineRenderer4.positionCount = 5;
        lineRenderer4.SetPosition (0, v3FrontTopRight);
        lineRenderer4.SetPosition (1, v3BackTopRight);
        lineRenderer4.SetPosition (2, v3BackBottomRight);
        lineRenderer4.SetPosition (3, v3FrontBottomRight);
        lineRenderer4.SetPosition (4, v3FrontTopRight);
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
}