using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCosmeticController : NetworkBehaviour
{
    public SpriteRenderer BodyRenderer;
    public SpriteRenderer CapeRenderer;
    public Color[] PlayerCosmetics;
    [Space]
    [Header("Child Player Object")]
    public GameObject PlayerObject;

    private void Start()
    {
    }

    public void PlayerCosmeticsSetup()
    {
        int idx = GetComponent<PlayerObjectController>().PlayerCosmetic;
        Color bodyColor = Color.white;
        if (PlayerCosmetics != null && PlayerCosmetics.Length > 0 && idx >= 0 && idx < PlayerCosmetics.Length)
            bodyColor = PlayerCosmetics[idx];

        BodyRenderer.color = bodyColor;

        // set cape to complementary color
        if (CapeRenderer != null)
        {
            float h, s, v;
            Color.RGBToHSV(bodyColor, out h, out s, out v);
            float compH = (h + 0.5f) % 1f;
            s = Mathf.Max(0.9f, s);
            Color capeColor = Color.HSVToRGB(compH, s, v);
            capeColor.a = bodyColor.a;  // keep alpha
            CapeRenderer.color = capeColor;
        }

        //Debug.Log("Set color to " + PlayerSprite.color);
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().name == "Game")
        {
            if (PlayerObject.activeSelf == false)
            {
                PlayerCosmeticsSetup();
            }
        }
    }
}
