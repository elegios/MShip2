using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkView))]
public class ChainInput : MonoBehaviour {

	public GameObject target;
	public ChainOutput[] outputs;

	private List<ChainOutput> targetingThis;

	void Awake() {
		targetingThis = new List<ChainOutput>();
	}

	public void OnChainActivate() {
		if (!Network.isServer) {
			return;
		}

		if (target != null) {
			target.SendMessage("OnActivate");
		}
		networkView.RPC("DoChainActivate", RPCMode.All);
	}
	public void StartChain() {
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

		p.selectedChainOutput.targetInput = transform;
		targetingThis.Add(p.selectedChainOutput);
		p.selectedChainOutput.SetConnectionVisibility(true);
		networkView.RPC("DoConnection", RPCMode.OthersBuffered, p.selectedChainOutput.networkView.viewID);

		p.selectedChainOutput = null;
	}
	[RPC]
	void DoConnection(NetworkViewID outputID) {
		ChainOutput output = NetworkView.Find(outputID).gameObject.GetComponent<ChainOutput>();

		output.targetInput = transform;
		targetingThis.Add(output);
	}

	public void DisconnectOutput(ChainOutput output) {
		targetingThis.Remove(output);
	}

	void OnMouseHoverChanged(bool hover) {
		foreach (ChainOutput output in targetingThis) {
			output.SetConnectionVisibility(hover);
		}
	}

}
