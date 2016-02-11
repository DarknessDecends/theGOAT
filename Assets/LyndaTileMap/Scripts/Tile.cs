using UnityEngine;
using System;
using System.Collections;
using UnityEditor;

[Serializable]
public class Tile : ScriptableObject {
	public Sprite sprite {
		get { return _sprite; }
		set {
			_sprite = value; //set new sprite
			var rect = _sprite.rect; //get pixels works this way
			pixels = _sprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
		}
	}
	private Sprite _sprite;
	public Color[] pixels;
	public bool solid;
	public bool applyRandRotation;
	public Sprite altSprite;

	public Color[] getRotatedPixels() {
		int size = sprite.texture.width;
		Color[] rotated = new Color[size*size];
		for (int y = 0; y < size; y++) { //rows
			for (var x = 0; x < size; x++) { //cols
				pixels[x + y*size] = rotated[y + x*size];
			}
		}
		return rotated;
	}
}

