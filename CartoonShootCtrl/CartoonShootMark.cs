using UnityEngine;
using System.Collections;

public class CartoonShootMark : MonoBehaviour {

	[Range(0.1f, 1000f)] public float MoveSpeed = 1f;
	[Range(0f, 1000f)] public float LookTime = 1f;

	void OnDrawGizmosSelected()
	{
		if (!enabled) {
			return;
		}
		
		Transform parTran = transform.parent;
		if (parTran == null) {
			return;
		}
		
		CartoonShootPathCtrl pathScript = parTran.GetComponent<CartoonShootPathCtrl>();
		pathScript.DrawPath();
	}
}
