using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpaceData {
    // public Bounds room1Bounds;
    public float[] room1Center;
    public float[] room1Extents;
    public float[] room2Center;
    public float[] room2Extents;
    public float[, ] connectingRoomCenters;
    public float[, ] connectingRoomExtents;

    public float[] frontGatePosition;
    public float[] backGatePosition;

    public SpaceData (DrawSpaceScript dss) {
        AssignCenterAndExtent (out room1Center, out room1Extents, dss.room1Bounds);
        AssignCenterAndExtent (out room2Center, out room2Extents, dss.room2Bounds);
        List<Bounds> connectingRoomBounds = dss.connectingRooms;
        connectingRoomCenters = new float[connectingRoomBounds.Count, 3];
        connectingRoomExtents = new float[connectingRoomBounds.Count, 3];
        for (int i = 0; i < connectingRoomBounds.Count; i++) {
            float[] connectCenter;
            float[] connectExtent;
            AssignCenterAndExtent (out connectCenter, out connectExtent, connectingRoomBounds[i]);
            for (int j = 0; j < 3; j++) {
                connectingRoomCenters[i, j] = connectCenter[j];
                connectingRoomExtents[i, j] = connectExtent[j];
            }
        }

        frontGatePosition = new float[3];
        frontGatePosition[0] = dss.frontGatePosition.x;
        frontGatePosition[1] = dss.frontGatePosition.y;
        frontGatePosition[2] = dss.frontGatePosition.z;

        backGatePosition = new float[3];
        backGatePosition[0] = dss.backGatePosition.x;
        backGatePosition[1] = dss.backGatePosition.y;
        backGatePosition[2] = dss.backGatePosition.z;

        Debug.Log (dss.frontGatePosition);
        Debug.Log (dss.backGatePosition);
    }

    private void AssignCenterAndExtent (out float[] center, out float[] extents, Bounds bounds) {
        center = new float[3];
        center[0] = bounds.center.x;
        center[1] = bounds.center.y;
        center[2] = bounds.center.z;

        extents = new float[3];
        extents[0] = bounds.extents.x * 2f;
        extents[1] = bounds.extents.y;
        extents[2] = bounds.extents.z * 2f;
    }
}