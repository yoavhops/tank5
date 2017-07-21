using UnityEngine;
using System.Collections;

public class camera_mover : MonoBehaviour {

	public GameObject m_camera;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.hasChanged) {
			transform.hasChanged = false;
			Vector3 newPos = new Vector3(-transform.position.x, -transform.position.y, m_camera.transform.position.z);
			m_camera.transform.position = newPos;
		}
	}
}
