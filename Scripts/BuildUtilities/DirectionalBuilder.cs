using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class DirectionalBuilder : MShipMono {

	private const int NORTH = 0;
	private const int EAST = 1;
	private const int SOUTH = 2;
	private const int WEST = 3;

	public GameObject constructible;

	public Transform north;
	public Transform east;
	public Transform south;
	public Transform west;

	void North() {
		Build(NORTH);
	}
	void East() {
		Build(EAST);
	}
	void South() {
		Build(SOUTH);
	}
	void West() {
		Build(WEST);
	}
	[RPC]
	void Build(int dir) {
		if (!Network.isServer) {
			networkView.RPC("Build", RPCMode.Server, dir);
			return;
		}
		GameObject thing = (GameObject) Network.Instantiate(constructible, transform.position, Quaternion.identity, 0);
		NetworkViewID thingID = thing.networkView.viewID;

		networkView.RPC("DoBuild", RPCMode.AllBuffered, dir, thingID);
	}
	[RPC]
	void DoBuild(int dir, NetworkViewID thingID) {
		north.GetComponent<PressButton>().DoDestroy();
		east.GetComponent<PressButton>().DoDestroy();
		south.GetComponent<PressButton>().DoDestroy();
		west.GetComponent<PressButton>().DoDestroy();

		Transform buildPoint;
		if (dir == NORTH) {
			buildPoint = north;
		} else if (dir == EAST) {
			buildPoint = east;
		} else if (dir == SOUTH) {
			buildPoint = south;
		} else {
			buildPoint = west;
		}

		Transform thing = NetworkView.Find(thingID).transform;
		thing.position = transform.position;
		thing.rotation = buildPoint.rotation;
		thing.parent = transform.parent;
	}

}