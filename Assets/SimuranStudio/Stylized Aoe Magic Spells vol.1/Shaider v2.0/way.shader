// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Simuranstudio/Particles/way"
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_Main_Texture("Main_Texture", 2D) = "white" {}
		_Texture_Noise("Texture_Noise", 2D) = "white" {}
		_Speed_Noise("Speed_Noise", Vector) = (-0.09,-0.22,0,0)
		_Multiply_Bot("Multiply_Bot", Float) = 1
		_Multiply_Top("Multiply_Top", Float) = 1
		_PowerMask_Top("PowerMask_Top", Float) = 10
		_PowerMask_Bot("PowerMask_Bot", Float) = 10
		_Color_Texture("Color_Texture", Color) = (1,1,1,1)
		_Emissive("Emissive", Float) = 1

	}


	Category 
	{
		SubShader
		{
		LOD 0

			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend One One
			ColorMask RGB
			Cull Off
			Lighting Off 
			ZWrite Off
			ZTest LEqual
			
			Pass {
			
				CGPROGRAM
				
				#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
				#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
				#endif
				
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				#include "UnityShaderVariables.cginc"
				#define ASE_NEEDS_FRAG_COLOR


				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD2;
					#endif
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
					
				};
				
				
				#if UNITY_VERSION >= 560
				UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
				#else
				uniform sampler2D_float _CameraDepthTexture;
				#endif

				//Don't delete this comment
				// uniform sampler2D_float _CameraDepthTexture;

				uniform sampler2D _MainTex;
				uniform fixed4 _TintColor;
				uniform float4 _MainTex_ST;
				uniform float _InvFade;
				uniform float4 _Color_Texture;
				uniform sampler2D _Main_Texture;
				SamplerState sampler_Main_Texture;
				uniform sampler2D _Texture_Noise;
				uniform float2 _Speed_Noise;
				uniform float _PowerMask_Top;
				uniform float _Multiply_Top;
				uniform float _PowerMask_Bot;
				uniform float _Multiply_Bot;
				uniform float _Emissive;


				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					

					v.vertex.xyz +=  float3( 0, 0, 0 ) ;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					o.color = v.color;
					o.texcoord = v.texcoord;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag ( v2f i  ) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID( i );
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( i );

					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate (_InvFade * (sceneZ-partZ));
						i.color.a *= fade;
					#endif

					float2 texCoord1 = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
					float4 tex2DNode3 = tex2D( _Main_Texture, texCoord1 );
					float2 panner25 = ( 1.0 * _Time.y * _Speed_Noise + texCoord1);
					float2 texCoord5 = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
					float2 texCoord31 = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
					float4 clampResult62 = clamp( ( _Color_Texture * tex2DNode3 * _Color_Texture.a * tex2DNode3.a * (( tex2D( _Texture_Noise, panner25 ) * ( 1.0 - ( ( pow( ( 1.0 - texCoord5.y ) , _PowerMask_Top ) * _Multiply_Top ) + ( pow( texCoord31.y , _PowerMask_Bot ) * _Multiply_Bot ) ) ) )).g * _Emissive * i.color * i.color.a ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
					

					fixed4 col = clampResult62;
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18500
75;56;1440;826;1452.121;446.1005;1.709907;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;5;-2035.723,314.4571;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;24;-1477.973,408.7311;Inherit;False;Property;_PowerMask_Top;PowerMask_Top;5;0;Create;True;0;0;False;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-1534.486,884.6251;Inherit;False;Property;_PowerMask_Bot;PowerMask_Bot;6;0;Create;True;0;0;False;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;6;-1705.385,306.5421;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;31;-1991.824,619.1967;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;14;-1116.97,860.7321;Inherit;False;Property;_Multiply_Bot;Multiply_Bot;3;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-1150.686,499.0876;Inherit;False;Property;_Multiply_Top;Multiply_Top;4;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;34;-1303.056,623.9928;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;23;-1253.128,246.2398;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-891.4,624.2481;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;27;-1268.452,102.0997;Inherit;False;Property;_Speed_Noise;Speed_Noise;2;0;Create;True;0;0;False;0;False;-0.09,-0.22;0,-0.22;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-964.1545,291.9648;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;1;-1321.988,-205.9101;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;36;-636.6316,420.8407;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;25;-813.4423,-12.79889;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;4;-470.6715,34.81287;Inherit;True;Property;_Texture_Noise;Texture_Noise;1;0;Create;True;0;0;False;0;False;-1;e93d49b87c1679243b96e512d313af7f;e93d49b87c1679243b96e512d313af7f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;37;-352.5542,423.8026;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-50.17554,342.5756;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;49;305.7505,101.6287;Inherit;True;False;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;356.6902,342.2907;Inherit;False;Property;_Emissive;Emissive;8;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-385.2864,-266.1486;Inherit;True;Property;_Main_Texture;Main_Texture;0;0;Create;True;0;0;False;0;False;-1;9c96217d52902d7498fdac1c175861bc;9c96217d52902d7498fdac1c175861bc;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;42;-286.2783,-506.7095;Inherit;False;Property;_Color_Texture;Color_Texture;7;0;Create;True;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;79;437.7465,445.9285;Inherit;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;798.2164,58.06863;Inherit;True;8;8;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;COLOR;0,0,0,0;False;7;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;62;1119.289,37.36338;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;1771.283,5.148024;Float;False;True;-1;2;ASEMaterialInspector;0;10;Simuranstudio/Particles/way;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;True;4;1;False;-1;1;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;True;2;False;-1;True;True;True;True;False;0;False;-1;False;False;False;False;True;2;False;-1;True;3;False;-1;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;6;0;5;2
WireConnection;34;0;31;2
WireConnection;34;1;29;0
WireConnection;23;0;6;0
WireConnection;23;1;24;0
WireConnection;28;0;34;0
WireConnection;28;1;14;0
WireConnection;13;0;23;0
WireConnection;13;1;30;0
WireConnection;36;0;13;0
WireConnection;36;1;28;0
WireConnection;25;0;1;0
WireConnection;25;2;27;0
WireConnection;4;1;25;0
WireConnection;37;0;36;0
WireConnection;39;0;4;0
WireConnection;39;1;37;0
WireConnection;49;0;39;0
WireConnection;3;1;1;0
WireConnection;66;0;42;0
WireConnection;66;1;3;0
WireConnection;66;2;42;4
WireConnection;66;3;3;4
WireConnection;66;4;49;0
WireConnection;66;5;50;0
WireConnection;66;6;79;0
WireConnection;66;7;79;4
WireConnection;62;0;66;0
WireConnection;0;0;62;0
ASEEND*/
//CHKSM=79A7C9A32921475A979F17552161F28A794B75BA