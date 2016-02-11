using UnityEngine;
using System.Collections;

public class TileBrush : MonoBehaviour {

	public Vector2 brushSize = Vector2.zero;
	public Vector2 tileXY = Vector2.zero;

	public new SpriteRenderer renderer;

	private TileMap map;

	//create brush and attach to object passed in
	public static TileBrush CreateChildOf(Transform parent) {
		GameObject brushObj = new GameObject("brush");

		//we are assuming TileMap editor passed in map
		brushObj.transform.SetParent(parent);

		//create the TileBrush to be returned
		TileBrush brush = brushObj.AddComponent<TileBrush>();

		brush.map = brushObj.transform.parent.GetComponent<TileMap>(); //get a reference to map

		brush.renderer = brushObj.AddComponent<SpriteRenderer>();
		brush.renderer.sortingOrder = 1000;

		Sprite sprite = brush.map.currentTileBrush;
		int pixelsToUnits = brush.map.pixelsToUnits;

		brush.brushSize = new Vector2(sprite.textureRect.width / pixelsToUnits,
									  sprite.textureRect.height / pixelsToUnits);

		brushObj.transform.position = new Vector2(brush.brushSize.x/2, -brush.brushSize.y/2);
		brush.setSprite(sprite);
		return brush;
	}

	//relocate brush position
	public void Move(Vector3 mouseHitPos, bool mouseOnMap) {
		getMap();

		var tileSize = map.tileSize.x / map.pixelsToUnits;

		var x = Mathf.Floor(mouseHitPos.x / tileSize) * tileSize;
		var y = Mathf.Floor(mouseHitPos.y / tileSize) * tileSize;

		if (!mouseOnMap)
			return;

		tileXY = new Vector2(x, y);

		x += map.transform.position.x + tileSize / 2;
		y += map.transform.position.y + tileSize / 2;

		this.transform.position = new Vector3(x, y, map.transform.position.z);
	}

	void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, brushSize);
	}

	void getMap() {
		if (map == null) {
			map = transform.parent.GetComponent<TileMap>();
		}
	}

	public void setSprite(Sprite sprite) {
		renderer.sprite = sprite;
	}
}
