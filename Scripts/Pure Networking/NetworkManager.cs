using UnityEngine;
using System.Collections;

public class NetworkManager : MShipMono {

	public string ipaddress = "127.0.0.1";
	public int port = 8000;
	public bool useNat = false;
	
	public int maxConnections = 10;
	
	public GameObject player;
	public GameObject station;
	public GameObject startPlatform;
	
	void OnGUI() {
		if (!Network.isClient && !Network.isServer) {
			GUILayout.BeginHorizontal();
			ipaddress = GUILayout.TextField(ipaddress);
			GUILayout.Label("Ip address");
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			port = int.Parse(GUILayout.TextField(port.ToString()));
			GUILayout.Label("Port");
			GUILayout.EndHorizontal();
			
			if (GUILayout.Button("Connect")) {
				Debug.Log("Connecting to " +ipaddress+ ":" +port);
				Network.Connect(ipaddress, port);
			}
			
			if (GUILayout.Button("Start server")) {
				Debug.Log("Starting server on port " +port);
				Network.InitializeServer(maxConnections, port, useNat);
			}
		}
	}
	
	void OnConnectedToServer() {
		Debug.Log("Connected to server.");
		Network.Instantiate(player, Vector3.up*10 + Vector3.right*20, Quaternion.identity, 0);
	}
	void OnPlayerConnected(NetworkPlayer player) {
		GameObject s = (GameObject) Network.Instantiate(station, Vector3.right*20, Quaternion.identity, 0);
		GameObject p = (GameObject) Network.Instantiate(startPlatform, Vector3.right*20, Quaternion.identity, 0);
		networkView.RPC("SetupNewStation", RPCMode.AllBuffered, s.networkView.viewID, p.networkView.viewID);
	}
	
	void OnServerInitialized() {
		Debug.Log("Server started.");
		GameObject s = (GameObject) Network.Instantiate(station, Vector3.zero, Quaternion.identity, 0);
		GameObject p = (GameObject) Network.Instantiate(startPlatform, Vector3.zero, Quaternion.identity, 0);
		networkView.RPC("SetupNewStation", RPCMode.AllBuffered, s.networkView.viewID, p.networkView.viewID);
		Network.Instantiate(player, Vector3.up*10, Quaternion.identity, 0);
	}

	[RPC]
	void SetupNewStation(NetworkViewID stationID, NetworkViewID platformID) {
		Transform station = NetworkView.Find(stationID).transform;
		Transform platform = NetworkView.Find(platformID).transform;
		platform.parent = station;
	}

	[RPC]
	public void RemoteLog(string str) {
		if (Network.isServer) {
			print(str);
			return;
		}

		networkView.RPC("RemoteLog", RPCMode.Server, str);
	}
	
}
