using UnityEngine;

public class PlayerMainCoordinator : MonoBehaviour
{
    public GameObject player;
    private Health healthObject;
    private PlayerMovementController playerMovementController;
    private MultiStaffObject staffMulti;

    private int baseMaxHealth;
    private float baseMovementSpeed;
    private float baseJumpForce;
    private float baseMagicPower;
    private float baseRecovery;
    private float baseProjectileSize;
    private float baseProjectileSpeed;

    public void Awake()
    {
        healthObject = player.GetComponent<Health>();
        playerMovementController = player.GetComponent<PlayerMovementController>();
        staffMulti = player.GetComponent<MultiStaffObject>();

        baseMaxHealth = GetMaxHealth();
        baseMovementSpeed = GetMovementSpeed();
        baseJumpForce = GetJumpForce();
        baseMagicPower = GetStaffPower();
        baseRecovery = GetStaffRecovery();
        baseProjectileSize = GetStaffProjectileSize();
        baseProjectileSpeed = GetStaffProjectileSpeed();
    }

    public void ResetStats()
    {
        SetMaxHealth(baseMaxHealth);
        SetMovementSpeed(baseMovementSpeed);
        SetJumpForce(baseJumpForce);
        SetStaffPower(baseMagicPower);
        SetStaffRecovery(baseRecovery);
        SetStaffProjectileSize(baseProjectileSize);
        SetStaffProjectileSpeed(baseProjectileSpeed);
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
