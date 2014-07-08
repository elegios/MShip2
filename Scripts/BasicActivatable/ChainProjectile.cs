using UnityEngine;
using System.Collections;

public class ChainProjectile : MShipMono {

	public float speed = 10;

	[HideInInspector]
	public Transform target;
	[HideInInspector]
	public Transform parent;
	private Vector3 locPos;

	void Update() {
		transform.position = parent.TransformPoint(locPos);
		if (target != null) {
			transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
			transform.LookAt(target);
			if (transform.position == target.position) {
				target.GetComponent<ChainInput>().OnChainActivate();
				target = null;
				particleSystem.enableEmission = false;
			}

		} else {
			transform.position += transform.forward.normalized * speed * Time.deltaTime;
			if (particleSystem.particleCount == 0) {
				Destroy(gameObject);
			}
		}
		SetLocPos();
	}

	public void SetLocPos() {
		locPos = parent.InverseTransformPoint(transform.position);
	}

}
