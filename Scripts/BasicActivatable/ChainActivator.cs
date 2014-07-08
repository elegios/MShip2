using UnityEngine;
using System.Collections;

public class ChainActivator : MShipMono {

	public ChainInput input;

	void OnActivate() {
		input.StartChain();
	}

}
