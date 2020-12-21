using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeleteScript : MonoBehaviour {
    public Button deleteButton;
    public SelectObjectsScript selectObjectsScript;
    public SetGovernScript setGovernScript;
    public PlacementScript placementScript;

    // Start is called before the first frame update
    void Start () {
        deleteButton.onClick.AddListener (DeletePressed);
    }

    // Update is called once per frame
    void Update () {

    }

    private void DeletePressed () {
        foreach (GameObject shape in placementScript.allSelects) {
            setGovernScript.RemoveObjectFromSet (shape);
            Destroy (shape);
        }
        List<GameObject> selectedShapes = selectObjectsScript.selectedShapes;
        foreach (GameObject shape in selectedShapes) {
            setGovernScript.RemoveObjectFromSet (shape);
            Destroy (shape);
        }
        selectObjectsScript.Reset ();
    }
}