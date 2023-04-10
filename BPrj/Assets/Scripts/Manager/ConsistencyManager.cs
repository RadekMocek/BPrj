using System.Collections.Generic;
using UnityEngine;

public class ConsistencyManager : MonoBehaviour
{
    private readonly string[] consistentTransformParentNames = new string[] { "Triggers", "Items", "Locks" }; //TODO: Enemy consistency

    private Dictionary<string, Dictionary<string, bool>> records;
    private string currentSceneName;

    public void OnSceneChanged(string sceneName)
    {
        currentSceneName = sceneName;

        if (!records.ContainsKey(sceneName)) {
            // Initialize Dictionary for this scene
            records[sceneName] = new Dictionary<string, bool>();

            // Make records
            foreach (string parentName in consistentTransformParentNames) {
                var GO = GameObject.Find(parentName);
                if (GO != null) {
                    foreach (Transform childTransform in GO.transform) {
                        records[sceneName][childTransform.name] = true;
                    }
                }
            }
        }
        else {
            // Consistency
            foreach (string parentName in consistentTransformParentNames) {
                var GO = GameObject.Find(parentName);
                if (GO != null) {
                    foreach (Transform childTransform in GO.transform) {
                        childTransform.gameObject.SetActive(records[sceneName][childTransform.name]);
                    }
                }
            }
        }
        
    }

    public void SetRecord(string childTransformName, bool value)
    {
        if (records.ContainsKey(currentSceneName) && records[currentSceneName].ContainsKey(childTransformName)) {
            records[currentSceneName][childTransformName] = value;
        }
    }

    private void Awake()
    {
        records = new Dictionary<string, Dictionary<string, bool>>();
        currentSceneName = "Outside";
    }
}
