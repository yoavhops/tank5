using UnityEngine;
using System.Collections;

public class Point{
	public int x;
	public int y;
	public Point(){
		this.x = 0;
		this.y = 0;
	}
	public Point (int x, int y){
		this.x = x;
		this.y = y;
	}
	public Point (Point other){
		this.x = other.x;
		this.y = other.y;
	}

	public void set(Point other){
		this.x = other.x;
		this.y = other.y;
	}
}

public class pixel_control : MonoBehaviour
{
    private Texture2D tex;
    private SpriteRenderer sr;
    public GameObject bomb;
    private Point b_l;//bottom left
    private Point b_r;
    private Point t_l;
    private Point t_r;//top rigt
    private Color[] bg_colors;
    private Color[] bg_colors_temp;
    private int bg_width;
    private int bg_height;

    //public GameObject tank;


    public int max_sloop = 10;
    // Use this for initialization
    void Start()
    {

        tex = GetComponent<SpriteRenderer>().sprite.texture;
        bg_colors = tex.GetPixels();
        bg_colors_temp = new Color[bg_colors.Length];
        bg_width = tex.width;
        bg_height = tex.height;
        b_l = new Point();
        b_r = new Point();
        t_l = new Point();
        t_r = new Point();

        set_pixel(500, get_hieght_at_point(500, 500), Color.blue);
        set_pixel(490, get_hieght_at_point(490, 500), Color.blue);
        set_pixel(480, get_hieght_at_point(480, 500), Color.blue);
        set_pixel(600, get_hieght_at_point(600, 200), Color.blue);

        tex.SetPixels(bg_colors);
        tex.Apply();
    }

    public void get_set_pixels(Color[] shape, Color[] from, Color[] to, int x, int y, int width, int height, int from_width, bool black_and_white = false, bool add_dirt = false)
    {
        int from_pos = x + y * from_width;
        int w, h;
        int i = 0;

        for (h = 0; h < height; h++)
        {
            for (w = 0; w < width; w++)
            {
                if (!black_and_white)
                {
                    if (shape[i].a > 0)
                    {
                        if (!add_dirt)
                        {
                            from[from_pos].a = 0;
                        }
                        else
                        {
                            from[from_pos] = shape[i];
                        }
                    }
                }
                else
                {
                    if (shape[i].r == 0)
                    {
                        from[from_pos].a = 0;
                    }
                }
                to[i] = from[from_pos];
                from_pos++;
                i++;
            }
            from_pos += from_width - width;
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        int p_x = 0;
        int p_y = 0;
        world_to_pixel_pos(bomb.transform.position.x,
                           bomb.transform.position.y,
                           ref p_x, ref p_y);

        kill_by_texture_shape(p_x, p_y, bomb.GetComponent<SpriteRenderer>().sprite.texture);
        */
    }


	public void killPixel(int x, int y)
    {
        tex.SetPixel(x, y, Color.clear);
    }
	public void set_pixel(int x, int y, Color color)
    {
        bg_colors[x + y * bg_width] = color;
    }

    /*
	 * use world to pixel before calling this.
	 */
    private void width_height_without_edges(int x, int y, ref int s_width, ref int s_height, int x_off , int y_off,
        ref int left_out_offset, ref int down_out_offset)
    {
        int shape_half_width = s_width / 2;
        int shape_half_height = s_height / 2;
        int bg_half_width = tex.width / 2;
        int bg_half_height = tex.height / 2;
        int right_out_offset = 0;
        int up_out_offset = 0;

        //b_l is the position of the shape on the bg image, in pixel.
        b_l.x = x - shape_half_width;
        b_l.y = y - shape_half_height;

        b_r.set(b_l);
        b_r.x = b_l.x + s_width;

        t_l.set(b_l);
        t_l.y = t_l.y + s_height;

        t_r.set(t_l);
        t_r.x = t_l.x + s_width;

        if (b_l.y < 0)
        {
            down_out_offset = -b_l.y;
            b_l.y = 0;
            b_r.y = 0;
        }

        if (t_l.y > tex.height)
        {
            up_out_offset = t_l.y - tex.height;
            t_l.y = tex.height;
            t_r.y = tex.height;
        }

        if (b_r.x > tex.width)
        {
            right_out_offset = b_r.x - tex.width;
            b_r.x = tex.width;
            t_r.x = tex.width;
        }

        if (b_l.x < 0)
        {
            left_out_offset = -b_l.x;
            b_l.x = 0;
            t_l.x = 0;
        }

        if (left_out_offset > s_width || right_out_offset > s_width ||
                        up_out_offset > s_height || down_out_offset > s_height)
        {
            return;
        }
        /*
		Debug.Log ("" + left_out_offset.ToString() + " " + down_out_offset.ToString() + " " + 
		           (shape.width - left_out_offset - right_out_offset).ToString() + " " + 
		           (shape.height - down_out_offset - up_out_offset).ToString() ); 
		*/
        s_width = s_width - left_out_offset - right_out_offset;
        s_height = s_height - down_out_offset - up_out_offset;
    }

    public void kill_by_sub_texture_shape_world_pos(float x, float y, int s_width, int s_height, int x_off, int y_off, Texture2D shape, bool black_and_white = false, bool add_dirt = false)
    {

        int p_x = 0;
        int p_y = 0;

        world_to_pixel_pos(x, y, ref p_x, ref p_y);

        kill_by_sub_texture_shape(p_x, p_y, s_width, s_height, x_off, y_off, shape, black_and_white, add_dirt);
    }

    /*
     * sub texture is the texuter of s sprite that made of multipal sprits
	 * use world to pixel before calling this
	 */
    public void kill_by_sub_texture_shape(int x, int y, int s_width, int s_height, int x_off, int y_off, Texture2D shape, bool black_and_white = false, bool add_dirt = false)
    {

        int down_out_offset = 0;
        int left_out_offset = 0;

        width_height_without_edges(x, y, ref s_width, ref s_height, x_off, y_off, ref left_out_offset, ref down_out_offset);
        
        Color[] c_shape = shape.GetPixels(x_off + left_out_offset, y_off + down_out_offset,
                s_width, s_height);

        kill_by_texture_shape(x, y, s_width, s_height, c_shape, black_and_white, add_dirt);
    }


    public void kill_by_texture_shape(int x, int y, Texture2D shape, bool black_and_white = false)
    {
        int down_out_offset = 0;
        int left_out_offset = 0;
        int s_width = shape.width;
        int s_height = shape.height;

        width_height_without_edges(x, y, ref s_width, ref s_height, 0, 0, ref left_out_offset, ref down_out_offset);

        Debug.Log("kill_by_texture_shape");
        Debug.Log(left_out_offset);
        Debug.Log(down_out_offset);
        Debug.Log(s_width);
        Debug.Log(s_height);



        Color[] c_shape = shape.GetPixels(left_out_offset, down_out_offset,
                s_width, s_height);

        kill_by_texture_shape(x, y, s_width, s_height, c_shape, black_and_white);
    }

    /*
	 * use world to pixel before calling this
	 */
    public void kill_by_texture_shape(int x, int y, int s_width, int s_height, Color[] c_shape, bool black_and_white = false, bool add_dirt = false)
    {
        get_set_pixels(c_shape, bg_colors, bg_colors_temp, b_l.x, b_l.y, s_width, s_height, bg_width, black_and_white, add_dirt);

        tex.SetPixels(b_l.x, b_l.y, s_width, s_height, bg_colors_temp);

        tex.Apply();

        update_collider();
    }

    

    /*
        void OnGUI() {
            Vector3 vec = new Vector3(); 

            float pixel_per_unit = Screen.height / (Camera.main.orthographicSize * 2f);
            vec.Set ((b_l.x - tex.width/2 )/100f, (b_l.y - tex.height/2)/100f, 0);
            vec = Camera.main.WorldToScreenPoint (vec);
            vec.y = Screen.height - vec.y;
            /*
            Debug.Log(new Rect( vec.x, vec.y,
                               (b_r.x - b_l.x)/100f , (t_l.y - b_l.y)/100f));
            */
    /*		GUI.Box(new Rect( vec.x, vec.y,
                    ((b_r.x - b_l.x)/100f) * pixel_per_unit, -((t_l.y - b_l.y)/100f) * pixel_per_unit),
                    "This is a title");
        }
    */


	public int get_hieght_at_point(int x, int y)
    {

        if (x < 0 || x > bg_width)
        {
            return 0;
        }
        if (y < 0)
        {
            return 0;
        }
        if (y >= bg_height)
        {
            y = bg_height - 1;
        }

        int from_pos = x + y * bg_width;
        bool up = false;

        if (bg_colors[from_pos].a > 0)
        {
            up = true;
        }

        while (true)
        {
            if (y < 0)
            {
                return 0;
            }
            if (y >= bg_height)
            {
                return bg_height;
            }


            if (bg_colors[from_pos].a > 0)
            {
                if (!up)
                {
                    return y;
                }
                from_pos += bg_width;
                y++;
            }

            else if (bg_colors[from_pos].a == 0)
            {
                if (up)
                {
                    return y;
                }
                from_pos -= bg_width;
                y--;
            }

        }
    }

	public void world_to_pixel_pos(float x, float y, ref int pixel_x, ref int pixel_y)
	{
		x = x - transform.position.x;
		y = y - transform.position.y;

        x = x * 100;
        y = y * 100;
        x += bg_width / 2;
        y += bg_height / 2;
        pixel_x = (int)x;
        pixel_y = (int)y;
    }


    public bool has_ground_in_pos(float x, float y)
    {
        int p_x = 0; 
        int p_y = 0;
        world_to_pixel_pos(x, y, ref p_x, ref p_y);
            
        if (p_x < 0 || p_y < 0 || p_x >= tex.width || p_y >= tex.height)
        {
            return false;
        }

        int bg_pos = p_x + p_y * bg_width;
        if (bg_colors[bg_pos].a > 0)
        {
            return true;
        }
        return false;
    }


	/*
    public void pixel_pos_to_world(int p_x, int p_y, ref float x, ref float y)
    {
        p_x -= bg_width / 2;
        p_y -= bg_height / 2;
        x = p_x / 100f;
        y = p_y / 100f;
    }
	*/

	public void update_collider(){
		Destroy(GetComponent<PolygonCollider2D>());
		
		gameObject.AddComponent<PolygonCollider2D>();
	}



    /*
    public bool set_tank_angle(float x, float y, int width, ref float new_h)
    {
        int i;
        int pixel_x = 0;
        int pixel_y = 0;
        Vector2 left_max = Vector2.zero;
        Vector2 right_max = Vector2.zero;
        int temp = 0;
        int prev = 0;
        world_to_pixel_pos(x, y, ref pixel_x, ref pixel_y);
        int pixel_i = pixel_x - width / 2;

        for (; pixel_i < pixel_x; pixel_i++)
        {
            temp = get_hieght_at_point(pixel_i, pixel_y);
            if (temp > left_max.y)
            {
                left_max.Set(pixel_i, temp);
            }
        }

        for (; pixel_i < pixel_x + width / 2; pixel_i++)
        {
            temp = get_hieght_at_point(pixel_i, pixel_y);
            if (temp > right_max.y)
            {
                right_max.Set(pixel_i, temp);
            }
        }

        Vector2 diff = right_max - left_max;

        float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        tank.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        */
        /*
		
		float middle_r = (right_max.x - pixel_x) / (right_max.x - left_max.x) ;

		Vector2 middle_pos = right_max + diff * middle_r;

		new_h = (middle_pos.y - (bg_height / 2)) / 100;
		
		pixel_pos_to_world ((int)right_max.x, (int)right_max.y, ref x, ref y);
		
		tank_r.transform.position = new Vector3 (x,y,0);
		
		pixel_pos_to_world ((int)left_max.x, (int)left_max.y, ref x, ref y);
		
		tank_l.transform.position = new Vector3 (x,y,0);
        */ /*
        return true;
    }
    */
}