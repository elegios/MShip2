using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
[RequireComponent(typeof(Mass))]
public class StationLink : MonoBehaviour {

	public Transform parentPoint;
	public Transform[] childPoints;

	public GameObject buildBoardPrefab;
	public GameObject stationPrefab;

	private GameObject[] buildBoards;
	private StationLink[] children;
	private StationLink parent;

	void ParentWillBeDestroyed() {
		GameObject station = (GameObject) Network.Instantiate(stationPrefab, transform.position, transform.rotation, 0);
		networkView.RPC("SetupStation", RPCMode.AllBuffered, station.networkView.viewID);

		GameObject board = (GameObject) Network.Instantiate(buildBoardPrefab, Vector3.zero, Quaternion.identity, 0);
		networkView.RPC("SetupBuildBoard", RPCMode.AllBuffered, childPoints.Length, board.networkView.viewID);
	}
	[RPC]
	void SetupStation(NetworkViewID stationID) {
		Transform station = NetworkView.Find(stationID).transform;
		station.position = transform.position;
		station.rotation = transform.rotation;
		transform.parent = station;
	}

	void ChildWillBeDestroyed(StationLink child) {
		int i = -1;
		while (i < childPoints.Length && children[i] != child)
			i++;
		if (i == childPoints.Length) {
			print("Couldn't find the child that will supposedly be destroyed");
			return;
		}

		GameObject board = (GameObject) Network.Instantiate(buildBoardPrefab, Vector3.zero, Quaternion.identity, 0);
		networkView.RPC("SetupBuildBoard", RPCMode.AllBuffered, i, board.networkView.viewID);
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
		Destroy(buildBoards[i]);
	}

	public void DoDestroy() {
		if (!Network.isServer) {
			print("This function should only be called on the server");
			return;
		}

		//TODO: transfer mass to the correct station when everything is split

		for (int i = 0; i < children.Length; i++) {
			if (children[i] != null)
				children[i].ParentWillBeDestroyed();
		}
		if (parent != null) {
			parent.ChildWillBeDestroyed(this);
		}
		Destroy(gameObject);
	}

	public void SetParent(StationLink parent) {
		if (!Network.isServer) {
			print("This function should only be called on the server");
			return;
		}

		if (parent != null) {
			Destroy(buildBoards[buildBoards.Length - 1]);
			this.parent = parent;
			//TODO: setup mass
		}
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