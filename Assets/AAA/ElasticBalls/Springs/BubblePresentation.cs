using UnityEngine;
using UnityEngine.Serialization;

public class BubblePresentation : MonoBehaviour
{
    [Header("Bones")] 
    [SerializeField] 
    private Transform _armature;

   [Header("Other Settings")]

    [Tooltip("Strength of spring")] 
    [SerializeField]
    private float _spring = 100f;

    [Tooltip("Higher the value the faster the spring oscillation stops")]
    [SerializeField]
    private float _damper = 0.2f;
    
    [SerializeField]
    private float _rigidbodyMass = 1f;

    private void Start()
    {
        var boneSoftbody = new BubbleAdjuster(_rigidbodyMass, _spring, _damper,
            RigidbodyConstraints.FreezeRotation);

        var root = _armature.GetChild(0).gameObject;
        boneSoftbody.SetRbSettings(ref root, 10f);

        for (var i = 1; i < _armature.childCount; i++)
        {
            var child = _armature.GetChild(i).gameObject;
            boneSoftbody.SetRbSettings(ref child);
            boneSoftbody.SetSpringDefaults(ref child, ref root);
        }
    }
    
    
    private class BubbleAdjuster 
    {
        private readonly float RigidbodyMass;  
        private readonly float Spring;
        private readonly float Damper;
        private readonly RigidbodyConstraints Constraints;

        public BubbleAdjuster(float rigidbodymass, float spring, float damper, RigidbodyConstraints constraints)
        {
            RigidbodyMass = rigidbodymass;
            Spring = spring;
            Damper = damper;
            Constraints = constraints;
        }
        public void SetSpringDefaults(ref GameObject go1, ref GameObject connectedGo)
        {
            SetSpringDefaults(ref go1, ref connectedGo, Spring, Damper);
        }

        public void SetRbSettings(ref GameObject go)
        {
            SetRbSettings(ref go, RigidbodyMass);
        }

        public void SetRbSettings(ref GameObject go, float mass)
        {
            var rb = go.GetComponent<Rigidbody>();
            rb.mass = mass;
            rb.linearDamping = 0f;
            rb.angularDamping = 10f;
            rb.constraints = Constraints;
        }

        private static void SetSpringDefaults(ref GameObject go1, ref GameObject connectedGo, float spring, float damper)
        {
            var sp = go1.GetComponent<SpringJoint>();
            sp.connectedBody = connectedGo.GetComponent<Rigidbody>();
            sp.spring = spring;
            sp.damper = damper;
        }
    }

}