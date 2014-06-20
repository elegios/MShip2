using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class Projectile : MonoBehaviour {

	public float force = 10;
	public float lifeTime = 60;

	void Start() {
		rigidbody.AddForce(transform.up * force, ForceMode.VelocityChange);
		Destroy(gameObject, lifeTime);
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
			linkToDestroy.DoDestroy(transform.position, -collision.relativeVelocity * rigidbody.mass);
		} else {
			print("collided with something else, destroying self anyway");
		}
	}

	[RPC]
	void NetDestroyThis() {
		Destroy(gameObject);
	}

}