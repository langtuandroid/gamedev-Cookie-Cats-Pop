Shader "Map/Tile_editor"
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
      "QUEUE" = "Background"
      "RenderType" = "Background"
    }
    LOD 100
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "QUEUE" = "Background"
        "RenderType" = "Background"
      }
      LOD 100
      ZClip Off
      ZWrite Off
      Cull Off
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
          float4 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float2 xlv_TEXCOORD0 :TEXCOORD0;
          float2 xlv_TEXCOORD1 :TEXCOORD1;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
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
          tmpvar_6 = ((tmpvar_4.xy + float2(1, 1)) * float2(0.5, 0.5));
          tmpvar_3 = tmpvar_6;
          out_v.vertex = tmpvar_4;
          out_v.xlv_TEXCOORD0 = tmpvar_2;
          out_v.xlv_TEXCOORD1 = tmpvar_3;
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
          tmpvar_3 = tex2D(_LightTexture, in_f.xlv_TEXCOORD1);
          float4 y_4;
          y_4 = (tmpvar_2 * _NightColor);
          float4 tmpvar_5;
          tmpvar_5 = lerp(lerp(tmpvar_2, y_4, float4(_NightBlend, _NightBlend, _NightBlend, _NightBlend)), tmpvar_2, tmpvar_3);
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
