using Bravasoft.Functional;
using Bravasoft.Functional.Unity;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public SerializableOption<string> _maybeString;
    
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
