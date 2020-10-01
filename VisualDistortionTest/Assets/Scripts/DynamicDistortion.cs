using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class DynamicDistortion : MonoBehaviour {

    [SerializeField]
    private Camera arCamera;
    [SerializeField]
    private Text effectText;

    private UnityEngine.Rendering.VolumeProfile v;
    private UnityEngine.Rendering.Universal.Bloom b;
    private UnityEngine.Rendering.Universal.LensDistortion ld;
    private UnityEngine.Rendering.Universal.ChromaticAberration ca;
    private UnityEngine.Rendering.Universal.FilmGrain fg;
    private UnityEngine.Rendering.Universal.WhiteBalance wb;

    private int worldsFound = 0;
    private System.Random random = new System.Random ();

    void Start () {
        UnityEngine.Rendering.VolumeProfile v = arCamera.GetComponent<UnityEngine.Rendering.Volume> ()?.profile;
        v.TryGet (out b);
        v.TryGet (out ld);
        v.TryGet (out ca);
        v.TryGet (out fg);
        v.TryGet (out wb);
    }

    void Update () {
        float distance = Vector3.Distance (arCamera.transform.position, transform.position);
        if (distance <= 3f) {
            ld.intensity.value = -((3f - distance) / 3f);
            b.intensity.value = (3f - distance);

        }
        if (distance <= 1f) {
            transform.position = (worldsFound % 2 == 0) ? new Vector3 (0, 0, 0) : new Vector3 (0, 0, 4);
            worldsFound += 1;
            TransformWorld ();
        }
    }

    void TransformWorld () {
        var worlds = new List<Action> ();
        worlds.Add (EnableRedWorld);
        worlds.Add (EnableGreenWorld);
        worlds.Add (EnableBlueWorld);
        worlds.Add (EnableYellowWorld);
        int index = random.Next (0, worlds.Count);
        Action newWorld = worlds[index];
        newWorld ();
        b.intensity.value = 0;
        ld.intensity.value = 0;

        var otherWorlds = new List<Action> ();
        otherWorlds.Add (EnableChromaticAberration);
        otherWorlds.Add (EnableFilmGrain);
        otherWorlds.Add (EnableWhiteBalance);
        int indexOther = random.Next (0, otherWorlds.Count);
        Action newWorldOther = otherWorlds[indexOther];

        fg.intensity.value = 0f;
        ca.intensity.value = 0f;
        wb.temperature.value = 0f;
        wb.tint.value = 0f;
        newWorldOther ();
    }

    void EnableChromaticAberration () {
        effectText.text = "Chromatic";
        ca.intensity.value = 1f;
    }

    void EnableFilmGrain () {
        effectText.text = "Film Grain";
        fg.intensity.value = 1f;
    }

    void EnableWhiteBalance () {
        effectText.text = "White Balance";
        wb.temperature.value = 100f;
        wb.tint.value = 100f;
    }

    void EnableRedWorld () {
        b.tint.value = Color.red;
    }

    void EnableGreenWorld () {
        b.tint.value = Color.green;
    }

    void EnableBlueWorld () {
        b.tint.value = Color.blue;
    }

    void EnableYellowWorld () {
        b.tint.value = Color.yellow;
    }
}