using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.VR;

public class NetworkController : NetworkManager
{

	public Transform spawnPosition;
	public GameObject VRPlayer;
	public GameObject ARPlayer;


	//Called on client when connect
	public override void OnClientConnect(NetworkConnection conn)
	{

		// Create message to set the player
		IntegerMessage msg = new IntegerMessage(0);

		// Call Add player and pass the message
		ClientScene.AddPlayer(conn, 0, msg);
	}

	// Server
	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
	{
		var playerPrefab = VRPlayer;
		//Select the prefab from the spawnable objects list
		if (VRSettings.loadedDeviceName == "HoloLens")
			playerPrefab = ARPlayer;

		// Create player object with prefab
		var player = Instantiate(playerPrefab, spawnPosition.position, Quaternion.identity) as GameObject;

		// Add player object for connection
		NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
	}
}