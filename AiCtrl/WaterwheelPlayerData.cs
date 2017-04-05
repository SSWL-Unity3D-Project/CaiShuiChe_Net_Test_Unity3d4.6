using UnityEngine;
using System.Collections;

public class WaterwheelPlayerData : MonoBehaviour
{
	public GameObject DianDaoFuSprite;
	public GameObject DingShenWater;
	public GameObject JuLiFuObjTeXiao;
	public GameObject HuanWeiFuTeXiao;
	public GameObject HuanYingFuTeXiao;
	// Use this for initialization
	void Start ()
	{
		if (GlobalData.GetInstance().gameMode == GameMode.OnlineMode) {
			if (DianDaoFuSprite == null) {
				DianDaoFuSprite.name = "null";
			}
			
			if (DingShenWater == null) {
				DingShenWater.name = "null";
			}
			
			if (JuLiFuObjTeXiao == null) {
				JuLiFuObjTeXiao.name = "null";
			}
			
			if (HuanWeiFuTeXiao == null) {
				HuanWeiFuTeXiao.name = "null";
			}
		}
		gameObject.SetActive(false);
	}
}

