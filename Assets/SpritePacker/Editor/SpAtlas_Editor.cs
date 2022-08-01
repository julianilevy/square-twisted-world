using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DoCreateAtlas : UnityEditor.ProjectWindowCallback.EndNameEditAction
{
	public override void Action(int instanceId, string pathName, string resourceFile)
	{
		var atlas = ScriptableObject.CreateInstance<SpAtlas>();

		atlas.name = System.IO.Path.GetFileName(pathName);

		AssetDatabase.CreateAsset(atlas, pathName);

		ProjectWindowUtil.ShowCreatedAsset(atlas);
	}
}

[CustomEditor(typeof(SpAtlas))]
public class SpAtlas_Editor : Editor
{
	private SpSource currentSource;
	
	private bool hasGreen;
	
	private bool hasYellow;
	
	private bool hasRed;

	private float lastPpu;
	
	[MenuItem("Assets/Create/SpAtlas (Sprite Atlas)")]
	public static void Create()
	{
		ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateAtlas>(), "New Atlas.asset", null as Texture2D, null as string);
	}
	
	public override bool HasPreviewGUI ()
	{
		return true;
	}
	
	public override void OnPreviewGUI(Rect rect, GUIStyle background)
	{
		var atlas = (SpAtlas)target;
		
		if (currentSource != null)
		{
			var path    = AssetDatabase.GUIDToAssetPath(currentSource.Identifier);
			var texture = AssetDatabase.LoadMainAssetAtPath(path) as Texture2D;
			
			if (texture != null)
			{
				GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit);
			}
		}
		else if (atlas.Texture != null)
		{
			GUI.DrawTexture(rect, atlas.Texture, ScaleMode.ScaleToFit);
		}
	}
	
	public override void OnInspectorGUI()
	{
		var atlas = (SpAtlas)target;
		
		DrawSettings(atlas);
		
		EditorGUILayout.Separator();
		
		DrawDefaults(atlas);

		lastPpu = -1;
		
		if (atlas.Sources.Count > 0)
		{
			EditorGUILayout.Separator();
			
			DrawSources(atlas);
		}
		
		EditorGUILayout.Separator();
		
		DrawAtlas(atlas);
		
		EditorGUILayout.Separator();
		
		DrawDragAndDrop(atlas);
	}
	
	private void DrawSettings(SpAtlas atlas)
	{
		EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
		
		atlas.AutoUpdate  = EditorGUILayout.Toggle("Auto Update", atlas.AutoUpdate);
		atlas.ForceSquare = EditorGUILayout.Toggle("Force Square", atlas.ForceSquare);
	}
	
	private void DrawDefaults(SpAtlas atlas)
	{
		EditorGUILayout.LabelField("Defaults", EditorStyles.boldLabel);
		
		atlas.DefaultTrim     = EditorGUILayout.Toggle("Trim", atlas.DefaultTrim);
		atlas.DefaultPadSize  = EditorGUILayout.IntField("Pad Size", atlas.DefaultPadSize);
		atlas.DefaultPadStyle = (SpPadStyle)EditorGUILayout.EnumPopup("Pad Style", atlas.DefaultPadStyle);
	}
	
	private void DrawDragAndDrop(SpAtlas atlas)
	{
		var dropRect = SpHelper.Reserve(48.0f);
		
		// Open drag and drop window?
		if (GUI.Button(dropRect, "Add Textures\n(Drag And Drop)") == true)
		{
			var drop = SpDrop_Window.Open();
			
			drop.CurrentAtlas = atlas;
			drop.Repaint();
		}
		
		SpHelper.DrawDragAndDropZone(dropRect);
		
		SpHelper.HandleDradAndDrop(atlas, dropRect);
	}
	
	private void DrawAtlas(SpAtlas atlas)
	{
		EditorGUILayout.LabelField("Atlas", EditorStyles.boldLabel);
		
		var rect    = SpHelper.Reserve(14.0f);
		var width   = rect.width / 3.0f;
		var rect0   = rect; rect0.width = width; rect0.x = rect.x + width * 0; rect0.xMax -= 2;
		var rect1   = rect; rect1.width = width; rect1.x = rect.x + width * 1; rect1.xMin += 1; rect1.xMax -= 1;
		var rect2   = rect; rect2.width = width; rect2.x = rect.x + width * 2; rect2.xMin += 2;
		
		if (GUI.Button(rect0, "Update", EditorStyles.miniButton) == true)
		{
			atlas.Update();
		}
		
		EditorGUI.BeginDisabledGroup(atlas.Texture == null);
		{
			if (GUI.Button(rect1, "Select", EditorStyles.miniButton) == true)
			{
				Selection.activeObject = atlas.Texture;
			}
			
			if (GUI.Button(rect2, "Link", EditorStyles.miniButton) == true)
			{
				SpLink_Wizard.Open(atlas);
			}
		}
		EditorGUI.EndDisabledGroup();
	}
	
	private void DrawSources(SpAtlas atlas)
	{
		hasGreen  = false;
		hasYellow = false;
		hasRed    = false;
		
		EditorGUILayout.LabelField("Textures", EditorStyles.boldLabel);
		
		atlas.Sources.RemoveAll(s => s.Flag == SpFlag.JustCreated && s.Texture == null);
		
		for (var i = 0; i < atlas.Sources.Count; i++)
		{
			DrawSource(atlas, atlas.Sources[i]);
		}
		
		if (hasGreen == true)
		{
			EditorGUILayout.HelpBox("A green box means the texture has just been added to the atlas. Press 'Update' to add these textures to the atlas.", MessageType.Info);
		}
		
		if (hasYellow == true)
		{
			EditorGUILayout.HelpBox("A yellow box means the texture has been modified. Press 'Update' to update these textures in the atlas.", MessageType.Info);
		}
		
		if (hasRed == true)
		{
			EditorGUILayout.HelpBox("A red box mean the texture has been marked for deletion. Press 'Update' to remove these textures from the atlas.", MessageType.Info);
		}
	}
	
	private void DrawSource(SpAtlas atlas, SpSource source)
	{
		var background    = default(GUIStyle);
		var sourceTexture = source.Texture;
		
		if (source.Flag == SpFlag.None && sourceTexture == null)
		{
			source.Flag = SpFlag.MarkedForDestruction;
		}
		
		if (source.Flag == SpFlag.JustCreated)
		{
			background = SpHelper.GreenBox;
			hasGreen   = true;
		}
		else if (source.Flag == SpFlag.MarkedForDestruction)
		{
			background = SpHelper.RedBox;
			hasRed     = true;
		}
		else
		{
			if (source.Dirty == true)
			{
				background = SpHelper.YellowBox;
				hasYellow  = true;
			}
		}
		
		SpHelper.BeginError(background != null, background);
		{
			var rect  = SpHelper.Reserve();
			var rect0 = rect; rect0.xMax -= 100.0f;
			var rect1 = rect; rect1.xMin = rect1.xMax - 45.0f; rect1.x -= 55.0f;
			var rect2 = rect; rect2.xMin = rect2.xMax - 35.0f; rect2.x -= 20.0f;
			var rect3 = rect; rect3.xMin = rect3.xMax - 20.0f;
			
			if (EditorGUI.Foldout(rect0, currentSource == source, source.Name) == true)
			{
				EditorGUI.BeginChangeCheck();
				{
					currentSource = source;
					
					EditorGUI.BeginDisabledGroup(source.Flag == SpFlag.MarkedForDestruction);
					{
						source.PadSize  = EditorGUILayout.IntField("Pad Size", source.PadSize);
						source.PadStyle = (SpPadStyle)EditorGUILayout.EnumPopup("Pad Style", source.PadStyle);
						
						if (source.PadStyle == SpPadStyle.Transparent)
						{
							source.Trim = EditorGUILayout.Toggle("Trim", source.Trim);
						}
						
						DrawPivot(source);
						DrawBorder(source);
					}
					EditorGUI.EndDisabledGroup();
				}
				if (EditorGUI.EndChangeCheck() == true)
				{
					source.Dirty = true;
				}
			}
			else if (currentSource == source)
			{
				currentSource = null;
			}

			// See if the PPU is inconsistent
			var importer = source.Importer;

			if (importer != null)
			{
				var ppu = importer.spritePixelsPerUnit;

				if (lastPpu < 0.0f)
				{
					lastPpu = ppu;
				}

				if (lastPpu != ppu)
				{
					EditorGUILayout.HelpBox("This sprite has an inconsistent PixelsPerUnit setting. These differences will be overridden in the final sprite.", MessageType.Warning);
				}
			}
			
			DrawSelectButton(source, rect1, sourceTexture);
			
			DrawFindButton(atlas, source, rect2, sourceTexture);
			
			DrawDestroyButton(atlas, source, rect3);
		}
		SpHelper.EndError();
	}
	
	private void DrawPivot(SpSource source)
	{
		var rect  = SpHelper.Reserve();
		var rect1 = rect; rect1.xMax  = EditorGUIUtility.labelWidth + 32.0f;
		var rect2 = rect; rect2.xMin += EditorGUIUtility.labelWidth + 16.0f;
		
		source.UseCustomPivot = EditorGUI.Toggle(rect1, "Custom Pivot", source.UseCustomPivot);
		
		if (source.UseCustomPivot == true)
		{
			source.CustomPivot = EditorGUI.Vector2Field(rect2, "", source.CustomPivot);
		}
	}
	
	private void DrawBorder(SpSource source)
	{
		var rect  = SpHelper.Reserve();
		var rect1 = rect; rect1.xMax  = EditorGUIUtility.labelWidth + 32.0f;
		var rect2 = rect; rect2.xMin += EditorGUIUtility.labelWidth + 16.0f; rect2.y -= 16.0f;
		
		source.UseCustomBorder = EditorGUI.Toggle(rect1, "Custom Border", source.UseCustomBorder);
		
		if (source.UseCustomBorder == true)
		{
			source.CustomBorder = EditorGUI.Vector4Field(rect2, default(string), source.CustomBorder);
		}
	}
	
	private void DrawSelectButton(SpSource source, Rect rect, Texture2D sourceTexture)
	{
		EditorGUI.BeginDisabledGroup(sourceTexture == null);
		{
			if (GUI.Button(rect, "select", EditorStyles.miniButtonLeft) == true)
			{
				Selection.activeObject = sourceTexture;
			}
		}
		EditorGUI.EndDisabledGroup();
	}
	
	private void DrawFindButton(SpAtlas atlas, SpSource source, Rect rect, Texture2D sourceTexture)
	{
		EditorGUI.BeginDisabledGroup(sourceTexture == null && source.Flag == SpFlag.JustCreated);
		{
			if (GUI.Button(rect, "find", EditorStyles.miniButtonMid) == true)
			{
				var sourceName    = source.Name;
				var sourceSprites = source.Sprites;
				var foundSprites  = source.FindSprites(atlas.Sprites, sourceSprites);
				
				if (foundSprites.Count > 0)
				{
					Selection.objects = foundSprites.ToArray();
				}
				else
				{
					var sprite = atlas.Sprites.Find(s => s.name == sourceName);
					
					if (sprite != null)
					{
						Selection.activeObject = sprite;
					}
					else
					{
						Debug.Log("Failed to find linked sprites");
					}
				}
			}
		}
		EditorGUI.EndDisabledGroup();
	}
	
	private void DrawDestroyButton(SpAtlas atlas, SpSource source, Rect rect)
	{
		if (GUI.Button(rect, "x", EditorStyles.miniButtonRight) == true)
		{
			switch (source.Flag)
			{
				case SpFlag.None:
				{
					source.Flag = SpFlag.MarkedForDestruction;
				}
				break;
				
				case SpFlag.JustCreated:
				{
					atlas.Sources.Remove(source);
				}
				break;
				
				case SpFlag.MarkedForDestruction:
				{
					source.Flag = SpFlag.None;
				}
				break;
			}
		}
	}
}