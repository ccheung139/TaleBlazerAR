using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditScript : MonoBehaviour {
    public SelectObjectsScript selectObjectsScript;
    public Button editButton;
    public Button dragButton;
    public Camera arCamera;
    public GameObject line1;
    public GameObject line2;
    public GameObject line3;
    public GameObject colliderObject;
    public bool editOn = false;

    private float lineWidth = 0.005f;
    private int cornerVertices = 5;
    private int endCapVertices = 5;
    private LineRenderer lineRenderer1;
    private LineRenderer lineRenderer2;
    private LineRenderer lineRenderer3;

    private Vector3 intialDirection;
    private Quaternion relative;

    private bool colliderTouched = false;
    private GameObject colliderTouchedObj;

    void Start () {
        editButton.onClick.AddListener (PressedEdit);
    }

    void Update () {
        if (editOn) {
            if (colliderTouched) {
                HandleRotate ();
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
                    CalculateRelativeAngle ();
                }
            } else {
                colliderTouched = false;
            }

        }

    }

    private void CalculateRelativeAngle () {
        Vector3 axis = colliderTouchedObj.GetComponent<LineRenderSphereScript> ().axis;
        GameObject selected = selectObjectsScript.selectedShape;
        Quaternion initialRotation = selected.transform.rotation;
        Vector3 direction = arCamera.transform.position - selected.transform.position;
        if (axis == Vector3.up) {
            direction.y = 0;
        } else if (axis == Vector3.right) {
            direction.x = 0;
        } else if (axis == Vector3.forward) {
            direction.z = 0;
        }
        var newRotation = Quaternion.LookRotation (direction) * Quaternion.Euler (0, 0, 0);
        relative = Quaternion.Inverse (newRotation) * initialRotation;
    }

    private void HandleRotate () {
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
        GameObject selected = selectObjectsScript.selectedShape;
        Vector3 selectedPosition = selected.transform.position;
        Vector3 direction = arCamera.transform.position - selected.transform.position;

        if (axis == Vector3.up) {
            direction.y = 0;
        } else if (axis == Vector3.right) {
            direction.x = 0;
        } else if (axis == Vector3.forward) {
            direction.z = 0;
        }

        var newRotation = Quaternion.LookRotation (direction) * Quaternion.Euler (0, 0, 0);
        var finalRotation = newRotation * relative;
        selected.transform.rotation = Quaternion.Slerp (selected.transform.rotation, finalRotation, Time.deltaTime * 1f);
    }

    private void PressedEdit () {
        editOn = !editOn;
        if (editOn) {
            DrawLines ();
        } else {
            EraseLines ();
        }
    }

    private void DrawLines () {
        GameObject selected = selectObjectsScript.selectedShape;
        List<GameObject> selectedObjects = selectObjectsScript.finalSelected;

        float radius = .1f;
        var segments = 360;
        var pointCount = segments + 1;
        var points1 = new Vector3[pointCount];
        var points2 = new Vector3[pointCount];
        var points3 = new Vector3[pointCount];

        Vector3 startPoint = selected.transform.position;

        for (int i = 0; i < pointCount; i++) {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points1[i] = startPoint + new Vector3 (0, Mathf.Sin (rad) * radius, Mathf.Cos (rad) * radius);
            points2[i] = startPoint + new Vector3 (Mathf.Sin (rad) * radius, 0, Mathf.Cos (rad) * radius);
            points3[i] = startPoint + new Vector3 (Mathf.Sin (rad) * radius, Mathf.Cos (rad) * radius, 0);
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

// private float CalculateTheta (Vector2 position2, Vector2 center, float radius = 0.1f) {
//     float numerator = Vector2.Dot (position2, center);
//     float denominator = position2.magnitude * center.magnitude;
//     float inside = numerator / denominator;
//     if (inside > 1.0f) {
//         inside = 1.0f;
//     } else if (inside < -1.0f) {
//         inside = -1.0f;
//     }
//     float radians = Mathf.Acos (inside);
//     float theta = radians * 180 / Mathf.PI;
//     Debug.Log ("theta: " + theta);
//     return theta;
//     // return radius * theta;
// }

// if (axis == Vector3.up) {
//     // Vector2 point1 = new Vector2 (lastPosition.x, lastPosition.z);
//     Vector2 point2 = new Vector2 (thisPosition.x, thisPosition.z);
//     Vector2 center = new Vector2 (initialPosition.x, initialPosition.z);
//     newTheta = CalculateTheta (point2, center);

// } else if (axis == Vector3.right) {
//     // Vector2 point1 = new Vector2 (lastPosition.y, lastPosition.z);
//     Vector2 point2 = new Vector2 (thisPosition.y, thisPosition.z);
//     Vector2 center = new Vector2 (initialPosition.y, initialPosition.z);
//     newTheta = CalculateTheta (point2, center);

// } else {
//     // Vector2 point1 = new Vector2 (lastPosition.x, lastPosition.y);
//     Vector2 point2 = new Vector2 (thisPosition.x, thisPosition.y);
//     Vector2 center = new Vector2 (initialPosition.x, initialPosition.y);
//     newTheta = CalculateTheta (point2, center);
// }
// if (Single.IsNaN (newTheta)) {
//     return;
// }

// delta = lastTheta - newTheta;
// lastTheta = newTheta;

// lastPosition = thisPosition;
// if (selected.name == "Cylinder") {
//     GameObject parent = selected.transform.parent.gameObject;
//     Transform topOfCylinder = selected.transform.Find ("TopOfCylinder");
//     parent.transform.RotateAround (topOfCylinder.position, axis, delta);

// } else {
//     selected.transform.RotateAround (selected.transform.position, axis, delta);
// }

// private void HandleRotateDrag () {
//     if (isMouseDragging && colliderTouched) {
//         float delta;
//         Vector3 axis = colliderTouchedObj.GetComponent<LineRenderSphereScript> ().axis;
//         if (axis == Vector3.up) {
//             delta = -(currentX - Input.mousePosition.x) * 20000f;
//             currentX = Input.mousePosition.x;
//         } else if (axis == Vector3.forward) {

//             delta = (currentY - Input.mousePosition.y) * 20000f;
//         } else {
//             delta = (currentY - Input.mousePosition.y) * 20000f;
//             currentY = Input.mousePosition.y;
//         }

//         GameObject selected = selectObjectsScript.selectedShape;

//         if (selected.name == "Cylinder") {
//             GameObject parent = selected.transform.parent.gameObject;
//             Transform topOfCylinder = selected.transform.Find ("TopOfCylinder");
//             parent.transform.RotateAround (topOfCylinder.position, axis, delta * 360);

//         } else {
//             selected.transform.RotateAround (selected.transform.position, axis, delta * 360);
//         }

//     }
// }

// private void HandleClosestPoint () {
//     if (!touchingLine) {
//         CalculateClosestLinePoint ();
//     }
// }

// private void CalculateClosestLinePoint () {
//     ClosestHelper (xPoints);
//     ClosestHelper (yPoints);
//     ClosestHelper (zPoints);
//     if (setMinDistance <= 0.1f) {
//         dragButton.gameObject.SetActive (true);
//         touchingLine = true;
//     }
// }

// private void ClosestHelper (Vector3[] points) {
//     foreach (Vector3 point in points) {
//         float distance = Vector3.Distance (point, arCamera.transform.position);
//         if (distance < setMinDistance) {
//             setMinDistance = distance;
//             setMinPosition = point;
//         }
//     }
// }

// private float CalculateTheta (Vector2 position1, Vector2 position2, float radius = 0.1f) {
//     float d = Vector2.Distance (position1, position2);
//     float inside = 1 - (d * d) / (2 * radius * radius);
//     Debug.Log ("inside: " + inside);
//     float radians = Mathf.Acos (inside);
//     float theta = radians * 180 / Mathf.PI;
//     Debug.Log ("theta: " + theta);
//     return theta;
// }

// private float CalculateArcLength (Vector2 position1, Vector2 position2, Vector2 center, float radius = 0.1f) {
//     float theta1 = ArcLengthHelper (position1, radius);
//     float theta2 = ArcLengthHelper (position2, radius);
//     float thetaDiff = theta1 - theta2;
//     return radius * thetaDiff;
// }

// private float ArcLengthHelper (Vector2 position, float radius = 0.1f) {
//     // float theta = Mathf.Acos (position.x / radius);
//     float z = Mathf.Sqrt (position.x * position.x + position.y * position.y);
//     float d = Vector2.Distance (position, new Vector2 (z, 0));
//     // Debug.Log (d);
//     float inside = 1 - (d * d) / (2 * radius * radius);
//     float theta = Mathf.Acos (inside);
//     return theta;
// }