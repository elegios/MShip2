using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class Engine : MonoBehaviour {

	public float strength = 20;
	public ParticleSystem onEffect;

	private bool on = false;

	[RPC]
	void OnActivate() {
		if (!Network.isServer) {
			networkView.RPC("OnActivate", RPCMode.Server);
			return;
		}

		networkView.RPC("DoActivate", RPCMode.AllBuffered, !on);
	}

	[RPC]
	void DoActivate(bool on) {
		this.on = on;

		onEffect.enableEmission = on;
	}

	void FixedUpdate() {
		if (!on)
			return;

		transform.parent.parent.rigidbody.AddForce(transform.up * strength);
	}

}