using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSwitcher : MonoBehaviour
{
	public float Interval = 0.25f;
	
	public int Index;
	
	public float Timer;
	
	public List<Sprite> SpriteFrames = new List<Sprite>();
	
	private SpriteRenderer spriteRenderer;
	
	protected virtual void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	protected virtual void Update()
	{
		Timer -= Time.deltaTime;
		
		if (Timer <= 0.0f)
		{
			Timer = Interval;
			Index = (Index + 1) % SpriteFrames.Count;
			
			if (SpriteFrames.Count > 0)
			{
				spriteRenderer.sprite = SpriteFrames[Index];
			}
			else
			{
				spriteRenderer.sprite = null;
			}
		}
	}
}