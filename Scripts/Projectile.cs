using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class Projectile : MonoBehaviour {

	public float force = 10;

	void Start() {
		rigidbody.AddForce(transform.up * force, ForceMode.VelocityChange);
	}

	void OnCollisionEnter(Collision collision) {
		if (!Network.isServer)
			return;

		networkView.RPC("NetDestroyThis", RPCMode.All);
		GameObject other = collision.contacts[0].otherCollider.gameObject;
		StationLink linkToDestroy = other.GetComponent<StationLink>();
		if (linkToDestroy == null) {
			if (other.GetComponent<RemoteStationLink>() != null)
				linkToDestroy = other.GetComponent<RemoteStationLink>().link;
		}
		if (linkToDestroy != null) {
			linkToDestroy.DoDestroy(rigidbody);
		} else {
			print("collided with something else, destroying self anyway");
		}
	}

	[RPC]
	void NetDestroyThis() {
		Destroy(gameObject);
	}

}