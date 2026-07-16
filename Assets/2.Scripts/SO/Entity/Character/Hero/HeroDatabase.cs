using FalseWorld;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroDatabase", menuName = "Scriptable Objects/HeroDatabase")]
public class HeroDatabase : DatabaseBase
{
    [SerializeField] private List<HeroDataSO> heros = new List<HeroDataSO>();

    private Dictionary<string, HeroDataSO> heroDict;

    public override void Initialize()
    {
        heroDict = new Dictionary<string, HeroDataSO>();

        foreach (HeroDataSO so in heros)
        {
            if ( so == null)
            {
                continue;
            }

            if ( heroDict.ContainsKey(so.ID))
            {
                Debug.Log("중복된 ID가 있습니다.");
                continue;
            }

            heroDict.Add(so.ID, so);
        }

        Debug.Log("Hero Database Initialize 완료");
    }

    public HeroDataSO Get(string id)
    {
        if ( heroDict == null )
        {
            Initialize();
        }

        if (heroDict.TryGetValue(id, out HeroDataSO data))
        {
            return data;
        }
        else
        {
            Debug.Log($"해당 ID가 없습니다. {id}");
            return null;
        }
    }

    public IReadOnlyList<HeroDataSO> AllHeros => heros;

}
