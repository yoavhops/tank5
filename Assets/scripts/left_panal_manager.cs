using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class left_panal_manager : MonoBehaviour {
	
	public canvas_util util;
	
	public GameObject btn;
	
	void Start () {
		btn.GetComponent<Button>().colors = util.cb_pressed;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	
	public void left_menu_pressed(GameObject obj)
	{
		btn.GetComponent<Button>().colors = util.cb_not_pressed;
		
		btn = obj;
		
		btn.GetComponent<Button>().colors = util.cb_pressed;
		
	}

}
