using UnityEngine;

/// <summary>
/// Handles damage dealing and collision-based hit detection
/// </summary>
public class DamageDealer : MonoBehaviour
{
    [SerializeField] private float damageAmount = 15f;
    [SerializeField] private string damageDealer = "Unknown";
    [SerializeField] private float damageRadius = 2f;
    [SerializeField] private bool isAbilityDamage = false;

    private bool hasDealtDamage = false;

    public void DealDamage(GameObject target)
    {
        if (hasDealtDamage) return;

        PlayerController targetPlayer = target.GetComponent<PlayerController>();
        if (targetPlayer != null && targetPlayer.IsAlive())
        {
            targetPlayer.GetCharacterData().TakeDamage(damageAmount, damageDealer);
            hasDealtDamage = true;
            AudioManager.Instance?.PlaySFX("Hit");
            Debug.Log($"[DamageDealer] {damageDealer} dealt {damageAmount} damage to {targetPlayer.GetPlayerName()}");
        }
    }

    public void DealAOEDamage(Vector3 centerPosition)
    {
        Collider[] hits = Physics.OverlapSphere(centerPosition, damageRadius);
        foreach (Collider hit in hits)
        {
            PlayerController player = hit.GetComponent<PlayerController>();
            if (player != null && player.IsAlive())
            {
                player.GetCharacterData().TakeDamage(damageAmount, damageDealer);
            }
        }
        AudioManager.Instance?.PlaySFX("ExplosionHit");
        Debug.Log($"[DamageDealer] AOE damage at {centerPosition} with radius {damageRadius}");
    }

    public void SetDealerName(string name) => damageDealer = name;
    public void SetDamageAmount(float amount) => damageAmount = amount;
    public void ResetDamage() => hasDealtDamage = false;
}
