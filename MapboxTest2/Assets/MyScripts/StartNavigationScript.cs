using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartNavigationScript : MonoBehaviour {
    public GameObject startHint;
    public GameObject goButton;
    public GameObject ArAlignedMap;

    public bool hasStarted;
    public List<GameObject> childrens = new List<GameObject> ();

    public void StartNavigation () {
        hasStarted = true;
        startHint.SetActive (false);
        goButton.SetActive (false);

        foreach (Transform child in ArAlignedMap.transform) {
            child.gameObject.layer = LayerMask.NameToLayer ("LowerMap");
            foreach (Transform building in child) {
                if (!childrens.Contains (building.gameObject)) {

                    // Renderer renderer = buildisng.transform.GetComponent<Renderer> ();
                    // building.gameObject.transform.localScale = new Vector3 (1, 7, 1);

                    // building.gameObject.active = false;

                    MeshRenderer meshRenderer = building.gameObject.GetComponent<MeshRenderer> ();
                    meshRenderer.enabled = false;
                    // foreach (Material material in meshRenderer.materials) {
                    //     // material.shader = invisibleShader;
                    //     material.color = Color.clear;
                    // }
                    childrens.Add (building.gameObject);
                }
            }
        }
    }
}