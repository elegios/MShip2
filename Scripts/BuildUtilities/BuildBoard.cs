using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class BuildBoard : MShipMono {

	public float timeTillAutoClose = 60;

	public GameObject[] buttonPrefabs;
	public GameObject buildButtonPrefab;
	public int defaultChoice;

	private ToggleButton[] buttons;
	private PressButton buildButton;
	private Transform buildPoint;

	private bool isOpen = false;
	private float openTimer = 0;

	void Awake() {
		buildPoint = transform.Find("BuildPoint");
	}

	[RPC]
	void ChooseBuildTarget(int id) {
		if (!Network.isServer) {
			networkView.RPC("ChooseBuildTarget", RPCMode.Server, id);
			return;
		}

		networkView.RPC("DoChooseBuildTarget", RPCMode.AllBuffered, id);
	}
	[RPC]
	void DoChooseBuildTarget(int id) {
		if (buttons == null) {
			print("There are no buttons, we've got some strange things");
			return;
		}

		for (int i = 0; i < buttons.Length; i++) {
			buttons[i].on = id == i;
		}

		ResetTimer();
	}

	[RPC]
	void Build() {
		if (!Network.isServer) {
			networkView.RPC("Build", RPCMode.Server);
			return;
		}
		ToggleButton button = null;
		for (int i = 0; i < buttons.Length; i++) {
			if (buttons[i].on) {
				button = buttons[i];
				break;
			}
		}

		GameObject thing = (GameObject) Network.Instantiate(button.GetComponent<Build>().constructible, Vector3.zero, Quaternion.identity, 0);
		networkView.RPC("DoBuild", RPCMode.AllBuffered, thing.networkView.viewID);
	}
	[RPC]
	void DoBuild(NetworkViewID thingID) {
		Transform thing = NetworkView.Find(thingID).transform;
		thing.position = buildPoint.position;
		thing.rotation = buildPoint.rotation;
		thing.parent = transform.parent;

		StationLink stationLink = thing.transform.Find("StationPart").GetComponent<StationLink>();

		if (Network.isServer) {
			stationLink.SetParent(transform.parent.GetComponent<StationLink>());
		}
		transform.parent.GetComponent<StationLink>().ChildAdded(gameObject, stationLink);

		DoActivate(false);
	}

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
			buttons = new ToggleButton[buttonPrefabs.Length];
			for (int i = 0; i < buttons.Length; i++) {
				GameObject button = (GameObject) Instantiate(buttonPrefabs[i], Vector3.zero, transform.rotation);
				button.transform.parent = transform;
				button.transform.localPosition = new Vector3(1, i * 0.5f, 0);
				buttons[i] = button.GetComponent<ToggleButton>();
				RemoteActivatable ra = button.GetComponent<RemoteActivatable>();
				ra.target = gameObject;
				ra.payload = i;
			}

			GameObject buildGO = (GameObject) Instantiate(buildButtonPrefab, Vector3.zero, transform.rotation);
			buildGO.transform.parent = transform;
			buildGO.transform.localPosition = new Vector3(1.5f, 0.5f, 0);
			buildButton = buildGO.GetComponent<PressButton>();
			buildGO.GetComponent<RemoteActivatable>().target = gameObject;

			buttons[defaultChoice].on = true;
			ResetTimer();

		} else {
			for (int i = 0; i < buttons.Length; i++) {
				buttons[i].DoDestroy();
			}
			buildButton.DoDestroy();

			buttons = null;
			buildButton = null;
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