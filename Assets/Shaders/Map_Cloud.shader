Shader "Map/Cloud"
{
  Properties
  {
    _MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
    _Noise ("Base (RGB), Alpha (A)", 2D) = "white" {}
    _MaxHeight ("MaxHeight", float) = 0
  }
  SubShader
  {
    Tags
    { 
      "IGNOREPROJECTOR" = "true"
      "QUEUE" = "Transparent"
      "RenderType" = "Cloud"
    }
    LOD 100
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "QUEUE" = "Transparent"
        "RenderType" = "Cloud"
      }
      LOD 100
      ZClip Off
      ZWrite Off
      Cull Off
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
      //uniform float4 _Time;
      uniform sampler2D _MainTex;
      uniform sampler2D _Noise;
      uniform float4 _NightColor;
      uniform float _NightBlend;
      uniform float _MaxHeight;
      struct appdata_t
      {
          float4 vertex :POSITION;
          float4 color :COLOR;
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float xlv_TEXCOORD1 :TEXCOORD1;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float xlv_TEXCOORD1 :TEXCOORD1;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          float4 tmpvar_1;
          tmpvar_1 = in_v.color;
          float2 tmpvar_2;
          float tmpvar_3;
          tmpvar_2 = (in_v.texcoord.xy * 0.05);
          tmpvar_3 = tmpvar_1.w;
          out_v.vertex = mul(mul(unity_MatrixVP, unity_ObjectToWorld), in_v.vertex);
          out_v.xlv_TEXCOORD0 = tmpvar_2;
          out_v.xlv_TEXCOORD1 = tmpvar_3;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          float4 col_2;
          float4 tmpvar_3;
          float2 P_4;
          P_4 = (in_f.xlv_TEXCOORD0 + (_Time * 0.025).xy);
          tmpvar_3 = tex2D(_Noise, P_4);
          float4 tmpvar_5;
          float2 P_6;
          P_6 = (((in_f.xlv_TEXCOORD0 * 3) + (tmpvar_3.x * 0.7)) - (_Time * 0.1).xy);
          tmpvar_5 = tex2D(_MainTex, P_6);
          float4 tmpvar_7;
          float4 y_8;
          y_8 = (tmpvar_5 * _NightColor);
          tmpvar_7 = lerp(tmpvar_5, y_8, float4(_NightBlend, _NightBlend, _NightBlend, _NightBlend));
          col_2 = tmpvar_7;
          float4 tmpvar_9;
          tmpvar_9.xyz = col_2.xyz;
          tmpvar_9.w = (((tmpvar_5.w * in_f.xlv_TEXCOORD1) * (0.7 + (tmpvar_3.x * 0.3))) * (1 - _MaxHeight));
          tmpvar_1 = tmpvar_9;
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
