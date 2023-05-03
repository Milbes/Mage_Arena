using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class PooledObjectSettings
    {
        private bool activate;
        public bool Activate { get { return activate; } }

        private bool useActiveOnHierarchy;
        public bool UseActiveOnHierarchy { get { return useActiveOnHierarchy; } }

        private Vector3 position;
        public Vector3 Position { get { return position; } }
        private bool applyPosition;
        public bool ApplyPosition { get { return applyPosition; } }

        private Vector3 localPosition;
        public Vector3 LocalPosition { get { return localPosition; } }
        private bool applyLocalPosition;
        public bool ApplyLocalPosition { get { return applyLocalPosition; } }

        private Vector3 eulerRotation;
        public Vector3 EulerRotation { get { return eulerRotation; } }
        private bool applyEulerRotation;
        public bool ApplyEulerRotation { get { return applyEulerRotation; } }

        private Vector3 localScale;
        public Vector3 LocalScale { get { return localScale; } }
        private bool applyLocalScale;
        public bool ApplyLocalScale { get { return applyLocalScale; } }

        private Transform parrent;
        public Transform Parrent { get { return parrent; } }
        private bool applyParrent;
        public bool ApplyParrent { get { return applyParrent; } }

        public PooledObjectSettings(bool activate = true, bool useActiveOnHierarchy = false)
        {
            this.activate = activate;
            this.useActiveOnHierarchy = useActiveOnHierarchy;

            applyPosition = false;
            applyEulerRotation = false;
            applyLocalScale = false;
            applyParrent = false;
        }

        public PooledObjectSettings SetActivate(bool activate)
        {
            this.activate = activate;
            return this;
        }

        public PooledObjectSettings SetPosition(Vector3 position)
        {
            this.position = position;
            applyPosition = true;
            return this;
        }

        public PooledObjectSettings SetLocalPosition(Vector3 localPosition)
        {
            this.localPosition = localPosition;
            applyLocalPosition = true;
            return this;
        }

        public PooledObjectSettings SetEulerRotation(Vector3 eulerRotation)
        {
            this.eulerRotation = eulerRotation;
            applyEulerRotation = true;
            return this;
        }

        public PooledObjectSettings SetLocalScale(Vector3 localScale)
        {
            this.localScale = localScale;
            applyLocalScale = true;
            return this;
        }

        public PooledObjectSettings SetParrent(Transform parrent)
        {
            this.parrent = parrent;
            applyParrent = true;
            return this;
        }
    }
}

// -----------------
// IAP Manager v 1.6.4
// -----------------