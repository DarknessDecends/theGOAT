using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(TileMap))]
public class TileMapEditor : Editor {

	public TileMap map; //our target script

	TileBrush brush; //tooltip that appears in the 

	//variables related to mouse position
	Vector3 mouseHitPos;
	bool mouseOnMap {
		get { return mouseHitPos.x > 0 && mouseHitPos.x < map.gridSize.x && mouseHitPos.y < 0 && mouseHitPos.y > -map.gridSize.y; }
	}

	//draws the following elements in the inspector each frame
	public override void OnInspectorGUI() {
		serializedObject.Update(); //keep serialized version of map object up to date

		var oldSize = map.mapSize;
		var newSize = EditorGUILayout.Vector2Field("Map Size:", map.mapSize);

		if (map.mapSize != oldSize) {
			if (newSize.x < oldSize.x || newSize.y < oldSize.y) {
				//Undo.RecordObject(map.tileArray, "resize tilemap");
			}
			//tileArray.resize()
			map.mapSize = newSize;
			UpdateCalculations();
			map.Create();
		}

		Debug.Log(map.tileSet.Count);

		//keep an empty sprite on the end as a buffer
		if (map.tileSet.Count <= 0) { //if tileSet is empty
			map.tileSet.Add(new Tile());
		} else {
			Tile lastElem = map.tileSet[map.tileSet.Count-1]; //index of last element
			if (lastElem.sprite != null) { //if last element has a sprite
				map.tileSet.Add(new Tile()); //create another element
			}
		}

		SerializedProperty tileSet = serializedObject.FindProperty("tileSet");
		for (int i = 0; i < tileSet.arraySize; i++) {

			
			Editor editor = null;
			var myAsset = tileSet.GetArrayElementAtIndex(i);
			CreateCachedEditor(myAsset.objectReferenceValue, null, ref editor);
			editor.OnInspectorGUI();
			


			EditorGUILayout.PropertyField(tileSet.GetArrayElementAtIndex(i), true);
		}

		EditorGUILayout.LabelField("Tile Size:", map.tileSize.x+"x"+map.tileSize.y);
		EditorGUILayout.LabelField("Grid Size In Units:", map.gridSize.x+"x"+map.gridSize.y);
		EditorGUILayout.LabelField("Pixels To Units:", map.pixelsToUnits.ToString());

		if (brush != null) {
			brush.setSprite(map.currentTileBrush);
		}

		if (GUILayout.Button("Clear Tiles")) {
			if (EditorUtility.DisplayDialog("Clear map's tiles?", "Are you sure?", "Clear", "Do not clear")) {
				ClearMap();
			}
		}

		serializedObject.ApplyModifiedProperties(); //apply changes (allows undo)
	}

	//called every time the inspector gains focus
	void OnEnable() {
		map = target as TileMap;
		Tools.current = Tool.View;

		//create tilemap if it doesn't already exist
		if (map.renderer == null) {
			map.Create();
		}

		UpdateCalculations();
		//ReDraw();

		//create brush if it doesn't already exist
		if (map.transform.childCount == 0) {
			CreateBrush();
		} else { //brush exists
			brush = map.transform.GetChild(0).GetComponent<TileBrush>();
		}
		//}
	}

	//called when tilemap loses focus in the heirarchy
	void OnDisable() {
		//if map has not been destroyed
		if (map != null && map.transform.childCount > 0) {
			DestroyImmediate(map.transform.GetChild(0).gameObject);
		}
	}

	//called every frame when sceneview is selected
	void OnSceneGUI() {
		if (brush != null) {
			UpdateHitPosition();
			brush.Move(mouseHitPos, mouseOnMap);

			if (map.tileSet != null && mouseOnMap) {
				Event current = Event.current;
				if (current.shift) {
					Draw();
				} else if (current.alt) {
					RemoveTile();
				}
			}
		}
	}



	void UpdateCalculations() {
		/*
		//http://answers.unity3d.com/questions/14246/editor-class-texture-importer-apply-import-setting.html

		GameObject[] tilePrefabs = Resources.LoadAll<GameObject>("");//get all tile prefabs in Resources

		Tile[] tiles = tilePrefabs.Select(x => x.GetComponent<Tile>()).ToArray(); //convert list of gameobjects to list of Tiles

		map.tileSet = tiles.Select(x => x.sprite).ToArray(); //convert list of Tiles to list of Sprites

		
		var path = AssetDatabase.GetAssetPath(map.tileSheet); //get our tilesets filepath (including itself)
		TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path); //create a texture importer for this asset
		importer.textureType = TextureImporterType.Advanced; //allows us to set isReadable to true
		importer.isReadable = true; //allows us to read pixels from this texture
		AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
		map.tileSet = AssetDatabase.LoadAllAssetsAtPath(importer.assetPath); //loads tiles as they are sliced in the unity editor
		*/

		var sprite = map.tileSet[0].sprite;
		var width = sprite.textureRect.width;
		var height = sprite.textureRect.height;

		map.tileSize = new Vector2(width, height);
		map.pixelsToUnits = (int)(sprite.rect.width / sprite.bounds.size.x);
		map.gridSize = new Vector2((width / map.pixelsToUnits) * map.mapSize.x, (height/map.pixelsToUnits) * map.mapSize.y);
	}

	void CreateBrush() {
		if (map.currentTileBrush != null) {
			brush = TileBrush.CreateChildOf(map.transform);
		}
	}

	//called every frame by onSceneGUI
	void UpdateHitPosition() {
		var p = new Plane(map.transform.TransformDirection(Vector3.forward), Vector3.zero);
		var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		var hit = Vector3.zero;
		var dist = 0f;

		if (p.Raycast(ray, out dist))
			hit = ray.origin + ray.direction.normalized * dist;

		mouseHitPos = map.transform.InverseTransformPoint(hit);
	}

	void Draw() {
		SetTile((int)brush.tileXY.x, (int)brush.tileXY.y, brush.renderer.sprite);
	}

	//this overload paints a sprite
	void SetTile(int x, int y, Sprite sprite) {
		var rect = sprite.rect;
		var colors = sprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
		SetTile(x, y, colors);
	}
	//this overload paints a solid colour
	void SetTile(int x, int y, Color color) {
		var colors = Enumerable.Repeat(Color.clear, (int)(map.tileSize.x*map.tileSize.y)).ToArray(); //create array of same colour
		SetTile((int)brush.tileXY.x, (int)brush.tileXY.y, colors);
	}
	//Paint a tile with an array of pixels
	void SetTile(int x, int y, Color[] pixels) {
		if (map.tileSet == null) {
			Debug.LogError("there was an error loading tiles prefabs");
			return;
		}

		y = (int)map.mapSize.y + y; //the texture has 0,0 in the bottom left, flip y to put it at upper left

		Texture2D mapTex = map.renderer.sprite.texture;
		//currently this is crazy inefficient
		//Undo.RecordObject(mapTex, "edit tile");

		//set the pixles & apply changes
		mapTex.SetPixels(
			x * (int)map.tileSize.x,
			y * (int)map.tileSize.y,
			(int)map.tileSize.x,
			(int)map.tileSize.y,
			pixels);
		mapTex.Apply();
	}

	void RemoveTile() {
		//paint tile with nothing
	}

	void ClearMap() {
		DestroyImmediate(map.renderer);
		map.Create(); //this is untested
	}
}