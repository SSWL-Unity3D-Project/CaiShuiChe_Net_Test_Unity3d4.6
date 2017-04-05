using UnityEngine;
using System.Collections;

public class SetPanelJiaoZhunDianCtrl : MonoBehaviour
{
	public Transform JiaoZhunDianTr;
	GameObject JiaoZhunDianObj;
	int IndexJiaoZhun;
	int StartX = -3;
	int StartY = 3;
	static SetPanelJiaoZhunDianCtrl _Instance;
	public static SetPanelJiaoZhunDianCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;
		UITexture TexturePoint = JiaoZhunDianTr.GetComponent<UITexture>();
		if (pcvr.bIsHardWare && pcvr.IsGetValByKey) {
			TexturePoint.color = Color.black;
		}
		else {
			TexturePoint.color = Color.red;
		}
		JiaoZhunDianObj = JiaoZhunDianTr.gameObject;
		JiaoZhunDianObj.SetActive(false);
		InputEventCtrl.GetInstance().ClickFireBtEvent += ClickFireBtEvent;
	}

	public void OpenJiaoZhunDian()
	{
		Vector3 pos = Vector3.zero;
		if (pcvr.IsUseZhunXingJZ_36) {
			StartX = -3;
			StartY = 3;
			pos.x = pcvr.ScreenW * StartX;
			pos.y = pcvr.ScreenH * StartY;
		}
		
		if (pcvr.IsUseLineHitCross) {
			StartX = 0;
			StartY = 0;
			pos.x = -3f * pcvr.ScreenW;
			pos.y = 3 * pcvr.ScreenH;
		}

		JiaoZhunDianTr.localPosition = pos;
		JiaoZhunDianObj.SetActive(true);
	}

	void CloseJiaoZhunDian()
	{
		JiaoZhunDianObj.SetActive(false);
		StartX = -3;
		StartY = 3;
		Vector3 pos = Vector3.zero;
		pos.x = pcvr.ScreenW * StartX;
		pos.y = pcvr.ScreenH * StartY;
		JiaoZhunDianTr.localPosition = pos;
	}
	
	void ClickFireBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			return;
		}

		switch (pcvr.JZPoint) {
		case PcvrJZCrossPoint.Num49:
			StartX++;
			if (StartX > 3) {
				StartX = -3;
				StartY--;
				if (StartY < -3) {
					CloseJiaoZhunDian();
					return;
				}
			}
			break;

		case PcvrJZCrossPoint.Num7:
			StartX++;
			StartY--;
			if (StartX > 3) {
				CloseJiaoZhunDian();
				return;
			}
			break;

		case PcvrJZCrossPoint.Num4:
			StartX++;
			StartY++;
			if (StartX > 3) {
				CloseJiaoZhunDian();
				return;
			}
			break;
		}

		Vector3 pos = Vector3.zero;
		if (pcvr.IsUseZhunXingJZ_36) {
			pos.x = pcvr.ScreenW * StartX;
			pos.y = pcvr.ScreenH * StartY;
		}

		if (pcvr.IsUseLineHitCross) {
			if (StartX == 0 || StartX == 3) {
				pos.x = -3f * pcvr.ScreenW;
			}
			
			if (StartX == 1 || StartX == 2) {
				pos.x = 3f * pcvr.ScreenW;
			}

			if (StartY == 0 || StartY == 1) {
				pos.y = 3f * pcvr.ScreenH;
			}
			
			if (StartY == 2 || StartY == 3) {
				pos.y = -3f * pcvr.ScreenH;
			}
		}
		JiaoZhunDianTr.localPosition = pos;
	}
}