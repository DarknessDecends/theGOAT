using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TileMap))]
public class TileMapEditor : Editor {

	public TileMap map;
	public TileBrush brush;

	private Vector3 mouseHitPos;

	bool mouseOnMap {
		get { return mouseHitPos.x > 0 && mouseHitPos.x < map.gridSize.x && mouseHitPos.y < 0 && mouseHitPos.y > -map.gridSize.y; }
	}

	void Awake() {
		EditorApplication.playmodeStateChanged += CreateColliders;
	}

	void CreateColliders() {
		map.tileManager.CreateColliders(map.mapSize);
	}

	public override void OnInspectorGUI() {
		var oldSize = map.mapSize;
		map.mapSize = EditorGUILayout.Vector2Field("Map Size:", map.mapSize);
		if (map.mapSize != oldSize) {
			UpdateCalculations();
		}

		var oldTexture = map.texture2D;
		map.texture2D = (Texture2D)EditorGUILayout.ObjectField("Texture2D:", map.texture2D, typeof(Texture2D), false);

		if (oldTexture != map.texture2D) {
			UpdateCalculations();
			map.tileID = 1;
			RefreshBrush();
		}

		map.solidIndex = EditorGUILayout.IntField("Solid Tiles Index:", map.solidIndex);

		if (map.texture2D == null) {
			EditorGUILayout.HelpBox("You have not selected a texture 2D yet.", MessageType.Warning);
		} else {
			EditorGUILayout.LabelField("Tile Size:", map.tileSize.x+"x"+map.tileSize.y);
			EditorGUILayout.LabelField("Grid Size In Units:", map.gridSize.x+"x"+map.gridSize.y);
			EditorGUILayout.LabelField("Pixels To Units:", map.pixelsToUnits.ToString());
			UpdateBrush(map.currentTileBrush);

			if (GUILayout.Button("Clear Tiles")) {
				if (EditorUtility.DisplayDialog("Clear map's tiles?", "Are you sure?", "Clear", "Do not clear")) {
					ClearMap();
				}
			}
		}
	}

	void OnEnable() {
		map = target as TileMap;
		brush = map.brush;
		Tools.current = Tool.View;

		//create new map
		if (map.tileManager == null) {
			var go = new GameObject("Tiles");
			go.transform.SetParent(map.transform);
			go.transform.position = Vector3.zero;
			go.layer = LayerMask.NameToLayer("Wall");
			map.tileManager = go.AddComponent<TileManager>();
		}

		//create new brush
		if (brush == null) {
			var sprite = map.currentTileBrush;
			if (sprite != null) {
				GameObject go = new GameObject("Brush");
				go.transform.SetParent(map.transform);

				brush = go.AddComponent<TileBrush>();
				brush.renderer2D = go.AddComponent<SpriteRenderer>();
				brush.renderer2D.sortingOrder = 1000;

				RefreshBrush();
			}
			//re-enable brush
		} else if (!brush.isActiveAndEnabled) {
			brush.gameObject.SetActive(true);

			if (map.texture2D != null) {
				UpdateCalculations();
				RefreshBrush();
			}
		}
	}

	void OnDisable() {
		//deactivate brush
		if (brush!= null) {
			brush.gameObject.SetActive(false);
		}

	}

	//update scene view
	void OnSceneGUI() {
		if (brush != null) {
			UpdateHitPosition();
			MoveBrush();

			if (map.texture2D != null && mouseOnMap) {
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
		var path = AssetDatabase.GetAssetPath(map.texture2D);
		map.spriteReferences = AssetDatabase.LoadAllAssetsAtPath(path);

		var sprite = (Sprite)map.spriteReferences[1];
		var width = sprite.textureRect.width;
		var height = sprite.textureRect.height;

		map.tileSize = new Vector2(width, height);
		map.pixelsToUnits = (int)(sprite.rect.width / sprite.bounds.size.x);
		map.gridSize = new Vector2((width / map.pixelsToUnits) * map.mapSize.x, (height/map.pixelsToUnits) * map.mapSize.y);
		map.offset = (int)(((Sprite)map.spriteReferences[2]).rect.x - sprite.rect.xMax);
	}

	void RefreshBrush() {
		var sprite = map.currentTileBrush;
		if (sprite != null) {
			var pixelsToUnits = map.pixelsToUnits;
			brush.brushSize = new Vector2(sprite.textureRect.width / pixelsToUnits, sprite.textureRect.height / pixelsToUnits);
			brush.UpdateBrush(sprite);
		}
	}

	public void UpdateBrush(Sprite sprite) {
		if (brush != null)
			brush.UpdateBrush(sprite);
	}

	void UpdateHitPosition() {
		var p = new Plane(map.transform.TransformDirection(Vector3.forward), Vector3.zero);
		var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		var hit = Vector3.zero;
		var dist = 0f;

		if (p.Raycast(ray, out dist))
			hit = ray.origin + ray.direction.normalized * dist;

		mouseHitPos = map.transform.InverseTransformPoint(hit);

	}

	void MoveBrush() {
		var tileSize = map.tileSize.x / map.pixelsToUnits;

		var x = Mathf.Floor(mouseHitPos.x / tileSize) * tileSize;
		var y = Mathf.Floor(mouseHitPos.y / tileSize) * tileSize;

		var row = x / tileSize;
		var column = Mathf.Abs(y / tileSize) - 1;

		if (!mouseOnMap)
			return;

		var id = (int)((column * map.mapSize.x) + row);

		brush.tileID = id;

		x += map.transform.position.x + tileSize / 2;
		y += map.transform.position.y + tileSize / 2;

		brush.transform.position = new Vector3(x, y, map.transform.position.z);
	}

	void Draw() {
		var mapID = map.tileID; //index of tile in texture

		var posX = brush.transform.localPosition.x;
		var posY = brush.transform.localPosition.y;

		GameObject tile = GameObject.Find(map.name + "/Tiles/tile_" + (int)posX + "~" + (int)posY);

		bool newTile = false;

		//create new tile
		if (tile == null) {
			newTile = true;
			tile = PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>("tilePrefab")) as GameObject;
			tile.name = "tile_" +(int) posX + "~" + (int)posY;
			Undo.RegisterCreatedObjectUndo(tile, "create tile"); //allows undo of tile creation
			tile.transform.SetParent(map.tileManager.transform);
			tile.transform.localPosition = new Vector3(posX, posY, map.transform.position.z);
		} else { //tile already created
			Undo.RegisterCompleteObjectUndo(tile, "edit tile"); //save snapshot for undo
		}

		//update sprite
		SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
		renderer.sprite = brush.renderer2D.sprite;

		//change tag if tile is not solid
		if (mapID <= map.solidIndex) { //if tile is solid
			if (tile.tag != "Wall") { //and not tagged as Wall
				tile.tag = "Wall";
				map.tileManager.dirtyColliders = true;
			}
		} else { //if tile is not solid
			if (tile.tag != "Floor") { //and not marked as floor
				tile.tag = "Floor";
				map.tileManager.dirtyColliders = true;
			}
		}

		if (!newTile) {
			EditorUtility.SetDirty(tile); //record changes for undo
		}

	}

	void RemoveTile() {
		GameObject tile = GameObject.Find(map.name + "/Tiles/tile_" + (int)brush.transform.localPosition.x + "~" + (int)brush.transform.localPosition.y);

		if (tile != null) {
			Undo.DestroyObjectImmediate(tile);
		}

		map.tileManager.dirtyColliders = true;
	}

	void ClearMap() {
		for (var i = 0; i < map.tileManager.transform.childCount; i++) {
			Transform t = map.tileManager.transform.GetChild(i);
			DestroyImmediate(t.gameObject);
			i--;
		}
		//map.tileManager.CreateColliders(map.mapSize);
	}
}