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
		if (map.tiles == null) {
			var go = new GameObject("Tiles");
			go.transform.SetParent(map.transform);
			go.transform.position = Vector3.zero;
			map.tiles = go;
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
		var brushID = brush.tileID.ToString(); //ID of tile under mouse
		var mapID = map.tileID; //index of tile in texture

		var posX = brush.transform.position.x;
		var posY = brush.transform.position.y;

		GameObject tile = GameObject.Find(map.name + "/Tiles/tile_" + brushID);

		bool newTile = false;

		//create new tile
		if (tile == null) {
			newTile = true;
			tile = PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>("tilePrefab")) as GameObject;
			tile.name = "tile_"+brushID;
			Undo.RegisterCreatedObjectUndo(tile, "create tile"); //allows undo of tile creation
			tile.transform.SetParent(map.tiles.transform);
			tile.transform.position = new Vector3(posX, posY, map.transform.position.z);
		} else { //tile already created
			Undo.RegisterCompleteObjectUndo(tile, "edit tile"); //save snapshot for undo
		}

		//update sprite
		SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
		renderer.sprite = brush.renderer2D.sprite;

		//add/remove collider
		BoxCollider2D collider = tile.GetComponent<BoxCollider2D>();
		if (mapID <= map.solidIndex) { //if tile is solid
			if (!collider) { //and has no collider
				collider = tile.AddComponent<BoxCollider2D>();
				Undo.RegisterCreatedObjectUndo(collider, "edit tile"); //allows undo of tile creation
				tile.tag = "Solid";
				CreateColliders();
			}
		} else if (collider) { //tile is not solid & has collider
			Undo.DestroyObjectImmediate(collider); //remove collider
			tile.tag = "Untagged";
		}

		if (!newTile) {
			EditorUtility.SetDirty(tile); //record changes for undo
		}
		
	}

	void RemoveTile() {
		var id = brush.tileID.ToString();

		GameObject tile = GameObject.Find(map.name + "/Tiles/tile_" + id);

		if (tile != null) {
			Undo.DestroyObjectImmediate(tile);
		}
	}

	void ClearMap() {
		for (var i = 0; i < map.tiles.transform.childCount; i++) {
			Transform t = map.tiles.transform.GetChild(i);
			DestroyImmediate(t.gameObject);
			i--;
		}
	}

	void CreateColliders() {
		GameObject.FindGameObjectsWithTag("Solid");
		/*ALGORITHM OF YOUR HEART, ALGORITHM OF THE BEAT
		._._._.
		!_!_!_!
		!_!_!_!
		!_!_!_!

		123
		4X6
		789
		RULE 1: START AT THE FIRST X AT CORNER 1.
		IF 4 IS UNOCCUPIED:
			MOVE TO CORNER 7
			IF 8 IS UNOCCUPIED:
				MOVE TO CORNER 9.
				IF 6 IS UNOCCUPIED:
					MOVE TO CORNER 3
					IF 2 IS UNOCCUPIED:
						GIVE THIS THING A BOX COLLIDER AND REPEAT RULE 1 ON THE NEXT X
					ELSE: start drawing clockwise
				ELSE: start drawing clockwise	
			ELSE: start drawing clockwise
		ELSE:
			start drawing clockwise

		StartDrawingClockwise:
			create a point at current position
			if 
		
		MUST ALSO CHECK FOR EDGE CASES:
		2
		X and 4X6
		8


		123
		4X
		78
		RULE 2: 
		RULE 2: IF positions 2,4,6,8 are empty, make a new path 1,3,9,7
		RULE 2: if !RULE 1, GENERATE POINTS CLOCWISE EXCEPT THE LAST ONE 
		RULE 3:
		XXXX  X XX XXX XX X XXX X
		XX XX X X XX X X XX X X
		X  X X   X  X  X   X  X X
		X   X  X    X   X     X X
		XXXX XX XX XX XXX XXXXX
		
		Starting at X[4,1], where X[1..3,1] have been marked:
		create points 1,3. Recurse on X8.
		X8:
			
				

		CASE:
			
			4X6
			
			
		CASE:
			123
			4X6
			789
			continue;
		CASE:
			 23
			4X6
			789
			continue;
		CASE:
			  3
			4X6
			789
			create a point at 1
		CASE:
			
			4X6
			789
			create a point at 1
		CASE:
			
			 X6
			789
			create points at 7, 1
		CASE:
			
			 X6
			7 9
			create points at 9,7,1
		CASE:
			  3
			 X 
			7  
			create new path 1,3,9,7

		case: tile has no adjacent tiles:
			create a new path at the four corners of the tile
		case: tile has 3 adjacent tiles:
			if corner 1 is occupied:
				place a point at corner
		*/
	}
}
