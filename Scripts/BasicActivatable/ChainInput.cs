using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class ChainInput : MonoBehaviour {

	public GameObject target;
	public ChainOutput[] outputs;

	public void OnChainActivate(bool fromChain) {
		if (!Network.isServer) {
			return;
		}

		if (fromChain) {
			target.SendMessage("OnActivate");
		}
		networkView.RPC("DoChainActivate", RPCMode.All);
	}
	[RPC]
	void DoChainActivate() {
		foreach (ChainOutput o in outputs) {
			o.OnChainActivate();
		}
	}

	void OnActivate(Player p) {
		if (p.selectedChainOutput == null)
			return;

		p.selectedChainOutput.targetInput = this;
		p.selectedChainOutput = null;
	}

}
