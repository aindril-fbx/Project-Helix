using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RacingLineOptimizer))]
public class RacingLineOptimizerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RacingLineOptimizer optimizer = (RacingLineOptimizer)target;
        if (GUILayout.Button("Generate Optimized Racing Line"))
        {
            optimizer.GenerateRacingLine();
        }

        if (GUILayout.Button("Spawn Racing Line Transforms"))
        {
            optimizer.SpawnRacingLineTransforms();
        }
    }
}
