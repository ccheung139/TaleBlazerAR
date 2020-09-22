using UnityEngine;

public static class RendererExtensions {
    public static bool IsVisibleFrom (this Renderer renderer, Camera camera) {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes (camera);
        return GeometryUtility.TestPlanesAABB (planes, renderer.bounds);
    }
}

// Renderer renderer = orb.GetComponent<Renderer> ();
// if (GetComponent<Renderer> ().IsVisibleFrom (arCamera)) {
//     // text.text = "is visible";
//     isSeen = true;
// } else {
//     // text.text = "not visible";
//     isSeen = false;
// }Î