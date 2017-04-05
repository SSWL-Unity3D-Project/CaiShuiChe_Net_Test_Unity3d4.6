using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CartoonShootPlayerPath : MonoBehaviour {
	
	public bool IsMoveByPathSpeed;
	[Range(0.1f, 1000f)] public float MoveSpeed = 1f;
	public CartoonShootPlayerPath NextPlayerPath;

	public readonly int iTweenEventIndex = -1;

	void OnDrawGizmosSelected()
	{
		if(!enabled)
		{
			return;
		}

		FixMarkName();
		
		if(transform.childCount <= 1)
		{
			return;
		}
		
		List<Transform> nodes = new List<Transform>(transform.GetComponentsInChildren<Transform>()){};
		nodes.Remove(transform);
		iTween.DrawPath(nodes.ToArray(), Color.blue);
	}
	
	public void DrawPath ()
	{
		OnDrawGizmosSelected();
	}

	public void FixMarkName()
	{
		Transform [] tranArray = transform.GetComponentsInChildren<Transform>();
		for (int i = 1; i < tranArray.Length; i++) {
			CartoonShootPlayerMark markScript = tranArray[i].GetComponent<CartoonShootPlayerMark>();
			markScript.MarkIndex = i - 1;
			markScript.ShootPlayerPath = this;
			tranArray[i].name = "Mark_" + i;
		}
	}
}
