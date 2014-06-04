using UnityEngine;
using System.Collections;

public class HasGrandParent : MonoBehaviour {

	public GameObject grandParent;

	public NetworkViewID GetCurrentID() {
		return grandParent.networkView.viewID;
	}

}
