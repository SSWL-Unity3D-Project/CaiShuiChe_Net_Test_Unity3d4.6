#define SHOW_NET_INFO

using UnityEngine;

public class RequestMasterServer : MonoBehaviour
{
	bool IsClickConnect;
	public static string MasterServerMovieComment = "Waterwheel Movie Scence";
	public static string MasterServerGameNetComment = "Waterwheel GameNet Scence";
	string ServerIp = "";
	float TimeConnect;

	static RequestMasterServer _Instance;
	public static RequestMasterServer GetInstance()
	{
		if (_Instance == null) {
			GameObject obj = new GameObject("_RequestMasterServer");
			_Instance = obj.AddComponent<RequestMasterServer>();
			DontDestroyOnLoad(obj);
		}
		return _Instance;
	}

	void Start() {
		InitLoopRequestHostList();

		CancelInvoke("CheckMasterServerList");
		InvokeRepeating("CheckMasterServerList", 3f, 0.1f);
	}

	void InitLoopRequestHostList()
	{
		CancelInvoke("RequestHostListLoop");
		InvokeRepeating("RequestHostListLoop", 0f, 3f);
	}

	void RequestHostListLoop()
	{
		MasterServer.RequestHostList(NetworkServerNet.GetInstance().mGameTypeName);
	}

	float RandConnectTime = Random.Range(3f, 10f);
	public static float TimeConnectServer = 0f;
	void OnGUI()
    {
		GameLeve levelVal = GlobalData.GetInstance().gameLeve;
		HostData[] data = MasterServer.PollHostList();

		// Go through all the hosts in the host list
		foreach (var element in data) {
#if SHOW_NET_INFO
            var name = element.gameName + " " + element.connectedPlayers + " / " + element.playerLimit;
			GUILayout.BeginHorizontal();	
			GUILayout.Box(name);	
			GUILayout.Space(5);

            var hostInfo = "[";
			foreach (var host in element.ip) {
				hostInfo = hostInfo + host + ":" + element.port + " ";
			}
			hostInfo = hostInfo + "]";
            GUILayout.Box(hostInfo);
            GUILayout.Space(5);
			GUILayout.Box(element.comment);
			GUILayout.Space(5);
			GUILayout.FlexibleSpace();
#endif

            if (element.comment == MasterServerGameNetComment
			    && ServerIp == element.ip[0]
			    && Toubi.GetInstance() != null
			    && !Toubi.GetInstance().IsIntoPlayGame) {
				Toubi.GetInstance().IsIntoPlayGame = true;
			}

			if (Network.peerType == NetworkPeerType.Disconnected) {

				if (!IsClickConnect) {

					bool isConnectServer = false;
					if ( levelVal == GameLeve.WaterwheelNet
					      && element.comment == MasterServerGameNetComment
					      && element.ip[0] != Network.player.ipAddress
					      && ServerIp == element.ip[0] ) {
						
						if (Time.realtimeSinceStartup - TimeConnectServer > RandConnectTime) {
							isConnectServer = true;
							TimeConnectServer = Time.realtimeSinceStartup;
							RandConnectTime = Random.Range(3f, 10f);
						}
					}
					else if ( levelVal == GameLeve.Movie
						         && element.comment == MasterServerMovieComment
						         && element.ip[0] != Network.player.ipAddress
					        	 && element.connectedPlayers < element.playerLimit
						         && Toubi.GetInstance() != null
						         && Toubi.GetInstance().CheckIsLoopWait() ) {
						
						if (Time.realtimeSinceStartup - TimeConnectServer > RandConnectTime) {
							isConnectServer = true;
							TimeConnectServer = Time.realtimeSinceStartup;
							RandConnectTime = Random.Range(3f, 10f);
						}
					}
					
					if (isConnectServer) {
						// Connect to HostData struct, internally the correct method is used (GUID when using NAT).
						Network.RemoveRPCs(Network.player);
						Network.DestroyPlayerObjects(Network.player);

						MasterServer.dedicatedServer = false;
						Network.Connect(element);
						IsClickConnect = true;
						if (levelVal == GameLeve.Movie) {
							ServerIp = element.ip[0];
							TimeConnect = 0f;
						}
						Debug.Log("Connect element.ip -> " + element.ip[0]
						          + ", element.comment " + element.comment
						          + ", gameLeve " + levelVal
						          + ", time " + Time.realtimeSinceStartup.ToString("f2"));
					}
				}
				else {
					if (levelVal == GameLeve.WaterwheelNet) {

						if (element.comment == MasterServerGameNetComment && ServerIp == element.ip[0]) {
							
							TimeConnect += Time.deltaTime;
							if (TimeConnect >= 2f) {
								TimeConnect = 0f;
								IsClickConnect = false;
							}
						}
					}
					else if (levelVal == GameLeve.Movie) {

						TimeConnect += Time.deltaTime;
						if (TimeConnect >= 2f) {
							TimeConnect = 0f;
							IsClickConnect = false;
							Debug.Log("reconnect masterServer...");
						}
					}
				}
            }
#if SHOW_NET_INFO
            GUILayout.EndHorizontal();
#endif
        }
    }

    public void ResetIsClickConnect()
	{
		IsClickConnect = false;
	}

	public void SetMasterServerIp(string ip)
	{
		ServerIp = ip;
	}

	public int GetMovieMasterServerNum()
	{
		int masterServerNum = 0;
		HostData[] data = MasterServer.PollHostList();
		
		// Go through all the hosts in the host list
		foreach (var element in data)
		{
			if (element.comment == MasterServerMovieComment) {
				masterServerNum++;
			}
		}
		return masterServerNum;
	}

	//float TestDVal;
	void CheckMasterServerList()
	{
		int masterServerNum = 0;
		//int masterServerGameNetNum = 0;
		bool isCreatMasterServer = true;
		HostData[] data = MasterServer.PollHostList();
		
		// Go through all the hosts in the host list
		foreach (var element in data)
		{
			if (element.comment == MasterServerMovieComment) {
				masterServerNum++;
				if (Network.peerType == NetworkPeerType.Disconnected) {
					if (masterServerNum > 0) {
						isCreatMasterServer = false;
					}
				}
				else  if (Network.peerType == NetworkPeerType.Server)
				{
					if (masterServerNum > 1 && Random.Range(0, 100) % 2 == 1) {
						isCreatMasterServer = false;
						Debug.Log("random remove masterServer...");
					}
				}
			}
			else if (element.comment == MasterServerGameNetComment && element.ip[0] == ServerIp) {
				//masterServerGameNetNum++;
			}
		}

		GameLeve levelVal = GlobalData.GetInstance().gameLeve;
		if (levelVal == GameLeve.None || levelVal == GameLeve.Waterwheel || levelVal == GameLeve.SetPanel)
		{
			isCreatMasterServer = false;
		}

		switch (Network.peerType) {
		case NetworkPeerType.Disconnected:
			if (isCreatMasterServer) {

				if (levelVal == GameLeve.Movie) {

					if ( ( Toubi.GetInstance() != null && !Toubi.GetInstance().CheckIsLoopWait() )
					    || Toubi.GetInstance() == null ) {

						return;
					}
					ServerIp = "";
				}
				NetworkServerNet.GetInstance().InitCreateServer();
				//MasterServerTime = Time.realtimeSinceStartup;
			}
			break;
			
		case NetworkPeerType.Server:
			if (!isCreatMasterServer) {
				NetworkServerNet.GetInstance().RemoveMasterServerHost();
			}
			else {
				if (levelVal == GameLeve.Movie) {
					
					//MasterServerTime = Time.realtimeSinceStartup;
					if (Toubi.GetInstance() != null && !Toubi.GetInstance().CheckIsLoopWait()) {
						NetworkServerNet.GetInstance().ResetMasterServerHost();
					}
				}
				/*else if (levelVal == GameLeve.WaterwheelNet) {

					if (masterServerGameNetNum == 0) {
						TestDVal = Time.realtimeSinceStartup - MasterServerTime;
						if (Time.realtimeSinceStartup - MasterServerTime > 10f) {
							Debug.Log("no masterServer...");
							NetworkServerNet.GetInstance().RemoveMasterServerHost();
							MasterServerTime = Time.realtimeSinceStartup;
						}
					}
				}*/
			}
			break;
		}
	}

	void OnFailedToConnectToMasterServer(NetworkConnectionError info) {
		Debug.Log("Could not connect to master server: " + info);
		//if (Application.loadedLevel == (int)GameLeve.Movie) {
		//	ServerLinkInfo.GetInstance().SetServerLinkInfo("Cannot Link MasterServer");
		//}
	}

	void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        //Debug.Log("OnMasterServerEvent: " + msEvent + ", time " + Time.time);
        if (msEvent == MasterServerEvent.RegistrationSucceeded) {
			Debug.Log("MasterServer registered, GameLevel " + Application.loadedLevel);
			if (Application.loadedLevel == 0)
            {
                //只在循环动画场景执行!
                //ServerLinkInfo.GetInstance().HiddenServerLinkInfo();
                NetworkRootMovie.GetInstance().mNetworkRpcMsgSpawn.CreateNetworkRpc();
            }
		}
	}
}

