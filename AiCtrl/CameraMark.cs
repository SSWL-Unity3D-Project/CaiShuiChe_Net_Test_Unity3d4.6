using UnityEngine;
using System.Collections;

public class CameraMark : MonoBehaviour {
	
	void OnDrawGizmosSelected()
	{
		if (!enabled) {
			return;
		}
		
		Transform parTran = transform.parent;
		if (parTran == null) {
			return;
		}
		
		CameraPathCtrl pathScript = parTran.GetComponent<CameraPathCtrl>();
		pathScript.DrawPath();
	}
}
