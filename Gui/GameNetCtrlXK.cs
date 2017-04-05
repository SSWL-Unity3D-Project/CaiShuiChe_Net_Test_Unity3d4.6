using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class GameNetCtrlXK : MonoBehaviour {

	public GameObject [] PlayerObj;
	public GameObject [] NpcObj;
	public GameObject [] PlayerPos;
	public GameObject [] AiPlayerEndPos;

	public GameObject DaoJuExplosionObj;
	public GameObject HuanWeiFuNetPrefab;
	public GameObject DingShenFuNetPrefab;
	public GameObject DianDaoFuNetPrefab;
	public GameObject HuanYingFuNetPrefab;
	public GameObject JuLiFuNetPrefab;
	[Range(0.01f, 0.5f)]public float AiMinMoveSpeedA = 0.1f;
	[Range(0.01f, 0.5f)]public float AiMinMoveSpeedB = 0.35f;
	[Range(0.1f, 1f)]public float AiMaxMoveSpeedA = 0.5f;
	[Range(0.1f, 1f)]public float AiMaxMoveSpeedB = 0.65f;

	bool IsSpawnPlayerServer;

	//List<GameObject> PlayerList = new List<GameObject>(){};
	int ServerInitCount;

	static GameNetCtrlXK _Instance;
	public static GameNetCtrlXK GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Awake () {
		_Instance = this;
		int max = PlayerPos.Length;
		for (int i = 0; i < max; i++) {
			if (PlayerPos[i].activeSelf) {
				PlayerPos[i].SetActive(false);
			}
		}
	}

	public void SetIsSpawnPlayerServer()
	{
		IsSpawnPlayerServer = true;
	}

	public bool GetIsSpawnPlayerServer()
	{
		ServerInitCount++;
		if (ServerInitCount == 1) {
			return true; //don't spawn player
		}
		return IsSpawnPlayerServer;
	}

	/*public void SetPlayerList(GameObject obj, int index)
	{
		if (PlayerList.Count > index && PlayerList[index] != null)
		{
			return;
		}
		PlayerList.Add(obj);
	}

	public bool CheckIsFillPlayerObj(int index)
	{
		if (PlayerList.Count > index && PlayerList[index] != null)
		{
			return false;
		}
		return true;
	}*/
}

public enum DaoJuState
{
	HuanWeiFu,
	DingShenFu,
	DianDaoFu,
	JuLiFu,
	HuanYingFu
}