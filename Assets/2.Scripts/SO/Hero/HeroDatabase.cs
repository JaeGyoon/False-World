using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroDatabase", menuName = "Scriptable Objects/HeroDatabase")]
public class HeroDatabase : ScriptableObject
{
    public List<HeroDataSO> heros;    
}
