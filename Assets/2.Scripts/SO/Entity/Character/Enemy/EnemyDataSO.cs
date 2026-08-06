using FalseWorld;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "Scriptable Objects/EnemyDataSO")]
public class EnemyDataSO : CharacterDataSO
{
    [Header("AI")]
    [SerializeField] private EnemyAISettings ai = new EnemyAISettings();

    public EnemyAISettings AI => ai;
}
