using UnityEngine;
using System.Collections.Generic;

public class SpMaxRects
{
	private int              w;
	private int              h;
	private int              x;
	private int              y;
	private List<SpMaxRects> children = new List<SpMaxRects>();
	
	private int r
	{
		get
		{
			return x + w;
		}
	}
	
	private int t
	{
		get
		{
			return y + h;
		}
	}
	
	public SpMaxRects(int newWidth, int newHeight)
	{
		w  = newWidth;
		h = newHeight;
		
		children.Add(new SpMaxRects(0, 0, newWidth, newHeight));
	}
	
	private SpMaxRects(int newX, int newY, int newWidth, int newHeight)
	{
		w  = newWidth;
		h = newHeight;
		x = newX;
		y = newY;
	}
	
	public bool TryPack(List<SpRect> rects)
	{
		foreach (var rect in rects)
		{
			if (TryPack(rect) == false)
			{
				return false;
			}
		}
		
		return true;
	}
	
	private bool TryPack(SpRect rect)
	{
		foreach (var child in children)
		{
			var gapX = child.w  - rect.W;
			var gapY = child.h - rect.H;
			
			if (gapX >= 0 && gapY >= 0)
			{
				rect.X = child.x;
				rect.Y = child.y;
				
				children.Remove(child);
				
				if (gapX > 0) children.Add(new SpMaxRects(child.x + rect.W, child.y         , gapX       , child.h));
				if (gapY > 0) children.Add(new SpMaxRects(child.x         , child.y + rect.H, child.w, gapY        ));
				
				Subdivide(rect);
				
				return true;
			}
		}
		
		return false;
	}
	
	private static List<SpMaxRects> newChildrenA = new List<SpMaxRects>();
	private static List<SpMaxRects> newChildrenB = new List<SpMaxRects>();
	
	private void Subdivide(SpRect rect)
	{
		newChildrenA.Clear();
		newChildrenB.Clear();
		
		foreach (var child in children)
		{
			if (rect.X >= child.r || rect.Y >= child.t || rect.R <  child.x || rect.T <  child.y)
			{
				newChildrenA.Add(child); continue;
			}
			
			var gapL = rect.X  - child.x;
			var gapB = rect.Y  - child.y;
			var gapR = child.r - rect.R;
			var gapT = child.t - rect.T;
			
			if (gapL > 0) newChildrenB.Add(new SpMaxRects(child.x       , child.y       , gapL       , child.h));
			if (gapB > 0) newChildrenB.Add(new SpMaxRects(child.x       , child.y       , child.w, gapB        ));
			if (gapR > 0) newChildrenB.Add(new SpMaxRects(child.r - gapR, child.y       , gapR       , child.h));
			if (gapT > 0) newChildrenB.Add(new SpMaxRects(child.x       , child.t - gapT, child.w, gapT        ));
		}
		
		children.Clear();
		children.AddRange(newChildrenA);
		children.AddRange(newChildrenB);
		
		for (var i = children.Count - 1; i >= 0; i--)
		{
			var child = children[i];
			
			foreach (var childB in children)
			{
				if (child != childB)
				{
					if (child.x >= childB.x && child.y >= childB.y && child.r <= childB.r && child.t <= childB.t)
					{
						children.Remove(child); break;
					}
				}
			}
		}
	}
}