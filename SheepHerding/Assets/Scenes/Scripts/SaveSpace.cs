using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSpace {

    public static void SavePlayerSpace (DrawSpaceScript dss) {
        BinaryFormatter formatter = new BinaryFormatter ();
        string path = Application.persistentDataPath + "/space.fun";
        FileStream stream = new FileStream (path, FileMode.Create);

        SpaceData data = new SpaceData (dss);

        formatter.Serialize (stream, data);
        stream.Close ();
    }

    public static SpaceData LoadPlayerSpace () {
        string path = Application.persistentDataPath + "/space.fun";

        if (File.Exists (path)) {
            BinaryFormatter formatter = new BinaryFormatter ();
            FileStream stream = new FileStream (path, FileMode.Open);

            SpaceData data = formatter.Deserialize (stream) as SpaceData;
            stream.Close ();

            return data;
        } else {
            Debug.LogError ("Save file not found in " + path);
            return null;
        }
    }
}