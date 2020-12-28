using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RotateScript : MonoBehaviour {
    public SelectObjectsScript selectObjectsScript;
    public Button rotateButton;
    public Camera arCamera;
    public GameObject line1;
    public GameObject line2;
    public GameObject line3;
    public GameObject colliderObject;
    public GameObject audioPlayer;
    public GameObject parentHolder;
    public bool rotateOn = false;

    public Material grayLineMaterial;
    public Material redLineMaterial;
    public Material blueLineMaterial;
    public Material greenLineMaterial;
    public Material grayMaterial;
    public Material blueMaterial;
    public Material yellowMaterial;
    public Material transparentBlue;
    public Material transparentYellow;

    private float lineWidth = 0.005f;
    private int cornerVertices = 5;
    private int endCapVertices = 5;
    private LineRenderer lineRenderer1;
    private LineRenderer lineRenderer2;
    private LineRenderer lineRenderer3;

    private Quaternion relative;
    private Quaternion relativeAngle;
    private List<Quaternion> relativeAngles = new List<Quaternion> ();

    private bool colliderTouched = false;
    private GameObject colliderTouchedObj;

    void Start () {
        rotateButton.onClick.AddListener (PressedRotate);
    }

    void Update () {
        if (rotateOn) {
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
                    Handheld.Vibrate ();
                    audioPlayer.GetComponent<AudioSource> ().Play ();
                    HandleLineChanges (colliderTouchedObj);
                }
            } else {
                colliderTouched = false;
            }

        }

    }

    private void HandleLineChanges (GameObject colliderTouchedObj) {
        Vector3 axis = colliderTouchedObj.GetComponent<LineRenderSphereScript> ().axis;
        LineRenderer currentLineRenderer;
        if (axis == Vector3.right) {
            currentLineRenderer = lineRenderer1;
            lineRenderer2.material = grayLineMaterial;
            lineRenderer3.material = grayLineMaterial;
        } else if (axis == Vector3.up) {
            currentLineRenderer = lineRenderer2;
            lineRenderer1.material = grayLineMaterial;
            lineRenderer3.material = grayLineMaterial;
        } else {
            currentLineRenderer = lineRenderer3;
            lineRenderer1.material = grayLineMaterial;
            lineRenderer2.material = grayLineMaterial;
        }
        currentLineRenderer.startWidth = .01f;
        currentLineRenderer.endWidth = .01f;
    }

    private void ResetLines () {
        ResetLineOption (lineRenderer1, redLineMaterial);
        ResetLineOption (lineRenderer2, greenLineMaterial);
        ResetLineOption (lineRenderer3, blueLineMaterial);
    }

    private void ResetLineOption (LineRenderer lineRenderer, Material material) {
        lineRenderer.material = material;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }

    private void CalculateRelativeAngle () {
        relativeAngles = new List<Quaternion> ();
        Vector3 axis = colliderTouchedObj.GetComponent<LineRenderSphereScript> ().axis;
        GameObject selected = parentHolder;
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
        relativeAngles.Add (Quaternion.Inverse (newRotation) * initialRotation);
        relativeAngle = Quaternion.Inverse (newRotation) * initialRotation;
    }

    private void HandleRotate () {
#if UNITY_EDITOR
        bool released = Input.GetMouseButtonUp (0);
#else
        bool released = Input.touchCount == 0;
#endif
        if (released) {
            ResetLines ();
            colliderTouched = false;
            return;
        }
        Vector3 axis = colliderTouchedObj.GetComponent<LineRenderSphereScript> ().axis;
        GameObject selected = parentHolder;
        Vector3 direction = arCamera.transform.position - parentHolder.transform.position;

        if (axis == Vector3.up) {
            direction.y = 0;
        } else if (axis == Vector3.right) {
            direction.x = 0;
        } else if (axis == Vector3.forward) {
            direction.z = 0;
        }
        var newRotation = Quaternion.LookRotation (direction) * Quaternion.Euler (0, 0, 0);
        var finalRotation = newRotation * relativeAngle;
        parentHolder.transform.rotation = Quaternion.Slerp (parentHolder.transform.rotation, finalRotation, Time.deltaTime * 5f);

    }

    private void PressedRotate () {
        rotateOn = !rotateOn;
        if (rotateOn) {
            DrawLines ();
            TransparencyHandler (true);
        } else {
            EraseLines ();
            TransparencyHandler (false);
        }
    }

    private void TransparencyHandler (bool isTransparent) {
        foreach (Transform objTransform in parentHolder.transform) {
            GameObject selectedObj = objTransform.gameObject;
            if (isTransparent) {
                TransparencyHelper (selectedObj);
            } else {
                OpaqueHelper (selectedObj);
            }
            foreach (Transform childTransform in selectedObj.transform) {
                GameObject child = childTransform.gameObject;
                if (child.name == "Cylinder" || child.name == "Cube(Clone)" || child.name == "Sphere(Clone)") {
                    if (isTransparent) {
                        TransparencyHelper (child);
                    } else {
                        OpaqueHelper (child);
                    }
                }
            }
        }
    }

    private void TransparencyHelper (GameObject obj) {
        Material mat = obj.GetComponent<Renderer> ().sharedMaterial;
        if (mat.name == "BlueMaterial") {
            obj.GetComponent<Renderer> ().sharedMaterial = transparentBlue;
        } else if (mat.name == "YellowMaterial") {
            obj.GetComponent<Renderer> ().sharedMaterial = transparentYellow;
        }
    }

    private void OpaqueHelper (GameObject obj) {
        Material mat = obj.GetComponent<Renderer> ().sharedMaterial;
        if (mat.name == "TransparentBlue") {
            obj.GetComponent<Renderer> ().sharedMaterial = blueMaterial;
        } else if (mat.name == "TransparentYellow") {
            obj.GetComponent<Renderer> ().sharedMaterial = yellowMaterial;
        }
    }

    private void DrawLines () {
        float radius = .1f;
        var segments = 360;
        var pointCount = segments + 1;
        var points1 = new Vector3[pointCount];
        var points2 = new Vector3[pointCount];
        var points3 = new Vector3[pointCount];

        Vector3 startPoint = parentHolder.transform.position;

        for (int i = 0; i < pointCount; i++) {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points1[i] = startPoint + new Vector3 (0, Mathf.Sin (rad) * radius, Mathf.Cos (rad) * radius);
            points2[i] = startPoint + new Vector3 (Mathf.Sin (rad) * radius, 0, Mathf.Cos (rad) * radius);
            points3[i] = startPoint + new Vector3 (Mathf.Sin (rad) * radius, Mathf.Cos (rad) * radius, 0);
        }

        GameObject parentObj = Instantiate (colliderObject, line1.transform.position, Quaternion.identity);

        lineRenderer1 = Instantiate (line1, parentHolder.transform.position, Quaternion.identity).GetComponent<LineRenderer> ();
        SetLineSettings (lineRenderer1);
        lineRenderer1.positionCount = segments + 1;
        lineRenderer1.SetPositions (points1);
        SetEmptyColliders (points1, parentObj, Vector3.right);

        lineRenderer2 = Instantiate (line2, parentHolder.transform.position, Quaternion.identity).GetComponent<LineRenderer> ();
        SetLineSettings (lineRenderer2);
        lineRenderer2.positionCount = segments + 1;
        lineRenderer2.SetPositions (points2);
        SetEmptyColliders (points2, parentObj, Vector3.up);

        lineRenderer3 = Instantiate (line3, parentHolder.transform.position, Quaternion.identity).GetComponent<LineRenderer> ();
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