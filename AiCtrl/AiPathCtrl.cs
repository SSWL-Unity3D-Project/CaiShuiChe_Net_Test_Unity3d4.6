using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AiPathCtrl : MonoBehaviour
{
	public int PathIndex;
	public int KeyState;
	public Transform mNextPath1 = null;
	public Transform mNextPath2 = null;
	public Transform mNextPath3 = null;
	int NextPathNum;

	int PrePathNum;
	Transform PrePathTran1;
	Transform PrePathTran2;
	Transform PrePathTran3;
	//public static float TestDistanceMarkMax;

	void OnDrawGizmosSelected()
	{
		if(!enabled)
		{
			return;
		}

		Transform [] tranArray = transform.GetComponentsInChildren<Transform>();
		for (int i = 1; i < tranArray.Length; i++) {
			tranArray[i].name = "Mark_" + i;
		}
		
		if(transform.childCount <= 1)
		{
			return;
		}

		List<Transform> nodes = new List<Transform>(transform.GetComponentsInChildren<Transform>()){};
		nodes.Remove(transform);
		iTween.DrawPath(nodes.ToArray(), Color.red);
	}

	public void DrawPath ()
	{
		OnDrawGizmosSelected();
	}

	public void SetPrePathTran(Transform tranVal)
	{
		PrePathNum++;
		switch(PrePathNum)
		{
		case 1:
			PrePathTran1 = tranVal;
			break;

		case 2:
			PrePathTran2 = tranVal;
			break;

		case 3:
			PrePathTran3 = tranVal;
			break;
		}
	}
	
	public Transform GetPrePathTran(int val)
	{
		Transform tranVal = PrePathTran1;
		switch(val)
		{
		case 1:
			tranVal = PrePathTran1;
			break;

		case 2:
			tranVal = PrePathTran2;
			break;

		case 3:
			tranVal = PrePathTran3;
			break;
		}
		return tranVal;
	}
	
	public int GetPrePathNum()
	{
		return PrePathNum;
	}

	public int GetNextPathNum()
	{
		return NextPathNum;
	}

	// Use this for initialization
	void Start()
	{
		AiPathCtrl pathScript;
		if(mNextPath1 != null)
		{
			NextPathNum++;
			pathScript = mNextPath1.GetComponent<AiPathCtrl>();
			pathScript.SetPrePathTran(transform);
		}
		
		if(mNextPath2 != null)
		{
			NextPathNum++;
			pathScript = mNextPath2.GetComponent<AiPathCtrl>();
			pathScript.SetPrePathTran(transform);
		}

		if(mNextPath3 != null)
		{
			NextPathNum++;
			pathScript = mNextPath3.GetComponent<AiPathCtrl>();
			pathScript.SetPrePathTran(transform);
		}

		AiMark markScript;
		int count = transform.childCount;
		for(int i = 0; i < count; i++)
		{
			Transform mark = transform.GetChild(i);
			markScript = mark.GetComponent<AiMark>();
			markScript.setMarkCount( i );
			if(i < (count - 1))
			{
				markScript.mNextMark = transform.GetChild(i + 1);
			}
			else
			{
				if(mNextPath1 != null && mNextPath1.childCount > 0)
				{
					markScript.mNextMark = mNextPath1.GetChild(0);
				}
				else if(mNextPath2 != null && mNextPath2.childCount > 0)
				{
					markScript.mNextMark = mNextPath2.GetChild(0);
				}
				else if(mNextPath3 != null && mNextPath3.childCount > 0)
				{
					markScript.mNextMark = mNextPath3.GetChild(0);
				}
			}
		}
		this.enabled = false;
	}

	public void SetPathIndexInfo(int val)
	{
		PathIndex = val;
	}
}

