using UnityEngine;
using System;

public class AttachScriptComponent : MonoBehaviour
{
    // Public field to input the script name in the Inspector
    public string scriptName;

    void Awake()
    {
        // Check if the scriptName is assigned
        if (!string.IsNullOrEmpty(scriptName))
        {
            // Attach the script component to all child objects
            AttachScriptToChildren(transform);
        }
        else
        {
            Debug.LogError("No script name provided to attach.");
        }
    }

    void AttachScriptToChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // Get the type of the script to attach
            Type scriptType = Type.GetType(scriptName);

            if (scriptType == null)
            {
                Debug.LogError($"Script '{scriptName}' not found. Ensure the script name is correct and includes the namespace.");
                return;
            }

            // Check if the child already has the component
            if (child.gameObject.GetComponent(scriptType) == null)
            {
                // Add the script component dynamically
                child.gameObject.AddComponent(scriptType);
            }

            // Recursively call this method for nested children
            AttachScriptToChildren(child);
        }
    }
}
