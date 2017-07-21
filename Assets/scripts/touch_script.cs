using UnityEngine;
using System.Collections;
using UnityEngine.UI;


[RequireComponent(typeof(PhotonView))]
public class touch_script : Photon.MonoBehaviour
{

	public manager_script manager;

	private Vector3 mouseposition;
	private Vector3 prev_mouseposition;
	private Vector3 mouse_diff;
	public float speed = 0.1f;
	public float fade_move_speed = 0.6f;
	private GameObject good_tank;
	private GameObject good_tank_canon;
	private GameObject good_tank_canon_shoting_edge;
	public GameObject good_tank_bullet;
	public float shoting_force = 100.0f;
	public float jumping_force = 10000.0f;
	private float canon_angle;
	public GameObject arrow;
	
	enum clicked_on {nothing ,background, good_tank, bad_tank};
	private clicked_on mouse_on;
	private GameObject hit_game_object;

	private GameObject[] arrows; 
	public float size_of_arrow = 0.1f;
	public float distance_between_arrows = 0.05f;
	public int max_amount_of_arrows = 10;



	public float circle_r = 1.0f;
	public float circle_width = 0.02f;
	public float theta_scale = 0.1f;  
	public float min_distance = 0.3f;
	public Color circle_color;
	public Material touch_circle_mat;
	private LineRenderer lineRenderer;
	
	private bool shot = false;
	private Vector3 diff_norm;
	private Vector3 new_mouse_pos;

	public GameObject the_game;
	public GameObject ui_background;

	public left_panal_manager lpm;
	public float enable_tank_rigid_time = 0.1f;

	public bool my_turn = false;
	private bool game_started = false;

    public timer m_timer;


    public bool debug;
    public GameObject debug_tank;

	// Use this for initialization
	void Start () {
		mouse_on = clicked_on.nothing;
		arrows = new GameObject[max_amount_of_arrows];
		for (int i = 0; i < max_amount_of_arrows; i++) {
			arrows[i]=(GameObject) Instantiate(arrow);
			arrows[i].SetActive(false);
		}

        if (debug)
        {
            start_game(debug_tank);
        }

	}

	public void start_game(GameObject good_tank){

		this.good_tank = good_tank;
		good_tank_canon = good_tank.GetComponent<tank_proproties>().tank_canon;
		good_tank_canon_shoting_edge = good_tank.GetComponent<tank_proproties> ().tank_canon_shoting_edge;
		
		//circle part
		int size = (int)((2.0f * Mathf.PI) / theta_scale) + 2; //Total number of points in circle.
		lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.material = touch_circle_mat;
		lineRenderer.SetColors(circle_color, circle_color);
		lineRenderer.SetWidth(circle_width, circle_width);
		lineRenderer.SetVertexCount(size);
		lineRenderer.enabled = false;

		scale_circle (good_tank.transform.position, 1.0f);
		game_started = true;
        Debug.Log("started");
	}



	void scale_circle(Vector3 center, float r){
		
		int size = (int)((2.0f * Mathf.PI) / theta_scale) + 2; //Total number of points in circle.
		
		int i = 0;
		float x;
		float y;
		float theta;
		Vector3 pos;
		
		for(theta = 0; theta < 2 *  Mathf.PI; theta += theta_scale) {
			x = r * Mathf.Cos(theta);
			y = r * Mathf.Sin(theta);
			
			pos = new Vector3(center.x +  x,center.y + y, -0.1f);
			lineRenderer.SetPosition(i, pos);
			i+=1;
		}
		
		theta = 0;
		x = r * Mathf.Cos(theta);
		y = r * Mathf.Sin(theta);			
		pos = new Vector3(center.x +  x,center.y + y, -0.1f);
		lineRenderer.SetPosition(i, pos);
	}


	public void reset(){
		mouse_on = clicked_on.nothing;
		mouse_diff = Vector3.zero;
		lineRenderer.enabled = false;
		ui_background.GetComponent<ScrollRect>().enabled = true;
		shot = false;
	}

	// Update is called once per frame
	void Update () {


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, 0));
        float distance;
        xy.Raycast(ray, out distance);
        Vector3 mousePosition = ray.GetPoint(distance);

        Vector2 touchPos = new Vector2(mousePosition.x, mousePosition.y);
        /*
        Debug.Log(mousePosition);
        Debug.Log(Input.mousePosition);
        */

        if (!game_started){
			return;
		}
		int i;

        //pressed first time
        if (Input.GetMouseButtonDown (0)) {

			mouse_diff = Vector3.zero;
			prev_mouseposition = mousePosition;
            Debug.Log("1");
			if (manager.is_my_turn() || debug){
                get_clicked_on_3d(ref mouse_on, ref hit_game_object, mousePosition);
			}
			if (mouse_on == clicked_on.good_tank){

                Debug.Log("2");
                ui_background.GetComponent<ScrollRect>().enabled = false;

				/*
				Vector3 look_at_tank = new Vector3(hit_game_object.transform.position.x , 
				                                   hit_game_object.transform.position.y, 
				                                   Camera.main.transform.position.z);
				Camera.main.transform.position = look_at_tank;
				*/
			}

		} else {
			//conitune to press.
			if (Input.GetMouseButton(0)){

                if (manager.is_my_turn() == false)
                {
                    if (mouse_on == clicked_on.good_tank)
                    {
                        lineRenderer.enabled = false;

                        ui_background.GetComponent<ScrollRect>().enabled = true;

                        mouse_on = clicked_on.nothing;
                        for (i = 0; i < max_amount_of_arrows; i++)
                        {
                            arrows[i].SetActive(false);
                        }
                    }
                }
                else
                {
                    mouse_diff += prev_mouseposition - mousePosition;
                    prev_mouseposition = mousePosition;

                    if ((mouse_on == clicked_on.good_tank) && (lineRenderer.enabled == false))
                    {
                        lineRenderer.enabled = true;
                    }
                }
			}
			//pick finger up
			if (Input.GetMouseButtonUp (0))  {

				if (mouse_on == clicked_on.good_tank){
					ui_background.GetComponent<ScrollRect>().enabled = true;
				}

				mouse_on = clicked_on.nothing;
				for (i = 0;i < max_amount_of_arrows ; i++ ){
					arrows[i].SetActive(false);
				}

				if (shot){
					shot = false;
                    shot_f();
				}
			}
		}
		
		//Debug.Log (mouse_on);

		if (mouse_on == clicked_on.background) {
			mouse_diff = mouse_diff * fade_move_speed;
			if (mouse_diff.sqrMagnitude > 0.01) {
				Camera.main.transform.position += (mouse_diff * (speed * Time.deltaTime));
			}
			return;
		}

		if (mouse_on == clicked_on.good_tank) {

			lineRenderer.enabled = true;
			shot = true;

			new_mouse_pos = new Vector3(mousePosition.x, mousePosition.y, 9.0f);
			new_mouse_pos = Camera.main.ScreenToWorldPoint(new_mouse_pos);

            /**
             * if camera is prespective 
             */
            new_mouse_pos = mousePosition;

            float distnace = Vector3.Distance(new_mouse_pos, good_tank.transform.position);
			int amount_of_arrows = (int)(distnace / size_of_arrow);
			Vector3 diff = good_tank.transform.position - new_mouse_pos;
			diff_norm = diff.normalized * -1;
			if (amount_of_arrows >= max_amount_of_arrows){
				distnace = max_amount_of_arrows * size_of_arrow;
				amount_of_arrows = max_amount_of_arrows;

				new_mouse_pos = new Vector3(good_tank.transform.position.x + diff_norm.x * distnace,
				                      good_tank.transform.position.y + diff_norm.y * distnace,
				                      good_tank.transform.position.z);
				diff = good_tank.transform.position - new_mouse_pos;
			}

			if (distnace < min_distance){
				shot = false;
				for ( i = 0; i < amount_of_arrows; i++ ){
					arrows[i].SetActive(false);
				}
				lineRenderer.enabled = false;
				return;
			}

			scale_circle (good_tank.transform.position, distnace);
			canon_angle = Mathf.Atan2(diff.y,diff.x) * Mathf.Rad2Deg;

			good_tank_canon.transform.rotation = Quaternion.AngleAxis(canon_angle, Vector3.forward);
			for (i = 0; i < amount_of_arrows; i++ ){
				arrows[i].SetActive(true);

				arrows[i].transform.rotation = Quaternion.AngleAxis(canon_angle, Vector3.forward);

				Vector3 arrow_pos = new Vector3(new_mouse_pos.x + (diff.x * i)/amount_of_arrows, new_mouse_pos.y + (diff.y * i)/amount_of_arrows, 0f);
				arrows[i].transform.position = arrow_pos;
			}
			for (;i < max_amount_of_arrows ; i++ ){
				arrows[i].SetActive(false);
			}
		}
		else{
			lineRenderer.enabled = false;
		}
	}


	void get_clicked_on(ref clicked_on mouse_on, ref GameObject hit_game_object)
    {
		Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);

        Debug.Log(LayerMask.NameToLayer("tank_p1"));

        int layerMask = ((1 << LayerMask.NameToLayer("tank_p1")) | (1 << LayerMask.NameToLayer("tank_p2")));
		
		RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, Mathf.Infinity, layerMask);
		if (hit == null || hit.collider == null) {
			mouse_on = clicked_on.nothing;
			hit_game_object = null;
			return;
		}
		hit_game_object = hit.collider.gameObject;
		if (hit.collider.name == "background") {
			mouse_on = clicked_on.background;
			return;
		}
		
		if (hit.collider.gameObject.GetComponent<get_tank_proproties>().tank.GetComponent<tank_proproties>().is_good_tank) {
			mouse_on = clicked_on.good_tank;
			return;
		}
		mouse_on = clicked_on.nothing;
	}

    void get_clicked_on_3d(ref clicked_on mouse_on, ref GameObject hit_game_object, Vector3 mousePosition)
    {

        int layerMask = ((1 << LayerMask.NameToLayer("tank_p1")) | (1 << LayerMask.NameToLayer("tank_p2")));

        Vector2 touchPos = new Vector2(mousePosition.x, mousePosition.y);
        Collider2D[] currentButtons = Physics2D.OverlapPointAll(touchPos, layerMask);

        Debug.Log(touchPos);
        foreach (Collider2D col in currentButtons)
        {
            get_tank_proproties tank_prop = col.gameObject.GetComponent<get_tank_proproties>();

            if (tank_prop && tank_prop.tank.GetComponent<tank_proproties>().is_good_tank)
            {
                Debug.Log("3");
                mouse_on = clicked_on.good_tank;
                return;
            }
        }

        mouse_on = clicked_on.background;
    }

    void disable_tank_rigids(GameObject tank){
		Collider2D[] coliders;
		coliders = tank.GetComponentsInChildren<Collider2D> ();
		foreach (Collider2D collide in coliders) {
			if (collide.gameObject == tank){
				continue;
			}
			//disable wheels
			collide.isTrigger = true;
		}
		StartCoroutine(enable_tank_rigids(tank));
	}

	IEnumerator enable_tank_rigids(GameObject tank){
		yield return new WaitForSeconds(enable_tank_rigid_time);
		Collider2D[] coliders;
		coliders = tank.GetComponentsInChildren<Collider2D> ();
		foreach (Collider2D collide in coliders) {
			if (collide.gameObject == tank){
				continue;
			}
			collide.isTrigger = false;
		}
	}
    
    public void shot_f()
    {
        if (manager.is_player_turn(PhotonNetwork.player.ID))
        {
            m_timer.stop_timer();
        }
        Vector2 canon_angle_vec2 = new Vector2(diff_norm.x, diff_norm.y);
        canon_angle_vec2 *= -1;
        float distnace = Vector3.Distance(new_mouse_pos, good_tank.transform.position);
        float distnace_ratio = distnace / (max_amount_of_arrows * size_of_arrow);

        if (distnace > min_distance)
        {
            Vector2 fire_dir = canon_angle_vec2 * shoting_force * distnace_ratio;

            if (lpm.btn.name == "jump")
            {
                good_tank.GetComponent<Rigidbody2D>().AddForce(fire_dir);
                disable_tank_rigids(good_tank);
            }

            if (lpm.btn.name == "weapon")
            {

                Debug.Log(good_tank_canon_shoting_edge.transform.position);
                // 2 is the amount_of_players, beacuse right before the tanks layer, comes the bullets layers

                Debug.Log("layer");
                Debug.Log(good_tank.layer);

                if (manager.is_player_turn(PhotonNetwork.player.ID))
                {

                    Debug.Log("shoting at my turn.");

                    GameObject new_bullet = PhotonNetwork.Instantiate("bullet/" + good_tank_bullet.name, good_tank_canon_shoting_edge.transform.position,
                        Quaternion.identity, 0, null);

                    new_bullet.layer = good_tank.layer - 2;

                    /*only controled by owner*/
                    new_bullet.GetComponent<Rigidbody2D>().isKinematic = false;
                    new_bullet.GetComponent<Rigidbody2D>().AddForce(fire_dir);
                    bullet_script new_bullet_script = new_bullet.GetComponent<bullet_script>();

                    photonView.RPC("new_bullet_RPC", PhotonTargets.All, new_bullet.GetComponent<PhotonView>().viewID, true, player_to_bullet_layer(PhotonNetwork.player.ID));

                    Debug.Log("shoting amount");
                    Debug.Log(new_bullet_script.bullet_amount);

                    StartCoroutine(shot_bullets(PhotonNetwork.player.ID, fire_dir, good_tank_canon_shoting_edge.transform.position,
                        good_tank_bullet.name, good_tank.layer - 2, new_bullet_script.bullet_amount - 1, new_bullet_script.wait_between_bullets));



                    /*
                    photonView.RPC("shot_RPC", PhotonTargets.All, PhotonNetwork.player.ID, fire_dir, good_tank_canon_shoting_edge.transform.position,
                    good_tank_bullet.name, good_tank.layer - 2);
                    */
                }
            }
        }
    }
    /*
    [PunRPC]
    public void shot_RPC(int photon_player_id, Vector2 fire_dir, Vector3 canon_edge_pos, string bullet_type, int bullet_layer)
    {
        bullet_script.init();
        Debug.Log("shoting rpc");
        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("shoting master");
            StartCoroutine(shot_bullets(photon_player_id, fire_dir, canon_edge_pos, bullet_type, bullet_layer, new_bullet_script.bullet_amount - 1, new_bullet_script.wait_between_bullets));
        }
    }
    */
    IEnumerator shot_bullets(int photon_player_id, Vector2 fire_dir, Vector3 canon_edge_pos, string bullet_type, int bullet_layer, int amount, float wait)
    {
        Debug.Log(amount);
        for (int i = 0; i < amount; i++)
        {
            yield return new WaitForSeconds(wait);
            GameObject new_bullet = PhotonNetwork.Instantiate("bullet/" + bullet_type, canon_edge_pos, Quaternion.identity, 0, null);
            new_bullet.layer = bullet_layer;
            photonView.RPC("new_bullet_RPC", PhotonTargets.All, new_bullet.GetComponent<PhotonView>().viewID, false, player_to_bullet_layer(photon_player_id));

            new_bullet.GetComponent<Rigidbody2D>().isKinematic = false;
            new_bullet.GetComponent<Rigidbody2D>().AddForce(fire_dir);

            Debug.Log("shoting left");
            Debug.Log(amount - i);
        }
    }
    
    int player_to_bullet_layer(int photon_player_id)
    {
        if (photon_player_id == 1)
        {
            return LayerMask.NameToLayer("bullet_p1");
        }
        if (photon_player_id == 2)
        {
            return LayerMask.NameToLayer("bullet_p2");
        }
        Debug.LogError("problem");
        return -1;
    }


    [PunRPC]
    public void new_bullet_RPC(int view_id, bool first, int layer)
    {
        GameObject new_bullet = PhotonView.Find(view_id).gameObject;
        if (first)
        {
            manager.follow_bullet(new_bullet);
            bullet_script.init();
            //todo timer stop
            m_timer.stop_timer();
        }
        new_bullet.transform.parent = the_game.transform;
    }
}
