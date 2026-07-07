using System.Collections.Generic;
using UnityEngine;
using FalseWorld;

[CreateAssetMenu(fileName = "StatDataSO", menuName = "Scriptable Objects/StatDataSO")]
public class StatDataSO : ScriptableObject
{
    [SerializeField] private List<Stat> stats = new List<Stat>();

    public IReadOnlyList<Stat> Stats => stats;
}
