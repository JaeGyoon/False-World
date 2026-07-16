using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;


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

   /* public static class AddressablesSettings
    {
        public const string HeroLabel = "Heros";
        public const string EnemyLabel = "Enemies";
        public const string UILabel = "UI";
        public const string EffectLabel = "Effects";
    }*/

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
            if (reference == null)
            {
                Debug.Log($"reference : NULL");
            }

            string runtimeKey = reference.RuntimeKey.ToString();

            if (cacheEntry.TryGetValue(runtimeKey, out CacheEntry entry))
            {
                if (entry.AssetType != typeof(T))
                {
                    Debug.Log($"AssetType이 서로 다름! {entry.AssetType} : {typeof(T)}");
                }

                entry.Retain();

                return new AssetHandle<T>(runtimeKey, entry.GetAsset<T>());
            }

            var operation = reference.LoadAssetAsync<T>();

            await operation.Task;

            if (operation.Status != AsyncOperationStatus.Succeeded)
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

            if (entry.IsReleased == false)
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

    public abstract class Entity<TData> : EntityBase where TData : EntityData
    {
        public TData Data { get; protected set; }

        public virtual void Initialize(TData data)
        {
            Data = data;
        }
    }

    public abstract class Character<TData> : Entity<TData> where TData : CharacterData
    {

    }

    public sealed class Hero : Character<HeroDataSO>
    {
        public override void Release()
        {
            throw new NotImplementedException();
        }
    }

    public sealed class Enemy : Character<EnemyDataSO>
    {
        public override void Release()
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public sealed class Stat
    {
        [SerializeField] private StatType type;

        [SerializeField] private float baseValue;

        public StatType Type => type;
        public float BaseValue => baseValue;
    }

    public abstract class StatModifierBase : IStatModifier
    {
        public StatModifierOrder Order { get; }
        public IStatModifierSource Source { get; }

        public float Value { get; }

        protected StatModifierBase(float value, StatModifierOrder order, IStatModifierSource source)
        {
            Value = value;
            Order = order;
            Source = source;
        }

        public abstract float StatCalculate(float currentValue);
    }

    public sealed class AddModifier : StatModifierBase
    {
        public AddModifier(float value, StatModifierOrder order, IStatModifierSource source) : base(value, order, source)
        {

        }

        public override float StatCalculate(float currentValue)
        {
            return currentValue + Value;
        }
    }

    public sealed class MultiplyModifier : StatModifierBase
    {

        public MultiplyModifier(float value, StatModifierOrder order, IStatModifierSource source) : base(value, order, source)
        {

        }

        public override float StatCalculate(float currentValue)
        {
            return currentValue * Value;
        }
    }

    public sealed class OverrideModifier : StatModifierBase
    {
        public OverrideModifier(float value, StatModifierOrder order, IStatModifierSource source) : base(value, order, source)
        {

        }

        public override float StatCalculate(float currentValue)
        {
            return Value;
        }
    }

    public sealed class StatValue
    {
        private readonly List<IStatModifier> modifiers = new List<IStatModifier>();

        private bool isDirty = true;

        private float cachedValue;

        public float BaseValue { get; private set; }
        public float FinalValue
        {
            get
            {
                if (isDirty)
                {
                    Recalculate();
                }

                return cachedValue;
            }
        }

        public StatValue(float baseValue)
        {
            BaseValue = baseValue;

            cachedValue = baseValue;
        }

        private void Recalculate()
        {
            float value = BaseValue;

            // Flat 적용 후 Percent 적용
            foreach (IStatModifier modifier in modifiers)
            {
                value = modifier.StatCalculate(value);
            }

            cachedValue = value;

            isDirty = false;
        }

        public void SetBaseValue(float value)
        {
            BaseValue = value;
            isDirty = true;
        }

        public void AddModifier(IStatModifier modifier)
        {
            modifiers.Add(modifier);

            modifiers.Sort(CompareModifier);


            isDirty = true;
        }

        public void RemoveModifier(IStatModifierSource source)
        {
            modifiers.RemoveAll(x => x.Source == source);

            isDirty = true;
        }

        public void ClearModifiers()
        {
            modifiers.Clear();

            isDirty = true;
        }

        private static int CompareModifier(IStatModifier left, IStatModifier right)
        {
            return left.Order.CompareTo(right.Order);
        }

    }

    public sealed class RuntimeStat
    {
        private readonly Dictionary<StatType, StatValue> stats = new Dictionary<StatType, StatValue>();

        public RuntimeStat(StatDataSO data)
        {
            if (data == null)
            {
                Debug.Log("StatDataSO가 null 입니다. 확인 요망");
            }

            foreach (Stat stat in data.Stats)
            {
                if (stats.ContainsKey(stat.Type))
                {
                    Debug.Log("스탯 타입 중복 오류");
                }

                stats.Add(stat.Type, new StatValue(stat.BaseValue));
            }
        }

        public StatValue GetStat(StatType statType)
        {
            if (stats.TryGetValue(statType, out StatValue statValue))
            {
                return statValue;
            }
            else
            {
                Debug.Log("없는 스탯 타입?");
                return null;
            }
        }

        public float GetValue(StatType statType)
        {
            return GetStat(statType).FinalValue;
        }

        public float GetBaseValue(StatType statType)
        {
            return GetStat(statType).BaseValue;
        }

        public void SetBaseValue(StatType statType, float value)
        {
            GetStat(statType).SetBaseValue(value);
        }

        public void AddModifier(StatType statType, IStatModifier modifier)
        {
            GetStat(statType).AddModifier(modifier);
        }

        public void RemoveBySource(IStatModifierSource source)
        {
            foreach (StatValue statValue in stats.Values)
            {
                statValue.RemoveModifier(source);
            }
        }

        public void RemoveModifier(StatType statType, IStatModifierSource source)
        {
            GetStat(statType).RemoveModifier(source);
        }

        public void ClearModifiers(StatType statType)
        {
            GetStat(statType).ClearModifiers();
        }

        public bool HasStat(StatType statType)
        {
            return stats.ContainsKey(statType);
        }
    }

    public sealed class StatModifierDefinition
    {
        [Header("StatType")]
        [SerializeField] private StatType statType;

        [Header("Modifier")]
        [SerializeField] private StatModifierType modifierType;
        [SerializeField] private float value;
        [SerializeField] private StatModifierOrder order;

        public StatType StatType => statType;

        public StatModifierType ModifierType => modifierType;

        public float Value => value;

        public StatModifierOrder Order => order;
    }

    public static class StatModifierFactory
    {
        public static IStatModifier Create(StatModifierDefinition definition, IStatModifierSource source)
        {
            return definition.ModifierType switch
            {
                StatModifierType.Add =>
                    new AddModifier(definition.Value, definition.Order, source),

                StatModifierType.Multiply =>
                    new MultiplyModifier(definition.Value, definition.Order, source),

                StatModifierType.Override =>
                    new OverrideModifier(definition.Value, definition.Order, source),

                _ => throw new ArgumentOutOfRangeException(nameof(definition.ModifierType))
            };
        }
    }















    /*public sealed class EquipmentInstance : IStatModifierSource
    {
        private readonly List<IStatModifier> modifiers = new List<IStatModifier>();

        public EquipmentDataSO DataSO { get; }

        public string SourceID => DataSO.ID;

        public string DisplayName => DataSO.DisplayName;

        public EquipmentInstance(EquipmentDataSO so)
        {
            DataSO = so;

            BuildModifiers();
        }

        public void BuildModifiers()
        {
            modifiers.Clear();

            foreach (StatModifierDefinition definition in DataSO.Modifiers)
            {
                IStatModifier modifier = StatModifierFactory.Create(definition, this);

                modifiers.Add(modifier);
            }
        }
    }*/

    public abstract class ItemInstanceBase
    {
        public Guid InstanceID { get; }

        public int Count { get; private set; }

        public bool IsLocked {  get; private set; }

        public DateTime AcquiredTime { get; }

        protected ItemInstanceBase(int count)
        {
            InstanceID = Guid.NewGuid();

            Count = count;

            AcquiredTime = DateTime.UtcNow;
        }

        public virtual void SetCount(int count)
        {
            Count = count;
        }

        public virtual void Lock()
        {
            IsLocked = true;
        }

        public virtual void Unlock()
        {
            IsLocked = false;
        }
    }

    public abstract class ItemInstance<TData> : ItemInstanceBase where TData : ItemDataSO
    {
        public TData DataSO { get; }

        protected ItemInstance(TData so, int count = 1) : base(count)
        {
            DataSO = so;
        }
    }

    public sealed class EquipmentInstance : ItemInstance<EquipmentDataSO>, IStatModifierSource
    {
        public int EnhanceLevel { get; private set; }

        public bool IsEquipped { get; internal set; }

        //public string SourceID => InstanceID.ToString();

        public string DisplayName => DataSO.DisplayName;
               

        public EquipmentInstance(EquipmentDataSO data, int count = 1) : base(data, count)
        {
        }

        internal void Equip()
        {
            IsEquipped = true;
        }

        internal void Unequip()
        {
            IsEquipped = false;
        }

        public void Enhance()
        {
            EnhanceLevel++;
        }
    }

    public sealed class Inventory
    {
        private readonly List<ItemInstanceBase> items = new List<ItemInstanceBase> ();


        public IReadOnlyList<ItemInstanceBase> Items => items;

        public event Action<ItemInstanceBase> ItemAdded;
        public event Action<ItemInstanceBase> ItemRemoved;

        public bool Add(ItemInstanceBase item)
        {
            if ( item == null)
            {
                return false;
            }

            if(items.Contains(item))
            {
                return false;
            }

            items.Add(item);

            ItemAdded?.Invoke(item);

            return true;
        }

        public bool Remove(ItemInstanceBase item)
        {
            if (item == null)
            {
                return false;
            }

            if (items.Remove(item) == false)
            {
                return false;
            }

            ItemRemoved?.Invoke(item);

            return true;
        }

        public bool Contains(ItemInstanceBase item)
        {
            return items.Contains(item);
        }

        public void Clear()
        {
            foreach (ItemInstanceBase item in items)
            {
                ItemRemoved?.Invoke(item);
            }

            items.Clear();
        }

        public T FindFirst<T>() where T : ItemInstanceBase
        {
            return items.OfType<T>().FirstOrDefault();
        }

        public IEnumerable<T> FindAll<T>() where T : ItemInstanceBase
        {
            return items.OfType<T>();
        }
    }


    public sealed class EquipmentSlot
    {
        public EquipmentSlotType SlotType { get; }

        public EquipmentInstance Equipment { get; private set; }

        public bool IsLocked { get; private set; }

        // 가독성을 위해 같은 기능 2개 보유
        public bool IsEmpty => (Equipment == null);

        public bool HasEquipment => (Equipment != null);

        public EquipmentSlot(EquipmentSlotType slotType)
        {
            SlotType = slotType;
        }

        public bool CanEquip(EquipmentInstance equipment)
        {
            if (equipment == null)
            {
                return false;
            }               

            if (IsLocked)
            {
                return false;
            }                

            return equipment.DataSO.SlotType == SlotType;
        }

        public bool Equip(EquipmentInstance equipment)
        {
            if (CanEquip(equipment) == false)
            {
                Debug.Log("장비 타입과 슬롯 타입이 다름");
                return false;
            }                

            Equipment = equipment;

            equipment.Equip();

            return true;
        }
        public EquipmentInstance Unequip()
        {
            if (Equipment == null)
            {
                return null;
            }                

            EquipmentInstance equipment = Equipment;

            equipment.Unequip();

            Equipment = null;

            return equipment;
        }
        public void Lock()
        {
            IsLocked = true;
        }
        public void Unlock()
        {
            IsLocked = false;
        }
    }


}

