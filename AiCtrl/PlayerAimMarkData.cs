using UnityEngine;
using System.Collections;

public class PlayerAimMarkData : MonoBehaviour
{
	public bool IsRunToEndPoint;
	public int RankNo;

	bool IsPlayer;
	bool IsGameOver;

	bool IsActiveDingShen;
	NetworkView viewNet;

	public WaterwheelPlayerNetCtrl playerNetScript;
	public WaterwheelAiPlayerNet AiNetScript;
	bool IsActiveHuanWei;
	bool IsActiveDianDao;

	public GameObject DingShenWater;
	public GameObject HuanWeiFuTeXiao;

	void Start()
	{
		if (GlobalData.GetInstance().gameMode == GameMode.OnlineMode) {
			viewNet = GetComponent<NetworkView>();
			if (viewNet != null) {
				viewNet.stateSynchronization = NetworkStateSynchronization.Off;
			}

			playerNetScript = GetComponent<WaterwheelPlayerNetCtrl>();
			if (playerNetScript != null) {
				DingShenWater = playerNetScript.GetWaterPlayerDt().DingShenWater;
				HuanWeiFuTeXiao = playerNetScript.GetWaterPlayerDt().HuanWeiFuTeXiao;
			}

			AiNetScript = GetComponent<WaterwheelAiPlayerNet>();
			if (AiNetScript != null) {
				DingShenWater = AiNetScript.DingShenWater;
				HuanWeiFuTeXiao = AiNetScript.HuanWeiFuTeXiao;
			}

			/*if (AiNetScript == null && playerNetScript == null) {
				ScreenLog.LogError("*****************AiNetScript or playerNetScript is null, name -> "+gameObject.name);
			}
			ScreenLog.Log("test ************** name "+gameObject.name);*/
		}
	}

	public void SetDingShenWater(GameObject obj)
	{
		if (obj == null) {
			return;
		}
		DingShenWater = obj;
	}

	public void SetIsRunToEndPoint()
	{
		if (IsRunToEndPoint) {
			return;
		}

		IsRunToEndPoint = true;
		Invoke("DelaySetIsKinematic", 1f);
		if (Network.peerType != NetworkPeerType.Disconnected) {
			viewNet.RPC("SendPlayerSetIsRunToEndPoint", RPCMode.OthersBuffered);
		}
	}

	[RPC]
	void SendPlayerSetIsRunToEndPoint()
	{
		if (IsRunToEndPoint) {
			return;
		}
		IsRunToEndPoint = true;
		Invoke("DelaySetIsKinematic", 1f);
	}

	void DelaySetIsKinematic()
	{
		Rigidbody rigObj = GetComponent<Rigidbody>();
		rigObj.isKinematic = true;

		ChuanLunZiCtrl lunZiScript = GetComponentInChildren<ChuanLunZiCtrl>();
		if (lunZiScript != null) {
			lunZiScript.CloseLunZiAction();
		}
	}

	public bool GetIsActiveDianDao()
	{
		return IsActiveDianDao;
	}

	void OpenDianDaoTeXiao()
	{
		if (playerNetScript != null && playerNetScript.GetIsHandlePlayer()) {
			AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioFuMianBuff);
		}

		//DianDaoTeXiao ChuLi
		IsActiveDianDao = true;
		if (playerNetScript != null) {
			playerNetScript.ShowDianDaoFuSprite();
		}

		CancelInvoke("CloseDianDaoState");
		Invoke("CloseDianDaoState", 3f);
	}

	public void ActiveDianDaoState()
	{
		if (playerNetScript == null) {
			return;
		}
		
		if (IsActiveDianDao) {
			return;
		}
		OpenDianDaoTeXiao();
		
		if (viewNet != null && Network.peerType != NetworkPeerType.Disconnected) {
			viewNet.RPC("SendAimMarkActiveDianDao", RPCMode.OthersBuffered);
		}
	}

	[RPC]
	void SendAimMarkActiveDianDao()
	{
		if (playerNetScript == null) {
			return;
		}
		OpenDianDaoTeXiao();
	}

	void CloseDianDaoState()
	{
		IsActiveDianDao = false;
		if (playerNetScript != null) {
			playerNetScript.HiddenDianDaoFuSprite();
		}
	}

	public void OpenHuanWeiFuTeXiao()
	{
		ShowHuanWeiFuTeXiao();
		if (Network.peerType != NetworkPeerType.Disconnected) {
			viewNet.RPC("SendOpenPlayerHuanWeiFuTeXiao", RPCMode.OthersBuffered);
		}
	}
	
	[RPC]
	void SendOpenPlayerHuanWeiFuTeXiao()
	{
		ShowHuanWeiFuTeXiao();
	}

	void ShowHuanWeiFuTeXiao()
	{
		if (HuanWeiFuTeXiao == null) {
			if (AiNetScript != null) {
				HuanWeiFuTeXiao = AiNetScript.HuanWeiFuTeXiao;
			}

			if (HuanWeiFuTeXiao != null) {
				HuanWeiFuTeXiao.SetActive(true);
			}
			else {
				ScreenLog.LogError("*****************HuanWeiFuTeXiao is null, name -> "+gameObject.name);
			}
		}
		else {
			if (HuanWeiFuTeXiao.activeSelf) {
				return;
			}
			HuanWeiFuTeXiao.SetActive(true);
		}
		
		DelayCloseHuanWeiFuTeXiao();
	}
	
	void DelayCloseHuanWeiFuTeXiao()
	{
		CancelInvoke("CloseHuanWeiFuTeXiao");
		Invoke("CloseHuanWeiFuTeXiao", 3f);
	}
	
	void CloseHuanWeiFuTeXiao()
	{
		if (HuanWeiFuTeXiao == null) {
			ScreenLog.LogError("*****************HuanWeiFuTeXiao is null, name -> "+gameObject.name);
		}
		else {
			if (!HuanWeiFuTeXiao.activeSelf) {
				return;
			}
			HuanWeiFuTeXiao.SetActive(false);
		}
	}

	public void ActiveHuanWeiState(Vector3 playerPos, int countPath, int countMark)
	{
		if (playerNetScript == null && AiNetScript == null) {
			return;
		}
		//OpenHuanWeiFuTeXiao();

		bool isSendMsg = false;
		if (playerNetScript != null && !playerNetScript.GetIsHandlePlayer()) {
			isSendMsg = true;
		}
		else if (AiNetScript != null && !AiNetScript.GetIsHandlePlayer()) {
			isSendMsg = true;
		}
		
		if (!isSendMsg) {
			//Debug.Log(gameObject.name + " is actived huanWeiState...");
			if (playerNetScript != null) {
				AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioFuMianBuff);
			}
			else {
				SetAiPlayerHuanWeiFuInfo(countPath, countMark);
			}
			transform.position = playerPos;

			//HuanWeiTeXiao ChuLi
			IsActiveHuanWei = true;
			CancelInvoke("CloseHuanWeiState");
			Invoke("CloseHuanWeiState", 3f);
		}
		else {
			if (viewNet != null && Network.peerType != NetworkPeerType.Disconnected) {
				viewNet.RPC("SendAimMarkActiveHuanWei", RPCMode.OthersBuffered, playerPos, countPath, countMark);
			}
		}
	}

	void SetAiPlayerHuanWeiFuInfo(int countPath, int countMark)
	{
		if (AiNetScript != null && AiNetScript.GetIsHandlePlayer()) {
			AiNetScript.SetHuanWeiFuActiveInfo(countPath, countMark);
		}
	}

	[RPC]
	void SendAimMarkActiveHuanWei(Vector3 playerPos, int countPath, int countMark)
	{
		if (playerNetScript == null && AiNetScript == null) {
			return;
		}
		
		bool isActive = false;
		if (playerNetScript != null && playerNetScript.GetIsHandlePlayer()) {
			isActive = true;
			AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioFuMianBuff);
		}
		else if (AiNetScript != null && AiNetScript.GetIsHandlePlayer()) {
			isActive = true;
			SetAiPlayerHuanWeiFuInfo(countPath, countMark);
		}
		
		if (!isActive) {
			return;
		}
		//Debug.Log(gameObject.name + " is actived huanWeiState...");
		transform.position = playerPos;

		//HuanWeiTeXiao ChuLi
		IsActiveHuanWei = true;
		CancelInvoke("CloseHuanWeiState");
		Invoke("CloseHuanWeiState", 3f);
	}

	void CloseHuanWeiState()
	{
		if (!IsActiveHuanWei) {
			return;
		}
		IsActiveHuanWei = false;
	}

	public bool GetIsActiveDingShen()
	{
		return IsActiveDingShen;
	}

	void OpenDingShenState()
	{
		//Debug.LogError("OpenDingShenState -> name " + gameObject.name);
		if (playerNetScript != null && playerNetScript.GetIsHandlePlayer()) {
			AudioListCtrl.PlayAudio(AudioListCtrl.GetInstance().AudioFuMianBuff);
		}

		if (AiNetScript != null && AiNetScript.GetIsHandlePlayer()) {
			AiNetScript.SetIsRunMoveAiPlayer(false);
		}
		IsActiveDingShen = true;
		if (DingShenWater == null) {
			if (AiNetScript != null) {
				DingShenWater = AiNetScript.DingShenWater;
				if (DingShenWater != null) {
					DingShenWater.SetActive(true);
				}
			}

			if (DingShenWater == null) {
				ScreenLog.LogError("*****************DingShenWater is null, name -> "+gameObject.name);
			}
		}
		else {
			DingShenWater.SetActive(true);
		}
		transform.position += new Vector3(0f, 3f, 0f);
		CancelInvoke("CloseDingShenState");
		Invoke("CloseDingShenState", 3f);
	}

	public void ActiveDingShenState()
	{
		if (playerNetScript == null && AiNetScript == null) {
			return;
		}

		if (IsActiveDingShen) {
			return;
		}

		OpenDingShenState();
		if (viewNet != null && Network.peerType != NetworkPeerType.Disconnected) {
			viewNet.RPC("SendAimMarkActiveDingShen", RPCMode.OthersBuffered);
		}
	}

	[RPC]
	void SendAimMarkActiveDingShen()
	{
		if (playerNetScript == null && AiNetScript == null) {
			return;
		}
				
		if (IsActiveDingShen) {
			return;
		}
		OpenDingShenState();
	}

	void CloseDingShenState()
	{
		//Debug.LogError("CloseDingShenState -> name " + gameObject.name);
		if (AiNetScript != null && AiNetScript.GetIsHandlePlayer()) {
			AiNetScript.SetIsRunMoveAiPlayer(true);
		}
		IsActiveDingShen = false;

		if (DingShenWater == null) {
			ScreenLog.LogError("*****************DingShenWater is null, name -> "+gameObject.name);
		}
		else {
			DingShenWater.SetActive(false);
		}
		//transform.position -= new Vector3(0f, 3f, 0f);
	}

	public void SetIsPlayer()
	{
		IsPlayer = true;
	}

	public bool GetIsPlayer()
	{
		if (AiNetScript != null) {
			IsPlayer = false;
		}
		else if (playerNetScript != null) {
			IsPlayer = true;
		}
		return IsPlayer;
	}

	public bool CheckIsHandlePlayer()
	{
		bool isHandlePlayer = false;
		if (playerNetScript != null) {
			isHandlePlayer = playerNetScript.GetIsHandlePlayer();
		}
		return isHandlePlayer;
	}

	public void SetIsGameOver()
	{
		IsGameOver = true;
		if (Network.peerType != NetworkPeerType.Disconnected) {
			viewNet.RPC("SendPlayerSetIsGameOver", RPCMode.OthersBuffered);
		}
	}
	
	[RPC]
	void SendPlayerSetIsGameOver()
	{
		IsGameOver = true;
	}

	public bool GetIsGameOver()
	{
		return IsGameOver;
	}
}

