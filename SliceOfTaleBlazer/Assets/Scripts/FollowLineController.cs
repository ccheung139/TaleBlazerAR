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

    public Button searchButton;
    public Text searchingText;

    [SerializeField, Tooltip ("To specify a min constraint when drawing the next position")]
    private float minPointPositionDistance = 1.0f;

    // to show a point when user just draws a point, so index = 2
    int index = 2;
    int sortingOrder = 2;

    private float timer = 0;
    private float timerTotal = 15.0f;
    private bool searching = false;
    private float textTimer = 10f;
    private float textTimerTotal = 5.0f;

    public GameObject nestSpawnerController;

    void Start () {
        clearButton.onClick.AddListener (ClearLines);
        startButton.onClick.AddListener (StartLine);
        stopButton.onClick.AddListener (StopLine);
        searchButton.onClick.AddListener (StartSearch);

        line.SetActive (false);
    }

    void Update () {
        if (searching) {
            if (timer >= timerTotal) {
                searching = false;
                searchingText.text = "A bird was found!";
                textTimer = 0;
                timer = 0;
                StopLine ();
            } else {
                timer += Time.deltaTime * 1.0f;
            }
        }

        if (searchingText.text == "A bird was found!") {
            if (textTimer >= textTimerTotal) {
                searchingText.text = "";
            } else {
                textTimer += Time.deltaTime * 1.0f;
            }
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
        nestSpawnerController.GetComponent<NestSpawner> ().SpawnBird (middlePosition);
        lineRenderer = null;
    }

    private void StartSearch () {
        searching = true;
        searchingText.text = "Searching...";
        StartLine ();
    }
}