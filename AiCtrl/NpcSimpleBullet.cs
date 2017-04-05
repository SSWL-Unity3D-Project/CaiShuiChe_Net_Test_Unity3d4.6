using UnityEngine;
using System.Collections;

public enum NpcBulletState
{
	NULL,
	ShuiLei,
	BoLiPing
}

public class NpcSimpleBullet : MonoBehaviour {

	public NpcBulletState BulletState = NpcBulletState.NULL;
	public float speed= 50f;
	public float lifeTime = 0.5f;
	public float dist = 10000f;
	public GameObject ExplodeObj;
	
	private float spawnTime = 0.0f;
	private Transform tr;

	bool IsBombShuiLei;
	bool IsHitDown;
	
	void OnEnable()
	{
		tr = transform;
		spawnTime = Time.time;
		switch (BulletState) {
		case NpcBulletState.ShuiLei:
			AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioShuiLei);
			if (ExplodeObj == null) {
				ExplodeObj.name = "null";
			}
			break;
		}
	}

	void Update()
	{
		if (BulletState == NpcBulletState.ShuiLei || BulletState == NpcBulletState.BoLiPing) {
			//CheckPlayerDistance();
			CheckAmmoLiftTime();
			CheckIsHitDown();
			return;
		}

		tr.position += tr.forward * speed * Time.deltaTime;
		dist -= speed * Time.deltaTime;
		if (Time.time > spawnTime + lifeTime || dist < 0)
		{
			switch (BulletState) {
			case NpcBulletState.ShuiLei:
				AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioShuiLeiBaoZha);
				break;
				
			case NpcBulletState.BoLiPing:
				AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioPingZiPoSui);
				break;
			
			default:
				/*if (GameCtrlXK.PlayerTran != null) {
					
					Vector3 vecA = Camera.main.transform.position;
					Vector3 vecB = tr.position;
					Vector3 vecC = GameCtrlXK.PlayerTran.position;
					if (Vector3.Distance(vecA, vecB) < (Vector3.Distance(vecA, vecC) - 3f)
					    && PlayerAutoFire.PlayerMvSpeed > GameCtrlXK.NpcHitPlayerShakeSpeed
					    && Random.Range(0, 1000) % GameCtrlXK.NpcShakeCamVal == 0) {
						CameraShake.GetInstance().SetCameraShakeImpulseValue();
					}
				}*/
				break;
			}
			Destroy(gameObject);
		}
	}
	
	void OnCollisionEnter(Collision collision)
	{
		if (GameCtrlXK.PlayerTran == null) {
			return;
		}
		
		if (IsBombShuiLei) {
			return;
		}

		if (LayerMask.LayerToName( collision.transform.gameObject.layer ) == "Water"
		    || LayerMask.LayerToName( collision.transform.gameObject.layer ) == "Terrain") {
			if (BulletState == NpcBulletState.BoLiPing) {
				IsBombShuiLei = true;
				ExplodeObj.transform.parent = GameCtrlXK.MissionCleanup;
				ExplodeObj.SetActive(true);
				AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioPingZiPoSui);

				//CameraShake.GetInstance().SetCameraShakeImpulseValue();
				//PlayerAutoFire.HandlePlayerHitShuiLei();
				Destroy(gameObject, 0.3f);
				return;
			}
		}

		if (collision.transform.root == GameCtrlXK.PlayerTran) {
			//Debug.Log("**************************** " + tr.name);
			IsBombShuiLei = true;
			ExplodeObj.transform.parent = GameCtrlXK.MissionCleanup;
			ExplodeObj.SetActive(true);
			
			switch (BulletState) {
			case NpcBulletState.ShuiLei:
				AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioShuiLeiBaoZha);
				pcvr.GetInstance().OnPlayerHitShake();
				break;
				
			case NpcBulletState.BoLiPing:
				AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioPingZiPoSui);
				break;
			}
			CameraShake.GetInstance().SetCameraShakeImpulseValue();
			PlayerAutoFire.HandlePlayerHitShuiLei();
			Destroy(gameObject, 0.3f);
		}
	}

	void CheckPlayerDistance()
	{
		if (GameCtrlXK.PlayerTran == null) {
			return;
		}

		if (IsBombShuiLei) {
			return;
		}

		Vector3 posA = GameCtrlXK.PlayerTran.position;
		Vector3 posB = tr.position;
		posA.y = posB.y = 0f;
		if (Vector3.Distance(posA, posB) < 5f) {
			IsBombShuiLei = true;
			ExplodeObj.transform.parent = GameCtrlXK.MissionCleanup;
			ExplodeObj.SetActive(true);

			switch (BulletState) {
			case NpcBulletState.ShuiLei:
				AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioShuiLeiBaoZha);
				pcvr.GetInstance().OnPlayerHitShake();
				break;
				
			case NpcBulletState.BoLiPing:
				AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioPingZiPoSui);
				break;
			}
			CameraShake.GetInstance().SetCameraShakeImpulseValue();
			PlayerAutoFire.HandlePlayerHitShuiLei();
			Destroy(gameObject, 0.3f);
		}
	}

	void CheckIsHitDown()
	{
		if (IsHitDown) {
			return;
		}

		RaycastHit hitInfo;
		Vector3 startPos = tr.position + Vector3.up * 2f;
		Vector3 forwardVal = Vector3.down;
		Physics.Raycast(startPos, forwardVal, out hitInfo, 10f, GameCtrlXK.GetInstance().NpcVertHitLayer.value);
		if (hitInfo.collider != null){

			if (Vector3.Distance(hitInfo.point, tr.position) < 2) {
				IsHitDown = true;

				Animator aniCom = tr.GetComponentInChildren<Animator>();
				if (aniCom != null && BulletState == NpcBulletState.ShuiLei) {
					aniCom.SetBool("Root", true);
				}
			}
		}
	}

	void CheckAmmoLiftTime()
	{
		if (tr.forward != Vector3.forward) {
			tr.forward = Vector3.Lerp(tr.forward, Vector3.forward, Time.deltaTime);
		}

		if (IsBombShuiLei) {
			return;
		}

		if (Time.time < spawnTime + lifeTime) {
			return;
		}

		if (tr.forward != Vector3.forward) {
			tr.forward = Vector3.forward;
		}

		Vector3 vecA = Camera.main.transform.forward;
		Vector3 vecB = tr.position - Camera.main.transform.position;
		vecA.y = vecB.y = 0f;
		if (Vector3.Dot(vecA, vecB) < 0f) {
			Destroy(gameObject);
		}
	}
}
