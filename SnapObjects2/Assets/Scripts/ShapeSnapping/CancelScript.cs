using System.Collections;
using System.Collections.Generic;
using cakeslice;
using UnityEngine;
using UnityEngine.UI;

public class CancelScript : MonoBehaviour {
    public PlacementScript ps;
    public SelectObjectsScript selectObjectsScript;
    public SetGovernScript setGovernScript;
    public AttachScript attachScript;
    public Button cancelButton;
    public Button placeButton;

    public GameObject cubePlacer;
    public GameObject spherePlacer;
    public GameObject cylinderPlacer;
    public Material grayMaterial;

    void Start () {
        cancelButton.onClick.AddListener (CancelActions);
    }

    private void CancelActions () {
        DisableAllPlacers (false);
    }

    public void DisableAllPlacers (bool preliminary = false) {
        if (selectObjectsScript.selectedShapes.Count > 0) {
            selectObjectsScript.DeselectShapes ();
            DeselectAssociatedShapes ();
        }
        spherePlacer.SetActive (false);
        cubePlacer.SetActive (false);
        cylinderPlacer.SetActive (false);
        placeButton.gameObject.SetActive (false);
        if (!preliminary && ps.preliminaryObject != null && !attachScript.attachOn) {
            RemoveFromSet ();
            Destroy (ps.preliminaryObject);
            ps.preliminaryObject = null;
        }
    }

    public void DeselectAssociatedShapes () {
        foreach (GameObject obj in ps.allSelects) {
            obj.GetComponent<cakeslice.Outline> ().enabled = false;
            // obj.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
            obj.transform.parent = null;
        }
        ps.allSelects.Clear ();
        attachScript.attachOn = false;

        if (ps.preliminaryObject != null) {
            ps.preliminaryObject.GetComponent<cakeslice.Outline> ().enabled = false;
            // ps.preliminaryObject.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
            ps.preliminaryObject = null;
        }
    }

    private void RemoveFromSet () {
        foreach (List<GameObject> potentialList in setGovernScript.shapeSets) {
            if (potentialList.Contains (ps.preliminaryObject)) {
                potentialList.Remove (ps.preliminaryObject);
                return;
            }
        }
    }
}