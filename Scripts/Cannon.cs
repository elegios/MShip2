using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class Cannon : MonoBehaviour {

	public GameObject projectile;
	public float spawnDist;

	[RPC]
	void OnActivate() {
		if (!Network.isServer) {
			networkView.RPC("OnActivate", RPCMode.Server);
			return;
		}

		Network.Instantiate(projectile, transform.position + transform.up * spawnDist, transform.rotation, 0);
	}

}