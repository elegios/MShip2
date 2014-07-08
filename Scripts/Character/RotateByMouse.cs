using UnityEngine;
using System.Collections;

public class RotateByMouse : MShipMono {

	public float horizontalFactor = 1;
	public float verticalFactor = 1;
	public bool horizontal = true;
	public bool vertical = false;

	public float minY = -60;
	public float maxY = 60;

	public float vertRot = 0;

	void Update() {
		if (horizontal) {
			transform.Rotate(0, horizontalFactor * Input.GetAxis(Co.MOUSE_HORI), 0, Space.World);
		}

		if (vertical) {
			vertRot = Mathf.Clamp(vertRot + verticalFactor * -Input.GetAxis(Co.MOUSE_VERT), minY, maxY);
			transform.eulerAngles = new Vector3(vertRot, transform.eulerAngles.y, transform.eulerAngles.z);
		}
	}

}
