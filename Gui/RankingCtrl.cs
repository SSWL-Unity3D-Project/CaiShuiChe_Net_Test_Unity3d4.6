using UnityEngine;
using System.Collections;

public class RankingCtrl : MonoBehaviour {

	static public int mRankCount = 0;
	static public  PlayerRankData [] mRankPlayer = null;
	public static bool IsStopCheckRank = false;

	GameObject parentPlayer = null;
	GameObject childPlayer = null;
	Transform aimMarkPar;
	Transform aimMarkCh;
	public static int MaxPlayerRankNum = 8;
	public static int ServerPlayerRankNum = 1;
	
	public UISprite []RankListUISprite;
	GameObject []RankListQiZhiObj;
	static int QiZhiCount;

	static RankingCtrl _Instance;
	public static RankingCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;

		//InitRankListUISprite();

		InitRankPlayer();
		InvokeRepeating("CheckNetPlayerRank", 6, 0.125f);
	}
	
	void InitRankPlayer()
	{
		RankListQiZhiObj = new GameObject[MaxPlayerRankNum];

		mRankPlayer = null;
		mRankPlayer = new PlayerRankData[MaxPlayerRankNum];
		Transform tmp = null;
		for (int i = 0; i < MaxPlayerRankNum; i++) {
			mRankPlayer[i] = new PlayerRankData();
			tmp = RankListUISprite[i].transform.GetChild(0);
			if (tmp != null) {
				RankListQiZhiObj[i] = tmp.gameObject;
				RankListQiZhiObj[i].SetActive(false);
			}
		}
		
		mRankCount = 0;
		QiZhiCount = 0;
		IsStopCheckRank = false;
	}

	public void ShowRankPlayerQiZhi()
	{
		if (QiZhiCount >= MaxPlayerRankNum) {
			return;
		}
		RankListQiZhiObj[QiZhiCount].SetActive(true);
		QiZhiCount++;
	}

	public void SetRankPlayerArray(GameObject rankObj)
	{
		if(mRankCount > 7)
		{
			return;
		}
		
		if(GlobalData.GetInstance().gameMode == GameMode.SoloMode)
		{
			return;
		}

		mRankPlayer[mRankCount].Player = rankObj;
		mRankCount++;
		//Debug.Log("SetRankPlayerArray::name " + rankObj.name + ", mRankCount " + mRankCount);
		if(mRankCount >= 7)
		{
			ChangeRankListUI();
		}
	}
	
	public void SetPlayerPathCount(string playerName)
	{
		if(playerName == "")
		{
			return;
		}
		
		int max = mRankPlayer.Length;
		for(int i = 0; i < max; i++)
		{
			if(mRankPlayer[i].Name == playerName)
			{
				mRankPlayer[i].PlayerPathCount++;
				break;
			}
		}
	}
	
	public void SetPlayerAimMark(int aimPathId, int markId, string playerName)
	{
		if (aimPathId < 0 || playerName == "") {
			return;
		}

		Transform path = AiPathGroupCtrl.FindAiPathTran(aimPathId);
		if (path == null) {
			return;
		}

		AiPathCtrl pathScript = path.GetComponent<AiPathCtrl>();
		if (pathScript == null) {
			return;
		}
		
		if (markId < 0 || markId >= path.childCount) {
			return;
		}
		
		Transform tranAim = path.GetChild( markId );
		int max = mRankPlayer.Length;
		for (int i = 0; i < max; i++) {
			if (mRankPlayer[i] != null && mRankPlayer[i].Name == playerName) {
				mRankPlayer[i].mPlayerAimMark = tranAim;
				mRankPlayer[i].mBikePathKey = pathScript.KeyState;
				break;
			}
		}
	}

	public void StopCheckPlayerRank()
	{
		if (IsStopCheckRank) {
			return;
		}
		/*Debug.Log("StopCheckPlayerRank**************test");
		for(int i = 0; i < MaxPlayerRankNum; i++)
		{
			if(mRankPlayer[i] != null)
			{
				Debug.Log("*******************name " + mRankPlayer[i].Name);
			}
		}*/
		IsStopCheckRank = true;
		gameObject.SetActive(false);
		CancelInvoke("CheckNetPlayerRank");
	}

	void CheckNetPlayerRank()
	{
		if(IsStopCheckRank)
		{
			CancelInvoke("CheckNetPlayerRank");
			return;
		}

		/*if (Time.frameCount % 100 == 0) {
			Debug.Log("*****************");
		}*/

		int j = 0;
		bool isContinue = false;
		AiPathCtrl pathScriptPar = null;
		AiPathCtrl pathScriptCh = null;
		for(int i = 0; i < MaxPlayerRankNum; i++)
		{
			if(IsStopCheckRank) {
				break;
			}

			if (mRankPlayer[i].IsRunToEndPoint) {
				continue;
			}

			if(mRankPlayer[i].AimMarkDataScript != null)
			{
				mRankPlayer[i].IsPlayer = mRankPlayer[i].AimMarkDataScript.GetIsPlayer();
				isContinue = mRankPlayer[i].AimMarkDataScript.GetIsGameOver();
			}
			else
			{
				isContinue = true;
				mRankPlayer[i].IsPlayer = false;
			}
			
			if(isContinue)
			{
				isContinue = false;
				continue;
			}
			
			j = i + 1;
			if(j > 7)
			{
				break;
			}
			
			parentPlayer = mRankPlayer[i].Player;
			childPlayer = mRankPlayer[j].Player;
			aimMarkPar = mRankPlayer[i].mPlayerAimMark;
			aimMarkCh = mRankPlayer[j].mPlayerAimMark;
			if(parentPlayer == null || childPlayer == null || aimMarkPar == null || aimMarkCh == null)
			{
				continue;
			}
			
			int pathKeyP = mRankPlayer[i].mBikePathKey;
			int pathKeyC = mRankPlayer[j].mBikePathKey;
			if(pathKeyP < pathKeyC)
			{
				/*if(mRankPlayer[i].IsPlayer || mRankPlayer[j].IsPlayer)
				{
					Debug.Log("***************************1111, IsPlayer_1 " + mRankPlayer[i].IsPlayer
					          + ", IsPlayer_2 " + mRankPlayer[j].IsPlayer);
				}*/
				UpdateNetPlayerRank( j );
				break;
			}
			else if(pathKeyP > pathKeyC)
			{
				continue;
			}
			
			int pathIdPar = aimMarkPar.parent.GetInstanceID();
			int pathIdCh = aimMarkCh.parent.GetInstanceID();
			if(pathIdPar == pathIdCh)
			{
				AiMark markScriptP = aimMarkPar.GetComponent<AiMark>();
				AiMark markScriptC = aimMarkCh.GetComponent<AiMark>();
				int markCountP = markScriptP.getMarkCount();
				int markCountC = markScriptC.getMarkCount();
				if(markCountP < markCountC)
				{
					/*if(mRankPlayer[i].IsPlayer || mRankPlayer[j].IsPlayer)
					{
						Debug.Log("***************************2222, IsPlayer_1 " + mRankPlayer[i].IsPlayer
						          + ", IsPlayer_2 " + mRankPlayer[j].IsPlayer);
					}*/
					UpdateNetPlayerRank( j );
					break;
				}
				else if(markCountP > markCountC)
				{
					continue;
				}
			}
			else {
				pathScriptPar = aimMarkPar.parent.GetComponent<AiPathCtrl>();
				pathScriptCh = aimMarkCh.parent.GetComponent<AiPathCtrl>();
				if (pathScriptPar.KeyState > pathScriptCh.KeyState) {
					continue;
				}
				else if (pathScriptPar.KeyState < pathScriptCh.KeyState) {
					UpdateNetPlayerRank( j );
					break;
				}
			}
			
			Vector3 vecA = parentPlayer.transform.position - childPlayer.transform.position;
			Vector3 vecB = aimMarkPar.position - parentPlayer.transform.position;
			Vector3 vecC = childPlayer.transform.forward;
			Vector3 vecD = parentPlayer.transform.forward;
			vecA.y = 0f;
			vecB.y = 0f;
			vecC.y = 0f;
			vecD.y = 0f;
			vecA = vecA.normalized;
			vecB = vecB.normalized;
			vecC = vecC.normalized;
			vecD = vecD.normalized;
			
			float cosDB = Vector3.Dot(vecD, vecB);
			float cosDC = Vector3.Dot(vecD, vecC);
			float cosAC = Vector3.Dot(vecA, vecC);
			if(   (cosDB > 0f && cosDC > 0f && cosAC < 0f)
			   || (cosDB <= 0f && cosDC > 0f && cosAC > 0f)
			   || (cosDB <= 0f && cosDC <= 0f && cosAC < 0f) ) {
				/*if(mRankPlayer[i].IsPlayer || mRankPlayer[j].IsPlayer)
				{
					Debug.Log("*********33333, IsPlayer_1 " + mRankPlayer[i].IsPlayer
					          + ", IsPlayer_2 " + mRankPlayer[j].IsPlayer
					          + ", pathIdPar " + pathIdPar + ", pathIdCh " + pathIdCh);
				}*/
				UpdateNetPlayerRank( j );
				break;
			}
			//Debug.Log("i = " + i + ", j = " + j);
		}
		return;
	}
	
	void UpdateNetPlayerRank(int rankNumCh)
	{
		GameObject objParent = mRankPlayer[rankNumCh - 1].Player;
		GameObject objChild = mRankPlayer[rankNumCh].Player;
		if(objParent == null || objChild == null)
		{
			return;
		}

		/*PlayerAimMarkData AimMarkScriptTmp = mRankPlayer[rankNumCh - 1].AimMarkDataScript;
		mRankPlayer[rankNumCh - 1].AimMarkDataScript = mRankPlayer[rankNumCh].AimMarkDataScript;
		mRankPlayer[rankNumCh].AimMarkDataScript = AimMarkScriptTmp;*/
		/*if(mRankPlayer[rankNumCh - 1].IsPlayer || mRankPlayer[rankNumCh].IsPlayer)
		{
			Debug.Log("1111***parAimId " + mRankPlayer[rankNumCh - 1].mBikeAimMark.GetInstanceID()
			          + ", chAimId " + mRankPlayer[rankNumCh].mBikeAimMark.GetInstanceID());
		}*/

		Transform mBikeAimMarkTmp = mRankPlayer[rankNumCh - 1].mPlayerAimMark;
		mRankPlayer[rankNumCh - 1].mPlayerAimMark = mRankPlayer[rankNumCh].mPlayerAimMark;
		mRankPlayer[rankNumCh].mPlayerAimMark = mBikeAimMarkTmp;
		/*if(mRankPlayer[rankNumCh - 1].IsPlayer || mRankPlayer[rankNumCh].IsPlayer)
		{
			Debug.Log("2222***parAimId " + mRankPlayer[rankNumCh - 1].mBikeAimMark.GetInstanceID()
			          + ", chAimId " + mRankPlayer[rankNumCh].mBikeAimMark.GetInstanceID());
		}*/
		
		int PlayerPathCountTmp = mRankPlayer[rankNumCh - 1].PlayerPathCount;
		mRankPlayer[rankNumCh - 1].PlayerPathCount = mRankPlayer[rankNumCh].PlayerPathCount;
		mRankPlayer[rankNumCh].PlayerPathCount = PlayerPathCountTmp;
		
		int pathCountP = mRankPlayer[rankNumCh - 1].PlayerPathCount;
		int pathCountC = mRankPlayer[rankNumCh].PlayerPathCount;
		if(pathCountP < pathCountC)
		{
			mRankPlayer[rankNumCh - 1].PlayerPathCount = pathCountC;
		}
		
		int pathKeyTmp = mRankPlayer[rankNumCh - 1].mBikePathKey;
		mRankPlayer[rankNumCh - 1].mBikePathKey = mRankPlayer[rankNumCh].mBikePathKey;
		mRankPlayer[rankNumCh].mBikePathKey = pathKeyTmp;
		
		mRankPlayer[rankNumCh - 1].Player = objChild;
		mRankPlayer[rankNumCh].Player = objParent;
		//ScreenLog.Log("**********************UpdateNetPlayerRank");

		if(mRankCount >= 7)
		{
			//Change Player Rank List
			ChangeRankListUI();
		}
		return;
	}

	/*void InitRankListUISprite()
	{
		RankListUISprite = transform.GetComponentsInChildren<UISprite>();
	}*/

	void ChangeRankListUI()
	{
		bool isFindPlayerOne = false;
		for (int i = 0; i < MaxPlayerRankNum; i++) {
			if (!isFindPlayerOne && mRankPlayer[i].IsPlayer) {
				isFindPlayerOne = true;
				PlayerRankData.PlayerOneDt = mRankPlayer[i];
			}

			if (mRankPlayer[i].AiPlayerNetScript != null) {
				mRankPlayer[i].AiPlayerNetScript.SetRankNoAi(i);
			}

			mRankPlayer[i].RankNo = i;
			RankListUISprite[i].spriteName = mRankPlayer[i].RankListName;
			if (mRankPlayer[i].AimMarkDataScript != null && mRankPlayer[i].AimMarkDataScript.CheckIsHandlePlayer()) {
				RankListUISprite[i].color = Color.green;
			}
			else {
				RankListUISprite[i].color = new Color(1f, 1f, 1f);
			}
		}
	}

	public IEnumerator SetRankListUISpriteColor(string playerNameVal)
	{
		bool isRun = true;
		while (isRun) {
			for (int i = 0; i < MaxPlayerRankNum; i++) {
				if (mRankPlayer[i].Name == playerNameVal) {
					isRun = false;
					RankListUISprite[i].color = Color.green;
				}
			}
			//Debug.Log("********************" + playerNameVal);
			yield return new WaitForSeconds(0.1f);
		}
	}
}

public class PlayerRankData
{
	public static PlayerRankData PlayerOneDt;

	public bool IsPlayer;
	public int RankNo;

	public string Name;
	public string RankListName;
	public bool IsRunToEndPoint
	{
		get
		{
			if (Player == null) {
				return false;
			}
			AimMarkDataScript = Player.GetComponent<PlayerAimMarkData>();
			return AimMarkDataScript.IsRunToEndPoint;
		}
	}

	public WaterwheelAiPlayerNet AiPlayerNetScript;
	public PlayerAimMarkData AimMarkDataScript;
	public Transform mPlayerAimMark;
	public int PlayerPathCount = 0;
	public int mBikePathKey;
	
	private GameObject _Player;
	public GameObject Player
	{
		get
		{
			return _Player;
		}
		set
		{
			if(value == null)
			{
				return;
			}
			
			_Player = value;
			Name = _Player.name;
			RankListName = GetRankListName(Name);
			
			if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
			{
				AimMarkDataScript = _Player.GetComponent<PlayerAimMarkData>();
				if (AimMarkDataScript != null) {
					AimMarkDataScript.RankNo = RankNo;
					IsPlayer = AimMarkDataScript.GetIsPlayer();
					if (!IsPlayer) {
						AiPlayerNetScript = _Player.GetComponent<WaterwheelAiPlayerNet>();
					}
				}
			}
		}
	}

	string GetRankListName(string nameVal)
	{
		string nameRank = "";
		string playerName = "WaterwheelPlayerNetCtrl";
		string playerRankName = "Player";

		string aiplayerName = "WaterwheelAiPlayerNet";
		string aiplayerRankName = "AiPlayer";
		if (string.Compare(nameVal, 0, playerName, 0, playerName.Length) == 0) {
			nameRank = playerRankName + nameVal.Substring(playerName.Length, 2);
		}
		else {			
			nameRank = aiplayerRankName + nameVal.Substring(aiplayerName.Length, 2);
		}
		//Debug.Log("GetRankListName::nameRank --- " + nameRank);
		return nameRank;
	}
}
