using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Navigation
{
	[RequireComponent(typeof(Navigation4TilemapAgent))]
	public class ChangePathMode : MonoBehaviour
	{
		// Start is called before the first frame update

		Navigation4TilemapAgent m_Agent;
		[HideInInspector] public Transform target;
		[Tooltip("Layer non walkable")]
		[SerializeField] private LayerMask wallLayer; // This could be assigned from the Navigation4Tilemap in the scene

		private void Awake()
		{
			m_Agent = GetComponent<Navigation4TilemapAgent>();
			target = m_Agent.target;
		}


		private void Update()
		{
			drawRaycast();
		}
		void drawRaycast()
		{

			Vector3 dir = target.transform.position - transform.position;
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

			GameObject tempObject = new GameObject();
			Transform tempTransform = tempObject.transform;
			tempTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			Destroy(tempObject);

			float distance = Vector3.Distance(transform.position, target.transform.position);

			RaycastHit2D hitWall = Physics2D.Raycast(transform.position, tempTransform.right, distance, wallLayer);

			if (hitWall.collider != null)
			{
				Debug.DrawRay(transform.position, tempTransform.right * distance, Color.yellow);
				if (m_Agent.pathMode == PathMode.diagonal)
				{
					Debug.Log("Change Path to vertical");
					m_Agent.pathMode = PathMode.vertical;
				}
			}
			else
			{
				Debug.DrawRay(transform.position, tempTransform.right * distance, Color.white);
				if (m_Agent.pathMode == PathMode.vertical)
				{
					Debug.Log("Change Path to diagonal");
					m_Agent.pathMode = PathMode.diagonal;
				}
			}
		}
	}

}
	