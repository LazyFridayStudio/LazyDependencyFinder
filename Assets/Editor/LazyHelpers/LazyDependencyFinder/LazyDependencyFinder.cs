#region NameSpaces

using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Object = UnityEngine.Object;
#endregion

//================================================================
//							IMPORTANT
//================================================================
//				Copyright LazyFridayStudio
//DO NOT SELL THIS CODE OR REDISTRIBUTE WITH INTENT TO SELL.
//
//Send an email to our support line for any questions or inquirys
//CONTACT: Lazyfridaystudio@gmail.com
//
//Alternatively join our Discord
//DISCORD: https://discord.gg/Z3tpMG
//
//Hope you enjoy the simple Scene loader 
//designed and made by lazyFridayStudio
//================================================================

namespace LazyHelper.LazyDependencyFinder
{
	public class LazyDependencyFinder : EditorWindow
	{
		#region Editor Values
		private static LazyDependencyFinder _window;
		private Vector2 _scrollArea = Vector2.zero;
		private Object targetObject = null;
		private List<string> targetObjectSearchResults = new List<string>();
		private bool isRecurse;

		[MenuItem("Window/LazyHelper/Lazy Dependency Finder")]
		public static void Init()
		{
			// Get existing open window or if none, make a new one:
			_window = (LazyDependencyFinder) GetWindow(typeof(LazyDependencyFinder));
			_window.titleContent.text = "Lazy Dependency Finder";
			_window.position = new Rect(0, 0, 600, 800);
			_window.autoRepaintOnSceneChange = false;
		}

		#region Textures
		Texture logoHeader;

		Texture2D headerBackground;
		Texture2D headerSeperator;

		Texture2D submenuBackground;

		Texture2D itemBackground;

		Texture2D itemOddBackground;
		Texture2D itemEvenBackground;
		#endregion

		#region Styles

		//Padding style
		public GUIStyle stylePadding = new GUIStyle();

		//Background Styles
		public GUIStyle evenBoxStyle = new GUIStyle();
		public GUIStyle oddBoxStyle = new GUIStyle();

		//Font Styles
		public GUIStyle itemNameStyle = new GUIStyle();
		public GUIStyle seperator = new GUIStyle();
		public GUIStyle icon = new GUIStyle();
		#endregion

		#region Sections

		Rect headerSection;
		Rect subMenuSection;
		Rect ItemSection;

		#endregion

		#endregion

		#region On Enable Functions

		//On SceneChange
		private void OnHierarchyChange()
		{
			OnEnable();
			Repaint();
		}

		//Start Function
		private void OnEnable()
		{
			InitTextures();
			InitStyle();
		}

		//Draw the textures and get images
		private void InitTextures()
		{
			string path = "Assets/Editor/LazyHelpers/Resources/Logo.png";
			logoHeader = EditorGUIUtility.Load(path) as Texture;

			headerBackground = new Texture2D(1, 1);
			headerBackground.SetPixel(0, 0, new Color32(22, 22, 22, 255));
			headerBackground.Apply();

			headerSeperator = new Texture2D(1, 1);
			headerSeperator.SetPixel(0, 0, new Color32(239, 143, 29, 255));
			headerSeperator.Apply();

			submenuBackground = new Texture2D(1, 1);
			submenuBackground.SetPixel(0, 0, new Color32(33, 33, 33, 255));
			submenuBackground.Apply();

			itemBackground = new Texture2D(1, 1);
			itemBackground.SetPixel(0, 0, new Color32(22, 22, 22, 255));
			itemBackground.Apply();

			itemEvenBackground = new Texture2D(1, 1);
			itemEvenBackground.SetPixel(0, 0, new Color32(44, 44, 44, 255));
			itemEvenBackground.Apply();

			itemOddBackground = new Texture2D(1, 1);
			itemOddBackground.SetPixel(0, 0, new Color32(33, 33, 33, 255));
			itemOddBackground.Apply();
		}

		//Create the styles
		private void InitStyle()
		{
			oddBoxStyle.normal.background = itemOddBackground;
			oddBoxStyle.padding = new RectOffset(3, 3, 3, 3);
			evenBoxStyle.border = new RectOffset(0, 0, 5, 5);
			oddBoxStyle.normal.textColor = new Color32(255, 255, 255, 255);

			evenBoxStyle.normal.background = itemEvenBackground;
			evenBoxStyle.border = new RectOffset(0, 0, 5, 5);
			evenBoxStyle.padding = new RectOffset(3, 3, 3, 3);
			evenBoxStyle.normal.textColor = new Color32(255, 255, 255, 255);

			itemNameStyle.normal.textColor = new Color32(239, 143, 29, 255);
			itemNameStyle.fontSize = 14;
			itemNameStyle.fontStyle = FontStyle.Bold;
			itemNameStyle.alignment = TextAnchor.MiddleCenter;

			stylePadding.margin = new RectOffset(2, 2, 4, 4);
		}

		#endregion

		#region Drawing Functions

		static GameObject obj = null;
		private void OnGUI()
		{
			// obj = EditorGUI.ObjectField(new Rect(3, 3, position.width - 6, 20), "Find Dependency", obj, typeof(GameObject)) as GameObject;
			//
			// if (obj)
			// {
			// 	Object[] roots = new Object[] { obj };
			//
			// 	if (GUI.Button(new Rect(3, 25, position.width - 6, 20), "Check Dependencies"))
			// 		Selection.objects = EditorUtility.CollectDependencies(roots);
			// }
			// else
			// 	EditorGUI.LabelField(new Rect(3, 25, position.width - 6, 20), "Missing:", "Select an object first");
			//
			
			if (headerBackground == null)
			{
				OnEnable();
			}

			DrawLayout();
			DrawHeader();
			DrawSubHeading();
			DrawItems();

		}

		private void DrawLayout()
		{
			headerSection.x = 0;
			headerSection.y = 0;
			headerSection.width = Screen.width;
			headerSection.height = 25;

			subMenuSection.x = 0;
			subMenuSection.y = headerSection.height;
			subMenuSection.width = Screen.width;
			subMenuSection.height = 70;

			ItemSection.x = 0;
			ItemSection.y = headerSection.height + subMenuSection.height;
			ItemSection.width = Screen.width;
			ItemSection.height = Screen.height;

			GUI.DrawTexture(headerSection, headerBackground);
			GUI.DrawTexture(subMenuSection, submenuBackground);
			GUI.DrawTexture(ItemSection, headerBackground);

			//Draw Seperators
			GUI.DrawTexture(new Rect(headerSection.x, headerSection.height - 2, headerSection.width, 2), headerSeperator);
			GUI.DrawTexture(new Rect(subMenuSection.x, (subMenuSection.height + headerSection.height) - 2, subMenuSection.width, 2), headerSeperator);
		}

		private void DrawHeader()
		{
			GUILayout.BeginArea(headerSection);
			Rect centerRect = LazyEditorHelperUtils.CenterRect(headerSection, logoHeader);
			GUI.Label(new Rect(centerRect.x + 13, centerRect.y - 2, centerRect.width, centerRect.height), logoHeader);
			GUILayout.EndArea();
		}

		private void DrawSubHeading()
		{
			GUILayout.BeginArea(subMenuSection);
			string helpMessage;
			MessageType messageType;
			if (targetObject != null)
			{
				if (targetObject.GetType() != typeof(MonoBehaviour))
				{
					helpMessage = "This only works for items that are prefabs or in your project view";
						messageType = MessageType.Info;
				}
				else
				{
					helpMessage = "This might return nothing, Works best with MonoBehaviours";
						messageType = MessageType.Warning;
				}
			}
			else
			{
				helpMessage = "Item is NULL";
				messageType = MessageType.Error;
			}
			EditorGUILayout.HelpBox(helpMessage, messageType, true);
			
			GUILayout.BeginHorizontal();
			targetObject = EditorGUILayout.ObjectField(targetObject, typeof(Object));
			GUILayout.Label("Total Dependency: " + targetObjectSearchResults.Count.ToString());

			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Find Dependency" ,GUILayout.MaxWidth(150)))
			{
				SearchForComponents();
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		private void DrawItems()
		{
			GUILayout.Space(headerSection.height + subMenuSection.height);
			_scrollArea = GUILayout.BeginScrollView(_scrollArea);
			GUILayout.BeginVertical();
			if (targetObjectSearchResults.Count > 0)
			{
				GUIStyle itemStyle = new GUIStyle();
				
				string currentItem = string.Empty;
				for (int i = 0; i < targetObjectSearchResults.Count; i++)
				{ 
					bool isEven = i % 2 == 0;
					
					if (isEven)
					{
						itemStyle = evenBoxStyle;
					}
					else
					{
						itemStyle = oddBoxStyle;
					}
					
					
					currentItem = targetObjectSearchResults[i];
					GUILayout.BeginHorizontal(itemStyle); 
					GUILayout.Label(AssetDatabase.GetCachedIcon(currentItem), GUILayout.MaxHeight(20),GUILayout.MaxWidth(20));
					GUILayout.Label("|");
	
						GUILayout.Label(Path.GetFileName(currentItem),itemNameStyle);
					
					
					GUILayout.Label("|");
					GUILayout.Label(currentItem);
					GUILayout.FlexibleSpace();
					
					if (GUILayout.Button("Select Item"))
					{
						Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(currentItem);
					}
					
					GUILayout.EndHorizontal();
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
		}
		#endregion
		
		#region Search Function
		private void SearchForComponents()
		{
			targetObjectSearchResults.Clear();
			
			Object[] targetAsset = new[] {targetObject};
			Object[] allDependent = EditorUtility.CollectDependencies(targetAsset);

			for (int i = 0; i < allDependent.Length; i++)
			{
				Object currentTarget = allDependent[i];
				if (!targetObjectSearchResults.Contains(AssetDatabase.GetAssetPath(currentTarget)))
				{
					targetObjectSearchResults.Add(AssetDatabase.GetAssetPath(currentTarget));
				}
			}
		}
		
		private List<string> getAllDependencys()
		{
			string[] tempResult = AssetDatabase.GetAllAssetPaths();
			List<string> result = new List<string>();
			foreach (string s in tempResult)
			{
				result.Add(s);
			}
			return result;
		}
		#endregion
		
	}

}