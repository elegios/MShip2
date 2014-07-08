using UnityEngine;
using System.Collections;

public class MShipMono : MonoBehaviour {

	public I GetIComponent<I>() where I : class {
		return GetComponent(typeof(I)) as I;
	}

	public I[] GetIComponents<I>() where I : class {
		return GetComponents(typeof(I)) as I[];
	}

}
