using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class SyncTransform : MonoBehaviour {

	public bool syncPosition = true;
	public bool syncRotation = true;

	private bool toSet = false;
	private Vector3 toSetPosition;
	private Vector3 toSetRotation;

	void Update () {
		if (toSet) {
			if (syncPosition)
				transform.position = toSetPosition;
			if (syncRotation)
				transform.eulerAngles = toSetRotation;
			toSet = false;
		}
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		Vector3 pos = transform.rotation.eulerAngles;
		Vector3 rot = transform.eulerAngles;
		if (syncPosition)
			stream.Serialize(ref pos);
		if (syncRotation)
			stream.Serialize(ref rot);
		if (stream.isReading) {
			if (syncPosition)
				toSetPosition = pos;
			if (syncRotation)
				toSetRotation = rot;
			toSet = true;
		}
	}
}
