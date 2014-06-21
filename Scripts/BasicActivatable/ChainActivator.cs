using UnityEngine;
using System.Collections;

public class ChainActivator : MonoBehaviour {

	public ChainInput input;

	void OnActivate() {
		input.OnChainActivate(false);
	}

}
