Shader "Map/Actor"
{
  Properties
  {
    _MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
  }
  SubShader
  {
    Tags
    { 
      "IGNOREPROJECTOR" = "true"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
    }
    LOD 100
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
      }
      LOD 100
      ZClip Off
      ZTest Less
      Fog
      { 
        Mode  Off
      } 
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixVP;
      uniform sampler2D _MainTex;
      uniform sampler2D _LightTexture;
      uniform float4 _NightColor;
      uniform float _NightBlend;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float3 xlv_TEXCOORD1 :TEXCOORD1;
          float2 xlv_TEXCOORD2 :TEXCOORD2;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float3 xlv_TEXCOORD1 :TEXCOORD1;
          float2 xlv_TEXCOORD2 :TEXCOORD2;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float2 tmpvar_1;
          tmpvar_1 = in_v.texcoord.xy;
          float2 tmpvar_2;
          float3 tmpvar_3;
          float2 tmpvar_4;
          float4 tmpvar_5;
          float4 tmpvar_6;
          tmpvar_6.w = 1;
          tmpvar_6.xyz = in_v.vertex.xyz;
          tmpvar_5 = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_6));
          float3x3 tmpvar_7;
          tmpvar_7[0] = conv_mxt4x4_0(unity_ObjectToWorld).xyz;
          tmpvar_7[1] = conv_mxt4x4_1(unity_ObjectToWorld).xyz;
          tmpvar_7[2] = conv_mxt4x4_2(unity_ObjectToWorld).xyz;
          tmpvar_3 = mul(tmpvar_7, in_v.normal);
          tmpvar_2 = tmpvar_1;
          float2 tmpvar_8;
          tmpvar_8 = ((tmpvar_5.xy + float2(1, 1)) * float2(0.5, 0.5));
          tmpvar_4 = tmpvar_8;
          out_v.vertex = tmpvar_5;
          out_v.xlv_TEXCOORD0 = tmpvar_2;
          out_v.xlv_TEXCOORD1 = tmpvar_3;
          out_v.xlv_TEXCOORD2 = tmpvar_4;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 col_1;
          float4 tmpvar_2;
          tmpvar_2 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
          float4 tmpvar_3;
          tmpvar_3 = tex2D(_LightTexture, in_f.xlv_TEXCOORD2);
          float4 y_4;
          y_4 = (tmpvar_2 * _NightColor);
          float4 tmpvar_5;
          tmpvar_5 = lerp(lerp(tmpvar_2, y_4, float4(_NightBlend, _NightBlend, _NightBlend, _NightBlend)), tmpvar_2, (tmpvar_3 * clamp(dot(in_f.xlv_TEXCOORD1, float3(-0.7071068, 0, (-0.7071068))), 0, 1)));
          col_1 = tmpvar_5;
          float4 tmpvar_6;
          tmpvar_6.w = 1;
          tmpvar_6.xyz = col_1.xyz;
          out_f.color = tmpvar_6;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
