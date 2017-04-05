using UnityEngine;
using System.Collections;

public class PlayerSimpleBullet : MonoBehaviour
{
	float speed= 250f;
	static public float lifeTime = 2f;
	public float dist = 10000f;
	
	private float spawnTime = 0.0f;
	private Transform tr;

	bool IsHandleBullet;
	NetworkView netView;

	void Awake()
	{
		tr = transform;
		tr.position = GlobalData.HiddenPosition;
		tr.parent = GameCtrlXK.MissionCleanup;

		netView = GetComponent<NetworkView>();
		if (netView != null) {
			if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
				netView.enabled = false;
			}
			else {
				netView.enabled = true;
			}
		}
	}

	void OnEnable()
	{
		tr = transform;
		spawnTime = Time.time;
	}
	
	void Update()
	{
		if (!IsHandleBullet) {
			return;
		}

		tr.position += tr.forward * speed * Time.deltaTime;
		dist -= speed * Time.deltaTime;
		if (Time.time > spawnTime + lifeTime || dist < 0)
		{
			Spawner.DestroyObj(gameObject);
			SetBulletActive(0);
			return;
		}

		if (GlobalData.GetInstance().gameMode == GameMode.OnlineMode && Network.peerType != NetworkPeerType.Disconnected) {

			netView.RPC("SendBulletTranToOther", RPCMode.OthersBuffered, tr.position, tr.rotation);
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (!IsHandleBullet) {
			return;
		}
		//Debug.Log("*************** colName " + other.name);
		
		// Get the health component of the target if any
		NpcHealthCtrl targetHealth = other.GetComponent<NpcHealthCtrl>();
		if (targetHealth) {
			// Apply damage
			if (!GlobalData.GetInstance().IsActiveJuLiFu) {
				targetHealth.OnDamage(2f / GlobalData.GetInstance().PlayerAmmoFrequency);
			}
			else {
				targetHealth.OnDamage(1f / GlobalData.GetInstance().PlayerAmmoFrequency);
			}
		}
	}

	public void SetIsHandleBullet()
	{
		//SetBulletActive(1);
		if (IsHandleBullet) {
			return;
		}
		IsHandleBullet = true;
	}

	[RPC]
	void SendBulletTranToOther(Vector3 pos, Quaternion rot)
	{
		transform.position = pos;
		transform.rotation = rot;
	}

	void SetBulletActive(int val)
	{
		if (GlobalData.GetInstance().gameMode != GameMode.OnlineMode) {
			return;
		}

		if (Network.peerType != NetworkPeerType.Disconnected) {
			
			netView.RPC("SendBulletIsActiveObj", RPCMode.OthersBuffered, val);
		}
	}

	[RPC]
	void SendBulletIsActiveObj(int val)
	{
		if (0 == val) {
			IsHandleBullet = false;
			tr.position = GlobalData.HiddenPosition;
		}
	}
}

