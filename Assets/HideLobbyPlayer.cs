using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HideLobbyPlayer : NetworkBehaviour {

	void Start () {

        NetworkLobbyPlayer[] lobbyPlayers = FindObjectsOfType<NetworkLobbyPlayer>();
        foreach(var p in lobbyPlayers)
            p.gameObject.SetActive(false);
	}
}
