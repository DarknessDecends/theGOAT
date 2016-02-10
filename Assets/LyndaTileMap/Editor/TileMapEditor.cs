using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(TileMap))]
public class TileMapEditor : Editor {

	public TileMap map; //our target script

	TileBrush brush; //tooltip that appears in the 

	Vector3 mouseHitPos;
	bool mouseOnMap{
		get { return mouseHitPos.x > 0 && mouseHitPos.x < map.gridSize.x && mouseHitPos.y < 0 && mouseHitPos.y > -map.gridSize.y;}
	}

	//draws the following elements in the inspector each frame
	public override void OnInspectorGUI() {
		var oldSize = map.mapSize;
		var newSize = EditorGUILayout.Vector2Field("Map Size:", map.mapSize);
		if (newSize.x < oldSize.x || newSize.y < oldSize.y) {
			//this is broken - popup prevents user fro entering two digits
			if (EditorUtility.DisplayDialog("reducing map size will cut off extra tiles", "Are you sure?", "resize", "Do not resize")) {
				map.mapSize = newSize;
				UpdateCalculations();
				map.Create();
			}
		} else if (map.mapSize != oldSize) {
			UpdateCalculations();
			map.Create();
		}

		var oldTexture = map.tileSheet;
		map.tileSheet = (Texture2D)EditorGUILayout.ObjectField("Texture2D:", map.tileSheet, typeof(Texture2D), false);

		if (oldTexture != map.tileSheet) {
			UpdateCalculations();
			map.tileID = 1;
			CreateBrush();
			map.Create();
		}

		if (map.tileSheet == null) {
			EditorGUILayout.HelpBox ("You have not selected a texture 2D yet.", MessageType.Warning);
		} else {
			EditorGUILayout.LabelField("Tile Size:", map.tileSize.x+"x"+map.tileSize.y);
			EditorGUILayout.LabelField("Grid Size In Units:", map.gridSize.x+"x"+map.gridSize.y);
			EditorGUILayout.LabelField("Pixels To Units:", map.pixelsToUnits.ToString());

			brush.setSprite(map.currentTileBrush);

			if(GUILayout.Button("Clear Tiles")){
				if(EditorUtility.DisplayDialog("Clear map's tiles?", "Are you sure?", "Clear", "Do not clear")){
					ClearMap();
				}
			}
		}
	}

	//called every time the inspector gains focus
	void OnEnable(){
		map = target as TileMap;
		Tools.current = Tool.View;

		//create tilemap if it doesn't already exist
		if (map.renderer == null) {
			map.Create();
		}

		if (map.tileSheet != null) {
			UpdateCalculations();

			//create brush if it doesn't already exist
			if (map.transform.childCount == 0) {
				CreateBrush();
			} else { //brush exists
				brush = map.transform.GetChild(0).GetComponent<TileBrush>();
			}
		}
	}

	//called when tilemap loses focus in the heirarchy
	void OnDisable(){
		//if map has not been destroyed
		if (map != null && map.transform.childCount > 0) {
			DestroyImmediate(map.transform.GetChild(0).gameObject);
		}
	}

	//called every frame when sceneview is selected
	void OnSceneGUI(){
		if (brush != null) {
			UpdateHitPosition();
			brush.Move(mouseHitPos, mouseOnMap);

			if(map.tileSheet != null && mouseOnMap){
				Event current = Event.current;
				if(current.shift){
					Draw();
				}else if(current.alt){
					RemoveTile();
				}
			}
		}
	}

	

	void UpdateCalculations(){
		//http://answers.unity3d.com/questions/14246/editor-class-texture-importer-apply-import-setting.html
		var path = AssetDatabase.GetAssetPath(map.tileSheet); //get our tilesets filepath (including itself)
		TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path); //create a texture importer for this asset
		importer.textureType = TextureImporterType.Advanced; //allows us to set isReadable to true
		importer.isReadable = true; //allows us to read pixels from this texture
		AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
		map.spriteReferences = AssetDatabase.LoadAllAssetsAtPath(importer.assetPath); //loads tiles as they are sliced in the unity editor

		var sprite = (Sprite)map.spriteReferences[1];
		var width = sprite.textureRect.width;
		var height = sprite.textureRect.height;
		
		map.tileSize = new Vector2(width, height);
		map.pixelsToUnits = (int)(sprite.rect.width / sprite.bounds.size.x);
		map.gridSize = new Vector2((width / map.pixelsToUnits) * map.mapSize.x, (height/map.pixelsToUnits) * map.mapSize.y);
	}

	void CreateBrush() {
		if (map.currentTileBrush != null) {
			GameObject brushObj = new GameObject("brush");
			brushObj.transform.SetParent(map.transform);
			brush = TileBrush.Create(brushObj);
		}
	}

	//called every frame by onSceneGUI
	void UpdateHitPosition() {
		var p = new Plane(map.transform.TransformDirection (Vector3.forward), Vector3.zero);
		var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		var hit = Vector3.zero;
		var dist = 0f;

		if (p.Raycast (ray, out dist))
			hit = ray.origin + ray.direction.normalized * dist;

		mouseHitPos = map.transform.InverseTransformPoint (hit);
	}

	void Draw(){
		SetTile((int)brush.tileXY.x, (int)brush.tileXY.y, brush.renderer.sprite);
	}

	//this overload paints a sprite
	void SetTile(int x, int y, Sprite sprite) {
		var rect = sprite.rect;
		var colors = sprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
		SetTile(x, y, colors);
	}
	//Paint a tile with an array of pixels
	void SetTile(int x, int y, Color[] pixels) {
		if (map.tileSheet == null) {
			Debug.LogError("TileSheet has not been assigned in the inspector");
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

	void RemoveTile(){
		var colors = Enumerable.Repeat(Color.clear, (int)(map.tileSize.x*map.tileSize.y)).ToArray(); //create array of same colour
		SetTile((int)brush.tileXY.x, (int)brush.tileXY.y, colors); //paint tile with nothing
	}

	void ClearMap(){
		DestroyImmediate(map.renderer);
	}
}