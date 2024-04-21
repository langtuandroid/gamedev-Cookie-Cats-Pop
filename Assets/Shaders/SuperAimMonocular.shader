Shader "SuperAimMonocular"
{
  Properties
  {
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
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
      Cull Off
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
      uniform sampler2D _MainTex;
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
          float4 tmpvar_1;
          tmpvar_1 = in_v.vertex;
          float2 tmpvar_2;
          tmpvar_2 = in_v.texcoord.xy;
          float2 tmpvar_3;
          float2 tmpvar_4;
          float4 tmpvar_5;
          tmpvar_5.w = 1;
          tmpvar_5.xyz = tmpvar_1.xyz;
          tmpvar_3 = tmpvar_2;
          tmpvar_4 = tmpvar_1.xy;
          out_v.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_5));
          out_v.xlv_COLOR = in_v.color;
          out_v.xlv_TEXCOORD0 = tmpvar_3;
          out_v.xlv_TEXCOORD1 = tmpvar_4;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          float4 tmpvar_1;
          float4 tmpvar_2;
          tmpvar_2 = tex2D(_MainTex, in_f.xlv_TEXCOORD0);
          float tmpvar_3;
          tmpvar_3 = pow((((in_f.xlv_TEXCOORD1.x * in_f.xlv_TEXCOORD1.x) + (in_f.xlv_TEXCOORD1.y * in_f.xlv_TEXCOORD1.y)) * 0.00011), 13);
          float4 tmpvar_4;
          tmpvar_4.xyz = tmpvar_2.xyz;
          tmpvar_4.w = ((1 - tmpvar_3) * in_f.xlv_COLOR.w);
          tmpvar_1 = tmpvar_4;
          out_f.color = tmpvar_1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
