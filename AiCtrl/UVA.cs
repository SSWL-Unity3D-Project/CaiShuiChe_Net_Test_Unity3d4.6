using UnityEngine;
using System.Collections;

public class UVA : MonoBehaviour {
	public float speed=0.5f;
	public int Array=0;

	void Update()
	{
		float offset = Time.time *speed;
		//Debug.Log (offset);
		renderer.materials[Array].SetTextureOffset("_MainTex",new Vector2 (offset,0));
	}
}
