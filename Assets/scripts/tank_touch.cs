using UnityEngine;
using System.Collections;

public class tank_touch : MonoBehaviour {


    Collider2D this_collider;
    // Use this for initialization
    void Start () {
        this_collider = GetComponent<Collider2D>();
	}
	
	// Update is called once per frame
	void Update () {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, 0));
        float distance;
        xy.Raycast(ray, out distance);
        Vector3 wp = ray.GetPoint(distance);

        //Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 touchPos = new Vector2(wp.x, wp.y);
        Collider2D[] currentButtons = Physics2D.OverlapPointAll(touchPos);
        
        Debug.Log(touchPos);
        foreach (Collider2D col in currentButtons)
        {
            if(col == this_collider)
            {
                Debug.Log("true");
            } 
        }

    }



    void OnMouseDown()
    {
        Debug.Log("clicked!!!!!");
    }

}
