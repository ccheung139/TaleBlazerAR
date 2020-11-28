using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScaleScript : MonoBehaviour {
    public SelectObjectsScript selectObjectsScript;
    public Button scaleButton;
    public Camera arCamera;
    public GameObject line1;
    public GameObject line2;
    public GameObject line3;
    public GameObject colliderObject;
    public bool scaleOn = false;

    private float lineWidth = 0.005f;
    private int cornerVertices = 5;
    private int endCapVertices = 5;
    private LineRenderer lineRenderer1;
    private LineRenderer lineRenderer2;
    private LineRenderer lineRenderer3;

    private float previousValue = 0;

    private bool colliderTouched = false;
    private GameObject colliderTouchedObj;

    void Start () {
        scaleButton.onClick.AddListener (PressedScale);
    }

    void Update () {
        if (scaleOn) {
            if (colliderTouched) {
                HandleScale ();
            } else {
                HandleTouch ();
            }
        }
    }

    private void HandleTouch () {
#if UNITY_EDITOR
        bool touching = Input.GetMouseButtonDown (0);
#else
        bool touching = Input.touchCount > 0;
#endif

        if (touching) {
            Ray ray = arCamera.ScreenPointToRay (Input.mousePosition);
            RaycastHit hitObject;
            if (Physics.Raycast (ray, out hitObject)) {
                GameObject obj = hitObject.transform.gameObject;
                if (obj.name == "SphereLine(Clone)") {
                    colliderTouchedObj = obj;
                    colliderTouched = true;
                    CalculateStartingPosition ();
                }
            } else {
                colliderTouched = false;
            }

        }

    }

    private void CalculateStartingPosition () {
        Vector3 axis = colliderTouchedObj.GetComponent<LineRenderSphereScript> ().axis;
        previousValue = 0;
        if (axis == Vector3.up) {
            previousValue = arCamera.transform.position.y;
        } else if (axis == Vector3.right) {
            previousValue = arCamera.transform.position.x;
        } else if (axis == Vector3.forward) {
            previousValue = arCamera.transform.position.z;
        }
    }

    private void HandleScale () {
#if UNITY_EDITOR
        bool released = Input.GetMouseButtonUp (0);
#else
        bool released = Input.touchCount == 0;
#endif
        if (released) {
            colliderTouched = false;
            return;
        }
        Vector3 axis = colliderTouchedObj.GetComponent<LineRenderSphereScript> ().axis;
        GameObject selected = selectObjectsScript.selectedShapes[0];
        Vector3 cameraPosition = arCamera.transform.position;

        Vector3 scaleChange = new Vector3 (0, 0, 0);
        float newValue = 0;
        if (axis == Vector3.up) {
            newValue = cameraPosition.y;
            float delta = newValue - previousValue;
            scaleChange = new Vector3 (0, delta, 0);
        } else if (axis == Vector3.right) {
            newValue = cameraPosition.x;
            float delta = newValue - previousValue;
            scaleChange = new Vector3 (delta, 0, 0);
        } else {
            newValue = cameraPosition.z;
            float delta = newValue - previousValue;
            scaleChange = new Vector3 (0, 0, delta);
        }

        if (selected.name == "Cylinder") {
            GameObject parent = selected.transform.parent.gameObject;
            parent.transform.localScale += scaleChange;
        } else {
            selected.transform.localScale += scaleChange;
        }
        previousValue = newValue;
    }

    private void PressedScale () {
        scaleOn = !scaleOn;
        if (scaleOn) {
            DrawLines ();
        } else {
            EraseLines ();
        }
    }

    private void DrawLines () {
        GameObject selected = selectObjectsScript.selectedShapes[0];
        List<GameObject> selectedObjects = selectObjectsScript.finalSelected;

        // float radius = .1f;
        var segments = 35;
        var pointCount = segments + 1;
        var points1 = new Vector3[pointCount];
        var points2 = new Vector3[pointCount];
        var points3 = new Vector3[pointCount];

        Vector3 startPoint = selected.transform.position;

        points1[0] = startPoint + selected.transform.right * .005f;
        points2[0] = startPoint + selected.transform.up * .005f;
        points3[0] = startPoint + selected.transform.forward * .005f;
        for (int i = 1; i < pointCount; i++) {
            // var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points1[i] = points1[i - 1] + selected.transform.right * .005f;
            points2[i] = points2[i - 1] + selected.transform.up * .005f;
            points3[i] = points3[i - 1] + selected.transform.forward * .005f;
        }

        GameObject parentObj = Instantiate (colliderObject, line1.transform.position, Quaternion.identity);

        lineRenderer1 = Instantiate (line1, selected.transform.position, Quaternion.identity).GetComponent<LineRenderer> ();
        SetLineSettings (lineRenderer1);
        lineRenderer1.positionCount = segments + 1;
        lineRenderer1.SetPositions (points1);
        SetEmptyColliders (points1, parentObj, Vector3.right);

        lineRenderer2 = Instantiate (line2, selected.transform.position, Quaternion.identity).GetComponent<LineRenderer> ();
        SetLineSettings (lineRenderer2);
        lineRenderer2.positionCount = segments + 1;
        lineRenderer2.SetPositions (points2);
        SetEmptyColliders (points2, parentObj, Vector3.up);

        lineRenderer3 = Instantiate (line3, selected.transform.position, Quaternion.identity).GetComponent<LineRenderer> ();
        SetLineSettings (lineRenderer3);
        lineRenderer3.positionCount = segments + 1;
        lineRenderer3.SetPositions (points3);
        SetEmptyColliders (points3, parentObj, Vector3.forward);
    }

    private GameObject[] GetAllLinesInScene () {
        return GameObject.FindGameObjectsWithTag ("LineAxis");
    }

    public void EraseLines () {
        GameObject[] lines = GetAllLinesInScene ();
        foreach (GameObject currentLine in lines) {
            DestroyImmediate (currentLine);
        }
    }

    private void SetEmptyColliders (Vector3[] points, GameObject line, Vector3 axis) {
        foreach (Vector3 point in points) {
            GameObject colliderPoint = Instantiate (colliderObject, point, Quaternion.identity, line.transform);
            colliderPoint.GetComponent<LineRenderSphereScript> ().axis = axis;
        }
    }

    private void SetLineSettings (LineRenderer currentLineRenderer) {
        currentLineRenderer.startWidth = lineWidth;
        currentLineRenderer.endWidth = lineWidth;
        currentLineRenderer.numCornerVertices = cornerVertices;
        currentLineRenderer.numCapVertices = endCapVertices;
    }
}