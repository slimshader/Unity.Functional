using Bravasoft.Functional;
using System;
using UnityEngine;

[Serializable]
public class SerializedStringOption : SerializedOption<string>
{ }

public class NewBehaviourScript : MonoBehaviour
{
    public SerializedOption<string> _maybeString;
    
    // Start is called before the first frame update
    void Start()
    {
        _maybeString.Option.Iter(x =>
        {
            Debug.Log($"Maybe string {x}");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
