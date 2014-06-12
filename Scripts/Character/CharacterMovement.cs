using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkView))]
public class CharacterMovement : MonoBehaviour {

	public float gravity = 1;
	public float speedChangeFactor = 2;
	public float speed = 2;
	public float timeTillPlatformLoss = 5;
	public float jumpPower = 3;

	private CharacterController controller;

	private Vector3 velocity;
	private float hori = 0;
	private float vert = 0;
	private bool jump = false;

	private Transform platform;
	private Vector3 localRelativePoint;
	private Quaternion lastPlatformRotation;
	private NetworkViewID platformID;
	private float platformConnectionFactor;

	private class ToSet {
		public Vector3 pos;
		public Vector3 rot;
		public NetworkViewID id;
		public Vector3 locPos;
		public Vector3 velocity;
		public float hori;
		public float vert;
		public bool toSet = false;
	}
	
	private ToSet toSet = new ToSet();

	void Awake() {
		controller = GetComponent<CharacterController>();
	}

	void Update() {
		UpdateFromNetwork();
		TakeInput();

		MoveWithPlatform();

		UpdateVelocity();

		DoMove();

		PlatformEndOfUpdate();
	}

	void DoMove() {
		controller.Move(velocity * Time.deltaTime);
		//Platform related stuff is set in OnControllerColliderHit

		if (controller.isGrounded)
			velocity.y = 0;
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
		Transform grandParent = hit.transform.root;
		if (grandParent.rigidbody != null) {
			platformID = grandParent.networkView.viewID;
			platform = grandParent;
			platformConnectionFactor = 1;
		}
	}

	void TakeInput() {
		if (!networkView.isMine)
			return;

		hori = Input.GetAxis(Co.HORIZONTAL);
		vert = Input.GetAxis(Co.VERTICAL);
		jump = Input.GetButtonDown(Co.JUMP);
	}

	void UpdateVelocity() {
		Vector3 targetVelocity = hori * transform.right * speed;
		targetVelocity += vert * transform.forward * speed;

		float ySpeed = velocity.y;
		velocity = Vector3.Lerp(new Vector3(velocity.x, 0, velocity.z), targetVelocity, speedChangeFactor * Time.deltaTime);
		velocity.y = ySpeed - gravity * Time.deltaTime;

		if (jump && controller.isGrounded)
			velocity.y = jumpPower;
	}

	void MoveWithPlatform() {
		if (platformConnectionFactor == 0 || platform == null)
			return;

		Vector3 move = (platform.TransformPoint(localRelativePoint) - transform.position) * platformConnectionFactor;
		transform.position += move;
		float yRot = (Quaternion.Inverse(lastPlatformRotation) * platform.rotation).eulerAngles.y;
		if (yRot > 180)
			yRot -= 360; 
		transform.Rotate(0, yRot * platformConnectionFactor, 0, Space.World);

		platformConnectionFactor = Mathf.Max(0, platformConnectionFactor - 1/timeTillPlatformLoss*Time.deltaTime);
	}

	void PlatformEndOfUpdate() {
		if (platform == null)
			return;

		localRelativePoint = platform.InverseTransformPoint(transform.position);
		lastPlatformRotation = platform.rotation;
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		toSet.pos = transform.position;
		toSet.rot = transform.eulerAngles;
		toSet.id = platformID;
		toSet.hori = hori;
		toSet.vert = vert;
		toSet.velocity = velocity;
		toSet.locPos = localRelativePoint;
		stream.Serialize(ref toSet.pos);
		stream.Serialize(ref toSet.rot);
		stream.Serialize(ref toSet.id);
		stream.Serialize(ref toSet.hori);
		stream.Serialize(ref toSet.vert);
		stream.Serialize(ref toSet.locPos);
		stream.Serialize(ref toSet.velocity);
		toSet.toSet = stream.isReading;
	}

	void UpdateFromNetwork() {
		if (!toSet.toSet)
			return;

		transform.position = toSet.pos;
		transform.eulerAngles = toSet.rot;
		velocity = toSet.velocity;
		hori = toSet.hori;
		vert = toSet.vert;
		localRelativePoint = toSet.locPos;
		if (platformID != toSet.id) {
			platformID = toSet.id;
			platform = NetworkView.Find(platformID).transform;
		}

		toSet.toSet = false;
	}

}