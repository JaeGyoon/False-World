using FalseWorld;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "EquipmentDataSO", menuName = "Scriptable Objects/EquipmentDataSO")]
public class EquipmentDataSO : ItemDataSO
{
    [Header("Equipment")]    
    [SerializeField]
    private EquipmentSlotType slotType;

    [SerializeField]
    private EquipmentRarityType rarity;

    [Header("Stat")]
    [SerializeField]
    private List<StatModifierDefinition> modifiers = new();
        
    [Header("Prefab")]
    [SerializeField] private AssetReferenceGameObject prefab;



    public EquipmentSlotType SlotType => slotType;

    public EquipmentRarityType Rarity => rarity;

    public AssetReferenceGameObject Prefab => prefab;

    public IReadOnlyList<StatModifierDefinition> Modifiers => modifiers;
}
