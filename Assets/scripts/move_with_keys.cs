using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class move_with_keys : MonoBehaviour 
{

    public float speed = 3.0f;
	public float up = 0.01f;

    private WheelJoint2D[] wheels;
    public float motor_speed = 0;
    private float motor_speed_last = 0;

    // Use this for initialization
    void Start () {
        StartCoroutine("flip_tank");
        wheels = GetComponents<WheelJoint2D>();
    }

	// Update is called once per frame
	void Update () {
		/*
        if (Input.GetAxis("Horizontal")!=0 || Input.GetAxis("Vertical")!= 0)
        {
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
            transform.position += move * speed * Time.deltaTime;
			//gameObject.rigidbody2D.AddForce(transform.right * speed);
		}
		*/
        if (motor_speed_last!= motor_speed)
        {
            int i;
            for (i = 0; i < wheels.Length; i++)
            {
                wheels[i].useMotor = true;
                JointMotor2D m = wheels[i].motor;
                m.motorSpeed = motor_speed;
                wheels[i].motor = m;
            }
            motor_speed_last = motor_speed;
        }
	}

	
	void FixedUpdate(){
	/*
		float move = Input.GetAxis("Horizontal");
		if (move != 0) {
			rigidbody2D.velocity = new Vector2 (move * speed, rigidbody2D.velocity.y + up);
		}
	*/
	}


    /*
	 * if tank is upside,
	 * flip it.
	 */
    IEnumerator flip_tank()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);
                if (GetComponent<Rigidbody2D>().velocity.magnitude < 0.01f)
                {
                    break;
                }
            }
            Vector3 rot = transform.rotation.eulerAngles;
            if (transform.rotation.eulerAngles.z > 90 && transform.rotation.eulerAngles.z < 270)
            {
                rot.Set(rot.x, rot.y, rot.z + 180);
                transform.rotation = Quaternion.Euler(rot);
            }
        }
    }

}
