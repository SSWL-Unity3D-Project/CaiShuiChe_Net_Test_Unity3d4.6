using UnityEngine;
using System;
using System.IO;

public enum GameDiffState : int
{
	GameDiffLow,
	GameDiffMiddle,
	GameDiffHigh
}

public class GlobalData  {

	private static  GlobalData Instance;

	public static int NetWorkGroup = 0;
	static string startCoinInfo = "";
	static string FilePath = "";
	static public string fileName = "../config/XKGameConfig.xml";
	static public HandleJson handleJsonObj = null;
	public static int GameAudioVolume = 7;

	static public string bikeGameCtrl = "bikegamectrl";
	static public string netCtrl = "_NetCtrl";
	static public string NetworkServerNet = "_NetworkServerNet";
	
	public static Vector3 HiddenPosition = new Vector3(0f, -1000f, 0f);

	public bool IsActiveJuLiFu;
	public float PlayerAmmoFrequency = 10f;
	public float MinDistancePlayer = 50f;
	public static float PlayerPowerShipMax = 2000f;

	private GlobalData()
	{
		//gameLeve=GameLeve.None;
		gameLeve = (GameLeve)Application.loadedLevel;
		gameMode = GameMode.None;
		//InitPlayer();
	}
//	public void InitPlayer()
//	{
//		player=new Player();
//		player.Energy = 100f;
//		player.Speed = 0;
//		player.Score = 0;
//		player.Life = 30;
//		player.LotteryCount = 0;
//		player.FinalRank = 8;
//	}
	public static GlobalData GetInstance()
	{
		if(Instance==null)
		{
			Instance=new GlobalData();
			Instance.InitInfo();
			if (!Directory.Exists(FilePath)) {
				Directory.CreateDirectory(FilePath);
			}

			//init gameMode
			Instance.gameMode = GameMode.OnlineMode;
			if(Application.loadedLevel == (int)GameLeve.Waterwheel)
			{
				Instance.gameMode = GameMode.SoloMode;
			}

			if(handleJsonObj == null)
			{
				handleJsonObj = HandleJson.GetInstance();
			}

			//start coin info
			startCoinInfo = handleJsonObj.ReadFromFileXml(fileName, "START_COIN");
			if(startCoinInfo == null || startCoinInfo == "")
			{
				startCoinInfo = "1";
				handleJsonObj.WriteToFileXml(fileName, "START_COIN", startCoinInfo);
			}
			Instance.XUTOUBI = Convert.ToInt32( startCoinInfo );
			//Instance.XUTOUBI = 3; //test

			//free mode
			bool isFreeModeTmp = false;
			string modeGame = handleJsonObj.ReadFromFileXml(fileName, "GAME_MODE");
			if(modeGame == null || modeGame == "")
			{
				modeGame = "1";
				handleJsonObj.WriteToFileXml(fileName, "GAME_MODE", modeGame);
			}

			if(modeGame == "0")
			{
				isFreeModeTmp = true;
			}
			//isFreeModeTmp = true; //test
			Instance.IsFreeMode = isFreeModeTmp;

			if(Application.loadedLevel == (int)GameLeve.Movie && Application.loadedLevelName == GameLeve.Movie.ToString())
			{
				Toubi.GetInstance().ShowInsertCoinImg();
			}

			//output caiPiao
			/*bool isOutput = true;
			string outputStr = handleJsonObj.ReadFromFileXml(fileName, "GAME_OUTPUT_TICKET");
			if(outputStr == null || outputStr == "")
			{
				outputStr = "1";
				handleJsonObj.WriteToFileXml(fileName, "GAME_OUTPUT_TICKET", outputStr);
			}

			if(outputStr == "0")
			{
				isOutput = false;
			}
			Instance.IsOutputCaiPiao = isOutput;*/

			//coin to card num
			/*string ticketNumStr = handleJsonObj.ReadFromFileXml(fileName, "GAME_TICKET_NUM");
			if(ticketNumStr == null || ticketNumStr == "")
			{
				ticketNumStr = "10";
				handleJsonObj.WriteToFileXml(fileName, "GAME_TICKET_NUM", ticketNumStr);
			}
			float ticketNum = (float)Convert.ToInt32( ticketNumStr );
			Instance.CointToTicket = ticketNum;*/

			//ticket rate
			/*string ticketRateStr = handleJsonObj.ReadFromFileXml(fileName, "GAME_TICKET_RATE");
			if(ticketRateStr == null || ticketRateStr == "")
			{
				ticketRateStr = "1";
				handleJsonObj.WriteToFileXml(fileName, "GAME_TICKET_RATE", ticketRateStr);
			}
			Instance.TicketRate = ticketRateStr;*/

			//game diff
			string diffStr = handleJsonObj.ReadFromFileXml(fileName, "GAME_DIFFICULTY");
			if(diffStr == null || diffStr == "")
			{
				diffStr = "1";
				handleJsonObj.WriteToFileXml(fileName, "GAME_DIFFICULTY", diffStr);
			}
			Instance.GameDiff = diffStr;
			
			string val = handleJsonObj.ReadFromFileXml(fileName, "GameAudioVolume");
			if (val == null || val == "") {
				val = "7";
				handleJsonObj.WriteToFileXml(fileName, "GameAudioVolume", val);
			}
			GameAudioVolume = Convert.ToInt32(val);

			//SetPanelCtrl.GetInstance();
		}
		return Instance;
	}
	
	void InitInfo()
	{
		FilePath = Application.dataPath + "/../config";
	}

	//public Player player;
	public int XUTOUBI=3;
	public bool IsFreeMode = false;
	public bool IsOutputCaiPiao = true;
	public string TicketRate = "1";
	public float CointToTicket = 10f;
	public string GameDiff = "1"; //0 -> diffLow, 1 -> diffMiddel, 2 -> diffHigh

	public readonly int GAMETIME=90;
	//public int xutoubi;
	public delegate void EventHandel();
	public GameMode gameMode;
	public GameLeve gameLeve;
	public bool playCartoonEnd;
	//public event EventHandel IcoinCountChange;
	private int _icon;
	public int Icoin
	{
		get
		{
			return _icon;
		}
		set
		{
			_icon=value;
//			if(IcoinCountChange!=null)
//			{
//				IcoinCountChange();
//			}
		}
	}

	public int BikeHeadSpeedState = 2;
	public int BikeZuLiDengJi = 0;
}

public enum GameMode
{
	None,
	SoloMode,
	OnlineMode
}

public enum GameLeve:int
{
	None				= -1,
	Movie				= 0,
	Waterwheel			= 1,
	WaterwheelNet		= 2,
	SetPanel			= 3
}