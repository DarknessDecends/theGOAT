using UnityEngine;
using System.Collections.Generic;


/**
 * 1.	place bug on start tile
 * 2.	record your starting position
 * 3.	rewind bug counterclockwise until there is a tile to its left
 * loop:
 * 1.	if position == starting position:
 *			exit
 * 2.	if bug is facing right and upper tile is empty:
 *			mark this tile
 * 3.	drop a point at botom left corner
 * 4.	if front tile is occupied:
 *			rotate -90 degrees clockwise and move to 1
 *		else if 2 is occupied:
 *	 		move to 2
 * 		else:
 * 			rotate 90 degrees clockwise, drop a point, and run this algorithm again.
 * 			if the bug rotates 4 times, terminate this path and find a new one.
 */
public class Bug {
	private bool debug = false;

	public TileManager tileManager; //the tileManager to which this bug belongs

	private Direction prevTurn = Direction.left;
	private int infLoopCounter = 0;

	private int x {
		get { return _x; }
		set {
			if (value != _x) {
				infLoopCounter = 0;
				_x = value;
			}
		}
	}
	private int _x;

	private int y {
		get { return _y; }
		set {
			if (value != _y) {
				infLoopCounter = 0;
				_y = value;
			}
		}
	}
	private int _y;

	private enum Direction { right = 0, down = 1, left = 2, up = 3 }
	private Direction facing = Direction.right;
	private Vector2 front { //position of tile in front of bug
		get {
			switch (facing) {
				case Direction.right:
					return new Vector2(x+1, y);
				case Direction.down:
					return new Vector2(x, y+1);
				case Direction.left:
					return new Vector2(x-1, y);
				case Direction.up:
					return new Vector2(x, y-1);
				default:
					UnityEngine.Debug.LogError("invalid direction");
					return new Vector2(x, y);
			}
		}
	}
	private Vector2 frontLeft { //position of tile to left of bug
		get {
			switch (facing) {
				case Direction.right:
					return new Vector2(x+1, y-1);
				case Direction.down:
					return new Vector2(x+1, y+1);
				case Direction.left:
					return new Vector2(x-1, y+1);
				case Direction.up:
					return new Vector2(x-1, y-1);
				default:
					UnityEngine.Debug.LogError("invalid direction");
					return new Vector2(x, y);
			}
		}
	}
	private Vector2 dropPoint { //where the bug should drop new path points
		get {
			switch (facing) {
				case Direction.right:
					return new Vector2(x, -y); //-y because our y axis is flipped
				case Direction.down:
					return new Vector2(x+1, -y);
				case Direction.left:
					return new Vector2(x+1, -y-1);
				case Direction.up:
					return new Vector2(x, -y-1);
				default:
					UnityEngine.Debug.LogError("invalid direction");
					return new Vector2(x, -y);
			}
		}
	}

	public Bug(TileManager manager) {
		tileManager = manager;
	}

	private void turnLeft() {
		if (prevTurn == Direction.left) {
			infLoopCounter++;
		} else {
			infLoopCounter = 0;
		}

		facing = (Direction)((((int)facing-1)%4 + 4)%4); //adding 4 to facing will put it in the positive range without changing its value modulo 4
	}
	private void turnRight() {

		if (prevTurn == Direction.right) {
			infLoopCounter++;
		} else {
			infLoopCounter = 0;
		}

		facing = (Direction)((((int)facing)+1) % 4);
	}

	public void start(int newX, int newY) {
		x = newX;
		y = newY;

		//check edges in this order (where T is our current tile):
		//  4
		// 1T3
		//  2
		if (tileManager.occupied(x-1, y) == false) { //edge 1 is empty
			turnLeft(); //turn 90 degrees counter-clockwise

			if (tileManager.occupied(x, y+1) == false) { //edge 2 is empty
				turnLeft();

				if (tileManager.occupied(x+1, y) == false) { //edge 3 is empty
					turnLeft();

					if (tileManager.occupied(x, y-1) == false) { //edge 4 is empty
						//tile is singular, give it a square collider
						tileManager.mark(x, y);
						if (debug) {
							UnityEngine.Debug.Log("created box collider");
						}
						tileManager.addColliderPath(new Vector2[] { new Vector2(x+1, -y-1), new Vector2(x, -y-1), new Vector2(x, -y), new Vector2(x+1, -y) });
						return;
					}
				}
			}
		}

		Vector2 startPoint = new Vector2(dropPoint.x, dropPoint.y); //so that we know when we have completed the path

		List<Vector2> path = new List<Vector2>();

		tryMove(path); //first move ensures current dropPoint != startPoint

		while (!(dropPoint.x == startPoint.x && dropPoint.y == startPoint.y)) { //while we have not yet reached our starting point
			tryMove(path);

			if (infLoopCounter >= 4) {
				UnityEngine.Debug.LogError("infinite loop");
				return;
			}

			if (debug) {
				UnityEngine.Debug.Log("moved to ("+x+", "+y+")");
			}
		}

		tileManager.mark(x, y); //edge case. Make sure final tile is marked to avoid repeats

		tileManager.addColliderPath(path.ToArray());

		if (debug) {
			string debugString = "Finished! path = [";
			foreach (Vector2 vec in path) {
				debugString += vec+", ";
			}
			UnityEngine.Debug.Log(debugString+"]");
		}
	}

	private void tryMove(List<Vector2> path) {

		if (facing == Direction.right && tileManager.occupied(x, y-1) == false) { //if bug is facing right and upper tile is empty
			tileManager.mark(x, y); //mark this tile
		}

		if (tileManager.occupied((int)front.x, (int)front.y)) { //if forward tile is occupied
			if (tileManager.occupied((int)frontLeft.x, (int)frontLeft.y)) { //if forward-left corner is occupied
				x = (int)frontLeft.x;
				y = (int)frontLeft.y;

				turnLeft();
				path.Add(dropPoint); //drop a point at the back-left corner

			} else {
				x = (int)front.x;
				y = (int)front.y;

			}
		} else { //check other 6 corners
			turnRight();
			path.Add(dropPoint); //drop a point at the back-left corner
		}
	}
}