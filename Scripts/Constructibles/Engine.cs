using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class Engine : MShipMono {

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

		Vector3 force = transform.up * strength;
		if (Mathf.Abs(force.y) > 0.1f) {
			transform.root.rigidbody.AddForce(force);
		} else {
			Vector3 pos = transform.position;
			pos.y = transform.root.position.y;
			transform.root.rigidbody.AddForceAtPosition(force, pos);
		}
	}

}