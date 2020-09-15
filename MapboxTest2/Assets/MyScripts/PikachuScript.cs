using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PikachuScript : MonoBehaviour {
    public AbstractMap map;
    public Transform mapTransform;

    public GameObject camera;

    void Update () {
        float distance = Vector3.Distance (camera.transform.position, transform.position);
        float fc = (float) Math.Round (distance * 100f) / 100f;
        if (distance >= 7f) {
            transform.localScale = new Vector3 (0, 0, 0);
        } else {
            UpdateMapLocation ();

            // update to ensure Pikachu reappears when close
            distance = Vector3.Distance (camera.transform.position, transform.position);
            if (distance < 7f) {
                transform.localScale = new Vector3 (0.04f, 0.04f, 0.04f);
            }

        }

    }

    public void UpdateMapLocation () {
        var location = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation;
        map.UpdateMap (location.LatitudeLongitude, map.AbsoluteZoom);
        var playerPos = Camera.main.transform.position;
        mapTransform.position = new Vector3 (playerPos.x, mapTransform.position.y, playerPos.z);
    }
}