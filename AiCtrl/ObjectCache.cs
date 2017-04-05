using UnityEngine;
using System.Collections;

public class ObjectCache : MonoBehaviour
{
	public GameObject prefab;
	public int cacheSize;
	
	private GameObject [] objects;
	private int cacheIndex;
	bool IsHiddenAllObj;

	public void Initialize()
	{
		cacheSize = (int)(PlayerAutoFire.frequency * PlayerSimpleBullet.lifeTime) + 2;
		//Debug.Log("**************cacheSize " + cacheSize);
		objects = new GameObject[cacheSize];
		
		// Instantiate the objects in the array and set them to be inactive
		for (int i = 0; i < cacheSize; i++)
		{
			if (GlobalData.GetInstance().gameMode == GameMode.SoloMode) {
				
				objects[i] = (GameObject)Instantiate(prefab);
			}
			else {
				
				objects[i] = (GameObject)Network.Instantiate(prefab, GlobalData.HiddenPosition,
				                                             transform.rotation, GlobalData.NetWorkGroup);
			}

			//objects[i].SetActive (false);
			objects[i].name = objects[i].name + i;
		}
	}
	
	public GameObject GetNextObjectInCache()
	{
		GameObject obj = null;
		
		// The cacheIndex starts out at the position of the object created
		// the longest time ago, so that one is usually free,
		// but in case not, loop through the cache until we find a free one.
		for(int i = 0; i < cacheSize; i++)
		{
			if (objects == null)
			{
				return null;
			}

			obj = objects[cacheIndex];
			
			// If we found an inactive object in the cache, use that.
			if(!obj.activeSelf)
			{
				break;
			}
			
			// If not, increment index and make it loop around
			// if it exceeds the size of the cache
			cacheIndex = (cacheIndex + 1) % cacheSize;
		}
		
		// The object should be inactive. If it's not, log a warning and use
		// the object created the longest ago even though it's still active.
		if (obj.activeSelf) {
			Debug.LogWarning (
				"Spawn of " + prefab.name +
				" exceeds cache size of " + cacheSize +
				"! Reusing already active object.", obj);
			Spawner.DestroyObj(obj);
		}
		
		// Increment index and make it loop around
		// if it exceeds the size of the cache
		cacheIndex = (cacheIndex + 1) % cacheSize;
		
		return obj;
	}

	public void HiddenAllObj()
	{
		if (IsHiddenAllObj){
			return;
		}
		IsHiddenAllObj = true;

		for(int i = 0; i < cacheSize; i++)
		{
			if (objects == null)
			{
				return;
			}

			if (objects[i] != null)
			{
				objects[i].SetActive(false);
			}
		}
	}
}

