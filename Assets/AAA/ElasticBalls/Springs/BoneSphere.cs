using UnityEngine;
using UnityEngine.Serialization;

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

   [Header("Other Settings")]
   [SerializeField]
    private BoneSoftbody.ColliderShape _colliderShape = BoneSoftbody.ColliderShape.Box;

    [SerializeField]
    private float _colliderSize = 0.002f;

    [SerializeField]
    private float _rigidbodyMass = 1f;

    private void Start()
    {
        var boneSoftbody = new BoneSoftbody(_colliderShape, _colliderSize, _rigidbodyMass, _spring, _damper,
            RigidbodyConstraints.FreezeRotation);

        var root = _armature.GetChild(0).gameObject;
        boneSoftbody.AddCollider(ref root, BoneSoftbody.ColliderShape.Sphere, 0.005f, 10f);

        for (var i = 1; i < _armature.childCount; i++)
        {
            var child = _armature.GetChild(i).gameObject;
            boneSoftbody.AddCollider(ref child);
            boneSoftbody.AddSpring(ref child, ref root);
        }
    }
}