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

#if UNITY_2019_3_OR_NEWER
#define MESH_SET_TRIANGLES_PROVIDES_LENGTH_PARAM
#endif

#if !UNITY_2020_1_OR_NEWER
// Note: on Unity 2019.4 or older, e.g. operator* was not inlined via AggressiveInlining and at least with some
// configurations will lead to unnecessary overhead.
#define MANUALLY_INLINE_VECTOR_OPERATORS
#endif

// Optimization option: Allows faster BuildMeshWithArrays call and avoids calling SetTriangles at the cost of
// checking for mesh differences (vertex counts, member-wise attachment list compare) every frame.
#define SPINE_TRIANGLECHECK
//#define SPINE_DEBUG

// New optimization option to avoid rendering fully transparent attachments at slot alpha 0.
// Comment out this line to revert to previous behaviour.
// You may only need this option disabled when utilizing a custom shader which
// uses vertex color alpha for purposes other than transparency.
//
// Important Note: When disabling this define, also disable the one in SkeletonRenderInstruction.cs
#define SLOT_ALPHA_DISABLES_ATTACHMENT

// Note: This define below enables a bugfix where when Linear color space is used and `PMA vertex colors` enabled,
// additive slots add a too dark (too transparent) color value.
//
// If you want the old incorrect behaviour (darker additive slots) or are not using Linear but Gamma color space,
// you can comment-out the define below to deactivate the fix or just to skip unnecessary instructions.
//
// Details:
// Alpha-premultiplication of vertex colors happens in gamma-space, and vertexColor.a is set to 0 at additive slots.
// In the shader, gamma space vertex color has to be transformed from gamma space to linear space.
// Unfortunately vertexColorGamma.rgb=(rgb*a) while the desired color in linear space would be
// vertexColorLinear.rgb = GammaToLinear(rgb)*a = GammaToLinear(vertexColorGamma.rgb/a),
// but unfortunately 'a' is unknown as vertexColorGamma.a = 0 at additive slots.
// Thus the define below enables a fix where 'a' is transformed via
// a=LinearToGamma(a), so that the subsequent GammaToLinear() operation is canceled out on 'a'.
#define LINEAR_COLOR_SPACE_FIX_ADDITIVE_ALPHA

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity {
	public delegate void MeshGeneratorDelegate (MeshGeneratorBuffers buffers);
	public struct MeshGeneratorBuffers {
		/// <summary>The vertex count that will actually be used for the mesh. The Lengths of the buffer arrays may be larger than this number.</summary>
		public int vertexCount;

		/// <summary> Vertex positions. To be used for UnityEngine.Mesh.vertices.</summary>
		public Vector3[] vertexBuffer;

		/// <summary> Vertex texture coordinates (UVs). To be used for UnityEngine.Mesh.uv.</summary>
		public Vector2[] uvBuffer;

		/// <summary> Vertex colors. To be used for UnityEngine.Mesh.colors32.</summary>
		public Color32[] colorBuffer;

		/// <summary> Optional vertex texture coordinates (UVs), second channel. To be used for UnityEngine.Mesh.uv2.
		/// Using this accessor automatically allocates and resizes the buffer accordingly.</summary>
		public Vector2[] uv2Buffer { get { return meshGenerator.UV2; } }

		/// <summary> Optional vertex texture coordinates (UVs), third channel. To be used for UnityEngine.Mesh.uv3.
		/// Using this accessor automatically allocates and resizes the buffer accordingly.</summary>
		public Vector2[] uv3Buffer { get { return meshGenerator.UV3; } }

		/// <summary> The Spine rendering component's MeshGenerator. </summary>
		public MeshGenerator meshGenerator;
	}

	/// <summary>Holds several methods to prepare and generate a UnityEngine mesh based on a skeleton. Contains buffers needed to perform the operation, and serializes settings for mesh generation.</summary>
	[System.Serializable]
	public class MeshGenerator {
		public Settings settings = Settings.Default;

		[System.Serializable]
		public struct Settings {
			public bool useClipping;
			[Range(-0.1f, 0f)] public float zSpacing;
			public bool tintBlack;
			[UnityEngine.Serialization.FormerlySerializedAs("canvasGroupTintBlack")]
			[Tooltip("Enable when using SkeletonGraphic under a CanvasGroup. " +
				"When enabled, PMA Vertex Color alpha value is stored at uv2.g instead of color.a to capture " +
				"CanvasGroup modifying color.a. Also helps to detect correct parameter setting combinations.")]
			public bool canvasGroupCompatible;
			public bool pmaVertexColors;
			public bool addNormals;
			public bool calculateTangents;
			public bool immutableTriangles;

			static public Settings Default {
				get {
					return new Settings {
						pmaVertexColors = true,
						zSpacing = 0f,
						useClipping = true,
						tintBlack = false,
						calculateTangents = false,
						//renderMeshes = true,
						addNormals = false,
						immutableTriangles = false
					};
				}
			}
		}

		const float BoundsMinDefault = float.PositiveInfinity;
		const float BoundsMaxDefault = float.NegativeInfinity;

		[NonSerialized] protected readonly ExposedList<Vector3> vertexBuffer = new ExposedList<Vector3>(4);
		[NonSerialized] protected readonly ExposedList<Vector2> uvBuffer = new ExposedList<Vector2>(4);
		[NonSerialized] protected readonly ExposedList<Color32> colorBuffer = new ExposedList<Color32>(4);
		[NonSerialized] protected readonly ExposedList<ExposedList<int>> submeshes = new ExposedList<ExposedList<int>> { new ExposedList<int>(6) }; // start with 1 submesh.

		[NonSerialized] Vector2 meshBoundsMin, meshBoundsMax;
		[NonSerialized] float meshBoundsThickness;
		[NonSerialized] int submeshIndex = 0;

		[NonSerialized] SkeletonClipping clipper = new SkeletonClipping();
		[NonSerialized] float[] tempVerts = new float[8];
		[NonSerialized] int[] regionTriangles = { 0, 1, 2, 2, 3, 0 };

		#region Optional Buffers
		// These optional buffers are lazy-instantiated when the feature is used.
		[NonSerialized] Vector3[] normals;
		[NonSerialized] Vector4[] tangents;
		[NonSerialized] Vector2[] tempTanBuffer;
		[NonSerialized] ExposedList<Vector2> uv2;
		[NonSerialized] ExposedList<Vector2> uv3;

		/// <summary> Optional vertex texture coordinates (UVs), second channel. To be used for UnityEngine.Mesh.uv2.
		/// Using this accessor automatically allocates and resizes the buffer accordingly.</summary>
		public Vector2[] UV2 { get { PrepareOptionalUVBuffer(ref uv2, vertexBuffer.Count); return uv2.Items; } }
		/// <summary> Optional vertex texture coordinates (UVs), third channel. To be used for UnityEngine.Mesh.uv3.
		/// Using this accessor automatically allocates and resizes the buffer accordingly.</summary>
		public Vector2[] UV3 { get { PrepareOptionalUVBuffer(ref uv3, vertexBuffer.Count); return uv3.Items; } }
		#endregion

		public int VertexCount { get { return vertexBuffer.Count; } }
		public int SubmeshIndexCount (int submeshIndex) { return submeshes.Items[submeshIndex].Count; }

		/// <summary>A set of mesh arrays whose values are modifiable by the user. Modify these values before they are passed to the UnityEngine mesh object in order to see the effect.</summary>
		public MeshGeneratorBuffers Buffers {
			get {
				return new MeshGeneratorBuffers {
					vertexCount = this.VertexCount,
					vertexBuffer = this.vertexBuffer.Items,
					uvBuffer = this.uvBuffer.Items,
					colorBuffer = this.colorBuffer.Items,
					meshGenerator = this
				};
			}
		}

		/// <summary>Returns the <see cref="SkeletonClipping"/> used by this mesh generator for use with e.g.
		/// <see cref="Skeleton.GetBounds(out float, out float, out float, out float, ref float[], SkeletonClipping)"/>
		/// </summary>
		public SkeletonClipping SkeletonClipping { get { return clipper; } }

		public MeshGenerator () {
			submeshes.TrimExcess();
		}

		#region Step 1 : Generate Instructions
		/// <summary>
		/// A specialized variant of <see cref="GenerateSkeletonRendererInstruction"/>.
		/// Generates renderer instructions using a single submesh, using only a single material and texture.
		/// </summary>
		/// <param name="instructionOutput">The resulting instructions.</param>
		/// <param name="skeleton">The skeleton to generate renderer instructions for.</param>
		/// <param name="material">Material to be set at the renderer instruction. When null, the last attachment
		/// in the draw order list is assigned as the instruction's material.</param>
		public static void GenerateSingleSubmeshInstruction (SkeletonRendererInstruction instructionOutput, Skeleton skeleton, Material material) {
			ExposedList<Slot> drawOrder = skeleton.DrawOrder;
			int drawOrderCount = drawOrder.Count;

			// Clear last state of attachments and submeshes
			instructionOutput.Clear(); // submeshInstructions.Clear(); attachments.Clear();
			ExposedList<SubmeshInstruction> workingSubmeshInstructions = instructionOutput.submeshInstructions;

#if SPINE_TRIANGLECHECK
			instructionOutput.attachments.Resize(drawOrderCount);
			Attachment[] workingAttachmentsItems = instructionOutput.attachments.Items;
			int totalRawVertexCount = 0;
#endif

			SubmeshInstruction current = new SubmeshInstruction {
				skeleton = skeleton,
				preActiveClippingSlotSource = -1,
				startSlot = 0,
#if SPINE_TRIANGLECHECK
				rawFirstVertexIndex = 0,
#endif
				material = material,
				forceSeparate = false,
				endSlot = drawOrderCount
			};

#if SPINE_TRIANGLECHECK
			object rendererObject = null;
			bool skeletonHasClipping = false;
			Slot[] drawOrderItems = drawOrder.Items;
			for (int i = 0; i < drawOrderCount; i++) {
				Slot slot = drawOrderItems[i];
				if (!slot.Bone.Active
#if SLOT_ALPHA_DISABLES_ATTACHMENT
					|| slot.A == 0f
#endif
					) {
					workingAttachmentsItems[i] = null;
					continue;
				}
				if (slot.Data.BlendMode == BlendMode.Additive) current.hasPMAAdditiveSlot = true;
				Attachment attachment = slot.Attachment;

				workingAttachmentsItems[i] = attachment;
				int attachmentTriangleCount;
				int attachmentVertexCount;

				RegionAttachment regionAttachment = attachment as RegionAttachment;
				if (regionAttachment != null) {
					if (regionAttachment.Sequence != null) regionAttachment.Sequence.Apply(slot, regionAttachment);
					rendererObject = regionAttachment.Region;
					attachmentVertexCount = 4;
					attachmentTriangleCount = 6;
				} else {
					MeshAttachment meshAttachment = attachment as MeshAttachment;
					if (meshAttachment != null) {
						if (meshAttachment.Sequence != null) meshAttachment.Sequence.Apply(slot, meshAttachment);
						rendererObject = meshAttachment.Region;
						attachmentVertexCount = meshAttachment.WorldVerticesLength >> 1;
						attachmentTriangleCount = meshAttachment.Triangles.Length;
					} else {
						ClippingAttachment clippingAttachment = attachment as ClippingAttachment;
						if (clippingAttachment != null) {
							current.hasClipping = true;
							skeletonHasClipping = true;
						}
						attachmentVertexCount = 0;
						attachmentTriangleCount = 0;
					}
				}
				current.rawTriangleCount += attachmentTriangleCount;
				current.rawVertexCount += attachmentVertexCount;
				totalRawVertexCount += attachmentVertexCount;
			}

#if !SPINE_TK2D
			if (material == null && rendererObject != null)
				current.material = (Material)((AtlasRegion)rendererObject).page.rendererObject;
#else
			if (material == null && rendererObject != null)
				current.material = (rendererObject is Material) ? (Material)rendererObject : (Material)((AtlasRegion)rendererObject).page.rendererObject;
#endif

			instructionOutput.hasActiveClipping = skeletonHasClipping;
			instructionOutput.rawVertexCount = totalRawVertexCount;
#endif

#if SPINE_TRIANGLECHECK
			bool hasAnyVertices = totalRawVertexCount > 0;
#else
			bool hasAnyVertices = true;
#endif
			if (hasAnyVertices) {
				workingSubmeshInstructions.Resize(1);
				workingSubmeshInstructions.Items[0] = current;
			} else {
				workingSubmeshInstructions.Resize(0);
			}
		}

		public static bool RequiresMultipleSubmeshesByDrawOrder (Skeleton skeleton) {

#if SPINE_TK2D
			return false;
#endif
			ExposedList<Slot> drawOrder = skeleton.DrawOrder;
			int drawOrderCount = drawOrder.Count;
			Slot[] drawOrderItems = drawOrder.Items;

			Material lastRendererMaterial = null;
			for (int i = 0; i < drawOrderCount; i++) {
				Slot slot = drawOrderItems[i];
				if (!slot.Bone.Active
#if SLOT_ALPHA_DISABLES_ATTACHMENT
					|| slot.A == 0f
#endif
					) continue;
				Attachment attachment = slot.Attachment;
				IHasTextureRegion rendererAttachment = attachment as IHasTextureRegion;
				if (rendererAttachment != null) {
					if (rendererAttachment.Sequence != null) rendererAttachment.Sequence.Apply(slot, rendererAttachment);
					AtlasRegion atlasRegion = (AtlasRegion)rendererAttachment.Region;
					Material material = (Material)atlasRegion.page.rendererObject;
					if (lastRendererMaterial != material) {
						if (lastRendererMaterial != null)
							return true;
						lastRendererMaterial = material;
					}
				}
			}
			return false;
		}

		public static void GenerateSkeletonRendererInstruction (SkeletonRendererInstruction instructionOutput, Skeleton skeleton, Dictionary<Slot, Material> customSlotMaterials, List<Slot> separatorSlots, bool generateMeshOverride, bool immutableTriangles = false) {
			//			if (skeleton == null) throw new ArgumentNullException("skeleton");
			//			if (instructionOutput == null) throw new ArgumentNullException("instructionOutput");

			ExposedList<Slot> drawOrder = skeleton.DrawOrder;
			int drawOrderCount = drawOrder.Count;

			// Clear last state of attachments and submeshes
			instructionOutput.Clear(); // submeshInstructions.Clear(); attachments.Clear();
			ExposedList<SubmeshInstruction> workingSubmeshInstructions = instructionOutput.submeshInstructions;
#if SPINE_TRIANGLECHECK
			instructionOutput.attachments.Resize(drawOrderCount);
			Attachment[] workingAttachmentsItems = instructionOutput.attachments.Items;
			int totalRawVertexCount = 0;
			bool skeletonHasClipping = false;
#endif

			SubmeshInstruction current = new SubmeshInstruction {
				skeleton = skeleton,
				preActiveClippingSlotSource = -1
			};

#if !SPINE_TK2D
			bool isCustomSlotMaterialsPopulated = customSlotMaterials != null && customSlotMaterials.Count > 0;
#endif

			int separatorCount = separatorSlots == null ? 0 : separatorSlots.Count;
			bool hasSeparators = separatorCount > 0;

			int clippingAttachmentSource = -1;
			int lastPreActiveClipping = -1; // The index of the last slot that had an active ClippingAttachment.
			SlotData clippingEndSlot = null;
			int submeshIndex = 0;
			Slot[] drawOrderItems = drawOrder.Items;
			for (int i = 0; i < drawOrderCount; i++) {
				Slot slot = drawOrderItems[i];
				if (!slot.Bone.Active
#if SLOT_ALPHA_DISABLES_ATTACHMENT
					|| (slot.A == 0f && slot.Data != clippingEndSlot)
#endif
					) {
#if SPINE_TRIANGLECHECK
					workingAttachmentsItems[i] = null;
#endif
					continue;
				}
				if (slot.Data.BlendMode == BlendMode.Additive) current.hasPMAAdditiveSlot = true;
				Attachment attachment = slot.Attachment;
#if SPINE_TRIANGLECHECK
				workingAttachmentsItems[i] = attachment;
				int attachmentVertexCount = 0, attachmentTriangleCount = 0;
#endif

				object region = null;
				bool noRender = false; // Using this allows empty slots as separators, and keeps separated parts more stable despite slots being reordered

				RegionAttachment regionAttachment = attachment as RegionAttachment;
				if (regionAttachment != null) {
					if (regionAttachment.Sequence != null) regionAttachment.Sequence.Apply(slot, regionAttachment);
					region = regionAttachment.Region;
#if SPINE_TRIANGLECHECK
					attachmentVertexCount = 4;
					attachmentTriangleCount = 6;
#endif
				} else {
					MeshAttachment meshAttachment = attachment as MeshAttachment;
					if (meshAttachment != null) {
						if (meshAttachment.Sequence != null) meshAttachment.Sequence.Apply(slot, meshAttachment);
						region = meshAttachment.Region;
#if SPINE_TRIANGLECHECK
						attachmentVertexCount = meshAttachment.WorldVerticesLength >> 1;
						attachmentTriangleCount = meshAttachment.Triangles.Length;
#endif
					} else {
#if SPINE_TRIANGLECHECK
						ClippingAttachment clippingAttachment = attachment as ClippingAttachment;
						if (clippingAttachment != null) {
							clippingEndSlot = clippingAttachment.EndSlot;
							clippingAttachmentSource = i;
							current.hasClipping = true;
							skeletonHasClipping = true;
						}
#endif
						noRender = true;
					}
				}

				// Create a new SubmeshInstruction when material changes. (or when forced to separate by a submeshSeparator)
				// Slot with a separator/new material will become the starting slot of the next new instruction.
				if (hasSeparators) { //current.forceSeparate = hasSeparators && separatorSlots.Contains(slot);
					current.forceSeparate = false;
					for (int s = 0; s < separatorCount; s++) {
						if (Slot.ReferenceEquals(slot, separatorSlots[s])) {
							current.forceSeparate = true;
							break;
						}
					}
				}

				if (noRender) {
					if (current.forceSeparate && generateMeshOverride) { // && current.rawVertexCount > 0) {
						{ // Add
							current.endSlot = i;
							current.preActiveClippingSlotSource = lastPreActiveClipping;

							workingSubmeshInstructions.Resize(submeshIndex + 1);
							workingSubmeshInstructions.Items[submeshIndex] = current;

							submeshIndex++;
						}

						current.startSlot = i;
						lastPreActiveClipping = clippingAttachmentSource;
#if SPINE_TRIANGLECHECK
						current.rawTriangleCount = 0;
						current.rawVertexCount = 0;
						current.rawFirstVertexIndex = totalRawVertexCount;
						current.hasClipping = clippingAttachmentSource >= 0;
#endif
					}
				} else {
#if !SPINE_TK2D
					Material material;
					if (isCustomSlotMaterialsPopulated) {
						if (!customSlotMaterials.TryGetValue(slot, out material))
							material = (Material)((AtlasRegion)region).page.rendererObject;
					} else {
						material = (Material)((AtlasRegion)region).page.rendererObject;
					}
#else
					// An AtlasRegion in plain spine-unity, spine-TK2D hooks into TK2D's system. eventual source of Material object.
					Material material = (region is Material) ? (Material)region : (Material)((AtlasRegion)region).page.rendererObject;
#endif

#if !SPINE_TRIANGLECHECK
					if (current.forceSeparate || !System.Object.ReferenceEquals(current.material, material)) { // Material changed. Add the previous submesh.
#else
					if (current.forceSeparate || (current.rawVertexCount > 0 && !System.Object.ReferenceEquals(current.material, material))) { // Material changed. Add the previous submesh.
#endif
						{ // Add
							current.endSlot = i;
							current.preActiveClippingSlotSource = lastPreActiveClipping;

							workingSubmeshInstructions.Resize(submeshIndex + 1);
							workingSubmeshInstructions.Items[submeshIndex] = current;
							submeshIndex++;
						}
						current.startSlot = i;
						lastPreActiveClipping = clippingAttachmentSource;
#if SPINE_TRIANGLECHECK
						current.rawTriangleCount = 0;
						current.rawVertexCount = 0;
						current.rawFirstVertexIndex = totalRawVertexCount;
						current.hasClipping = clippingAttachmentSource >= 0;
#endif
					}

					// Update state for the next Attachment.
					current.material = material;
#if SPINE_TRIANGLECHECK
					current.rawTriangleCount += attachmentTriangleCount;
					current.rawVertexCount += attachmentVertexCount;
					current.rawFirstVertexIndex = totalRawVertexCount;
					totalRawVertexCount += attachmentVertexCount;
#endif
				}

				if (clippingEndSlot != null && slot.Data == clippingEndSlot && i != clippingAttachmentSource) {
					clippingEndSlot = null;
					clippingAttachmentSource = -1;
				}
			}

			if (current.rawVertexCount > 0) {
				{ // Add last or only submesh.
					current.endSlot = drawOrderCount;
					current.preActiveClippingSlotSource = lastPreActiveClipping;
					current.forceSeparate = false;

					workingSubmeshInstructions.Resize(submeshIndex + 1);
					workingSubmeshInstructions.Items[submeshIndex] = current;
					//submeshIndex++;
				}
			}

#if SPINE_TRIANGLECHECK
			instructionOutput.hasActiveClipping = skeletonHasClipping;
			instructionOutput.rawVertexCount = totalRawVertexCount;
#endif
			instructionOutput.immutableTriangles = immutableTriangles;
		}

		public static void TryReplaceMaterials (ExposedList<SubmeshInstruction> workingSubmeshInstructions, Dictionary<Material, Material> customMaterialOverride) {
			// Material overrides are done here so they can be applied per submesh instead of per slot
			// but they will still be passed through the GenerateMeshOverride delegate,
			// and will still go through the normal material match check step in STEP 3.
			SubmeshInstruction[] wsii = workingSubmeshInstructions.Items;
			for (int i = 0; i < workingSubmeshInstructions.Count; i++) {
				Material material = wsii[i].material;
				if (material == null) continue;

				Material overrideMaterial;
				if (customMaterialOverride.TryGetValue(material, out overrideMaterial))
					wsii[i].material = overrideMaterial;
			}
		}
		#endregion

		#region Step 2 : Populate vertex data and triangle index buffers.
		public void Begin () {
			vertexBuffer.Clear(false);
			colorBuffer.Clear(false);
			uvBuffer.Clear(false);
			clipper.ClipEnd();

			{
				meshBoundsMin.x = BoundsMinDefault;
				meshBoundsMin.y = BoundsMinDefault;
				meshBoundsMax.x = BoundsMaxDefault;
				meshBoundsMax.y = BoundsMaxDefault;
				meshBoundsThickness = 0f;
			}

			submeshIndex = 0;
			submeshes.Count = 1;
			//submeshes.Items[0].Clear(false);
		}

		public void AddSubmesh (SubmeshInstruction instruction, bool updateTriangles = true) {
			Settings settings = this.settings;

			int newSubmeshCount = submeshIndex + 1;
			if (submeshes.Items.Length < newSubmeshCount)
				submeshes.Resize(newSubmeshCount);
			submeshes.Count = newSubmeshCount;
			ExposedList<int> submesh = submeshes.Items[submeshIndex];
			if (submesh == null)
				submeshes.Items[submeshIndex] = submesh = new ExposedList<int>();
			submesh.Clear(false);

			Skeleton skeleton = instruction.skeleton;
			Slot[] drawOrderItems = skeleton.DrawOrder.Items;

			Color32 color = default(Color32);
			float skeletonA = skeleton.A, skeletonR = skeleton.R, skeletonG = skeleton.G, skeletonB = skeleton.B;
			Vector2 meshBoundsMin = this.meshBoundsMin, meshBoundsMax = this.meshBoundsMax;

			// Settings
			float zSpacing = settings.zSpacing;
			bool pmaVertexColors = settings.pmaVertexColors;
			bool tintBlack = settings.tintBlack;
#if LINEAR_COLOR_SPACE_FIX_ADDITIVE_ALPHA
			bool linearColorSpace = QualitySettings.activeColorSpace == ColorSpace.Linear;
#endif

#if SPINE_TRIANGLECHECK
			bool useClipping = settings.useClipping && instruction.hasClipping;
#else
			bool useClipping = settings.useClipping;
#endif
			bool canvasGroupTintBlack = settings.tintBlack && settings.canvasGroupCompatible;

			if (useClipping) {
				if (instruction.preActiveClippingSlotSource >= 0) {
					Slot slot = drawOrderItems[instruction.preActiveClippingSlotSource];
					clipper.ClipStart(slot, slot.Attachment as ClippingAttachment);
				}
			}

			for (int slotIndex = instruction.startSlot; slotIndex < instruction.endSlot; slotIndex++) {
				Slot slot = drawOrderItems[slotIndex];
				if (!slot.Bone.Active) {
					clipper.ClipEnd(slot);
					continue;
				}
				Attachment attachment = slot.Attachment;
				float z = zSpacing * slotIndex;

				float[] workingVerts = this.tempVerts;
				float[] uvs;
				int[] attachmentTriangleIndices;
				int attachmentVertexCount;
				int attachmentIndexCount;

				Color c = default(Color);

				// Identify and prepare values.
				RegionAttachment region = attachment as RegionAttachment;
				if (region != null) {
					region.ComputeWorldVertices(slot, workingVerts, 0);
					uvs = region.UVs;
					attachmentTriangleIndices = regionTriangles;
					c.r = region.R; c.g = region.G; c.b = region.B; c.a = region.A;
					attachmentVertexCount = 4;
					attachmentIndexCount = 6;
				} else {
					MeshAttachment mesh = attachment as MeshAttachment;
					if (mesh != null) {
						int meshVerticesLength = mesh.WorldVerticesLength;
						if (workingVerts.Length < meshVerticesLength) {
							workingVerts = new float[meshVerticesLength];
							this.tempVerts = workingVerts;
						}
						mesh.ComputeWorldVertices(slot, 0, meshVerticesLength, workingVerts, 0); //meshAttachment.ComputeWorldVertices(slot, tempVerts);
						uvs = mesh.UVs;
						attachmentTriangleIndices = mesh.Triangles;
						c.r = mesh.R; c.g = mesh.G; c.b = mesh.B; c.a = mesh.A;
						attachmentVertexCount = meshVerticesLength >> 1; // meshVertexCount / 2;
						attachmentIndexCount = mesh.Triangles.Length;
					} else {
						if (useClipping) {
							ClippingAttachment clippingAttachment = attachment as ClippingAttachment;
							if (clippingAttachment != null) {
								clipper.ClipStart(slot, clippingAttachment);
								continue;
							}
						}

						// If not any renderable attachment.
						clipper.ClipEnd(slot);
						continue;
					}
				}

				float tintBlackAlpha = 1.0f;
				if (pmaVertexColors) {
					float alpha = skeletonA * slot.A * c.a;
					bool isAdditiveSlot = slot.Data.BlendMode == BlendMode.Additive;
#if LINEAR_COLOR_SPACE_FIX_ADDITIVE_ALPHA
					if (linearColorSpace && isAdditiveSlot)
						alpha = Mathf.LinearToGammaSpace(alpha); // compensate GammaToLinear performed in shader
#endif
					color.a = (byte)(alpha * 255);
					color.r = (byte)(skeletonR * slot.R * c.r * color.a);
					color.g = (byte)(skeletonG * slot.G * c.g * color.a);
					color.b = (byte)(skeletonB * slot.B * c.b * color.a);
					if (canvasGroupTintBlack) {
						tintBlackAlpha = isAdditiveSlot ? 0 : alpha;
						color.a = 255;
					} else {
						if (isAdditiveSlot)
							color.a = 0;
					}
				} else {
					color.a = (byte)(skeletonA * slot.A * c.a * 255);
					color.r = (byte)(skeletonR * slot.R * c.r * 255);
					color.g = (byte)(skeletonG * slot.G * c.g * 255);
					color.b = (byte)(skeletonB * slot.B * c.b * 255);
				}

				if (useClipping && clipper.IsClipping) {
					clipper.ClipTriangles(workingVerts, attachmentTriangleIndices, attachmentIndexCount, uvs);
					workingVerts = clipper.ClippedVertices.Items;
					attachmentVertexCount = clipper.ClippedVertices.Count >> 1;
					attachmentTriangleIndices = clipper.ClippedTriangles.Items;
					attachmentIndexCount = clipper.ClippedTriangles.Count;
					uvs = clipper.ClippedUVs.Items;
				}

				// Actually add slot/attachment data into buffers.
				if (attachmentVertexCount != 0 && attachmentIndexCount != 0) {
					if (tintBlack) {
						float r2 = slot.R2;
						float g2 = slot.G2;
						float b2 = slot.B2;
						if (pmaVertexColors) {
							float alpha = skeletonA * slot.A * c.a;
#if LINEAR_COLOR_SPACE_FIX_ADDITIVE_ALPHA
							bool isAdditiveSlot = slot.Data.BlendMode == BlendMode.Additive;
							if (linearColorSpace && isAdditiveSlot)
								alpha = Mathf.LinearToGammaSpace(alpha); // compensate GammaToLinear performed in shader
#endif
							r2 *= alpha;
							g2 *= alpha;
							b2 *= alpha;
						}
						AddAttachmentTintBlack(r2, g2, b2, tintBlackAlpha, attachmentVertexCount);
					}

					//AddAttachment(workingVerts, uvs, color, attachmentTriangleIndices, attachmentVertexCount, attachmentIndexCount, ref meshBoundsMin, ref meshBoundsMax, z);
					int ovc = vertexBuffer.Count;
					// Add data to vertex buffers
					{
						int newVertexCount = ovc + attachmentVertexCount;
						int oldArraySize = vertexBuffer.Items.Length;
						if (newVertexCount > oldArraySize) {
							int newArraySize = (int)(oldArraySize * 1.3f);
							if (newArraySize < newVertexCount) newArraySize = newVertexCount;
							Array.Resize(ref vertexBuffer.Items, newArraySize);
							Array.Resize(ref uvBuffer.Items, newArraySize);
							Array.Resize(ref colorBuffer.Items, newArraySize);
						}
						vertexBuffer.Count = uvBuffer.Count = colorBuffer.Count = newVertexCount;
					}

					Vector3[] vbi = vertexBuffer.Items;
					Vector2[] ubi = uvBuffer.Items;
					Color32[] cbi = colorBuffer.Items;
					if (ovc == 0) {
						for (int i = 0; i < attachmentVertexCount; i++) {
							int vi = ovc + i;
							int i2 = i << 1; // i * 2
							float x = workingVerts[i2];
							float y = workingVerts[i2 + 1];

							vbi[vi].x = x;
							vbi[vi].y = y;
							vbi[vi].z = z;
							ubi[vi].x = uvs[i2];
							ubi[vi].y = uvs[i2 + 1];
							cbi[vi] = color;

							// Calculate bounds.
							if (x < meshBoundsMin.x) meshBoundsMin.x = x;
							if (x > meshBoundsMax.x) meshBoundsMax.x = x;
							if (y < meshBoundsMin.y) meshBoundsMin.y = y;
							if (y > meshBoundsMax.y) meshBoundsMax.y = y;
						}
					} else {
						for (int i = 0; i < attachmentVertexCount; i++) {
							int vi = ovc + i;
							int i2 = i << 1; // i * 2
							float x = workingVerts[i2];
							float y = workingVerts[i2 + 1];

							vbi[vi].x = x;
							vbi[vi].y = y;
							vbi[vi].z = z;
							ubi[vi].x = uvs[i2];
							ubi[vi].y = uvs[i2 + 1];
							cbi[vi] = color;

							// Calculate bounds.
							if (x < meshBoundsMin.x) meshBoundsMin.x = x;
							else if (x > meshBoundsMax.x) meshBoundsMax.x = x;
							if (y < meshBoundsMin.y) meshBoundsMin.y = y;
							else if (y > meshBoundsMax.y) meshBoundsMax.y = y;
						}
					}


					// Add data to triangle buffer
					if (updateTriangles) {
						int oldTriangleCount = submesh.Count;
						{ //submesh.Resize(oldTriangleCount + attachmentIndexCount);
							int newTriangleCount = oldTriangleCount + attachmentIndexCount;
							if (newTriangleCount > submesh.Items.Length) Array.Resize(ref submesh.Items, newTriangleCount);
							submesh.Count = newTriangleCount;
						}
						int[] submeshItems = submesh.Items;
						for (int i = 0; i < attachmentIndexCount; i++)
							submeshItems[oldTriangleCount + i] = attachmentTriangleIndices[i] + ovc;
					}
				}

				clipper.ClipEnd(slot);
			}
			clipper.ClipEnd();

			this.meshBoundsMin = meshBoundsMin;
			this.meshBoundsMax = meshBoundsMax;
			meshBoundsThickness = instruction.endSlot * zSpacing;

			// Trim or zero submesh triangles.
			int[] currentSubmeshItems = submesh.Items;
			for (int i = submesh.Count, n = currentSubmeshItems.Length; i < n; i++)
				currentSubmeshItems[i] = 0;

			submeshIndex++; // Next AddSubmesh will use a new submeshIndex value.
		}

		public void BuildMesh (SkeletonRendererInstruction instruction, bool updateTriangles) {
			SubmeshInstruction[] wsii = instruction.submeshInstructions.Items;
			for (int i = 0, n = instruction.submeshInstructions.Count; i < n; i++)
				this.AddSubmesh(wsii[i], updateTriangles);
		}

		// Use this faster method when no clipping is involved.
		public void BuildMeshWithArrays (SkeletonRendererInstruction instruction, bool updateTriangles) {
#if !SPINE_TRIANGLECHECK
			return;
#else
			Settings settings = this.settings;
			bool canvasGroupTintBlack = settings.tintBlack && settings.canvasGroupCompatible;
			int totalVertexCount = instruction.rawVertexCount;

#if LINEAR_COLOR_SPACE_FIX_ADDITIVE_ALPHA
			bool linearColorSpace = QualitySettings.activeColorSpace == ColorSpace.Linear;
#endif
			// Add data to vertex buffers
			{
				if (totalVertexCount > vertexBuffer.Items.Length) { // Manual ExposedList.Resize()
					Array.Resize(ref vertexBuffer.Items, totalVertexCount);
					Array.Resize(ref uvBuffer.Items, totalVertexCount);
					Array.Resize(ref colorBuffer.Items, totalVertexCount);
				}
				vertexBuffer.Count = uvBuffer.Count = colorBuffer.Count = totalVertexCount;
			}

			// Populate Verts
			Color32 color = default(Color32);

			int vertexIndex = 0;
			float[] tempVerts = this.tempVerts;
			Vector2 bmin = this.meshBoundsMin;
			Vector2 bmax = this.meshBoundsMax;

			Vector3[] vbi = vertexBuffer.Items;
			Vector2[] ubi = uvBuffer.Items;
			Color32[] cbi = colorBuffer.Items;
			int lastSlotIndex = 0;

			// drawOrder[endSlot] is excluded
			for (int si = 0, n = instruction.submeshInstructions.Count; si < n; si++) {
				SubmeshInstruction submesh = instruction.submeshInstructions.Items[si];
				Skeleton skeleton = submesh.skeleton;
				Slot[] drawOrderItems = skeleton.DrawOrder.Items;
				float a = skeleton.A, r = skeleton.R, g = skeleton.G, b = skeleton.B;

				int endSlot = submesh.endSlot;
				int startSlot = submesh.startSlot;
				lastSlotIndex = endSlot;

				if (settings.tintBlack) {
					Vector2 rg, b2;
					int vi = vertexIndex;
					b2.y = 1f;

					PrepareOptionalUVBuffer(ref uv2, totalVertexCount);
					PrepareOptionalUVBuffer(ref uv3, totalVertexCount);

					Vector2[] uv2i = uv2.Items;
					Vector2[] uv3i = uv3.Items;

					for (int slotIndex = startSlot; slotIndex < endSlot; slotIndex++) {
						Slot slot = drawOrderItems[slotIndex];
						if (!slot.Bone.Active
#if SLOT_ALPHA_DISABLES_ATTACHMENT
							|| slot.A == 0f
#endif
							) continue;
						Attachment attachment = slot.Attachment;

						rg.x = slot.R2; //r
						rg.y = slot.G2; //g
						b2.x = slot.B2; //b
						b2.y = 1.0f;

						RegionAttachment regionAttachment = attachment as RegionAttachment;
						if (regionAttachment != null) {
							if (settings.pmaVertexColors) {
								float alpha = a * slot.A * regionAttachment.A;
								bool isAdditiveSlot = slot.Data.BlendMode == BlendMode.Additive;
#if LINEAR_COLOR_SPACE_FIX_ADDITIVE_ALPHA
								if (linearColorSpace && isAdditiveSlot)
									alpha = Mathf.LinearToGammaSpace(alpha); // compensate GammaToLinear performed in shader
#endif
								rg.x *= alpha;
								rg.y *= alpha;
								b2.x *= alpha;
								b2.y = isAdditiveSlot ? 0 : alpha;
							}
							uv2i[vi] = rg; uv2i[vi + 1] = rg; uv2i[vi + 2] = rg; uv2i[vi + 3] = rg;
							uv3i[vi] = b2; uv3i[vi + 1] = b2; uv3i[vi + 2] = b2; uv3i[vi + 3] = b2;
							vi += 4;
						} else { //} if (settings.renderMeshes) {
							MeshAttachment meshAttachment = attachment as MeshAttachment;
							if (meshAttachment != null) {
								if (settings.pmaVertexColors) {
									float alpha = a * slot.A * meshAttachment.A;
									bool isAdditiveSlot = slot.Data.BlendMode == BlendMode.Additive;
#if LINEAR_COLOR_SPACE_FIX_ADDITIVE_ALPHA
									if (linearColorSpace && isAdditiveSlot)
										alpha = Mathf.LinearToGammaSpace(alpha); // compensate GammaToLinear performed in shader
#endif
									rg.x *= alpha;
									rg.y *= alpha;
									b2.x *= alpha;
									b2.y = isAdditiveSlot ? 0 : alpha;
								}
								int verticesArrayLength = meshAttachment.WorldVerticesLength;
								for (int iii = 0; iii < verticesArrayLength; iii += 2) {
									uv2i[vi] = rg;
									uv3i[vi] = b2;
									vi++;
								}
							}
						}
					}
				}

				for (int slotIndex = startSlot; slotIndex < endSlot; slotIndex++) {
					Slot slot = drawOrderItems[slotIndex];
					if (!slot.Bone.Active
#if SLOT_ALPHA_DISABLES_ATTACHMENT
						|| slot.A == 0f
#endif
						) continue;
					Attachment attachment = slot.Attachment;
					float z = slotIndex * settings.zSpacing;

					RegionAttachment regionAttachment = attachment as RegionAttachment;
					if (regionAttachment != null) {
						regionAttachment.ComputeWorldVertices(slot, tempVerts, 0);

						float x1 = tempVerts[RegionAttachment.BLX], y1 = tempVerts[RegionAttachment.BLY];
						float x2 = tempVerts[RegionAttachment.ULX], y2 = tempVerts[RegionAttachment.ULY];
						float x3 = tempVerts[RegionAttachment.URX], y3 = tempVerts[RegionAttachment.URY];
						float x4 = tempVerts[RegionAttachment.BRX], y4 = tempVerts[RegionAttachment.BRY];
						vbi[vertexIndex].x = x1; vbi[vertexIndex].y = y1; vbi[vertexIndex].z = z;
						vbi[vertexIndex + 1].x = x4; vbi[vertexIndex + 1].y = y4; vbi[vertexIndex + 1].z = z;
						vbi[vertexIndex + 2].x = x2; vbi[vertexIndex + 2].y = y2; vbi[vertexIndex + 2].z = z;
						vbi[vertexIndex + 3].x = x3; vbi[vertexIndex + 3].y = y3; vbi[vertexIndex + 3].z = z;

						if (settings.pmaVertexColors) {
							float alpha = a * slot.A * regionAttachment.A;
							bool isAdditiveSlot = slot.Data.BlendMode == BlendMode.Additive;
#if LINEAR_COLOR_SPACE_FIX_ADDITIVE_ALPHA
							if (linearColorSpace && isAdditiveSlot)
								alpha = Mathf.LinearToGammaSpace(alpha); // compensate GammaToLinear performed in shader
#endif
							color.a = (byte)(alpha * 255);
							color.r = (byte)(r * slot.R * regionAttachment.R * color.a);
							color.g = (byte)(g * slot.G * regionAttachment.G * color.a);
							color.b = (byte)(b * slot.B * regionAttachment.B * color.a);
							if (canvasGroupTintBlack) color.a = 255;
							else if (isAdditiveSlot) color.a = 0;

						} else {
							color.a = (byte)(a * slot.A * regionAttachment.A * 255);
							color.r = (byte)(r * slot.R * regionAttachment.R * 255);
							color.g = (byte)(g * slot.G * regionAttachment.G * 255);
							color.b = (byte)(b * slot.B * regionAttachment.B * 255);
						}

						cbi[vertexIndex] = color; cbi[vertexIndex + 1] = color; cbi[vertexIndex + 2] = color; cbi[vertexIndex + 3] = color;

						float[] regionUVs = regionAttachment.UVs;
						ubi[vertexIndex].x = regionUVs[RegionAttachment.BLX]; ubi[vertexIndex].y = regionUVs[RegionAttachment.BLY];
						ubi[vertexIndex + 1].x = regionUVs[RegionAttachment.BRX]; ubi[vertexIndex + 1].y = regionUVs[RegionAttachment.BRY];
						ubi[vertexIndex + 2].x = regionUVs[RegionAttachment.ULX]; ubi[vertexIndex + 2].y = regionUVs[RegionAttachment.ULY];
						ubi[vertexIndex + 3].x = regionUVs[RegionAttachment.URX]; ubi[vertexIndex + 3].y = regionUVs[RegionAttachment.URY];

						if (x1 < bmin.x) bmin.x = x1; // Potential first attachment bounds initialization. Initial min should not block initial max. Same for Y below.
						if (x1 > bmax.x) bmax.x = x1;
						if (x2 < bmin.x) bmin.x = x2;
						else if (x2 > bmax.x) bmax.x = x2;
						if (x3 < bmin.x) bmin.x = x3;
						else if (x3 > bmax.x) bmax.x = x3;
						if (x4 < bmin.x) bmin.x = x4;
						else if (x4 > bmax.x) bmax.x = x4;

						if (y1 < bmin.y) bmin.y = y1;
						if (y1 > bmax.y) bmax.y = y1;
						if (y2 < bmin.y) bmin.y = y2;
						else if (y2 > bmax.y) bmax.y = y2;
						if (y3 < bmin.y) bmin.y = y3;
						else if (y3 > bmax.y) bmax.y = y3;
						if (y4 < bmin.y) bmin.y = y4;
						else if (y4 > bmax.y) bmax.y = y4;

						vertexIndex += 4;
					} else { //if (settings.renderMeshes) {
						MeshAttachment meshAttachment = attachment as MeshAttachment;
						if (meshAttachment != null) {
							int verticesArrayLength = meshAttachment.WorldVerticesLength;
							if (tempVerts.Length < verticesArrayLength) this.tempVerts = tempVerts = new float[verticesArrayLength];
							meshAttachment.ComputeWorldVertices(slot, tempVerts);

							if (settings.pmaVertexColors) {
								float alpha = a * slot.A * meshAttachment.A;
								bool isAdditiveSlot = slot.Data.BlendMode == BlendMode.Additive;
#if LINEAR_COLOR_SPACE_FIX_ADDITIVE_ALPHA
								if (linearColorSpace && isAdditiveSlot)
									alpha = Mathf.LinearToGammaSpace(alpha); // compensate GammaToLinear performed in shader
#endif
								color.a = (byte)(alpha * 255);
								color.r = (byte)(r * slot.R * meshAttachment.R * color.a);
								color.g = (byte)(g * slot.G * meshAttachment.G * color.a);
								color.b = (byte)(b * slot.B * meshAttachment.B * color.a);
								if (canvasGroupTintBlack) color.a = 255;
								else if (isAdditiveSlot) color.a = 0;
							} else {
								color.a = (byte)(a * slot.A * meshAttachment.A * 255);
								color.r = (byte)(r * slot.R * meshAttachment.R * 255);
								color.g = (byte)(g * slot.G * meshAttachment.G * 255);
								color.b = (byte)(b * slot.B * meshAttachment.B * 255);
							}

							float[] attachmentUVs = meshAttachment.UVs;

							// Potential first attachment bounds initialization. See conditions in RegionAttachment logic.
							if (vertexIndex == 0) {
								// Initial min should not block initial max.
								// vi == vertexIndex does not always mean the bounds are fresh. It could be a submesh. Do not nuke old values by omitting the check.
								// Should know that this is the first attachment in the submesh. slotIndex == startSlot could be an empty slot.
								float fx = tempVerts[0], fy = tempVerts[1];
								if (fx < bmin.x) bmin.x = fx;
								if (fx > bmax.x) bmax.x = fx;
								if (fy < bmin.y) bmin.y = fy;
								if (fy > bmax.y) bmax.y = fy;
							}

							for (int iii = 0; iii < verticesArrayLength; iii += 2) {
								float x = tempVerts[iii], y = tempVerts[iii + 1];
								vbi[vertexIndex].x = x; vbi[vertexIndex].y = y; vbi[vertexIndex].z = z;
								cbi[vertexIndex] = color; ubi[vertexIndex].x = attachmentUVs[iii]; ubi[vertexIndex].y = attachmentUVs[iii + 1];

								if (x < bmin.x) bmin.x = x;
								else if (x > bmax.x) bmax.x = x;

								if (y < bmin.y) bmin.y = y;
								else if (y > bmax.y) bmax.y = y;

								vertexIndex++;
							}
						}
					}
				}
			}

			this.meshBoundsMin = bmin;
			this.meshBoundsMax = bmax;
			this.meshBoundsThickness = lastSlotIndex * settings.zSpacing;

			int submeshInstructionCount = instruction.submeshInstructions.Count;
			submeshes.Count = submeshInstructionCount;

			// Add triangles
			if (updateTriangles) {
				// Match submesh buffers count with submeshInstruction count.
				if (this.submeshes.Items.Length < submeshInstructionCount) {
					this.submeshes.Resize(submeshInstructionCount);
					for (int i = 0, n = submeshInstructionCount; i < n; i++) {
						ExposedList<int> submeshBuffer = this.submeshes.Items[i];
						if (submeshBuffer == null)
							this.submeshes.Items[i] = new ExposedList<int>();
						else
							submeshBuffer.Clear(false);
					}
				}

				SubmeshInstruction[] submeshInstructionsItems = instruction.submeshInstructions.Items; // This relies on the resize above.

				// Fill the buffers.
				int attachmentFirstVertex = 0;
				for (int smbi = 0; smbi < submeshInstructionCount; smbi++) {
					SubmeshInstruction submeshInstruction = submeshInstructionsItems[smbi];
					ExposedList<int> currentSubmeshBuffer = this.submeshes.Items[smbi];
					{ //submesh.Resize(submesh.rawTriangleCount);
						int newTriangleCount = submeshInstruction.rawTriangleCount;
						if (newTriangleCount > currentSubmeshBuffer.Items.Length)
							Array.Resize(ref currentSubmeshBuffer.Items, newTriangleCount);
						else if (newTriangleCount < currentSubmeshBuffer.Items.Length) {
							// Zero the extra.
							int[] sbi = currentSubmeshBuffer.Items;
							for (int ei = newTriangleCount, nn = sbi.Length; ei < nn; ei++)
								sbi[ei] = 0;
						}
						currentSubmeshBuffer.Count = newTriangleCount;
					}

					int[] tris = currentSubmeshBuffer.Items;
					int triangleIndex = 0;
					Skeleton skeleton = submeshInstruction.skeleton;
					Slot[] drawOrderItems = skeleton.DrawOrder.Items;
					for (int slotIndex = submeshInstruction.startSlot, endSlot = submeshInstruction.endSlot; slotIndex < endSlot; slotIndex++) {
						Slot slot = drawOrderItems[slotIndex];
						if (!slot.Bone.Active
#if SLOT_ALPHA_DISABLES_ATTACHMENT
							|| slot.A == 0f
#endif
							) continue;

						Attachment attachment = drawOrderItems[slotIndex].Attachment;
						if (attachment is RegionAttachment) {
							tris[triangleIndex] = attachmentFirstVertex;
							tris[triangleIndex + 1] = attachmentFirstVertex + 2;
							tris[triangleIndex + 2] = attachmentFirstVertex + 1;
							tris[triangleIndex + 3] = attachmentFirstVertex + 2;
							tris[triangleIndex + 4] = attachmentFirstVertex + 3;
							tris[triangleIndex + 5] = attachmentFirstVertex + 1;
							triangleIndex += 6;
							attachmentFirstVertex += 4;
							continue;
						}
						MeshAttachment meshAttachment = attachment as MeshAttachment;
						if (meshAttachment != null) {
							int[] attachmentTriangles = meshAttachment.Triangles;
							for (int ii = 0, nn = attachmentTriangles.Length; ii < nn; ii++, triangleIndex++)
								tris[triangleIndex] = attachmentFirstVertex + attachmentTriangles[ii];
							attachmentFirstVertex += meshAttachment.WorldVerticesLength >> 1; // length/2;
						}
					}
				}
			}
#endif // SPINE_TRIANGLECHECK
		}

		public void ScaleVertexData (float scale) {
			Vector3[] vbi = vertexBuffer.Items;
			for (int i = 0, n = vertexBuffer.Count; i < n; i++) {
#if MANUALLY_INLINE_VECTOR_OPERATORS
				vbi[i].x *= scale;
				vbi[i].y *= scale;
				vbi[i].z *= scale;
#else
				vbi[i] *= scale;
#endif
			}

			meshBoundsMin *= scale;
			meshBoundsMax *= scale;
			meshBoundsThickness *= scale;
		}

		public void ScaleAndOffsetVertexData (float scale, Vector2 offset2D) {
			Vector3 offset = new Vector3(offset2D.x, offset2D.y);
			Vector3[] vbi = vertexBuffer.Items;
			for (int i = 0, n = vertexBuffer.Count; i < n; i++) {
#if MANUALLY_INLINE_VECTOR_OPERATORS
				vbi[i].x = vbi[i].x * scale + offset.x;
				vbi[i].y = vbi[i].y * scale + offset.y;
				vbi[i].z = vbi[i].z * scale + offset.z;
#else
				vbi[i] = vbi[i] * scale + offset;
#endif
			}

			meshBoundsMin *= scale;
			meshBoundsMax *= scale;
			meshBoundsMin += offset2D;
			meshBoundsMax += offset2D;
			meshBoundsThickness *= scale;
		}

		public Bounds GetMeshBounds () {
			if (float.IsInfinity(meshBoundsMin.x)) { // meshBoundsMin.x == BoundsMinDefault // == doesn't work on float Infinity constants.
				return new Bounds();
			} else {
				//mesh.bounds = ArraysMeshGenerator.ToBounds(meshBoundsMin, meshBoundsMax);
				float halfWidth = (meshBoundsMax.x - meshBoundsMin.x) * 0.5f;
				float halfHeight = (meshBoundsMax.y - meshBoundsMin.y) * 0.5f;
				return new Bounds {
					center = new Vector3(meshBoundsMin.x + halfWidth, meshBoundsMin.y + halfHeight),
					extents = new Vector3(halfWidth, halfHeight, meshBoundsThickness * 0.5f)
				};
			}
		}

		void AddAttachmentTintBlack (float r2, float g2, float b2, float a, int vertexCount) {
			Vector2 rg = new Vector2(r2, g2);
			Vector2 bo = new Vector2(b2, a);

			int ovc = vertexBuffer.Count;
			int newVertexCount = ovc + vertexCount;

			PrepareOptionalUVBuffer(ref uv2, newVertexCount);
			PrepareOptionalUVBuffer(ref uv3, newVertexCount);

			Vector2[] uv2i = uv2.Items;
			Vector2[] uv3i = uv3.Items;
			for (int i = 0; i < vertexCount; i++) {
				uv2i[ovc + i] = rg;
				uv3i[ovc + i] = bo;
			}
		}

		void PrepareOptionalUVBuffer (ref ExposedList<Vector2> uvBuffer, int vertexCount) {
			if (uvBuffer == null) {
				uvBuffer = new ExposedList<Vector2>();
			}
			if (vertexCount > uvBuffer.Items.Length) { // Manual ExposedList.Resize()
				Array.Resize(ref uvBuffer.Items, vertexCount);
			}
			uvBuffer.Count = vertexCount;
		}

		void ResizeOptionalUVBuffer (ref ExposedList<Vector2> uvBuffer, int vertexCount) {
			if (uvBuffer != null) {
				if (vertexCount != uvBuffer.Items.Length) {
					Array.Resize(ref uvBuffer.Items, vertexCount);
					uvBuffer.Count = vertexCount;
				}
			}
		}
		#endregion

		#region Step 3 : Transfer vertex and triangle data to UnityEngine.Mesh
		public void FillVertexData (Mesh mesh) {
			Vector3[] vbi = vertexBuffer.Items;
			Vector2[] ubi = uvBuffer.Items;
			Color32[] cbi = colorBuffer.Items;
			int vbiLength = vbi.Length;

			// Zero the extra.
			{
				int listCount = vertexBuffer.Count;
				// unfortunately even non-indexed vertices are still used by Unity's bounds computation,
				// (considered a Unity bug), thus avoid Vector3.zero and use last vertex instead.
				Vector3 extraVertex = listCount == 0 ? Vector3.zero : vbi[listCount - 1];
				for (int i = listCount; i < vbiLength; i++)
					vbi[i] = extraVertex;
			}

			// Set the vertex buffer.
			{
				mesh.vertices = vbi;
				mesh.uv = ubi;
				mesh.colors32 = cbi;
				mesh.bounds = GetMeshBounds();
			}

			{
				if (settings.addNormals) {
					int oldLength = 0;

					if (normals == null)
						normals = new Vector3[vbiLength];
					else
						oldLength = normals.Length;

					if (oldLength != vbiLength) {
						Array.Resize(ref this.normals, vbiLength);
						Vector3[] localNormals = this.normals;
						for (int i = oldLength; i < vbiLength; i++) localNormals[i] = Vector3.back;
					}
					mesh.normals = this.normals;
				}

				// Sometimes, the vertex buffer becomes smaller. We need to trim the size of
				// the uv2 and uv3 buffers (used for tint black) to match.
				ResizeOptionalUVBuffer(ref uv2, vbiLength);
				ResizeOptionalUVBuffer(ref uv3, vbiLength);
				mesh.uv2 = this.uv2 == null ? null : this.uv2.Items;
				mesh.uv3 = this.uv3 == null ? null : this.uv3.Items;
			}
		}

		public void FillLateVertexData (Mesh mesh) {
			if (settings.calculateTangents) {
				int vertexCount = this.vertexBuffer.Count;
				ExposedList<int>[] sbi = submeshes.Items;
				int submeshCount = submeshes.Count;
				Vector3[] vbi = vertexBuffer.Items;
				Vector2[] ubi = uvBuffer.Items;

				MeshGenerator.SolveTangents2DEnsureSize(ref this.tangents, ref this.tempTanBuffer, vertexCount, vbi.Length);
				for (int i = 0; i < submeshCount; i++) {
					int[] submesh = sbi[i].Items;
					int triangleCount = sbi[i].Count;
					MeshGenerator.SolveTangents2DTriangles(this.tempTanBuffer, submesh, triangleCount, vbi, ubi, vertexCount);
				}
				MeshGenerator.SolveTangents2DBuffer(this.tangents, this.tempTanBuffer, vertexCount);
				mesh.tangents = this.tangents;
			}
		}

		public void FillTriangles (Mesh mesh) {
			int submeshCount = submeshes.Count;
			ExposedList<int>[] submeshesItems = submeshes.Items;
			mesh.subMeshCount = submeshCount;

			for (int i = 0; i < submeshCount; i++)
#if MESH_SET_TRIANGLES_PROVIDES_LENGTH_PARAM
				mesh.SetTriangles(submeshesItems[i].Items, 0, submeshesItems[i].Count, i, false);
#else
				mesh.SetTriangles(submeshesItems[i].Items, i, false);
#endif
		}
		#endregion

		public void EnsureVertexCapacity (int minimumVertexCount, bool inlcudeTintBlack = false, bool includeTangents = false, bool includeNormals = false) {
			if (minimumVertexCount > vertexBuffer.Items.Length) {
				Array.Resize(ref vertexBuffer.Items, minimumVertexCount);
				Array.Resize(ref uvBuffer.Items, minimumVertexCount);
				Array.Resize(ref colorBuffer.Items, minimumVertexCount);

				if (inlcudeTintBlack) {
					if (uv2 == null) {
						uv2 = new ExposedList<Vector2>(minimumVertexCount);
						uv3 = new ExposedList<Vector2>(minimumVertexCount);
					}
					uv2.Resize(minimumVertexCount);
					uv3.Resize(minimumVertexCount);
				}

				if (includeNormals) {
					if (normals == null)
						normals = new Vector3[minimumVertexCount];
					else
						Array.Resize(ref normals, minimumVertexCount);

				}

				if (includeTangents) {
					if (tangents == null)
						tangents = new Vector4[minimumVertexCount];
					else
						Array.Resize(ref tangents, minimumVertexCount);
				}
			}
		}

		/// <summary>Trims internal buffers to reduce the resulting mesh data stream size.</summary>
		public void TrimExcess () {
			vertexBuffer.TrimExcess();
			uvBuffer.TrimExcess();
			colorBuffer.TrimExcess();

			if (uv2 != null) uv2.TrimExcess();
			if (uv3 != null) uv3.TrimExcess();

			int vbiLength = vertexBuffer.Items.Length;
			if (normals != null) Array.Resize(ref normals, vbiLength);
			if (tangents != null) Array.Resize(ref tangents, vbiLength);
		}

		#region TangentSolver2D
		// Thanks to contributions from forum user ToddRivers

		/// <summary>Step 1 of solving tangents. Ensure you have buffers of the correct size.</summary>
		/// <param name="tangentBuffer">Eventual Vector4[] tangent buffer to assign to Mesh.tangents.</param>
		/// <param name="tempTanBuffer">Temporary Vector2 buffer for calculating directions.</param>
		/// <param name="vertexCount">Number of vertices that require tangents (or the size of the vertex array)</param>
		internal static void SolveTangents2DEnsureSize (ref Vector4[] tangentBuffer, ref Vector2[] tempTanBuffer, int vertexCount, int vertexBufferLength) {
			if (tangentBuffer == null || tangentBuffer.Length != vertexBufferLength)
				tangentBuffer = new Vector4[vertexBufferLength];

			if (tempTanBuffer == null || tempTanBuffer.Length < vertexCount * 2)
				tempTanBuffer = new Vector2[vertexCount * 2]; // two arrays in one.
		}

		/// <summary>Step 2 of solving tangents. Fills (part of) a temporary tangent-solution buffer based on the vertices and uvs defined by a submesh's triangle buffer. Only needs to be called once for single-submesh meshes.</summary>
		/// <param name="tempTanBuffer">A temporary Vector3[] for calculating tangents.</param>
		/// <param name="vertices">The mesh's current vertex position buffer.</param>
		/// <param name="triangles">The mesh's current triangles buffer.</param>
		/// <param name="uvs">The mesh's current uvs buffer.</param>
		/// <param name="vertexCount">Number of vertices that require tangents (or the size of the vertex array)</param>
		/// <param name = "triangleCount">The number of triangle indexes in the triangle array to be used.</param>
		internal static void SolveTangents2DTriangles (Vector2[] tempTanBuffer, int[] triangles, int triangleCount, Vector3[] vertices, Vector2[] uvs, int vertexCount) {
			Vector2 sdir;
			Vector2 tdir;
			for (int t = 0; t < triangleCount; t += 3) {
				int i1 = triangles[t + 0];
				int i2 = triangles[t + 1];
				int i3 = triangles[t + 2];

				Vector3 v1 = vertices[i1];
				Vector3 v2 = vertices[i2];
				Vector3 v3 = vertices[i3];

				Vector2 w1 = uvs[i1];
				Vector2 w2 = uvs[i2];
				Vector2 w3 = uvs[i3];

				float x1 = v2.x - v1.x;
				float x2 = v3.x - v1.x;
				float y1 = v2.y - v1.y;
				float y2 = v3.y - v1.y;

				float s1 = w2.x - w1.x;
				float s2 = w3.x - w1.x;
				float t1 = w2.y - w1.y;
				float t2 = w3.y - w1.y;

				float div = s1 * t2 - s2 * t1;
				float r = (div == 0f) ? 0f : 1f / div;

				sdir.x = (t2 * x1 - t1 * x2) * r;
				sdir.y = (t2 * y1 - t1 * y2) * r;
				tempTanBuffer[i1] = tempTanBuffer[i2] = tempTanBuffer[i3] = sdir;

				tdir.x = (s1 * x2 - s2 * x1) * r;
				tdir.y = (s1 * y2 - s2 * y1) * r;
				tempTanBuffer[vertexCount + i1] = tempTanBuffer[vertexCount + i2] = tempTanBuffer[vertexCount + i3] = tdir;
			}
		}

		/// <summary>Step 3 of solving tangents. Fills a Vector4[] tangents array according to values calculated in step 2.</summary>
		/// <param name="tangents">A Vector4[] that will eventually be used to set Mesh.tangents</param>
		/// <param name="tempTanBuffer">A temporary Vector3[] for calculating tangents.</param>
		/// <param name="vertexCount">Number of vertices that require tangents (or the size of the vertex array)</param>
		internal static void SolveTangents2DBuffer (Vector4[] tangents, Vector2[] tempTanBuffer, int vertexCount) {
			Vector4 tangent;
			tangent.z = 0;
			for (int i = 0; i < vertexCount; ++i) {
				Vector2 t = tempTanBuffer[i];

				// t.Normalize() (aggressively inlined). Even better if offloaded to GPU via vertex shader.
				float magnitude = Mathf.Sqrt(t.x * t.x + t.y * t.y);
				if (magnitude > 1E-05) {
					float reciprocalMagnitude = 1f / magnitude;
					t.x *= reciprocalMagnitude;
					t.y *= reciprocalMagnitude;
				}

				Vector2 t2 = tempTanBuffer[vertexCount + i];
				tangent.x = t.x;
				tangent.y = t.y;
				//tangent.z = 0;
				tangent.w = (t.y * t2.x > t.x * t2.y) ? 1 : -1; // 2D direction calculation. Used for binormals.
				tangents[i] = tangent;
			}
		}
		#endregion

		#region AttachmentRendering
		static List<Vector3> AttachmentVerts = new List<Vector3>();
		static List<Vector2> AttachmentUVs = new List<Vector2>();
		static List<Color32> AttachmentColors32 = new List<Color32>();
		static List<int> AttachmentIndices = new List<int>();

		/// <summary>Fills mesh vertex data to render a RegionAttachment.</summary>
		public static void FillMeshLocal (Mesh mesh, RegionAttachment regionAttachment) {
			if (mesh == null) return;
			if (regionAttachment == null) return;

			AttachmentVerts.Clear();
			float[] offsets = regionAttachment.Offset;
			AttachmentVerts.Add(new Vector3(offsets[RegionAttachment.BLX], offsets[RegionAttachment.BLY]));
			AttachmentVerts.Add(new Vector3(offsets[RegionAttachment.ULX], offsets[RegionAttachment.ULY]));
			AttachmentVerts.Add(new Vector3(offsets[RegionAttachment.URX], offsets[RegionAttachment.URY]));
			AttachmentVerts.Add(new Vector3(offsets[RegionAttachment.BRX], offsets[RegionAttachment.BRY]));

			AttachmentUVs.Clear();
			float[] uvs = regionAttachment.UVs;
			AttachmentUVs.Add(new Vector2(uvs[RegionAttachment.ULX], uvs[RegionAttachment.ULY]));
			AttachmentUVs.Add(new Vector2(uvs[RegionAttachment.URX], uvs[RegionAttachment.URY]));
			AttachmentUVs.Add(new Vector2(uvs[RegionAttachment.BRX], uvs[RegionAttachment.BRY]));
			AttachmentUVs.Add(new Vector2(uvs[RegionAttachment.BLX], uvs[RegionAttachment.BLY]));

			AttachmentColors32.Clear();
			Color32 c = (Color32)(new Color(regionAttachment.R, regionAttachment.G, regionAttachment.B, regionAttachment.A));
			for (int i = 0; i < 4; i++)
				AttachmentColors32.Add(c);

			AttachmentIndices.Clear();
			AttachmentIndices.AddRange(new[] { 0, 2, 1, 0, 3, 2 });

			mesh.Clear();
			mesh.name = regionAttachment.Name;
			mesh.SetVertices(AttachmentVerts);
			mesh.SetUVs(0, AttachmentUVs);
			mesh.SetColors(AttachmentColors32);
			mesh.SetTriangles(AttachmentIndices, 0);
			mesh.RecalculateBounds();

			AttachmentVerts.Clear();
			AttachmentUVs.Clear();
			AttachmentColors32.Clear();
			AttachmentIndices.Clear();
		}

		public static void FillMeshLocal (Mesh mesh, MeshAttachment meshAttachment, SkeletonData skeletonData) {
			if (mesh == null) return;
			if (meshAttachment == null) return;
			int vertexCount = meshAttachment.WorldVerticesLength / 2;

			AttachmentVerts.Clear();
			if (meshAttachment.IsWeighted()) {
				int count = meshAttachment.WorldVerticesLength;
				int[] meshAttachmentBones = meshAttachment.Bones;
				int v = 0;

				float[] vertices = meshAttachment.Vertices;
				for (int w = 0, b = 0; w < count; w += 2) {
					float wx = 0, wy = 0;
					int n = meshAttachmentBones[v++];
					n += v;
					for (; v < n; v++, b += 3) {
						BoneMatrix bm = BoneMatrix.CalculateSetupWorld(skeletonData.Bones.Items[meshAttachmentBones[v]]);
						float vx = vertices[b], vy = vertices[b + 1], weight = vertices[b + 2];
						wx += (vx * bm.a + vy * bm.b + bm.x) * weight;
						wy += (vx * bm.c + vy * bm.d + bm.y) * weight;
					}
					AttachmentVerts.Add(new Vector3(wx, wy));
				}
			} else {
				float[] localVerts = meshAttachment.Vertices;
				Vector3 pos = default(Vector3);
				for (int i = 0; i < vertexCount; i++) {
					int ii = i * 2;
					pos.x = localVerts[ii];
					pos.y = localVerts[ii + 1];
					AttachmentVerts.Add(pos);
				}
			}

			float[] uvs = meshAttachment.UVs;
			Vector2 uv = default(Vector2);
			Color32 c = (Color32)(new Color(meshAttachment.R, meshAttachment.G, meshAttachment.B, meshAttachment.A));
			AttachmentUVs.Clear();
			AttachmentColors32.Clear();
			for (int i = 0; i < vertexCount; i++) {
				int ii = i * 2;
				uv.x = uvs[ii];
				uv.y = uvs[ii + 1];
				AttachmentUVs.Add(uv);

				AttachmentColors32.Add(c);
			}

			AttachmentIndices.Clear();
			AttachmentIndices.AddRange(meshAttachment.Triangles);

			mesh.Clear();
			mesh.name = meshAttachment.Name;
			mesh.SetVertices(AttachmentVerts);
			mesh.SetUVs(0, AttachmentUVs);
			mesh.SetColors(AttachmentColors32);
			mesh.SetTriangles(AttachmentIndices, 0);
			mesh.RecalculateBounds();

			AttachmentVerts.Clear();
			AttachmentUVs.Clear();
			AttachmentColors32.Clear();
			AttachmentIndices.Clear();
		}
		#endregion
	}
}
