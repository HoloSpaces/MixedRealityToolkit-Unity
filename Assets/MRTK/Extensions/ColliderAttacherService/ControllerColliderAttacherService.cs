using UnityEngine;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace HoloSpaces.Extensions
{
	[MixedRealityExtensionService(SupportedPlatforms.WindowsStandalone|SupportedPlatforms.MacStandalone|SupportedPlatforms.LinuxStandalone|SupportedPlatforms.WindowsUniversal|SupportedPlatforms.WindowsEditor|SupportedPlatforms.Android|SupportedPlatforms.MacEditor|SupportedPlatforms.LinuxEditor|SupportedPlatforms.IOS|SupportedPlatforms.Web|SupportedPlatforms.Lumin)]
	public class ControllerColliderAttacherService : BaseExtensionService, IControllerColliderAttacherService, IMixedRealityExtensionService, IMixedRealitySourceStateHandler
    {
		private ControllerColliderAttacherServiceProfile controllerColliderAttacherServiceProfile;
        private List<GameObject> controllerVisuals = new List<GameObject>();

        public ControllerColliderAttacherService(string name,  uint priority,  BaseMixedRealityProfile profile) : base(name, priority, profile) 
		{
			controllerColliderAttacherServiceProfile = (ControllerColliderAttacherServiceProfile)profile;
		}

		public override void Initialize()
		{
            foreach (var detectedController in CoreServices.InputSystem.DetectedControllers)
            {
                if (detectedController.InputSource.SourceType == InputSourceType.Controller)
                    AddCollider(detectedController.Visualizer.GameObjectProxy);
            }

            CoreServices.InputSystem?.RegisterHandler<IMixedRealitySourceStateHandler>(this);
        }

        public override void Destroy()
        {
            base.Destroy();
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealitySourceStateHandler>(this);
        }

        public override void Update() { }

        private void AddCollider(GameObject proxyObject)
        {
            if (proxyObject == null)
                return;

            if (!controllerVisuals.Contains(proxyObject))
            {
                controllerVisuals.Add(proxyObject);
                var sphereCollider = proxyObject.AddComponent<SphereCollider>();
                proxyObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                sphereCollider.radius = .06f;
                sphereCollider.isTrigger = true;
            }
        }

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            if (eventData.InputSource.SourceType == InputSourceType.Controller)
                AddCollider(eventData.Controller.Visualizer.GameObjectProxy);
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            if (eventData.InputSource.SourceType == InputSourceType.Controller &&
                !eventData.Controller.IsNull() && !eventData.Controller.Visualizer.IsNull())
                controllerVisuals.Remove(eventData.Controller.Visualizer.GameObjectProxy);
        }
    }
}