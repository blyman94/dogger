using UnityEngine;

namespace Dogger.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Mover : MonoBehaviour
    {
        public Rigidbody Rb { get; set; }

        /// <summary>
        /// Position to move to using force application feedback control loop.
        /// </summary>
        [HideInInspector]
        public Vector3 TargetPos;

        /// <summary>
        /// Maximum X velocity.
        /// </summary>
        public float MaxVel = 3.0f;

        /// <summary>
        /// Factor to convert distance delta to velocity.
        /// </summary>
        protected float toVel = 2.5f;

        /// <summary>
        /// Maximum force to apply to get dog to max velocity.
        /// </summary>
        protected float maxForce = 20.0f;

        /// <summary>
        /// Feedback amount to dictate accuracy of stop behavior.
        /// </summary>
        protected float gain = 5.0f;

        protected virtual void Awake()
        {
            Rb = GetComponent<Rigidbody>();
            Rb.useGravity = false;
            Rb.constraints = RigidbodyConstraints.FreezeRotation;

            TargetPos = transform.position;
        }

        protected virtual void FixedUpdate()
        {
            ApplyMoveForceX();
        }

        /// <summary>
        /// Applies force to the object to move it along the x axis. Adds more
        /// force depending on how far the object is from the target position.
        /// Allows the object to stop at the target position using a feedback 
        /// control loop.
        /// </summary>
        private void ApplyMoveForceX()
        {
            Vector3 dist = TargetPos - transform.position;
            dist.y = 0;
            dist.z = 0;
            Vector3 targetVel = Vector3.ClampMagnitude(toVel * dist, MaxVel);
            Vector3 error = targetVel - Rb.velocity;
            Vector3 force = Vector3.ClampMagnitude(gain * error, maxForce);
            Rb.AddForce(force);
        }
    }
}