/******************************************************************************
 * Spine Runtimes License Agreement
 * Last updated April 5, 2025. Replaces all prior versions.
 *
 * Copyright (c) 2013-2025, Esoteric Software LLC
 *
 * Integration of the Spine Runtimes into software or otherwise creating
 * derivative works of the Spine Runtimes is permitted under the terms and
 * conditions of Section 2 of the Spine Editor License Agreement:
 * http://esotericsoftware.com/spine-editor-license
 *
 * Otherwise, it is permitted to integrate the Spine Runtimes into software
 * or otherwise create derivative works of the Spine Runtimes (collectively,
 * "Products"), provided that each user of the Products must obtain their own
 * Spine Editor license and redistribution of the Products in any form must
 * include this license and copyright notice.
 *
 * THE SPINE RUNTIMES ARE PROVIDED BY ESOTERIC SOFTWARE LLC "AS IS" AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL ESOTERIC SOFTWARE LLC BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES,
 * BUSINESS INTERRUPTION, OR LOSS OF USE, DATA, OR PROFITS) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THE SPINE RUNTIMES, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

using Spine.Unity.AnimationTools;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity {

	/// <summary>
	/// Add this component to a SkeletonMecanim GameObject
	/// to turn motion of a selected root bone into Transform or RigidBody motion.
	/// Local bone translation movement is used as motion.
	/// All top-level bones of the skeleton are moved to compensate the root
	/// motion bone location, keeping the distance relationship between bones intact.
	/// </summary>
	/// <remarks>
	/// Only compatible with <c>SkeletonMecanim</c>.
	/// For <c>SkeletonAnimation</c> or <c>SkeletonGraphic</c> please use
	/// <see cref="SkeletonRootMotion">SkeletonRootMotion</see> instead.
	/// </remarks>
	[HelpURL("http://esotericsoftware.com/spine-unity#SkeletonMecanimRootMotion")]
	public class SkeletonMecanimRootMotion : SkeletonRootMotionBase {
		#region Inspector
		const int DefaultMecanimLayerFlags = -1;
		public int mecanimLayerFlags = DefaultMecanimLayerFlags;
		#endregion

		protected Vector2 movementDelta;
		protected float rotationDelta;

		SkeletonMecanim skeletonMecanim;
		public SkeletonMecanim SkeletonMecanim {
			get {
				return skeletonMecanim ? skeletonMecanim : skeletonMecanim = GetComponent<SkeletonMecanim>();
			}
		}

		public override Vector2 GetRemainingRootMotion (int layerIndex) {
			KeyValuePair<Animation, float> pair = skeletonMecanim.Translator.GetActiveAnimationAndTime(layerIndex);
			Animation animation = pair.Key;
			float time = pair.Value;
			if (animation == null)
				return Vector2.zero;

			float start = time;
			float end = animation.Duration;
			return GetAnimationRootMotion(start, end, animation);
		}

		public override RootMotionInfo GetRootMotionInfo (int layerIndex) {
			KeyValuePair<Animation, float> pair = skeletonMecanim.Translator.GetActiveAnimationAndTime(layerIndex);
			Animation animation = pair.Key;
			float time = pair.Value;
			if (animation == null)
				return new RootMotionInfo();
			return GetAnimationRootMotionInfo(animation, time);
		}

		protected override void Reset () {
			base.Reset();
			mecanimLayerFlags = DefaultMecanimLayerFlags;
		}

		public override void Initialize () {
			base.Initialize();
			skeletonMecanim = GetComponent<SkeletonMecanim>();
			if (skeletonMecanim) {
				skeletonMecanim.Translator.OnClipApplied -= OnClipApplied;
				skeletonMecanim.Translator.OnClipApplied += OnClipApplied;
			}
		}

		void OnClipApplied (Spine.Animation animation, int layerIndex, float weight,
				float time, float lastTime, bool playsBackward) {

			if (((mecanimLayerFlags & 1 << layerIndex) == 0) || weight == 0)
				return;

			if (!playsBackward) {
				movementDelta += weight * GetAnimationRootMotion(lastTime, time, animation);
			} else {
				movementDelta -= weight * GetAnimationRootMotion(time, lastTime, animation);
			}
			if (transformRotation) {
				if (!playsBackward) {
					rotationDelta += weight * GetAnimationRootMotionRotation(lastTime, time, animation);
				} else {
					rotationDelta -= weight * GetAnimationRootMotionRotation(time, lastTime, animation);
				}
			}
		}

		protected override Vector2 CalculateAnimationsMovementDelta () {
			// Note: movement delta is not gathered after animation but
			// in OnClipApplied after every applied animation.
			Vector2 result = movementDelta;
			movementDelta = Vector2.zero;
			return result;
		}

		protected override float CalculateAnimationsRotationDelta () {
			// Note: movement delta is not gathered after animation but
			// in OnClipApplied after every applied animation.
			float result = rotationDelta;
			rotationDelta = 0;
			return result;
		}
	}
}
