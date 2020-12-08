using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopyPasteScript : MonoBehaviour {
    public SelectObjectsScript selectObjectsScript;
    public SetGovernScript setGovernScript;
    public Button copyButton;
    public Button pasteButton;
    public Camera arCamera;
    public Material grayMaterial;

    private List<GameObject> copiedObjects = new List<GameObject> ();
    private List<GameObject> seenObjectsMultiple = new List<GameObject> ();

    void Start () {
        copyButton.onClick.AddListener (CopyPressed);
        pasteButton.onClick.AddListener (PastePressed);
    }

    private void CopyPressed () {
        ResetCopied ();
        for (int i = 0; i < selectObjectsScript.selectedShapes.Count; i++) {
            GameObject selectedObj = selectObjectsScript.selectedShapes[i];
            if (selectedObj.name == "Cylinder") {
                GameObject parentObj = selectedObj.transform.parent.gameObject;
                GameObject duplicate = Instantiate (parentObj, parentObj.transform.position, parentObj.transform.rotation, arCamera.transform);
                duplicate.name = parentObj.name;
                duplicate.SetActive (false);
                copiedObjects.Add (duplicate.transform.Find ("Cylinder").gameObject);
            } else {
                GameObject duplicate = Instantiate (selectedObj, selectedObj.transform.position, selectedObj.transform.rotation, arCamera.transform);
                duplicate.name = selectedObj.name;
                duplicate.SetActive (false);
                copiedObjects.Add (duplicate);
            }

        }
    }

    private void PastePressed () {
        foreach (GameObject obj in copiedObjects) {
            if (obj.name == "Cylinder") {
                obj.transform.parent.gameObject.SetActive (true);
                obj.transform.parent.parent = null;
            } else {
                obj.SetActive (true);
                obj.transform.parent = null;
            }

        }
        DFSMultipleFindAttached ();
        DisassembleChildren ();
    }

    private void DisassembleChildren () {
        foreach (GameObject obj in copiedObjects) {
            if (obj.name == "Cylinder") {
                foreach (Transform child in obj.transform.parent) {
                    if (child.gameObject.name.Contains ("Cube") ||
                        child.gameObject.name.Contains ("Sphere")) {
                        child.gameObject.transform.parent = null;
                        child.gameObject.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
                    } else if (child.gameObject.name.Contains ("CylinderParent")) {
                        child.gameObject.transform.parent = null;
                        child.Find ("Cylinder").gameObject.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
                    }
                }
                obj.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
            } else {
                foreach (Transform child in obj.transform) {
                    if (child.gameObject.name.Contains ("Cube") ||
                        child.gameObject.name.Contains ("Sphere")) {
                        child.gameObject.transform.parent = null;
                        child.gameObject.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
                    } else if (child.gameObject.name.Contains ("CylinderParent")) {
                        child.gameObject.transform.parent = null;
                        child.Find ("Cylinder").gameObject.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
                    }
                }
                obj.GetComponent<Renderer> ().sharedMaterial = grayMaterial;
            }

        }
    }

    private void ResetCopied () {
        List<GameObject> copiedObjects = new List<GameObject> ();
        List<Vector3> copiedOffsets = new List<Vector3> ();
        List<Quaternion> copiedRotations = new List<Quaternion> ();
    }

    private void DFSMultipleFindAttached () {
        List<List<GameObject>> newSets = new List<List<GameObject>> ();
        foreach (GameObject copiedObj in copiedObjects) {
            List<GameObject> newSet = new List<GameObject> ();
            newSet = DFSMultipleHelper (copiedObj, newSet);
            newSets.Add (newSet);
        }
        foreach (List<GameObject> foundSet in newSets) {
            setGovernScript.shapeSets.Add (foundSet);
        }
        seenObjectsMultiple = new List<GameObject> ();
    }

    private List<GameObject> DFSMultipleHelper (GameObject copiedObj, List<GameObject> newSet) {
        if (copiedObj == null || seenObjectsMultiple.Contains (copiedObj) || CheckPlacer (copiedObj)) {
            return newSet;
        }
        seenObjectsMultiple.Add (copiedObj);
        newSet.Add (copiedObj);
        if (copiedObj.name == "Cylinder") {
            CylinderColliderScript ccs = copiedObj.GetComponent<CylinderColliderScript> ();
            foreach (GameObject newObj in ccs.collidingObjects) {
                newSet = DFSMultipleHelper (newObj, newSet);
            }
        } else {
            ColliderScript cs = copiedObj.GetComponent<ColliderScript> ();
            foreach (GameObject newObj in cs.collidingObjects) {
                newSet = DFSMultipleHelper (newObj, newSet);
            }
        }
        return newSet;
    }

    private bool CheckPlacer (GameObject obj) {
        return obj.name.Contains ("Placed");
    }
}