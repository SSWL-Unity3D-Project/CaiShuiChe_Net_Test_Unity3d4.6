using UnityEngine;
using System.Collections;

public class ZhuJiaoNv : MonoBehaviour {

	float CenterPerPx = 0.3f;
	int PosX_1;
	int PosX_2;
	public static bool FireRight;
	public static bool FireLeft;
	public static bool IsFire;
	bool Fail;
	bool Win_1;
	bool Win_2;

	Animator AnimatorCom;
	bool IsFireOtherPort;

	static ZhuJiaoNv _Instance;
	public static ZhuJiaoNv GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;
		CenterPerPx = GameCtrlXK.GetInstance().ZhuJiaoNvCenterPerPx;
		AnimatorCom = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update()
	{
		if (Fail || Win_1 || Win_2) {
			return;
		}

		if (IsFire) {
			PosX_1 = (int)(608f * (1f - CenterPerPx));
			PosX_2 = (int)(608f * (1f + CenterPerPx));
			if (pcvr.CrossPosition.x < PosX_1) {
				if (!FireLeft) {
					FireLeft = true;
					FireRight = false;
					AnimatorCom.SetBool("FireRight", false);
					AnimatorCom.SetBool("FireLeft", true);
				}
			}
			else if (pcvr.CrossPosition.x > PosX_2) {
				if (!FireRight) {
					FireLeft = false;
					FireRight = true;
					AnimatorCom.SetBool("FireRight", true);
					AnimatorCom.SetBool("FireLeft", false);
				}
			}
			else {
				if (FireRight || FireLeft) {
					FireLeft = false;
					FireRight = false;
					AnimatorCom.SetBool("FireRight", false);
					AnimatorCom.SetBool("FireLeft", false);
				}
			}
		}
	}

	public void OpenFireOtherPort()
	{
		if (IsFireOtherPort) {
			return;
		}
		IsFireOtherPort = true;
		AnimatorCom.SetBool("IsFire", true);
		AnimatorCom.SetBool("FireRight", false);
		AnimatorCom.SetBool("FireLeft", false);
	}

	public void CloseFireOtherPort()
	{
		if (!IsFireOtherPort) {
			return;
		}
		IsFireOtherPort = false;
		AnimatorCom.SetBool("IsFire", false);
		AnimatorCom.SetBool("FireRight", false);
		AnimatorCom.SetBool("FireLeft", false);
	}

	public void UpdateZhuJiaoNvFireAction(float crossPx)
	{
		if (Fail || Win_1 || Win_2) {
			return;
		}

		if (!IsFireOtherPort) {
			return;
		}

		PosX_1 = (int)(608f * (1f - CenterPerPx));
		PosX_2 = (int)(608f * (1f + CenterPerPx));
		if (crossPx < PosX_1) {
			if (!FireLeft) {
				FireLeft = true;
				FireRight = false;
				AnimatorCom.SetBool("FireRight", false);
				AnimatorCom.SetBool("FireLeft", true);
			}
		}
		else if (crossPx > PosX_2) {
			if (!FireRight) {
				FireLeft = false;
				FireRight = true;
				AnimatorCom.SetBool("FireRight", true);
				AnimatorCom.SetBool("FireLeft", false);
			}
		}
		else {
			if (FireRight || FireLeft) {
				FireLeft = false;
				FireRight = false;
				AnimatorCom.SetBool("FireRight", false);
				AnimatorCom.SetBool("FireLeft", false);
			}
		}
	}

	public void OpenFire()
	{
		if (IsFire) {
			return;
		}
		IsFire = true;
		AnimatorCom.SetBool("IsFire", true);
		AnimatorCom.SetBool("FireRight", false);
		AnimatorCom.SetBool("FireLeft", false);
	}
	
	public void CloseFire()
	{
		if (!IsFire) {
			return;
		}
		IsFire = false;
		AnimatorCom.SetBool("IsFire", false);
		AnimatorCom.SetBool("FireRight", false);
		AnimatorCom.SetBool("FireLeft", false);
	}

	public void PlayFailAction()
	{
		Fail = true;
		AnimatorCom.SetBool("Fail", true);
		CloseFire();
	}
	
	public void PlayWinAction(int key)
	{
		if (key == 1) {
			Win_1 = true;
			AnimatorCom.SetBool("Win_1", true);
		}
		else {
			Win_2 = true;
			AnimatorCom.SetBool("Win_2", true);
		}
		CloseFire();
	}
}