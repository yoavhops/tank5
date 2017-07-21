using UnityEngine;
using System.Collections;
using System;

public class tank_proproties : MonoBehaviour {

	public string tank_name = "p1";
    public int tank_photon_id = -1;
	public bool is_good_tank = false;
	public GameObject tank_canon;
	public GameObject tank_canon_shoting_edge;
    public GameObject health_bar;
    public int hp = 100;
    private int current_hp;
    private manager_script manager;
    public GameObject wheels; 
	// Use this for initialization
	void Start () {
        current_hp = hp;
        manager = GameObject.Find("the game").GetComponent<manager_script>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    internal void take_damage(float damage)
    {
        Debug.Log("taking damage : " + damage);
        current_hp -= (int)damage;

        if (current_hp < 0)
        {
            //todo end game;
            manager.lost(tank_photon_id);
        }

        Vector3 scale = health_bar.transform.localScale;
        scale.x = ((float)current_hp) / ((float)hp);
        health_bar.transform.localScale = scale;
    }
}
