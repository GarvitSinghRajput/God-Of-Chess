// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FX/Diamond Transparent U5"
{
    Properties
    {
        _Color ("Color (RGB) Trans (A)", Color) = (1,1,1,1)
        _Multiplier ("Multiplier", Range (1.0, 5.0)) = 1
        _ReflectTex ("Reflection Texture", Cube) = ""{}
        _RefractTex ("Refraction Texture", Cube) = ""{}
    }
    SubShader
    {
        Tags
        { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Alphatest Greater 0
 		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB
		
        CGINCLUDE
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
#pragma exclude_renderers gles
        #pragma vertex vert
        #include "UnityCG.cginc"
        float4      _Color;
        float 		_Multiplier;
        samplerCUBE _ReflectTex;
        samplerCUBE _RefractTex;
        struct v2f
        {
            float4 pos    : SV_POSITION;
            float3 uv     : TEXCOORD0;
        };
        v2f vert (appdata_base v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
 
            // calculate world space reflection vector
            float3 viewDir = WorldSpaceViewDir( v.vertex );
            float3 worldN = UnityObjectToWorldNormal( v.normal );
            o.uv = reflect( -viewDir, worldN );
            return o;
        }

        half4 fragFront (v2f i) : SV_Target
        {
            float4 reflcol = texCUBE(_ReflectTex, i.uv);
            float4 col = texCUBE(_RefractTex, i.uv) * _Color + reflcol;
            col.a = _Color.a  ;   
            col.rgb = col.rgb * _Multiplier;
            return col;
        }
        ENDCG
     
//---------------------------------------------------------------------------------------------
        // First pass - here we render the backfaces of the diamonds. Since those diamonds are more-or-less
        // convex objects, this is effectively rendering the inside of them
       
        // Second pass - here we render the front faces of the diamonds.          
        Pass
        {
            ZWrite On
     		// Blend  SrcAlpha One
     		
            CGPROGRAM
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
#pragma exclude_renderers gles
            #pragma fragment fragFront
            ENDCG
        }
    }
    Fallback "Legacy Shaders/Transparent/VertexLit"
}
