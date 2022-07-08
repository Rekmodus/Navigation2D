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
		//public bool showPath = true;


		// ~~~~~~~~~~~~~~ADDDED CODE~~~~~~~~~~
		//public bool mouse = false; // Option to navigate to the target instead of the mouse
		//bool canMove = true;


		public GameObject WallMark;
		//public PathMode pathMode = PathMode.vertical;
		//public WalkMode walkMode = WalkMode.smooth;

		public float Radius = 0.4f;

		[Tooltip("Layer non walkable")]
		public LayerMask wallLayer;

		//public Transform agent;

		//public Transform target; // Option to navigate to the target instead of the mouse

		//private Vector3 destPos;

		//List<NodeItem> pathNodes;

		//[Tooltip("Make sense when walk mode is 'smooth', suggest 0 to 10")]
		//public float smoothMoveSpeed = 5f;
		//[Tooltip("Make sense when walk mode is 'step by step'")]
		//public float StepByStepInterval = 0.5f;

		[Tooltip("Position of tilemap's left bottom corner")]
		public Vector2 tilemapStart;
		[Tooltip("Position of tilemap's right top corner")]
		public Vector2 tilemapEnd;

		//walk direction
		//public WalkDirection walkDirec = WalkDirection.idle;

		public NodeItem[,] map;
		[HideInInspector] public int w, h;

		private GameObject WallMarks;
		//private List<GameObject> pathObj = new List<GameObject> ();

		void Awake ()
		{
			WallMarks = new GameObject ("WallMarks");
			//PathMarks = new GameObject ("PathMarks");
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
			//for (int i = 0; i < PathMarks.transform.childCount; i++) {  
			//	PathMarks.transform.GetChild (i).gameObject.SetActive (false);
			//}  
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

			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ADDED THIS CODE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			//OnMouseDown();
		}
		


		/*
		void Update ()
		{
			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ADDED THIS CODE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			if (canMove)
			{
				FindingPath(new Vector2(agent.position.x, agent.position.y), new Vector2(destPos.x, destPos.y));
				if ((destPos != agent.transform.position))
				{
					OnMouseDown();
				}
			}
		}

		public NodeItem getItem (Vector2 position)
		{
			int x = Mathf.FloorToInt (position.x - tilemapStart.x);
			int y = Mathf.FloorToInt (position.y - tilemapStart.y);
			x = Mathf.Clamp (x, 0, w - 1);
			y = Mathf.Clamp (y, 0, h - 1);
			return map [x, y];
		}


		public List<NodeItem> getNeighbourNodes (NodeItem node)
		{
			List<NodeItem> list = new List<NodeItem> ();
			switch (pathMode) {
			case PathMode.diagonal:
				for (int i = -1; i <= 1; i++) {
					for (int j = -1; j <= 1; j++) {
						// skip self
						if (i == 0 && j == 0)
							continue;
						int x = node.x + i;
						int y = node.y + j;
						// check out of bound or not, if not add to map
						if (x < w && x >= 0 && y < h && y >= 0)
							list.Add (map [x, y]);
					}
				}
				break;
			case PathMode.vertical:
				if (node.x + 1 < w)
					list.Add (map [node.x + 1, node.y]);
				if (node.x - 1 >= 0)
					list.Add (map [node.x - 1, node.y]);
				if (node.y + 1 < h)
					list.Add (map [node.x, node.y + 1]);
				if (node.y - 1 >= 0)
					list.Add (map [node.x, node.y - 1]);
				break;
			}


			return list;
		}

		public void updatePath (List<NodeItem> lines)
		{
			int curListSize = pathObj.Count;
			if (PathMark && showPath) {
				for (int i = 0, max = lines.Count; i < max; i++) {
					if (i < curListSize) {
						pathObj [i].transform.position = lines [i].pos + new Vector2 (0.5f, 0.5f);
						pathObj [i].SetActive (true);
					} else {
						GameObject obj = GameObject.Instantiate (PathMark, new Vector3 (lines [i].pos.x + 0.5f, lines [i].pos.y + 0.5f, 0), Quaternion.identity) as GameObject;
						obj.transform.SetParent (PathMarks.transform);
						pathObj.Add (obj);
					}
				}
				for (int i = lines.Count; i < curListSize; i++) {
					pathObj [i].SetActive (false);
				}
			}
			pathNodes = lines;
		}

		void OnMouseUp () // OnMouseUp seems to not call when mouse click is lifted. So calling this in Update might be necessary
		{
			//StopFinding ();

			switch (walkMode) {
			case WalkMode.smooth:
				StartCoroutine (SmoothMove ());
				break;
			case WalkMode.stepByStep:
				StartCoroutine (StepByStepMove ());
				break;
			case WalkMode.blink:
				BlinkMove ();
				break;
			}

		}

		void OnMouseDown () // OnMouseDown seems to not call when mouse is clicked. So calling this in Update might be necessary
		{
			// ~~~~~~~~~~~~~~ ADDED CODE ~~~~~~~~~~~~

			Vector3 world = target.transform.position; //the target
			if (mouse) {
				world = Camera.main.ScreenToWorldPoint(Input.mousePosition); //mouse pointer
			} else{
				world = target.transform.position; //the target
			}

			destPos = world;
			StopAllCoroutines ();

		}


		
		public void moveStart()
        {
			canMove = true;
        }
		public void moveStop()
        {
			canMove = false;
        }

		IEnumerator SmoothMove ()
		{
			for (int i = 0, max = pathNodes.Count; i < max; i++) {
				bool isOver = false;
				while (!isOver && canMove) {
					Vector3 offSet = new Vector3 (pathNodes [i].pos.x + 0.5f, pathNodes [i].pos.y + 0.5f, 0) - agent.position;
					if (offSet.y > 0) {
						walkDirec = WalkDirection.up;
					} else if (offSet.y < 0) {
						walkDirec = WalkDirection.down;
					} else if (offSet.x < 0) {
						walkDirec = WalkDirection.left;
					} else if (offSet.x > 0) {
						walkDirec = WalkDirection.right;
					} else {
						walkDirec = WalkDirection.idle;
					}

					agent.position += offSet.normalized * smoothMoveSpeed * Time.deltaTime;
					if (Vector2.Distance (pathNodes [i].pos + new Vector2 (0.5f, 0.5f), new Vector2 (agent.position.x, agent.position.y)) < 0.1f) {
						isOver = true;
						agent.position = new Vector3 (pathNodes [i].pos.x + 0.5f, pathNodes [i].pos.y + 0.5f, 0);
					}
					yield return new WaitForFixedUpdate ();
				}
			}
			walkDirec = WalkDirection.idle;
		}

		IEnumerator StepByStepMove()
		{
			for (int i = 0, max = pathNodes.Count; i < max; i++) {
				agent.position = new Vector3 (pathNodes [i].pos.x + 0.5f, pathNodes [i].pos.y + 0.5f, 0);
				yield return new WaitForSeconds (StepByStepInterval);
			}
		}

		void BlinkMove ()
		{
			agent.position = new Vector3 (pathNodes [pathNodes.Count - 1].pos.x + 0.5f, pathNodes [pathNodes.Count - 1].pos.y + 0.5f, 0);
		}

		void FindingPath (Vector2 s, Vector2 e)
		{
			NodeItem startNode = getItem (s);
			NodeItem endNode = getItem (e);

			List<NodeItem> openSet = new List<NodeItem> ();
			HashSet<NodeItem> closeSet = new HashSet<NodeItem> ();
			openSet.Add (startNode);

			while (openSet.Count > 0) {
				NodeItem curNode = openSet [0];

				for (int i = 0, max = openSet.Count; i < max; i++) {
					if (openSet [i].costTotal <= curNode.costTotal &&
					   openSet [i].costToEnd < curNode.costToEnd) {
						curNode = openSet [i];
					}
				}

				openSet.Remove (curNode);
				closeSet.Add (curNode);

				// find target node
				if (curNode == endNode) {
					generatePath (startNode, endNode);
					return;
				}

				// select best node in neighbour
				foreach (var item in getNeighbourNodes(curNode)) {
					if (item.isWall || closeSet.Contains (item))
						continue;
					int newCost = curNode.costToStart + getDistanceBetweenNodes (curNode, item);
					if (newCost < item.costToStart || !openSet.Contains (item)) {
						item.costToStart = newCost;
						item.costToEnd = getDistanceBetweenNodes (item, endNode);
						item.parent = curNode;
						if (!openSet.Contains (item)) {
							openSet.Add (item);
						}
					}
				}
			}

			generatePath (startNode, null);
		}

		void generatePath (NodeItem startNode, NodeItem endNode)
		{
			List<NodeItem> path = new List<NodeItem> ();
			if (endNode != null) {
				NodeItem temp = endNode;
				while (temp != startNode) {
					path.Add (temp);
					temp = temp.parent;
				}
				path.Reverse ();
			}
			updatePath (path);

			//~~~~~~~~~~~~~~~~~~ADDED THIS CODE~~~~~~~~~~~
			OnMouseUp();

		}

		int getDistanceBetweenNodes (NodeItem a, NodeItem b)
		{
			int cntX = Mathf.Abs (a.x - b.x);
			int cntY = Mathf.Abs (a.y - b.y);
			if (cntX > cntY) {
				return 14 * cntY + 10 * (cntX - cntY);
			} else {
				return 14 * cntX + 10 * (cntY - cntX);
			}
		}

		//	int getDistanceBetweenNodes(NavGround.NodeItem a, NavGround.NodeItem b) {
		//		int cntX = Mathf.Abs (a.x - b.x);
		//		int cntY = Mathf.Abs (a.y - b.y);
		//		return cntX + cntY;
		//	}

		*/
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