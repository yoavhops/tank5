using UnityEngine;
using System.Collections;

public class create_texture_copy : MonoBehaviour {
	
	private Texture2D tex; 
	private GUITexture myGUITexture;
	
	void Awake()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        tex = sr.sprite.texture;
        Debug.Log(tex.width);
        /*
		sr.sprite = Sprite.Create((Texture2D)GameObject.Instantiate(tex),
		                          new Rect(0, 0, tex.width, tex.height),
		                          new Vector2(0.5f,0.5f));
                                  */
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
