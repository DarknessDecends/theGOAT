using UnityEngine;
using System.Collections;
using UnityEditor;

public class TilePickerWindow : EditorWindow {

	public enum Scale { x1, x2, x3, x4, x5 }

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

	void OnGUI() {
		//don't render if no tilemap is selected
		if (Selection.activeObject == null)
			return;

		//tilemap currently selected in hierarchy
		var selection = (Selection.activeObject as GameObject).GetComponent<TileMap>();

		if (selection != null) {
			if (selection.tileSet != null) {

				//get scale from inspector
				scale = (Scale)EditorGUILayout.EnumPopup("Zoom", scale);
				var newScale = ((int)scale) + 1;

				//tile size * scale
				var newTileSize = selection.tileSize * newScale;

				//size of all tiles in a horizontal line
				var allTiles = new Vector2(selection.tileSet.Count*newTileSize.x, newTileSize.y);
				var offset = new Vector2(10, 25);

				var viewPort = new Rect(0, 0, position.width-5, position.height-5);
				var contentSize = new Rect(0, 0, allTiles.x + offset.x, allTiles.y + offset.y);
				scrollPosition = GUI.BeginScrollView(viewPort, scrollPosition, contentSize);
				for (int i = 0; i < selection.tileSet.Count; i++) { //for all sprites in spritereferences
					GUI.DrawTexture(new Rect(offset.x + newTileSize.x*i, offset.y, newTileSize.x, newTileSize.y), selection.tileSet[i].sprite.texture); //draw the tileSheet
				}
				
				var grid = new Vector2(allTiles.x / newTileSize.x, allTiles.y / newTileSize.y); //grid size

				var selectionPos = new Vector2(newTileSize.x * currentSelection.x + offset.x, //currentSelection is (0, 0) be default
											   newTileSize.y * currentSelection.y + offset.y);

				//create blue square around a tile
				var boxTex = new Texture2D(1, 1);
				boxTex.SetPixel(0, 0, new Color(0, 0.5f, 1f, 0.4f)); //tranparent blue
				boxTex.Apply();

				var style = new GUIStyle(GUI.skin.customStyles[0]);
				style.normal.background = boxTex;

				GUI.Box(new Rect(selectionPos.x, selectionPos.y, newTileSize.x, newTileSize.y), "", style);

				//handle mouse click
				var cEvent = Event.current;
				Vector2 mousePos = new Vector2(cEvent.mousePosition.x, cEvent.mousePosition.y);
				if (cEvent.type == EventType.mouseDown && cEvent.button == 0) {
					currentSelection.x = Mathf.Floor((mousePos.x + scrollPosition.x) / newTileSize.x);
					currentSelection.y = Mathf.Floor((mousePos.y + scrollPosition.y) / newTileSize.y);

					//clamp out-of-bound clicks
					if (currentSelection.x > grid.x -1)
						currentSelection.x = grid.x -1;

					if (currentSelection.y > grid.y -1)
						currentSelection.y = grid.y -1;

					//set the tilemap's current tile
					selection.tileID = (int)(currentSelection.x + (currentSelection.y * grid.x));

					Repaint();
				}

				GUI.EndScrollView();
			}
		}
	}
}
