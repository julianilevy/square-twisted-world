#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2
	#define OLD_SCENE_MANAGEMENT
#else
	using UnityEditor.SceneManagement;
#endif

using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

public enum SpLink_Scene
{
	None,
	This,
	All
}

public class SpLink_Wizard : ScriptableWizard
{
	private SpAtlas atlas;

	private SpLink_Scene inScenes = SpLink_Scene.All;

	private bool inPrefabs = true;

	private bool inScriptableObjects = true;

	private bool inAnimationClips = true;

	private Sprite[] sourceSprites;

	private Sprite[] packedSprites;

	private Vector2 scrollPosition;

	public static SpLink_Wizard Open(SpAtlas atlas)
	{
		if (atlas != null)
		{
			var path    = AssetDatabase.GUIDToAssetPath(atlas.Identifier);
			var texture = AssetDatabase.LoadMainAssetAtPath(path) as Texture2D;

			if (texture != null)
			{
				var allSourceSprites = new List<Sprite>();
				var allPackedSprites = new List<Sprite>();

				for (var i = 0; i < atlas.Sources.Count; i++)
				{
					var source        = atlas.Sources[i];
					var sourceSprites = source.Sprites;

					if (sourceSprites.Count > 0)
					{
						allSourceSprites.AddRange(sourceSprites);
						allPackedSprites.AddRange(source.FindSprites(atlas.Sprites, sourceSprites));
					}
				}

				if (allSourceSprites.Count > 0)
				{
					var linker = ScriptableWizard.DisplayWizard<SpLink_Wizard>("Sprite Linker", "Link", "Cancel");

					linker.atlas         = atlas;
					linker.sourceSprites = allSourceSprites.ToArray();
					linker.packedSprites = allPackedSprites.ToArray();

					return linker;
				}
			}
		}

		return null;
	}

	public void OnGUI()
	{
		if (atlas != null)
		{
			EditorGUILayout.HelpBox("This tool will replace all your non-atlas sprite references with these atlas sprite references (i.e. your whole project will be searched for the sprites on the left, and be replaced by the sprites on the right)", MessageType.Info);

			EditorGUILayout.Separator();

			inScenes = (SpLink_Scene)EditorGUILayout.EnumPopup("In Scenes", inScenes);

			inPrefabs = EditorGUILayout.Toggle("In Prefabs", inPrefabs);

			inScriptableObjects = EditorGUILayout.Toggle("In Scriptable Objects", inScriptableObjects);

			inAnimationClips = EditorGUILayout.Toggle("In Animation Clips", inAnimationClips);

			EditorGUILayout.Separator();

			EditorGUILayout.LabelField("Replace", EditorStyles.boldLabel);

			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
			{
				for (var i = 0; i < sourceSprites.Length; i++)
				{
					var rect  = SpHelper.Reserve();
					var rect0 = rect; rect0.xMax = rect0.center.x - 20;
					var rect1 = rect; rect1.xMin = rect1.center.x + 20;
					var rect2 = rect; rect2.xMin = rect2.center.x - 20; rect2.xMax = rect2.center.x + 20;

					if (sourceSprites[i] == null)
					{
						GUI.Box(rect0, "", SpHelper.RedBox);
					}

					sourceSprites[i] = (Sprite)EditorGUI.ObjectField(rect0, sourceSprites[i], typeof(Sprite), true);
					packedSprites[i] = (Sprite)EditorGUI.ObjectField(rect1, packedSprites[i], typeof(Sprite), true);

					EditorGUI.LabelField(rect2, "With");
				}
			}
			EditorGUILayout.EndScrollView();

			EditorGUILayout.Separator();

			DrawButtons();
		}
		else
		{
			Close();
		}
	}

	private void DrawButtons()
	{
		var rect  = SpHelper.Reserve();
		var width = rect.width / 3;
		var rect0 = new Rect(rect.xMin + width * 0    , rect.yMin, width - 1, rect.height);
		var rect1 = new Rect(rect.xMin + width * 1 + 1, rect.yMin, width - 2, rect.height);
		var rect2 = new Rect(rect.xMin + width * 2    , rect.yMin, width - 1, rect.height);

		if (GUI.Button(rect0, "Link Sprites") == true)
		{
			if (EditorUtility.DisplayDialog("Are you sure?", "This will search through your whole project and replace all sprites on the LEFT with sprites on the RIGHT. This may be difficult to undo if you make a mistake, so please back up your project first!", "ok") == true)
			{
				switch (inScenes)
				{
					case SpLink_Scene.This:
					{
						LinkInScene();
					}
					break;

					case SpLink_Scene.All:
					{
#if OLD_SCENE_MANAGEMENT
						if (EditorApplication.SaveCurrentSceneIfUserWantsTo() == true)
						{
							var currentScene = EditorApplication.currentScene;
							var sceneGuids   = AssetDatabase.FindAssets("t:scene");

							foreach (var sceneGuid in sceneGuids)
							{
								var scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);

								if (EditorApplication.OpenScene(scenePath) == true)
								{
									LinkInScene();

									EditorApplication.SaveScene();
								}
							}

							EditorApplication.OpenScene(currentScene);
						}
#else
						if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == true)
						{
							var setup      = EditorSceneManager.GetSceneManagerSetup();
							var sceneGuids = AssetDatabase.FindAssets("t:scene");
						
							foreach (var sceneGuid in sceneGuids)
							{
								var scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
								var scene     = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

								if (scene.isLoaded == true)
								{
									LinkInScene();
								
									EditorSceneManager.SaveScene(scene);
								}
							}

							EditorSceneManager.RestoreSceneManagerSetup(setup);
						}
#endif
					}
					break;
				}

				if (inPrefabs == true)
				{
					LinkInPrefabs();
				}

				if (inScriptableObjects == true)
				{
					LinkInScriptableObjects();
				}

				if (inAnimationClips == true)
				{
					LinkInAnimationClips();
				}

#if OLD_SCENE_MANAGEMENT
				EditorApplication.MarkSceneDirty();
#else
				EditorSceneManager.MarkAllScenesDirty();
#endif
			}
		}

		if (GUI.Button(rect1, "Swap Sides") == true)
		{
			if (EditorUtility.DisplayDialog("Are you sure?", "This will swap all sprites on the left and right sides, but not link them. Please confirm the sprites and click Link to complete.", "ok") == true)
			{
				for (var i = 0; i < sourceSprites.Length; i++)
				{
					var tempSprite = sourceSprites[i];

					sourceSprites[i] = packedSprites[i];
					packedSprites[i] = tempSprite;
				}
			}
		}

		if (GUI.Button(rect2, "Cancel") == true)
		{
			Close();
		}
	}

	private void LinkInScene()
	{
		var gameObjects = Object.FindObjectsOfType<GameObject>();

		foreach (var gameObject in gameObjects)
		{
			var transform = gameObject.transform;

			if (transform.parent == null)
			{
				LinkInTransform(transform);
			}
		}
	}

	private void LinkInPrefabs()
	{
		var guids = AssetDatabase.FindAssets("t:prefab");

		foreach (var guid in guids)
		{
			var path   = AssetDatabase.GUIDToAssetPath(guid);
			var prefab = AssetDatabase.LoadMainAssetAtPath(path) as GameObject;

			if (prefab != null)
			{
				LinkInTransform(prefab.transform);
			}
		}
	}

	private void LinkInScriptableObjects()
	{
		var guids = AssetDatabase.FindAssets("t:scriptableobject");

		foreach (var guid in guids)
		{
			var path             = AssetDatabase.GUIDToAssetPath(guid);
			var scriptableObject = AssetDatabase.LoadMainAssetAtPath(path) as ScriptableObject;

			if (scriptableObject != null)
			{
				var o = (object)scriptableObject;

				if (LinkInObject(ref o, 0) == true)
				{
					EditorUtility.SetDirty(scriptableObject);
				}
			}
		}
	}

	private void LinkInAnimationClips()
	{
		var guids = AssetDatabase.FindAssets("t:animationclip");

		foreach (var guid in guids)
		{
			var path          = AssetDatabase.GUIDToAssetPath(guid);
			var animationClip = AssetDatabase.LoadMainAssetAtPath(path) as AnimationClip;

			if (animationClip != null)
			{
				var bindings = AnimationUtility.GetObjectReferenceCurveBindings(animationClip);

				foreach (var binding in bindings)
				{
					var keys  = AnimationUtility.GetObjectReferenceCurve(animationClip, binding);
					var dirty = false;

					for (var i = 0; i < keys.Length; i++)
					{
						var key    = keys[i];
						var sprite = key.value as Sprite;

						if (Link(ref sprite) == true)
						{
							key.value = sprite;

							keys[i] = key;

							dirty = true;
						}
					}

					if (dirty == true)
					{
						AnimationUtility.SetObjectReferenceCurve(animationClip, binding, keys); EditorUtility.SetDirty(animationClip);
					}
				}
			}
		}
	}

	private void LinkInTransform(Transform t)
	{
		var components = t.GetComponents<Component>();

		foreach (var component in components)
		{
			LinkInComponent(component);
		}

		for (var i = 0; i < t.childCount; i++)
		{
			LinkInTransform(t.GetChild(i));
		}
	}

	private void LinkInComponent(Component c)
	{
		// Sprite renderers don't store the sprite in a field, so manually replace it
		var spriteRenderer = c as SpriteRenderer;

		if (spriteRenderer != null)
		{
			var sprite = spriteRenderer.sprite;

			if (Link(ref sprite) == true)
			{
				spriteRenderer.sprite = sprite; EditorUtility.SetDirty(spriteRenderer);
			}

			return;
		}

		// Images don't store the sprite in a field, so manually replace it
		var image = c as UnityEngine.UI.Image;

		if (image != null)
		{
			var sprite = image.sprite;

			if (Link(ref sprite) == true)
			{
				image.sprite = sprite; EditorUtility.SetDirty(image);
			}

			return;
		}

		var o = (object)c;

		if (LinkInObject(ref o, 0) == true)
		{
			EditorUtility.SetDirty(c);
		}
	}

	private static IEnumerable<FieldInfo> GetAllFields(System.Type t)
	{
		if (t == null) return Enumerable.Empty<FieldInfo>();

		var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;

		return t.GetFields(flags).Concat(GetAllFields(t.BaseType));
	}

	private bool LinkInObject(ref object o, int depth)
	{
		var dirty = false;
		
		if (o != null && depth < 3)
		{
			var fields = GetAllFields(o.GetType());

			foreach (var field in fields)
			{
				try
				{
					var type = field.FieldType;

					// Array?
					if (type.IsArray == true)
					{
						/*
						var array = (System.Array)field.GetValue(o);

						if (array != null)
						{
							Debug.Log(field);
							for (var i = 0; i < array.Length; i++)
							{
								var element = array.GetValue(i);

								if (LinkInObject(ref element, depth + 1) == true)
								{
									array.SetValue(element, i);

									//	dirty = true;
								}
							}
						}
						*/
					}
					// Direct?
					else if (type == typeof(Sprite))
					{
						var sprite = (Sprite)field.GetValue(o);

						if (Link(ref sprite) == true)
						{
							field.SetValue(o, sprite);

							dirty = true;
						}
					}
					// Class?
					else if (type.IsClass == true)
					{
						var c = field.GetValue(o);

						if (LinkInObject(ref c, depth + 1) == true)
						{
							dirty = true;
						}
					}
					// Struct?
					else if (type.IsValueType == true && type.IsEnum == false && type.IsPrimitive == false)
					{
						var s = field.GetValue(o);
						var r = LinkInObject(ref s, depth + 1);

						if (r == true)
						{
							field.SetValue(o, s);

							dirty = true;
						}
					}
				}
				catch { }
			}
		}

		return dirty;
	}

	private bool Link(ref Sprite sprite)
	{
		if (sprite != null)
		{
			var index = System.Array.IndexOf(sourceSprites, sprite);

			if (index != -1)
			{
				sprite = packedSprites[index];

				if (sprite != null)
				{
					return true;
				}
			}
		}

		return false;
	}
}