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
//Check out our projects here
//WEBSITE: https://www.lazyfridaystudio.com/projects
//
//Hope you enjoy the simple Lazy Dependency Finder 
//designed and made by LazyFridayStudio
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
		private Texture2D mainBackground;
		private Texture2D secondaryBackground;
		private Texture2D seperator;
		private Texture2D oddBackground;
		private Texture2D evenBackground;
		#endregion

		#region Styles
		public GUIStyle mainTextStyle = new GUIStyle();
		public GUIStyle secondaryTextStyle = new GUIStyle();
		public GUIStyle generalTexStyle = new GUIStyle();
		public GUIStyle paddingStyle = new GUIStyle();

		public GUIStyle evenItemStyle = new GUIStyle();
		public GUIStyle oddItemStyle = new GUIStyle();
		#endregion

		#region Sections

		Rect headerSection;
		Rect subMenuSection;
		Rect itemSection;

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
			mainBackground = new Texture2D(1, 1);
			mainBackground.SetPixel(0, 0, LazyEditorHelperUtils.LazyFridayBackgroundColor);
			mainBackground.Apply();
            
			secondaryBackground = new Texture2D(1, 1);
			secondaryBackground.SetPixel(0, 0, new Color32(33, 33, 33, 255));
			secondaryBackground.Apply();

			seperator = new Texture2D(1, 1);
			seperator.SetPixel(0, 0, new Color32(242, 242, 242, 255));
			seperator.Apply();
            
			//Item areas
			evenBackground = new Texture2D(1, 1);
			evenBackground.SetPixel(0, 0, new Color32(44, 44, 44, 255));
			evenBackground.Apply();

			oddBackground = new Texture2D(1, 1);
			oddBackground.SetPixel(0, 0, new Color32(33, 33, 33, 255));
			oddBackground.Apply();
		}

		//Create the styles
		private void InitStyle()
		{
			string path = "Assets/Editor/LazyHelpers/Resources/HeaderFont.ttf";
			Font headerFont = EditorGUIUtility.Load(path) as Font;
            
			//Main Text
			mainTextStyle.normal.textColor = LazyEditorHelperUtils.LazyFridayMainColor;
			mainTextStyle.fontSize = 16;
			mainTextStyle.alignment = TextAnchor.LowerCenter;
			mainTextStyle.font = headerFont; 
            
			//Secondary Text
			secondaryTextStyle.normal.textColor = LazyEditorHelperUtils.LazyFridayMainColor;
			secondaryTextStyle.fontSize = 12;
			secondaryTextStyle.fontStyle = FontStyle.Bold;
			secondaryTextStyle.alignment = TextAnchor.MiddleCenter;

			//General Text
			generalTexStyle.normal.textColor = LazyEditorHelperUtils.LazyFridaySecondaryColor;
			generalTexStyle.fontSize = 12;
			generalTexStyle.alignment = TextAnchor.MiddleCenter;
            
			//Item Styles
			oddItemStyle.normal.background = oddBackground;
			oddItemStyle.padding = new RectOffset(3, 3, 3, 3);
			oddItemStyle.border = new RectOffset(0, 0, 5, 5);
			oddItemStyle.normal.textColor = LazyEditorHelperUtils.LazyFridaySecondaryColor;

			evenItemStyle.normal.background = evenBackground;
			evenItemStyle.border = new RectOffset(0, 0, 5, 5);
			evenItemStyle.padding = new RectOffset(3, 3, 3, 3);
			evenItemStyle.normal.textColor = LazyEditorHelperUtils.LazyFridaySecondaryColor;
            
			paddingStyle.margin = new RectOffset(2, 2, 4, 4);
		}

		#endregion

		#region Drawing Functions

		static GameObject obj = null;
		private void OnGUI()
		{
			if (mainBackground == null)
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
			subMenuSection.height = 27;

			itemSection.x = 0;
			itemSection.y = headerSection.height + subMenuSection.height;
			itemSection.width = Screen.width;
			itemSection.height = Screen.height;

			GUI.DrawTexture(headerSection, mainBackground);
			GUI.DrawTexture(subMenuSection, secondaryBackground);
			GUI.DrawTexture(itemSection, mainBackground);

			//Draw Seperators
			GUI.DrawTexture(new Rect(headerSection.x, headerSection.height - 2, headerSection.width, 2), seperator);
			GUI.DrawTexture(new Rect(subMenuSection.x, (subMenuSection.height + headerSection.height) - 2, subMenuSection.width, 2), seperator);
		}

		private void DrawHeader()
		{

			GUILayout.BeginArea(headerSection);
			// Rect centerRect = LazyEditorHelperUtils.CenterRect(headerSection, logoHeader);
			GUILayout.Space(7);
			GUILayout.BeginHorizontal();
			GUILayout.Label("LAZY FRIDAY STUDIO", mainTextStyle);
			GUILayout.EndHorizontal();
			//GUI.Label(new Rect(centerRect.x + 13, centerRect.y - 2, centerRect.width, centerRect.height), logoHeader);
			GUILayout.EndArea();
		}

		private void DrawSubHeading()
		{

			GUILayout.BeginArea(subMenuSection);
			GUILayout.Space(3);
			GUILayout.BeginHorizontal();
			targetObject = EditorGUILayout.ObjectField(targetObject, typeof(Object));
			GUILayout.Label("Total Dependency: " + targetObjectSearchResults.Count.ToString());

			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Find Dependency" ,GUILayout.MaxWidth(150)))
			{
				SearchForComponents();
			}
			if (GUILayout.Button("Help" ,GUILayout.MaxWidth(150)))
			{
				Application.OpenURL("https://www.lazyfridaystudio.com/lazydependencyfinder");
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
						itemStyle = evenItemStyle;
					}
					else
					{
						itemStyle = oddItemStyle;
					}
					
					
					currentItem = targetObjectSearchResults[i];
					GUILayout.BeginHorizontal(itemStyle); 
					GUILayout.Label(AssetDatabase.GetCachedIcon(currentItem), GUILayout.MaxHeight(20),GUILayout.MaxWidth(20));
					GUILayout.Label("|");
	
						GUILayout.Label(Path.GetFileName(currentItem),secondaryTextStyle);
						
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
		#endregion
	}
}