using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PhotonView))]
public class follow_obj : Photon.MonoBehaviour
{

    public List<GameObject> objs;
    private manager_script game_manager;
    private bool all_null = false;
    // Use this for initialization
    void Start ()
    {
        game_manager = GameObject.Find("the game").GetComponent<manager_script>();
        game_manager.follow_bullet(this.gameObject);
    }
	
	// Update is called once per frame
	void Update () {

        if (photonView.isMine)
        {
            if (all_null)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }

    void FixedUpdate()
    {
        if (photonView.isMine)
        {
            if (all_null == false)
            {
                Vector3 avg = Vector3.zero;
                int count = 0;
                foreach (GameObject obj in objs)
                {
                    if (obj != null)
                    {
                        avg += obj.transform.position;
                        count++;
                    }
                }
                avg = avg / count;

                if (count == 0)
                {
                    all_null = true;
                }
                else
                {
                    transform.position = avg;
                }
            }
        }
    }

}
