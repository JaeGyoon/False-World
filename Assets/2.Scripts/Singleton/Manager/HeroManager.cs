using System.Threading.Tasks;
using UnityEngine;

public class HeroManager : ManagerBase<HeroManager>
{
    [SerializeField]
    private HeroDatabase database;

    public HeroDataSO SelectedHero    { get; private set; }

    public override async Task Initialize()
    {
        //string heroID = SaveManager.Instance.data
    }
}
