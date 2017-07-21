using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class timer : Photon.MonoBehaviour
{

    public int turn_time = 20;
    private int turn_time_tmp;
    private Text text;
	public Text whos_turn;
	private int turn_index = 0;
	public manager_script manager;
    private int turn_end_approved_amount = 0;
    private int amount_of_players = 0;
    private IEnumerator timer_coroutine = null;
    // Use this for initialization
    void Start()
    {
		text = GetComponent<Text>();
    }

	public void start_timer(string player_name, int amount_of_players){
        turn_end_approved_amount = 0;
		turn_time_tmp = turn_time;
        this.amount_of_players = amount_of_players;
        whos_turn.text = manager.player_name() + "'s turn";
		if (timer_coroutine != null) {
			StopCoroutine (timer_coroutine);
            timer_coroutine = null;
        }
		timer_coroutine = counter ();
		StartCoroutine(timer_coroutine);
	}


    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator counter()
    {
		Debug.Log ("Start_counter");
        while (true)
        {
            text.text = turn_time_tmp.ToString();
            yield return new WaitForSeconds(1);
            turn_time_tmp--;
            if (turn_time_tmp <= 0)
            {
                turn_time_tmp = 0;
                text.text = turn_time_tmp.ToString();

                timer_coroutine = null;
                photonView.RPC("turn_end_RPC", PhotonTargets.All);

                break;
            }
        }
    }

    [PunRPC]
    void turn_end_RPC()
    {
        turn_end_approved_amount++;
        Debug.Log("HEY1");
        if (turn_end_approved_amount == amount_of_players)
        {
            Debug.Log("HEY2");
            manager.turn_end_by_timer();
        }
    }

    public void stop_timer()
    {
        Debug.Log("Stop_timer1");
        if (timer_coroutine != null)
        {
            Debug.Log("Stop_timer2");
            StopCoroutine(timer_coroutine);
            timer_coroutine = null;
        }
    }

}
