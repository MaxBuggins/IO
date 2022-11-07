
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//WebGL Be Stupid true
#if (UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
using Steamworks;
#endif

public class UI_MainMenu : MonoBehaviour
{
    public TMP_InputField usernameTextBox;

	public MyNetworkManager myNetworkManager;

	private void Start()
    {
#if (UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
		usernameTextBox.text = SteamFriends.GetPersonaName();

#endif
	}


	public void JoinGame()
	{
#if (UNITY_WEBGL)
		myNetworkManager.StartClient();
#endif

	}

	public void Quit()
    {
		Application.Quit();
    }
}
