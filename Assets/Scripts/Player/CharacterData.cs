using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Base character data structure
/// Contains all stats and abilities for a character
/// Data-driven design allows easy addition of new characters
/// </summary>
public class CharacterData : MonoBehaviour
{
    [SerializeField] protected string characterId = "default";
    [SerializeField] protected string characterName = "Character";
    [SerializeField] protected CharacterClass characterClass = CharacterClass.Speedster;
    [SerializeField] protected Color characterColor = Color.white;

    [Header("Base Stats")]
    [SerializeField] protected float maxHealth = GameConstants.DEFAULT_MAX_HEALTH;
    [SerializeField] protected float maxEnergy = GameConstants.DEFAULT_MAX_ENERGY;
    [SerializeField] protected float movementSpeed = GameConstants.DEFAULT_MOVEMENT_SPEED;
    [SerializeField] protected float sprintSpeed = GameConstants.DEFAULT_SPRINT_SPEED;
    [SerializeField] protected float dashSpeed = GameConstants.DEFAULT_DASH_SPEED;
    [SerializeField] protected float baseAttackDamage = 15f;

    [Header("Ability Data")]
    [SerializeField] protected AbilityDefinition basicAttackAbility;
    [SerializeField] protected AbilityDefinition specialAbility;
    [SerializeField] protected AbilityDefinition ultimateAbility;

    [Header("Passive Ability")]
    [SerializeField] protected string passiveAbilityName = "Default Passive";
    [SerializeField] protected string passiveAbilityDescription = "No additional passive ability";

    // Runtime stats
    protected float currentHealth;
    protected float currentEnergy;
    protected bool isAlive = true;

    protected PlayerController playerController;
    protected Animator animator;
    protected AudioSource audioSource;

    private float lastAttackTime = 0f;
    private float lastAbilityTime = 0f;
    private float lastUltimateTime = 0f;

    protected virtual void Awake()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        currentHealth = maxHealth;
        currentEnergy = maxEnergy / 2f; // Start with half energy
    }

    // Getters
    public string GetCharacterId() => characterId;
    public string GetCharacterName() => characterName;
    public CharacterClass GetCharacterClass() => characterClass;
    public Color GetCharacterColor() => characterColor;

    public float GetMaxHealth() => maxHealth;
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxEnergy() => maxEnergy;
    public float GetCurrentEnergy() => currentEnergy;
    public float GetMovementSpeed() => movementSpeed;
    public float GetSprintSpeed() => sprintSpeed;
    public float GetDashSpeed() => dashSpeed;
    public float GetBaseAttackDamage() => baseAttackDamage;
    public bool IsAlive() => isAlive;

    // Health Management
    public virtual void TakeDamage(float damageAmount, string damageDealer = "")
    {
        if (!isAlive) return;

        currentHealth -= damageAmount;
        if (currentHealth < 0) currentHealth = 0;

        EventSystem.Instance.TriggerPlayerDamaged(playerController.GetPlayerId());

        if (currentHealth <= 0)
        {
            Die(damageDealer);
        }
    }

    public virtual void Heal(float healAmount)
    {
        if (!isAlive) return;

        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    protected virtual void Die(string killedBy)
    {
        isAlive = false;
        animator?.SetBool("IsDead", true);
        EventSystem.Instance.TriggerPlayerEliminated(playerController.GetPlayerId(), killedBy);
    }

    // Energy Management
    public virtual void UpdateEnergy(float deltaTime)
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += GameConstants.ENERGY_REGEN_PER_SECOND * deltaTime;
            if (currentEnergy > maxEnergy) currentEnergy = maxEnergy;
        }
    }

    public bool ConsumeEnergy(float amount)
    {
        if (currentEnergy >= amount)
        {
            currentEnergy -= amount;
            return true;
        }
        return false;
    }

    // Attack Methods
    public virtual bool CanUseBasicAttack()
    {
        return isAlive && (Time.time - lastAttackTime) >= GameConstants.BASIC_ATTACK_COOLDOWN;
    }

    public virtual bool CanUseAbility()
    {
        return isAlive && (Time.time - lastAbilityTime) >= GameConstants.ABILITY_COOLDOWN_BASE &&
               ConsumeEnergy(GameConstants.ABILITY_ENERGY_COST);
    }

    public virtual bool CanUseUltimate()
    {
        return isAlive && (Time.time - lastUltimateTime) >= GameConstants.ULTIMATE_COOLDOWN_BASE &&
               ConsumeEnergy(GameConstants.ULTIMATE_ENERGY_COST);
    }

    public virtual void ExecuteBasicAttack(Vector3 targetPosition)
    {
        if (!CanUseBasicAttack()) return;

        lastAttackTime = Time.time;
        animator?.SetTrigger("BasicAttack");
        AudioManager.Instance?.PlaySFX("BasicAttack");
    }

    public virtual void ExecuteAbility(Vector3 targetPosition)
    {
        if (!CanUseAbility()) return;

        lastAbilityTime = Time.time;
        animator?.SetTrigger("Ability");
        EventSystem.Instance.TriggerAbilityCast(playerController.GetPlayerId(), specialAbility?.abilityName ?? "Unknown");
        AudioManager.Instance?.PlayAbilitySound(characterId);
    }

    public virtual void ExecuteUltimate(Vector3 targetPosition)
    {
        if (!CanUseUltimate()) return;

        lastUltimateTime = Time.time;
        animator?.SetTrigger("Ultimate");
        EventSystem.Instance.TriggerAbilityCast(playerController.GetPlayerId(), ultimateAbility?.abilityName ?? "Unknown");
        AudioManager.Instance?.PlayAbilitySound(characterId + "_ultimate");
    }

    // Ability Getters
    public AbilityDefinition GetBasicAttack() => basicAttackAbility;
    public AbilityDefinition GetSpecialAbility() => specialAbility;
    public AbilityDefinition GetUltimateAbility() => ultimateAbility;
    public string GetPassiveAbilityName() => passiveAbilityName;
    public string GetPassiveAbilityDescription() => passiveAbilityDescription;

    // Respawn
    public virtual void Respawn(Vector3 spawnPosition)
    {
        isAlive = true;
        currentHealth = maxHealth;
        currentEnergy = maxEnergy / 2f;
        transform.position = spawnPosition;
        animator?.SetBool("IsDead", false);
        EventSystem.Instance.TriggerPlayerRespawned(playerController.GetPlayerId());
    }
}
