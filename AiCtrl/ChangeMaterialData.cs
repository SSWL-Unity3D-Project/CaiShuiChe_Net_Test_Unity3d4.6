using UnityEngine;
using System.Collections;

public class ChangeMaterialData : MonoBehaviour {

	public SkinnedMeshRenderer SkinnedMashRenderData;
	public MeshRenderer MashRenderData;
	public Material OldMaterial;
	public Material NewMaterial;

	public void ChangeToNewMaterial()
	{
		if(SkinnedMashRenderData != null)
		{
			SkinnedMashRenderData.material = NewMaterial;
		}
		else if(MashRenderData != null)
		{
			MashRenderData.material = NewMaterial;
		}
	}

	public void ChangeToOldMaterial()
	{
		if(SkinnedMashRenderData != null)
		{
			SkinnedMashRenderData.material = OldMaterial;
		}
		else if(MashRenderData != null)
		{
			MashRenderData.material = OldMaterial;
		}
	}
}