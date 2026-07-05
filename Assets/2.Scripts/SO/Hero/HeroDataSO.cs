using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "HeroDataSO", menuName = "Scriptable Objects/HeroDataSO")]
public class HeroDataSO : ScriptableObject
{
    public string heroID;

    public string heroName;

    public AssetReferenceGameObject prefab;
}
