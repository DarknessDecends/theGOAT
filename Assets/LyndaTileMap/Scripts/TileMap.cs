using UnityEngine;
using System.Collections;

public class TileMap : MonoBehaviour {

	public Vector2 mapSize = new Vector2(20, 10); //map size in tiles
	public Vector2 gridSize = new Vector2(); //map size in world units

	public Texture2D tileSheet; //this is the tileset from the inspector
	public Object[] spriteReferences;

	public Vector2 tileSize = new Vector2();
	public int pixelsToUnits = 100;
	public int tileID = 0;

	public new SpriteRenderer renderer;

	public Sprite currentTileBrush {
		get { return spriteReferences[tileID] as Sprite; }
	}

	//draws grid once a frame in inspector
	void OnDrawGizmosSelected() {
		var pos = transform.position;

		if (tileSheet != null) {
			Gizmos.color = Color.gray;
			var row = 0;
			var maxColumns = mapSize.x;
			var total = mapSize.x * mapSize.y;
			var tile = new Vector3(tileSize.x / pixelsToUnits, tileSize.y / pixelsToUnits);
			var offset = new Vector2(tile.x / 2, tile.y / 2);

			for (var i = 0; i < total; i++) {

				var column = i % maxColumns;

				var newX = (column * tile.x) + offset.x + pos.x;
				var newY = -(row * tile.y) - offset.y + pos.y;

				Gizmos.DrawWireCube(new Vector2(newX, newY), tile);

				if (column == maxColumns - 1) {
					row++;
				}
			}

			Gizmos.color = Color.white;
			var centerX = pos.x + (gridSize.x / 2);
			var centerY = pos.y - (gridSize.y / 2);

			Gizmos.DrawWireCube(new Vector2(centerX, centerY), gridSize);
		}
	}

	public void Create() {
		if (tileSheet != null) {
			// (number of tiles)*(size of each tile)
			int pixelWidth = (int)(mapSize.x*tileSize.x);
			int pixelHeight = (int)(mapSize.y*tileSize.y);

			//create new empty texture
			Texture2D texture = new Texture2D(pixelWidth, pixelHeight, TextureFormat.ARGB32, false);
			texture.filterMode = FilterMode.Point; //prevents blurring
			texture.wrapMode = TextureWrapMode.Clamp;

			renderer = this.GetComponent<SpriteRenderer>();

			//create new renderer or recreate old texture
			if (renderer == null) {
				renderer = this.gameObject.AddComponent<SpriteRenderer>(); //create the new renderer				
			} else { //keep pixels of the old texture
				Texture2D oldTexture = renderer.sprite.texture;

				int width = Mathf.Clamp(oldTexture.width, 0, texture.width);
				int height = Mathf.Clamp(oldTexture.height, 0, texture.height);

				Color[] colors = oldTexture.GetPixels(0, 0, width, height);

				texture.SetPixels(0, 0, width, height, colors);
				texture.Apply();
			}

			//create the new tilemap sprite
			Rect rect = new Rect(0, 0, pixelWidth, pixelHeight); //create new rectangle (same size)
			Sprite sprite = Sprite.Create(texture, rect, new Vector2(0, 1), pixelsToUnits); //set pivot to Upper-left corner and scale by pixel size
			sprite.name = "map";

			renderer.sprite = sprite;
		}
	}

}