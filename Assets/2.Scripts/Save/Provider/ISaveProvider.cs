using UnityEngine;
using FalseWorld;
public interface ISaveProvider
{
    bool Exists();

    SaveData Load();

    void Save(SaveData data);

    void Delete();
}