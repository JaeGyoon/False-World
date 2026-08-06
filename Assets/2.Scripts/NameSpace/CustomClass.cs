using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;


namespace FalseWorld
{
    /* Bootstrap 관련 클래스
     * 
     
     
     
     */

    public sealed class GameCompositionRoot
    {
        // Service
        private ISceneLoader sceneLoader;
        private IAddressableService addressableService;
        private ISaveService saveService;
               

        // Managers

        // Factories

        // GamePlay

        // Modules
        

        public async Task InitializeAsync()
        {
            await CreateServiceAsync();

            await LoadLobbyAsync();
        }

        private async Task CreateServiceAsync()
        {
            sceneLoader = new UnitySceneLoader();
            addressableService = new AddressableService();
            saveService = new JsonSaveService();

            await addressableService.InitializeAsync();
            await saveService.InitializeAsync();

        }

        private async Task LoadLobbyAsync()
        {
            await sceneLoader.LoadScene(SceneName.Lobby);
        }
    }

    


    /* Core System
     *     
     
     */

    /* Service
     * 
     * 
     * 
     * 
     */

    public sealed class UnitySceneLoader : ISceneLoader
    {
        public async Task LoadScene(SceneName sceneName)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName.ToString());

            while (operation.isDone == false)
            {
                await Task.Yield();
            }
        }
    }

    public sealed class AddressableService : IAddressableService
    {
        public async Task InitializeAsync()
        {
            AsyncOperationHandle handle = Addressables.InitializeAsync();

            await handle.Task;
        }

        public Task<T> LoadAssetAsync<T>(AssetReference reference) where T : UnityEngine.Object
        {
            AsyncOperationHandle<T> handle = reference.LoadAssetAsync<T>();

            return handle.Task;
        }

        public async Task<GameObject> InstantiateAsync(AssetReference reference)
        {
            AsyncOperationHandle<GameObject> handle = reference.InstantiateAsync();

            return await handle.Task;
        }        

        public void Release(object asset)
        {
            Addressables.Release(asset);
        }

        public void ReleaseInstance(GameObject instance)
        {
            Addressables.ReleaseInstance(instance);
        }
    }

    [SerializeField]
    public sealed class PlayerSaveData
    {
        public string SelectedHeroID;

        public string SelectedStageID;
    }

    public static class SavePath
    {
        public static string PlayerSavePath => Path.Combine(Application.persistentDataPath, "player_save.json");
    }

    public sealed class JsonSaveService : ISaveService
    {
        public PlayerSaveData CurrentSaveData {  get; private set; }

        private bool isDirty;

        public async Task InitializeAsync()
        {
            if ( File.Exists(SavePath.PlayerSavePath))
            {
                string json = await File.ReadAllTextAsync(SavePath.PlayerSavePath);

                CurrentSaveData = JsonUtility.FromJson<PlayerSaveData>(json);
            }
            else
            {
                Debug.Log("새로운 플레이어 데이터 생성!");

                CurrentSaveData = new PlayerSaveData();

                await SaveAsync();
            }
        }

        public async Task SaveAsync()
        {
            if ( (isDirty == false) && File.Exists(SavePath.PlayerSavePath) )
            {
                return;
            }

            string json = JsonUtility.ToJson(CurrentSaveData, true);

            await File.WriteAllTextAsync(SavePath.PlayerSavePath, json);

            isDirty = false;
        }

        public void MarkDirty()
        {
            isDirty = true;
        }

        public string GetSelectedHero()
        {
            return CurrentSaveData.SelectedHeroID;
        }

        public void SetSelectedHero(string heroID)
        {
            if (CurrentSaveData.SelectedHeroID == heroID)
            {
                return;
            }

            CurrentSaveData.SelectedHeroID = heroID;

            isDirty = true;
        }

        public string GetSelectedStage()
        {
            return CurrentSaveData.SelectedStageID;
        }

        public void SetSelectedStage(string stageID)
        {
            if (CurrentSaveData.SelectedStageID == stageID)
            {
                return;
            }

            CurrentSaveData.SelectedStageID = stageID;

            isDirty = true;
        }
    }

    /* 로비 파트
     * 
     * 
     * 
     * 
     * 
     */















   /* public sealed class AssetHandle<T> where T : UnityEngine.Object
    {
        public AsyncOperationHandle<T> Handle { get; }

        public T Asset { get; }

        public int ReferenceCount { get; private set; }

        public AssetHandle(AsyncOperationHandle<T> handle)
        {
            Handle = handle;

            ReferenceCount = 1;
        }

        public void Retain()
        {
            ReferenceCount++;
        }

        public bool Release()
        {
            ReferenceCount--;

            return (ReferenceCount <= 0);
        }
    }*/














    public sealed class FactoryRegistry
    {
        private readonly Dictionary<Type, IRuntimeFactory> factories = new Dictionary<Type, IRuntimeFactory>();

        public void Register<TFactory>(TFactory factory) where TFactory : class, IRuntimeFactory
        {
            factories[typeof(TFactory)] = factory;
        }

        public TFactory Get<TFactory>() where TFactory : class, IRuntimeFactory
        {
            if (factories.TryGetValue(typeof(TFactory), out IRuntimeFactory factory))
            {
                return factory as TFactory;
            }
            else
            {
                Debug.Log("타입 오류");
                return null;
            }

        }
    }

    public static class FactoryBootstrap
    {
        public static FactoryRegistry Initialize()
        {
            FactoryRegistry registry = new FactoryRegistry();

            // Item 팩토리 
            ItemFactory itemFactory = new ItemFactory();
            ItemFactoryRegistrar.Register(itemFactory);
            registry.Register(itemFactory);


            return registry;
        }
    }


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

    /*public sealed class AddressableLoader
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
    }*/

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

    /*public abstract class Character<TData> : Entity<TData> where TData : CharacterDataSO
    {
        public CharacterComponents Components { get; protected set; }

        public RuntimeStat RuntimeStat => Components.RuntimeStat;

        public Inventory Inventory => Components.Inventory;

        public Equipment Equipment => Components.Equipment;

        public override void Initialize(TData dataSO)
        {
            base.Initialize(dataSO);

            Components = CharacterComponentFactory.CreateCharacter(dataSO);

            Components.Initialize();
        }

        public override void Release()
        {
            Components.Release();
        }
    }*/

    /*public sealed class Hero : Character<HeroDataSO>
    {
        
        public override void Initialize(HeroDataSO dataSO)
        {
            base.Initialize(dataSO);

        }

        public override void Release()
        {
            throw new NotImplementedException();
        }
    }

    public sealed class Enemy : Character<EnemyDataSO>
    {
        public override void Initialize(EnemyDataSO dataSO)
        {
            base.Initialize(dataSO);

        }

        public override void Release()
        {
            throw new NotImplementedException();
        }
    }*/

    /* 스탯 클래스 모음
     
     
     */




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
        private readonly Dictionary<StatType, StatValue> stats;

        public RuntimeStat(IReadOnlyDictionary<StatType, StatValue> statDict)
        {            
            if (statDict == null )
            {
                Debug.Log("statDict : null");
            }

            stats = new Dictionary<StatType, StatValue>(statDict);
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

    public static class RuntimeStatFactory
    {
        public static RuntimeStat CreateRuntimeStat(CharacterDataSO dataSO)
        {
            Dictionary<StatType, StatValue> stats = new Dictionary<StatType, StatValue>();

            foreach (Stat stat in dataSO.Stats)
            {
                stats.Add(stat.Type, new StatValue(stat.BaseValue));
            }

            return new RuntimeStat(stats);
        }
    }













    /* Item에 관련된 클래스
     * 
     * 
    */

    public abstract class ItemInstanceBase
    {
        public Guid Guid { get; }

        public abstract ItemDataSO DataSO { get; }

        public int StackCount { get; private set; }

        protected ItemInstanceBase(int count)
        {
            Guid = Guid.NewGuid();
            StackCount = count;
        }

        protected ItemInstanceBase(Guid guid, int stackCount)
        {
            Guid = guid;
            StackCount = stackCount;
        }

        public bool IsFullStack => (StackCount >= DataSO.MaxStack);
        public bool isEmpty     => (StackCount <= 0);

        public void AddStack(int amount)
        {
            if ( DataSO.Stackable == false)
            {
                Debug.Log("Stack 불가 Item 입니다.");
                return;
            }

            StackCount = Math.Min(DataSO.MaxStack, StackCount + amount);
        }

        public void RemoveStack(int amount)
        {
            if (DataSO.Stackable == false)
            {
                Debug.Log("Stack 불가 Item 입니다.");
                return;
            }

            StackCount = Math.Max(0, StackCount - amount);
        }

    }

    public abstract class ItemInstance<TData> : ItemInstanceBase where TData : ItemDataSO
    {
        private readonly TData definition;

        public TData Definition => definition;

        public override ItemDataSO DataSO => definition;

        /*public DateTime AcquiredTime { get; }*/

        protected ItemInstance(TData dataSO, int count = 1) : base(count)
        {
            definition = dataSO;
        }

        // 로드 기능을 사용할 때 기존 Guid 복원
        protected ItemInstance(TData dataSO, int count, Guid guid) : base(guid, count)
        {
            definition = dataSO;
        }
    }

    public sealed class InventorySlot
    {
        public int Index { get; }

        public ItemInstanceBase Item { get; private set; }

        public bool IsEmpty => (Item == null);

        public InventorySlot(int index)
        {
            Index = index;
        }

        /*internal void SetItem(ItemInstanceBase item)*/
        private void SetItem(ItemInstanceBase item)
        {
            Item = item;
        }

        public ItemInstanceBase RemoveItem()
        {
            ItemInstanceBase item = Item;

            Item = null;

            return item;
        }
    }

    public sealed class Inventory
    {
        private readonly List<InventorySlot> slots;

        public IReadOnlyList<InventorySlot> Slots => slots;

        public int Capacity => slots.Count;


        public event Action<ItemInstanceBase> ItemAdded;
        public event Action<ItemInstanceBase> ItemRemoved;

        public Inventory(int capacity)
        {
            if (capacity <= 0)
            {
                Debug.Log("인벤토리 크기가 0입니다.");
            }

            slots = new List<InventorySlot>();

            for (int i = 0; i < capacity; i++)
            {
                slots.Add(new InventorySlot(i));
            }
        }

        public InventorySlot GetSlot(int index)
        {
            return slots[index];
        }

        public bool ContainsItem(ItemInstanceBase item)
        {
            foreach (InventorySlot slot in slots)
            {
                if (slot.Item == item)
                {
                    return true;
                }
            }

            return false;
        }

        public int FindEmptySlot()
        {
            foreach (InventorySlot slot in slots)
            {
                if (slot.IsEmpty)
                {
                    return slot.Index;
                }                    
            }

            // 매직 넘버 사용? 다른 변경 방법 
            return -1;
        }

        public InventorySlot FindSlot(ItemInstanceBase item)
        {
            foreach (InventorySlot slot in slots)
            {
                if (slot.Item == item)
                {
                    return slot;
                }                    
            }

            return null;
        }
    }

    public sealed class ItemFactory : IRuntimeFactory
    {
        private readonly Dictionary<Type, Func<ItemDataSO, ItemInstanceBase>> createMethods = new();

        public void Register<TData>(Func<TData, ItemInstanceBase> creator) where TData : ItemDataSO
        {
            createMethods[typeof(TData)] = (data => creator((TData)data));
        }

        public ItemInstanceBase Create(ItemDataSO dataSO)
        {
            if ( dataSO == null)
            {
                Debug.Log("data SO가 null 입니다.");
                return null;
            }

            if (createMethods.TryGetValue(dataSO.GetType(), out Func<ItemDataSO, ItemInstanceBase> createMethod))
            {
                return createMethod(dataSO);
            }
            else
            {
                Debug.Log("data SO 타입 오류");
                return null;
            }
        }

        public T_item Create<T_item>(ItemDataSO so) where T_item : ItemInstanceBase
        {
            return (T_item)Create(so);
        }
    }
    public static class ItemFactoryRegistrar
    {
        public static void Register(ItemFactory factory)
        {
            /*factory.Register<EquipmentDataSO>(
                data => new EquipmentInstance(data));*/

            /*factory.Register<ConsumableData>(
                data => new ConsumableInstance(data));

            factory.Register<MaterialData>(
                data => new MaterialInstance(data));*/
        }
    }


    /*public sealed class EquipmentInstance : ItemInstance<EquipmentDataSO>
    {
        public int EnhanceLevel { get; private set; }                      

        public EquipmentInstance(EquipmentDataSO data) : base(data)
        {

        }

        public EquipmentInstance(EquipmentDataSO so, int count, Guid guid, int enhanceLevel) : base (so, count, guid)
        {
            EnhanceLevel = enhanceLevel;
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

    public sealed class Equipment
    {
        private readonly Dictionary<EquipmentSlotType, EquipmentSlot> slotDict;

        private readonly Inventory inventory;

        private readonly RuntimeStat runtimeStat;

        public  Equipment(Inventory inven, RuntimeStat stat)
        {
            inventory = inven;

            runtimeStat = stat;

            slotDict = new Dictionary<EquipmentSlotType, EquipmentSlot>();

            foreach (EquipmentSlotType slotType in Enum.GetValues(typeof(EquipmentSlotType)))
            {
                slotDict.Add(slotType, new EquipmentSlot(slotType) );
            }
        }

        public bool Equip(EquipmentInstance equipment)
        {
            EquipmentSlot slot = slotDict[equipment.DataSO.SlotType];

            if (slot.IsEmpty == false)
            {
                return false;
            }

            if (inventory.Remove(equipment))
            {
                return false;
            }

            slot.Equip(equipment);

            ApplyModifiers(equipment);

            return true;
        }

        private void ApplyModifiers(EquipmentInstance equipment)
        {
            foreach (IStatModifier modifier in equipment.modi)
            {
                _runtimeStat.AddModifier(
                    modifier.StatType,
                    modifier);
            }
        }
    } */

    /* Character 에 관련된 클래스 
     CharacterComponents : 캐릭터들이 공통으로 가지고 있는 Runtime 객체들 보관

     
    */

    /*public sealed class CharacterComponents
    {
        public RuntimeStat RuntimeStat { get; }
         
        public Inventory Inventory { get; }

        public Equipment Equipment { get; }

        public CharacterComponents(RuntimeStat runtimeStat, Inventory inventory, Equipment equipment)
        {
            RuntimeStat = runtimeStat;
            Inventory = inventory;
            Equipment = equipment;
        }

        public void Initialize()
        {

        }

        public void Release()
        {

        }

    }

    public static class CharacterComponentFactory
    {
        public static CharacterComponents CreateCharacter(CharacterDataSO dataSO)
        {
            RuntimeStat runtimeStat = RuntimeStatFactory.CreateRuntimeStat(dataSO);

            Inventory inventory = new Inventory();

            Equipment equipment = new Equipment(inventory, runtimeStat);

            return new CharacterComponents(runtimeStat, inventory, equipment);
        }
    }*/



    /*  Hero 에 관련된 클래스 
     *  
     *     
     */





























}

