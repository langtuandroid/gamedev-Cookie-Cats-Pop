Shader "Map/RevealEffectNew"
{
  Properties
  {
    _MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
    _WhiteTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
    _RevealPosition ("Texture Fade Out", float) = 0
    _WhiteSize ("WhiteSize", float) = 0.5
    _FadeOutSize ("FadeOutSize", float) = 0.1
  }
  SubShader
  {
    Tags
    { 
      "DisableBatching" = "true"
      "IGNOREPROJECTOR" = "true"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
    }
    LOD 100
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "DisableBatching" = "true"
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
      Blend SrcAlpha OneMinusSrcAlpha
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixVP;
      uniform float _RevealPosition;
      uniform float4x4 _QuadSpace;
      uniform float2 _QuadSize;
      uniform float4 _QuadTexCoords;
      uniform sampler2D _MainTex;
      uniform float _FadeOutSize;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float xlv_TEXCOORD0 :TEXCOORD0;
          float2 xlv_TEXCOORD1 :TEXCOORD1;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float xlv_TEXCOORD0 :TEXCOORD0;
          float2 xlv_TEXCOORD1 :TEXCOORD1;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float2 normalized_1;
          float4 localVertex_2;
          float tmpvar_3;
          float2 tmpvar_4;
          localVertex_2.xzw = in_v.vertex.xzw;
          localVertex_2.y = (clamp(_RevealPosition, 0, 1) * in_v.vertex.y);
          float2 tmpvar_5;
          tmpvar_5 = ((float2(1, 1) + ((mul(_QuadSpace, mul(unity_ObjectToWorld, localVertex_2)).xy / _QuadSize) * float2(2, 2))) * float2(0.5, 0.5));
          normalized_1 = tmpvar_5;
          float2 tmpvar_6;
          tmpvar_6.x = (_QuadTexCoords.x + (_QuadTexCoords.z * normalized_1.x));
          tmpvar_6.y = (_QuadTexCoords.y + (_QuadTexCoords.w * normalized_1.y));
          tmpvar_4 = tmpvar_6;
          float tmpvar_7;
          tmpvar_7 = clamp((_RevealPosition - 1), 0, 1);
          float tmpvar_8;
          tmpvar_8 = (1 - _RevealPosition);
          tmpvar_3 = (((in_v.vertex.y / in_v.texcoord.y) * (1 - tmpvar_7)) + (((in_v.texcoord.y - in_v.vertex.y) * tmpvar_8) / in_v.texcoord.y));
          out_v.vertex = mul(mul(unity_MatrixVP, unity_ObjectToWorld), localVertex_2);
          out_v.xlv_TEXCOORD0 = tmpvar_3;
          out_v.xlv_TEXCOORD1 = tmpvar_4;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          float4 targetColor_2;
          float4 tmpvar_3;
          tmpvar_3 = tex2D(_MainTex, in_f.xlv_TEXCOORD1);
          float4 tmpvar_4;
          float _tmp_dvx_1 = (1 - clamp(((1 - in_f.xlv_TEXCOORD0) / _FadeOutSize), 0, 1));
          tmpvar_4 = lerp(tmpvar_3, float4(1, 1, 1, 1), float4(_tmp_dvx_1, _tmp_dvx_1, _tmp_dvx_1, _tmp_dvx_1));
          targetColor_2 = tmpvar_4;
          float4 tmpvar_5;
          tmpvar_5.xyz = targetColor_2.xyz;
          tmpvar_5.w = tmpvar_3.w;
          tmpvar_1 = tmpvar_5;
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
