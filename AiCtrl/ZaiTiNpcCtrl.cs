using UnityEngine;
using System.Collections;

public class ZaiTiNpcCtrl : MonoBehaviour {

	public NPC_STATE NPC_Type = NPC_STATE.ZAI_TI_NPC;
	public GameObject BuWaWaObj;
	public GameObject ExplodeObj;
	public GameObject NpcSimpleBulletObj;
	public Transform SpawnBulletTran;
	public bool IsDiLeiNpc = false;
	[Range(0.5f, 10f)] public float TimeFireMin = 2f;
	public GameObject AudioHitNpcObj;
	public GameObject AudioNpcFireObj_1;
	public GameObject AudioNpcFireObj_2;

	public AudioClip AudioHitNpc;
	public AudioClip AudioNpcFire_1;
	public AudioClip AudioNpcFire_2;
	float DisPlayerFireMin = 100f;

	bool IsDoFireAction;
	Transform WaterwheelPlayer;
	Animator AnimatorNpc;

	Transform ChuanTran;
	Transform ZaiTiNpcTran;
	//Vector3 LocalPosOffset;
	
	float TimeLastFire;
	bool IsDeadNpc;
	bool IsFireNpc;

	NpcMoveCtrl NpcMoveScript;

	int FireCount;
	bool IsDanCheLvNpc;
	bool IsTeShuZiDanHaiDao = false;
	float BuWaWaPowerVal = 500f;

	// Use this for initialization
	void Awake()
	{
		if(NPC_Type == NPC_STATE.BOSS_NPC || NPC_Type == NPC_STATE.ZAI_TI_NPC)
		{
		}
		else
		{
			NPC_Type = NPC_STATE.ZAI_TI_NPC;
		}
		ZaiTiNpcTran = transform;
		ChuanTran = ZaiTiNpcTran.parent;
		//LocalPosOffset = Vector3.Scale(ZaiTiNpcTran.localPosition, ChuanTran.localScale);
		//LocalPosOffset = ZaiTiNpcTran.localPosition;
		//ZaiTiNpcTran.parent = GameCtrlXK.MissionCleanup;

		WaterwheelPlayer = WaterwheelPlayerCtrl.GetInstance().transform;
		AnimatorNpc = GetComponent<Animator>();
		if(BuWaWaObj != null)
		{
			BuWaWaObj.SetActive(false);
			Rigidbody rigObj = BuWaWaObj.GetComponent<Rigidbody> ();
			if (rigObj == null) {
				rigObj.name = "null";
			}
		}
		
		NpcMoveScript = ZaiTiNpcTran.parent.GetComponent<NpcMoveCtrl>();
		if (!IsDiLeiNpc) {
			RandomDoRootAction();
		}
		else if (!NpcMoveScript.IsDanCheLvNpc) {
			IsDoFireAction = true;
		}

		BuWaWaPowerVal = NpcMoveScript.BuWaWaPowerVal;
		IsTeShuZiDanHaiDao = NpcMoveScript.IsTeShuZiDanHaiDao;
		/*if (IsTeShuZiDanHaiDao) {
			NpcSimpleBulletObj.SetActive(false);
		}*/

		ResetActionInfo();
	}

	// Update is called once per frame
	void Update()
	{
		if (IsDeadNpc) {
			return;
		}

		if (IsDoFireAction) {
			if (!IsDiLeiNpc) {
				ZaiTiNpcTran.LookAt(WaterwheelPlayer);
			}
			else {
				if (Time.realtimeSinceStartup - TimeLastFire > TimeFireMin) {
					if (IsDanCheLvNpc && FireCount >= 2) {
						return;
					}
					FireCount++;
					//Debug.Log("*****************PlayZaiNpcFireAction, name " + transform.parent.name);
					TimeLastFire = Time.realtimeSinceStartup;
					PlayZaiNpcFireAction();
				}
			}
		}
		//ZaiTiNpcTran.position = ChuanTran.position - LocalPosOffset;
		//ZaiTiNpcTran.position = ChuanTran.position + LocalPosOffset;

		/*if (Input.GetKeyUp(KeyCode.P) && IsDiLeiNpc) {
			PlayZaiNpcFireAction();
		}*/
	}
	
	void OnDrawGizmosSelected()
	{
		if (NPC_Type == NPC_STATE.BOSS_NPC || NPC_Type == NPC_STATE.ZAI_TI_NPC) {
			
		} else if (NPC_Type != NPC_STATE.ZAI_TI_NPC) {
			NPC_Type = NPC_STATE.ZAI_TI_NPC;
		}
	}
	
	void DelayActiveNpcSimpleBulletObj()
	{
		NpcSimpleBulletObj.SetActive(true);
	}

	void OnMouseFireActive()
	{
		//Debug.Log("OnMouseFireActive**********");
		//		if(NpcSimpleBulletObj == null)
		//		{
		//			return;
		//		}
		
		NpcSimpleBullet bulletScript = NpcSimpleBulletObj.GetComponent<NpcSimpleBullet>();
		if (bulletScript == null) {
			if (AnimatorNpc.GetBool("IsFire_1")) {
				/*if (AudioNpcFire_1 != null) {
					AudioListCtrl.PlayAudio( AudioNpcFire_1 );
				}*/
				PlayNpcAudio(AudioNpcFireObj_1);
			}
			else {
				/*if (AudioNpcFire_2 != null) {
					AudioListCtrl.PlayAudio( AudioNpcFire_2 );
				}*/
				PlayNpcAudio(AudioNpcFireObj_2);
			}
		}
		else {
			switch (bulletScript.BulletState) {
			case NpcBulletState.BoLiPing:
			case NpcBulletState.ShuiLei:
				break;
				
			default:
				if (AnimatorNpc.GetBool("IsFire_1")) {
					/*if (AudioNpcFire_1 != null) {
						AudioListCtrl.PlayAudio( AudioNpcFire_1 );
					}*/
					PlayNpcAudio(AudioNpcFireObj_1);
				}
				else {
					/*if (AudioNpcFire_2 != null) {
						AudioListCtrl.PlayAudio( AudioNpcFire_2 );
					}*/
					PlayNpcAudio(AudioNpcFireObj_2);
				}
				break;
			}
		}

		if (IsTeShuZiDanHaiDao) {
			GameObject haiDanQiangHuo = (GameObject)Instantiate(NpcSimpleBulletObj, SpawnBulletTran.position, SpawnBulletTran.rotation);
			Transform qiangHuoTran = haiDanQiangHuo.transform;
			qiangHuoTran.parent = GameCtrlXK.MissionCleanup;
			/*NpcSimpleBulletObj.SetActive(false);
			if (!IsInvoking("DelayActiveNpcSimpleBulletObj")) {
				Invoke("DelayActiveNpcSimpleBulletObj", 0.1f);
			}*/
			return;
		}

		Vector3 vecA = Camera.main.transform.position - ZaiTiNpcTran.position;
		Vector3 vecB = Camera.main.transform.forward;
		vecA.y = vecB.y = 0f;
		if (Vector3.Dot(vecA, vecB) > 0f) {
			return;
		}

		Vector3 startPos = SpawnBulletTran.position;
		Vector3 endPos = Camera.main.transform.position;
		vecA = ZaiTiNpcTran.position - WaterwheelPlayer.position;
		vecA.y = vecB.y = 0f;
		if(Random.Range(0, 100) > 50 || Vector3.Dot(vecA, vecB) < 0.866f)
		{
			endPos = WaterwheelPlayer.position + Vector3.up * 1.5f;
		}

		float distance = Vector3.Distance(startPos, endPos);
		Vector3 forwardVec = endPos - startPos;
		forwardVec = forwardVec.normalized;

		RaycastHit hitInfo;
		Physics.Raycast(startPos, forwardVec, out hitInfo, distance, GameCtrlXK.GetInstance().NpcAmmoHitLayer.value);
		if (hitInfo.collider != null){
			endPos = hitInfo.point;
		}
		GameObject ammo = (GameObject)Instantiate(NpcSimpleBulletObj);
		Transform ammoTran = ammo.transform;
		ammoTran.parent = GameCtrlXK.MissionCleanup;
		ammoTran.position = startPos;
		ammoTran.forward = forwardVec;

		bulletScript = ammo.GetComponent<NpcSimpleBullet>();
		if (bulletScript == null) {
			bulletScript.gameObject.name = "null";
			return;
		}

		switch (bulletScript.BulletState) {
		case NpcBulletState.BoLiPing:
		case NpcBulletState.ShuiLei:
			InitMoveBullet(bulletScript);
			CloseFireAction();
			break;
		}
		bulletScript.dist = Vector3.Distance(startPos, endPos);
	}

	
	void InitMoveBullet(NpcSimpleBullet bulletScript)
	{
		//Debug.Log("********InitMoveBullet");
		Rigidbody rigObj = bulletScript.GetComponent<Rigidbody>();
		Transform TranCam = Camera.main.transform;
		Vector3 vecDir = Vector3.Lerp(TranCam.forward, Vector3.up, 0.2f);
		rigObj.AddForce(vecDir * 10f, ForceMode.Impulse);
	}
	
	/// <summary>
	/// Sets the npc animator action. if val == 0 is false, else is true.
	/// </summary>
	/// <param name="action">Action.</param>
	/// <param name="val">Value.</param>
	/// <param name="speedAction">Speed action.</param>
	void SetNpcAnimatorAction(string action, int val, float speedAction)
	{
		bool isPlay = val == 0 ? false : true;
		if (AnimatorNpc.GetBool(action) == isPlay) {
			return;
		}
		
		AnimatorNpc.speed = speedAction;
		AnimatorNpc.SetBool(action, isPlay);
		/*if (Network.peerType != NetworkPeerType.Disconnected) {
			netView.RPC("SendSetNpcAnimatorAction", RPCMode.OthersBuffered, action, val, speedAction);
		}*/
	}

	public void RandomDoRootAction()
	{
		SetNpcAnimatorAction("IsRun_2", 0, AnimatorNpc.speed);
		if (Random.Range(0, 100) % 2 == 0) {
			SetNpcAnimatorAction("IsRoot_2", 0, AnimatorNpc.speed);
			SetNpcAnimatorAction("IsRoot_1", 1, AnimatorNpc.speed);
		}
		else {
			SetNpcAnimatorAction("IsRoot_1", 0, AnimatorNpc.speed);
			SetNpcAnimatorAction("IsRoot_2", 1, AnimatorNpc.speed);
		}
	}
	
	public void CloseRootAction()
	{
		SetNpcAnimatorAction("IsRoot_1", 0, AnimatorNpc.speed);
		SetNpcAnimatorAction("IsRoot_2", 0, AnimatorNpc.speed);
	}

	public void PlayZaiNpcFireAction()
	{
		IsDoFireAction = true;
		if (!IsDiLeiNpc) {
			ZaiTiNpcTran.LookAt(WaterwheelPlayer);
		}
		else {
			if (NpcMoveScript.GetMoveSpeed() <= 1f
			    || Vector3.Distance(ZaiTiNpcTran.position, WaterwheelPlayer.position) > DisPlayerFireMin) {
				return;
			}
		}
		
		CloseRootAction();
		AnimatorNpc.speed = 1f;
		if (Random.Range(0, 100) % 2 == 0) {
			SetNpcAnimatorAction("IsFire_1", 1, AnimatorNpc.speed);
		}
		else {
			SetNpcAnimatorAction("IsFire_2", 1, AnimatorNpc.speed);
		}
	}

	void CloseFireAction()
	{
		AnimatorNpc.speed = 1f;
		SetNpcAnimatorAction("IsFire_1", 0, AnimatorNpc.speed);
		SetNpcAnimatorAction("IsFire_2", 0, AnimatorNpc.speed);
	}

	public void SetIsFireNpc()
	{
		IsFireNpc = true;
	}

	void ActiveBuWaWa ()
	{
		if(BuWaWaObj == null || BuWaWaObj.activeSelf)
		{
			return;
		}
		AnimatorNpc.enabled = false;
		BuWaWaObj.SetActive(true);
		Rigidbody rigObj = BuWaWaObj.GetComponent<Rigidbody>();
		Vector3 vecDir = Vector3.Lerp(-ZaiTiNpcTran.forward, ZaiTiNpcTran.up, GameCtrlXK.GetInstance().NpcBuWaWaVal);
		if (IsDiLeiNpc) {
			Transform TranCam = Camera.main.transform;
			vecDir = TranCam.forward;
			vecDir.y = 0f;
			vecDir = Vector3.Lerp(vecDir, Vector3.up, GameCtrlXK.GetInstance().NpcBuWaWaVal);
		}
		rigObj.AddForce(vecDir * BuWaWaPowerVal, ForceMode.Impulse);
	}

	void OnHitWaterwheelPlayer ()
	{
		ActiveBuWaWa();
		Invoke("DelaySpawnExplode", GameCtrlXK.GetInstance().TimeNpcSpawnExplode);
	}

	void DelaySpawnExplode()
	{
		Transform tran = BuWaWaObj.transform;
		Instantiate(ExplodeObj, tran.position, tran.rotation);
		Destroy(gameObject, 0.1f);
	}

	public void ShootedByPlayer(int key)
	{
		if (IsDeadNpc) {
			return;
		}
		
		if (IsFireNpc) {
			WaterwheelPlayerCtrl.GetInstance().AddKillFireNpcNum();
		}

		if (key == 0 || (key == 1 && !StartBtCtrl.GetInstanceP2().CheckIsActivePlayer())) {
			//XingXingCtrl.GetInstance().AddStarNum();
			WaterwheelCameraCtrl.GetInstance().SpawnPlayerNengLiangLiZi(ZaiTiNpcTran.position);
		}

		NpcMoveCtrl npcScript = ChuanTran.GetComponent<NpcMoveCtrl>();
		npcScript.OnZaiTiNpcDead();
		
		/*if (AudioHitNpc != null) {
			AudioListCtrl.PlayAudio(AudioHitNpc);
		}*/
		PlayNpcAudio(AudioHitNpcObj);

		OnHitWaterwheelPlayer();
		IsDeadNpc = true;
	}
	
	void ResetActionInfo()
	{
		if (AnimatorNpc == null) {
			return;
		}
		SetNpcAnimatorAction("IsRoot_1", 0, AnimatorNpc.speed);
		SetNpcAnimatorAction("IsRoot_2", 0, AnimatorNpc.speed);
		SetNpcAnimatorAction("IsRun_2", 0, AnimatorNpc.speed);
		SetNpcAnimatorAction("IsFire_1", 0, AnimatorNpc.speed);
		SetNpcAnimatorAction("IsFire_2", 0, AnimatorNpc.speed);
	}

	public void SetIsDanCheLvNpc()
	{
		IsDanCheLvNpc = true;
		TimeFireMin = 1f;
	}

	void PlayNpcAudio(GameObject audioObj)
	{
		if (audioObj == null) {
			/*Debug.LogError(gameObject.name + ": audioObj is Null");
			gameObject.name = "null";*/
			return;
		}

		if (!audioObj.activeSelf) {
			audioObj.SetActive(true);
		}
		
		AudioSource ad = audioObj.GetComponent<AudioSource>();
		/*if (ad.isPlaying) {
			return;
		}*/
		ad.Play();
	}
}
