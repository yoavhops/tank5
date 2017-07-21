using UnityEngine;
using System.Collections;

public class explosion_script : MonoBehaviour {

	public Sprite[] sprites;
	public float speed = 0.3f;
	public pixel_control pixel_cntr;
    public int[] destory_ground_index;
    public Sprite explotion_destroy;
    public bool use_explotion_destroy = true;
    public bool add_dirt = false;



    // Use this for initialization
    void Start () {

        pixel_cntr = GameObject.Find("ground").GetComponent<pixel_control>();
        StartCoroutine("explode");
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator explode()
	{
		int i = 0, j = 0;

        for (j = 0; j < destory_ground_index.Length; j++)
        {
            i = destory_ground_index[j];
            pixel_cntr.kill_by_sub_texture_shape_world_pos(transform.localPosition.x, transform.localPosition.y,
                (int)sprites[i].textureRect.width, (int)sprites[i].textureRect.height,
                (int)sprites[i].textureRect.x, (int)sprites[i].textureRect.y, sprites[i].texture);
        }

        /*black and white image*/
        if (use_explotion_destroy)
        {
            pixel_cntr.kill_by_sub_texture_shape_world_pos(transform.localPosition.x, transform.localPosition.y,
                (int)explotion_destroy.textureRect.width, (int)explotion_destroy.textureRect.height,
                (int)explotion_destroy.textureRect.x, (int)explotion_destroy.textureRect.y, explotion_destroy.texture, true);

        }
        
        /*black and white image*/
        if (add_dirt)
        {
            pixel_cntr.kill_by_sub_texture_shape_world_pos(transform.localPosition.x, transform.localPosition.y,
                (int)explotion_destroy.textureRect.width, (int)explotion_destroy.textureRect.height,
                (int)explotion_destroy.textureRect.x, (int)explotion_destroy.textureRect.y, explotion_destroy.texture, false, true);

        }


        for (i = 0; i < sprites.Length; i++) {

			GetComponent<SpriteRenderer>().sprite = sprites[i];

			Texture2D shape = sprites[i].texture;

            /*
            if (j < destory_ground_index.Length && i > destory_ground_index[j])
            {
                pixel_cntr.kill_by_sub_texture_shape_world_pos(transform.localPosition.x, transform.localPosition.y,
                    (int)sprites[i].textureRect.width, (int)sprites[i].textureRect.height,
                    (int)sprites[i].textureRect.x, (int)sprites[i].textureRect.y, sprites[i].texture);
                j++;
            }
            */
            //pixel_cont.kill_by_sub_texture_shape(p_x, p_y, shape);

            //pixel_cont.update_collider();

            yield return new WaitForSeconds (speed);
		}
        Destroy(gameObject);
	}






}
