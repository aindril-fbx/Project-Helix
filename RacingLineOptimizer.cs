using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class RacingLineOptimizer : MonoBehaviour
{
    [Header("Input Path")]
    public Transform[] centerPath;

    [Header("Track Properties")]
    public float roadWidth = 6f;

    [Header("Optimization Settings")]
    public int apexSamples = 5;
    public int iterations = 3;

    [Header("Generated Output")]
    public List<Vector3> racingLine = new List<Vector3>();
    public Transform racingLineParent; // Optional parent object
    public string waypointPrefix = "RacingPoint_";

    public void GenerateRacingLine()
    {
        racingLine.Clear();
        if (centerPath == null || centerPath.Length < 3) return;

        List<Vector3> currentLine = new List<Vector3>();
        foreach (var t in centerPath) currentLine.Add(t.position);

        for (int iter = 0; iter < iterations; iter++)
        {
            List<Vector3> newLine = new List<Vector3> { currentLine[0] };

            for (int i = 1; i < currentLine.Count - 1; i++)
            {
                Vector3 prev = currentLine[i - 1];
                Vector3 curr = currentLine[i];
                Vector3 next = currentLine[i + 1];

                Vector3 forward = (next - prev).normalized;
                Vector3 side = Vector3.Cross(forward, Vector3.up).normalized;

                Vector3 bestPoint = curr;
                float lowestAngle = 180f;

                for (int s = -apexSamples; s <= apexSamples; s++)
                {
                    float offset = (s / (float)apexSamples) * (roadWidth / 2f);
                    Vector3 candidate = curr + side * offset;

                    Vector3 dir1 = (candidate - prev).normalized;
                    Vector3 dir2 = (next - candidate).normalized;
                    float angle = Vector3.Angle(dir1, dir2);

                    if (angle < lowestAngle)
                    {
                        lowestAngle = angle;
                        bestPoint = candidate;
                    }
                }

                newLine.Add(bestPoint);
            }

            newLine.Add(currentLine[currentLine.Count - 1]);
            currentLine = newLine;
        }

        racingLine = currentLine;
        Debug.Log($"Racing line generated with {iterations} refinement passes.");
    }

    public void SpawnRacingLineTransforms()
    {
        if (racingLine == null || racingLine.Count == 0)
        {
            Debug.LogWarning("No racing line data found. Generate it first.");
            return;
        }

        // Delete old children
        if (racingLineParent != null)
        {
            List<Transform> children = new List<Transform>();
            foreach (Transform child in racingLineParent) children.Add(child);
            foreach (Transform child in children) DestroyImmediate(child.gameObject);
        }
        else
        {
            // Create parent if missing
            GameObject parentGO = new GameObject("RacingLine_Container");
            parentGO.transform.SetParent(this.transform);
            racingLineParent = parentGO.transform;
        }

        // Create transforms
        for (int i = 0; i < racingLine.Count; i++)
        {
            GameObject point = new GameObject(waypointPrefix + i.ToString("D3"));
            point.transform.position = racingLine[i];
            point.transform.SetParent(racingLineParent);
        }

        Debug.Log($"Spawned {racingLine.Count} racing line transforms.");
    }

    void OnDrawGizmos()
    {
        if (centerPath != null && centerPath.Length > 1)
        {
            Gizmos.color = Color.gray;
            for (int i = 0; i < centerPath.Length - 1; i++)
            {
                Gizmos.DrawLine(centerPath[i].position, centerPath[i + 1].position);
            }
        }

        if (racingLine != null && racingLine.Count > 1)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < racingLine.Count - 1; i++)
            {
                Gizmos.DrawLine(racingLine[i], racingLine[i + 1]);
            }
        }
    }
}
