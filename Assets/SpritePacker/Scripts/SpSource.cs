using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

[System.Serializable]
public class SpSource
{
	public SpFlag Flag = SpFlag.JustCreated;
	
	public string Identifier; // Texture/Sprite GUID
	
	public bool Dirty = true;
	
	public bool Trim;
	
	public int PadSize;
	
	public SpPadStyle PadStyle;
	
	public bool UseCustomPivot;
	
	public Vector2 CustomPivot = new Vector2(0.5f, 0.5f);
	
	public bool UseCustomBorder;
	
	public Vector4 CustomBorder;
	
#if UNITY_EDITOR
	public string Path
	{
		get
		{
			return AssetDatabase.GUIDToAssetPath(Identifier);
		}
	}
	
	public string Name
	{
		get
		{
			var texture = Texture;
			
			if (texture != null)
			{
				return texture.name;
			}
			
			return "(Missing)";
		}
	}
	
	public Texture2D Texture
	{
		get
		{
			var path = Path;
			
			if (string.IsNullOrEmpty(path) == false)
			{
				return AssetDatabase.LoadMainAssetAtPath(path) as Texture2D;
			}
			
			return null;
		}
	}

	public TextureImporter Importer
	{
		get
		{
			var path = Path;
			
			if (string.IsNullOrEmpty(path) == false)
			{
				return AssetImporter.GetAtPath(path) as TextureImporter;
			}
			
			return null;
		}
	}
	
	public List<Sprite> Sprites
	{
		get
		{
			var sprites = new List<Sprite>();
			var path    = Path;
			
			if (string.IsNullOrEmpty(path) == false)
			{
				foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(path))
				{
					var sprite = asset as Sprite;
					
					if (sprite != null)
					{
						sprites.Add(sprite);
					}
				}
			}
			
			return sprites;
		}
	}
	
	public List<Sprite> FindSprites(List<Sprite> atlasSprites, List<Sprite> sourceSprites)
	{
		var foundSprites = new List<Sprite>();
		
		for (var i = 0; i < sourceSprites.Count; i++)
		{
			foundSprites.Add(atlasSprites.Find(s => s.name == sourceSprites[i].name));
		}
		
		return foundSprites;
	}
#endif
}