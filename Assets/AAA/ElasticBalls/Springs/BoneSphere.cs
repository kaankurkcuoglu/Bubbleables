using UnityEngine;

public class BoneSphere : MonoBehaviour
{
    [Header("Bones")] 
    [SerializeField] 
    private Transform _armature;

    [Header("Spring Joint Settings")] 
    [Tooltip("Strength of spring")] 
    [SerializeField]
    private float _spring = 100f;

    [Tooltip("Higher the value the faster the spring oscillation stops")]
    [SerializeField]
    private float _damper = 0.2f;

    [SerializeField]
    private float _colliderSize = 0.002f;

    [SerializeField]
    private float _colliderOffset = 0.002f;

    [SerializeField]
    private float _rigidbodyMass = 1f;

    private static int LastGivenLayerNumber;
    
    private void Start()
    {
        var layer = GetNextBubbleDeformerLayer();

        var boneSoftbody = new BoneSoftbody(_colliderSize, _colliderOffset, _rigidbodyMass, _spring, _damper,
            RigidbodyConstraints.FreezeRotation);

        var root = _armature.GetChild(0).gameObject;
        boneSoftbody.AddCollider(ref root, 0.005f, 0f, 10f);

        for (var i = 1; i < _armature.childCount; i++)
        {
            var child = _armature.GetChild(i).gameObject;
            child.layer = layer;
            boneSoftbody.AddCollider(ref child);
            boneSoftbody.AddSpring(ref child, ref root);
        }
    }

    private int GetNextBubbleDeformerLayer()
    {
        var result = LayerMask.NameToLayer("BubbleDeformer" + LastGivenLayerNumber);
        LastGivenLayerNumber++;
        if (LastGivenLayerNumber >= 12)
            LastGivenLayerNumber = 0;
        return result;
    }
}