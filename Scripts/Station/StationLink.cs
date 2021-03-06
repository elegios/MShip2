using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
[RequireComponent(typeof(Mass))]
public class StationLink : MShipMono {

	public Transform parentPoint;
	public Transform[] childPoints;

	public GameObject buildBoardPrefab;
	public GameObject stationPrefab;

	private GameObject[] buildBoards;
	private StationLink[] children;
	private StationLink parent;

	void ParentWillBeDestroyed(Vector3 position, Vector3 impulse) {
		transform.parent = null;

		GameObject station = (GameObject) Network.Instantiate(stationPrefab, transform.position, transform.rotation, 0);
		networkView.RPC("SetupStation", RPCMode.AllBuffered, station.networkView.viewID, position, impulse);

		GameObject board = (GameObject) Network.Instantiate(buildBoardPrefab, Vector3.zero, Quaternion.identity, 0);
		networkView.RPC("SetupBuildBoard", RPCMode.AllBuffered, childPoints.Length, board.networkView.viewID);
	}
	[RPC]
	void SetupStation(NetworkViewID stationID, Vector3 position, Vector3 impulse) {
		Transform station = NetworkView.Find(stationID).transform;
		station.position = transform.position;
		station.rotation = transform.rotation;
		transform.parent = station;

		Mass[] ms = GetComponentsInChildren<Mass>();
		foreach (Mass m in ms) {
			m.TransferMassTo(transform.root.rigidbody);
		}

		Push(position, impulse);
	}

	void ChildWillBeDestroyed(StationLink child, Vector3 position, Vector3 impulse) {
		int i = 0;
		while (i < childPoints.Length && children[i] != child)
			i++;
		if (i == childPoints.Length) {
			print("Couldn't find the child that will supposedly be destroyed");
			return;
		}

		networkView.RPC("Push", RPCMode.All, position, impulse);

		GameObject board = (GameObject) Network.Instantiate(buildBoardPrefab, Vector3.zero, Quaternion.identity, 0);
		networkView.RPC("SetupBuildBoard", RPCMode.AllBuffered, i, board.networkView.viewID);
	}
	[RPC]
	void Push(Vector3 position, Vector3 impulse) {
		transform.root.rigidbody.AddForceAtPosition(impulse, position, ForceMode.Impulse);
	}

	//Must be called from code that is run for all players
	public void ChildAdded(GameObject buildBoard, StationLink child) {
		int i = 0;
		while (i < buildBoards.Length && buildBoards[i] != buildBoard)
			i++;
		if (i == buildBoards.Length) {
			print("Couldn't find the buildboard that built the child");
			return;
		}

		children[i] = child;
		child.GetComponent<Mass>().TransferMassTo(transform.root.rigidbody);
		Destroy(buildBoards[i]);
	}

	public void DoDestroy(Vector3 position, Vector3 impulse) {
		if (!Network.isServer) {
			print("This function should only be called on the server");
			return;
		}

		for (int i = 0; i < children.Length; i++) {
			if (children[i] != null)
				children[i].ParentWillBeDestroyed(position, impulse);
		}
		if (parent != null) {
			parent.ChildWillBeDestroyed(this, position, impulse);
		}
		networkView.RPC("NetDestroyThis", RPCMode.AllBuffered);
	}

	public void SetParent(StationLink parent) {
		if (!Network.isServer) {
			print("This function should only be called on the server");
			return;
		}

		if (parent != null) {
			networkView.RPC("NetDestroy", RPCMode.AllBuffered, buildBoards[buildBoards.Length - 1].networkView.viewID);
			this.parent = parent;
		}
	}

	[RPC]
	void NetDestroy(NetworkViewID id) {
		GameObject o = NetworkView.Find(id).gameObject;
		Destroy(o);
	}
	[RPC]
	void NetDestroyThis() {
		GetComponent<Mass>().TransferMassTo(null);
		Destroy(transform.parent.gameObject);
	}

	void Awake() {
		buildBoards = new GameObject[childPoints.Length + 1];
		children = new StationLink[childPoints.Length + 1];

		if (!Network.isServer)
			return;

		for (int i = 0; i < buildBoards.Length; i++) {
			GameObject board = (GameObject) Network.Instantiate(buildBoardPrefab, Vector3.zero, Quaternion.identity, 0);
			networkView.RPC("SetupBuildBoard", RPCMode.AllBuffered, i, board.networkView.viewID);
		}
	}

	[RPC]
	void SetupBuildBoard(int pointID, NetworkViewID boardID) {
		Transform board = NetworkView.Find(boardID).transform;
		Transform point = parentPoint;
		if (pointID < childPoints.Length)
			point = childPoints[pointID];

		board.position = point.position;
		board.rotation = point.rotation;
		board.parent = point.parent;

		buildBoards[pointID] = board.gameObject;
	}

}