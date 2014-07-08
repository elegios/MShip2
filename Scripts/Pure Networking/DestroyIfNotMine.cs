using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class DestroyIfNotMine : MShipMono {
	
	public GameObject toDestroy;

	void OnNetworkInstantiate (NetworkMessageInfo info) {
		Debug.Log ("network init");
		if (!networkView.isMine) {
			GameObject.Destroy(toDestroy);
		}
	}
	
}
