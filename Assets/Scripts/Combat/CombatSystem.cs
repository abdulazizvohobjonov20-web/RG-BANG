using UnityEngine;

/// <summary>
/// Central combat system for all combat interactions
/// Manages attacks, abilities, cooldowns, and damage flow
/// </summary>
public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Process basic attack from attacker to target
    /// </summary>
    public void ProcessBasicAttack(PlayerController attacker, PlayerController target)
    {
        if (!attacker.IsAlive() || !target.IsAlive()) return;

        CharacterData attackerData = attacker.GetCharacterData();
        float damage = attackerData.GetBaseAttackDamage();

        target.GetCharacterData().TakeDamage(damage, attacker.GetPlayerName());
        NetworkManager.Instance?.BroadcastDamage(attacker.GetPlayerId(), target.GetPlayerId(), damage);

        Debug.Log($"[CombatSystem] {attacker.GetPlayerName()} dealt {damage} damage to {target.GetPlayerName()}");
    }

    /// <summary>
    /// Process ability usage from character
    /// </summary>
    public void ProcessAbilityUsage(PlayerController player, AbilityDefinition ability)
    {
        if (!player.IsAlive()) return;

        CharacterData charData = player.GetCharacterData();
        if (!charData.ConsumeEnergy(ability.energyCost)) return;

        EventSystem.Instance.TriggerAbilityCast(player.GetPlayerId(), ability.abilityName);
        NetworkManager.Instance?.BroadcastAbility(player.GetPlayerId(), ability.abilityName);

        // Execute ability effect
        switch (ability.type)
        {
            case AbilityType.AOE:
                HandleAOEAbility(player, ability);
                break;
            case AbilityType.Projectile:
                HandleProjectileAbility(player, ability);
                break;
            case AbilityType.Buff:
                HandleBuffAbility(player, ability);
                break;
            case AbilityType.Debuff:
                HandleDebuffAbility(player, ability);
                break;
            case AbilityType.Heal:
                HandleHealAbility(player, ability);
                break;
        }
    }

    private void HandleAOEAbility(PlayerController player, AbilityDefinition ability)
    {
        Vector3 effectPos = player.transform.position;
        Collider[] hits = Physics.OverlapSphere(effectPos, ability.effectRadius);

        foreach (Collider hit in hits)
        {
            PlayerController targetPlayer = hit.GetComponent<PlayerController>();
            if (targetPlayer != null && targetPlayer != player && targetPlayer.IsAlive())
            {
                targetPlayer.GetCharacterData().TakeDamage(ability.damageAmount, player.GetPlayerName());
            }
        }

        // Spawn visual effect
        SpawnEffectAtPosition(effectPos, ability.effectColor, ability.effectRadius);
    }

    private void HandleProjectileAbility(PlayerController player, AbilityDefinition ability)
    {
        // Projectiles will be spawned as separate objects
        Debug.Log($"[CombatSystem] Projectile ability: {ability.abilityName}");
    }

    private void HandleBuffAbility(PlayerController player, AbilityDefinition ability)
    {
        Debug.Log($"[CombatSystem] Buff ability: {ability.abilityName}");
    }

    private void HandleDebuffAbility(PlayerController player, AbilityDefinition ability)
    {
        Debug.Log($"[CombatSystem] Debuff ability: {ability.abilityName}");
    }

    private void HandleHealAbility(PlayerController player, AbilityDefinition ability)
    {
        player.GetCharacterData().Heal(ability.damageAmount);
        SpawnEffectAtPosition(player.transform.position, Color.green, 2f);
    }

    private void SpawnEffectAtPosition(Vector3 position, Color color, float scale)
    {
        // Create simple visual feedback
        GameObject effectSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        effectSphere.transform.position = position;
        effectSphere.transform.localScale = Vector3.one * scale;
        effectSphere.GetComponent<Renderer>().material.color = color;
        Destroy(effectSphere.GetComponent<Collider>());
        Destroy(effectSphere, 0.3f);
    }
}
