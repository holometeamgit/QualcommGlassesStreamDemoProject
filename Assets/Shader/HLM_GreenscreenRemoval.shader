// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "HLM/Unlit/GreenscreenRemoval"
{
    Properties
    {
        _t("Transparency", Float) = 1

		[MaterialToggle] _UseBlendTex("UseBlendTex", Float) = 1.0
        _BlendTex("BlendTexture", 2D) = "white" {}
	    
        _MainTex ("Texture", 2D) = "white" {}
        
        _H1("H1", Float) =  60
        _H2("H2", Float) = 120
        _S ("S",  Float) = 0.22
        _V ("V",  Float) = 0.21
        
        _smallPolyhedron1("sp1", Float) = 0.2
        _smallPolyhedron2("sp2", Float) = 0.3
        _mediumPolyhedron("mp",  Float) = 0.34
        _largePolyhedron ("lp",  Float) = 2.0
        
        _maskContrastL("maskContrastL", Float) = 0.0
        _maskContrastM("maskContrastM", Float) = 1.0
        _maskContrastS("maskContrastS", Float) = 1.0
        _maskContrast ("maskContrast",  Float) = 0.0
        
        _rb ("BR", Int) = 3
        
        _Hx("Hx", Float) = 0.0
        _Px("Px", Float) = 0.5
        
        _By("By", Float) = 0.0
        _Qy("Qy", Float) = 0.5
        
        _Qz("Qz", Float) = 0.0
        _Rz("Rz", Float) = 0.5
        
        _QyC("_QyC", Float) = 0.0
    }
    
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "LightMode" = "ForwardBase"}
        LOD 100
        
        Cull Off
        ZWrite Off
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha 

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature USE_AMBIENT_LIGHTING

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 diff : COLOR0;
            };

			sampler2D _BlendTex;
            float4 _BlendTex_ST;
            float _UseBlendTex;

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
           
            float _H1;
            float _H2;
            float _S;
            float _V;
            float _Distance;
            
            float _smallPolyhedron1;
            float _smallPolyhedron2;
            float _mediumPolyhedron;
            float _largePolyhedron;

            float _maskContrastL;
            float _maskContrastM;
            float _maskContrastS;
            float _maskContrast;
            
            uniform float Epsilon = 1e-10;
            float4 _MainTex_TexelSize;

            int _rb;
            
            uniform float _Hx;
            uniform float _Px;

            uniform float _By;
            uniform float _Qy;

            uniform float _Qz;
            uniform float _Rz;
            
            uniform float _QyC;

            float _t;
            
            float4 median_blur(float r, float2 coord)
            {
                float stepX = _MainTex_TexelSize.x;
                float stepY = _MainTex_TexelSize.y;
                
                float avgR = 0.0;
                float avgG = 0.0;
                float avgB = 0.0;
                float avgA = 0.0;
                
                int count = 0;
                
                for (int i = -r; i <= r; i++)
                {
                    for (int j = -r; j <= r; j++)
                    {
                        float x = coord.x + stepX * float(i);
                        float y = coord.y + stepY * float(j);
                        if(x >= 0.0 && x <= 1.0 && y >= 0.0 && y <= 1.0)
                        {
                            float2 currC = float2(x, y);
                            float4 color = tex2D(_MainTex, currC);
                            
                            avgR += color.r;
                            avgG += color.g;
                            avgB += color.b;
                            count++;
                        }
                    }
                }
                
                return float4(avgR/float(count), avgG/float(count), avgB/float(count), avgA/float(count));
            }
            
             half3 rgb_to_hcv(half3 rgb) {
                half4 p = rgb.g < rgb.b ? half4(rgb.bg, -1.0, 2.0/3.0) : half4(rgb.gb, 0.0, -1.0/3.0);
                half4 q = rgb.r < p.x ? half4(p.xyw, rgb.r) : half4(rgb.r, p.yzx);

                float c = q.x - min(q.w, q.y);
                
                return half3(abs((q.w - q.y) / (6 * c + Epsilon) + q.z), q.x - min(q.w, q.y), q.x);
            }
            
            half3 rgb_to_hsv(half3 rgb) {
                half3 hcv = rgb_to_hcv(rgb);
                return half3(hcv.x, hcv.y / (hcv.z + Epsilon), hcv.z);
            }
            
            float hsvDistance(float3 hsv, float ih1, float ih2, float is, float iv)
            {
                float h = hsv.r * 255;
                float s = hsv.g;
                float v = hsv.b;
                
                float dh1 = min(abs(h-ih1), 360-abs(h-ih1)) / 180.0;
                float dh2 = min(abs(h-ih2), 360-abs(h-ih2)) / 180.0;
                
                float dh = min(dh1, dh2);
                
                float ds = abs(s-is);
                float dv = abs(v-iv);
                
                float distance = sqrt(dh*dh  + ds*ds + dv*dv);
                return distance;
            }
            
            bool is_hsv_good(float3 hsv, float ih1, float ih2, float is, float iv) {
                return hsv.r * 255 >= ih1 && hsv.r * 255 <= ih2 && hsv.g >= is && hsv.b >= iv;
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.diff = _LightColor0;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float step_x = _MainTex_TexelSize.x;
                float step_y = _MainTex_TexelSize.y;
                
                half4 col = tex2D(_MainTex, i.uv);
                half4 blur_res = median_blur(_rb, i.uv);

                half3 hsv = rgb_to_hsv(blur_res.rgb);
                
                half4 up    = tex2D(_MainTex, i.uv + fixed2(0, step_y));
                half4 down  = tex2D(_MainTex, i.uv - fixed2(0, step_y));
                half4 left  = tex2D(_MainTex, i.uv - fixed2(step_x, 0));
                half4 right = tex2D(_MainTex, i.uv + fixed2(step_x, 0));

                half3 hsv_up    = rgb_to_hsv(up.rgb);
                half3 hsv_down  = rgb_to_hsv(down.rgb);
                half3 hsv_left  = rgb_to_hsv(left.rgb);
                half3 hsv_right = rgb_to_hsv(right.rgb);

                half4 up_left    = tex2D(_MainTex, i.uv + fixed2(-step_x,  step_y));
                half4 down_left  = tex2D(_MainTex, i.uv + fixed2(-step_x, -step_y));
                half4 up_right   = tex2D(_MainTex, i.uv + fixed2( step_x,  step_y));
                half4 down_right = tex2D(_MainTex, i.uv + fixed2( step_x, -step_y));

                half3 hsv_up_left    = rgb_to_hsv(up_left.rgb);
                half3 hsv_down_left  = rgb_to_hsv(down_left.rgb);
                half3 hsv_up_right   = rgb_to_hsv(up_right.rgb);
                half3 hsv_down_right = rgb_to_hsv(down_right.rgb);
                
                if (is_hsv_good(hsv, _H1, _H2, _S, _V)) {
                    half distance1 = hsvDistance(hsv, _H1, _H2, _S, _V);
                    
                    half distance_l = hsvDistance(hsv_left, _H1, _H2, _S, _V);
                    half distance_r = hsvDistance(hsv_right, _H1, _H2, _S, _V);
                    half distance_u = hsvDistance(hsv_up, _H1, _H2, _S, _V);
                    half distance_d = hsvDistance(hsv_down, _H1, _H2, _S, _V);

                    half distance_ul = hsvDistance(hsv_up_left, _H1, _H2, _S, _V);
                    half distance_dl = hsvDistance(hsv_down_left, _H1, _H2, _S, _V);
                    half distance_ur = hsvDistance(hsv_up_right, _H1, _H2, _S, _V);
                    half distance_dr = hsvDistance(hsv_down_right, _H1, _H2, _S, _V);
                    
                    half d = 1  - distance1;
                    col.a = d * _maskContrast;
                    
                    if (distance1 < _smallPolyhedron1)
                    {
                        if (distance_l <= _smallPolyhedron1 && distance_r <= _smallPolyhedron1 && distance_u <= _smallPolyhedron1 && distance_d <= _smallPolyhedron1 &&
                            distance_ul <= _smallPolyhedron1 && distance_dl <= _smallPolyhedron1 && distance_ur <= _smallPolyhedron1 && distance_dr <= _smallPolyhedron1) {
                            col.a = d * _maskContrastS;
                        }
                    } else if (distance1 < _smallPolyhedron2) {
                        if (distance_l <= _smallPolyhedron2 && distance_r <= _smallPolyhedron2 && distance_u <= _smallPolyhedron2 && distance_d <= _smallPolyhedron2 &&
                            distance_ul <= _smallPolyhedron2 && distance_dl <= _smallPolyhedron2 && distance_ur <= _smallPolyhedron2 && distance_dr <= _smallPolyhedron2) {
                            col.a = d * _maskContrastS;
                        }
                    } else if (distance1 < _mediumPolyhedron) {
                        if (distance_l > _mediumPolyhedron || distance_r > _mediumPolyhedron || distance_u > _mediumPolyhedron || distance_d > _mediumPolyhedron ||
                            distance_ul > _mediumPolyhedron || distance_dl > _mediumPolyhedron || distance_ur > _mediumPolyhedron || distance_dr > _mediumPolyhedron) {
                            col.a = d * _maskContrastM;
                        }
                    } else if (distance1 < _largePolyhedron) {
                        col.a = d * _maskContrastL;
                    }

                     if (distance1 < 0.5) {
                         half res = 1.0 - distance1 * 2.0f;
                         col.r *= res;
                         col.g *= res;
                         col.b *= res;

                         col.a = res * _maskContrast * (1 / (sqrt(sqrt(i.uv.y)) * 1.5));
                     } 
                }
                
                half g = col.g;
                half r = col.r;
                half b = col.b;
                
                if (g > (b + 2 * r) / 3) {
                    col.g = (b + 2 * r) / 3;
                } else {
                    col.g = g;
                }
                
                
#ifdef USE_AMBIENT_LIGHTING
				   i.diff.rgb = clamp(i.diff.rgb, float3(0.25, 0.25, 0.25), float3(1.75, 1.75, 1.75));
				   col.rgb *= i.diff;
#endif
                
				fixed4 blendTextureColour = tex2D(_BlendTex, i.uv);
                if (_UseBlendTex == 0) {
                    blendTextureColour.a = 1;
                }
            
                return fixed4(col.rgb, col.a * _t);
            }
            ENDCG
        }
    }
}
