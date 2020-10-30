using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class FollowLineController : MonoBehaviour {
    public float distanceFromCamera = 1f;
    public GameObject line;
    public GameObject cube;
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

    public Color randomStartColor = Color.white;
    public Color randomEndColor = Color.white;

    public float lineWidth = 0.0493f;
    public Button clearButton;

    public Button searchButton;
    public Text searchingText;

    [SerializeField, Tooltip ("To specify a min constraint when drawing the next position")]
    public float minPointPositionDistance = 1.0f;

    // to show a point when user just draws a point, so index = 2
    int index = 2;

    private float timer = 0;
    private float timerTotal = 15.0f;
    private bool searching = false;
    private float textTimer = 10f;
    private float textTimerTotal = 5.0f;

    public GameObject nestSpawnerController;
    public GameObject breadPrefab;
    public InventoryScript invScript;

    private int totalBreads = 4;

    void Start () {
        clearButton.onClick.AddListener (ClearLines);
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
        Vector3 middlePosition = lineRenderer.GetPosition (numVertices / 2);
        middlePosition.y = -0.5f;
        // Instantiate (cube, middlePosition, Quaternion.identity);
        nestSpawnerController.GetComponent<NestSpawner> ().SpawnBird (middlePosition);
        lineRenderer = null;
    }

    private void StartSearch () {
        searching = true;
        searchingText.text = "Searching...";
        StartLine ();

        System.Random rand = new System.Random ();
        for (int i = 0; i < totalBreads; i++) {
            SpawnBreads (rand);
        }
    }

    private void SpawnBreads (System.Random rand) {
        Vector3 playerPosition = arCamera.transform.position;

        float randX = (float) (rand.NextDouble () * 2);
        float randZ = (float) (rand.NextDouble () * 2);
        Vector3 newPosition = playerPosition + new Vector3 (rand.Next (2) == 1 ? randX : -randX, -0.2f, randZ);

        GameObject newBread = Instantiate (breadPrefab, newPosition, Quaternion.identity);
        BreadScript bs = newBread.GetComponent<BreadScript> ();
        bs.arCamera = arCamera;
        bs.invScript = invScript;
        bs.owned = false;
    }
}