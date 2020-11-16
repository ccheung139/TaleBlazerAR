using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveScript : MonoBehaviour {
    public Button moveButton;
    public Button finishButton;
    public Camera arCamera;
    public PlacementScript ps;
    public SetGovernScript setGovernScript;
    public SelectObjectsScript selectObjectsScript;
    public CancelScript cancelScript;

    private Transform cylinderParent;
    private bool isMoving = false;
    private List<GameObject> seenObjects = new List<GameObject> ();

    void Start () {
        moveButton.onClick.AddListener (PressedMove);
        finishButton.onClick.AddListener (PressedFinish);
    }

    private void PressedMove () {
        GameObject selected = selectObjectsScript.selectedShape;
        List<GameObject> selectedObjects = selectObjectsScript.finalSelected;
        Debug.Log (selected);
        Debug.Log (selectedObjects.Count);
        if (selected != null && selectedObjects.Count == 0) {
            MoveOneObject (selected);
        } else if (selected == null && selectedObjects.Count != 0) {
            MoveMultipleObjects (selectedObjects);
        } else {
            return;
        }
        StartMovements ();
    }

    private void MoveOneObject (GameObject selected) {
        if (selected.name == "Cylinder") {
            cylinderParent = selected.transform.parent;
            selected.transform.parent.parent = arCamera.transform;
        } else {
            selected.transform.parent = arCamera.transform;
        }
    }

    private void MoveMultipleObjects (List<GameObject> selectedObjects) {
        foreach (GameObject obj in selectedObjects) {
            MoveOneObject (obj);
        }
    }

    private void StartMovements () {
        isMoving = true;
        finishButton.gameObject.SetActive (true);
        seenObjects = new List<GameObject> ();
    }

    private void PressedFinish () {
        if (!isMoving) {
            return;
        }
        GameObject selected = selectObjectsScript.selectedShape;
        List<GameObject> selectedObjects = selectObjectsScript.finalSelected;
        Debug.Log (selectedObjects);
        if (selected == null && selectedObjects.Count == 0) {
            isMoving = false;
            return;
        } else if (selected != null && selectedObjects.Count == 0) {
            FinishMovementOneObject (selected);
            DFSFindAttachedObjects (selected);
        } else if (selected == null && selectedObjects.Count != 0) {
            FinishMovementMultiple (selectedObjects);
        }
        isMoving = false;
        finishButton.gameObject.SetActive (false);
        cancelScript.DeselectAssociatedShapes ();
        cancelScript.DisableAllPlacers (true);
    }

    private void FinishMovementOneObject (GameObject selected) {
        if (selected.name == "Cylinder") {
            selected.transform.parent = cylinderParent;
            selected.transform.parent.parent = null;
        } else {
            selected.transform.parent = null;
        }
    }

    private void FinishMovementMultiple (List<GameObject> multipleSelected) {
        foreach (GameObject selected in multipleSelected) {
            FinishMovementOneObject (selected);
        }
        selectObjectsScript.DeselectGroup ();
    }

    private void DFSFindAttachedObjects (GameObject selected) {
        DFSHelper (selected);
        int foundCount = seenObjects.Count;
        int setCount = FindSetCount (selected);
        if (foundCount != setCount) {
            DetachSets (selected, seenObjects);
        }
        seenObjects = new List<GameObject> ();
    }

    private void DFSHelper (GameObject nextObj) {
        if (nextObj == null || seenObjects.Contains (nextObj) || CheckPlacer (nextObj)) {
            return;
        }
        seenObjects.Add (nextObj);
        if (nextObj.name == "Cylinder") {
            CylinderColliderScript ccs = nextObj.GetComponent<CylinderColliderScript> ();
            foreach (GameObject newObj in ccs.collidingObjects) {
                DFSHelper (newObj);
            }
        } else {
            ColliderScript cs = nextObj.GetComponent<ColliderScript> ();
            foreach (GameObject newObj in cs.collidingObjects) {
                DFSHelper (newObj);
            }
        }
    }

    private bool CheckPlacer (GameObject obj) {
        return obj.name.Contains ("Placed");
    }

    public int FindSetCount (GameObject selected) {
        foreach (List<GameObject> potentialList in setGovernScript.shapeSets) {
            if (potentialList.Contains (selected)) {
                return potentialList.Count;
            }
        }
        return 0;
    }

    public void DetachSets (GameObject selected, List<GameObject> newSelectedList) {
        foreach (List<GameObject> potentialList in setGovernScript.shapeSets) {
            if (potentialList.Contains (selected)) {
                List<GameObject> newList = new List<GameObject> ();
                foreach (GameObject obj in potentialList) {
                    if (!newSelectedList.Contains (obj)) {
                        newList.Add (obj);
                    }
                }
                setGovernScript.shapeSets.Remove (potentialList);
                setGovernScript.shapeSets.Add (newList);
                setGovernScript.shapeSets.Add (newSelectedList);
                return;
            }
        }
    }
}