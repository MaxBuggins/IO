using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if (UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
using Steamworks;
#endif

public class SteamFriendDisplay : MonoBehaviour
{
    public TextMeshProUGUI nameDisplay;
    public RawImage avatarDisplay;

	private CSteamID steamFriend;

    void Start()
    {
		steamFriend = SteamFriends.GetFriendByIndex(Random.Range(0, SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate)), EFriendFlags.k_EFriendFlagImmediate);

		nameDisplay.text = SteamFriends.GetFriendPersonaName(steamFriend);

		FriendGameInfo_t friendGameInfo;
		SteamFriends.GetFriendGamePlayed(steamFriend, out friendGameInfo);

		if (friendGameInfo.m_gameID.ToString() == "0")
			print("none");


        avatarDisplay.texture = GetSteamImageAsTexture2D(SteamFriends.GetMediumFriendAvatar(steamFriend));
    }

	public static Texture2D GetSteamImageAsTexture2D(int iImage)
	{
		Texture2D texture = null;
		uint ImageWidth;
		uint ImageHeight;
		bool bIsValid = SteamUtils.GetImageSize(iImage, out ImageWidth, out ImageHeight);

		if (bIsValid)
		{
			byte[] Image = new byte[ImageWidth * ImageHeight * 4];

			bIsValid = SteamUtils.GetImageRGBA(iImage, Image, (int)(ImageWidth * ImageHeight * 4));
			if (bIsValid)
			{
				texture = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
				texture.LoadRawTextureData(Image);
				texture.Apply();
			}
		}

		return texture;
	}
}
