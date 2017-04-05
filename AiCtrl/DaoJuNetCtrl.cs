using UnityEngine;
using System.Collections;

public class DaoJuNetCtrl : MonoBehaviour {
	public DaoJuState DaoJuType = DaoJuState.DianDaoFu;

	// Use this for initialization
	void Start () {
		SetDaoJuType();
		if (GlobalData.GetInstance().gameMode != GameMode.SoloMode) {
			InitCheckPeerType();
		}
	}

	void SetDaoJuType()
	{
		bool isHidden = false;
		string type = "";
		switch (DaoJuType) {
		case DaoJuState.DianDaoFu:
			type = "DianDaoFuObj";
			isHidden = true;
			break;
		case DaoJuState.DingShenFu:
			type = "DingShenFuObj";
			isHidden = true;
			break;
		case DaoJuState.HuanWeiFu:
			type = "HuanWeiFuObj";
			isHidden = true;
			break;
		case DaoJuState.HuanYingFu:
			type = "HuanYingFuObj";
			break;
		case DaoJuState.JuLiFu:
			type = "JuLiFuObj";
			break;
		}
		tag = type;

		if (isHidden && GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
			gameObject.SetActive(false);
		}
	}
	
	void InitCheckPeerType()
	{
		if (GlobalData.GetInstance().gameMode != GameMode.OnlineMode) {
			return;
		}
		
		gameObject.SetActive(false);
		Invoke("CheckPeerType", 2f);
	}
	
	void CheckPeerType()
	{
		//Debug.Log("********* Network.peerType " + Network.peerType);
		switch (Network.peerType) {
		case NetworkPeerType.Client:
			//CancelInvoke("CheckPeerType");
			return;
		case NetworkPeerType.Server:
			CreateDaoJuNet();
			//CancelInvoke("CheckPeerType");
			return;
		}
		Invoke("CheckPeerType", 0.1f);
	}

	void CreateDaoJuNet()
	{
		GameObject objPrefab = null;
		string type = "";
		switch (DaoJuType) {
		case DaoJuState.DianDaoFu:
			if (Network.connections.Length < 1) {
				return;
			}
			type = "DianDaoFuObj";
			objPrefab = GameNetCtrlXK.GetInstance().DianDaoFuNetPrefab;
			break;
		case DaoJuState.DingShenFu:
			type = "DingShenFuObj";
			objPrefab = GameNetCtrlXK.GetInstance().DingShenFuNetPrefab;
			break;
		case DaoJuState.HuanWeiFu:
			type = "HuanWeiFuObj";
			objPrefab = GameNetCtrlXK.GetInstance().HuanWeiFuNetPrefab;
			break;
		case DaoJuState.HuanYingFu:
			type = "HuanYingFuObj";
			objPrefab = GameNetCtrlXK.GetInstance().HuanYingFuNetPrefab;
			break;
		case DaoJuState.JuLiFu:
			type = "JuLiFuObj";
			objPrefab = GameNetCtrlXK.GetInstance().JuLiFuNetPrefab;
			break;
		}
		DaoJuNetCtrl netScript = objPrefab.GetComponent<DaoJuNetCtrl>();
		if (netScript != null) {
			Debug.LogError(objPrefab.name + " should not add DaoJuNetCtrl component!");
			return;
		}

		GameObject spawnObj = (GameObject)Network.Instantiate(objPrefab, transform.position,
		                                                      transform.rotation, GlobalData.NetWorkGroup);

		NpcHealthCtrl healthScript = spawnObj.GetComponent<NpcHealthCtrl>();
		healthScript.SetObjType(type);
	}
}
