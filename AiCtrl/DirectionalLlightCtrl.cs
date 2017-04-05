using UnityEngine;
using System.Collections;

public class DirectionalLlightCtrl : MonoBehaviour {

	[Range(0f, 8f)] public float LightIntensityValMin = 0.2f;
	public static float MinIntensityVal = 0.2f;
	bool IsInitChangeIt;
	Light DirLight;
	
	float StartIntensityVal;
	float EndIntensityVal;
	float DeltaItval = 0.001f;
	static DirectionalLlightCtrl _Instance;
	static public DirectionalLlightCtrl GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;
		DirLight = GetComponent<Light>();
		StartIntensityVal = DirLight.intensity;
		MinIntensityVal = LightIntensityValMin;
		if (MinIntensityVal >= StartIntensityVal) {
			Debug.LogError("DirectionalLlightCtrl -> LightIntensityValMin was wrong");
			DirLight = null;
			DirLight.gameObject.name = "null";
		}
		//Invoke("TestDelayInitChangeIntensity", 3f); //test
	}

	void TestDelayInitChangeIntensity()
	{
		InitChangeIntensity(0.2f, 3f); //test
	}

	public void ResetIntensity(float timeVal)
	{
		InitChangeIntensity(StartIntensityVal, timeVal);
	}

	public void InitChangeIntensity(float val, float timeVal)
	{
		//Debug.Log("InitChangeIntensity***********val " + val);
		if (val == DirLight.intensity) {
			return;
		}

		if (IsInitChangeIt) {
			return;
		}
		IsInitChangeIt = true;
		EndIntensityVal = val;

		float dis = Mathf.Abs(DirLight.intensity - val);
		if (timeVal <= 0f) {
			timeVal = 3f;
		}
		DeltaItval = ( dis * 0.03f ) / timeVal;
		//Debug.Log("DeltaItval = " + DeltaItval + ", time " + Time.realtimeSinceStartup);

		StopCoroutine(ChangeIntensity());
		StartCoroutine(ChangeIntensity());
	}
	
	IEnumerator ChangeIntensity()
	{
		if (!IsInitChangeIt) {
			yield break;
		}

		float dis = Mathf.Abs( DirLight.intensity - EndIntensityVal );
		while (dis > DeltaItval) {
			
			if (EndIntensityVal > DirLight.intensity) {
				DirLight.intensity += DeltaItval;
			}
			else if (EndIntensityVal < DirLight.intensity) {
				DirLight.intensity -= DeltaItval;
			}
			dis = Mathf.Abs( DirLight.intensity - EndIntensityVal );
			yield return new WaitForSeconds(0.03f);
		}
		DirLight.intensity = EndIntensityVal;
		StopChangeIntensity();
	}

	void StopChangeIntensity()
	{
		if (!IsInitChangeIt) {
			return;
		}
		StopCoroutine(ChangeIntensity());
		IsInitChangeIt = false;
		//Debug.Log("time " + Time.realtimeSinceStartup);
	}
}
