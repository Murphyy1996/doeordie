//Author: James Murphy
//Date: 22/02/2017
//Purpose: This will spawn all the scripts etc and fulfill the values and needs for the character controller in a user friendly way
//Location: On the object you will like to become a character controller
using UnityEngine;
using System.Collections;

public class SetUp : MonoBehaviour
{
    //Variables required for setting up this script easily and efficiently
    private Transform thisTransform;
    private GameObject thisGameObject;
    private Rigidbody thisRigidbody;
    private Mesh thisMesh;
    private CharacterController thisCharacterController;
    //The variables required for the user to input and configure
    [Header("Movement")]
    [Range(0f, 20f)]
    [SerializeField]
    private int movementSpeed = 8;
    //Variables for sprint
    [SerializeField] private bool sprintEnabled = true, toggleSprint = false;
    [Range(5, 200)] [SerializeField] private int sprintPercentIncrease = 70;
    //Jump variables
    [Range(0, 10)] [SerializeField] private int jumpHeight = 7;
    [SerializeField] private bool bunnyHoppingEnabled = false;
    [Header("Gravity")]
    [SerializeField]
    private bool gravityEnabled = true;
    [Range(0f, 50f)] [SerializeField] private int gravityStrength = 40;
    //Physics variables
    private bool useRigidbodyPhysics = false;
    [Header("General Physics")]
    [SerializeField]
    private bool reduceSpeedInAir = false;
    private bool automaticMass;
    private float mass = 1;
    private int friction = 5;

    private enum colliderTypes
    {
        automatic,
        sphere,
        box,
        capsule,
        useExisting
    }

    ;

    private colliderTypes colliderType;
    //This will create a drop down for the player for collider types
    [Header("Character Controller Collider")]
    [SerializeField]
    private bool useManualSettingsBelow = false;
    [SerializeField] private Vector3 colliderOffsets;
    [SerializeField] private float colliderHeight = 0, colliderRadius = 0.5f;
    [Range(0, 180)]
    [SerializeField]
    private float slopeLimit = 45;
    //Variables for the camera
    private enum cameraTypes
    {
        firstPerson,
        thirdPerson
    }

    ;

    [Header("General Camera")]
    [SerializeField]
    private cameraTypes cameraType;
    //This will create a drop down for the player
    [Range(0f, 120f)] [SerializeField] private float fieldOfView = 60f;
    [Range(0f, 15f)] [SerializeField] private int cameraXAxisSensitivity = 4, cameraYAxisSensitivity = 4;
    [Range(0f, 130f)] [SerializeField] private float cameraYClampValue = 80f;
    [SerializeField] private bool invertYAxis;
    [Range(-10f, 10f)] [SerializeField] private float verticalOffset = 1;
    [Header("Third Person Camera")]
    [Range(2f, 15f)]
    [SerializeField]
    private float distanceFromPlayer;
    [Range(0.1f, 5f)] [SerializeField] private float cameraFriction;
    [Range(-10f, 10f)] [SerializeField] private float horizontalOffset;
    [SerializeField] private bool zoomEnabled;
    [SerializeField] private string wallTag;
    //Variables for movement
    [Header("Keyboard Input")]
    [SerializeField]
    private KeyCode sprintButton;
    [SerializeField] private KeyCode jumpButton, forwardButton, backwardButton, leftButton, rightButton;
    [Header("Controller Input (Advanced)")]
    [SerializeField]
    private bool controllerSupport = false;
    [SerializeField] private string leftAnalogXAxisName, leftAnalogYAxisName, rightAnalogXAxisName, rightAnalogYAxisName;
    [SerializeField] private KeyCode jumpButtonController, sprintButtonController, thirdPersonZoomOut, thirdPersonZoomIn;
    [Header("Miscellaneous")]
    [SerializeField]
    private GameObject volumetricCamera, spectatorUIPrefab;
    [SerializeField]
    private LayerMask roofLayerMask;
    [SerializeField]
    private bool destroyThisScriptAfterSetUp = false;

    //Variables that user does not need to see
    private Camera spawnedCamera;

    private void Awake() //Run everything needs on awake
    {
        Destroy(GetComponent<CharacterController>());
        //Get this transform and gameobject components now to avoid using "Transform.position" for example as that is equivalent to a "GetComponent<>" call
        thisTransform = GetComponent<Transform>();
        thisGameObject = thisTransform.gameObject;

        //Check to see if the supplied controller axis are valid, if they are not disable the controller support
        if (controllerSupport == true)
        {
            try
            {

                Input.GetAxis(leftAnalogXAxisName);
                Input.GetAxis(leftAnalogYAxisName);
                Input.GetAxis(rightAnalogXAxisName);
                Input.GetAxis(rightAnalogYAxisName);
            }
            catch
            {
                controllerSupport = false;
                Debug.Log("1 or more invalid axis, please check your controller input mappings");
            }

            //Also check to see if a controller is connected
            if (Input.GetJoystickNames().Length == 0)
            {
                controllerSupport = false;
            }
        }

        //Set up the camera based on the user preferences
        SetUpCamera();

        //If the user has chosen to use unitys built in physics (rigidbody)
        if (useRigidbodyPhysics == true)
        {
            SetUpRigidBody();
        }
        else //If the user has elected against using a rigidbody
        {
            SetUpCharacterController();
        }


        //Spawn in the spectator prefab if possible
        if (spectatorUIPrefab != null)
        {
            GameObject spectatorUI = Instantiate(spectatorUIPrefab) as GameObject;
            spectatorUI.name = "Pre-Game";
        }

        //Once the set up is finished, decide whether to destroy this script of not
        if (destroyThisScriptAfterSetUp == true)
        {
            Destroy(this);
        }
    }

    private void SetUpCamera() //This method will be called when the camera needs to be set up
    {
        //If there is an already active camera, turn it off
        if (Camera.main != null)
        {
            Camera.main.enabled = false;
        }

        //Spawn an empty object, name it, set it as a child and give it a camera component
        GameObject cameraObject = new GameObject();
        spawnedCamera = cameraObject.AddComponent<Camera>();
        cameraObject.tag = "MainCamera";

        //If volumetric light is turned on, spawn in the volumetric camera
        if (volumetricCamera != null)
        {
            //Destroy the existing camera as it will be replaced by the volumetric camera
            Destroy(cameraObject);
            //Spawn in the volumetric camera
            cameraObject = Instantiate(volumetricCamera);
            spawnedCamera = cameraObject.GetComponent<Camera>();
        }

        //Name the spawned camera object
        cameraObject.name = "CameraObj";
        cameraObject.transform.SetParent(thisTransform);
        //Set the field of view
        spawnedCamera.fieldOfView = fieldOfView;
        //Spawn the camera on the player
        cameraObject.transform.position = new Vector3(thisTransform.position.x, thisTransform.position.y + verticalOffset, thisTransform.position.z);
        //Give the camera the default rotation of the player on the y axis
        cameraObject.transform.rotation = thisTransform.rotation;



        //If a First Person camera has been selected...
        if (cameraType == cameraTypes.firstPerson)
        {
            //Spawn the first person camera script
            FirstPersonCamera fpsScript = cameraObject.AddComponent<FirstPersonCamera>();
            //Fill in the variables on the first person camera script
            fpsScript.SetValues(cameraYAxisSensitivity, cameraXAxisSensitivity, cameraYClampValue, invertYAxis, controllerSupport, rightAnalogXAxisName, rightAnalogYAxisName);
        }
        else //Set up a third person camera
        {
            //Spawn the first person camera script
            ThirdPersonCamera tpcScript = cameraObject.AddComponent<ThirdPersonCamera>();
            //Fill in the variables on the first person camera script
            tpcScript.SetValues(thisTransform, cameraYAxisSensitivity, cameraXAxisSensitivity, cameraYClampValue, distanceFromPlayer, cameraFriction, verticalOffset, horizontalOffset, gravityEnabled, invertYAxis, zoomEnabled, controllerSupport, rightAnalogXAxisName, rightAnalogYAxisName, thirdPersonZoomIn, thirdPersonZoomOut, wallTag);
        }

    }

    private void SetUpRigidBody() //This method will be run when a rigidbody character controller needs to be set up
    {
        //Check if the game object already has a rigidbody or not...
        if (thisTransform.GetComponent<Rigidbody>() == null)
        {
            //Spawn a rigidbody component
            thisRigidbody = thisGameObject.AddComponent<Rigidbody>();
        }
        else //If the gameobject already has a rigidbody...
        {
            //Get the alreadt present rigidbody
            thisRigidbody = thisTransform.GetComponent<Rigidbody>();
        }

        //Set up all rigidbody colliders
        SetUpRigidbodyColliders();

        //Check if any components on the object will interfere with the rigidbody
        if (thisTransform.GetComponent<CharacterController>() != null)
        {
            Destroy(thisTransform.GetComponent<CharacterController>());
        }

        //Set the desired settings

        if (gravityEnabled == true) //If gravity is enabled...
        {
            //Turn it on
            thisRigidbody.useGravity = true;
            //Also set the strength
            Physics.gravity = new Vector3(0, -gravityStrength, 0);
        }
        else //If gravity isn't enabled...
        {
            //Make sure it is off
            thisRigidbody.useGravity = false;
        }

        //Set additional rigidody values
        //If automatic mass is active
        if (automaticMass == true)
        {
            //This detects the size of an object and if it is bigger, it is more likely to have more mass
            Vector3 massRaw = thisTransform.GetComponent<Renderer>().bounds.size;
            mass = massRaw.x + massRaw.y + massRaw.z;
        }
        thisRigidbody.drag = friction;
        thisRigidbody.mass = mass;

        //Spawn the rigidbody movement script on the player
        RigidMovement rigidbodyMovementScript = thisGameObject.AddComponent<RigidMovement>();

        //Set up the variables required on the rigidbody movement script
        rigidbodyMovementScript.SetValues(movementSpeed, jumpHeight, sprintButton, sprintEnabled, sprintPercentIncrease, spawnedCamera, gravityEnabled, gravityStrength, forwardButton, backwardButton, rightButton, leftButton, jumpButton, jumpButtonController, sprintButtonController, controllerSupport, leftAnalogXAxisName, leftAnalogYAxisName);
    }

    private void SetUpRigidbodyColliders() // This will set up any rigidbody colliders as specified in the set ups script
    {
        //Only set up the collider if the user has chosen not to use existing colliders
        if (colliderType != colliderTypes.useExisting)
        {
            //Get any colliders on this game object
            Collider[] foundColliders = GetComponents<Collider>();

            //If any previous colliders have been found
            if (foundColliders.Length > 0)
            {
                //Destroy any previous colliders on this game object
                foreach (Collider coll in foundColliders)
                {
                    Destroy(coll);
                }
            }

            //Delete any child colliders that could cause an issue aswell
            foreach (Transform childObject in thisTransform)
            {
                //Check if the child has any colliders
                if (childObject.GetComponent<Collider>() != null)
                {
                    Destroy(childObject.GetComponent<Collider>());
                }
            }

            //Decide what collider to add based off the users selection
            switch (colliderType)
            {
                case colliderTypes.automatic:
                    //Only run the automatic collider code if a mesh exists
                    if (thisMesh != null)
                    {
                        //Detect the amount of vertices and compare them to unity stock shapes, if there is a match, place the relevant collider
                        switch (thisMesh.vertexCount)
                        {
                            case 24: //Box Mesh vertices
                                thisGameObject.AddComponent<BoxCollider>();
                                break;
                            case 88: //Cyllinder Mesh vertices
                                thisGameObject.AddComponent<CapsuleCollider>();
                                break;
                            case 515: //Sphere Mesh vertices
                                thisGameObject.AddComponent<SphereCollider>();
                                break;
                            case 550: //Capsule Mesh vertices
                                thisGameObject.AddComponent<CapsuleCollider>();
                                break;
                            default: //If the mesh is not recognized, give it a capsule collider as it is the most common collider for movement
                                thisGameObject.AddComponent<CapsuleCollider>();
                                break;
                        }
                    }
                    else //If there is no mesh, just add a box collider as it is the most common collider
                    {
                        thisGameObject.AddComponent<BoxCollider>();
                    }
                    break;
                case colliderTypes.box:
                    thisGameObject.AddComponent<BoxCollider>();
                    break;
                case colliderTypes.capsule:
                    thisGameObject.AddComponent<CapsuleCollider>();
                    break;
                case colliderTypes.sphere:
                    thisGameObject.AddComponent<SphereCollider>();
                    break;
            }
        }
    }

    private void SetUpCharacterController() //This method will be run when a character controller without rigidbody needs to be set up
    {
        //Check if any components on the object will interfere with the character controller
        if (thisTransform.GetComponent<Rigidbody>() != null)
        {
            Destroy(thisTransform.GetComponent<Rigidbody>());
        }
        //If this object has an existing collider, remove it as the character controller adds a collider
        if (thisTransform.GetComponent<Collider>() != null)
        {
            //Get any colliders on this game object
            Collider[] foundColliders = thisTransform.GetComponents<Collider>();

            //Destroy any previous colliders on this game object
            foreach (Collider coll in foundColliders)
            {
                Destroy(coll);
            }
        }
        if (thisTransform.GetComponent<CharacterController>() != null)
        {
            thisCharacterController = GetComponent<CharacterController>();
        }
        //Check if the game object doesnt have a character controller
        if (thisTransform.GetComponent<CharacterController>() == null)
        {
            //Spawn a character controller component
            thisCharacterController = thisGameObject.AddComponent<CharacterController>();

            if (useManualSettingsBelow == true)
            {
                //Apply any user settings to the character controller
                thisCharacterController.slopeLimit = slopeLimit;
                thisCharacterController.center = colliderOffsets;
                thisCharacterController.height = colliderHeight;
                thisCharacterController.radius = colliderRadius;
            }

        }

        //Add the character controller script to the object
        CharacterControllerMovement ccMovementScript = thisGameObject.AddComponent<CharacterControllerMovement>();

        //Set up the values on the character controller movement script
        ccMovementScript.SetValues(gravityEnabled, reduceSpeedInAir, gravityStrength, spawnedCamera, movementSpeed, jumpHeight, sprintButton, sprintEnabled, sprintPercentIncrease, forwardButton, backwardButton, rightButton, leftButton, jumpButton, jumpButtonController, sprintButtonController, controllerSupport, leftAnalogXAxisName, leftAnalogYAxisName, roofLayerMask, toggleSprint, bunnyHoppingEnabled);
    }
}
