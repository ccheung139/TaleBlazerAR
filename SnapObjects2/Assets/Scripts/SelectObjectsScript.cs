using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectObjectsScript : MonoBehaviour {
    public PlacementScript ps;

    public GameObject selectedShape;
    public GameObject cubePlacer;
    public GameObject spherePlacer;
    public GameObject cylinderPlacer;
    public Material blueMaterial;
    public Material grayMaterial;

    public List<GameObject> selectedShapes = new List<GameObject> ();
    private List<GameObject> previousSelected = new List<GameObject> ();
    private Vector3 previousCenter;
    private Vector3 previousScale;

    public void SelectShape (GameObject obj) {
        if (cubePlacer.activeSelf || spherePlacer.activeSelf || cylinderPlacer.activeSelf || ps.preliminaryObject != null) {
            return;
        }
        DeselectShapes ();

        Renderer renderer = obj.GetComponent<Renderer> ();
        if (obj.GetComponent<Renderer> ().sharedMaterial == grayMaterial) {
            obj.GetComponent<Renderer> ().sharedMaterial = blueMaterial;
            selectedShapes.Add (obj);
            // selectedShape = obj;
        } else {
            obj.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
            selectedShapes = new List<GameObject> ();
            // selectedShape = null;
        }
    }

    public void DeselectShapes () {
        foreach (GameObject shape in selectedShapes) {
            shape.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
        }
        Reset ();
    }

    public void SelectInBox (Vector3 center, Vector3 scale, bool isFinished) {
        if (previousCenter == center && previousScale == scale && !isFinished) {
            return;
        }
        selectedShapes = new List<GameObject> ();
        previousCenter = center;
        previousScale = scale;

        float maxZDistance = scale.z;
        Collider[] hits = Physics.OverlapBox (center, scale);
        List<GameObject> newSelected = new List<GameObject> ();
        foreach (Collider hit in hits) {
            if (!hit.name.Contains ("LineCube") && !hit.name.Contains ("SphereLine(Clone)")) {
                newSelected.Add (hit.GetComponent<Collider> ().gameObject);
            }
        }
        CheckRemoved (newSelected);
        SelectGroup (newSelected);
        previousSelected = newSelected;

        if (isFinished) {
            selectedShapes = newSelected;
        }
    }

    private void SelectGroup (List<GameObject> newSelected) {
        foreach (GameObject newSelect in newSelected) {
            newSelect.GetComponent<Renderer> ().sharedMaterial = blueMaterial;
        }
    }

    public void DeselectGroup () {
        foreach (GameObject previousSelect in previousSelected) {
            previousSelect.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
        }
        previousSelected = new List<GameObject> ();
        selectedShapes = new List<GameObject> ();
    }

    private void CheckRemoved (List<GameObject> newSelected) {
        foreach (GameObject previousSelect in previousSelected) {
            if (!newSelected.Contains (previousSelect)) {
                previousSelect.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
            }
        }
    }

    public void Reset () {
        previousSelected = new List<GameObject> ();
        selectedShapes = new List<GameObject> ();
    }
}