using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public class Activator : MShipMono {

	public float maxDistance = 7;

	private string toShow;
	private Player player;

	private Transform hoveredTransform;

	private List<Collider> raycastColliders;

	void Update() {
		raycastColliders.RemoveAll( c => c == null );

		if (raycastColliders.Count == 0)
			return;

		raycastColliders.Sort(
			(one, two)
				=> (one.transform.position - transform.position).sqrMagnitude.CompareTo(
					 (two.transform.position - transform.position).sqrMagnitude)
			);

		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit = new RaycastHit(); //Compiler pleasing, the uses of hit will never happen without it being set

		for (int i = 0; i < raycastColliders.Count; i++) {
			if (raycastColliders[i].Raycast(ray, out hit, maxDistance)) {
				break;
			}
			if (i == raycastColliders.Count - 1) {
				//No hit
				toShow = "";
				SetHoveredTransform(null);
				return;
			}
		}

		SetHoveredTransform(hit.transform);

		IDescriptiveText desc = hit.transform.GetIComponent<IDescriptiveText>();
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

	void Awake() {
		player = transform.parent.GetComponent<Player>();
		raycastColliders = new List<Collider>();
	}

	void Start() {
		((SphereCollider) collider).radius = maxDistance;
	}

	void OnTriggerEnter(Collider other) {
		raycastColliders.Add(other);
	}

	void OnTriggerExit(Collider other) {
		raycastColliders.Remove(other);
	}

}