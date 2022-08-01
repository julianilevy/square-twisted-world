#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class SpPacker
{
	public static bool AutoPack(List<SpSource> sources, bool forceSquare, ref int width, ref int height, ref List<SpRect> rects)
	{
		width  = Mathf.Clamp(width , 2, 8192);
		height = Mathf.Clamp(height, 2, 8192);
		rects  = CompileSources(sources);
		
		var testRects = CloneRects(rects);
		
		if (forceSquare == true)
		{
			width = height;
		}
		
		if (TryPack(rects, width, height) == true)
		{
			AutoPackSmaller(forceSquare, ref testRects, ref rects, ref width, ref height);
		}
		else
		{
			AutoPackLarger(forceSquare, ref testRects, ref rects, ref width, ref height);
		}
		
		if (width > 0 && height > 0)
		{
			return true;
		}
		
		return false;
	}
	
	private static void AutoPackSmaller(bool forceSquare, ref List<SpRect> testRects, ref List<SpRect> rects, ref int width, ref int height)
	{
		var testWidth  = width;
		var testHeight = height;
		
		while (true)
		{
			// Shrink by one step
			if (forceSquare == true)
			{
				testWidth = testHeight = testWidth / 2;
			}
			else
			{
				if (testWidth == testHeight) testHeight /= 2; else testWidth /= 2;
			}
			
			// To small?
			if (testWidth < 2 || testHeight < 2)
			{
				return;
			}
			// Continues to fit?
			else if (TryPack(testRects, testWidth, testHeight) == true)
			{
				Swap(ref rects, ref testRects);
				
				width  = testWidth;
				height = testHeight;
			}
			// Too small?
			else
			{
				return;
			}
		}
	}
	
	private static void AutoPackLarger(bool forceSquare, ref List<SpRect> testRects, ref List<SpRect> rects, ref int width, ref int height)
	{
		var testWidth  = width;
		var testHeight = height;
		
		while (true)
		{
			// Expand by one step
			if (forceSquare == true)
			{
				testWidth = testHeight = testWidth * 2;
			}
			else
			{
				if (testWidth == testHeight) testWidth *= 2; else testHeight *= 2;
			}
			
			// Too big?
			if (testWidth > 8192 || testHeight > 8192)
			{
				width  = -1;
				height = -1;
				
				return;
			}
			// Now fits?
			else if (TryPack(testRects, testWidth, testHeight) == true)
			{
				Swap(ref rects, ref testRects);
				
				width  = testWidth;
				height = testHeight;
				
				return;
			}
		}
	}
	
	private static void Swap(ref List<SpRect> a, ref List<SpRect> b)
	{
		var t = a;
		
		a = b;
		b = t;
	}
	
	private static bool TryPack(List<SpRect> rects, int width, int height)
	{
		return new SpMaxRects(width, height).TryPack(rects);
	}
	
	private static List<SpRect> CompileSources(List<SpSource> sources)
	{
		var rects = new List<SpRect>();
		
		for (var i = sources.Count - 1; i >= 0; i--)
		{
			var source        = sources[i];
			var sourceTexture = source.Texture;
			
			if (sourceTexture != null)
			{
				var sourcePath     = source.Path;
				var sourcePixels   = default(SpPixels);
				var sourceSprites  = source.Sprites;
				var sourceImporter = SpHelper.GetAssetImporter<TextureImporter>(sourcePath);
				
				if (sourceImporter != null)
				{
					// Make the texture temporarily readable, or directly read pixels
					if (sourceImporter.isReadable == false)
					{
						sourceImporter.isReadable = true; SpHelper.ReimportAsset(sourcePath);
						{
							sourcePixels = new SpPixels(sourceTexture);
						}
						sourceImporter.isReadable = false; SpHelper.ReimportAsset(sourcePath);
					}
					else
					{
						sourcePixels = new SpPixels(sourceTexture);
					}
					
					// Add sprites or whole texture
					if (sourceSprites.Count > 0)
					{
						for (var j = 0; j < sourceSprites.Count; j++)
						{
							var sourceSprite = sourceSprites[j];
							
							CompileRect(rects, source, sourcePixels.GetSubset(sourceSprite.rect), sourceSprite.name, sourceSprite);
						}
					}
					else
					{
						CompileRect(rects, source, sourcePixels, sourceTexture.name);
					}
					
					continue;
				}
			}
			
			sources.RemoveAt(i);
		}
		
		rects.Sort((a, b) => Mathf.Max(b.W, b.H) - Mathf.Max(a.W, a.H));
		
		return rects;
	}
	
	private static void CompileRect(List<SpRect> rects, SpSource source, SpPixels pixels, string name, Sprite sprite = null)
	{
		var newRect = new SpRect();
		
		newRect.Name   = name;
		newRect.Source = source;
		newRect.Pixels = pixels;
		
		// Read pivot and border from sprite
		if (sprite != null)
		{
			newRect.Pivot  = SpHelper.GetSpritePivot(sprite);
			newRect.Border = sprite.border;
		}
		// Use default pivot and border settings
		else
		{
			newRect.Pivot  = new Vector2(0.5f, 0.5f);
			newRect.Border = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
		}
		
		// Override the pivot?
		if (source.UseCustomPivot == true)
		{
			newRect.Pivot = source.CustomPivot;
		}
		
		// Override the border?
		if (source.UseCustomBorder == true)
		{
			newRect.Border = source.CustomBorder;
		}
		
		newRect.Trim();
		
		rects.Add(newRect);
	}
	
	private static List<SpRect> CloneRects(List<SpRect> rects)
	{
		var newRects = new List<SpRect>();
		
		for (var i = 0; i < rects.Count; i++)
		{
			newRects.Add(rects[i].GetClone());
		}
		
		return newRects;
	}
}
#endif