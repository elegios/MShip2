using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {

	public float force = 10;
	public float lifeTime = 60;

	void Start() {
		rigidbody.AddForce(transform.up * force, ForceMode.VelocityChange);
		Destroy(gameObject, lifeTime);
	}

	void OnTriggerEnter(Collider collider) {
		if (!Network.isServer)
			return;

		Debug.Break();
		Debug.DrawRay(transform.position, rigidbody.velocity - collider.transform.root.rigidbody.velocity);

		networkView.RPC("NetDestroyThis", RPCMode.All);
		GameObject other = collider.gameObject;
		StationLink linkToDestroy = other.GetComponent<StationLink>();
		if (linkToDestroy == null) {
			if (other.GetComponent<RemoteStationLink>() != null)
				linkToDestroy = other.GetComponent<RemoteStationLink>().link;
		}
		if (linkToDestroy != null) {

			linkToDestroy.DoDestroy(transform.position, (rigidbody.velocity - other.transform.root.rigidbody.velocity) * rigidbody.mass);
		} else {
			print("collided with something else, destroying self anyway");
		}
	}

	[RPC]
	void NetDestroyThis() {
		Destroy(gameObject);
	}

}