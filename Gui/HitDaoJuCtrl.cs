using UnityEngine;
using System.Collections;

public class HitDaoJuCtrl : MonoBehaviour {

	public GameObject HitDaoJuCtrl_1;
	public GameObject HitDaoJuCtrl_2;

	static HitDaoJuCtrl _Instance;
	public static HitDaoJuCtrl GetInstance()
	{
		return _Instance;
	}

	void Start()
	{
		_Instance = this;
	}

	// Update is called once per frame
	/*void Update()
	{
		if (Input.GetKeyUp(KeyCode.Keypad1)) {
			SpawnHitDaoJuSprite(DaoJuTypeIndex.huanWeiFu);
		}

		if (Input.GetKeyUp(KeyCode.Keypad2)) {
			SpawnHitDaoJuSprite(DaoJuTypeIndex.huanYingFu);
		}
		
		if (Input.GetKeyUp(KeyCode.Keypad3)) {
			SpawnHitDaoJuSprite(DaoJuTypeIndex.juLiFu);
		}
		
		if (Input.GetKeyUp(KeyCode.Keypad4)) {
			SpawnHitDaoJuSprite(DaoJuTypeIndex.dianDaoFu);
		}
		
		if (Input.GetKeyUp(KeyCode.Keypad5)) {
			SpawnHitDaoJuSprite(DaoJuTypeIndex.dingShenFu);
		}
	}*/

	public void SpawnHitDaoJuSprite(DaoJuTypeIndex val)
	{
		if (!StartBtCtrl.GetInstanceP1().CheckIsActivePlayer()
		    && !StartBtCtrl.GetInstanceP2().CheckIsActivePlayer()) {
			return;
		}

		string spriteNameVal = "x" + (int)val;
		if (StartBtCtrl.GetInstanceP1().CheckIsActivePlayer()) {
			GameObject objP1 = null;
			TweenPosition tweenPosP1 = null;
			objP1 = (GameObject)Instantiate(HitDaoJuCtrl_1);
			objP1.GetComponent<UISprite>().spriteName = spriteNameVal;

			tweenPosP1 = objP1.GetComponent<TweenPosition>();
			EventDelegate.Add(tweenPosP1.onFinished, delegate{
				ActivePlayerDaoJu(objP1, val, 1);
			});
			objP1.transform.parent = transform;
			objP1.SetActive(true);
		}

		if (StartBtCtrl.GetInstanceP2().CheckIsActivePlayer()) {
			GameObject objP2 = null;
			TweenPosition tweenPosP2 = null;
			objP2 = (GameObject)Instantiate(HitDaoJuCtrl_2);
			objP2.GetComponent<UISprite>().spriteName = spriteNameVal;
			
			tweenPosP2 = objP2.GetComponent<TweenPosition>();
			EventDelegate.Add(tweenPosP2.onFinished, delegate{
				ActivePlayerDaoJu(objP2, val, 2);
			});
			objP2.transform.parent = transform;
			objP2.SetActive(true);
		}
	}

	void ActivePlayerDaoJu(GameObject spriteObj, DaoJuTypeIndex val, int key)
	{
		if (key == 1) {
			if (StartBtCtrl.GetInstanceP1().CheckIsActivePlayer()) {
				ActiveDaJuCtrl.GetInstanceP1().ActiveDaoJuType((int)val);
				HeadCtrlPlayer.GetInstanceP1().InitChangeHeadUI();
			}
		}
		else if (key == 2) {
			if (StartBtCtrl.GetInstanceP2().CheckIsActivePlayer()) {
				ActiveDaJuCtrl.GetInstanceP2().ActiveDaoJuType((int)val);
				HeadCtrlPlayer.GetInstanceP2().InitChangeHeadUI();
			}
		}

		//Debug.Log("ActivePlayerDaoJu -> key = " + key);
		Destroy(spriteObj);
	}
}
