using FalseWorld;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : EntityData
{
    [Header("Animation")]
    [SerializeField] private RuntimeAnimatorController animatorController;

    [Header("Skills")]
    [SerializeField] private List<SkillData> skills = new List<SkillData>();

    public RuntimeAnimatorController AnimatorController => animatorController;
    public List<SkillData> Skills => skills;
}
