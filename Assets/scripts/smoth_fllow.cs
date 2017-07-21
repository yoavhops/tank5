using UnityEngine;
using System.Collections;
 

public enum camera_follow_type{Rect, Bullet};
public class smoth_fllow : MonoBehaviour {
	
	public float dampTime_scroll_rect = 0.2f;
	public float dampTime_bullet_follow = 0.001f;
	private Vector3 velocity = Vector3.zero;
	public GameObject target;
	public float dampTime = 0.0f;

	void Start(){
		dampTime = dampTime_scroll_rect;
	}

	// Update is called once per frame
	void Update () 
	{
		if (target)
		{
			Vector3 point = GetComponent<Camera>().WorldToViewportPoint(target.transform.position);
			Vector3 delta = target.transform.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
			Vector3 destination = transform.position + delta;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
		}
		
	}
	public void follow(GameObject obj, camera_follow_type type){
		if (type == camera_follow_type.Rect) {
			dampTime = dampTime_scroll_rect;
		}
		if (type == camera_follow_type.Bullet) {
			dampTime = dampTime_bullet_follow;
		}
		target = obj;
	}


}