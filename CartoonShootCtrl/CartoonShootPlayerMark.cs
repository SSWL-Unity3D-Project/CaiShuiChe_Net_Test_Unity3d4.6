using UnityEngine;
using System.Collections;

public class CartoonShootPlayerMark : MonoBehaviour {
	
	public int MarkIndex;
	public CartoonShootPlayerPath ShootPlayerPath;
	[Range(0.1f, 1000f)] public float MoveSpeed = 1f;
	[Range(0f, 1000f)] public float LookTime = 1f;

	public GameObject[] NpcObj;
	public string[] ActionState;
	[Range(0f, 10f)]public float DelayTimeCloseAction = 0.5f;
	Animator[] AniComponent = new Animator[10];

	Transform TriggerTran;
	float TimeVal;
	bool IsStopUpdateInfo;

	void Start()
	{
		TriggerTran = transform;
		for (int i = 0; i < NpcObj.Length; i++) {
			if (ActionState[i] != "" && NpcObj[i] != null) {
				AniComponent[i] = NpcObj[i].GetComponent<Animator>();
			}
		}
	}

	void Update()
	{
		if (IsStopUpdateInfo) {
			return;
		}

		if (Time.realtimeSinceStartup - TimeVal < 0.1f) {
			return;
		}
		TimeVal = Time.realtimeSinceStartup;
		
		Vector3 vecA = TriggerTran.position;
		Vector3 vecB = CartoonShootCamCtrl.GZhuJiaoTran.position;
		vecA.y = vecB.y = 0f;
		float dis = Vector3.Distance(vecA, vecB);
		if (dis <= 12f) {
			
			vecA = TriggerTran.forward;
			vecB = TriggerTran.position - CartoonShootCamCtrl.GZhuJiaoTran.position;
			vecA.y = vecB.y = 0f;
			float cosAB = Vector3.Dot(vecA, vecB);
			if (cosAB <= 0f) {
				IsStopUpdateInfo = true;
				PlayNpcAction();
			}
		}
	}

	void OnDrawGizmosSelected()
	{
		if (!enabled) {
			return;
		}
		
		Transform parTran = transform.parent;
		if (parTran == null) {
			return;
		}
		
		CartoonShootPlayerPath pathScript = parTran.GetComponent<CartoonShootPlayerPath>();
		pathScript.DrawPath();
	}

	public void PlayNpcAction()
	{
		for (int i = 0; i < ActionState.Length; i++) {
			if (AniComponent[i] != null) {
				AniComponent[i].SetBool(ActionState[i], true);
			}
		}
		Invoke("StopNpcAction", DelayTimeCloseAction);
	}

	void StopNpcAction()
	{
		for (int i = 0; i < ActionState.Length; i++) {
			if (AniComponent[i] != null) {
				AniComponent[i].SetBool(ActionState[i], false);
			}
		}
	}
}
