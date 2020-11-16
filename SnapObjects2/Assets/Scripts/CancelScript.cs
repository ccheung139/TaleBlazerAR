using System.Collections;
using System.Collections.Generic;
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
    public Material blueMaterial;
    public Material grayMaterial;

    void Start () {
        cancelButton.onClick.AddListener (CancelActions);
    }

    private void CancelActions () {
        DisableAllPlacers (false);
    }

    public void DisableAllPlacers (bool preliminary = false) {
        if (selectObjectsScript.selectedShape != null) {
            selectObjectsScript.DeselectShape ();
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
        if (attachScript.attachOn) {
            if (!preliminary) {
                if (ps.preliminaryObject.name == "Cylinder") {
                    ps.preliminaryObject.transform.parent.transform.position = attachScript.previousAttachPosition;
                    ps.preliminaryObject.transform.parent.transform.rotation = attachScript.previousAttachRotation;
                } else {
                    ps.preliminaryObject.transform.position = attachScript.previousAttachPosition;
                    ps.preliminaryObject.transform.rotation = attachScript.previousAttachRotation;
                }
                DeselectAssociatedShapes ();
            }
        }
    }

    public void DeselectAssociatedShapes () {
        foreach (GameObject obj in ps.allSelects) {
            if (obj.name.Contains ("CylinderParent")) {
                obj.transform.Find ("Cylinder").GetComponent<Renderer> ().sharedMaterial = grayMaterial;
            } else {
                obj.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
            }
            obj.transform.parent = null;
        }
        ps.allSelects.Clear ();
        attachScript.attachOn = false;

        if (ps.preliminaryObject != null) {
            ps.preliminaryObject.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
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