using System.Collections.Generic;
using UnityEngine;

namespace CustomRagdoll
{
	/// <summary>
	/// To handle Ragdoll model. 
	/// Adding this script the ragdoll's root to create the ragdoll components, based on the scriptable object profile.
	/// </summary>
	[RequireComponent(typeof(Animator))]
	public partial class Ragdoll : MonoBehaviour
	{

		#region Variables

		bool initializedValues;
		//initial head position from chest (used for resizing chest collider based on head offset)				
		float initialHeadOffsetFromChest;

		// Script to store information for ragdoll
		public RagdollProfile ragdollProfile;

		[HideInInspector] public bool preBuilt;
		Dictionary<HumanBodyBones, RagdollTransform> boneElements;
		RagdollTransform[] allElements;
		Renderer[] allRenderers;

		// Layer assigned to Ragdoll model.
		public static int layer { get { return LayerMask.NameToLayer("Ragdoll"); } }

		/// <summary>
		/// Creates an array instance of the enum HumanBodyBones with all the important bones we need to use.
		/// </summary>
		public static HumanBodyBones[] humanBones = new HumanBodyBones[] {
			HumanBodyBones.Hips, //HIPS NEEDS TO BE FIRST
			
			HumanBodyBones.Chest,
			HumanBodyBones.Head,

			HumanBodyBones.RightLowerLeg,
			HumanBodyBones.LeftLowerLeg,
			HumanBodyBones.RightUpperLeg,
			HumanBodyBones.LeftUpperLeg,

			HumanBodyBones.RightLowerArm,
			HumanBodyBones.LeftLowerArm,
			HumanBodyBones.RightUpperArm,
			HumanBodyBones.LeftUpperArm,
		};

		// Get number of bones from the last instance of the enum.
		public readonly static int bonesCount = humanBones.Length;

		#endregion

		#region Methods

		/// <summary>
		/// Build the runtime representations of the ragdoll bones
		/// Adds the ragdoll components(Rigidbodies, joints, colliders...) if they werent pre built in the editor.
		/// Then the variables, (like joint limits and rigidbody masses), are adjusted via the ragdoll profile.
		/// </summary>
		void Awake()
		{
			if (!initializedValues)
			{
				initializedValues = true;

				Animator myAnimator = GetComponent<Animator>();

				//if there werent any errros
				if (RagdollBuilder.BuildRagdollElements(myAnimator, out allElements, out boneElements))
				{

					RagdollBuilder.BuildBones(myAnimator, ragdollProfile, !preBuilt, boneElements, out initialHeadOffsetFromChest);

					//set the bones to the ragdoll layer
					SetLayer(LayerMask.NameToLayer("Ragdoll"));

					//get all renderers
					allRenderers = GetComponentsInChildren<Renderer>();

					InitializeRagdollBoneComponents();
				}
				//display errors
				else
				{
					CheckForErroredRagdoll("Awake");
				}
			}
		}

		/// <summary>
		/// Initialize when starting.
		/// </summary>
		void Start()
		{
			for (int i = 0; i < bonesCount; i++)
			{
				allElements[i].OnStart();
			}
		}

		/// <summary>
		/// Returns the index of the bone.
		/// </summary>
		/// <param name="bone"></param>
		/// <returns></returns>
		public static bool BoneIsPhysicsBone(HumanBodyBones bone)
		{
			return Bone2Index(bone) != -1;
		}
		public static int Bone2Index(HumanBodyBones bone)
		{
			switch (bone)
			{
				case HumanBodyBones.Hips: return 0;
				case HumanBodyBones.Chest: return 1;
				case HumanBodyBones.Head: return 2;
				case HumanBodyBones.RightLowerLeg: return 3;
				case HumanBodyBones.LeftLowerLeg: return 4;
				case HumanBodyBones.RightUpperLeg: return 5;
				case HumanBodyBones.LeftUpperLeg: return 6;
				case HumanBodyBones.RightLowerArm: return 7;
				case HumanBodyBones.LeftLowerArm: return 8;
				case HumanBodyBones.RightUpperArm: return 9;
				case HumanBodyBones.LeftUpperArm: return 10;
			}
			return -1;
		}

		/// <summary>
		/// Get the parent bone of type enum, given a specific bone.
		/// </summary>
		/// <param name="bone"></param>
		/// <returns></returns>
		public static HumanBodyBones GetParentBone(HumanBodyBones bone)
		{
			switch (bone)
			{
				case HumanBodyBones.Chest: return HumanBodyBones.Hips;
				case HumanBodyBones.Head: return HumanBodyBones.Chest;
				case HumanBodyBones.RightLowerLeg: return HumanBodyBones.RightUpperLeg;
				case HumanBodyBones.LeftLowerLeg: return HumanBodyBones.LeftUpperLeg;
				case HumanBodyBones.RightUpperLeg: return HumanBodyBones.Hips;
				case HumanBodyBones.LeftUpperLeg: return HumanBodyBones.Hips;
				case HumanBodyBones.RightLowerArm: return HumanBodyBones.RightUpperArm;
				case HumanBodyBones.LeftLowerArm: return HumanBodyBones.LeftUpperArm;
				case HumanBodyBones.RightUpperArm: return HumanBodyBones.Chest;
				case HumanBodyBones.LeftUpperArm: return HumanBodyBones.Chest;
			}
			return HumanBodyBones.Hips;
		}

		/// <summary>
		/// Get the child bone of type enum, given a specific bone.
		/// </summary>
		/// <param name="bone"></param>
		/// <returns></returns>
		public static HumanBodyBones GetChildBone(HumanBodyBones bone)
		{
			switch (bone)
			{
				case HumanBodyBones.RightUpperLeg: return HumanBodyBones.RightLowerLeg;
				case HumanBodyBones.LeftUpperLeg: return HumanBodyBones.LeftLowerLeg;
				case HumanBodyBones.RightUpperArm: return HumanBodyBones.RightLowerArm;
				case HumanBodyBones.LeftUpperArm: return HumanBodyBones.LeftLowerArm;
			}
			return HumanBodyBones.Hips;
		}

		/// <summary>
		/// Set a velocity to each RigidBody.
		/// </summary>
		/// <param name="velocity"></param>
		public void SetVelocity(Vector3 velocity)
		{
			for (int i = 0; i < bonesCount; i++)
			{
				allElements[i].rigidbody.velocity = velocity;
			}
		}


		/// <summary>
		/// Creates a list of type T.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T[] AddComponentsToBones<T>() where T : Component
		{
			T[] r = new T[bonesCount];
			for (int i = 0; i < bonesCount; i++)
			{
				r[i] = allElements[i].AddComponent<T>();
			}
			return r;
		}

		/// <summary>
		/// Calcualte the mass of all RigidBodies.
		/// </summary>
		/// <returns></returns>
		public float CalculateMass()
		{
			if (CheckForErroredRagdoll("LoadSnapshot"))
				return 0;

			float m = 0;
			for (int i = 0; i < bonesCount; i++)
			{
				m += allElements[i].rigidbody.mass;
			}
			return m;
		}

		/// <summary>
		/// Enable/Disable the joint limits.
		/// </summary>
		/// <param name="enabled"></param>
		public void EnableJointLimits(bool enabled)
		{
			if (CheckForErroredRagdoll("EnableJointLimits"))
				return;

			ConfigurableJointMotion m = enabled ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Free;
			for (int i = 0; i < bonesCount; i++)
			{
				allElements[i].EnableJointLimits(m);
			}
		}

		/// <summary>
		/// Save the position and rotation of all elements.
		/// </summary>
		public void SaveSnapshot()
		{
			if (CheckForErroredRagdoll("SaveSnapshot"))
				return;

			for (int i = 0; i < allElements.Length; i++)
			{
				allElements[i].SaveSnapshot();
			}
		}

		/// <summary>
		/// Sets the positions and rotations of the bones to recreate the saved snapshot.
		/// Snapshot blend is the blend amount:
		/// 1 = fully in snapshot
		/// 0 = original position/rotation, or followTarget position/rotation if useFollowTarget == true
		/// </summary>
		/// <param name="snapshotBlend"></param>
		/// <param name="useFollowTarget"></param>
		public void LoadSnapshot(float snapshotBlend, bool useFollowTarget)
		{
			if (CheckForErroredRagdoll("LoadSnapshot"))
				return;

			for (int i = 0; i < allElements.Length; i++)
			{
				allElements[i].LoadSnapshot(snapshotBlend, useFollowTarget);
			}
		}


		/// <summary>
		/// Enable or disable ragdoll renderers.
		/// </summary>
		/// <param name="enabled"></param>
		public void EnableRenderers(bool enabled)
		{
			if (CheckForErroredRagdoll("EnableRenderers"))
				return;

			for (int i = 0; i < allRenderers.Length; i++)
			{
				allRenderers[i].enabled = enabled;
			}
		}

		/// <summary>
		/// Set kinematic on all ragdoll rigidbodies.
		/// </summary>
		/// <param name="value"></param>
		public void SetKinematic(bool value)
		{
			if (CheckForErroredRagdoll("SetKinematic"))
				return;

			for (int i = 0; i < bonesCount; i++)
			{
				allElements[i].rigidbody.isKinematic = value;
			}
		}


		/// <summary>
		/// Set use gravity on all ragdoll rigidbodies.
		/// </summary>
		/// <param name="value"></param>
		public void UseGravity(bool value)
		{
			if (CheckForErroredRagdoll("UseGravity"))
				return;

			for (int i = 0; i < bonesCount; i++)
			{
				allElements[i].rigidbody.useGravity = value;
			}
		}

		/// <summary>
		/// Set layer on all ragdoll physics gameobjects.
		/// </summary>
		/// <param name="layer"></param>
		public void SetLayer(int layer)
		{
			if (CheckForErroredRagdoll("SetLayer"))
				return;

			for (int i = 0; i < bonesCount; i++)
			{
				allElements[i].transform.gameObject.layer = layer;
			}
		}

		public RagdollTransform GetBone(HumanBodyBones bone)
		{
			if (CheckForErroredRagdoll("GetPhysicsBone"))
				return null;

			RagdollTransform r;
			if (boneElements.TryGetValue(bone, out r))
			{
				return r;
			}
			Debug.LogWarning("Cant find: " + bone + " on ragdoll: " + name);
			return null;
		}

		public RagdollTransform RootBone()
		{
			return allElements[0];
		}

		/// <summary>
		/// To check for errors.
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
		bool CheckForErroredRagdoll(string msg)
		{
			if (!initializedValues)
			{
				Awake();
			}
			if (ragdollProfile == null)
			{
				Debug.LogError("Ragdoll is in error state, no profile assigned!!! (" + msg + ")", transform);
				return true;
			}
			if (boneElements == null)
			{
				Debug.LogError("Ragdoll is in error state, maybe it's not humanoid? (" + msg + ")", transform);
				return true;
			}
			return false;
		}

        #endregion

        // ==================================== Not used! from here...

        /*
			if we're having too much trouble with joint stretching when grabbed,
			keep the joints at their original local position
		*/

        //void PerformJointFix()
        //{
        //    float jointFixMagnitude2 = ragdollProfile.jointFixMagnitude2;
        //    if (RigidbodyGrabbed())
        //    {
        //        for (int i = 0; i < bonesCount; i++)
        //        {
        //            allElements[i].PerformJointFix(jointFixMagnitude2);
        //        }
        //    }
        //}

        //void LateUpdate()
        //{
        //	if (ragdollProfile.jointFixEnabled)
        //	{
        //		PerformJointFix();
        //	}
        //}

        //public void ForceDetach()
        //{
        //    for (int i = 0; i < bonesCount; i++)
        //    {
        //        RagdollPhysics.DetachRigidbody(allElements[i].rigidbody, null, true);
        //    }
        //}

        //public bool RigidbodyGrabbed()
        //{
        //	for (int i = 0; i < bonesCount; i++)
        //	{
        //		if (allElements[i].RigidbodyGrabbed())
        //		{
        //			return true;
        //		}
        //	}
        //	return false;
        //}


        //public RagdollController controller;
        //public void SetController(RagdollController controller)
        //{
        //	this.controller = controller;
        //}
        //public bool hasController { get { return controller != null; } }

        // ==================================== Not used! to here.


        //update values during runtime (if not in build)
        //for easier adjustments
#if UNITY_EDITOR
        [Header("Editor Only")] public bool setValuesUpdate = true;
		void Update()
		{
			if (setValuesUpdate)
			{
				if (CheckForErroredRagdoll("Update"))
				{
					return;
				}

				// // if we're using a custom profile
				// if (ragdollProfile) {

				//if no errors
				// if (boneElements!= null) {
				UpdateBonesToProfileValues(boneElements, ragdollProfile, initialHeadOffsetFromChest);
				// }
				// }
			}
		}
#endif

	}
}

