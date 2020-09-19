using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class DrawLineController : MonoBehaviour {
    [SerializeField]
    private float distanceFromCamera = 1f;

    [SerializeField]
    private GameObject line;

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

    [SerializeField, Tooltip ("To specify a min constraint when drawing the next position")]
    private float minPointPositionDistance = 1.0f;

    // to show a point when user just draws a point, so index = 2
    int index = 2;

    int sortingOrder = 2;

    void Start () {
        clearButton.onClick.AddListener (ClearLines);
    }

    void Update () {
        if (Input.touchCount > 0) {
            DrawOnTouch ();
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

    void DrawOnTouch () {
        for (int i = 0; i < Input.touchCount; i++) {
            if (Input.GetTouch (i).phase == TouchPhase.Began) {
                InitializeLineRenderer (i);
            } else if (Input.GetTouch (i).phase == TouchPhase.Moved || Input.GetTouch (i).phase == TouchPhase.Stationary) {
                if (lineRenderer != null) {
                    UpdateLineRenderer (i);
                }
            } else if (Input.GetTouch (i).phase == TouchPhase.Ended) {
                index = 2;
                lineRenderer = null;
            }
        }
    }

    void InitializeLineRenderer (int i = 0) {
        if (lineRenderer == null && !EventSystem.current.IsPointerOverGameObject ()) {
            Vector3 screenPoint = GetScreenPoint (i);
            lineRenderer = Instantiate (line, arCamera.transform.position, Quaternion.identity).GetComponent<LineRenderer> ();

            // set line settings
            SetLineSettings (lineRenderer);

            // draw a point if the user just wants to draw a point
            lineRenderer.SetPosition (0, screenPoint - lineRenderer.transform.position);
            lineRenderer.SetPosition (1, screenPoint - lineRenderer.transform.position);
        }
    }

    void UpdateLineRenderer (int i = 0) {
        if (lineRenderer != null && !EventSystem.current.IsPointerOverGameObject ()) {
            Vector3 newPos = GetScreenPoint (i);
            if (PositionChanged (newPos)) {
                if (index >= lineRenderer.positionCount) {
                    lineRenderer.positionCount++;
                }

                // subtracting to always draw in front of the camera
                lineRenderer.SetPosition (index++, newPos - lineRenderer.transform.position);
            }
        }
    }

    // get the position to draw the line on the screen
    Vector3 GetScreenPoint (int i = 0) {
        return arCamera.ScreenToWorldPoint (new Vector3 (Input.GetTouch (i).position.x, Input.GetTouch (i).position.y, distanceFromCamera));
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

    private void LineWidthChanged (float newValue) {
        lineWidth = newValue;
    }

}