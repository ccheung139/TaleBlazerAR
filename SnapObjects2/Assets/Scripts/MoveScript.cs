using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveScript : MonoBehaviour {
    public Button moveButton;
    public Button finishButton;
    public Camera arCamera;
    public PlacementScript placementScript;
    public SetGovernScript setGovernScript;
    public SelectObjectsScript selectObjectsScript;
    public CancelScript cancelScript;
    public Material blueMaterial;
    public Material yellowMaterial;

    private Transform cylinderParent;
    private bool isMoving = false;
    private List<GameObject> seenObjects = new List<GameObject> ();
    private List<GameObject> seenObjectsMultiple = new List<GameObject> ();

    void Start () {
        moveButton.onClick.AddListener (PressedMove);
        finishButton.onClick.AddListener (PressedFinish);
    }

    // void Update () {
    //     if (isMoving && selectObjectsScript.selectedShapes.Count > 0) {
    //         bool isAttached = false;
    //         GameObject movingObject = selectObjectsScript.selectedShapes[0];
    //         if (movingObject.name.Contains ("Cylinder")) {
    //             movingObject = movingObject.transform.parent.gameObject;
    //         }
    //         RaycastHit[] hits = Physics.RaycastAll (arCamera.transform.position - arCamera.transform.up * 0.1f, arCamera.transform.forward, 1.5f);
    //         if (hits.Length > 0) {
    //             foreach (RaycastHit hit in hits) {
    //                 GameObject obj = hit.transform.gameObject;
    //                 if (!selectObjectsScript.selectedShapes.Contains (obj) && !placementScript.allSelects.Contains (obj)) {
    //                     float distance = Vector3.Distance (obj.transform.position, arCamera.transform.position);
    //                     if (distance <= .6f) {
    //                         movingObject.transform.position = hit.point;
    //                         ChangeColor (movingObject, yellowMaterial);
    //                         isAttached = true;
    //                         break;
    //                     }
    //                 }
    //             }
    //         }
    //         if (!isAttached) {
    //             ChangeColor (movingObject, blueMaterial);
    //             movingObject.transform.position = arCamera.transform.position + arCamera.transform.forward * 0.3f - arCamera.transform.up * 0.1f;
    //         }
    //     }
    // }

    // private void ChangeColor (GameObject movingObject, Material newColor) {
    //     GameObject toChange = movingObject;
    //     if (movingObject.name.Contains ("CylinderParent")) {
    //         toChange = movingObject.transform.Find ("Cylinder").gameObject;
    //     }
    //     toChange.GetComponent<Renderer> ().sharedMaterial = newColor;
    // }

    private void PressedMove () {
        List<GameObject> selectedObjects = selectObjectsScript.selectedShapes;
        MoveMultipleObjects (selectedObjects);
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
        List<GameObject> selectedObjects = selectObjectsScript.selectedShapes;
        if (selectedObjects.Count == 0) {
            isMoving = false;
            return;
        }
        FinishMovementMultiple (selectedObjects);
        DFSMultipleFindAttached (selectedObjects);

        seenObjectsMultiple = new List<GameObject> ();
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
        selectObjectsScript.DeselectShapes ();
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