using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	public string ipaddress = "127.0.0.1";
	public int port = 8000;
	public bool useNat = false;
	
	public int maxConnections = 10;
	
	public GameObject player;
	public GameObject stationBase;
	
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
		Network.Instantiate(player, Vector3.up*10, Quaternion.identity, 0);
	}
	
	void OnServerInitialized() {
		Debug.Log("Server started.");
		Network.Instantiate(stationBase, Vector3.zero, Quaternion.identity, 0);
		Network.Instantiate(player, Vector3.up*10, Quaternion.identity, 0);
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
