using UnityEngine;
using System.Collections;

public class Activator : MonoBehaviour {

	public float maxDistance = 7;

	private string toShow;
	private Player player;

	private Transform hoveredTransform;

	void Awake() {
		player = transform.parent.GetComponent<Player>();
	}

	void Update() {
		RaycastHit hit;
		if (!Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, Co.LAYER_CONTROLS)) {
			toShow = "";
			SetHoveredTransform(null);
			return;
		}

		SetHoveredTransform(hit.transform);

		DescriptiveText desc = hit.transform.GetComponent<DescriptiveText>();
		if (desc != null)
			toShow = desc.text;

		hit.transform.SendMessage("OnActivatorHover", player, SendMessageOptions.DontRequireReceiver);
		if (Input.GetButtonDown(Co.ACTIVATE))
			hit.transform.SendMessage("OnActivate", player);
	}

	void SetHoveredTransform(Transform t) {
		if (hoveredTransform == t)
			return;

		if (hoveredTransform != null)
			hoveredTransform.SendMessage("OnMouseHoverChanged", false, SendMessageOptions.DontRequireReceiver);

		hoveredTransform = t;

		if (hoveredTransform != null)
			hoveredTransform.SendMessage("OnMouseHoverChanged", true, SendMessageOptions.DontRequireReceiver);
	}

	void OnGUI() {
		GUI.Box(new Rect((Screen.width - 10)/2, (Screen.height - 10)/2, 10, 10), "");

		if (toShow == "")
			return;

		GUILayout.Label(toShow);
	}

}