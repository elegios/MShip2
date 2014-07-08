using UnityEngine;
using System.Collections;

public class PressButton : MShipMono {

	public float normalScale = 0.5f;
	public float pressScale = 0.4f;

	public float changeSpeed = 7;

	private float targetScale = 0.5f;

	void Update() {
		transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(targetScale, targetScale, transform.localScale.z), changeSpeed * Time.deltaTime);
		if (transform.localScale.x < 0.41f && targetScale == pressScale)
			targetScale = normalScale;

		if (transform.localScale.x < 0.01f && targetScale == 0)
			Destroy(gameObject);
	}

	public void DoPress() {
		targetScale = pressScale;
	}

	public void DoDestroy() {
		targetScale = 0;
	}

}