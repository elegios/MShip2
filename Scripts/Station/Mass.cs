using UnityEngine;
using System.Collections;

public class Mass : MShipMono {

	public float value = 1;

	private Rigidbody parent;

	public void TransferMassTo(Rigidbody newParent) {
		if (parent != null)
			parent.mass -= value;

		parent = newParent;

		if (newParent != null)
			newParent.mass += value;
	}

}