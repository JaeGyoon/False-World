using FalseWorld;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "ItemDataSO", menuName = "Scriptable Objects/ItemDataSO")]
public class ItemDataSO : DataAsset
{
    [Header("Item Data")]
    [SerializeField] private int buyPrice;
    [SerializeField] private int sellPrice;
    [SerializeField] private int maxStack = 1;
    [SerializeField] private int weight;

    public int BuyPrice => buyPrice;
    public int SellPrice => sellPrice;
    public int MaxStack => maxStack;
    public int Weight => weight;
}
