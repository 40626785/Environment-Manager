namespace EnvironmentManager.Interfaces;

/// <summary>
/// Enables use of specification pattern, an extensible way of validating objects against business rules.
/// </summary>
public interface IThresholdRules<T>
{
    bool IsBreachedBy(T candidate);

    string ThresholdDetail();
}
