using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obscurable : MonoBehaviour {
    void Start () {
        // get all renderers in this object and its children:
        var renders = GetComponentsInChildren (typeof (Renderer));
        foreach (Renderer rendr in renders) {
            rendr.material.renderQueue = 2002; // set their renderQueue
        }
    }
}