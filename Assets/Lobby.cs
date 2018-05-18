using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class Lobby : NetworkLobbyManager {

	void Start () {
        StartMatchMaker();
        matchMaker.ListMatches(0, 20, string.Empty, true, 0, 0,
                               (success, extendedInfo, matchList) =>
                               {    print("List");
                                    base.OnMatchList(success, extendedInfo, matchList);

                                   if (!success)
                                       print("Match list failed : " + extendedInfo);
                                   else
                                   {
                                       if(matchList.Count > 0)
                                       {
                                           print("Successfully listed matches : " + matchList[0]);
                                           matchMaker.JoinMatch(matchList[0].networkId, string.Empty, string.Empty, string.Empty, 0, 0, OnMatchJoined);
                                       }
                                       else
                                       {
                                           matchMaker.CreateMatch("DGA FPS", 10, true, "", "", "", 0, 0, OnMatchCreate);
                                       }
                                   }
                               });
	}

    public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        print("OnMatchJoined");
        base.OnMatchJoined(success, extendedInfo, matchInfo);

        if (!success)
            print("Failed to join match : " + extendedInfo);
        else
        {
            print("Successfully joined match : " + matchInfo.networkId);
        }
    }

    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        print("OnMatchCreate");
        base.OnMatchCreate(success, extendedInfo, matchInfo);

        if (!success)
            print("Failed to create match : " + extendedInfo);
        else
            print("Successfully created match : " + matchInfo.networkId);
    }

    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        lobbyPlayer.SetActive(false);
        return base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayer, gamePlayer);
    }
}