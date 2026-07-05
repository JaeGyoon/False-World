using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;


namespace FalseWorld
{
    [Serializable]
    public class SaveData
    {
        // Hero
        public string selectedHeroID;

        // Currency
        public int gold;
        public int diamond;

        // Stage
        public int lastStage;

        // Option
        public float bgmVolume = 1f;
        public float sfxVolume = 1f;
        public int language;

        // Collection
        public List<string> unlockHeroList = new List<string>();
    }

    public static class SaveSettings
    {
        public const int SaveVersion = 1;

        public const string SaveFileName = "SaveData.json";

        public static string SavePath = Path.Combine(Application.persistentDataPath, SaveFileName);
    }

    public static class AddressablesSettings
    {
        public const string HeroLabel = "Heros";
        public const string EnemyLabel = "Enemies";
        public const string UILabel = "UI";
        public const string EffectLabel = "Effects";
    }




    // sealed = 상속 금지 키워드
    public sealed class AssetHandle<T>
    {
        internal AsyncOperationHandle<T> handle;

        public T Asset => handle.Result;

        internal AssetHandle(AsyncOperationHandle<T> handle)
        {
            this.handle = handle;
        }
    }

    public class AddressableLoader
    {
        public async Task<AssetHandle<T>> LoadAsync<T>(AssetReference reference)
        {
            var operation = reference.LoadAssetAsync<T>();

            await operation.Task;

            return new AssetHandle<T>(operation);
        }

        public void Release<T> (AssetHandle<T> assetHandle)
        {
            Addressables.Release(assetHandle.handle);
        }

    }

    internal class AssetCache
    {
        private readonly Dictionary<object, object> cache = new Dictionary<object, object>();

        public bool TryGet<T>(object key, out AssetHandle<T> handle)
        {
            if ( cache.TryGetValue(key, out var value))
            {
                handle = value as AssetHandle<T>;

                return handle != null;
            }

            handle = null;

            return false;
        }

        public void Add<T>(object key, AssetHandle<T> handle)
        {
            // cache.Add(key,handle) 과 같은 기능
            cache[key] = handle;
        }

        public void Remove(object key)
        {
            cache.Remove(key);
        }

        public void Clear()
        {
            cache.Clear();
        }
    }
}

