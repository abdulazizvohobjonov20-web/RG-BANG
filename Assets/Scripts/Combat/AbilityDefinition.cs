using UnityEngine;

/// <summary>
/// Data-driven ability definition
/// Allows abilities to be configured without code changes
/// </summary>
[CreateAssetMenu(menuName = "RG BANG/Ability Definition", fileName = "New Ability")]
public class AbilityDefinition : ScriptableObject
{
    public string abilityName = "New Ability";
    public string abilityDescription = "Ability description";
    public float cooldown = 5f;
    public float energyCost = 30f;
    public float castRange = 5f;
    public float castDuration = 0.5f;
    public float damageAmount = 20f;
    public float effectRadius = 3f;
    public Sprite abilityIcon;
    public Color effectColor = Color.white;

    public enum AbilityType
    {
        Projectile,
        AOE,
        Self,
        Debuff,
        Heal,
        Buff
    }

    public AbilityType type = AbilityType.Projectile;
    public bool isUltimate = false;
}
