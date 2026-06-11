using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCosmeticController : NetworkBehaviour
{
    public SpriteRenderer PlayerSprite;
    public Color[] PlayerCosmetics;
    [Space]
    [Header("Child Player Object")]
    public GameObject PlayerObject;

    private void Start()
    {
        PlayerSprite = PlayerObject.GetComponentInChildren<SpriteRenderer>();
    }

    public void PlayerCosmeticsSetup()
    {
        PlayerSprite.color = PlayerCosmetics[GetComponent<PlayerObjectController>().PlayerCosmetic];
        //Debug.Log("Set color to " + PlayerSprite.color);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (PlayerObject.activeSelf == false)
            {
                PlayerCosmeticsSetup();
            }
        }
    }
}
