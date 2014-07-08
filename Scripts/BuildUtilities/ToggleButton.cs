using UnityEngine;
using System.Collections;

public class ToggleButton : MShipMono {

	public float onScale = 0.4f;
	public float offScale = 0.5f;

	public float changeSpeed = 2;

	private bool onInternal;
	public bool on {
		get { return onInternal; }
		set {
			if (value) {
				targetScale = onScale;
			} else {
				targetScale = offScale;
			}
			onInternal = value;
		}
	}

	private float targetScale;

	void Awake() {
		on = false;
	}

	void Update() {
		transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(targetScale, targetScale, transform.localScale.z), changeSpeed * Time.deltaTime);
		if (transform.localScale.x < 0.01f && targetScale == 0)
			Destroy(gameObject);
	}

	public void DoDestroy() {
		targetScale = 0;
	}

}