#ifndef SKELETONLIT_FORWARD_PASS_URP_INCLUDED
#define SKELETONLIT_FORWARD_PASS_URP_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "SpineCoreShaders/Spine-Common.cginc"
#include "Spine-Common-URP.hlsl"
#include "SpineCoreShaders/Spine-Skeleton-Tint-Common.cginc"

#if (defined(_MAIN_LIGHT_SHADOWS) || defined(MAIN_LIGHT_CALCULATE_SHADOWS)) && !defined(_RECEIVE_SHADOWS_OFF)
#define SKELETONLIT_RECEIVE_SHADOWS
#endif

#if !defined(DYNAMICLIGHTMAP_ON) && !defined(LIGHTMAP_ON) && (defined(PROBE_VOLUMES_L1) || defined(PROBE_VOLUMES_L2))
#define USE_ADAPTIVE_PROBE_VOLUMES
#endif

struct appdata {
	float3 pos : POSITION;
	float3 normal : NORMAL;
	half4 color : COLOR;
	float2 uv0 : TEXCOORD0;
#if defined(_TINT_BLACK_ON)
	float2 tintBlackRG : TEXCOORD1;
	float2 tintBlackB : TEXCOORD2;
#endif
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct VertexOutput {
	half4 color : COLOR0;
	float2 uv0 : TEXCOORD0;
	float4 pos : SV_POSITION;

#if defined(SKELETONLIT_RECEIVE_SHADOWS)
	float4 shadowCoord : TEXCOORD1;
	half3 shadowedColor : TEXCOORD2;
#endif
#if defined(_ADDITIONAL_LIGHTS) && USE_FORWARD_PLUS
	float3 positionWS : TEXCOORD3;
	half3 normalWS : TEXCOORD4;
#endif
#if defined(_TINT_BLACK_ON)
	float3 darkColor : TEXCOORD5;
#endif
#if defined(USE_ADAPTIVE_PROBE_VOLUMES) && defined(_ADAPTIVE_PROBE_VOLUMES_PER_PIXEL)
	float3 positionCS : TEXCOORD6;
#endif
	UNITY_VERTEX_OUTPUT_STEREO
};

half3 ProcessLight(float3 positionWS, half3 normalWS, uint meshRenderingLayers, int lightIndex)
{
	Light light = GetAdditionalLight(lightIndex, positionWS);
#ifdef USE_LIGHT_LAYERS
	if (!IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
		return half3(0, 0, 0);
#endif

	half3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
	return LightingLambert(attenuatedLightColor, light.direction, normalWS);
}

half3 LightweightLightVertexSimplified(float3 positionWS, half3 normalWS, out half3 shadowedColor) {
	Light mainLight = GetMainLight();
	half3 attenuatedLightColor = mainLight.color * (mainLight.distanceAttenuation * mainLight.shadowAttenuation);
	half3 mainLightColor = LightingLambert(attenuatedLightColor, mainLight.direction, normalWS);

	half3 additionalLightColor = half3(0, 0, 0);
	// Note: we don't add any lighting in the fragment shader, thus we include both variants below
#if defined(_ADDITIONAL_LIGHTS) || defined(_ADDITIONAL_LIGHTS_VERTEX)
	uint meshRenderingLayers = GetMeshRenderingLayerBackwardsCompatible();
#if USE_FORWARD_PLUS
	for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++)
	{
		FORWARD_PLUS_SUBTRACTIVE_LIGHT_CHECK
		additionalLightColor += ProcessLight(positionWS, normalWS, meshRenderingLayers, lightIndex);
	}
#else // !USE_FORWARD_PLUS
	uint pixelLightCount = GetAdditionalLightsCount();
	LIGHT_LOOP_BEGIN_SPINE(pixelLightCount)
		additionalLightColor += ProcessLight(positionWS, normalWS, meshRenderingLayers, lightIndex);
	LIGHT_LOOP_END_SPINE
#endif // USE_FORWARD_PLUS
#endif

	shadowedColor = additionalLightColor;
	return mainLightColor + additionalLightColor;
}

#if defined(_ADDITIONAL_LIGHTS) && USE_FORWARD_PLUS
half3 LightweightLightFragmentSimplified(float3 positionWS, float2 positionCS, half3 normalWS, out half3 shadowedColor) {
	half3 additionalLightColor = half3(0, 0, 0);
	shadowedColor = half3(0, 0, 0);

	InputData inputData; // LIGHT_LOOP_BEGIN macro requires InputData struct in USE_FORWARD_PLUS branch
	inputData.positionWS = positionWS;
	inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(positionCS);

	uint meshRenderingLayers = GetMeshRenderingLayerBackwardsCompatible();
	uint pixelLightCount = GetAdditionalLightsCount();
	LIGHT_LOOP_BEGIN_SPINE(pixelLightCount)
		additionalLightColor += ProcessLight(positionWS, normalWS, meshRenderingLayers, lightIndex);
	LIGHT_LOOP_END_SPINE
	return additionalLightColor;
}
#endif

VertexOutput vert(appdata v) {
	VertexOutput o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

	half4 color = PMAGammaToTargetSpace(v.color);
	float3 positionWS = TransformObjectToWorld(v.pos);
	half3 fixedNormal = half3(0, 0, -1);
	half3 normalWS = normalize(mul((float3x3)unity_ObjectToWorld, fixedNormal));
	o.uv0 = v.uv0;
	o.pos = TransformWorldToHClip(positionWS);

#ifdef _DOUBLE_SIDED_LIGHTING
	// unfortunately we have to compute the sign here in the vertex shader
	// instead of using VFACE in fragment shader stage.
	half3 viewDirWS = UNITY_MATRIX_V[2].xyz;
	half faceSign = sign(dot(viewDirWS, normalWS));
	normalWS *= faceSign;
#endif

#if defined(_ADDITIONAL_LIGHTS) && USE_FORWARD_PLUS
	o.positionWS = positionWS;
	o.normalWS = normalWS;
#endif

#if defined(_TINT_BLACK_ON)
	color *= _Color;
	o.darkColor = GammaToTargetSpace(
		half3(v.tintBlackRG.r, v.tintBlackRG.g, v.tintBlackB.r)) + (_Black.rgb * v.color.a);
#endif

	half3 shadowedColor;
#if !defined(_LIGHT_AFFECTS_ADDITIVE)
	if (color.a == 0) {
		o.color = color;
#if defined(SKELETONLIT_RECEIVE_SHADOWS)
		o.shadowedColor = color.rgb;
		o.shadowCoord = float4(0, 0, 0, 0);
#endif
		return o;
	}
#endif // !defined(_LIGHT_AFFECTS_ADDITIVE)

	color.rgb *= LightweightLightVertexSimplified(positionWS, normalWS, shadowedColor);

	// Note: ambient light is also handled via SH.
	half3 vertexSH;
	float4 ignoredProbeOcclusion;
#if IS_URP_15_OR_NEWER
	#ifdef OUTPUT_SH4
		#if IS_URP_17_OR_NEWER
			OUTPUT_SH4(positionWS, normalWS.xyz, GetWorldSpaceNormalizeViewDir(positionWS), vertexSH, ignoredProbeOcclusion);
		#else // 15 or newer
			OUTPUT_SH4(positionWS, normalWS.xyz, GetWorldSpaceNormalizeViewDir(positionWS), vertexSH);
		#endif
	#else
		OUTPUT_SH(positionWS, normalWS.xyz, GetWorldSpaceNormalizeViewDir(positionWS), vertexSH);
	#endif
#else
	OUTPUT_SH(normalWS.xyz, vertexSH);
#endif

#if defined(USE_ADAPTIVE_PROBE_VOLUMES)
	#if !defined(_ADAPTIVE_PROBE_VOLUMES_PER_PIXEL)
		half4 shadowMask = 1.0;
		half3 bakedGI = SAMPLE_GI(vertexSH,
			GetAbsolutePositionWS(positionWS),
			normalWS.xyz,
			GetWorldSpaceNormalizeViewDir(positionWS),
			o.pos.xy,
			ignoredProbeOcclusion,
			shadowMask) * v.color.a;
	#else // _ADAPTIVE_PROBE_VOLUMES_PER_PIXEL
		half3 bakedGI = half3(0.0, 0.0, 0.0);
		o.positionCS = o.pos;
	#endif
#else
	half3 bakedGI = SAMPLE_GI(v.lightmapUV, vertexSH, normalWS) * v.color.a;
#endif
	color.rgb += bakedGI;
	o.color = color;

#if defined(SKELETONLIT_RECEIVE_SHADOWS)
	shadowedColor += bakedGI;
	o.shadowedColor = shadowedColor;
	
	VertexPositionInputs vertexInput;
	vertexInput.positionWS = positionWS;
	vertexInput.positionCS = o.pos;
	o.shadowCoord = GetShadowCoord(vertexInput);
#endif
	return o;
}

half4 frag(VertexOutput i
#ifdef USE_WRITE_RENDERING_LAYERS
	, out float4 outRenderingLayers : SV_Target1
#endif
) : SV_Target0
{
	half4 tex = tex2D(_MainTex, i.uv0);
#if !defined(_TINT_BLACK_ON) && defined(_STRAIGHT_ALPHA_INPUT)
	tex.rgb *= tex.a;
#endif

#if defined(USE_ADAPTIVE_PROBE_VOLUMES) && defined(_ADAPTIVE_PROBE_VOLUMES_PER_PIXEL)
	half3 vertexSH;
	float4 ignoredProbeOcclusion;
	OUTPUT_SH4(i.positionWS, i.normalWS.xyz, GetWorldSpaceNormalizeViewDir(i.positionWS), vertexSH, ignoredProbeOcclusion);
	half4 shadowMask = 1.0;
	half3 bakedGI = SAMPLE_GI(vertexSH,
		GetAbsolutePositionWS(i.positionWS),
		i.normalWS.xyz,
		GetWorldSpaceNormalizeViewDir(i.positionWS),
		i.positionCS.xy,
		ignoredProbeOcclusion,
		shadowMask) * i.color.a;
	i.color.rgb += bakedGI;
#endif

	if (i.color.a == 0)	{
#if defined(_TINT_BLACK_ON)
		return fragTintedColor(tex, i.darkColor, i.color, _Color.a, _Black.a);
#else
		return tex * i.color;
#endif
	}

#if defined(_ADDITIONAL_LIGHTS) && USE_FORWARD_PLUS
	// USE_FORWARD_PLUS lights need to be processed in fragment shader,
	// otherwise light culling by vertex will create a very bad lighting result.
	half3 shadowedColor;
	i.color.rgb += LightweightLightFragmentSimplified(i.positionWS, i.pos.xy, i.normalWS, shadowedColor);
#if defined(SKELETONLIT_RECEIVE_SHADOWS)
	i.shadowedColor += shadowedColor;
#endif
#endif

#if defined(SKELETONLIT_RECEIVE_SHADOWS)
	half shadowAttenuation = MainLightRealtimeShadow(i.shadowCoord);
	i.color.rgb = lerp(i.shadowedColor, i.color.rgb, shadowAttenuation);
#endif

#ifdef USE_WRITE_RENDERING_LAYERS
	uint renderingLayers = GetMeshRenderingLayerBackwardsCompatible();
	outRenderingLayers = float4(EncodeMeshRenderingLayer(renderingLayers), 0, 0, 0);
#endif

#if defined(_TINT_BLACK_ON)
	return fragTintedColor(tex, i.darkColor, i.color, _Color.a, _Black.a);
#else
	return tex * i.color;
#endif
}

#endif
