using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using System.Collections.Generic;


[RequireComponent(typeof(LineRenderer))]
public class MiniMapPathVisualizer : MonoBehaviour
{
    private LineRenderer line;

    // Target position for the agent
    public Transform targetTransform;
    [SerializeField] private GameObject destinationPrefab;

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }
    Vector3 startPos, endPos;
    public GameObject destination;
    void Update()
    {

        if (targetTransform != null)
        {
            if (destination == null) destination = Instantiate(destinationPrefab, targetTransform.position, destinationPrefab.transform.rotation);
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit startHit, 50f, NavMesh.AllAreas) &&
                NavMesh.SamplePosition(targetTransform.position, out NavMeshHit endHit, 50f, NavMesh.AllAreas))
            {
                startPos = startHit.position;
                endPos = endHit.position;
                NavMeshPath path = new NavMeshPath();
                NavMesh.CalculatePath(startPos, endPos, NavMesh.AllAreas, path);
                line.positionCount = path.corners.Length;
                line.SetPositions(path.corners);
            }

            if (Vector3.Distance(transform.position, endPos) < 20f)
            {
                targetTransform = null; // Reset targetTransform if it's too close to the start position
                if (destination != null)
                {
                    Destroy(destination); // Destroy the destination object if targetTransform is null
                }
            }

        }
        else
        {
            line.positionCount = 0; // Hide line if there's no path
        }
    }
}
