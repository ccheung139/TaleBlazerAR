using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class CustomReactionScript : MonoBehaviour {
    public Button defineReactionButton;
    public Button drawReactionButton;
    public Button finishReactionButton;
    public Camera arCamera;
    public LineEvaluateScript lineEvaluateScript;
    public GameObject referenceCube;
    public Button resetButton;

    public float distanceFromCamera = 1f;
    public GameObject line;
    public int cornerVertices = 5;
    public int endCapVertices = 5;

    [Header ("For simplifying lines")]
    public float tolerance = 2f;
    public float deltaBetweenPoints = 0.03f;

    [Header ("For detecting button press")]
    public GraphicRaycaster GraphicRaycaster;
    public EventSystem eventSystem;
    public LineRenderer lineRenderer;

    public float lineWidth = 0.0493f;

    private bool drawing = false;
    private bool reactMove = false;

    [SerializeField, Tooltip ("To specify a min constraint when drawing the next position")]
    public float minPointPositionDistance = 1.0f;

    int index = 2;
    private float movementSpeed = 0.5f;
    int i = 0;
    Vector3[] positions;
    private List<float> times = new List<float> ();
    private List<Quaternion> rotations = new List<Quaternion> ();
    Gyroscope m_Gyro;
    private Quaternion startRotation;
    private Quaternion startCubeRotation;

    // Start is called before the first frame update
    void Start () {
        defineReactionButton.onClick.AddListener (DefinePressed);
        drawReactionButton.onClick.AddListener (DrawPressed);
        finishReactionButton.onClick.AddListener (FinishPressed);
        resetButton.onClick.AddListener (ResetPressed);

        m_Gyro = Input.gyro;
        m_Gyro.enabled = true;
    }

    // Update is called once per frame
    void Update () {
        if (drawing) {
            Vector3 newPos = arCamera.transform.position;
            if (PositionChanged (newPos)) {
                if (index >= lineRenderer.positionCount) {
                    lineRenderer.positionCount++;
                }
                lineRenderer.SetPosition (index++, newPos);
                times.Add (Time.time);
                Quaternion relativeRotation = startRotation * Quaternion.Inverse (arCamera.transform.rotation);
                Quaternion rotation = Quaternion.Inverse (startCubeRotation * relativeRotation);
                rotations.Add (rotation);
            }
        } else if (reactMove) {
            ReactMovement ();
        }
        // Debug.Log (Input.gyro.attitude);

        // referenceCube.transform.rotation = startRotation * Quaternion.Inverse (arCamera.transform.rotation);
        // referenceCube.transform.rotation = arCamera.transform.rotation;
        // referenceCube.transform.rotation = GyroToUnity(Input.gyro.attitude);
    }

    private Quaternion GyroToUnity (Quaternion q) {
        return new Quaternion (q.x, q.y, -q.z, -q.w);
    }

    private void DefinePressed () {
        drawReactionButton.gameObject.SetActive (true);
        finishReactionButton.gameObject.SetActive (true);
    }

    private void DrawPressed () {
        StartLine ();
    }

    private void FinishPressed () {
        StopLine ();
        drawReactionButton.gameObject.SetActive (false);
        finishReactionButton.gameObject.SetActive (false);
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

    private void StartLine () {
        lineRenderer = Instantiate (line, arCamera.transform.position, Quaternion.identity).GetComponent<LineRenderer> ();
        SetLineSettings (lineRenderer);
        lineRenderer.SetPosition (0, arCamera.transform.position);
        lineRenderer.SetPosition (1, arCamera.transform.position);
        drawing = true;
        startRotation = arCamera.transform.rotation;
        startCubeRotation = referenceCube.transform.rotation;
    }

    private void StopLine () {
        index = 2;
        int numVertices = lineRenderer.positionCount;
        // lineRenderer = null;
        drawing = false;
        lineRenderer.gameObject.SetActive (false);
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
        GameObject[] lines = GetAllLinesInScene ();
        foreach (GameObject currentLine in lines) {
            DestroyImmediate (currentLine);
        }
    }

    public void PerformReact () {
        reactMove = true;
        positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions (positions);
    }

    public void ReactMovement () {
        if (lineRenderer != null) {
            Vector3 targetPosition = positions[i];
            if (referenceCube.transform.position == targetPosition) {
                i += 1;
                if (i == lineRenderer.positionCount) {
                    reactMove = false;
                    return;
                }
                targetPosition = positions[i];
            }
            float step = movementSpeed * Time.deltaTime;
            if (i > 2) {
                Vector3 previousPoint = positions[i - 1];
                float previousTime = times[i - 3];

                float targetTime = times[i - 2];
                float distance = Vector3.Distance (targetPosition, previousPoint);
                float timeDifference = targetTime - previousTime;

                float speed = distance / timeDifference;
                float newStep = speed * Time.deltaTime;
                referenceCube.transform.position = Vector3.MoveTowards (referenceCube.transform.position, targetPosition, newStep);

                Quaternion targetRotation = rotations[i - 2];
                float rotStep = 100f * Time.deltaTime;
                referenceCube.transform.rotation = Quaternion.RotateTowards (referenceCube.transform.rotation, targetRotation, rotStep);
            } else {

                referenceCube.transform.position = Vector3.MoveTowards (referenceCube.transform.position, targetPosition, step);
            }
        }
    }

    private void ResetPressed () {
        ClearLines ();
        positions = new Vector3[0];
        times = new List<float> ();
        rotations = new List<Quaternion> ();
        reactMove = false;
        i = 0;
    }
}