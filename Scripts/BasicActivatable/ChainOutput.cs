using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class ChainOutput : MonoBehaviour {

	public GameObject activateProjectilePrefab;
	public Transform connection;

	[HideInInspector]
	public Transform targetInput;

	private bool showConnection;
	private float baseZScale;

	void Awake() {
		baseZScale = connection.localScale.z;
	}

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

	void OnActivatorHover() {
		showConnection = true;
	}

	void Update() {
		if (targetInput == null) {
			showConnection = false;
		}
		if (!showConnection) {
			connection.GetChild(0).renderer.enabled = false;
			return;
		}
		connection.GetChild(0).renderer.enabled = true;

		connection.LookAt(targetInput);
		connection.localScale = new Vector3(connection.localScale.x, connection.localScale.y, baseZScale * (targetInput.position - transform.position).magnitude);
		showConnection = false;
	}

}
