using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class CustomActionsScript : MonoBehaviour {
    public Camera arCamera;
    public Button defineButton;
    public Button drawInitialButton;
    public Button finishInitialButton;
    public Button drawReplicaButton;
    public Button finishReplicaButton;
    public Button clearButton;
    public Button hintButton;

    public LineEvaluateScript lineEvaluateScript;
    public CustomReactionScript customReactionScript;

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
    public LineRenderer lineRenderer;
    public LineRenderer lineRenderer2;

    public float lineWidth = 0.0493f;

    private bool drawing = false;
    private bool drawing2 = false;

    [SerializeField, Tooltip ("To specify a min constraint when drawing the next position")]
    public float minPointPositionDistance = 1.0f;

    int index = 2;
    int index2 = 2;

    void Start () {
        defineButton.onClick.AddListener (DefinePressed);
        drawInitialButton.onClick.AddListener (DrawInitialPressed);
        finishInitialButton.onClick.AddListener (FinishInitialPressed);
        drawReplicaButton.onClick.AddListener (DrawReplicaPressed);
        finishReplicaButton.onClick.AddListener (FinishReplicaPressed);
        clearButton.onClick.AddListener (ClearLines);
        hintButton.onClick.AddListener (ToggleHint);
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

    private void DefinePressed () {
        drawInitialButton.gameObject.SetActive (true);
        finishInitialButton.gameObject.SetActive (true);
    }

    private void DrawInitialPressed () {
        StartLine ();
    }

    private void FinishInitialPressed () {
        StopLine ();
        drawInitialButton.gameObject.SetActive (false);
        finishInitialButton.gameObject.SetActive (false);
        drawReplicaButton.gameObject.SetActive (true);
        finishReplicaButton.gameObject.SetActive (true);
        hintButton.gameObject.SetActive (true);
    }

    private void DrawReplicaPressed () {
        StartLine2 ();
    }

    private void FinishReplicaPressed () {
        StopLine2 ();

    }

    private void SetLineSettings (LineRenderer currentLineRenderer) {
        currentLineRenderer.gameObject.SetActive (true);
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
        lineRenderer.gameObject.SetActive (false);
    }

    private void StopLine2 () {
        index2 = 2;
        drawing2 = false;
        // lineRenderer2 = null;
        CompareLines ();
    }

    private void DrawPressed () {
        StartLine ();
    }

    private void DrawPressed2 () {
        StartLine2 ();
    }

    private void ToggleHint () {
        if (lineRenderer != null) {
            lineRenderer.gameObject.SetActive (true);
        }
    }

    private void CompareLines () {
        if (lineEvaluateScript.CompareLines (lineRenderer, lineRenderer2)) {
            customReactionScript.PerformReact ();
            drawReplicaButton.gameObject.SetActive (false);
            finishReplicaButton.gameObject.SetActive (false);
            hintButton.gameObject.SetActive (false);
            ClearLines ();
        }
        DestroyImmediate (lineRenderer2);
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

        drawInitialButton.gameObject.SetActive (false);
        finishInitialButton.gameObject.SetActive (false);
        drawReplicaButton.gameObject.SetActive (false);
        finishReplicaButton.gameObject.SetActive (false);
        hintButton.gameObject.SetActive (false);
    }
}