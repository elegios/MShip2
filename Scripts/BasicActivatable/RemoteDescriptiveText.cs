using UnityEngine;
using System.Collections;

public class RemoteDescriptiveText : MShipMono, IDescriptiveText {

	public Transform target;
	private IDescriptiveText t;

	string IDescriptiveText.text {get {return t.text;}}

	void Awake() {
		t = target.GetIComponent<IDescriptiveText>();
		if (t == null)
			print("Error here! Should've been an IDescriptiveText, but there was none");
	}

}