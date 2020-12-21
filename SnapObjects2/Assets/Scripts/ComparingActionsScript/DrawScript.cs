using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class DrawScript : MonoBehaviour {
    public float distanceFromCamera = 1f;
    public GameObject line;
    public GameObject line2;
    public int cornerVertices = 5;
    public int endCapVertices = 5;

    [Header ("For simplifying lines")]
    public float tolerance = 2f;
    public float deltaBetweenPoints = 0.03f;

    [Header ("For detecting button press")]
    public GraphicRaycaster GraphicRaycaster;
    public EventSystem eventSystem;
    public Camera arCamera;
    public LineRenderer lineRenderer;
    public LineRenderer lineRenderer2;

    public float lineWidth = 0.0493f;
    public Button clearButton;

    public Button drawButton;
    public Button drawButton2;
    public Button stopButton;
    public Button stopButton2;
    public Button compareButton;
    public LineCompareScript lineCompareScript;

    private bool drawing = false;
    private bool drawing2 = false;

    [SerializeField, Tooltip ("To specify a min constraint when drawing the next position")]
    public float minPointPositionDistance = 1.0f;

    // to show a point when user just draws a point, so index = 2
    int index = 2;
    int index2 = 2;

    void Start () {
        // clearButton.onClick.AddListener (ClearLines);
        drawButton.onClick.AddListener (DrawPressed);
        drawButton2.onClick.AddListener (DrawPressed2);
        stopButton.onClick.AddListener (StopLine);
        stopButton2.onClick.AddListener (StopLine2);
        compareButton.onClick.AddListener (CompareLines);
        clearButton.onClick.AddListener (ClearLines);
    }

    void Update () {
        if (drawing) {
            Vector3 newPos = arCamera.transform.position;
            if (PositionChanged (newPos)) {
                if (index >= lineRenderer.positionCount) {
                    lineRenderer.positionCount++;
                }
                lineRenderer.SetPosition (index++, newPos);
            }
        }
        if (drawing2) {
            Vector3 newPos = arCamera.transform.position;
            if (PositionChanged2 (newPos)) {
                if (index2 >= lineRenderer2.positionCount) {
                    lineRenderer2.positionCount++;
                }
                lineRenderer2.SetPosition (index2++, newPos);
            }
        }
    }

    private void SetLineSettings (LineRenderer currentLineRenderer) {
        currentLineRenderer.startWidth = lineWidth;
        currentLineRenderer.endWidth = lineWidth;
        currentLineRenderer.numCornerVertices = cornerVertices;
        currentLineRenderer.numCapVertices = endCapVertices;
        currentLineRenderer.Simplify (tolerance);
    }

    bool PositionChanged (Vector3 newPos) {
        Vector3 pos = lineRenderer.GetPosition (index - 1);
        float diff = Vector3.Distance (newPos, pos);

        if (diff >= deltaBetweenPoints) {
            return true;
        }
        return false;
    }

    bool PositionChanged2 (Vector3 newPos) {
        Vector3 pos = lineRenderer2.GetPosition (index2 - 1);
        float diff = Vector3.Distance (newPos, pos);

        if (diff >= deltaBetweenPoints) {
            return true;
        }
        return false;
    }

    private void StartLine () {
        lineRenderer = Instantiate (line, arCamera.transform.position, Quaternion.identity).GetComponent<LineRenderer> ();
        SetLineSettings (lineRenderer);
        lineRenderer.SetPosition (0, arCamera.transform.position);
        lineRenderer.SetPosition (1, arCamera.transform.position);
        drawing = true;
    }

    private void StartLine2 () {
        lineRenderer2 = Instantiate (line2, arCamera.transform.position, Quaternion.identity).GetComponent<LineRenderer> ();
        SetLineSettings (lineRenderer2);
        lineRenderer2.SetPosition (0, arCamera.transform.position);
        lineRenderer2.SetPosition (1, arCamera.transform.position);
        drawing2 = true;
    }

    private void StopLine () {
        index = 2;
        int numVertices = lineRenderer.positionCount;
        // lineRenderer = null;
        drawing = false;
    }

    private void StopLine2 () {
        index2 = 2;
        drawing2 = false;
        // lineRenderer2 = null;
    }

    private void DrawPressed () {
        StartLine ();
    }

    private void DrawPressed2 () {
        StartLine2 ();
    }

    private void CompareLines () {
        lineCompareScript.CompareLines (lineRenderer, lineRenderer2);
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
        drawing = false;
        drawing2 = false;
        GameObject[] lines = GetAllLinesInScene ();
        foreach (GameObject currentLine in lines) {
            DestroyImmediate (currentLine);
        }
    }
}