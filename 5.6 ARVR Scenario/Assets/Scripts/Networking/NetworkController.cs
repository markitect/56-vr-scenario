using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.VR;

public class NetworkController : NetworkManager
{

	public Transform ARSpawnPoint;

	public Transform[] VRSpawnPoints;

	public GameObject VRPlayer;
	public GameObject ARPlayer;

    void Start()
    {
        if (VRSettings.loadedDeviceName == "HoloLens")
        {
            StartHost();
        }
    }

	//Called on client when connect
	public override void OnClientConnect(NetworkConnection conn)
	{

		// Create message to set the player
		IntegerMessage msg = new IntegerMessage(0);
		if (VRSettings.loadedDeviceName == "HoloLens")
			msg = new IntegerMessage(0);
		else
			msg = new IntegerMessage(1);


		// Call Add player and pass the message
		ClientScene.AddPlayer(conn, 0, msg);
	}

	// Server
	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
	{
		int curPlayer = 0;
		if (extraMessageReader != null)
		{
			var stream = extraMessageReader.ReadMessage<IntegerMessage>();
			curPlayer = stream.value;
		}

		var spawnPoint = this.VRSpawnPoints[Random.Range(0, this.VRSpawnPoints.Length - 1)];

		var playerPrefab = VRPlayer;
		if (curPlayer == 0)
		{
			playerPrefab = ARPlayer;
			spawnPoint = this.ARSpawnPoint;
		}
		//Select the prefab from the spawnable objects list
        
		// Create player object with prefab
		var player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity) as GameObject;

		// Add player object for connection
		NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
	}
}