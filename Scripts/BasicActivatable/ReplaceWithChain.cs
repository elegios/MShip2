using UnityEngine;
using System.Collections;

public class ReplaceWithChain : MonoBehaviour {

	public Transform inputPoint;
	public Transform[] outputPoints;
	public GameObject target;
	public ChainActivator[] activators;

	public GameObject outputPrefab;
	public GameObject inputPrefab;

	void Awake() {
		GameObject input = (GameObject) Instantiate(inputPrefab, inputPoint.position, inputPoint.rotation);
		input.transform.parent = transform;
		input.networkView.viewID = inputPoint.networkView.viewID;
		Destroy(inputPoint.gameObject);
		ChainInput chainInput = input.GetComponent<ChainInput>();
		chainInput.outputs = new ChainOutput[outputPoints.Length];
		chainInput.target = target;

		for (int i = 0; i < outputPoints.Length; i++) {
			GameObject output = (GameObject) Instantiate(outputPrefab, outputPoints[i].position, outputPoints[i].rotation);
			output.transform.parent = transform;
			output.networkView.viewID = outputPoints[i].networkView.viewID;
			Destroy(outputPoints[i].gameObject);
			chainInput.outputs[i] = output.GetComponent<ChainOutput>();
		}

		foreach (ChainActivator activator in activators) {
			activator.input = chainInput;
		}
	}

}
