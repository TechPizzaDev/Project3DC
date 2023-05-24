using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ProceduralRooms
{
    public class EntranceTrigger : MonoBehaviour
    {
        private Collider caller;

        private void Awake()
        {
            caller = GetComponent<Collider>();
        }

        [Tooltip("Function to call when trigger is entered.")]
        public UnityEvent<Event> Enter;

        void OnTriggerEnter(Collider other)
        {
            if (gameObject.layer == other.gameObject.layer)
            {
                Enter.Invoke(new Event(caller, other));
            }
        }

        /// <summary>
        /// Stores which collider triggered this call and which collider belongs to the other object.
        /// </summary>
        public struct Event
        {
            /// <summary>
            /// Creates an OnTriggerDelegation struct.
            /// Stores which collider triggered this call and which collider belongs to the other object.
            /// </summary>
            /// <param name="caller">The trigger collider which triggered the call.</param>
            /// <param name="other">The collider which belongs to the other object.</param>
            public Event(Collider caller, Collider other)
            {
                Caller = caller;
                Other = other;
            }

            /// <summary>
            /// The trigger collider which triggered the call.
            /// </summary>
            public Collider Caller { get; }

            /// <summary>
            /// The other collider.
            /// </summary>
            public Collider Other { get; }
        }
    }
}
