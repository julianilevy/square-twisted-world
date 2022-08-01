using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class SpHelper
{
#if UNITY_EDITOR
	private static Texture2D checker;
	
	private static GUIStyle redBox;
	
	private static GUIStyle yellowBox;
	
	private static GUIStyle greenBox;
	
	private static GUIStyle clearBox;
	
	public static Texture2D Checker
	{
		get
		{
			if (checker == null)
			{
				checker = CreateTempTexture(20, 20, "iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAAQElEQVQ4EWNkYGBoAGJiQAMxipiIUUSKmlEDSQkt7GpHwxB7uJAiykiC4gZi1I5GCjGhhF/NaBjiDx9iZKkehgA9MgGnI6/4wwAAAABJRU5ErkJggg==");
			}
			
			return checker;
		}
	}
	
	public static GUIStyle RedBox
	{
		get
		{
			if (redBox == null)
			{
				redBox                   = new GUIStyle();
				redBox.border            = new RectOffset(5, 5, 5, 5);
				redBox.normal            = new GUIStyleState();
				redBox.normal.background = CreateTempTexture(12, 12, "iVBORw0KGgoAAAANSUhEUgAAAAwAAAAMCAYAAABWdVznAAAAgklEQVQoFZWSOw6AIBBEwRi5A/Z6BWwsPLgFjVwBe7yDNjpj0FLZTYbPMm8hgD7VGz1GDmqhJmcP9AlaoJU5nYEJ45GJj/BYm2s0HfRnZh16UoVm4KwwHAFbaKbNEjACwBAQBYFdQBwENgFw31IQAIE7RMgXQPTE56Xp//oaPAULqwvAehfT/RZRSAAAAABJRU5ErkJggg==");
			}
			
			return redBox;
		}
	}
	
	public static GUIStyle YellowBox
	{
		get
		{
			if (yellowBox == null)
			{
				yellowBox                   = new GUIStyle();
				yellowBox.border            = new RectOffset(5, 5, 5, 5);
				yellowBox.normal            = new GUIStyleState();
				yellowBox.normal.background = CreateTempTexture(12, 12, "iVBORw0KGgoAAAANSUhEUgAAAAwAAAAMCAYAAABWdVznAAAAgklEQVQoFZWSOw6AIBBEwRi9g/Z6BWwsPLgFjVwBe7yDNDpjoEV2k+GzzFsIoJ9H5ZgxMNAIdSkZ0QfogE7mdAI2jFcmCmGxtrdoJujPzDr0hAbNwlllGAJDpZm2gUAvAHoCoiBwC4hI4BIA3y05AeC4g4dsBUSPzy9Nf+lr8BQsrF5OMxrQ/YA85gAAAABJRU5ErkJggg==");
			}
			
			return yellowBox;
		}
	}
	
	public static GUIStyle GreenBox
	{
		get
		{
			if (greenBox == null)
			{
				greenBox                   = new GUIStyle();
				greenBox.border            = new RectOffset(5, 5, 5, 5);
				greenBox.normal            = new GUIStyleState();
				greenBox.normal.background = CreateTempTexture(12, 12, "iVBORw0KGgoAAAANSUhEUgAAAAwAAAAMCAYAAABWdVznAAAAgklEQVQoFZWSOw6AIBBEwRi5g/Z6BWwsPLgFjVwBe7yDNDpjoFV2k+GzzFsIoNWtSkwYWGiAupxM6CO0QwdzOgMrxgsTH+GwtrVoRujPzDr0xAbNzFllWAJ9pZm2noARAIaAKAhcAiIROAXAe0teAHjuECBXAdETykvT//U1eAoWVg+9fRfTlCY3lwAAAABJRU5ErkJggg==");
			}
			
			return greenBox;
		}
	}
	
	public static GUIStyle ClearBox
	{
		get
		{
			if (clearBox == null)
			{
				clearBox        = new GUIStyle();
				clearBox.border = new RectOffset(3, 3, 3, 3);
				clearBox.normal = new GUIStyleState();
			}
			
			return clearBox;
		}
	}
	
	public static void DrawDragAndDropZone(Rect rect)
	{
		DrawTiledTexture(new Rect(rect.xMin    , rect.yMin    , rect.width, 2 ), Checker);
		DrawTiledTexture(new Rect(rect.xMin    , rect.yMax - 2, rect.width, 2 ), Checker);
		DrawTiledTexture(new Rect(rect.xMin    , rect.yMin    , 2, rect.height), Checker);
		DrawTiledTexture(new Rect(rect.xMax - 2, rect.yMin    , 2, rect.height), Checker);
	}
	
	public static void DrawTiledTexture(Rect rect, Texture texture)
	{
		var coords = new Rect(0.0f, 0.0f, rect.width / (float)texture.width, rect.height / (float)texture.height);
		
		GUI.DrawTextureWithTexCoords(rect, texture, coords, true);
	}
	
	public static Texture2D CreateTempTexture(int width, int height, string encoded)
	{
		var texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
		
		texture.hideFlags = HideFlags.HideAndDontSave;
		texture.LoadImage(System.Convert.FromBase64String(encoded));
		texture.Apply();
		
		return texture;
	}
	
	public static Rect Reserve(float height = 16.0f)
	{
		var rect = EditorGUILayout.BeginVertical();
		{
			EditorGUILayout.LabelField("", GUILayout.Height(height));
		}
		EditorGUILayout.EndVertical();
		
		return rect;
	}
	
	public static void HandleDradAndDrop(SpAtlas atlas, Rect dropRect)
	{
		if (dropRect.Contains(Event.current.mousePosition) == true)
		{
			switch (Event.current.type)
			{
				case EventType.DragUpdated:
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				}
				break;
				
				case EventType.DragPerform:
				{
					if (Event.current.type == EventType.DragPerform)
					{
						DragAndDrop.AcceptDrag();
						
						for (var i = 0; i < DragAndDrop.objectReferences.Length; i++)
						{
							var objectReference = DragAndDrop.objectReferences[i];
							var objectPath      = AssetDatabase.GetAssetPath(objectReference);
							
							if (string.IsNullOrEmpty(objectPath) == false)
							{
								atlas.TryAddSource(objectPath);
							}
						}
					}
				}
				break;
			}
		}
	}
	
	public static void BeginError(bool error = true)
	{
		BeginError(error, RedBox);
	}
	
	public static void BeginError(bool error, GUIStyle errorStyle)
	{
		EditorGUILayout.BeginVertical(error == true ? errorStyle : ClearBox);
	}
	
	public static void EndError()
	{
		EditorGUILayout.EndVertical();
	}
	
	public static Object GUIDToObject(string guid)
	{
		if (string.IsNullOrEmpty(guid) == false)
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);
			
			if (string.IsNullOrEmpty(path) == false)
			{
				return AssetDatabase.LoadMainAssetAtPath(path);
			}
		}
		
		return null;
	}
	
	public static Vector2 GetSpritePivot(Sprite sprite)
	{
		var pivot = default(Vector2);
		var min   = sprite.bounds.min;
		var size  = sprite.bounds.size;
		
		pivot.x = Divide(-min.x, size.x);
		pivot.y = Divide(-min.y, size.y);
		
		return pivot;
	}
	
	public static TextureImporter SaveTextureAsset(Texture2D texture, string path, bool overwrite = false)
	{
		var bytes = texture.EncodeToPNG();
		var fs    = new System.IO.FileStream(path, overwrite == true ? System.IO.FileMode.Create : System.IO.FileMode.CreateNew);
		var bw    = new System.IO.BinaryWriter(fs);
		
		bw.Write(bytes);
		
		bw.Close();
		fs.Close();
		
		var importer = GetAssetImporter<TextureImporter>(path);
		
		if (importer == null)
		{
			ReimportAsset(path);
			
			importer = GetAssetImporter<TextureImporter>(path);
		}
		
		return importer;
	}
	
	public static T GetAssetImporter<T>(string path)
		where T : AssetImporter
	{
		return (T)AssetImporter.GetAtPath(path);
	}
	
	public static void ReimportAsset(string path)
	{
		AssetDatabase.ImportAsset(path);
	}
#endif

	public static float Divide(float a, float b)
	{
		return b == 0.0f == false ? a / b : 0.0f;
	}
}