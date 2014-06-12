using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public float force = 10;

	void Start() {
		rigidbody.AddForce(transform.up * force, ForceMode.VelocityChange);
	}

	void OnCollisionEnter(Collision collision) {
		Destroy(gameObject);
		Destroy(collision.contacts[0].otherCollider.gameObject);
	}

}