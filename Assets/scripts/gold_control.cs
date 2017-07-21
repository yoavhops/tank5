using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class gold_control : MonoBehaviour {
	private const int DEFAULT_GOLD = 100;
	private const string GOLD_KEY = "gold";
	public Text current_gold;
    public bool Debug_mode = false;
    public ConnectAndJoinRandomYoav connect;

	void Start () {

        if (Debug_mode)
        {
            set_gold(100);
        }
        
        current_gold.text = get_gold().ToString();
    }

	public static int get_gold() {
		if (!PlayerPrefs.HasKey(GOLD_KEY)) {
			set_gold(DEFAULT_GOLD);
		}
		return PlayerPrefs.GetInt(GOLD_KEY);
	}
	
	public static void set_gold(int gold) {
		PlayerPrefs.SetInt(GOLD_KEY,gold);
	}
	
	public void Buy100() {
		BuyGold(100);
	}
	
	public void PayFee(int fee)	{
		int gold = get_gold();
		if (gold >= fee) {
			gold -= fee;
			set_gold(gold);
			current_gold.text = gold.ToString();

            connect.fee = fee;
            Debug.Log("HEY1");
            connect.connect = true;
		}
		else {
			//TODO: goto buy screen
		}
	}
	
	public void BuyGold(int gold) {
		//TODO: goto buy screen
		set_gold(get_gold()+gold);
		current_gold.text = get_gold().ToString();
	}
}
