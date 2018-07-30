using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles the drawing of car paths that the 
// car will follow (shown in editor only).
public class CarPathDrawer : MonoBehaviour
{

    public Color lineColor;

    private List<Transform> nodes;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = lineColor;

        Transform[] pathTransforms = GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        // Remove self from transform array.
        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }

        // Draw lines between all nodes on the path.
        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 currNodePos = nodes[i].position;
            Vector3 prevNodePos = Vector3.zero;
            if (i > 0)
            {
                prevNodePos = nodes[i - 1].position;
            }
            else if (i == 0 && nodes.Count > 1)
            {
                prevNodePos = nodes[nodes.Count - 1].position;
            }

            Gizmos.DrawLine(prevNodePos, currNodePos);
            Gizmos.DrawWireSphere(currNodePos, 0.3f);
        }

    }
}
