using UnityEngine;
using System.Collections;

public class HeatDistort : MonoBehaviour
{
	[Range(0f, 128f)] public float DistortionMax = 30f;
	[Range(0.1f, 3f)] public float SubTimeVal = 1f;
	float SubDistortion = 1f;
	float Distortion = 10f;

	MeshRenderer MeshRenderObj;
	Material material;
	GameObject GameObj;

	static HeatDistort _Instance;
	public static HeatDistort GetInstance()
	{
		return _Instance;
	}

	void Awake()
	{
		GameObj = gameObject;
		_Instance = this;

		MeshRenderObj = GetComponent<MeshRenderer>();
		material = MeshRenderObj.materials[0];
		GameObj.SetActive(false);
	}

	public void InitPlayScreenWater()
	{
		if (GameObj.activeSelf) {
			return;
		}

		Distortion = DistortionMax;
		SubDistortion = (0.03f * DistortionMax) / SubTimeVal;
		if (SubDistortion < 0.00001f) {
			SubDistortion = 0.00001f;
		}
		material.SetFloat("_BumpAmt", Distortion);

		GameObj.SetActive(true);
		StartCoroutine(UpdateDistortionVal());
	}

	IEnumerator UpdateDistortionVal()
	{
		while (Distortion > 0f) {

			yield return new WaitForSeconds(0.03f);

			Distortion -= SubDistortion;
			if (Distortion < 0f) {
				Distortion = 0f;
			}
			material.SetFloat("_BumpAmt", Distortion);
		}
		GameObj.SetActive(false);
	}
}