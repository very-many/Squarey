using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using TMPro;

public class PlayerListItem : MonoBehaviour
{
    public string PlayerName;
    public int ConnectionID;
    public ulong PlayerSteamID;
    private bool AvatarReceived;

    public TextMeshProUGUI PlayerNameText;
    public RawImage PlayerIcon;
    public TextMeshProUGUI PlayerReadyText;
    public bool Ready;


    protected Callback<AvatarImageLoaded_t> ImageLoaded;


    public void ChangeReadyStatus()
    {
        if (Ready)
        {
            PlayerReadyText.text = "Ready";
            PlayerReadyText.color = Color.green;
        }
        else
        {
            PlayerReadyText.text = "Unready";
            PlayerReadyText.color = Color.red;
        }
    }







    private void Start()
    {
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
    }

    void GetPlayerIcon()
    {
        int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)PlayerSteamID);
        if (ImageID == -1)
        {
            Debug.LogError("Failed to get avatar image for " + PlayerName);
            return;
        }
        PlayerIcon.texture = GetSteamImageAsTexture(ImageID);
    }

    public void SetPlayerValues()
    {
        PlayerNameText.text = PlayerName;
        ChangeReadyStatus();
        Debug.Log("PlayerListItem: SetPlayerValues called for " + PlayerName + " Avatar: " + AvatarReceived);
        if (!AvatarReceived)
        {
            Debug.Log("PlayerListItem: Avatar not received, calling GetPlayerIcon for " + PlayerName);
            GetPlayerIcon();
        }
    }

    private void OnImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID == PlayerSteamID)
        {
            PlayerIcon.texture = GetSteamImageAsTexture(callback.m_iImage);
            Debug.Log("PlayerListItem: Avatar received for " + PlayerName);
        }
        else
        {
            Debug.LogError("Received avatar for wrong player! " + callback.m_steamID.m_SteamID + " expected: " + PlayerSteamID);
            return;
        }
    }

    private Texture GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        AvatarReceived = true;
        return texture;
    }
}
