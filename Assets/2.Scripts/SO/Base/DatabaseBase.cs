using FalseWorld;
using UnityEngine;

public abstract class DatabaseBase : ScriptableObject, IDatabase
{
    public abstract void Initialize();
}
