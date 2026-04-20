using UnityEngine;
using Steamworks;
using UnityEngine.UI;
using Mirror;
using TMPro;

[DefaultExecutionOrder(1)]
public class CharacterCosmetics : MonoBehaviour
{
    private const string PrefKeyCosmeticIndex = "CurrentCosmeticIndex";

    public int CurrentCosmeticIndex = 0;
    public Color[] PlayerCosmetics;
    public Image CurrentCosmeticImage;
    public TextMeshProUGUI CurrentCosmeticText;

    private void Start()
    {
        CurrentCosmeticIndex = PlayerPrefs.GetInt(PrefKeyCosmeticIndex, 0);
        ApplyCosmeticToUI(CurrentCosmeticIndex);
    }

    private void ApplyCosmeticToUI(int index)
    {
        if (PlayerCosmetics == null || PlayerCosmetics.Length == 0)
            return;

        CurrentCosmeticIndex = Mathf.Clamp(index, 0, PlayerCosmetics.Length - 1);
        PlayerPrefs.SetInt(PrefKeyCosmeticIndex, CurrentCosmeticIndex);

        if (CurrentCosmeticImage != null)
            CurrentCosmeticImage.color = PlayerCosmetics[CurrentCosmeticIndex];

        if (CurrentCosmeticText != null)
            CurrentCosmeticText.text = PlayerCosmetics[CurrentCosmeticIndex].ToString();
    }

    private void SetCosmetic(int index)
    {
        bool ServerUpdateSuccess = TrySendToServer(index);
        if (ServerUpdateSuccess)
        {
            ApplyCosmeticToUI(CurrentCosmeticIndex);
        }
        else
        {
            Debug.LogWarning("Failed to update cosmetic on server. Cosmetic change will not be applied.");
        }
    }


    public void NextCosmetic()
    {
        if (PlayerCosmetics == null || PlayerCosmetics.Length == 0)
        {
            Debug.LogWarning("No cosmetics available to select.");
            return;
        }
        CurrentCosmeticIndex = (CurrentCosmeticIndex + 1) % PlayerCosmetics.Length;

        SetCosmetic(CurrentCosmeticIndex);
    }

    public void PreviousCosmetic()
    {
        if (PlayerCosmetics == null || PlayerCosmetics.Length == 0)
        {
            Debug.LogWarning("No cosmetics available to select.");
            return;
        }
        CurrentCosmeticIndex = (CurrentCosmeticIndex - 1 + PlayerCosmetics.Length) % PlayerCosmetics.Length;

        SetCosmetic(CurrentCosmeticIndex);
    }

    private bool TrySendToServer(int index)
    {
        var lobby = LobbyController.Instance;
        if (lobby == null || lobby.LocalPlayerController == null)
            return false;
        if (!lobby.LocalPlayerController.isOwned)
            return false;

        lobby.LocalPlayerController.CmdUpdatePlayerCosmetic(index);
        return true;
    }
}
