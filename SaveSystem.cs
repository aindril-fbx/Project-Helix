using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{

    public static void SaveData(CarStats carStats, string name){
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + name + ".helix";
        FileStream stream = new FileStream(path , FileMode.Create);

        CarData data = new CarData(carStats);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static CarData LoadCar(string name){
        string path = Application.persistentDataPath + "/" + name + ".helix";
        if(File.Exists(path)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path , FileMode.Open);

            CarData data = formatter.Deserialize(stream) as CarData;
            stream.Close();
            return data;

        }else{
            Debug.LogError("Save File not found in " + path);
            return null;
        }
    }
}
