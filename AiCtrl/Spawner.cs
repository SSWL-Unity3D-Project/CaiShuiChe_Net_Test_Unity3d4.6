using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
	public static Spawner SpawnerScript;

	Hashtable activeCachedObjects;

	public ObjectCache []ObjectCaches;
	
	void Awake()
	{
		// Set the global variable
		SpawnerScript = this;
		if (GlobalData.GetInstance().gameMode != GameMode.SoloMode) {
			return;
		}
		
		// Total number of cached objects
		int amount = 0;
		
		// Loop through the caches
		for (int i = 0; i < ObjectCaches.Length; i++)
		{
			// Initialize each cache
			ObjectCaches[i].Initialize ();
			
			// Count
			amount += ObjectCaches[i].cacheSize;
		}
		
		// Create a hashtable with the capacity set to the amount of cached objects specified
		activeCachedObjects = new Hashtable (amount);
	}

	public void SpawnNetObj()
	{
//		if (activeCachedObjects != null) {
//			return;
//		}

		// Total number of cached objects
		int amount = 0;
		
		// Loop through the caches
		for (int i = 0; i < ObjectCaches.Length; i++)
		{
			// Initialize each cache
			ObjectCaches[i].Initialize ();
			
			// Count
			amount += ObjectCaches[i].cacheSize;
		}
		
		// Create a hashtable with the capacity set to the amount of cached objects specified
		activeCachedObjects = new Hashtable (amount);
	}

	public static GameObject Spawn(GameObject prefab, Vector3 position, Vector3 forwardVal)
	{
		ObjectCache cache = null;
		
		// Find the cache for the specified prefab
		if(SpawnerScript)
		{
			for(var i = 0; i < SpawnerScript.ObjectCaches.Length; i++)
			{
				if(SpawnerScript.ObjectCaches[i].prefab == prefab)
				{
					cache = SpawnerScript.ObjectCaches[i];
				}
			}
		}
		
		// If there's no cache for this prefab type, just instantiate normally
		if(cache == null)
		{
			float coneAngle = 1.5f;
			Quaternion coneRandomRotation = Quaternion.Euler(Random.Range (-coneAngle, coneAngle), Random.Range (-coneAngle, coneAngle), 0);
			return (GameObject)Instantiate(prefab, position, coneRandomRotation);
		}
		
		// Find the next object in the cache
		GameObject obj = cache.GetNextObjectInCache();
		
		// Set the position and rotation of the object
		obj.transform.position = position;
		obj.transform.forward = forwardVal;
		
		// Set the object to be active
		obj.SetActive (true);
		SpawnerScript.activeCachedObjects[obj] = true;
		return obj;
	}

	public static void HiddenCacheObj(GameObject prefab)
	{
		if (prefab == null) {
			return;
		}
		ObjectCache cache = null;
		
		// Find the cache for the specified prefab
		if(SpawnerScript)
		{
			for(var i = 0; i < SpawnerScript.ObjectCaches.Length; i++)
			{
				if(SpawnerScript.ObjectCaches[i].prefab == prefab)
				{
					cache = SpawnerScript.ObjectCaches[i];
					break;
				}
			}
		}

		if (cache != null) {
			cache.HiddenAllObj();
		}
	}
	
	public static void DestroyObj (GameObject objectToDestroy)
	{
		if(SpawnerScript && SpawnerScript.activeCachedObjects.ContainsKey(objectToDestroy))
		{
			objectToDestroy.SetActive (false);
			SpawnerScript.activeCachedObjects[objectToDestroy] = false;
		}
		else
		{
			Destroy(objectToDestroy);
		}
	}
}

