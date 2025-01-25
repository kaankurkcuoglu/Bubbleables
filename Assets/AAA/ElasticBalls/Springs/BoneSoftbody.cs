using UnityEngine;

public class BoneSoftbody 
{
    #region --- helpers ---
    public enum ColliderShape
    {
        Box,
        Sphere,
    }
    #endregion

    private float ColliderSize;
    private float ColliderOffset;
    private float RigidbodyMass;    
    private float Spring;
    private float Damper;
    private RigidbodyConstraints Constraints;

    public BoneSoftbody(float collidersize, float collideroffset, float rigidbodymass, float spring, float damper, RigidbodyConstraints constraints)
    {
        ColliderSize = collidersize;
        ColliderOffset = collideroffset;
        RigidbodyMass = rigidbodymass;
        Spring = spring;
        Damper = damper;
        Constraints = constraints;
    }
    public Rigidbody AddCollider(ref GameObject go)
    {
        return AddCollider(ref go, ColliderSize, ColliderOffset, RigidbodyMass);        
    }
    public SpringJoint AddSpring(ref GameObject go1, ref GameObject go2)
    {
        var sp = AddSpring(ref go1, ref go2, Spring, Damper);

        return sp;
    }

    public Rigidbody AddCollider(ref GameObject go, float size, float offset, float mass)
    {
        var sc = go.AddComponent<SphereCollider>();
        sc.radius = size;
        sc.center = new Vector3(0f, offset, 0f);

        var rb = go.AddComponent<Rigidbody>();
        rb.mass = mass;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.constraints = Constraints;
        return rb;
    }
    private static SpringJoint AddSpring(ref GameObject go1, ref GameObject go2, float spring, float damper)
    {
        var sp = go1.AddComponent<SpringJoint>();
        sp.connectedBody = go2.GetComponent<Rigidbody>();
        sp.spring = spring;
        sp.damper = damper;
        // sp.enablePreprocessing = true;
        // sp.minDistance = 0f;
        // sp.maxDistance = 100f;
        return sp;
    }
}
