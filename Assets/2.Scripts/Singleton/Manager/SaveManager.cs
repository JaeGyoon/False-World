using UnityEngine;
using System.Threading.Tasks;
using FalseWorld;

public class SaveManager : ManagerBase<SaveManager>
{
    private ISaveProvider provider;

    public SaveData data
    {
        get; private set;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public override async Task Initialize()
    {
        Debug.Log("Save Manager Init");

        // provider 종류 설정
        provider = new JsonSaveProvider();

        //data = provider.Load();
        Load();

        await base.Initialize();
    }

    private void Save()
    {
        provider.Save(data);
    }

    private void Load()
    {
        data = provider.Load();
    }

    private void Delete()
    {
        provider.Delete();

        data = new SaveData();
    }

}
