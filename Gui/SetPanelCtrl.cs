using UnityEngine;
using System.Collections;

public class SetPanelCtrl : MonoBehaviour {

	static private SetPanelCtrl Instance = null;

	static public SetPanelCtrl GetInstance()
	{
		if(Instance == null)
		{
			GameObject obj = new GameObject("_SetPanelCtrl");
			//DontDestroyOnLoad(obj);
			Instance = obj.AddComponent<SetPanelCtrl>();

			pcvr.GetInstance();
			
			if (Application.loadedLevel == (int)GameLeve.Movie) {
				NetworkServerNet.GetInstance();
			}
			ScreenLog.init();
		}
		return Instance;
	}

	void Start()
	{
		InputEventCtrl.GetInstance().ClickSetEnterBtEvent += ClickSetEnterBtEvent;
	}

	void ClickSetEnterBtEvent(ButtonState val)
	{
		if(val == ButtonState.DOWN)
		{
			return;
		}

		//link mode, donot into setPanel
		if(Application.loadedLevel == (int)GameLeve.WaterwheelNet || !FinishPanelCtrl.IsCanLoadSetPanel)
		{
			return;
		}

		if(Application.loadedLevel != (int)GameLeve.SetPanel)
		{
			loadLevelSetPanel();
		}
	}
	public static bool IsIntoSetPanel = false;
	public static GameObject SetPanelObj;

	void loadLevelSetPanel()
	{
		//ScreenLog.Log("SetPanelCtrl -> loadLevelSetPanel...");
		GlobalData.GetInstance().gameLeve = GameLeve.SetPanel;
		System.GC.Collect();
		Application.LoadLevel( (int)GameLeve.SetPanel );
	}

	void loadMoveLevel()
	{
		GlobalData.GetInstance().gameLeve = GameLeve.Movie;
		System.GC.Collect();
		Application.LoadLevel( (int)GameLeve.Movie );
	}
}