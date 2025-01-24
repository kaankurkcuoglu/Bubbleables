using UnityEngine;

/*
[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsInitializeGroup)), UpdateBefore(typeof(PhysicsSimulationGroup))]
public partial struct ElasticBallPhysics : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var world = SystemAPI.GetSingletonRW<PhysicsWorldSingleton>().ValueRW.PhysicsWorld;
        state.EntityManager.CompleteDependencyBeforeRW<PhysicsWorldSingleton>();

        var localTransform = SystemAPI.GetComponent<LocalTransform>();

        var hit = world.CastRay(raycastInput, out var rayResult);
    }
}

*/


public class ElasticBallPhysics : MonoBehaviour
{
    private Mesh Mesh;
    private Vector3[] OriginalVertices;
    private float[] OriginalVertexDistances;
    private Vector3[] DeformedVertices;
    private bool[] SquishedVertices;
    private float MaxDistance;

    void Awake()
    {
        Mesh = GetComponent<MeshFilter>().mesh; // Get a copy of the original mesh
        GetComponent<MeshFilter>().sharedMesh = Mesh; // Assign the copied instance
        
        OriginalVertices = Mesh.vertices;
        OriginalVertexDistances = new float[OriginalVertices.Length];
        SquishedVertices = new bool[OriginalVertices.Length];
        DeformedVertices = Mesh.vertices;

        MaxDistance = 0f;
        var scale = transform.localScale.x; // Assuming the scale is equal in all axes.
        for (var i = 0; i < OriginalVertices.Length; i++)
        {
            var distance = OriginalVertices[i].magnitude * scale;
            OriginalVertexDistances[i] = distance;
            if (distance > MaxDistance)
            {
                MaxDistance = distance;
            }
        }
    }

    void FixedUpdate()
    {
        double totalSquish = 0;
        int squishCount = 0;
        var position = transform.position;
        for (int i = 0; i < OriginalVertices.Length; i++)
        {
            var isHit = Physics.Raycast(position - OriginalVertices[i], OriginalVertices[i] * 2, out RaycastHit hit, OriginalVertexDistances[i] * 2);
            SquishedVertices[i] = isHit;
            if (isHit)
            {
                var hitDistance = hit.distance - OriginalVertexDistances[i] - 0.01f;
                totalSquish += OriginalVertexDistances[i] - hitDistance;
                squishCount++;
                DeformedVertices[i] = (hitDistance / OriginalVertexDistances[i]) * OriginalVertices[i];
            }
            // Debug.DrawLine(position, position + hit.point, Color.red);
        }
        
        // Apply additional squish effect
        var totalNonSquishedCount = OriginalVertices.Length - squishCount;
        float additionalInflate = (float)(totalSquish / totalNonSquishedCount);
        for (int i = 0; i < OriginalVertices.Length; i++)
        {
            if (!SquishedVertices[i])
            {
                DeformedVertices[i] = ((additionalInflate + OriginalVertexDistances[i]) / OriginalVertexDistances[i]) * OriginalVertices[i];
            }
        }

        Mesh.vertices = DeformedVertices;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        // Gizmos.color = Color.yellow;
        // Gizmos.DrawSphere(transform.position, MaxDistance);
        Gizmos.color = Color.green;
        var position = transform.position;
        foreach (var vertex in DeformedVertices)
        {
            Gizmos.DrawSphere(vertex + position, 0.01f);
        }
    }
}