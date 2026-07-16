using FalseWorld;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "EntityData", menuName = "Scriptable Objects/EntityData")]
public class EntityData : DataAsset
{
    [Header("Entity")]
    [SerializeField] private EntityType type;

    [Header("Prefab")]
    [SerializeField] private AssetReferenceGameObject prefab;

    public EntityType Type => type;
    public AssetReferenceGameObject Prefab => prefab;
    
}
