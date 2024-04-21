Shader "Unlit/RainbowEffect"
{
  Properties
  {
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _ShimmerTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _PositionX ("PositionX", float) = 0.5
    _PositionY ("PositionY", float) = 0.5
    _Speed ("Speed", float) = 1
    _Scale ("Scale", float) = 1
    _LocationOffset ("LocationOffset", float) = 0
    _LocationSize ("LocationSize", float) = 1
    _ScreenAspect ("ScreenAspect", float) = 1
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
      //uniform float4 _Time;
      uniform sampler2D _MainTex;
      uniform sampler2D _ShimmerTex;
      uniform float _PositionX;
      uniform float _PositionY;
      uniform float _Speed;
      uniform float _Scale;
      uniform float _LocationOffset;
      uniform float _LocationSize;
      uniform float _ScreenAspect;
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
          float2 xlv_TEXCOORD1 :TEXCOORD1;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 xlv_COLOR :COLOR;
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float2 xlv_TEXCOORD1 :TEXCOORD1;
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
          float2 tmpvar_3;
          float4 tmpvar_4;
          float4 tmpvar_5;
          tmpvar_5.w = 1;
          tmpvar_5.xyz = in_v.vertex.xyz;
          tmpvar_4 = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_5));
          tmpvar_2 = tmpvar_1;
          float2 tmpvar_6;
          tmpvar_6 = tmpvar_4.xy;
          tmpvar_3 = tmpvar_6;
          out_v.vertex = tmpvar_4;
          out_v.xlv_COLOR = in_v.color;
          out_v.xlv_TEXCOORD0 = tmpvar_2;
          out_v.xlv_TEXCOORD1 = tmpvar_3;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          float4 finalColor_2;
          float len_3;
          float2 toCenter_4;
          float4 tmpvar_5;
          tmpvar_5 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
          float2 tmpvar_6;
          tmpvar_6.x = _PositionX;
          tmpvar_6.y = _PositionY;
          float2 tmpvar_7;
          tmpvar_7 = (in_f.xlv_TEXCOORD1 - tmpvar_6);
          toCenter_4 = tmpvar_7;
          toCenter_4.x = (toCenter_4.x * _ScreenAspect);
          float tmpvar_8;
          tmpvar_8 = sqrt(dot(toCenter_4, toCenter_4));
          len_3 = tmpvar_8;
          float2 tmpvar_9;
          tmpvar_9.x = 0;
          tmpvar_9.y = ((len_3 * _Scale) - (_Time.x * _Speed));
          float4 tmpvar_10;
          float4 y_11;
          y_11 = (tex2D(_ShimmerTex, tmpvar_9) * 1.3);
          float _tmp_dvx_0 = sin((clamp(((len_3 - _LocationOffset) / _LocationSize), 0, 1) * 3.141592));
          tmpvar_10 = lerp(tmpvar_5, y_11, float4(_tmp_dvx_0, _tmp_dvx_0, _tmp_dvx_0, _tmp_dvx_0));
          finalColor_2.xyz = tmpvar_10.xyz;
          finalColor_2.w = tmpvar_5.w;
          tmpvar_1 = (finalColor_2 * in_f.xlv_COLOR);
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
