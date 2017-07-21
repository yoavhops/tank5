/// <summary>
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// This script automatically connects to Photon (using the settings file),
/// tries to join a random room and creates one if none was found (which is ok).
/// </summary>
public class ConnectAndJoinRandomYoav : Photon.MonoBehaviour
{
	/// <summary>Connect automatically? If false you can set this to true later on or call ConnectUsingSettings in your own scripts.</summary>
	public int amount_of_players = 1;
	
	public byte Version = 1;
    public bool game_scene = false;
    public bool connect_scene = false;
    private int player_in_game = 0;
    private float exit_time = -1.0f;
    private float exit = -1.0f;
    public int fee;
    private bool first_run = true;
    public bool connect = false;
	
	public virtual void Start()
	{
        
	}

    [PunRPC]
    void ready_RPC()
    {
        player_in_game++;
        Debug.Log(player_in_game);
        if ( player_in_game == amount_of_players && PhotonNetwork.isMasterClient)
        {
            GetComponent<manager_script>().start_game();
        }
    }

    public virtual void Update()
	{

        if (first_run)
        {
            first_run = false;
            Debug.Log("HEY!");
            if (game_scene)
            {
                if (PhotonNetwork.connected)
                {
                    Debug.Log("I am connected");
                    photonView.RPC("ready_RPC", PhotonTargets.All);
                }
                else
                {
                    exit = exit_time;
                    //Application.LoadLevel("menus");
                }
            }
            PhotonNetwork.autoJoinLobby = false;    // we join randomly. always. no need to join a lobby to get the list of rooms.
            return;
        }

        if (exit > 0)
        {
            exit -= Time.deltaTime;
            if (exit < 0)
            {
                Application.LoadLevel("menus");
            }
        }

        if (PhotonNetwork.connectionState == ConnectionState.Disconnected)
        {
            Debug.Log("Update() was called by Unity. Scene is loaded. Let's connect to the Photon Master Server. Calling: PhotonNetwork.ConnectUsingSettings();");
            PhotonNetwork.ConnectUsingSettings(Version + "." + Application.loadedLevel);
        }

        if (PhotonNetwork.connectedAndReady && connect)
        {
            Debug.Log("Trying to join room"); 
            
            var options = new RoomOptions()
            {
                MaxPlayers = 2,
            };

            PhotonNetwork.JoinOrCreateRoom(fee.ToString(), options, TypedLobby.Default);

        }

	}
	
	// to react to events "connected" and (expected) error "failed to join random room", we implement some methods. PhotonNetworkingMessage lists all available methods!
	
	public virtual void OnConnectedToMaster()
	{
		if (PhotonNetwork.networkingPeer.AvailableRegions != null) Debug.LogWarning("List of available regions counts " + PhotonNetwork.networkingPeer.AvailableRegions.Count + ". First: " + PhotonNetwork.networkingPeer.AvailableRegions[0] + " \t Current Region: " + PhotonNetwork.networkingPeer.CloudRegion);
		Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
    }
	
	public virtual void OnPhotonRandomJoinFailed()
	{
		Debug.Log("OnPhotonRandomJoinFailed() was called by PUN. ");
        //Yoav Todo: failed popup.
    }
	
	// the following methods are implemented to give you some context. re-implement them as needed.
	
	public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
	{
		Debug.LogError("Cause: " + cause);
	}
	
	public void OnJoinedRoom()
	{
        connect = false;
		Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
		Debug.Log(PhotonNetwork.playerList.Length);
		if (amount_of_players == PhotonNetwork.playerList.Length) {
            Application.LoadLevel("main");
        }
	}
	
	public void OnJoinedLobby()
	{
		Debug.Log("OnJoinedLobby(). Use a GUI to show existing rooms available in PhotonNetwork.GetRoomList().");	
	}
	
	void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		Debug.Log("OnPhotonPlayerConnected: " + player);
		
		// when new players join, we send "who's it" to let them know
		// only one player will do this: the "master"
		
		if (PhotonNetwork.isMasterClient)
		{
			Debug.Log("MasterClient");
		}

        Debug.Log(PhotonNetwork.playerList.Length);
        if (amount_of_players == PhotonNetwork.playerList.Length)
        {
            Application.LoadLevel("main");
        }
    }

    public void start_the_game()
    {
        if (PhotonNetwork.isMasterClient) {
            PhotonNetwork.room.open = false;
        }        
        Application.LoadLevel("main");
    }

	
	void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		Debug.Log("OnPhotonPlayerDisconnected: " + player);
		/**
		 * add I won,
		 */
		Debug.Log("I won");

        manager_script manager = GameObject.Find("the game").GetComponent<manager_script>();

        manager.won(PhotonNetwork.player.ID);
    }
	
}

