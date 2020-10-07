using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TrophyScript : MonoBehaviour {
    public Camera arCamera;
    public GameObject gameOverPanel;
    public GameObject gameWonPanel;

    private UnityEngine.Rendering.VolumeProfile v;
    private UnityEngine.Rendering.Universal.LensDistortion ld;

    void Start () {
        UnityEngine.Rendering.VolumeProfile v = arCamera.GetComponent<UnityEngine.Rendering.Volume> ()?.profile;
        v.TryGet (out ld);
    }

    void Update () {
        float distance = Vector3.Distance (arCamera.transform.position, new Vector3 (transform.position.x, arCamera.transform.position.y, transform.position.z));
        if (distance <= 8f) {
            ld.intensity.value = -((8f - distance) / 8f);

        }
        if (distance < 2f) {
            if (!gameOverPanel.activeSelf) {
                gameWonPanel.SetActive (true);
            }
        }
    }
}