namespace FalseWorld
{
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
}