# mrtk_development_holospaces

mrtk_development_holospaces is a custom branch of mrtk_development

note: mrtk_development is a fork of https://github.com/microsoft/MixedRealityToolkit-Unity.git

mrtk_development is always upto date with the upstream branch and please do not modify mrtk_development branch at any chances

Try to avoid modifying this branch(mrtk_development_holospaces) as much as possible. If its a bug or required feature, please do it
locally and then create pull request to upstream branch. 

Pull request to Microsoft/mrtk_development:
Update mrtk_development branch then create a clone and do the changes on cloned branch and request pull request. 

Also recommend to track locall changes here on this document.

mrtk_development_holospaces change history
 
#fix 27/10/20 
Line renderer points are not exposed - already created pull request -pull request succeded
TODO: merge it back
namespace Microsoft.MixedReality.Toolkit.Utilities
{
    public class MixedRealityLineRenderer : BaseMixedRealityLineRenderer
    {
	+/// <summary>
        +/// Gets the LineRenderer points
        +/// </summary>
        +public Vector3[] Positions
        +{
        +    get => positions;
        +}
    }
}


#fix 27/10/20
#request feature  and do a pull request
if swich betwenn tranform update and rigid body(if there is already a rigid body present)
functionality need to be added,  pull request must be created from another branch
/// <summary>
        /// Enable/desable rigidody physics
        /// </summary>
        public bool UseRigidBody;



#fix 27/10/20 
manually set cursor distance from code, pull request must be created from another branch

    public interface IMixedRealityCursor : IMixedRealityFocusChangedHandler, IMixedRealitySourceStateHandler, IMixedRealityPointerHandler
    {
  	++float SurfaceCursorDistance { get;set;}
     }


public class BaseCursor : MonoBehaviour, IMixedRealityCursor
{
  public float SurfaceCursorDistance
        {
            get { return surfaceCursorDistance; }
            ++set { surfaceCursorDistance = value; }
        }
}


#fix 28/10/20 
public class UnityJoystickManager : BaseInputDeviceManager
{

       private void RefreshDevices()
       {
            -var joystickNames = UInput.GetJoystickNames();
             +var joystickNames = GetConnectedJoystickNames();	
       }

        +/// <summary>
        +/// Gets all connected joysticks names.
        +/// </summary>
        +/// <returns>All connected joysticks names.</returns>
        +protected virtual String[] GetConnectedJoystickNames()
        +{
        +    var joystickNames = UInput.GetJoystickNames();
        +    return joystickNames;
        +}
}

SupportedControllerType.cs
public enum SupportedControllerType
{
        +GenericAndroid = 1 << 12,
        +OculusGoRemote = 1 << 13,
        +OculusQuestRemote = 1 << 14
}

public class GenericJoystickController: BaseController
{
   -protected virtual  void UpdateButtonData(MixedRealityInteractionMapping interactionMapping)
   +protected virtual  void UpdateButtonData(MixedRealityInteractionMapping interactionMapping)
   
    -protected virtual void  UpdateSingleAxisData(MixedRealityInteractionMapping interactionMapping)
    +protected virtual void  UpdateSingleAxisData(MixedRealityInteractionMapping interactionMapping)

     -protected virtual void UpdateDualAxisData(MixedRealityInteractionMapping interactionMapping)
     +protected virtual void UpdateDualAxisData(MixedRealityInteractionMapping interactionMapping)
      -protected static readonly ProfilerMarker UpdateSingleAxisDataPerfMarker = new ProfilerMarker("[MRTK] GenericJoystickController.UpdateSingleAxisData");
      +protected static readonly ProfilerMarker UpdateSingleAxisDataPerfMarker = new ProfilerMarker("[MRTK] GenericJoystickController.UpdateSingleAxisData");
}


public class NonNativeKeyboard
{
	private bool IsMicrophoneActive()
        {
            -var result = _recordImage.color != _defaultColor; 
            var result = false;
            + if (_recordImage != null)
            +{
             +   result  = _recordImage.color != _defaultColor; 
            +}
            return result;
        }
}

#fix 28/10/20 
Custom for holospaces URP 
 public class CameraFaderQuad : ICameraFader
    {
	 public async Task FadeOutAsync(float fadeOutTime, Color color, IEnumerable<Camera> targets)
        {
          +string colorPropertyName = quadMaterial.HasProperty("_BaseColor") ? "_BaseColor" : QuadMaterialColorName; // support for URP shader
          +quadMaterial.SetColor(colorPropertyName, currentColor);

		+quad.PropertyBlock.SetColor(colorPropertyName, currentColor);
	}

	public async Task FadeInAsync(float fadeInTime)
        {
  		+string colorPropertyName = quadMaterial.HasProperty("_BaseColor") ? "_BaseColor" : QuadMaterialColorName; // support for URP shader
                +quad.PropertyBlock.SetColor(colorPropertyName, currentColor);
	}
    }
}

#fix 29/10/20 
// keyboard double entry fix and press on enter down 
 public class KeyboardValueKey : MonoBehaviour
{
	//compare the original one with current one
}


#fix 29/10/20 
// Object manipulator enable desable rigid body
// Request for this feature and create a pull requst
public void ObjectManipulator 
{
	//compare the original one with current one or check against "UseRigidbody"
}


#fix 29/10/20  bug enabling desabling teleport system
 public class MixedRealityTeleportSystem : BaseCoreSystem, IMixedRealityTeleportSystem
{
	+public bool Enabled { get; set;}
        + public override void Enable()
        {
            Enabled = true;
        }

       + public override void Disable()
        {
            Enabled = false;
        }

}

IMixedRealityTeleportSystem
{
	 +bool Enabled { get; set; }
}

TeleportPointer
{
   +if (!CoreServices.TeleportSystem.Enabled) return;
}

#fix 29/10/20 glb import confict with gltfuilty from wolf 3d
#no need of pull request, poject specific
  -//[ScriptedImporter(1, "glb")]
  +//[ScriptedImporter(1, "glb")]
    public class GlbAssetImporter : ScriptedImporter
    {
}

-//[ScriptedImporter(1, "glb")]
+//[ScriptedImporter(1, "gltf")]
    public class GltfAssetImporter : ScriptedImporter
    {}

#fix 29/10/20 fix pointer extent override not working
create pullrewuest, veryfy the fix is correct

ShellHandRayPointer
{
   +public override void OnPreSceneQuery()
        {
            base.OnPreSceneQuery();
            Ray ray = new Ray(Position, Rotation * Vector3.forward);
            Rays[0].CopyRay(ray, PointerExtent);
        }

}

feature 29/10/20 feature contue object velocity based on its last positin
Custom for holospaces

ObjectManipulator
{
	SerializeField]
        [Tooltip("Multiply with existing velocity")]
        public float keepVelocityMutliplier = 1.0f;
        
        [SerializeField]
        [Tooltip("Uses object veolicity Instead of hand velocity")]
        public bool useObjectVelocity = true;
	
        ReleaseRigidBody()
	{

 		if (releaseBehavior.HasFlag(ReleaseBehaviorType.KeepVelocity))
                {
                    +Vector3 finalVelocity = useObjectVelocity?objectVelocity: velocity;
                    +finalVelocity = finalVelocity * keepVelocityMutliplier;
                    +rigidBody.velocity = finalVelocity;
                }

	}

      ApplyTargetTransform()
	{

	UpdateVelocity(HostTransform.position);
	}
      +protected void UpdateVelocity(Vector3 position) // code from basehand
      {
	+check the entire function
      }
}

ObjectManipulatorInspector
{
        +private SerializedProperty useObjectVelocity;
        +private SerializedProperty keepVelocityMutliplier;

	+useObjectVelocity = serializedObject.FindProperty("useObjectVelocity");
         +keepVelocityMutliplier = serializedObject.FindProperty("keepVelocityMutliplier");
	+EditorGUILayout.PropertyField(useObjectVelocity);
         +EditorGUILayout.PropertyField(keepVelocityMutliplier);
}

#fix 29/10/20 null reference on property

ManipulationHandler
{
public RotationConstraintType ConstraintOnRotation
        {
                +if (rotateConstraint != null)
                {
                    rotateConstraint.ConstraintOnRotation = RotationConstraintHelper.ConvertToAxisFlags(constraintOnRotation);
                }
}
}


#fix 08/12/20 feature restric backward movement when there is a collider
Custom for holospaces
 public class TeleportPointer 
  {
        +[SerializeField]
        +[Tooltip("The condition if a Strafe Height is needed")]
        +private bool requiresStrafeHeight = false;
        
        +[SerializeField]
        +[Tooltip("The height of required strafe")]
        +private float strafeHeight = 0.5f;

	 public override void OnInputChanged(InputEventData<Vector2> eventData)
        {

	+if (requiresStrafeHeight)
	 {
   		isValidStrafe = CheckPossibleBackStep(newPosition, out strafeHitPosition);
                newPosition = strafeHitPosition;
         }
         else
         {
              newPosition.y = height;
         }                                                             
                              
          if (isValidStrafe)                              
         MixedRealityPlayspace.Position = newPosition;  

        }

	+private bool CheckPossibleBackStep(Vector3 newPosition, out Vector3 hitStrafePosition)
        {
            var raycastProvider = CoreServices.InputSystem.RaycastProvider;

            Vector3 strafeOrigin = new Vector3(newPosition.x, MixedRealityPlayspace.Position.y + strafeHeight, newPosition.z);
            Vector3 strafeTerminus = strafeOrigin + (Vector3.down * strafeHeight * 2f);
            RayStep rayStep = new RayStep(strafeOrigin, strafeTerminus);
            MixedRealityRaycastHit hitInfo;
            LayerMask[] layerMasks = new LayerMask[] { ValidLayers };

            if (raycastProvider.Raycast(rayStep, layerMasks, false, out hitInfo))
            {
                hitStrafePosition = hitInfo.point;
                return true;
            }

            hitStrafePosition = Vector3.zero;
            return false;
        }                                                                                           
                                       
}
 public class TeleportPointerInspector : LinePointerInspector
    {
 	+private SerializedProperty requiresStrafeHeight;
        +private SerializedProperty strafeHeight;

	protected override void OnEnable()
	{
 	   +requiresStrafeHeight = serializedObject.FindProperty("requiresStrafeHeight");
            +strafeHeight = serializedObject.FindProperty("strafeHeight")
	}

	public override void OnInspectorGUI()
        {
 		EditorGUILayout.PropertyField(strafeAmount);
                EditorGUILayout.PropertyField(requiresStrafeHeight);
        }
   }

