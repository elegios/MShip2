using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class Cannon : MonoBehaviour {

	public GameObject projectile;
	public float spawnDist = 5;
	public float cooldown = 5;

	private float lastTimeFired;

	[RPC]
	void OnActivate() {
		if (!Network.isServer) {
			networkView.RPC("OnActivate", RPCMode.Server);
			return;
		}

		if (Time.time - lastTimeFired > cooldown) {
			Network.Instantiate(projectile, transform.position + transform.up * spawnDist, transform.rotation, 0);
			lastTimeFired = Time.time;
		}
	}

}