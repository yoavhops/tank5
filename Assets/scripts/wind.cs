using UnityEngine;
using System.Collections;

public class wind : MonoBehaviour {

    public Vector2 wind_str;
    public float partical_str = 40.0f;
    public GameObject particle_system;
	// Use this for initialization
	void Start () {
 
		
		particle_system.transform.rotation = Quaternion.Euler(90 - wind_str.x * partical_str, 90, 90);
		
		particle_system.GetComponent<ParticleSystem> ().GetComponent<ParticleSystem>().startSpeed = Mathf.Abs(wind_str.x * (partical_str / 6)) /2 + 1.5f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
