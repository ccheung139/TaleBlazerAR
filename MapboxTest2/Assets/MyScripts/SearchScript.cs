using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using UnityEngine;
using UnityEngine.UI;

public class SearchScript : MonoBehaviour {
    [SerializeField]
    AbstractMap _map;

    [SerializeField]
    Transform _mapTransform;

    public DistanceScript distanceScript;

    public GameObject camera;
    public bool found = false;
    public GameObject pikachu;

    public void SearchForPikachu () {
        UpdateMapLocation();
        GameObject chosenBuilding = distanceScript.chosenBuilding;

        Renderer renderer = chosenBuilding.transform.GetComponent<Renderer> ();

        Vector3 groundOfBuilding = new Vector3 (renderer.bounds.center.x, 0, renderer.bounds.center.z);
        Vector3 sizeVec = renderer.bounds.size;
        Vector3 closestPoint = findClosestEdge (groundOfBuilding, sizeVec, camera.transform.position);
        float distance = Vector3.Distance (camera.transform.position, closestPoint);

        if (distance <= 5f) {
            MeshRenderer meshRenderer = chosenBuilding.gameObject.GetComponent<MeshRenderer> ();
            foreach (Material material in meshRenderer.materials) {
                material.color = Color.red;
            }

            GameObject newPikachu = Instantiate (pikachu, closestPoint, Quaternion.identity) as GameObject;
            newPikachu.transform.parent = chosenBuilding.transform;
            newPikachu.AddComponent<PikachuScript> ().camera = camera;

            found = true;
        }
    }

    Vector3 findClosestEdge (Vector3 groundOfBuilding, Vector3 sizeVec, Vector3 cameraPosition) {
        Vector3 pointPlusX = groundOfBuilding + new Vector3 (sizeVec.x / 2, 0, 0);
        Vector3 pointMinusX = groundOfBuilding + new Vector3 (-sizeVec.x / 2, 0, 0);
        Vector3 pointPlusZ = groundOfBuilding + new Vector3 (0, 0, sizeVec.z / 2);
        Vector3 pointMinusZ = groundOfBuilding + new Vector3 (0, 0, -sizeVec.z / 2);

        float smallestDistance = float.PositiveInfinity;
        Vector3 closestPoint = pointPlusX;

        Vector3[] positionArray = new [] { pointPlusX, pointMinusX, pointPlusZ, pointMinusZ };
        foreach (Vector3 point in positionArray) {
            float distance = Vector3.Distance (cameraPosition, point);
            if (distance < smallestDistance) {
                closestPoint = point;
                smallestDistance = distance;
            }
        }
        return closestPoint;
    }

    public void UpdateMapLocation () {
        var location = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation;
        _map.UpdateMap (location.LatitudeLongitude, _map.AbsoluteZoom);
        var playerPos = Camera.main.transform.position;
        _mapTransform.position = new Vector3 (playerPos.x, _mapTransform.position.y, playerPos.z);
    }

}