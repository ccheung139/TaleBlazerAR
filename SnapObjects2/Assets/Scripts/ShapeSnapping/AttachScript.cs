using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttachScript : MonoBehaviour {
    public PlacementScript ps;
    public SelectObjectsScript selectObjectsScript;
    public CancelScript cancelScript;
    public SetGovernScript setGovernScript;

    public Button placeButton;
    public Material yellowMaterial;

    public bool attachOn = false;
    public Vector3 previousAttachPosition;
    public Quaternion previousAttachRotation;

}