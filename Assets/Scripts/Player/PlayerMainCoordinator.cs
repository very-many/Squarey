using NUnit.Framework;
using System;
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
    private float _baseBulletDamageMultiplier;
    private float _baseBulletHealthMultiplier;

    public void Awake()
    {
        player = gameObject;

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
        _baseBulletDamageMultiplier = GetBulletDamageMult();
        _baseBulletHealthMultiplier = GetBulletHealthMult();
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
        SetBulletDamageMult(_baseBulletDamageMultiplier);
        SetBulletHealthMult(_baseBulletHealthMultiplier);
    }

    public void SetMaxHealth(int maxHealth) { healthObject.SetMaxHealth(maxHealth); }
    public int GetMaxHealth () {  return healthObject.maxHealth; }
    public int GetCurrentHealth() { return healthObject.currentHealth; }

    public void SetMovementSpeed(float movementSpeed) { playerMovementController.speed = movementSpeed; }
    public float GetMovementSpeed() { return playerMovementController.speed; }
    public float GetBaseMovementSpeed() { return _baseMovementSpeed; }

    public void SetJumpForce(float jumpForce) { playerMovementController.jumpForce = jumpForce; }
    public float GetJumpForce() { return playerMovementController.jumpForce; }
    public float GetBaseJumpForce(float jumpForce) { return _baseJumpForce; }

    public void SetStaffPower(float power) { staffMulti.MagicPower = power; }
    public float GetStaffPower() { return staffMulti.MagicPower; }

    public void SetBulletDamageMult(float bulletDamageMult) { staffMulti.bulletDamageMult = bulletDamageMult; }
    public float GetBulletDamageMult() { return staffMulti.bulletDamageMult; }

    public void SetBulletHealthMult(float bulletHealthMult) { staffMulti.bulletHealthMult = bulletHealthMult; }
    public float GetBulletHealthMult() { return staffMulti.bulletHealthMult; }


    public void SetStaffRecovery(float recovery) { staffMulti.Recovery = recovery; }
    public float GetStaffRecovery() { return staffMulti.Recovery; }

    public void SetStaffProjectileSize(float size) { staffMulti.ProjectileSize = size; }
    public float GetStaffProjectileSize() { return staffMulti.ProjectileSize; }

    public void SetStaffProjectileSpeed(float speed) { staffMulti.ProjectileSpeed = speed; }
    public float GetStaffProjectileSpeed() { return staffMulti.ProjectileSpeed; }

    public MultiStaffObject GetMultiStaffObject() { return staffMulti; }
    public Health GetHealthObject() { return healthObject; }
}
