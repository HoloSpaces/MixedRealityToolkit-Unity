﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Facades
{
    /// <summary>
    /// Lightweight MonoBehavior used to represent active services in scene.
    /// </summary>
    [ExecuteAlways]
    public class ServiceFacade : MonoBehaviour
    {
        private const string DESTROYED = "(Destroyed)";

        public static Dictionary<Type, ServiceFacade> FacadeServiceLookup = new Dictionary<Type, ServiceFacade>();
        public static List<ServiceFacade> ActiveFacadeObjects = new List<ServiceFacade>();

        public IMixedRealityService Service { get { return service; } }
        public Type ServiceType { get { return serviceType; } }
        public bool Destroyed { get { return destroyed; } }

        private IMixedRealityService service = null;
        private Type serviceType = null;
        private bool destroyed = false;
        private Transform facadeParent;

        public void SetService(IMixedRealityService service, Transform facadeParent)
        {
            this.service = service;
            this.facadeParent = facadeParent;

            if (service == null)
            {
                serviceType = null;
                if (!name.Equals(DESTROYED))
                {
                    name = DESTROYED;
                }
                gameObject.SetActive(false);
                return;
            }
            else
            {
                this.serviceType = service.GetType();

                if (!name.Equals(serviceType.Name))
                {
                    name = serviceType.Name;
                }

                gameObject.SetActive(true);

                if (!FacadeServiceLookup.ContainsKey(serviceType))
                {
                    FacadeServiceLookup.Add(serviceType, this);
                }
                else
                {
                    FacadeServiceLookup[serviceType] = this;
                }

                if (!ActiveFacadeObjects.Contains(this))
                {
                    ActiveFacadeObjects.Add(this);
                }
            }
        }

        public void CheckIfStillValid()
        {
            if (service == null || transform.parent != facadeParent)
            {
                GameObjectExtensions.DestroyGameObject(gameObject);
            }
        }

        private void OnDestroy()
        {
            destroyed = true;

            if (FacadeServiceLookup != null && serviceType != null)
            {
                FacadeServiceLookup.Remove(serviceType);
            }

            ActiveFacadeObjects.Remove(this);
        }
    }
}
