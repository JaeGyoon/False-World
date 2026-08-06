using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FalseWorld
{
    // Core System

    public interface ISceneLoader
    {
        Task LoadScene(SceneName sceneName);
    }

    public interface IAddressableService
    {
        Task InitializeAsync();

        Task<T> LoadAssetAsync<T>(AssetReference reference) where T : Object;

        Task<GameObject> InstantiateAsync(AssetReference reference);

        void Release(object asset);

        void ReleaseInstance(GameObject instance);
    }

    public interface ISaveService
    {
        PlayerSaveData CurrentSaveData { get; }

        Task InitializeAsync();

        Task SaveAsync();

        void MarkDirty();

        string GetSelectedHero();

        void SetSelectedHero(string heroID);

        string GetSelectedStage();

        void SetSelectedStage(string stageID);

    }




    public interface IDatabase
    {
        void Initialize();
    }

    public interface IStatModifierSource
    {                
        string DisplayName { get; }
    }

    public interface IStatModifier
    {
        StatModifierOrder Order { get; }

        IStatModifierSource Source { get; }

        float StatCalculate(float currentValue);
    }

    public interface IRuntimeFactory
    {

    }

}