using UnityEngine;

/// <summary>
/// Base ability system for all character abilities
/// Handles cooldowns, energy costs, and execution
/// </summary>
public class Ability : MonoBehaviour
{
    [SerializeField] protected AbilityDefinition abilityDef;
    protected float lastCastTime = -999f;
    protected PlayerController playerController;
    protected CharacterData characterData;

    protected virtual void Start()
    {
        playerController = GetComponent<PlayerController>();
        characterData = GetComponent<CharacterData>();
    }

    public bool CanCast()
    {
        float timeSinceLastCast = Time.time - lastCastTime;
        return timeSinceLastCast >= abilityDef.cooldown && 
               characterData.ConsumeEnergy(abilityDef.energyCost);
    }

    public virtual void Cast(Vector3 targetPosition)
    {
        if (!CanCast()) return;

        lastCastTime = Time.time;
        ExecuteAbility(targetPosition);
        AudioManager.Instance?.PlayAbilitySound(abilityDef.abilityName);
    }

    protected virtual void ExecuteAbility(Vector3 targetPosition)
    {
        // Override in derived classes
        Debug.Log($"[Ability] {abilityDef.abilityName} cast at {targetPosition}");
    }

    public float GetCooldownRemaining()
    {
        float remaining = abilityDef.cooldown - (Time.time - lastCastTime);
        return Mathf.Max(0, remaining);
    }

    public bool IsCooldownActive() => GetCooldownRemaining() > 0;
    public AbilityDefinition GetDefinition() => abilityDef;
}
