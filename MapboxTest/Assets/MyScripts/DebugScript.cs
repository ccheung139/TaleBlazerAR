using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour {
    [SerializeField]
    public Shader invisibleShader;

    public GameObject ArAlignedMap;
    public bool debugOn = false;

    public void OpenDebug () {
        foreach (Transform child in ArAlignedMap.transform) {
            child.gameObject.layer = LayerMask.NameToLayer (debugOn ? "LowerMap" : "Default");

            foreach (Transform building in child) {
                MeshRenderer meshRenderer = building.gameObject.GetComponent<MeshRenderer> ();
                foreach (Material material in meshRenderer.materials) {
                    material.shader = debugOn ? invisibleShader : Shader.Find("Standard");
                }

                // if (!childrens.Contains (building.gameObject)) {

                //     Renderer renderer = building.transform.GetComponent<Renderer> ();
                //     building.gameObject.transform.localScale = new Vector3 (1, 7, 1);

                //     MeshRenderer meshRenderer = building.gameObject.GetComponent<MeshRenderer> ();
                //     foreach (Material material in meshRenderer.materials) {
                //         material.shader = invisibleShader;
                //     }
                //     childrens.Add (building.gameObject);
                // }
            }
        }
        debugOn = !debugOn;
    }
}