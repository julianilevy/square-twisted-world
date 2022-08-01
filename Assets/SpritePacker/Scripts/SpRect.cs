using UnityEngine;

public class SpRect
{
	public string Name;
	
	public SpSource Source;
	
	public SpPixels Pixels;
	
	public Vector4 Border;
	
	public Vector2 Pivot;
	
	public int X;
	
	public int Y;
	
	public int W;
	
	public int H;
	
	public int R
	{
		get
		{
			return X + W;
		}
	}
	
	public int T
	{
		get
		{
			return Y + H;
		}
	}
	
	public Rect Rect
	{
		get
		{
			return new Rect(X + Source.PadSize, Y + Source.PadSize, Pixels.Width, Pixels.Height);
		}
	}
	
	public SpRect GetClone()
	{
		var clone = new SpRect();
		
		clone.Name   = Name;
		clone.Source = Source;
		clone.Pixels = Pixels;
		clone.Border = Border;
		clone.Pivot  = Pivot;
		clone.X      = X;
		clone.Y      = Y;
		clone.W      = W;
		clone.H      = H;
		
		return clone;
	}
	
	public void Trim()
	{
		if (Source.Trim == true && Source.PadStyle == SpPadStyle.Transparent)
		{
			var sourceRect  = new Rect(0.0f, 0.0f, Pixels.Width, Pixels.Height);
			var trimmedRect = default(Rect);
			var pivotX      = Pivot.x * sourceRect.width;
			var pivotY      = Pivot.y * sourceRect.height;
			
			Pixels = Pixels.GetTrimmed(ref trimmedRect, ref Border);
			
			pivotX = SpHelper.Divide(pivotX - trimmedRect.xMin, trimmedRect.width );
			pivotY = SpHelper.Divide(pivotY - trimmedRect.yMin, trimmedRect.height);
			
			Pivot = new Vector2(pivotX, pivotY);
		}
		
		W = Pixels.Width  + Source.PadSize * 2;
		H = Pixels.Height + Source.PadSize * 2;
	}
	
	public void PasteInto(SpPixels atlas)
	{
		atlas.SetPixels(X, Y, ExpandedPixels);
	}
	
	public SpPixels ExpandedPixels
	{
		get
		{
			var s = Source.PadSize;
			var expanded = new SpPixels(W, H);
			
			switch (Source.PadStyle)
			{
				case SpPadStyle.Transparent:
				{
					for (var y = 0; y < H; y++)
					{
						for (var x = 0; x < W; x++)
						{
							expanded.SetPixel(x, y, Pixels.GetPixelTransparent(x - s, y - s));
						}
					}
				}
				break;
				
				case SpPadStyle.Clamp:
				{
					for (var y = 0; y < H; y++)
					{
						for (var x = 0; x < W; x++)
						{
							expanded.SetPixel(x, y, Pixels.GetPixelClamp(x - s, y - s));
						}
					}
				}
				break;
				
				case SpPadStyle.Repeat:
				{
					for (var y = 0; y < H; y++)
					{
						for (var x = 0; x < W; x++)
						{
							expanded.SetPixel(x, y, Pixels.GetPixelRepeat(x - s, y - s));
						}
					}
				}
				break;
			}
			
			return expanded;
		}
	}
}