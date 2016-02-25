using UnityEngine;
using System.Collections;
using UnityEditor;

public class TilePickerWindow : EditorWindow {

	public enum Scale {
		x1,
		x2,
		x3,
		x4,
		x5
	}

	Scale scale;
	Vector2 currentSelection = Vector2.zero;

	public Vector2 scrollPosition = Vector2.zero;

	[MenuItem("Window/Tile Picker")]
	public static void OpenTilePickerWindow() {
		var window = EditorWindow.GetWindow(typeof(TilePickerWindow));
		var title = new GUIContent();
		title.text = "Tile Picker";
		window.titleContent = title;
	}

	void OnSelectionChange() {
		var activeObject = Selection.activeObject as GameObject;
		if (activeObject == null)
			return;

		if (activeObject.GetComponent<TileMap>() != null)
			Repaint();
	}

	void OnGUI() {
		var activeObject = Selection.activeObject as GameObject;
		if (activeObject == null || activeObject.GetComponent<TileMap>() == null) {
			EditorGUILayout.HelpBox("Please click on an existing tilemap in the Hierarchy,\nor select GameObject->TileMap for a new tile map.", MessageType.Info);
		} else {
			var selection = activeObject.GetComponent<TileMap>();

			var texture2D = selection.texture2D;
			if (texture2D != null) {

				scale = (Scale)EditorGUILayout.EnumPopup("Zoom", scale);
				var newScale = ((int)scale) + 1;
				var newTextureSize = new Vector2(texture2D.width, texture2D.height) * newScale;
				var offset = new Vector2(10, 25);
				var spacing = selection.offset; //space between tileSheet tiles

				var viewPort = new Rect(0, 0, position.width-5, position.height-5);
				var contentSize = new Rect(0, 0, newTextureSize.x + offset.x, newTextureSize.y + offset.y);
				scrollPosition = GUI.BeginScrollView(viewPort, scrollPosition, contentSize);
				GUI.DrawTexture(new Rect(offset.x, offset.y, newTextureSize.x, newTextureSize.y), texture2D);

				var tile = new Vector2(selection.tileSize.x+spacing, selection.tileSize.y+spacing) * newScale;
				var grid = new Vector2(newTextureSize.x / tile.x, newTextureSize.y / tile.y);

				var selectionPos = new Vector2(tile.x * currentSelection.x + offset.x,
											   tile.y * currentSelection.y + offset.y);

				var boxTex = new Texture2D(1, 1);
				boxTex.SetPixel(0, 0, new Color(0, 0.5f, 1f, 0.4f));
				boxTex.Apply();

				var style = new GUIStyle(GUI.skin.customStyles[0]);
				style.normal.background = boxTex;

				GUI.Box(new Rect(selectionPos.x, selectionPos.y, tile.x, tile.y), "", style);

				var cEvent = Event.current;
				Vector2 mousePos = new Vector2(cEvent.mousePosition.x, cEvent.mousePosition.y);
				if (cEvent.type == EventType.mouseDown && cEvent.button == 0) {
					currentSelection.x = Mathf.Floor((mousePos.x - offset.x) / tile.x);
					currentSelection.y = Mathf.Floor((mousePos.y - offset.y) / tile.y);

					if (currentSelection.x > grid.x -1)
						currentSelection.x = grid.x -1;

					if (currentSelection.y > grid.y -1)
						currentSelection.y = grid.y -1;

					selection.tileID = (int)(currentSelection.x + (currentSelection.y * grid.x) + 1);

					Repaint();
				}

				GUI.EndScrollView();
			}
		}
	}
}
