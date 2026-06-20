using Mirror;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] public int maxHealth = 1000;
    [SyncVar(hook = nameof(OnHealthChanged))]
    [SerializeField] public int currentHealth;


    [SerializeField] private HealthBar healthBar;

    void OnEnable()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        float relativeAmount = (float)currentHealth / maxHealth;
        maxHealth = newMaxHealth;
        currentHealth = (int)(relativeAmount * newMaxHealth);

        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);
    }

    public void TakeDamage(int damage)
    {
        if (!isServer)
            return;

        if (currentHealth <= 0)
            return;

        currentHealth -= damage;

        TakeHitOnClient();
        if (currentHealth <= 0)
        {
            DieOnClient();
            var playerController = GetComponent<PlayerObjectController>();
            var orchestrator = GameOrchestrator.Instance;

            if (orchestrator != null && playerController != null && !orchestrator.readyPlayers.Contains(playerController))
            {
                Debug.Log(playerController);
                orchestrator.readyPlayers.Add(playerController);
            }

        }
    }

    [ClientRpc]
    private void TakeHitOnClient()
    {
        //spawn hit effect bzw. particles
        //spawn hit sound
    }

    [ClientRpc]
    private void DieOnClient()
    {
        //play death animation
        //spawn death sound

        //disable player object or trigger respawn
        gameObject.SetActive(false);
        Debug.Log("Player died");

    }

    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        // mebby animation or sound effect for health change
        healthBar.SetHealth(newHealth);
        Debug.Log($"Health changed from {oldHealth} to {newHealth}");
    }
}
