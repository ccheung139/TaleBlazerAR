using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttachScript : MonoBehaviour {
    public PlacementScript ps;
    public SelectObjectsScript selectObjectsScript;
    public CancelScript cancelScript;
    public SetGovernScript setGovernScript;

    public Button attachButton;
    public Button placeButton;
    public Material yellowMaterial;

    public bool attachOn = false;
    public Vector3 previousAttachPosition;
    public Quaternion previousAttachRotation;

    void Start () {
        // attachButton.onClick.AddListener (AttachObject);
    }

    public void AttachObject () {
        attachOn = true;
        previousAttachPosition = selectObjectsScript.selectedShapes[0].transform.position;
        previousAttachRotation = selectObjectsScript.selectedShapes[0].transform.rotation;
    }

    // public void HandleAttach (GameObject obj, RaycastHit hit) {
    //     if (obj.name.Contains ("Cylinder")) {
    //         AttachToCylinder(obj);
    //     } else {

    //     }
    // }

    private void AttachToCylinder (GameObject obj) {
        CylinderColliderScript cs = obj.GetComponent<CylinderColliderScript> ();
        if (cs.topNeighbor == null) {
            Transform topOfCylinder = obj.transform.Find ("TopOfCylinder");
            selectObjectsScript.selectedShapes[0].transform.position = topOfCylinder.position;
        } else if (cs.bottomNeighbor == null) {
            Transform bottomOfCylinder = obj.transform.Find ("BottomOfCylinder");
            selectObjectsScript.selectedShapes[0].transform.position = bottomOfCylinder.position;
        } else {
            Debug.Log ("all spaces taken up!");
            return;
        }

        selectObjectsScript.selectedShapes[0].transform.rotation = obj.transform.rotation;
    }

    public void HandleAttach (GameObject obj, RaycastHit hit) {
        if (selectObjectsScript.selectedShapes[0].name.Contains ("Cube") || selectObjectsScript.selectedShapes[0].name.Contains ("Sphere")) {
            CylinderColliderScript cs = obj.GetComponent<CylinderColliderScript> ();
            if (cs.topNeighbor == null) {
                Transform topOfCylinder = obj.transform.Find ("TopOfCylinder");
                selectObjectsScript.selectedShapes[0].transform.position = topOfCylinder.position;
            } else if (cs.bottomNeighbor == null) {
                Transform bottomOfCylinder = obj.transform.Find ("BottomOfCylinder");
                selectObjectsScript.selectedShapes[0].transform.position = bottomOfCylinder.position;
            } else {
                Debug.Log ("all spaces taken up!");
                return;
            }

            selectObjectsScript.selectedShapes[0].transform.rotation = obj.transform.rotation;

        } else {
            GameObject parentCylinder = selectObjectsScript.selectedShapes[0].transform.parent.gameObject;
            parentCylinder.transform.position = hit.transform.position;
            parentCylinder.transform.rotation = Quaternion.FromToRotation (Vector3.up, hit.normal);

            Vector3 upVector = parentCylinder.transform.forward;
            parentCylinder.transform.RotateAround (selectObjectsScript.selectedShapes[0].transform.position, upVector, 180);
        }
        ps.preliminaryObject = selectObjectsScript.selectedShapes[0];
        selectObjectsScript.DeselectShapes ();
        ps.preliminaryObject.GetComponent<Renderer> ().sharedMaterial = yellowMaterial;
        cancelScript.DisableAllPlacers (true);
        placeButton.gameObject.SetActive (true);

        setGovernScript.CombineSets (obj, ps.preliminaryObject);
    }

}