using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadScript : MonoBehaviour {
    public Camera arCamera;
    public InventoryScript invScript;
    public bool owned;

    // Update is called once per frame
    void Update () {
        float distance = Vector3.Distance (transform.position, arCamera.transform.position);
        if (distance <= 0.5f && !owned) {
            invScript.AddBread();
            Destroy (gameObject);
        }
    }
}