using UnityEngine;
using System.Collections;

public class SpawnNpcPointCtrl : MonoBehaviour {
	
	public NpcRunState RunStateVal = NpcRunState.NULL;
	public GameObject NpcPlayer;
	public GameObject NpcPathCtrl;
	public bool IsLoopPath;

	public GameObject AiPathMark;
	public bool IsNiXingAiPath;

	[Range(1f, 100f)] public float MoveSpeed = 10f;
	[Range(0f, 100f)] public float FireDistance = 10f;

	[Range(0.1f, 50f)] public float ShuaGuaiUnitTime = 3f;
	[Range(0f, 50f)] public float ShuaGuaiAllTime = 0f;
	[Range(0.001f, 5f)] public float ShuiQiangNpcFlyTime = 2.5f;

	int SpawnNpcNum;
	const int MaxSpawnNpcNum = 100;
	NpcMoveCtrl []SpawnNpcMoveScript;

	bool IsRemoveSpawnPointNpc;
	
	void Awake()
	{
		if (NpcPlayer == null) {
			Debug.LogError("NpcPlayer should not is null!");
			NpcPlayer.name = "null";
			return;
		}

		NpcMoveCtrl npcScript = NpcPlayer.GetComponent<NpcMoveCtrl>();
		if (GlobalData.GetInstance().gameMode == GameMode.OnlineMode) {
			if (FireDistance > 0f
			    || npcScript.NPC_Type == NPC_STATE.BOSS_NPC
			    || npcScript.NPC_Type == NPC_STATE.ZAI_TI_NPC) {
				gameObject.SetActive(false);
				return;
			}
		}

		if(SpawnNpcMoveScript == null)
		{
			SpawnNpcMoveScript = new NpcMoveCtrl[MaxSpawnNpcNum];
		}

		if(npcScript.NPC_Type == NPC_STATE.ZAI_TI_NPC || npcScript.NPC_Type == NPC_STATE.BOSS_NPC)
		{
			if (AiPathMark == null && NpcPathCtrl == null) {
				Debug.LogError("AiPathMark or NpcPathCtrl should not be null!");
				AiPathMark.name = "null";
				NpcPathCtrl.name = "null";
				return;
			}
		}
		else if (npcScript.NPC_Type != NPC_STATE.FEI_XING_NPC)
		{
			if (NpcPathCtrl == null) {
				Debug.LogError("NpcPathCtrl should not is null!");
				NpcPathCtrl.name = "null";
				return;
			}
		}

		if (IsLoopPath) {
			if (NpcPathCtrl != null && NpcPathCtrl.transform.childCount <= 1) {
				Debug.LogError("NpcPath childCount should be greater than 1");
				Transform tr = null;
				tr.name = "null";
			}
		}

		if (npcScript.IsDanCheLvNpc) {
			if (npcScript.NPC_Type != NPC_STATE.ZAI_TI_NPC) {
				Debug.LogError("npcScript.IsDanCheLvNpc is true, NPC_Type should is ZAI_TI_NPC!");
				Transform tr = null;
				tr.name = "null";
			}

			if (FireDistance <= 0f) {
				Debug.LogError("FireDistance should not is zero!");
				Transform tr = null;
				tr.name = "null";
			}
		}

		switch (npcScript.NPC_Type) {
		case NPC_STATE.ZAI_TI_NPC:
		case NPC_STATE.BOSS_NPC:
			for (int i = 0; i < npcScript.ZaiTiScript.Length; i++) {
				if (npcScript.ZaiTiScript[i] != null) {
					if (npcScript.IsDanCheLvNpc) {
						npcScript.ZaiTiScript[i].IsDiLeiNpc = true;
					}

					if (FireDistance > 0f) {
						if (npcScript.ZaiTiScript[i].NpcSimpleBulletObj == null) {
							Debug.LogError("ZaiTiScript num -> " + i);
							npcScript.ZaiTiScript[i].NpcSimpleBulletObj.name = "null";
						}

						if (npcScript.ZaiTiScript[i].SpawnBulletTran == null) {
							Debug.LogError("ZaiTiScript num -> " + i);
							npcScript.ZaiTiScript[i].SpawnBulletTran.name = "null";
						}
					}

					if (npcScript.ZaiTiScript[i].BuWaWaObj == null) {
						Debug.LogError("ZaiTiScript num -> " + i);
						npcScript.ZaiTiScript[i].BuWaWaObj.name = "null";
					}
					
					if (npcScript.ZaiTiScript[i].ExplodeObj == null) {
						Debug.LogError("ZaiTiScript num -> " + i);
						npcScript.ZaiTiScript[i].ExplodeObj.name = "null";
					}
				}
			}
			break;

		default:
			if (FireDistance > 0f) {
				if (npcScript.NpcSimpleBulletObj == null) {
					npcScript.NpcSimpleBulletObj.name = "null";
				}
				
				if (npcScript.NpcSimpleBulletObjFire_2 == null) {
					npcScript.NpcSimpleBulletObjFire_2 = npcScript.NpcSimpleBulletObj;
				}

				if (npcScript.SpawnBulletTran == null) {
					npcScript.SpawnBulletTran.name = "null";
				}

				if (npcScript.SpawnBulletTranFire_2 == null) {
					npcScript.SpawnBulletTranFire_2 = npcScript.SpawnBulletTran;
				}
			}

			if (npcScript.BuWaWaObj == null) {
				npcScript.BuWaWaObj.name = "null";
			}

			if (npcScript.ExplodeObj == null) {
				npcScript.ExplodeObj.name = "null";
			}
			break;
		}
	}

	void OnDrawGizmosSelected()
	{
		if (!enabled) {
			return;
		}

		if (AiPathMark != null) {
			Transform AiPathTran = AiPathMark.transform.parent;
			AiPathCtrl pathScript = AiPathTran.GetComponent<AiPathCtrl>();
			pathScript.DrawPath();
		}

		if (NpcPathCtrl != null) {
			Transform NpcMarkTran = NpcPathCtrl.transform.GetChild(0);
			NpcMark NpcMarkScript = NpcMarkTran.GetComponent<NpcMark>();
			NpcMarkScript.DrawPath();
		}

		if (NpcPathCtrl != null || AiPathMark != null) {
			Transform [] tranArray = new Transform[2];
			if (NpcPathCtrl == null && AiPathMark != null) {
				tranArray[0] = transform;
				tranArray[1] = AiPathMark.transform;
				iTween.DrawPath(tranArray, Color.yellow);
			}
			else if (NpcPathCtrl != null) {
				tranArray[0] = transform;
				tranArray[1] = NpcPathCtrl.transform.GetChild(0);
				iTween.DrawPath(tranArray, Color.yellow);

				if (AiPathMark != null) {
					tranArray[0] = NpcPathCtrl.transform.GetChild(NpcPathCtrl.transform.childCount - 1);
					tranArray[1] = AiPathMark.transform;
					iTween.DrawPath(tranArray, Color.yellow);
				}
			}
		}

		CheckTransformScale();
		UpdateMeshFilter();
	}

	void CheckTransformScale()
	{
		if (transform.localScale.y != 1f) {
			Vector3 size = Vector3.zero;
			size = transform.localScale;
			size.y = 1f;
			transform.localScale = size;
		}
	}

	public void SpawnAiPlayer()
	{
		if (IsRemoveSpawnPointNpc) {
			return;
		}

		if(NpcPlayer == null)
		{
			Debug.LogError("NpcPlayer is null");
			return;
		}
		
		if (!gameObject.activeSelf) {
			return;
		}

		NpcMoveCtrl npcScript = NpcPlayer.GetComponent<NpcMoveCtrl>();
		if(npcScript.NPC_Type == NPC_STATE.ZAI_TI_NPC || npcScript.NPC_Type == NPC_STATE.BOSS_NPC)
		{
			if (GlobalData.GetInstance().gameMode != GameMode.SoloMode) {
				Debug.LogError("Cannot spawn zaiTiNpc! gameMode = " + GlobalData.GetInstance().gameMode);
				return;
			}

			if(!IsNiXingAiPath)
			{
				if(!NpcMoveCtrl.CheckIsSpawnZhengXingZaiTiNpc())
				{
					return;
				}
			}
			else
			{
				if(!NpcMoveCtrl.CheckIsSpawnNiXingZaiTiNpc())
				{
					return;
				}
			}
		}

		GameObject npcObjSpawn = null;
		if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
			npcObjSpawn = (GameObject)Instantiate(NpcPlayer, transform.position, transform.rotation);
		}
		else {
			if (Network.peerType == NetworkPeerType.Server) {
				npcObjSpawn = (GameObject)Network.Instantiate(NpcPlayer, transform.position, transform.rotation, GlobalData.NetWorkGroup);
			}
			else {
				Debug.LogError("Cannot spawn npc, peerType = " + Network.peerType);
				return;
			}
		}

		NpcMoveCtrl NpcMoveScripte = npcObjSpawn.GetComponent<NpcMoveCtrl>();
		SpawnNpcMoveScript[SpawnNpcNum] = NpcMoveScripte;
		if(NpcMoveScripte != null)
		{
			NpcMoveScripte.SpawnPointObj = gameObject;
			NpcMoveScripte.SetIsHandleNpc();
			switch(NpcMoveScripte.NPC_Type)
			{
			case NPC_STATE.ZAI_TI_NPC:
				if(AiPathMark != null || NpcPathCtrl != null)
				{
					//set AiPathMark NPC
					if(NpcPathCtrl != null)
					{
						NpcMoveScripte.SetNpcPathTran(NpcPathCtrl.transform, MoveSpeed, false,
						                              FireDistance, RunStateVal, AiPathMark, IsNiXingAiPath);
					}
					else
					{
						NpcMoveScripte.SetNpcPathTran(null, MoveSpeed, false,
						                              FireDistance, RunStateVal, AiPathMark, IsNiXingAiPath);
					}
				}
				break;

			case NPC_STATE.FEI_XING_NPC:
				NpcMoveScripte.SetNpcPathTran(null, MoveSpeed, false, FireDistance, RunStateVal, null, false);
				break;

			default:
				if(NpcPathCtrl != null)
				{
					NpcMoveScripte.SetNpcPathTran(NpcPathCtrl.transform, MoveSpeed, IsLoopPath, FireDistance, RunStateVal, AiPathMark, IsNiXingAiPath);
					NpcMoveScripte.SetShuiQiangNpcFlyTime(ShuiQiangNpcFlyTime);
				}
				break;
			}
		}
		SpawnNpcNum++;

		if(ShuaGuaiAllTime > 0f && SpawnNpcNum < MaxSpawnNpcNum)
		{
			if(ShuaGuaiUnitTime <= 0.1f)
			{
				ShuaGuaiUnitTime = 0.1f;
			}

			if(ShuaGuaiAllTime > SpawnNpcNum * ShuaGuaiUnitTime)
			{
				Invoke("SpawnAiPlayer", ShuaGuaiUnitTime);
			}
		}
	}

	public void RemoveSpawnPointNpc()
	{
		if(NpcPlayer == null)
		{
			Debug.LogError("NpcPlayer is null");
			return;
		}

		if (!gameObject.activeSelf) {
			return;
		}

		IsRemoveSpawnPointNpc = true;
		if (IsInvoking("SpawnAiPlayer")){
			CancelInvoke("SpawnAiPlayer");
		}

		int max = SpawnNpcMoveScript.Length;
		for(int i = 0; i < max; i++)
		{
			if(SpawnNpcMoveScript[i] != null)
			{
				SpawnNpcMoveScript[i].RemoveNpcObj(true);
			}
		}
	}

	void UpdateMeshFilter()
	{
		MeshFilter filter = GetComponent<MeshFilter>();
		if (filter != null
		    && GameCtrlXK.GetInstance().PathMarkFilter != null
		    && filter.sharedMesh != GameCtrlXK.GetInstance().PathMarkFilter.sharedMesh) {
			DestroyImmediate(filter);
			MeshFilter newFilter = gameObject.AddComponent<MeshFilter>();
			newFilter.sharedMesh = GameCtrlXK.GetInstance().PathMarkFilter.sharedMesh;
		}
	}
}
