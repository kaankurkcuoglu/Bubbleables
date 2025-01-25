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

    private ColliderShape Shape;
    private float ColliderSize;
    private float RigidbodyMass;    
    private float Spring;
    private float Damper;
    private RigidbodyConstraints Constraints;

    public BoneSoftbody(ColliderShape shape, float collidersize, float rigidbodymass, float spring, float damper, RigidbodyConstraints constraints)
    {
        Shape = shape;
        ColliderSize = collidersize;
        RigidbodyMass = rigidbodymass;
        Spring = spring;
        Damper = damper;
        Constraints = constraints;
    }
    public Rigidbody AddCollider(ref GameObject go)
    {
        return AddCollider(ref go, Shape, ColliderSize, RigidbodyMass);        
    }
    public SpringJoint AddSpring(ref GameObject go1, ref GameObject go2)
    {
        var sp = AddSpring(ref go1, ref go2, Spring, Damper);

        return sp;
    }

    public Rigidbody AddCollider(ref GameObject go, ColliderShape shape, float size, float mass)
    {
        switch (shape)
        {
            case ColliderShape.Box:
                var bc = go.AddComponent<BoxCollider>();
                bc.size = new Vector3(size, size, size);
                break;
            case ColliderShape.Sphere:
                var sc = go.AddComponent<SphereCollider>();
                sc.radius = size;
                break;
        }

        var rb = go.AddComponent<Rigidbody>();
        rb.mass = mass;
        rb.linearDamping = 0f;
        rb.angularDamping = 10f;
        rb.constraints = Constraints;
        return rb;
    }
    private static SpringJoint AddSpring(ref GameObject go1, ref GameObject go2, float spring, float damper)
    {
        var sp = go1.AddComponent<SpringJoint>();
        sp.connectedBody = go2.GetComponent<Rigidbody>();
        sp.spring = spring;
        sp.damper = damper;
        return sp;
    }
}
