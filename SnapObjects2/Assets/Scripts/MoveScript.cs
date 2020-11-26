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
    private List<GameObject> seenObjectsMultiple = new List<GameObject> ();

    void Start () {
        moveButton.onClick.AddListener (PressedMove);
        finishButton.onClick.AddListener (PressedFinish);
    }

    private void PressedMove () {
        GameObject selected = selectObjectsScript.selectedShape;
        List<GameObject> selectedObjects = selectObjectsScript.finalSelected;
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
        if (selected == null && selectedObjects.Count == 0) {
            isMoving = false;
            return;
        } else if (selected != null && selectedObjects.Count == 0) {
            FinishMovementOneObject (selected);
            DFSFindAttachedObjects (selected);
        } else if (selected == null && selectedObjects.Count != 0) {
            FinishMovementMultiple (selectedObjects);
            DFSMultipleFindAttached (selectedObjects);
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

    private void DFSMultipleFindAttached (List<GameObject> selectedObjects) {
        List<List<GameObject>> newSets = new List<List<GameObject>> ();
        foreach (GameObject selectedObj in selectedObjects) {
            List<GameObject> newSet = new List<GameObject> ();
            newSet = DFSMultipleHelper (selectedObj, newSet);
            newSets.Add (newSet);
        }
        if (selectedObjects.Count != 0) {
            DetachSetsMultiple (newSets, selectedObjects);
        }
    }

    private List<GameObject> DFSMultipleHelper (GameObject selectedObj, List<GameObject> newSet) {
        if (selectedObj == null || seenObjectsMultiple.Contains (selectedObj) || CheckPlacer (selectedObj)) {
            return newSet;
        }
        seenObjectsMultiple.Add (selectedObj);
        newSet.Add (selectedObj);
        if (selectedObj.name == "Cylinder") {
            CylinderColliderScript ccs = selectedObj.GetComponent<CylinderColliderScript> ();
            foreach (GameObject newObj in ccs.collidingObjects) {
                newSet = DFSMultipleHelper (newObj, newSet);
            }
        } else {
            ColliderScript cs = selectedObj.GetComponent<ColliderScript> ();
            foreach (GameObject newObj in cs.collidingObjects) {
                newSet = DFSMultipleHelper (newObj, newSet);
            }
        }
        return newSet;
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

    public void DetachSetsMultiple (List<List<GameObject>> newSets, List<GameObject> selectedObjects) {
        List<List<GameObject>> newComplementarySets = new List<List<GameObject>> ();
        List<List<GameObject>> oldSets = new List<List<GameObject>> ();
        foreach (GameObject selectedObj in selectedObjects) {
            foreach (List<GameObject> potentialList in setGovernScript.shapeSets) {
                if (potentialList.Contains (selectedObj)) {
                    List<GameObject> unselectedObjects = new List<GameObject> ();
                    foreach (GameObject obj in potentialList) {
                        if (!selectedObjects.Contains (obj)) {
                            unselectedObjects.Add (obj);
                        }
                    }
                    newComplementarySets.Add (unselectedObjects);
                    oldSets.Add (potentialList);
                }
            }
        }
        foreach (List<GameObject> oldSet in oldSets) {
            setGovernScript.shapeSets.Remove (oldSet);
        }
        foreach (List<GameObject> newSet in newSets) {
            setGovernScript.shapeSets.Add (newSet);
        }
        foreach (List<GameObject> newComplementarySet in newComplementarySets) {
            setGovernScript.shapeSets.Add (newComplementarySet);
        }
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