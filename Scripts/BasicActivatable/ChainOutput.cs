using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class ChainOutput : MonoBehaviour {

	public GameObject activateProjectilePrefab;
	public Transform connection;

	[HideInInspector]
	public Transform targetInput;

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
		networkView.RPC("DisconnectMe", RPCMode.AllBuffered);
	}
	[RPC]
	void DisconnectMe() {
		if (targetInput == null)
			return;

		targetInput.GetComponent<ChainInput>().DisconnectOutput(this);
		SetConnectionVisibility(false);
		targetInput = null;
	}

	void OnMouseHoverChanged(bool hover) {
		SetConnectionVisibility(hover);
	}

	public void SetConnectionVisibility(bool visible) {
		if (targetInput == null)
			return;

		connection.GetChild(0).renderer.enabled = visible;
		if (visible) {
			connection.LookAt(targetInput);
			connection.localScale = new Vector3(connection.localScale.x, connection.localScale.y, baseZScale * (targetInput.position - transform.position).magnitude);
		}

	}

}
