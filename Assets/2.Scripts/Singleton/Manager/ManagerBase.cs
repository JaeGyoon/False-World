using UnityEngine;
using System.Threading.Tasks;

public abstract class ManagerBase<T> : Singleton<T> where T : ManagerBase<T>
{
   public bool IsInitialized {  get; private set; }

    protected override void Awake()
    {
        base.Awake();
    }

    public virtual async Task Initialize()
    {
        IsInitialized = true;

        await Task.CompletedTask;
    }

}
