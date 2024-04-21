Shader "Unlit/Transparent Colorizer"
{
  Properties
  {
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _SaturationFactor ("Saturation range", Range(0, 5)) = 2
    _ColorBoost ("Color boost", Range(0, 5)) = 1
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
      ZWrite Off
      Fog
      { 
        Mode  Off
      } 
      Blend SrcAlpha OneMinusSrcAlpha
      ColorMask RGB
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixVP;
      uniform float _ColorBoost;
      uniform sampler2D _MainTex;
      uniform float _SaturationFactor;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 color :COLOR;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float4 xlv_COLOR :COLOR;
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_COLOR :COLOR;
          float2 xlv_TEXCOORD0 :TEXCOORD0;
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
          float4 tmpvar_3;
          tmpvar_3.w = 1;
          tmpvar_3.xyz = in_v.vertex.xyz;
          tmpvar_2 = tmpvar_1;
          float4 tmpvar_4;
          tmpvar_4.xyz = (in_v.color.xyz * _ColorBoost);
          tmpvar_4.w = in_v.color.w;
          out_v.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_3));
          out_v.xlv_COLOR = tmpvar_4;
          out_v.xlv_TEXCOORD0 = tmpvar_2;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          float3 tinted_2;
          float4 tmpvar_3;
          tmpvar_3 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
          float tmpvar_4;
          tmpvar_4 = max(max(tmpvar_3.x, tmpvar_3.y), tmpvar_3.z);
          float tmpvar_5;
          tmpvar_5 = min(min(tmpvar_3.x, tmpvar_3.y), tmpvar_3.z);
          float tmpvar_6;
          float tmpvar_7;
          tmpvar_7 = clamp(((tmpvar_4 - tmpvar_5) * _SaturationFactor), 0, 1);
          tmpvar_6 = tmpvar_7;
          float tmpvar_8;
          tmpvar_8 = (tmpvar_4 + tmpvar_5);
          float3 tmpvar_9;
          tmpvar_9 = (in_f.xlv_COLOR.xyz * tmpvar_8);
          tinted_2 = tmpvar_9;
          float3 tmpvar_10;
          tmpvar_10 = lerp(tmpvar_3.xyz, tinted_2, float3(tmpvar_6, tmpvar_6, tmpvar_6));
          float4 tmpvar_11;
          tmpvar_11.xyz = float3(tmpvar_10);
          tmpvar_11.w = (tmpvar_3.w * in_f.xlv_COLOR.w);
          tmpvar_1 = tmpvar_11;
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
