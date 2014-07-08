using UnityEngine;
using System.Collections;

public class DescriptiveText : MShipMono, IDescriptiveText {

	public string text;

	string IDescriptiveText.text {get {return text;}}

}