using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class BridgeBuildBoard : MonoBehaviour {

	private const int DOWN = 0;
	private const int MIDDLE = 1;
	private const int UP = 2;

	public float timeTillAutoClose = 60;

	public GameObject upPrefab;
	public GameObject middlePrefab;
	public GameObject downPrefab;
	public GameObject buildPrefab;

	public GameObject upBridgePrefab;
	public GameObject middleBridgePrefab;
	public GameObject downBridgePrefab;

	public GameObject mediumPlatfomPrefab;

	private ToggleButton up;
	private ToggleButton middle;
	private ToggleButton down;
	private PressButton build;

	private GameObject upButton;
	private GameObject middleButton;
	private GameObject downButton;
	private GameObject buildButton;

	private bool isOpen = false;

	private float openTimer = 0;
	private Transform buildPoint;

	void Awake() {
		buildPoint = transform.Find("BuildPoint");
	}

	void SetDown() {
		SetBridgeMode(DOWN);
	}
	void SetMiddle() {
		SetBridgeMode(MIDDLE);
	}
	void SetUp() {
		SetBridgeMode(UP);
	}
	[RPC]
	void SetBridgeMode(int mode) {
		if (!Network.isServer) {
			networkView.RPC("SetBridgeMode", RPCMode.Server, mode);
			return;
		}
		networkView.RPC("DoSetBridgeMode", RPCMode.AllBuffered, mode);
	}
	[RPC]
	void DoSetBridgeMode(int mode) {
		if (up == null)
			return;

		up.on = mode == UP;
		middle.on = mode == MIDDLE;
		down.on = mode == DOWN;
		ResetTimer();
	}

	[RPC]
	void Build() {
		if (!Network.isServer) {
			networkView.RPC("Build", RPCMode.Server);
			return;
		}
		GameObject bridge;
		if (up.on) {
			bridge = (GameObject) Network.Instantiate(upBridgePrefab, buildPoint.position, buildPoint.rotation, 0);
		} else if (middle.on) {
			bridge = (GameObject) Network.Instantiate(middleBridgePrefab, buildPoint.position, buildPoint.rotation, 0);
		} else {
			bridge = (GameObject) Network.Instantiate(downBridgePrefab, buildPoint.position, buildPoint.rotation, 0);
		}
		NetworkViewID bridgeID = bridge.networkView.viewID;
		GameObject platform = (GameObject) Network.Instantiate(mediumPlatfomPrefab, Vector3.zero, Quaternion.identity, 0);
		NetworkViewID platformID = platform.networkView.viewID;

		networkView.RPC("DoBuild", RPCMode.AllBuffered, bridgeID, platformID);
	}
	[RPC]
	void DoBuild(NetworkViewID bridgeID, NetworkViewID platformID) {
		GameObject grandParent = transform.parent.GetComponent<HasGrandParent>().grandParent;

		Transform bridge = NetworkView.Find(bridgeID).transform;
		bridge.position = buildPoint.position;
		bridge.rotation = buildPoint.rotation;
		bridge = bridge.Find("Bridge");

		bridge.GetComponent<HasGrandParent>().grandParent = grandParent;
		bridge.GetComponent<FakeFixedJoint>().lockTo = grandParent.transform;

		Transform platformPoint = bridge.Find("BuildPoint");
		Transform platform = NetworkView.Find(platformID).transform;
		platform.position = platformPoint.position;
		platform.rotation = platformPoint.rotation;
		platform = platform.Find("Platform");

		platform.GetComponent<HasGrandParent>().grandParent = grandParent;
		platform.GetComponent<FakeFixedJoint>().lockTo = grandParent.transform;

		bridge.hingeJoint.connectedBody = transform.parent.rigidbody;
		HingeJoint[] platformJoints = platform.GetComponents<HingeJoint>();
		platformJoints[0].connectedBody = bridge.rigidbody;
		platformJoints[1].connectedBody = grandParent.rigidbody;

		DoActivate(false);
	}

	//TODO: add information about activating player
	[RPC]
	void OnActivate() {
		if (!Network.isServer) {
			networkView.RPC("OnActivate", RPCMode.Server);
			return;
		}
		networkView.RPC("DoActivate", RPCMode.AllBuffered, !isOpen);
	}

	[RPC]
	void DoActivate(bool open) {
		if (open) {
			upButton = (GameObject) Instantiate(upPrefab, Vector3.zero, transform.rotation);
			upButton.transform.parent = transform;
			upButton.transform.localPosition = new Vector3(1, 1.5f, 0);
			up = upButton.GetComponent<ToggleButton>();
			upButton.GetComponent<RemoteActivatable>().target = gameObject;

			middleButton = (GameObject) Instantiate(middlePrefab, Vector3.zero, transform.rotation);
			middleButton.transform.parent = transform;
			middleButton.transform.localPosition = new Vector3(1, 1, 0);
			middle = middleButton.GetComponent<ToggleButton>();
			middleButton.GetComponent<RemoteActivatable>().target = gameObject;

			downButton = (GameObject) Instantiate(downPrefab, Vector3.zero, transform.rotation);
			downButton.transform.parent = transform;
			downButton.transform.localPosition = new Vector3(1, 0.5f, 0);
			down = downButton.GetComponent<ToggleButton>();
			downButton.GetComponent<RemoteActivatable>().target = gameObject;

			buildButton = (GameObject) Instantiate(buildPrefab, Vector3.zero, transform.rotation);
			buildButton.transform.parent = transform;
			buildButton.transform.localPosition = new Vector3(1.5f, 0.5f, 0);
			build = buildButton.GetComponent<PressButton>();
			buildButton.GetComponent<RemoteActivatable>().target = gameObject;

			middle.on = true;
			ResetTimer();

		} else {
			up.DoDestroy();
			middle.DoDestroy();
			down.DoDestroy();
			build.DoDestroy();

			up = null;
			middle = null;
			down = null;
			build = null;
		}
		isOpen = open;
	}

	void ResetTimer() {
		openTimer = timeTillAutoClose;
	}

	void Update() {
		if (!isOpen || !Network.isServer || openTimer <= 0)
			return;

		openTimer -= Time.deltaTime;
		if (openTimer > 0)
			return;

		networkView.RPC("DoActivate", RPCMode.AllBuffered, false);
	}

}