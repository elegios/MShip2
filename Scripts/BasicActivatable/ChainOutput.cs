using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class ChainOutput : MonoBehaviour {

	public GameObject activateProjectilePrefab;

	[HideInInspector]
	public Transform targetInput;

	public void OnChainActivate() {
		if (targetInput == null)
			return;

		GameObject projectile =
		  (GameObject) Instantiate(activateProjectilePrefab,
			                         transform.position, 
		                           Quaternion.LookRotation(targetInput.position - transform.position));
		ChainProjectile p = projectile.GetComponent<ChainProjectile>();
		p.parent = transform.parent;
		p.target = targetInput;
		p.SetLocPos();
	}

	void OnActivate(Player p) {
		p.selectedChainOutput = this;
	}

}
