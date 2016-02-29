using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class TileManager : MonoBehaviour {
	public bool dirtyColliders = false;
	bool debug = false;

	public struct Tile {
		public bool occupied;
		public bool marked;
	}

	private new PolygonCollider2D collider {
		get {
			if (_collider == null) {
				_collider = GetComponent<PolygonCollider2D>();
			}
			return _collider;
		}
	}
	private PolygonCollider2D _collider;

	private Tile[,] grid; //a 2D array representing the tilemap

	public void CreateColliders(Vector2 mapSize) {
		if (!dirtyColliders) {
			return;
		}
		dirtyColliders = false;

		collider.pathCount = 0;

		List<Transform> solids = new List<Transform>();
		foreach (Transform child in transform) {
			if (child.gameObject.tag == "Wall") {
				solids.Add(child);
			}
		}

		Vector2[] tiles = new Vector2[solids.Count];
		grid = new Tile[(int)mapSize.x, (int)mapSize.y];

		for (int i = 0; i < solids.Count; i++) {
			tiles[i] = solids[i].transform.localPosition;
			tiles[i].y = -tiles[i].y; //flip negative y values
			grid[(int)tiles[i].x, (int)tiles[i].y].occupied = true;
		}

		//find tiles that form walls

		foreach (Vector2 tile in tiles) {
			//if its upper tile is unoccupied:
			//if it is unmarked:
			//start the bug algorithm
			if (marked((int)tile.x, (int)tile.y) == false && occupied((int)tile.x, (int)tile.y-1) == false) { //if bug is facing right and upper tile is empty
				BugAlgorithm((int)tile.x, (int)tile.y);
			}
		}

		if (debug) {
			string debugString = "";
			for (int y = 0; y < grid.GetLength(1); y++) {
				for (int x = 0; x < grid.GetLength(0); x++) {
					if (grid[x, y].occupied) {
						if (grid[x, y].marked) {
							debugString += "M";
						} else {
							debugString += "T";
						}
					} else {
						debugString += "X";
					}
					debugString += ", ";
				}
				debugString += "\n";
			}
			UnityEngine.Debug.Log(debugString);
		}
	}

	//Bug, Direction, and getTile() are all used by the Bug class

	public bool occupied(int x, int y) {
		if (x < 0 || y < 0 || x >= grid.GetLength(0) || y >= grid.GetLength(1)) {
			return false;
		} else {
			return grid[x, y].occupied;
		}
	}

	public bool marked(int x, int y) {
		if (occupied(x, y)) {
			return grid[x, y].marked;
		} else {
			return false;
		}
	}

	public void mark(int x, int y) {
		if(occupied(x, y)) {
			grid[x, y].marked = true;
		}
	}

	public void addColliderPath(Vector2[] path) {
		collider.pathCount++;
		collider.SetPath(collider.pathCount-1, path);
	}

	void BugAlgorithm(int x, int y) {
		Bug bug = new Bug(this);

		if (marked(x, y) == false && occupied(x, y-1) == false) { //if this tile is marked OR if the tile above is occupied:
			bug.start(x, y);
		}
	}


	public void varyFloorSprites() {
		int i = 0;
		foreach(Transform child in transform) {
			if (child.gameObject.tag == "Floor") {
				i++;
				child.localRotation = Quaternion.Euler(0, 0, 90*((int)(Random.value*4)));
			}
		}
		Debug.Log(i);
	}


	static void SortChildren(GameObject gameObject) {
		// Get selected transforms.
		Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();

		int newIndex = GetLowestIndex(transforms);

		// Sort the transforms by number.
		foreach (Transform t in transforms.Cast<Transform>().OrderBy(t => findInt(t.name))) {
			if (t != t.root) {
				t.SetSiblingIndex(newIndex);
				newIndex++;
			}
		}

		UnityEngine.Debug.Log("Children Sorted.");
	}

	static int GetLowestIndex(Transform[] t) {
		int lowestIndex = 9999;
		int index;

		for (int i = 0; i < t.Length; i++) {
			index = t[i].GetSiblingIndex();

			if (index < lowestIndex) {
				lowestIndex = index;
			}
		}

		return lowestIndex;
	}

	static int findInt(string name) {
		int number;
		int.TryParse(Regex.Match(name, @"\d+").Value, out number);
		return number;
	}

}
