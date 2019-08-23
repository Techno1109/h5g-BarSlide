using Unity.Entities;
using Unity.Mathematics;
namespace RigidBodySystems
{
    public struct RigidBody : IComponentData
    {
        public bool IsActive;
        public bool UseGravity;
        public float Drag;
        public float3 Velocity;
        public bool3 ActiveVec;

        public void AddForce(float3 AddVec)
        {
            Velocity += AddVec;
        }
    }
}