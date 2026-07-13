using FalseWorld;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "EntityData", menuName = "Scriptable Objects/EntityData")]
public class EntityData : DataAsset
{
    [Header("Identity")]
    [SerializeField] private EntityID id;
    [SerializeField] private EntityType type;

    [Header("Prefab")]
    [SerializeField] private AssetReferenceGameObject prefab;

    /*[Header("Stats")]
    [SerializeField] private StatData stats = new StatData();*/

    public EntityID ID => id;
    public EntityType Type => type;
    public AssetReferenceGameObject Prefab => prefab;
    /*public StatData Stats => stats;*/
}
