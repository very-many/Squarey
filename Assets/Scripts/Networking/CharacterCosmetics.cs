using UnityEngine;
using Steamworks;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class CharacterCosmetics : MonoBehaviour
{
    public int CurrentCosmeticIndex = 0;
    public Color[] PlayerCosmetics;
    public Image CurrentCosmeticImage;
    public TextMeshProUGUI CurrentCosmeticText;

    private void Start()
    {
        CurrentCosmeticIndex = PlayerPrefs.GetInt("CurrentCosmeticIndex", 0);
        SetCosmetic(CurrentCosmeticIndex);
    }

    private void SetCosmetic(int index)
    {
        PlayerPrefs.SetInt("currentColourIndex", CurrentCosmeticIndex);
        CurrentCosmeticImage.color = PlayerCosmetics[CurrentCosmeticIndex];
        CurrentCosmeticText.text = PlayerCosmetics[CurrentCosmeticIndex].ToString();
        LobbyController.Instance.LocalPlayerController.CmdUpdatePlayerCosmetic(index);
    }


    public void NextCosmetic()
    {
        if (CurrentCosmeticIndex < PlayerCosmetics.Length - 1)
        {
            CurrentCosmeticIndex++;
        }
        else
        {
            CurrentCosmeticIndex = 0;
        }
        SetCosmetic(CurrentCosmeticIndex);
    }

    public void PreviousCosmetic()
    {
        if (CurrentCosmeticIndex > 0)
        {
            CurrentCosmeticIndex--;
        }
        else
        {
            CurrentCosmeticIndex = PlayerCosmetics.Length - 1;
        }
        SetCosmetic(CurrentCosmeticIndex);
    }
}
