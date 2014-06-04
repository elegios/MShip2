using UnityEngine;
using System.Collections;

public class RandomPushes : MonoBehaviour {

	public float timer = 2;			
	public float force = 2;
	public float torque = 2;

	private float currentTime = 0;

	void FixedUpdate() {
		if (!networkView.isMine)
			return;
			
		currentTime -= Time.deltaTime;
		if (currentTime < 0) {
			currentTime += timer;
		} else {
			return;
		}

		if (Random.value > 0.5f) {
			rigidbody.AddForce((Random.value - 0.5f) * force * 2, (Random.value - 0.5f) * force * 2, (Random.value - 0.5f) * force * 2);
		} else {
			rigidbody.AddTorque(0, (Random.value - 0.5f) * torque * 2, 0);
		}
	}

}