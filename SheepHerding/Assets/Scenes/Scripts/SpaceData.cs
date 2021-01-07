using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpaceData 
{
    public float[] center;
    public float[] extents;

    public SpaceData(DrawSpaceScript dss) {
        center = new float[3];
        center[0] = dss.v3Center.x;
        center[1] = dss.v3Center.y;
        center[2] = dss.v3Center.z;

        extents = new float[3];
        extents[0] = dss.v3Extents.x;
        extents[1] = dss.v3Extents.y;
        extents[2] = dss.v3Extents.z;
    }
}
