using UnityEngine;
using System.Collections;

public class FakeFixedJoint : MonoBehaviour {

	private Vector3 locPos;
	private Quaternion relRot;
	private Transform target;

	public Transform lockTo {
		set {
			target = value;
			locPos = target.InverseTransformPoint(transform.position);
			relRot = Quaternion.Inverse(target.rotation) * transform.rotation;
		}
	}

	void Update() {
		if (target == null)
			return;

		if (transform.parent.name == "UpBridge(Clone)")
			print(relRot.eulerAngles);

		transform.position = target.TransformPoint(locPos);
		transform.rotation = target.rotation * relRot;
	}

}