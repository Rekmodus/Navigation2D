/**
 * Author: Chris Zhu 
 * Created Data: 01/01/2018
 * A solution for navigation with Unity3D tilemap
*/

/*
 * Rekmodus: I edited some stuff
 * */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Navigation
{
	public class NodeItem
	{
		// walkable
		public bool isWall;
		// node position
		public Vector2 pos;
		// position in node array
		public int x, y;

		// distance to start
		public int costToStart;
		// distance to end
		public int costToEnd;

		// total distance
		public int costTotal
		{
			get { return costToStart + costToEnd; }
		}

		// parent node
		public NodeItem parent;

		public NodeItem(bool isWall, Vector2 pos, int x, int y)
		{
			this.isWall = isWall;
			this.pos = pos;
			this.x = x;
			this.y = y;
		}
	}

	public class Navigation4Tilemap : MonoBehaviour
	{
		public bool showWallMark = true;

		public GameObject WallMark;

		public float Radius = 0.4f;

		[Tooltip("Layer non walkable")]
		public LayerMask wallLayer;

		[Tooltip("Position of tilemap's left bottom corner")]
		public Vector2 tilemapStart;
		[Tooltip("Position of tilemap's right top corner")]
		public Vector2 tilemapEnd;

		public NodeItem[,] map;
		[HideInInspector] public int w, h;

		private GameObject WallMarks;

		void Awake ()
		{
			WallMarks = new GameObject ("WallMarks");
			initNavigationMap ();
		}

		// Drawing the bounds for easier editing
		void OnDrawGizmos()
		{
			float width = tilemapEnd.x - tilemapStart.x;
			float height = tilemapEnd.y - tilemapStart.y;	

			Gizmos.color = Color.yellow;
			float wHalf = (width * .5f);
			float hHalf = (height * .5f);
			Vector3 topLeftCorner = new Vector3(transform.position.x - wHalf, transform.position.y + hHalf, 1f);
			Vector3 topRightCorner = new Vector3(transform.position.x + wHalf, transform.position.y + hHalf, 1f);
			Vector3 bottomLeftCorner = new Vector3(transform.position.x - wHalf, transform.position.y - hHalf, 1f);
			Vector3 bottomRightCorner = new Vector3(transform.position.x + wHalf, transform.position.y - hHalf, 1f);
			Gizmos.DrawLine(topLeftCorner, topRightCorner);
			Gizmos.DrawLine(topRightCorner, bottomRightCorner);
			Gizmos.DrawLine(bottomRightCorner, bottomLeftCorner);
			Gizmos.DrawLine(bottomLeftCorner, topLeftCorner);
		}

		/**
	 * initiate navigation map
	*/
		public void initNavigationMap ()
		{
			for (int i = 0; i < WallMarks.transform.childCount; i++) {  
				Destroy (WallMarks.transform.GetChild (i).gameObject);  
			}  

			w = Mathf.RoundToInt (tilemapEnd.x - tilemapStart.x + 1);
			h = Mathf.RoundToInt (tilemapEnd.y - tilemapStart.y + 1);
			map = new NodeItem[w, h];

			// write unwalkable node 
			for (int x = 0; x < w; x++) {
				for (int y = 0; y < h; y++) {
					Vector2 pos = new Vector2 (tilemapStart.x + x, tilemapStart.y + y);
					// check walkable or not
					bool isWall = Physics2D.OverlapCircle (pos + new Vector2 (0.5f, 0.5f), Radius, wallLayer);
					// new a node
					map [x, y] = new NodeItem (isWall, pos, x, y);
					// mark unwalkable node
					if (isWall && showWallMark && WallMark) {
						GameObject obj = Instantiate (WallMark, new Vector3 (pos.x + 0.5f, pos.y + 0.5f, 0), Quaternion.identity);
						obj.transform.SetParent (WallMarks.transform);
					}
				}
			}

			Actions.OnMapInit();
		}
	}
	

	public enum PathMode {
		diagonal,
		vertical
	}

	public enum WalkMode {
		stepByStep,
		smooth,
		blink
	}

	public enum WalkDirection{
		idle,
		up,
		down,
		left,
		right
	}
}