using UnityEngine;
using System.Collections;

public class RemoteActivatable : MShipMono {

	public GameObject target;
	public string message;
	public int payload;

	// add info about which player activated
	void OnActivate() {
		target.SendMessage(message, payload);
	}

}