using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class FollowLineController : MonoBehaviour {
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

    [SerializeField]
    private Camera arCamera;

    private LineRenderer lineRenderer;

    private Color randomStartColor = Color.white;
    private Color randomEndColor = Color.white;

    [SerializeField]
    private float lineWidth = 0.0493f;

    [SerializeField]
    private Button clearButton;

    [SerializeField]
    private Button startButton;

    [SerializeField]
    private Button stopButton;

    [SerializeField, Tooltip ("To specify a min constraint when drawing the next position")]
    private float minPointPositionDistance = 1.0f;

    // to show a point when user just draws a point, so index = 2
    int index = 2;

    int sortingOrder = 2;

    void Start () {
        clearButton.onClick.AddListener (ClearLines);
        startButton.onClick.AddListener (StartLine);
        stopButton.onClick.AddListener (StopLine);
    }

    void Update () {
        if (lineRenderer != null) {
            Vector3 newPos = arCamera.transform.position;
            if (PositionChanged (newPos)) {
                if (index >= lineRenderer.positionCount) {
                    lineRenderer.positionCount++;
                }
                lineRenderer.SetPosition (index++, newPos);
            }
        }
    }

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
}