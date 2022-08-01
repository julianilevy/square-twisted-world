using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class SpAtlas : ScriptableObject
{
	public bool Dirty = true;
	
	public bool AutoUpdate;
	
	public bool ForceSquare;
	
	public bool DefaultTrim = true;
	
	public int DefaultPadSize = 1;
	
	public string Identifier;
	
	public int Width;
	
	public int Height;
	
	public SpPadStyle DefaultPadStyle = SpPadStyle.Transparent;
	
	public Texture2D Texture;
	
	public List<SpSource> Sources = new List<SpSource>();
	
	public List<Sprite> Sprites = new List<Sprite>();
	
#if UNITY_EDITOR
	public void Update()
	{
		var newWidth  = Width;
		var newHeight = Height;
		var newRects  = default(List<SpRect>);
		
		// Remove deleted textures
		Sources.RemoveAll(s => s.Flag == SpFlag.MarkedForDestruction);
		
		EditorUtility.DisplayProgressBar("Updating " + name, "Packing...", 0.25f);
		
		// Try to pack
		if (SpPacker.AutoPack(Sources, ForceSquare, ref newWidth, ref newHeight, ref newRects) == true)
		{
			EditorUtility.DisplayProgressBar("Updating " + name, "Pasting...", 0.5f);
			
			var pixels    = new SpPixels(newWidth, newHeight);
			var metaDatas = new SpriteMetaData[newRects.Count];
			
			for (var i = newRects.Count - 1; i >= 0; i--)
			{
				var rect     = newRects[i];
				var metaData = default(SpriteMetaData);
				
				rect.PasteInto(pixels);
				
				metaData.name   = rect.Name;
				metaData.rect   = rect.Rect;
				metaData.pivot  = rect.Pivot;
				metaData.border = rect.Border;
				metaData.alignment = (int)SpriteAlignment.Custom;
				
				metaDatas[i] = metaData;
			}
			
			EditorUtility.SetDirty(this);
			
			EditorUtility.DisplayProgressBar("Updating " + name, "Reimporting...", 0.75f);
			
			UpdateTextureAsset(metaDatas, pixels.Apply());
		}
		else
		{
			Debug.LogError("Failed to pack atlas, because the source textures are too large!");
		}
		
		EditorUtility.ClearProgressBar();
	}
	
	public void TryAddSource(string newPath)
	{
		if (string.IsNullOrEmpty(newPath) == false)
		{
			if (System.IO.Directory.Exists(newPath) == true)
			{
				var directories = System.IO.Directory.GetDirectories(newPath);
				var files       = System.IO.Directory.GetFiles(newPath);
				
				for (var i = 0; i < directories.Length; i++)
				{
					TryAddSource(directories[i]);
				}
				
				for (var i = 0; i < files.Length; i++)
				{
					TryAddSource(files[i]);
				}
			}
			else
			{
				var newTexture2D = AssetDatabase.LoadMainAssetAtPath(newPath) as Texture2D;
				
				if (newTexture2D != null)
				{
					var newIdentifier = AssetDatabase.AssetPathToGUID(newPath);
					
					if (string.IsNullOrEmpty(newIdentifier) == false && newIdentifier != Identifier)
					{
						if (Sources.Find(s => s.Identifier == newIdentifier) == null)
						{
							AddSource(newIdentifier);
						}
					}
				}
			}
		}
	}
	
	private void AddSource(string newIdentifier)
	{
		var newSource = new SpSource();
		
		newSource.Identifier = newIdentifier;
		newSource.Trim       = DefaultTrim;
		newSource.PadSize    = DefaultPadSize;
		newSource.PadStyle   = DefaultPadStyle;
		
		Sources.Add(newSource);
	}
	
	private void UpdateTextureAsset(SpriteMetaData[] metaDatas, Texture2D tempTexture)
	{
		if (tempTexture == null)
		{
			throw new System.Exception("TempTexture is null");
		}

		var path      = default(string);
		var importer  = default(TextureImporter);
		
		// Try and find the path of an existing 
		if (string.IsNullOrEmpty(Identifier) == false)
		{
			path = AssetDatabase.GUIDToAssetPath(Identifier);
		}
		
		// Create asset texture for the first time?
		if (string.IsNullOrEmpty(path) == true)
		{
			path     = AssetDatabase.GetAssetPath(this);
			path     = path.Substring(0, path.Length - ".asset".Length);
			path     = AssetDatabase.GenerateUniqueAssetPath(path + ".png");
			importer = SpHelper.SaveTextureAsset(tempTexture, path, false);
			
			importer.maxTextureSize     = 8192;
			importer.textureCompression = TextureImporterCompression.Uncompressed;
		}
		// Update existing asset texture?
		else
		{
			importer = SpHelper.SaveTextureAsset(tempTexture, path, true);
		}
		
		// Update the atlas settings
		importer.textureType      = TextureImporterType.Sprite;
		importer.spriteImportMode = SpriteImportMode.Multiple;
		importer.spritesheet      = metaDatas;

		EditorUtility.SetDirty(importer);
		
		// Apply new settings
		SpHelper.ReimportAsset(path);
		
		// Update settings
		Texture    = AssetDatabase.LoadMainAssetAtPath(path) as Texture2D;
		Width      = Texture.width;
		Height     = Texture.height;
		Identifier = AssetDatabase.AssetPathToGUID(path);
		
		// Find all packed sprites
		Sprites.Clear();
		
		foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(path))
		{
			var sprite = asset as Sprite;
			
			if (sprite != null)
			{
				Sprites.Add(sprite);
			}
		}
		
		// Destroy temp texture
		DestroyImmediate(tempTexture);
		
		// Unmark dirty
		Dirty = false;
		
		Sources.ForEach(s => { s.Flag = SpFlag.None; s.Dirty = false; });
	}
#endif
}