using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class TransferNetworkView : MShipMono {

	public void TakeOwnership(NetworkView view) {
		if (view.isMine)
			return;
		networkView.RPC("SetViewID", view.owner, view.viewID, Network.AllocateViewID());
	}

	public void TakeOwnershipAll(GameObject go) {
		foreach (NetworkView view in go.GetComponents<NetworkView>()) {
			TakeOwnership(view);
		}
	}

	[RPC]
	void SetViewID(NetworkViewID oldID, NetworkViewID newID) {
		NetworkView v = NetworkView.Find(oldID);
		if (v.isMine)
			networkView.RPC("SetViewID", RPCMode.OthersBuffered, oldID, newID);
		v.viewID = newID;
	}

}
