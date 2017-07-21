using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class weapon_manger : MonoBehaviour {

	public GameObject weapon;
	public canvas_util util;
    public touch_script touch;
    public GameObject touch_obj;

    public GameObject rocket_bullet;
    public GameObject grenade_bullet;
    public GameObject mag_bullet;
    public GameObject split_bullet;


    // Use this for initialization
    void Start () {
		weapon.GetComponent<Button>().colors = util.cb_pressed;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	
	public void weapon_pressed(GameObject obj)
	{
		weapon.GetComponent<Button>().colors = util.cb_not_pressed;

		weapon = obj;

		weapon.GetComponent<Button>().colors = util.cb_pressed;

        Debug.Log(weapon.name);
        
        if (weapon.name == "rocket")
        {
            touch.good_tank_bullet = rocket_bullet;
        }

        if (weapon.name == "grenade")
        {
            touch.good_tank_bullet = grenade_bullet;
        }

        if (weapon.name == "mag")
        {
            touch.good_tank_bullet = mag_bullet;
        }

        if (weapon.name == "split")
        {
            touch.good_tank_bullet = split_bullet;
        }

    }
	
	
	void OnDisable() {
		print("OnDisable");
        touch_obj.SetActive (true);
	}
	
	void OnEnable() {
		print("OnEnable");
        touch_obj.SetActive (false);
	}

}
