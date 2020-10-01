using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedWorld : MonoBehaviour {

    [SerializeField]
    private Camera arCamera;

    private UnityEngine.Rendering.VolumeProfile v;
    private UnityEngine.Rendering.Universal.Bloom b;
    private UnityEngine.Rendering.Universal.LensDistortion ld;

    void Start () {
        UnityEngine.Rendering.VolumeProfile v = arCamera.GetComponent<UnityEngine.Rendering.Volume> ()?.profile;
        v.TryGet (out b);
        v.TryGet (out ld);
        // b.tint = Color.green;
    }
}