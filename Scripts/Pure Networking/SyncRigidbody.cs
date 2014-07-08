using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class SyncRigidbody : MShipMono {
	
	private class ToSet {
		public Vector3 pos;
		public Vector3 rot;
		public Vector3 angVel;
		public Vector3 posVel;
		public bool toSet = false;
	}
	
	private ToSet toSet = new ToSet();
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		Vector3 pos = rigidbody.position;
		Vector3 rot = rigidbody.rotation.eulerAngles;
		Vector3 angVel = rigidbody.angularVelocity;
		Vector3 posVel = rigidbody.velocity;
		stream.Serialize(ref pos);
		stream.Serialize(ref rot);
		stream.Serialize(ref angVel);
		stream.Serialize(ref posVel);
		if (stream.isReading) {
			toSet.pos = pos;
			toSet.rot = rot;
			toSet.angVel = angVel;
			toSet.posVel = posVel;
			toSet.toSet = true;
		}
	}
	
	void FixedUpdate() {
		if (toSet.toSet) {
			rigidbody.position = toSet.pos;
			rigidbody.rotation = Quaternion.Euler(toSet.rot);
			rigidbody.angularVelocity = toSet.angVel;
			rigidbody.velocity = toSet.posVel;
			toSet.toSet = false;
		}
	}
	
}
