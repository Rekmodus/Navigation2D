using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Navigation
{
    public class ChangeTarget : MonoBehaviour
    {
        [SerializeField] Transform[] targets;

        Navigation4TilemapAgent tilemapAgent;

        ChangePathMode changePathMode;

        Transform minTransform;
        float minDistance;

        private void Awake()
        {
            tilemapAgent = GetComponent<Navigation4TilemapAgent>();
            changePathMode = GetComponent<ChangePathMode>();
        }

        private void Start()
        {
            minTransform = targets[0];
            minDistance = Vector2.Distance(transform.position, targets[0].position);
        }
        private void Update()
        {
            minDistance = Vector2.Distance(transform.position, tilemapAgent.target.position);
            foreach (Transform t in targets)
            {
                if (Vector2.Distance(transform.position,t.position) < minDistance)
                {
                    minTransform = t;
                    minDistance = Vector2.Distance(transform.position, t.position);
                    tilemapAgent.target = t;
                    if(changePathMode != null)
                    {
                        changePathMode.target = t;
                    }
                }
            }
        }

    }
}

