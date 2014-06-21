using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class ChainOutput : MonoBehaviour {

	public GameObject activateProjectilePrefab;

	public float movementSpeed;

	[HideInInspector]
	public ChainInput targetInput;
	private Transform activateProjectile;
	private Vector3 projLocPos;

	public void OnChainActivate() {
		if (activateProjectile != null || targetInput == null)
			return;

		GameObject projectile =
		  (GameObject) Instantiate(activateProjectilePrefab,
			                         transform.position, 
		                           Quaternion.LookRotation(targetInput.transform.position - transform.position));
		activateProjectile = projectile.transform;
		projLocPos = transform.InverseTransformPoint(activateProjectile.position);
	}

	void OnActivate(Player p) {
		p.selectedChainOutput = this;
	}

	void Update() {
		if (activateProjectile == null)
			return;

		activateProjectile.position =
			Vector3.MoveTowards(transform.TransformPoint(projLocPos),
				                  targetInput.transform.position,
				                  movementSpeed * Time.deltaTime);
		activateProjectile.LookAt(targetInput.transform);
		projLocPos = transform.InverseTransformPoint(activateProjectile.position);

		if (activateProjectile.position == targetInput.transform.position) {
			Destroy(activateProjectile.gameObject);
			targetInput.OnChainActivate(true);
		}
	}

}
