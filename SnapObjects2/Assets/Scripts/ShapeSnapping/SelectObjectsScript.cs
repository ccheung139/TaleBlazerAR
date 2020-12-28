using System.Collections;
using System.Collections.Generic;
using cakeslice;
using UnityEngine;

public class SelectObjectsScript : MonoBehaviour {
    public PlacementScript ps;
    public ButtonScript buttonScript;

    public GameObject selectedShape;
    public GameObject cubePlacer;
    public GameObject spherePlacer;
    public GameObject cylinderPlacer;
    public GameObject parentHolder;

    public List<GameObject> selectedShapes = new List<GameObject> ();
    private List<GameObject> previousSelected = new List<GameObject> ();
    private Vector3 previousCenter;
    private Vector3 previousScale;

    public void SelectShape (GameObject obj) {

        if (cubePlacer.activeSelf || spherePlacer.activeSelf || cylinderPlacer.activeSelf || ps.preliminaryObject != null) {
            return;
        }
        DeselectShapes ();

        Outline outline = obj.GetComponent<Outline> ();
        if (!outline.enabled) {
            outline.enabled = true;
            selectedShapes.Add (obj);
        } else {
            outline.enabled = false;
            selectedShapes = new List<GameObject> ();
        }
        buttonScript.HideNeutralChoices ();
        buttonScript.ShowSelectChoices ();
        ParentSelected ();
    }

    public void DeselectShapes () {
        foreach (GameObject shape in selectedShapes) {
            shape.GetComponent<Outline> ().enabled = false;
            shape.transform.parent = null;
        }
        Reset ();
        buttonScript.CancelPressed ();
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
        ParentSelected ();
    }

    private void SelectGroup (List<GameObject> newSelected) {
        foreach (GameObject newSelect in newSelected) {
            newSelect.GetComponent<Outline> ().enabled = true;
        }
    }

    public void DeselectGroup () {
        foreach (GameObject previousSelect in previousSelected) {
            previousSelect.GetComponent<Outline> ().enabled = false;
        }
        previousSelected = new List<GameObject> ();
        selectedShapes = new List<GameObject> ();
        buttonScript.CancelPressed ();
    }

    private void CheckRemoved (List<GameObject> newSelected) {
        foreach (GameObject previousSelect in previousSelected) {
            if (!newSelected.Contains (previousSelect)) {
                previousSelect.GetComponent<Outline> ().enabled = false;
            }
        }
    }

    public void Reset () {
        previousSelected = new List<GameObject> ();
        selectedShapes = new List<GameObject> ();
    }

    private void ParentSelected () {
        if (selectedShapes.Count > 0) {
            parentHolder.transform.localScale = new Vector3 (1, 1, 1);
            parentHolder.transform.position = selectedShapes[0].transform.position;
            parentHolder.transform.rotation = Quaternion.identity;
            foreach (GameObject obj in selectedShapes) {
                obj.transform.parent = parentHolder.transform;
            }
        }
    }
}