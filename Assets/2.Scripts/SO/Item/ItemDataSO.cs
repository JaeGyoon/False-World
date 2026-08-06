using FalseWorld;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "ItemDataSO", menuName = "Scriptable Objects/ItemDataSO")]
public class ItemDataSO : DataAsset
{
    [Header("Item Data")]
    [SerializeField] private ItemType itemType;
    [SerializeField] private bool stackable;
    [SerializeField] private int sellPrice;
    [SerializeField] private int buyPrice;
    [SerializeField] private int maxStack = 1;
    [SerializeField] private int weight;

    public ItemType ItemType => itemType;
    public bool Stackable => stackable;
    public int SellPrice => sellPrice;

    public int BuyPrice => buyPrice;
    public int MaxStack => maxStack;
    public int Weight => weight;
}
