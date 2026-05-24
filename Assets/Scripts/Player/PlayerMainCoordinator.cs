using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMainCoordinator : MonoBehaviour
{
    public GameObject player;
    
    public List<Upgrade> Upgrades = new List<Upgrade>();
    public List<Spell> SpellPool = new List<Spell>();

    private Health healthObject;
    private PlayerMovementController playerMovementController;
    private MultiStaffObject staffMulti;

    private int _baseMaxHealth;
    private float _baseMovementSpeed;
    private float _baseJumpForce;
    private float _baseMagicPower;
    private float _baseRecovery;
    private float _baseProjectileSize;
    private float _baseProjectileSpeed;

    public void Awake()
    {
        healthObject = player.GetComponent<Health>();
        playerMovementController = player.GetComponent<PlayerMovementController>();
        staffMulti = player.GetComponent<MultiStaffObject>();

        _baseMaxHealth = GetMaxHealth();
        _baseMovementSpeed = GetMovementSpeed();
        _baseJumpForce = GetJumpForce();
        _baseMagicPower = GetStaffPower();
        _baseRecovery = GetStaffRecovery();
        _baseProjectileSize = GetStaffProjectileSize();
        _baseProjectileSpeed = GetStaffProjectileSpeed();
    }

    public void ApplyUpgrades()
    {
        ResetStats();

        for (int i = 0; i < Upgrades.Count - 1; i++)
        {
            Upgrades[i].ReApplyUpgradeStats(this);
        }

        Upgrades.Last().ApplyUpgrade(this);
    }

    public void ResetStats()
    {
        SetMaxHealth(_baseMaxHealth);
        SetMovementSpeed(_baseMovementSpeed);
        SetJumpForce(_baseJumpForce);
        SetStaffPower(_baseMagicPower);
        SetStaffRecovery(_baseRecovery);
        SetStaffProjectileSize(_baseProjectileSize);
        SetStaffProjectileSpeed(_baseProjectileSpeed);
    }

    public void SetMaxHealth(int maxHealth) { healthObject.maxHealth = maxHealth; }
    public int GetMaxHealth () {  return healthObject.maxHealth; }
    public int GetCurrentHealth() { return healthObject.currentHealth; }

    public void SetMovementSpeed(float movementSpeed) { playerMovementController.speed = movementSpeed; }
    public float GetMovementSpeed() { return playerMovementController.speed; }

    public void SetJumpForce(float jumpForce) { playerMovementController.jumpForce = jumpForce; }
    public float GetJumpForce() { return playerMovementController.jumpForce; }

    public void SetStaffPower(float power) { staffMulti.MagicPower = power; }
    public float GetStaffPower() { return staffMulti.MagicPower; }

    public void SetStaffRecovery(float recovery) { staffMulti.Recovery = recovery; }
    public float GetStaffRecovery() { return staffMulti.Recovery; }

    public void SetStaffProjectileSize(float size) { staffMulti.ProjectileSize = size; }
    public float GetStaffProjectileSize() { return staffMulti.ProjectileSize; }

    public void SetStaffProjectileSpeed(float speed) { staffMulti.ProjectileSpeed = speed; }
    public float GetStaffProjectileSpeed() { return staffMulti.ProjectileSpeed; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
