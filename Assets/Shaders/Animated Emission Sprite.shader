// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:1,cusa:True,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:True,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:1873,x:33229,y:32719,varname:node_1873,prsc:2|emission-2231-OUT,alpha-603-OUT;n:type:ShaderForge.SFN_Tex2d,id:4805,x:32057,y:32775,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:True,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1086,x:32333,y:32845,cmnt:RGB,varname:node_1086,prsc:2|A-4805-R,B-5983-RGB;n:type:ShaderForge.SFN_Color,id:5983,x:32057,y:32997,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:1749,x:32595,y:32856,cmnt:Premultiply Alpha,varname:node_1749,prsc:2|A-1086-OUT,B-5310-OUT;n:type:ShaderForge.SFN_Multiply,id:603,x:32351,y:33010,cmnt:A,varname:node_603,prsc:2|A-4805-A,B-5983-A;n:type:ShaderForge.SFN_Sin,id:318,x:32359,y:32347,varname:node_318,prsc:2|IN-160-OUT;n:type:ShaderForge.SFN_Time,id:9453,x:31767,y:32449,varname:node_9453,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2231,x:32954,y:32806,varname:node_2231,prsc:2|A-7238-OUT,B-1749-OUT;n:type:ShaderForge.SFN_Multiply,id:160,x:32082,y:32399,varname:node_160,prsc:2|A-4396-OUT,B-9453-T;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:7238,x:32697,y:32600,varname:node_7238,prsc:2|IN-318-OUT,IMIN-3012-OUT,IMAX-2418-OUT,OMIN-2955-OUT,OMAX-2418-OUT;n:type:ShaderForge.SFN_Vector1,id:3012,x:32359,y:32497,varname:node_3012,prsc:2,v1:-1;n:type:ShaderForge.SFN_Vector1,id:2955,x:32359,y:32587,varname:node_2955,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:4396,x:31689,y:32330,ptovrint:False,ptlb:Animation Speed,ptin:_AnimationSpeed,varname:node_4396,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:1,max:5;n:type:ShaderForge.SFN_Slider,id:2418,x:32254,y:32700,ptovrint:False,ptlb:Minimum Intensity,ptin:_MinimumIntensity,varname:node_2418,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.5,cur:0.7,max:1;n:type:ShaderForge.SFN_Slider,id:5310,x:32552,y:33056,ptovrint:False,ptlb:Emission Value,ptin:_EmissionValue,varname:node_5310,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:1,max:3;proporder:4805-5983-4396-2418-5310;pass:END;sub:END;*/

Shader "Custom/Animated Emission Sprite" {
    Properties {
        [PerRendererData]_MainTex ("MainTex", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _AnimationSpeed ("Animation Speed", Range(1, 5)) = 1
        _MinimumIntensity ("Minimum Intensity", Range(0.5, 1)) = 0.7
        _EmissionValue ("Emission Value", Range(1, 3)) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _Color;
            uniform float _AnimationSpeed;
            uniform float _MinimumIntensity;
            uniform float _EmissionValue;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 node_9453 = _Time + _TimeEditor;
                float node_3012 = (-1.0);
                float node_2955 = 1.0;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 emissive = ((node_2955 + ( (sin((_AnimationSpeed*node_9453.g)) - node_3012) * (_MinimumIntensity - node_2955) ) / (_MinimumIntensity - node_3012))*((_MainTex_var.r*_Color.rgb)*_EmissionValue));
                float3 finalColor = emissive;
                return fixed4(finalColor,(_MainTex_var.a*_Color.a));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
