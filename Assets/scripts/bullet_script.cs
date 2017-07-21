using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum bullet_type_e { normal_rocket, grenade , split, split_child };

[RequireComponent(typeof(PhotonView))]
public class bullet_script : Photon.MonoBehaviour
{
    private Vector2 wind_str;
    private pixel_control pixel_cnr;
    public GameObject explosion;
	private manager_script game_manager;
    public AnimationCurve dmg_graph;
    public float damage = 40;
    public float hit_radius = 10.0f;
    private Rigidbody2D rigid;
    public bullet_type_e bullet_type;
    private Text text;
    private GameObject text_game_obj;
    private int counter_tmp = 5;
    public float find_pos_step = 0.01f;

    private static int count_destroyed = 0;
    public int bullet_amount = 1;
    public float wait_between_bullets = 0.3f;

    public float split_force = 100.0f;

    private IEnumerator timer_coroutine = null;

    // Use this for initialization
    void Start ()
    {
        Debug.Log("created " + gameObject.name);
        wind_str = GameObject.Find("wind").GetComponent<wind>().wind_str;
		pixel_cnr = GameObject.Find("ground").GetComponent<pixel_control>();
        game_manager = GameObject.Find("the game").GetComponent<manager_script>();
        Debug.Log (game_manager);
        GameObject the_game = GameObject.Find("the game");
        text_game_obj = GameObject.Find("bullet_text");
        transform.parent = the_game.transform;

        rigid = GetComponent<Rigidbody2D>();

        if (bullet_type == bullet_type_e.grenade)
        {
            text = text_game_obj.GetComponent<Text>();
            timer_coroutine = counter();
            StartCoroutine(timer_coroutine);
        }

    }

    public static void init()
    {
        count_destroyed = 0;
    }

	
	// Update is called once per frame
	void Update () {
        Vector2 dir = rigid.velocity; 
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);


        if (bullet_type == bullet_type_e.grenade)
        {
            text_game_obj.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        }

        if (bullet_type == bullet_type_e.split)
        {
            if (photonView.isMine)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("split bullet spliting");
                    split_bullet();
                }
            }
        }

        /*
        if (pixel_cnr.has_ground_in_pos(transform.position.x, transform.position.y))
        {
            __OnCollisionEnter2D();
        }
        */

    }

	void FixedUpdate(){
        if (photonView.isMine)
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce(wind_str);
            if (transform.position.y < -6)
            {
                Debug.Log("position out of bound");
                destory_this();
            }
        }
	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        //__OnCollisionEnter2D();
        Debug.Log("hit:");
        Debug.Log(coll.gameObject.name);
        Vector2 avg_pos = Vector2.zero;
        Debug.Log("Contact:");
        if (coll.gameObject.layer == LayerMask.NameToLayer("ground"))
        {

            if (bullet_type != bullet_type_e.grenade)
            {
                GetComponent<Rigidbody2D>().isKinematic = true;
                GetComponent<SpriteRenderer>().enabled = false;


                foreach (ContactPoint2D contact in coll.contacts)
                {
                    Debug.Log(contact.point);
                    avg_pos += contact.point;
                }
                avg_pos = avg_pos / coll.contacts.Length;
                Vector3 avg_pos_3 = new Vector3(avg_pos.x, avg_pos.y, transform.position.z);


                find_hit_pos(coll.relativeVelocity, avg_pos_3);

                __OnCollisionEnter2D();
            }
        }


    }

    public void find_hit_pos(Vector3 relativeVelocity, Vector3 contact_pos)
    {
        Vector2 dir = relativeVelocity.normalized;

        Debug.Log("COLLIDED");
        Debug.Log("vlocity:");
        Debug.Log(dir);
        float dis = 0;
        Vector2 pos = Vector2.zero;

        pos.x = contact_pos.x;
        pos.y = contact_pos.y;

        Vector2 org_pos = new Vector2(contact_pos.x, contact_pos.y);

        for (dis = 0; dis < 1.0f; dis += find_pos_step)
        {
            pos = org_pos + Vector2.ClampMagnitude(dir, dis);
            Debug.Log(pos);
            if (pixel_cnr.has_ground_in_pos(pos.x, pos.y)) {
                break;
            }
        }
        if (dis > 1.0f)
        {
            Debug.LogError("problem");

            contact_pos.x = org_pos.x;
            contact_pos.y = org_pos.y;
        }
        else
        {
            contact_pos.x = pos.x;
            contact_pos.y = pos.y;
        }

        transform.position = contact_pos;
    }

    void __OnCollisionEnter2D()
    {
        switch (bullet_type)
        {
            case bullet_type_e.split:
                Debug.Log("__OnCollisionEnter2D split");
                split_bullet();
                break;
            default:
                Debug.Log("__OnCollisionEnter2D explode");
                explode();
                break;
        }
    }

    void explode()
    {
        Debug.Log("explode");
        photonView.RPC("bulllet_collided_RPC", PhotonTargets.All, transform.position);

        destory_this();
    }

    [PunRPC]
    public void bulllet_collided_RPC(Vector3 position)
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GameObject explo = Instantiate(explosion) as GameObject;
        explo.transform.position = position;
        if (PhotonNetwork.isMasterClient)
        {
            hit_tanks(position);
        }
    }

    void destory_this()
    {
        Debug.Log("Destroy this.");


        if (bullet_type != bullet_type_e.split)
        {
            photonView.RPC("destory_this_RPC", PhotonTargets.All);
        }
        /*done by owner,*/
        PhotonNetwork.Destroy(this.gameObject);
    }

    [PunRPC]
    public void destory_this_RPC()
    {
        count_destroyed++;
        Debug.Log(count_destroyed);
        Debug.Log(gameObject.name);
        if (bullet_type == bullet_type_e.grenade)
        {
            text.text = "";
        }

        if (count_destroyed == bullet_amount)
        {
            game_manager.turn_over();
        }
    }

    void hit_tanks(Vector3 position)
    {
        Debug.Log(this);
        Debug.Log(this.gameObject);
        Debug.Log(game_manager);
        for (int i = 0; i < game_manager.player.Length; i++)
        {
            float dis = Vector3.Distance(game_manager.player[i].transform.position, position);
            
            if (dis - hit_radius < 0)
            {
                game_manager.hit_tank(i, dmg_graph.Evaluate(dis / hit_radius) * damage);
            }
        }
    }

    /*
     *
     */
    IEnumerator counter()
    {
        Debug.Log("Start_counter");
        while (true)
        {
            text.text = "" + counter_tmp;
            yield return new WaitForSeconds(1);
            counter_tmp--;
            if (counter_tmp < 0)
            {
                timer_coroutine = null;
                break;
            }
        }
        if (photonView.isMine)
        {
            explode();
        }
    }


    private void split_bullet()
    {
        /*create two inst*/

        GameObject new_bullet_1 = PhotonNetwork.Instantiate("bullet/" + "split_child_bullet", transform.position,
            Quaternion.identity, 0, null);
        GameObject new_bullet_2 = PhotonNetwork.Instantiate("bullet/" + "split_child_bullet", transform.position,
            Quaternion.identity, 0, null);

        new_bullet_1.layer = gameObject.layer;
        new_bullet_2.layer = gameObject.layer;
        new_bullet_1.transform.parent = gameObject.transform.parent;
        new_bullet_2.transform.parent = gameObject.transform.parent;


        /*create follow inst*/

        GameObject follow_obj_1 = PhotonNetwork.Instantiate("follow_obj", transform.position,
            Quaternion.identity, 0, null);

        follow_obj_1.GetComponent<follow_obj>().objs.Add(new_bullet_1);
        follow_obj_1.GetComponent<follow_obj>().objs.Add(new_bullet_2);

        /*change inst phyc*/

        new_bullet_1.GetComponent<Rigidbody2D>().isKinematic = false;
        new_bullet_2.GetComponent<Rigidbody2D>().isKinematic = false;


        new_bullet_1.GetComponent<Rigidbody2D>().AddForce((Vector2.up * 4 + Vector2.left) * split_force);

        new_bullet_2.GetComponent<Rigidbody2D>().AddForce((Vector2.up * 4 + Vector2.right) * split_force);

        /*diable rigid for for a few ms.*/

        /*init set 2 - no need, already on script frmo unity ide*/


        /*destroy this*/

        /*done by owner,*/
        PhotonNetwork.Destroy(this.gameObject);


    }

}
