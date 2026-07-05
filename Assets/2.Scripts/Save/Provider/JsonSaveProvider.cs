using FalseWorld;
using System.IO;
using UnityEngine;

public class JsonSaveProvider : ISaveProvider
{    
    public bool Exists()
    {
        return File.Exists(SaveSettings.SavePath);
    }

    public SaveData Load()
    {
        if ( !Exists())
        {
            return new SaveData();
        }

        string json = File.ReadAllText(SaveSettings.SavePath);

        return JsonUtility.FromJson<SaveData>(json);
    }

    public void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(SaveSettings.SavePath, json);
    }

    public void Delete()
    {
        if (!Exists())
        {
            File.Delete(SaveSettings.SavePath);
        }
    }
}
