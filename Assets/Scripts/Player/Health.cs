using Mirror;
using System;
using Unity.VisualScripting;
using UnityEngine;
using SmallHedge.SoundManager;

public class Health : NetworkBehaviour
{
    [SyncVar]
    [SerializeField] public int maxHealth = 300;
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
        SetMaxHealthBase(newMaxHealth);
        CmdSetMaxHealth(newMaxHealth);
    }

    [Command]
    private void CmdSetMaxHealth(int newMaxHealth)
    {
        SetMaxHealthBase(newMaxHealth);
    }

    private void SetMaxHealthBase(int newMaxHealth)
    {
        float relativeAmount = (float)currentHealth / maxHealth;
        maxHealth = newMaxHealth;
        currentHealth = (int)(relativeAmount * newMaxHealth);

        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);
    }

    public void ResetHealth()
    {
        if (!isServer)
            return;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (!isServer)
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

    public void Kill()
    {
        if (!isServer)
            return;

        TakeDamage(maxHealth);
    }

    [ClientRpc]
    private void TakeHitOnClient()
    {
        //spawn hit effect bzw. particles
        //spawn hit sound
        SoundManager.PlaySound(SoundType.Hit);
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