using UnityEngine;
using System.Collections;

[RequireComponent( typeof( PhotonView ) )]
public class manager_script : Photon.MonoBehaviour {

	public smoth_fllow camera; 
	private GameObject tank;/*the tank that it is his turn right now*/
	public GameObject[] player;
	public GameObject game_rect;
	public GameObject camera_follow_obj;
    public SpriteRenderer back_ground;
    public Color bg_start_tilt;
    public Color bg_end_tilt;

    public timer m_timer;
	public float focos_time_on_explotion = 3.0f;
	private int player_turn = 0;
	public bool master_player_is_first = false;
	public touch_script m_touch_screen;
    public float fire_work_time = 8.0f;
    public GameObject fire_work;

    private float start_time;

    public void Start()
    {
        /*
        Debug.Log("HEYYEYEY");
        StartCoroutine(__won());
        */
    }

	public void start_game()
    {
        player_turn = Random.Range (0, player.Length);
		if (master_player_is_first) {
			player_turn = 0;
		}
		photonView.RPC ("start_game_RPC", PhotonTargets.All, player_turn);
	}

	/**
	 * only will be called from the master client.
	 */
	[PunRPC]
	void start_game_RPC(int player_turn){

		Debug.Log ("player turn:");
		Debug.Log (player_turn);
		this.player_turn = player_turn;
		Debug.Log (player_turn);
		tank = player [player_turn];
		m_timer.start_timer (player [player_turn].GetComponent<tank_proproties>().tank_name, player.Length);
		/**
		 * each player settes is own tank to be good tank.
		 */
		player [PhotonNetwork.player.ID - 1].GetComponent<tank_proproties> ().is_good_tank = true;
        player[PhotonNetwork.player.ID - 1].GetComponent<tank_proproties>().tank_photon_id = PhotonNetwork.player.ID;
        player [PhotonNetwork.player.ID - 1].GetComponent<PhotonView>().photonView.TransferOwnership(PhotonNetwork.player.ID);
		player [PhotonNetwork.player.ID - 1].GetComponent<Rigidbody2D> ().isKinematic = false;
		player [PhotonNetwork.player.ID - 1].GetComponent<move_with_keys> ().enabled = true;
        player[PhotonNetwork.player.ID - 1].GetComponent<tank_proproties>().tank_canon.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.player.ID);
        gui_debug.debug("I am player : " + ( PhotonNetwork.player.ID - 1) );

        /*
        player[PhotonNetwork.player.ID - 1].GetComponent<tank_proproties>().wheels.transform.position = Vector3.zero;
        player[PhotonNetwork.player.ID - 1].GetComponent<tank_proproties>().wheels.SetActive(true);
        */
        m_touch_screen.start_game (player [PhotonNetwork.player.ID - 1]);
	}

	// Update is called once per frame
	void Update() {
        if (Input.GetKey("escape"))
        {
            Application.LoadLevel("menus");
        }
        if (Input.GetKey(KeyCode.D))
        {
            Debug.Break();
        }
	}

	public void follow_bullet(GameObject bullet)
    {
        camera.GetComponent<smoth_fllow>().follow(bullet, camera_follow_type.Bullet);
    }

    //bullet_exploded
	public void turn_over(){
		Debug.Log("pre");
        player_turn++;
        player_turn = player_turn % player.Length;
        tank = player[player_turn];
        StartCoroutine(__go_to_tank(focos_time_on_explotion));
	}

	IEnumerator __go_to_tank(float wait){
		yield return new WaitForSeconds (wait);
		//camera.target = good_tank;
		Vector3 newPos = new Vector3(-tank.transform.position.x, -tank.transform.position.y, game_rect.transform.position.z);
		game_rect.transform.position = newPos;
		camera.GetComponent<smoth_fllow>().follow(camera_follow_obj, camera_follow_type.Rect);
        turn_end(player_turn);
    }

	public void turn_end_by_timer(){
		player_turn++;
		player_turn = player_turn % player.Length;
        turn_end(player_turn);
    }
	
	public void turn_end(int player_turn)
	{
		Debug.Log ("player turn:");
		Debug.Log (player_turn);
		this.player_turn = player_turn;
		tank = player[player_turn];
		m_timer.start_timer (player [player_turn].GetComponent<tank_proproties>().tank_name, player.Length);
	}

	public bool is_my_turn(){

		if (player_turn == PhotonNetwork.player.ID - 1) {
			return true;
		}
		return false;
	}

    public bool is_player_turn(int photon_player_id)
    {
        if (player_turn == photon_player_id - 1)
        {
            return true;
        }
        return false;
    }


    public string player_name(){
		return tank.GetComponent<tank_proproties>().tank_name;
	}



    public void hit_tank(int tank, float damage)
    {
        photonView.RPC("reduce_tank_hp_RPC", PhotonTargets.All, tank, damage);
    }

    [PunRPC]
    public void reduce_tank_hp_RPC(int tank, float damage)
    {
        Debug.Log("reduce tank hp");
        Debug.Log("damage");
        player[tank].GetComponent<tank_proproties>().take_damage(damage);
    }




    public void won(int photon_player_id)
    {

        tank = player[PhotonNetwork.player.ID - 1];

        StartCoroutine(__go_to_tank(0.0f));


        for (int i = 0; i < player.Length; i++)
        {
            if (i != photon_player_id - 1)
            {
                player[i].SetActive(false);
            }
        }


        StartCoroutine(__won());
    }


    IEnumerator __won()
    {
        start_time = Time.time;



        while (true) {
            
            Color color = Color.Lerp(bg_start_tilt, bg_end_tilt, ((Time.time - start_time) / (fire_work_time / 2)));
            back_ground.color = new Color(color.r, color.g, color.b);

            if ((Time.time - start_time) > (fire_work_time / 4))
            {
                fire_work.SetActive(true);
            }


            if ( (Time.time - start_time) > (fire_work_time / 2))
            {
                break;
            }

            yield return new WaitForSeconds(0.03f);

        }

        Debug.Log("won1");
        yield return new WaitForSeconds(fire_work_time);
        Debug.Log("won2");
        PhotonNetwork.LeaveRoom();
        Application.LoadLevel("menus");
    }

    /*
    carefull this doesnt really give the id correctly.
    */
    public void lost(int photon_player_id)
    {
        if (PhotonNetwork.player.ID == photon_player_id) {
            PhotonNetwork.LeaveRoom();
            Application.LoadLevel("menus");
        }
    }

}
