using UnityEngine;
using System.Collections;

public class Build : MShipMono, IDescriptiveText {

	public GameObject constructible;
	public string text;

	string IDescriptiveText.text {
		get {
			return text;
		}
	}

}