using FalseWorld;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CharacterDataSO", menuName = "Scriptable Objects/CharacterData")]
public class CharacterDataSO : EntityData
{
    [Header("Stat")]
    [SerializeField] List<Stat> stats = new List<Stat>();

    public IReadOnlyList<Stat> Stats => stats;

    [Header("Animation")]
    [SerializeField] private RuntimeAnimatorController animatorController;

    [Header("Skills")]
    [SerializeField] private List<SkillData> skills = new List<SkillData>();

    public RuntimeAnimatorController AnimatorController => animatorController;
    public List<SkillData> Skills => skills;
}
