using Bravasoft.Functional;
using Bravasoft.Functional.Unity;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public SerializableOption<string> _maybeString;
    public Transform _handle;

    [Range(0f, 1f)]
    public float _value = 0f;
    private Behavior<Vector2> _pos;

    void Start()
    {
        _maybeString.Option.Iter(x =>
        {
            Debug.Log($"Maybe string {x}");
        });

        _pos = from t in Behavior.Time()
               let phase = 2 * Mathf.PI * t
               select new Vector2(Mathf.Sin(phase), Mathf.Cos(phase));
    }

    // Update is called once per frame
    void Update()
    {
        _handle.position = _pos.Run(_value);
    }
}
