// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.30;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:32719,y:32712,varname:node_3138,prsc:2|emission-8957-OUT;n:type:ShaderForge.SFN_Tex2d,id:6483,x:32202,y:32780,ptovrint:False,ptlb:Main Tex,ptin:_MainTex,varname:node_6483,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-8635-OUT;n:type:ShaderForge.SFN_Add,id:8635,x:31974,y:32780,varname:node_8635,prsc:2|A-6280-OUT,B-7438-OUT;n:type:ShaderForge.SFN_Multiply,id:7438,x:31507,y:32897,varname:node_7438,prsc:2|A-9454-OUT,B-2608-OUT;n:type:ShaderForge.SFN_TexCoord,id:7813,x:31507,y:32655,varname:node_7813,prsc:2,uv:0;n:type:ShaderForge.SFN_RemapRange,id:2608,x:31269,y:32939,varname:node_2608,prsc:2,frmn:0,frmx:1,tomn:0,tomx:0.01|IN-4801-OUT;n:type:ShaderForge.SFN_Slider,id:4801,x:30870,y:33009,ptovrint:False,ptlb:Displacement Value,ptin:_DisplacementValue,varname:node_4801,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_RemapRange,id:9454,x:31269,y:32717,varname:node_9454,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-7394-OUT;n:type:ShaderForge.SFN_Append,id:7394,x:31049,y:32717,varname:node_7394,prsc:2|A-4846-R,B-4846-G;n:type:ShaderForge.SFN_Tex2d,id:4846,x:30771,y:32697,ptovrint:False,ptlb:Displacement Texture,ptin:_DisplacementTexture,varname:node_4846,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-2515-OUT;n:type:ShaderForge.SFN_Add,id:2515,x:30531,y:32697,varname:node_2515,prsc:2|A-4377-OUT,B-8407-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:8407,x:30303,y:32812,varname:node_8407,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:4377,x:30303,y:32639,varname:node_4377,prsc:2|A-3731-OUT,B-2760-TSL;n:type:ShaderForge.SFN_Slider,id:3731,x:29910,y:32604,ptovrint:False,ptlb:Displacement Speed,ptin:_DisplacementSpeed,varname:node_3731,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:1,max:3;n:type:ShaderForge.SFN_Time,id:2760,x:30067,y:32737,varname:node_2760,prsc:2;n:type:ShaderForge.SFN_Add,id:6280,x:31751,y:32631,varname:node_6280,prsc:2|A-3145-OUT,B-7813-UVOUT;n:type:ShaderForge.SFN_Slider,id:2200,x:31112,y:32398,ptovrint:False,ptlb:Offset Speed,ptin:_OffsetSpeed,varname:node_2200,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Time,id:7722,x:31269,y:32525,varname:node_7722,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3145,x:31507,y:32474,varname:node_3145,prsc:2|A-2200-OUT,B-7722-TSL;n:type:ShaderForge.SFN_Slider,id:642,x:32045,y:33016,ptovrint:False,ptlb:Emission Value,ptin:_EmissionValue,varname:node_642,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:3;n:type:ShaderForge.SFN_Multiply,id:8957,x:32458,y:32780,varname:node_8957,prsc:2|A-6483-RGB,B-642-OUT;proporder:6483-4846-4801-3731-2200-642;pass:END;sub:END;*/

Shader "Custom/Background Movement" {
    Properties {
        _MainTex ("Main Tex", 2D) = "white" {}
        _DisplacementTexture ("Displacement Texture", 2D) = "white" {}
        _DisplacementValue ("Displacement Value", Range(0, 1)) = 0
        _DisplacementSpeed ("Displacement Speed", Range(1, 3)) = 1
        _OffsetSpeed ("Offset Speed", Range(0, 1)) = 1
        _EmissionValue ("Emission Value", Range(0, 3)) = 1
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _DisplacementValue;
            uniform sampler2D _DisplacementTexture; uniform float4 _DisplacementTexture_ST;
            uniform float _DisplacementSpeed;
            uniform float _OffsetSpeed;
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
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 node_7722 = _Time + _TimeEditor;
                float4 node_2760 = _Time + _TimeEditor;
                float2 node_2515 = ((_DisplacementSpeed*node_2760.r)+i.uv0);
                float4 _DisplacementTexture_var = tex2D(_DisplacementTexture,TRANSFORM_TEX(node_2515, _DisplacementTexture));
                float2 node_8635 = (((_OffsetSpeed*node_7722.r)+i.uv0)+((float2(_DisplacementTexture_var.r,_DisplacementTexture_var.g)*2.0+-1.0)*(_DisplacementValue*0.01+0.0)));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_8635, _MainTex));
                float3 emissive = (_MainTex_var.rgb*_EmissionValue);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
