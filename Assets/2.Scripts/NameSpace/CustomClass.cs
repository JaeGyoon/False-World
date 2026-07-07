using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


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

    public sealed class AssetHandle<T> where T : UnityEngine.Object
    {
        public string RuntimeKey { get; }

        public T Asset { get; }

        internal AssetHandle(string runtimeKey, T asset)
        {
            if (string.IsNullOrWhiteSpace(runtimeKey))
            {
                Debug.Log($"RuntimeKey 오류 : {runtimeKey}");
            }

            if (asset == null)
            {
                Debug.Log($"Asset 오류 : {asset.name}");
            }                

            RuntimeKey = runtimeKey;
            Asset = asset;
        }
    }

    internal sealed class CacheEntry
    {
        public string RuntimeKey { get; }

        public AsyncOperationHandle Handle { get; }

        public Type AssetType { get; }

        public int ReferenceCount { get; private set; }

        public bool IsReleased { get; private set; }

        public CacheEntry(string runtimeKey, AsyncOperationHandle handle, Type assetType)
        {
            if (string.IsNullOrWhiteSpace(runtimeKey))
            {
                Debug.Log($"RuntimeKey 는 비어있을수 없음 : {runtimeKey}");
            }

            RuntimeKey = runtimeKey;
            Handle = handle;
            AssetType = assetType;

            ReferenceCount = 1;
            IsReleased = false;
        }

        public void Retain()
        {
            ReferenceCount++;
        }

        public bool ReleaseReference()
        {
            ReferenceCount--;

            return (ReferenceCount <= 0);
        }

        public void MarkReleased()
        {
            IsReleased = true;
        }

        public T GetAsset<T>() where T : UnityEngine.Object
        {
            return Handle.Result as T;
        }
    }

    public sealed class AddressableLoader
    {
        private readonly Dictionary<string, CacheEntry> cacheEntry = new Dictionary<string, CacheEntry>();

        public int CacheCount => cacheEntry.Count;

        public async Task<AssetHandle<T>> LoadAsync<T>(AssetReference reference) where T : UnityEngine.Object
        {
            if ( reference == null)
            {
                Debug.Log($"reference : NULL");
            }

            string runtimeKey = reference.RuntimeKey.ToString();

            if(cacheEntry.TryGetValue(runtimeKey, out CacheEntry entry))
            {
                if ( entry.AssetType != typeof(T))
                {
                    Debug.Log($"AssetType이 서로 다름! {entry.AssetType} : {typeof(T)}");
                }

                entry.Retain();

                return new AssetHandle<T>(runtimeKey, entry.GetAsset<T>());
            }

            var operation = reference.LoadAssetAsync<T>(); 

            await operation.Task;

            if ( operation.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"에셋 로드 실패 {reference.RuntimeKey}");
            }

            CacheEntry cache = new CacheEntry(runtimeKey, operation, typeof(T));

            cacheEntry.Add(runtimeKey, cache);

            return new AssetHandle<T>(runtimeKey, operation.Result);
        }

        public void Release<T>(AssetHandle<T> handle) where T : UnityEngine.Object
        {
            if (handle == null)
            {
                return;
            }

            if (cacheEntry.TryGetValue(handle.RuntimeKey, out CacheEntry entry) == false)
            {
                return;
            }

            if (entry.ReleaseReference() == false)
            {
                return;
            }

            if ( entry.IsReleased == false)
            {
                Addressables.Release(entry.Handle);

                entry.MarkReleased();
            }

            cacheEntry.Remove(handle.RuntimeKey);
        }

        public bool IsLoaded(string runtimeKey)
        {
            return cacheEntry.ContainsKey(runtimeKey);
        }
    }

    [Serializable]
    public class StatData
    {
        [Header("Health")]
        [SerializeField] private float maxHealth = 100f;

        [Header("Combat")]
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float defense = 0f;
        [SerializeField] private float attackSpeed = 1f;
        [SerializeField] private float criticalChance = 5f;
        [SerializeField] private float criticalDamage = 150f;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;

        // 외부 수정을 막기 위해 읽기만 가능한 데이터
        public float MaxHealth => maxHealth;
        public float AttackDamage => attackDamage;
        public float Defense => defense;
        public float AttackSpeed => attackSpeed;
        public float CriticalChance => criticalChance;
        public float CriticalDamage => criticalDamage;
        public float MoveSpeed => moveSpeed;

    }

    // 아직 구현하진 않고 명시
    [Serializable]
    public class SkillData
    {

    }

    [Serializable]
    public class EnemyAISettings
    {
        [SerializeField] private AIBehaviorType behaviorType;
        [SerializeField] private SpawnState spawnState;

        [SerializeField] private float detectRange = 10f;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float patrolRange = 5f;
        [SerializeField] private float chaseDistance = 12f;

        public AIBehaviorType BehaviorType => behaviorType;
        public SpawnState SpawnState => spawnState;

        public float DetectRange => detectRange;
        public float AttackRange => attackRange;
        public float PatrolRange => patrolRange;
        public float ChaseDistance => chaseDistance;


    }

}

