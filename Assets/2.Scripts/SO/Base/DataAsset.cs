using FalseWorld;
using UnityEngine;

[CreateAssetMenu(fileName = "DataAsset", menuName = "Scriptable Objects/DataAsset")]
public abstract class DataAsset : ScriptableObject
{
    [Header("Common")]
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField][TextArea] private string description;
    [SerializeField] private Sprite icon;

    public string ID => id;
    public string DisplayName => displayName;
    public string Description => description;
    public Sprite Icon => icon;
}
