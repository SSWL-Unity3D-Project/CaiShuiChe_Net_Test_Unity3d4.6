using UnityEngine;
using System.Collections;

public class HeadCtrlPlayer : MonoBehaviour {
	public PlayerBtState PlayerState;

	TweenColor HeadColor;
	TweenScale HeadScale;
	UISprite HeadSprite;

	bool IsChangeHead;
	bool IsHeadToSmall;
	
	public static HeadCtrlPlayer _InstanceP1;
	public static HeadCtrlPlayer GetInstanceP1()
	{
		return _InstanceP1;
	}

	public static HeadCtrlPlayer _InstanceP2;
	public static HeadCtrlPlayer GetInstanceP2()
	{
		return _InstanceP2;
	}

	// Use this for initialization
	void Awake()
	{
		switch(PlayerState)
		{
		case PlayerBtState.PLAYER_1:
			_InstanceP1 = this;
			break;
		
		case PlayerBtState.PLAYER_2:
			_InstanceP2 = this;
			break;
		}
		HeadColor = GetComponent<TweenColor>();
		HeadScale = GetComponent<TweenScale>();
		HeadSprite = GetComponent<UISprite>();

		//Invoke("TestDaleyInitChangeHeadUI", 3f); //test
	}

	void TestDaleyInitChangeHeadUI()
	{
		InvokeRepeating("InitChangeHeadUI", 0f, 0.2f);
		//InitChangeHeadUI();
	}

	public void InitChangeHeadUI()
	{
		if (IsChangeHead) {
			return;
		}
		IsChangeHead = true;
		IsHeadToSmall = false;
		MakeHeadToBig();
	}

	void MakeHeadToBig()
	{
		if (!IsChangeHead) {
			return;
		}

		if (IsHeadToSmall) {
			EventDelegate.Remove(HeadScale.onFinished, delegate{
				MakeHeadToBig();
			});

			IsHeadToSmall = false;
			IsChangeHead = false;
			return;
		}
		//Debug.Log("********************");

		HeadScale.from = new Vector3(1f, 1f, 1f);
		HeadScale.to = new Vector3(1.3f, 1.3f, 1f);

		EventDelegate.Add(HeadScale.onFinished, delegate{
			MakeHeadToSmall();
		});
		HeadScale.enabled = true;
		HeadScale.PlayForward();
	}

	void MakeHeadToSmall()
	{
		IsHeadToSmall = true;
		EventDelegate.Add(HeadScale.onFinished, delegate{
			MakeHeadToBig();
		});
		HeadScale.enabled = true;
		HeadScale.PlayReverse();
	}

	public void PlayColor()
	{
		HeadColor.enabled = true;
		HeadColor.ResetToBeginning();
		HeadColor.PlayForward();
	}

	public void StopColor()
	{
		HeadColor.enabled = false;
		HeadColor.ResetToBeginning();
		HeadSprite.color = new Color(255f, 255f, 255f); //old color
	}

	public void SetHeadColor()
	{
		HeadColor.enabled = false;
		HeadColor.ResetToBeginning();
		HeadSprite.color = new Color(0f, 0f, 0f); //new color
	}
	
	public void SetPlayerHeadIsActive(bool isActive)
	{
		if (gameObject.activeSelf != isActive) {
			gameObject.SetActive(isActive);
		}
	}
}
