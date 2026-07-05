using UnityEngine;
using System.Threading.Tasks;

public class GameManager : ManagerBase<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override async Task Initialize()
    {
        Debug.Log("Game Manager Init");

        await base.Initialize();
    }


}
