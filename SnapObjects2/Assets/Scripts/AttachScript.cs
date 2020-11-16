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
        attachButton.onClick.AddListener (AttachObject);
    }

    public void AttachObject () {
        attachOn = true;
        previousAttachPosition = selectObjectsScript.selectedShape.transform.position;
        previousAttachRotation = selectObjectsScript.selectedShape.transform.rotation;
    }

    public void HandleAttach (GameObject obj, RaycastHit hit) {
        if (selectObjectsScript.selectedShape.name.Contains ("Cube") || selectObjectsScript.selectedShape.name.Contains ("Sphere")) {
            CylinderColliderScript cs = obj.GetComponent<CylinderColliderScript> ();
            if (cs.topNeighbor == null) {
                Transform topOfCylinder = obj.transform.Find ("TopOfCylinder");
                selectObjectsScript.selectedShape.transform.position = topOfCylinder.position;
            } else if (cs.bottomNeighbor == null) {
                Transform bottomOfCylinder = obj.transform.Find ("BottomOfCylinder");
                selectObjectsScript.selectedShape.transform.position = bottomOfCylinder.position;
            } else {
                Debug.Log ("all spaces taken up!");
                return;
            }

            selectObjectsScript.selectedShape.transform.rotation = obj.transform.rotation;

        } else {
            GameObject parentCylinder = selectObjectsScript.selectedShape.transform.parent.gameObject;
            parentCylinder.transform.position = hit.transform.position;
            parentCylinder.transform.rotation = Quaternion.FromToRotation (Vector3.up, hit.normal);

            Vector3 upVector = parentCylinder.transform.forward;
            parentCylinder.transform.RotateAround (selectObjectsScript.selectedShape.transform.position, upVector, 180);
        }
        ps.preliminaryObject = selectObjectsScript.selectedShape;
        selectObjectsScript.DeselectShape ();
        ps.preliminaryObject.GetComponent<Renderer> ().sharedMaterial = yellowMaterial;
        cancelScript.DisableAllPlacers (true);
        placeButton.gameObject.SetActive (true);

        setGovernScript.CombineSets (obj, ps.preliminaryObject);
    }

}