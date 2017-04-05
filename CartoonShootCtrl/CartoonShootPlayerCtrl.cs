using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CartoonShootPlayerCtrl : MonoBehaviour {
	
	public string OnFinishMoveCamera = "OnFinishMovePlayerByShootPath";
	public CartoonShootPlayerPath ShootPlayerPath;
	public CartoonShootPlayerMark ShootPlayerMark;
	public iTweenEvent[] ITweenEventArray;
	
	GameObject PlayerObj;
	Transform PlayerTran;
	Transform PlayerPathTran;
	
	int MarkCurrentIndex;
	static int pathCount = 0;
	public static bool IsActiveRunPlayer;
	
	// Update is called once per frame
	void Update()
	{
		if (pcvr.bIsHardWare) {
			return;
		}

		if (Input.GetKeyUp(KeyCode.P)) {
			ActiveRunPlayer();
		}
	}
	
	void OnDrawGizmosSelected()
	{
		if(!enabled)
		{
			return;
		}

		if (ShootPlayerPath != null) {
			ShootPlayerPath.DrawPath();
		}
	}

	void ActiveRunPlayer()
	{
		if (ShootPlayerPath == null) {
			Debug.LogError("ActiveRunPlayer -> ShootPlayerPath was null!");
			return;
		}

		if (IsActiveRunPlayer) {
			return;
		}
		IsActiveRunPlayer = true;
		Debug.Log("ActiveRunPlayer...");

		PlayerObj = gameObject;
		PlayerTran = transform;

		if (pathCount <= 0) {
			CartoonShootCamCtrl.GetInstance().CheckGenSuiCamTranStartGame();
			if (ShootPlayerMark != null) {
				MarkCurrentIndex = ShootPlayerMark.MarkIndex;
				ShootPlayerPath = ShootPlayerMark.ShootPlayerPath;
				PlayerPathTran = ShootPlayerPath.transform;
			}
			else {
				MarkCurrentIndex = 0;
				PlayerPathTran = ShootPlayerPath.transform;
			}
			PlayerTran.position = PlayerPathTran.GetChild(MarkCurrentIndex).position;
			PlayerTran.rotation = PlayerPathTran.GetChild(MarkCurrentIndex).rotation;
		}
		else {
			MarkCurrentIndex = 0;
			PlayerPathTran = ShootPlayerPath.transform;
		}

		MovePlayerByShootPath();
		pathCount++;
	}

	void MovePlayerByShootPath()
	{
		if (ShootPlayerPath.iTweenEventIndex != -1) {
			ITweenEventArray[ShootPlayerPath.iTweenEventIndex].enabled = true;
		}
		else {
			
			if (ShootPlayerPath.IsMoveByPathSpeed) {
			
				if (PlayerPathTran.childCount <= 1) {
					return;
				}
				
				List<Transform> nodes = new List<Transform>(PlayerPathTran.GetComponentsInChildren<Transform>()){};
				nodes.Remove(PlayerPathTran);
				for (int i = 0; i < MarkCurrentIndex; i++) {
					nodes.Remove(PlayerPathTran.GetChild(i));
				}

				iTween.MoveTo(PlayerObj, iTween.Hash("speed", ShootPlayerPath.MoveSpeed,
				                                     "orienttopath", true,
				                                     "easeType", iTween.EaseType.linear,
				                                     "path", nodes.ToArray(),
				                                     "oncomplete", "OnFinishMovePlayerByShootPath"));
			}
			else {
				
				Transform tran_0 = PlayerPathTran.GetChild(MarkCurrentIndex);
				Transform tran_1 = PlayerPathTran.GetChild(MarkCurrentIndex + 1);
				CartoonShootPlayerMark mark = tran_0.GetComponent<CartoonShootPlayerMark>();
				Transform[] path = new Transform[2];
				
				tran_0.position = PlayerTran.position;
				tran_0.rotation = PlayerTran.rotation;
				path[0] = tran_0;
				path[1] = tran_1;

				iTween.LookTo(PlayerObj, iTween.Hash("path", path,
				                                     "looktarget", tran_1,
				                                     "time", mark.LookTime,
				                                     "easeType", iTween.EaseType.linear,
				                                     "oncomplete", "OnFinishMovePlayerByRotation"));
			}
		}
	}

	void OnFinishMovePlayerByRotation()
	{
		Transform tran_0 = PlayerPathTran.GetChild(MarkCurrentIndex);
		Transform tran_1 = PlayerPathTran.GetChild(MarkCurrentIndex + 1);
		CartoonShootPlayerMark mark = tran_0.GetComponent<CartoonShootPlayerMark>();
		Transform[] path = new Transform[2];
		
		tran_0.position = PlayerTran.position;
		tran_0.rotation = PlayerTran.rotation;
		path[0] = tran_0;
		path[1] = tran_1;
		iTween.MoveTo(PlayerObj, iTween.Hash("path", path,
		                                     "speed", mark.MoveSpeed,
		                                     "orienttopath", true,
		                                     "easeType", iTween.EaseType.linear,
		                                     "oncomplete", "OnFinishMovePlayerByShootPath"));
	}
	
	void OnFinishMovePlayerByShootPath()
	{
		if (ShootPlayerPath.IsMoveByPathSpeed) {
			Debug.Log("OnFinishMovePlayerByShootPath...");
			HandleFinishMovePlayerByPath();
			return;
		}
		else {
			if (MarkCurrentIndex >= PlayerPathTran.childCount - 2) {
				Debug.Log("OnFinishMovePlayerByShootPath...");
				HandleFinishMovePlayerByPath();
				return;
			}
			MarkCurrentIndex++;
			MovePlayerByShootPath();
		}
	}

	void HandleFinishMovePlayerByPath()
	{
		if (ShootPlayerPath.NextPlayerPath) {
			ShootPlayerPath = ShootPlayerPath.NextPlayerPath;
			IsActiveRunPlayer = false;
			ActiveRunPlayer();
		}
	}
}
