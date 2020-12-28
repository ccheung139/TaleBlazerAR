using System.Collections;
using System.Collections.Generic;
using cakeslice;
using UnityEngine;
using UnityEngine.UI;

public class CopyPasteScript : MonoBehaviour {
    public SelectObjectsScript selectObjectsScript;
    public SetGovernScript setGovernScript;
    public Button copyButton;
    public Button pasteButton;
    public Camera arCamera;
    public GameObject parentHolder;

    private List<GameObject> copiedObjects = new List<GameObject> ();
    private List<GameObject> seenObjectsMultiple = new List<GameObject> ();

    void Start () {
        copyButton.onClick.AddListener (CopyPressed);
        pasteButton.onClick.AddListener (PastePressed);
    }

    private void CopyPressed () {
        ResetCopied ();
        foreach (Transform objTransform in parentHolder.transform) {
            GameObject selectedObj = objTransform.gameObject;
            GameObject duplicate = Instantiate (selectedObj, selectedObj.transform.position, selectedObj.transform.rotation, arCamera.transform);
            duplicate.name = selectedObj.name;
            duplicate.SetActive (false);
            copiedObjects.Add (duplicate);
        }
    }

    private void PastePressed () {
        foreach (GameObject obj in copiedObjects) {
            obj.SetActive (true);
            obj.transform.parent = null;
            obj.GetComponent<cakeslice.Outline> ().enabled = false;
        }
        DFSMultipleFindAttached ();
        DisassembleChildren ();
    }

    private void DisassembleChildren () {
        foreach (GameObject obj in copiedObjects) {
            List<GameObject> objectsToDissasemble = new List<GameObject> ();
            foreach (Transform child in obj.transform) {
                if (child.gameObject.name == "Cube(Clone)" ||
                    child.gameObject.name == "Sphere(Clone)" || child.gameObject.name == "Cylinder") {
                    objectsToDissasemble.Add (child.gameObject);
                }
            }
            foreach (GameObject objToDissasemble in objectsToDissasemble) {
                objToDissasemble.transform.parent = null;
                objToDissasemble.GetComponent<cakeslice.Outline> ().enabled = false;
            }
            obj.GetComponent<cakeslice.Outline> ().enabled = false;
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