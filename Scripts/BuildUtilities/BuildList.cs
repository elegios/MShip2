using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class BuildList : MonoBehaviour {

	public float minHeight = 1;
	public int columns = 5;
	public float columnWidth = .6f;
	public float rowHeight = .6f;

	public GameObject[] buildButtonPrefabs;
	public GameObject togglePrefab;
	public GameObject directionalBuilder;

	private ToggleButton toggle;
	private GameObject[] buildButtons;

	void Awake() {
		GameObject toggleButton = (GameObject) Instantiate(togglePrefab, transform.position, transform.rotation);
		toggleButton.transform.parent = transform;
		toggle = toggleButton.GetComponent<ToggleButton>();
		toggle.on = false;
		toggleButton.GetComponent<RemoteActivatable>().target = gameObject;
	}

	[RPC]
	void Toggle() {
		if (!Network.isServer) {
			networkView.RPC("Toggle", RPCMode.Server);
			return;
		}

		networkView.RPC("DoToggle", RPCMode.AllBuffered, !toggle.on);
	}
	[RPC]
	void DoToggle(bool on) {
		if (on) {
			SetupButtons();
		} else {
			DeleteButtons();
		}
		toggle.on = on;
	}

	void SetupButtons() {
		buildButtons = new GameObject[buildButtonPrefabs.Length];

		float xPos = -columnWidth * Mathf.Min(columns, buildButtonPrefabs.Length) / 2;
		float yPos = minHeight + rowHeight * ((buildButtonPrefabs.Length - 1) / columns + 1);

		for (int i = 0; i < buildButtonPrefabs.Length; i++) {
			Vector3 position = transform.TransformPoint(new Vector3(xPos + (i % columns) - 0.5f, yPos + i / columns, 0));
			buildButtons[i] = (GameObject) Instantiate(buildButtonPrefabs[i], position, transform.rotation);

			RemoteActivatable act = buildButtons[i].GetComponent<RemoteActivatable>();
			act.target = gameObject;
			act.payload = i;
			buildButtons[i].transform.parent = transform;
		}
	}

	void DeleteButtons() {
		foreach (GameObject button in buildButtons) {
			button.GetComponent<PressButton>().DoDestroy();
		}
	}

	[RPC]
	void Build(int selection) {
		if (!Network.isServer) {
			networkView.RPC("Build", RPCMode.Server, selection);
			return;
		}

		GameObject dirBuilder = (GameObject) Network.Instantiate(directionalBuilder, transform.position, transform.rotation, 0);
		networkView.RPC("DoBuild", RPCMode.AllBuffered, selection, dirBuilder.networkView.viewID);
	}
	[RPC]
	void DoBuild(int selection, NetworkViewID dirBuilderID) {
		Transform dirBuilder = NetworkView.Find(dirBuilderID).transform;
		dirBuilder.parent = transform.parent;
		dirBuilder.GetComponent<DirectionalBuilder>().constructible = buildButtons[selection].GetComponent<Build>().constructible;
		DeleteButtons();
		toggle.on = false;
	}

}