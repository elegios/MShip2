using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class ReplaceWithPrefab : MShipMono {

	public GameObject prefab;

	void Awake() {
		GameObject replacement = (GameObject) Instantiate(prefab, transform.position, transform.rotation);
		replacement.networkView.viewID = networkView.viewID;
		replacement.transform.parent = transform.parent;
		Destroy(gameObject);

		replacement.SendMessage("OnReplaceInitiated", transform.parent.gameObject, SendMessageOptions.DontRequireReceiver);
	}

}