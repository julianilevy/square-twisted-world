using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SpUpdate_Postprocessor : AssetPostprocessor
{
	private static List<SpAtlas> atlases = new List<SpAtlas>();
	
	private static bool skip;
	
	private static bool busy;
	
	public void OnPreprocessTexture()
	{
		var texture = AssetDatabase.LoadMainAssetAtPath(assetPath) as Texture2D;
		
		// Skip the dirty check if this is a newly imported texture, else Unity will crash
		skip = texture == null;
	}
	
	public void OnPostprocessTexture(Texture2D texture)
	{
		if (busy == false && skip == false)
		{
			var identifier = AssetDatabase.AssetPathToGUID(assetPath);
			var guids      = AssetDatabase.FindAssets("t:scriptableobject");
			
			foreach (var guid in guids)
			{
				var path  = AssetDatabase.GUIDToAssetPath(guid);
				var atlas = AssetDatabase.LoadMainAssetAtPath(path) as SpAtlas;
				
				if (atlas != null)
				{
					var source = atlas.Sources.Find(s => s.Identifier == identifier);
					
					if (source != null)
					{
						source.Dirty = true;
						
						if (atlas.AutoUpdate == true && atlases.Contains(atlas) == false)
						{
							atlases.Add(atlas);
							
							EditorApplication.delayCall -= UpdateAtlases;
							EditorApplication.delayCall += UpdateAtlases;
						}
					}
				}
			}
		}
	}
	
	private static void UpdateAtlases()
	{
		while (atlases.Count > 0)
		{
			var index = atlases.Count - 1;
			var atlas = atlases[index]; atlases.RemoveAt(index);
			
			if (atlas != null && atlas.AutoUpdate == true)
			{
				busy = true;
				{
					atlas.Update();
				}
				busy = false;
			}
		}
	}
}