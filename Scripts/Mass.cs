using UnityEngine;
using System.Collections;

public class Mass : MonoBehaviour {

	public float value = 1;

	private Rigidbody parent;

	public void TransferMassTo(Rigidbody newParent) {
		if (newParent == parent)
			return;
		if (parent != null)
			parent.mass -= value;

		newParent.mass += value;
		parent = newParent;
	}

}