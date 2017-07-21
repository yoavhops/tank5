using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class gui_debug : MonoBehaviour {

	static gui_debug s_gui_debug;
	
	private Text text;
	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();
		s_gui_debug = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void add_line(string s){
		text.text += "\n" + s;
	}

	public static void debug(string s){
		s_gui_debug.add_line (s);
	}
}
