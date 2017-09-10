using UnityEngine;
using System.Collections;

public class NetworkServerNet : MonoBehaviour {

	public bool IsServer = false;
	
	string MasterServerIpFile = "./MasterServerIP.info";
	string MasterServerIp = "192.168.0.2";
	private const int port = 23465;

	public Transform NetCtrlPrefab = null;

	bool IsTryToLinkServer = true;
	bool IsCreateServer = true;
	int LinkServerIpCount;
	int IndexSpawnClient;
	float TimeCreateServer;

	private static NetworkServerNet _Instance;
	public static NetworkServerNet GetInstance()
	{
		if (_Instance == null) {
			GameObject obj = new GameObject("_NetworkServerNet");
			DontDestroyOnLoad(obj);
			_Instance = obj.AddComponent<NetworkServerNet>();

			RequestMasterServer.GetInstance();
		}
		return _Instance;
	}

	void Awake()
    {
        if (GameMovieCtrl.GetInstance() != null
            && GameMovieCtrl.GetInstance().GameLinkSt == GameMovieCtrl.GameLinkEnum.NO_LINK)
        {
            return;
        }

        if (!pcvr.bIsHardWare) {
			MasterServerIp = HandleJson.GetInstance().ReadFromFilePathXml(MasterServerIpFile, "MasterServerIp");
			if (MasterServerIp == null || MasterServerIp == "") {
				MasterServerIp = "192.168.0.2";
				HandleJson.GetInstance().WriteToFilePathXml(MasterServerIpFile, "MasterServerIp", MasterServerIp);
			}
		}

		if (MasterServerIp == Network.player.ipAddress) {
			XKMasterServerCtrl.CheckMasterServerIP();
		}

		MasterServer.ipAddress = MasterServerIp;
		Network.natFacilitatorIP = MasterServerIp;
	}

	void Update()
	{
		switch(Network.peerType)
		{
		case NetworkPeerType.Disconnected:
			if (!IsCreateServer)
			{
				TryToCreateServer();
			}
			break;
			
		case NetworkPeerType.Server:
			break;
			
		case NetworkPeerType.Client:
			break;
			
		case NetworkPeerType.Connecting:
			break;
		}
	}

	public void SetIndexSpawnClient(int val)
	{
		IndexSpawnClient = val;
	}

	IEnumerator CheckConnectToServer()
	{
		if (GlobalData.GetInstance().gameLeve != GameLeve.WaterwheelNet) {
			yield break;
		}

		while (true) {
			Debug.Log("***************loadedLevel " + Application.loadedLevel
			          + ", IsIntoPlayGame " + Toubi.GetInstance().IsIntoPlayGame);
			if (Application.loadedLevel == (int)GameLeve.WaterwheelNet) {
				break;
			}

			if (Application.loadedLevel == (int)GameLeve.Movie) {
				if (!Toubi.GetInstance().IsIntoPlayGame) {
					Toubi.GetInstance().StartIntoGame();
					Toubi.GetInstance().IsIntoPlayGame = true;
				}
				yield return new WaitForSeconds(0.5f);
			}
		}

		GlobalData.GetInstance().RemoveNetworkRpc();
		int playerIndex = IndexSpawnClient;
		Debug.Log("CheckConnectToServer::playerIndex " + playerIndex);
		
		GameObject obj = GameNetCtrlXK.GetInstance().PlayerObj[playerIndex];
		Transform tran = GameNetCtrlXK.GetInstance().PlayerPos[playerIndex].transform;
		GameObject player = (GameObject)Network.Instantiate(obj, tran.position, tran.rotation, GlobalData.NetWorkGroup);
		WaterwheelPlayerNetCtrl playerScript = player.GetComponent<WaterwheelPlayerNetCtrl>();
		//playerScript.SetPlayerNetworkPlayer(playerNet);
		playerScript.SetIsHandlePlayer();
		
		//GameNetCtrlXK.GetInstance().SetPlayerList(player, playerIndex);
		
		Spawner.SpawnerScript.SpawnNetObj();
	}

	void OnConnectedToServer()
	{
		Debug.Log("OnConnectedToServer, gameLevel " + GlobalData.GetInstance().gameLeve
		          + ", appLevel " + Application.loadedLevel);

		if (GlobalData.GetInstance().gameLeve == GameLeve.WaterwheelNet) {
			StartCoroutine(CheckConnectToServer());
		}

		//CheckShowAllCamera();
		
		/*if (GlobalData.GetInstance().gameLeve == GameLeve.WaterwheelNet
		    && Application.loadedLevel == (int)GameLeve.WaterwheelNet) {*/
//		if (GlobalData.GetInstance().gameLeve == GameLeve.WaterwheelNet) {
//			GlobalData.GetInstance().RemoveNetworkRpc();
//
//			int playerIndex = IndexSpawnClient;
//			Debug.Log("OnConnectedToServer::playerIndex " + playerIndex);
//			
//			GameObject obj = GameNetCtrlXK.GetInstance().PlayerObj[playerIndex];
//			Transform tran = GameNetCtrlXK.GetInstance().PlayerPos[playerIndex].transform;
//			GameObject player = (GameObject)Network.Instantiate(obj, tran.position, tran.rotation, GlobalData.NetWorkGroup);
//			WaterwheelPlayerNetCtrl playerScript = player.GetComponent<WaterwheelPlayerNetCtrl>();
//			//playerScript.SetPlayerNetworkPlayer(playerNet);
//			playerScript.SetIsHandlePlayer();
//			
//			//GameNetCtrlXK.GetInstance().SetPlayerList(player, playerIndex);
//
//			Spawner.SpawnerScript.SpawnNetObj();
//		}
	}

	void OnFailedToConnect(NetworkConnectionError error)
	{
		ScreenLog.Log("Could not connect to server: " + error);
		if (GlobalData.GetInstance().gameLeve == GameLeve.Movie) {
			InitCreateServer();
		}
	}
	
	void OnServerInitialized()
	{
		Debug.Log("OnServerInitialized, gameLevel " + GlobalData.GetInstance().gameLeve
		          + ", appLevel " + Application.loadedLevel);
		
		//create player
		if (GlobalData.GetInstance().gameLeve == GameLeve.WaterwheelNet ) {
		    //&& Application.loadedLevel == (int)GameLeve.WaterwheelNet) {

			StartCoroutine(CheckServerInitialized());
		}
	}

	IEnumerator CheckServerInitialized()
	{
		bool isCheck = true;
		while (isCheck) {
			
			yield return new WaitForSeconds(0.1f);
			if (Application.loadedLevel != (int)GameLeve.WaterwheelNet || Network.peerType == NetworkPeerType.Disconnected) {
				
				if (Toubi.GetInstance() != null
				    && !Toubi.GetInstance().IsIntoPlayGame) {
					Toubi.GetInstance().IsIntoPlayGame = true;
				}
				continue;
			}
			isCheck = false;
		}
		GlobalData.GetInstance().RemoveNetworkRpc();
		
		GameObject obj = GameNetCtrlXK.GetInstance().PlayerObj[0];
		Transform tran = GameNetCtrlXK.GetInstance().PlayerPos[0].transform;
		GameObject player = (GameObject)Network.Instantiate(obj, tran.position, tran.rotation, GlobalData.NetWorkGroup);
		WaterwheelPlayerNetCtrl playerScript = player.GetComponent<WaterwheelPlayerNetCtrl>();
		playerScript.SetIsHandlePlayer();
		
		Spawner.SpawnerScript.SpawnNetObj();
		
		CreateAiPlayer(); //create AiPlayer
	}

	void CreateAiPlayer()
	{
		if (LinkServerIpCount + RankingCtrl.ServerPlayerRankNum >= RankingCtrl.MaxPlayerRankNum) {
			return;
		}

		int aiPlayerMax = RankingCtrl.MaxPlayerRankNum - RankingCtrl.ServerPlayerRankNum - LinkServerIpCount;
		int aiPosNum = RankingCtrl.ServerPlayerRankNum + LinkServerIpCount;

		GameObject obj;
		Transform tran;
		GameObject player;
		WaterwheelAiPlayerNet playerScript;
		for (int i = 0; i < aiPlayerMax; i++) {
			obj = GameNetCtrlXK.GetInstance().NpcObj[i];
			tran = GameNetCtrlXK.GetInstance().PlayerPos[aiPosNum].transform;
			player = (GameObject)Network.Instantiate(obj, tran.position, tran.rotation, GlobalData.NetWorkGroup);
			playerScript = player.GetComponent<WaterwheelAiPlayerNet>();
			playerScript.SetIsHandlePlayer();

			aiPosNum++;
		}
		LinkServerIpCount = 0;
	}

	void OnPlayerConnected(NetworkPlayer playerNet)
	{
		//CheckShowAllCamera();
		ScreenLog.Log("NetworkServerNet::OnPlayerConnected -> ip " + playerNet.ipAddress
		              + ", gameLevel " + GlobalData.GetInstance().gameLeve
		              + ", appGameLevel " + Application.loadedLevel);
		
		if (GlobalData.GetInstance().gameLeve == GameLeve.WaterwheelNet
		    && Application.loadedLevel == (int)GameLeve.WaterwheelNet) {

			StartCoroutine(CheckOpenAllCamera());
		}
		else if (GlobalData.GetInstance().gameLeve == GameLeve.Movie) {
			LinkServerIpCount = Network.connections.Length;
			NetworkRpcMsgCtrl.GetInstance().SetSpawnClientIndex(playerNet, Network.connections.Length);
		}
	}

	IEnumerator CheckOpenAllCamera()
	{
		if (Network.isServer) {
			Debug.Log("CheckOpenAllCamera ***** over");
			yield break;
		}

		while (WaterwheelPlayerNetCtrl.GetInstance() == null) {
			yield return new WaitForSeconds(0.5f);
		}
		WaterwheelPlayerNetCtrl.GetInstance().CheckServerPortPlayerLoop();
	}
	
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		ScreenLog.Log("NetworkServerNet::OnPlayerDisconnected -> ip " + player.ipAddress);
		RemoveAllRPC(player);
	}

	void OnDisconnectedFromServer(NetworkDisconnection info) {
		if (Network.isServer) {
			Debug.Log("Local server connection disconnected");
		}
		else if (info == NetworkDisconnection.LostConnection) {
			Debug.Log("Lost connection to the server");
		}
		else {
			Debug.Log("Successfully diconnected from the server");
			RequestMasterServer.TimeConnectServer = Time.realtimeSinceStartup;
			if (GlobalData.GetInstance().gameMode == GameMode.OnlineMode
			    && Toubi.GetInstance() != null && !Toubi.GetInstance().IsIntoPlayGame) {
				Toubi.GetInstance().IsIntoPlayGame = true;
			}
		}
	}

	public void InitTryToLinkServer()
	{
		if(!IsTryToLinkServer)
		{
			return;
		}
		IsTryToLinkServer = false;
	}

	public void InitCreateServer()
	{
		if (GlobalData.GetInstance().gameLeve == GameLeve.Movie) {

			if (Time.realtimeSinceStartup - TimeCreateServer < 8f) {
				//Debug.Log("test**********************InitCreateServer");
				return;
			}
			TimeCreateServer = Time.realtimeSinceStartup;
			int randVal = (Random.Range(0, 100) % 4) * 3;
			Debug.Log("InitCreateServer -> randVal ***** " + randVal);
			Invoke("DelayInitCreateServer", randVal);
			return;
		}

		if (!IsCreateServer) {
			return;
		}
		IsCreateServer = false;
	}

	void DelayInitCreateServer()
	{
		if (!IsCreateServer) {
			return;
		}
		IsCreateServer = false;
	}

	void RemoveAllRPC(NetworkPlayer playerNet)
	{
		if (Network.isServer) {
			Network.RemoveRPCs(playerNet);
			Network.DestroyPlayerObjects(playerNet);
		}
	}

	void RemoveAllClientRPC()
	{
		if (!Network.isServer) {
			return;
		}

		int max = Network.connections.Length;
		if (max > 0) {
			NetworkPlayer [] netPlayerArray = new NetworkPlayer[max];
			for (int i = 0; i < max; i++) {
				Network.CloseConnection(netPlayerArray[i], true);
			}
		}
	}

	public void RemoveMasterServerHost()
	{
		if (Network.isServer) {
			//RemoveAllClientRPC();
			RemoveAllRPC(Network.player);
		}

		Network.Disconnect(30);
		MasterServer.UnregisterHost();
	}

	void CloseMasterServerHost()
	{
		MasterServer.dedicatedServer = false;
	}

	public void ResetMasterServerHostLoop()
	{
		if (Network.peerType != NetworkPeerType.Server) {
			return;
		}

		if (Network.connections.Length > 0) {
			//Debug.Log("ResetMasterServerHostLoop**********");
			Invoke("ResetMasterServerHostLoop", 1f);
			return;
		}
		Application.LoadLevel((int)GameLeve.Movie);
		ResetMasterServerHost();
	}

	public void ResetMasterServerHost()
	{
		RequestMasterServer.GetInstance().ResetIsClickConnect();
		if (Network.peerType != NetworkPeerType.Server) {

			if (Network.peerType != NetworkPeerType.Disconnected) {
				Network.Disconnect(30);
			}
			return;
		}

		RemoveMasterServerHost();
		CloseMasterServerHost();
	}

	void TryToCreateServer()
	{
		if (IsCreateServer) {
			return;
		}
		IsCreateServer = true;

		if (!MasterServer.dedicatedServer && GlobalData.GetInstance().gameLeve == GameLeve.WaterwheelNet) {
			return;
		}

		ScreenLog.Log("start create to server");
		Network.InitializeServer(7, port, true);

//		Debug.Log("masterServer.ip " + MasterServer.ipAddress + ", port " + MasterServer.port
//		          + ", updateRate " + MasterServer.updateRate);
		MasterServer.dedicatedServer = true;
		//MasterServer.RegisterHost("MyUniqueGameType", "JohnDoes game", "l33t game for all");

		if (GlobalData.GetInstance().gameLeve == GameLeve.None) {
			GlobalData.GetInstance().gameLeve = (GameLeve)Application.loadedLevel;
		}

		switch (GlobalData.GetInstance().gameLeve) {
		case GameLeve.Movie:
			RequestMasterServer.GetInstance().SetMasterServerIp(Network.player.ipAddress);
			MasterServer.RegisterHost("MyUniqueGameType", "JohnDoes game", RequestMasterServer.MasterServerMovieComment);
			break;

		case GameLeve.WaterwheelNet:
			MasterServer.RegisterHost("MyUniqueGameType", "JohnDoes game", RequestMasterServer.MasterServerGameNetComment);
			break;
		}
	}
	
	void OnApplicationQuit()
	{
		Debug.Log("OnApplicationQuit...NetServer");
		if (MasterServerIp == Network.player.ipAddress) {
			XKMasterServerCtrl.CloseMasterServer();
		}
	}
}