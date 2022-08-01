using UnityEngine;

[System.Serializable]
public class SpPixels
{
	[SerializeField]
	private int width;
	
	[SerializeField]
	private int height;
	
	[SerializeField]
	private Color32[] pixels;
	
	public int Width
	{
		get
		{
			return width;
		}
	}
	
	public int Height
	{
		get
		{
			return height;
		}
	}
	
	public Color32[] Pixels
	{
		get
		{
			return pixels;
		}
	}
	
	public SpPixels()
	{
	}
	
	public SpPixels(Texture texture) : this((Texture2D)texture)
	{
	}
	
	public SpPixels(Texture2D texture)
	{
		if (texture == null) throw new System.ArgumentNullException();
		
		width  = texture.width;
		height = texture.height;
		pixels = texture.GetPixels32();
	}
	
	public SpPixels(int newWidth, int newHeight)
	{
		if (newWidth  < 0) throw new System.ArgumentOutOfRangeException();
		if (newHeight < 0) throw new System.ArgumentOutOfRangeException();
		
		width  = newWidth;
		height = newHeight;
		pixels = new Color32[newWidth * newHeight];
	}
	
	public void Fill(Color32 colour)
	{
		for (var i = pixels.Length - 1; i >= 0; i--)
		{
			pixels[i] = colour;
		}
	}
	
	public void FillRGB(Color32 colour)
	{
		for (var i = pixels.Length - 1; i >= 0; i--)
		{
			var c = pixels[i];
			
			c.r = colour.r;
			c.g = colour.g;
			c.b = colour.b;
			
			pixels[i] = c;
		}
	}
	
	public Color32 GetPixel(int x, int y)
	{
		return pixels[x + width * y];
	}
	
	public Color32 GetPixelTransparent(int x, int y)
	{
		if (x < 0 || y < 0 || x >= width || y >= height) return new Color32(0, 0, 0, 0);
		
		return pixels[x + width * y];
	}
	
	public Color32 GetPixelClamp(int x, int y)
	{
		if (x < 0) x = 0; else if (x >= width ) x = width  -1;
		if (y < 0) y = 0; else if (y >= height) y = height -1;
		
		return pixels[x + width * y];
	}
	
	public Color32 GetPixelRepeat(int x, int y)
	{
		x = x >= 0 ? x % width  : width  + (x % width );
		y = y >= 0 ? y % height : height + (y % height);
		
		return pixels[x + width * y];
	}
	
	public SpPixels GetSubset(Rect rect)
	{
		return GetSubset((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
	}
	
	public SpPixels GetSubset(int x, int y, int w, int h)
	{
		if (w < 0)
		{
			w = -w;
			x -= w;
		}
		
		if (h < 0)
		{
			h = -h;
			y -= h;
		}
		
		var o = new SpPixels(w, h);
		
		for (var oy = 0; oy < h; oy++)
		{
			for (var ox = 0; ox < w; ox++)
			{
				o.SetPixel(ox, oy, GetPixel(ox + x, oy + y));
			}
		}
		
		return o;
	}
	
	public void SetPixel(int x, int y, Color32 colour)
	{
		pixels[x + width * y] = colour;
	}
	
	public void SetPixelClamp(int x, int y, Color32 colour)
	{
		if (x < 0) x = 0; else if (x >= width ) x = width  -1;
		if (y < 0) y = 0; else if (y >= height) y = height -1;
		
		pixels[x + width * y] = colour;
	}
	
	public void SetPixels(int x, int y, SpPixels s)
	{
		for (var sy = 0; sy < s.height; sy++)
		{
			for (var sx = 0; sx < s.width; sx++)
			{
				SetPixel(x + sx, y + sy, s.GetPixel(sx, sy));
			}
		}
	}
	
	public void SetPixelsClamp(int x, int y, SpPixels s)
	{
		for (var sy = 0; sy < s.height; sy++)
		{
			for (var sx = 0; sx < s.width; sx++)
			{
				SetPixelClamp(x + sx, y + sy, s.GetPixelClamp(sx, sy));
			}
		}
	}
	
	public SpPixels GetRotated90()
	{
		var o = new SpPixels(height, width);
		
		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				var c = GetPixel(x, y);
				
				o.SetPixel(y, width - x - 1, c);
			}
		}
		
		return o;
	}
	
	public SpPixels GetRotated270()
	{
		var o = new SpPixels(height, width);
		
		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				o.SetPixel(height - y - 1, x, GetPixel(x, y));
			}
		}
		
		return o;
	}
	
	public SpPixels GetFlippedHorizontally()
	{
		var o = new SpPixels(width, height);
		
		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				o.SetPixel(width - x - 1, y, GetPixel(x, y));
			}
		}
		
		return o;
	}
	
	public SpPixels GetFlippedVertically()
	{
		var o = new SpPixels(width, height);
		
		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				o.SetPixel(x, height - y - 1, GetPixel(x, y));
			}
		}
		
		return o;
	}
	
	public SpPixels GetTrimmed(ref Rect trimmedRect, ref Vector4 border)
	{
		var xMin = 0;
		var xMax = width;
		var yMin = 0;
		var yMax = height;
		
		while (xMin < xMax) { for (var y = yMin; y < yMax; y++) { if (GetPixel(xMin    , y       ).a > 0) goto Exit1; } xMin++; if (border.x > 0) border.x -= 1; } Exit1:
		while (xMax > xMin) { for (var y = yMin; y < yMax; y++) { if (GetPixel(xMax - 1, y       ).a > 0) goto Exit2; } xMax--; if (border.z > 0) border.z -= 1; } Exit2:
		while (yMin < yMax) { for (var x = xMin; x < xMax; x++) { if (GetPixel(x       , yMin    ).a > 0) goto Exit3; } yMin++; if (border.y > 0) border.y -= 1; } Exit3:
		while (yMax > yMin) { for (var x = xMin; x < xMax; x++) { if (GetPixel(x       , yMax - 1).a > 0) goto Exit4; } yMax--; if (border.w > 0) border.w -= 1; } Exit4:
		
		trimmedRect.xMin = xMin;
		trimmedRect.yMin = yMin;
		trimmedRect.xMax = xMax;
		trimmedRect.yMax = yMax;
		
		return GetSubset(xMin, yMin, xMax - xMin, yMax - yMin);
	}
	
	public Texture2D Apply()
	{
		var texture = new Texture2D(width, height);
		
		texture.SetPixels32(pixels);
		texture.Apply();
		
		return texture;
	}
}