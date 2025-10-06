Shader "Unlit/Grid"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		[HDR] _Inner("Inner",Color) = (0,0,0,0.5)
		[HDR] _GridColor("GridColor",Color) = (0.5,0.5,0.5,1.0)
		[HDR] _Outer("Outor",Color) = (1,1,1,0.5)
		_Width("Width",int)=8
		_Col("Col",int)=30
		_Row("Row",int)=18
		
		[HideInInspector] _CellTexel("CellTexel",int)=40
		[HideInInspector] _ScreenWidth("ScreenWidth",int)=1920
		[HideInInspector] _ScreenHeight("ScreenHeight",int)=1080

	}
	SubShader
	{
		//去掉遮挡和深度缓冲
		Cull Off
		ZWrite Off
		//开启深度测试
		ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha
 
		HLSLINCLUDE
		//添加一个计算方法
		float mod(float a,float b)
		{
			//floor(x)方法是Cg语言内置的方法，返回小于x的最大的整数
			return a - b*floor(a / b);
		}
		ENDHLSL
 
		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #if defined(DEBUG_DISPLAY)
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/InputData2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging2D.hlsl"
            #endif
 
			uniform float4 _Inner;
			uniform float4 _GridColor;
			uniform float4 _Outer;
			uniform float _Width;
			int _Col;
			int _Row;
			int _CellTexel;
            TEXTURE2D(_MainTex);
            half4 _MainTex_ST;
			int _ScreenWidth;
			int _ScreenHeight;
 
			struct appdata
			{
				float4 vertex:POSITION;
				float2 uv:TEXCOORD0;
			};
			struct v2f
			{
				float2 uv:TEXCOORD0;
				float4 vertex:SV_POSITION;
			};
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = TransformObjectToHClip(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
 
			half4 frag(v2f i) :SV_Target
			{
				int2 size=int2(_CellTexel*_Col,_CellTexel*_Row);

				half l=(_ScreenWidth-size.x)/2.0/(half)_ScreenWidth;
				half r=(_ScreenWidth+size.x)/2.0/(half)_ScreenWidth;
				half u=(_ScreenHeight+size.y)/2.0/(half)_ScreenHeight;
				half d=(_ScreenHeight-size.y)/2.0/(half)_ScreenHeight;

				if(i.uv.x<l||i.uv.x>r||i.uv.y<d||i.uv.y>u) return _Outer;
				if(mod((i.uv.x-l)*_ScreenWidth+_Width/2,_CellTexel)<_Width||
					mod((i.uv.y-d)*_ScreenHeight+_Width/2,_CellTexel)<_Width) return _GridColor;

				return _Inner;
			}
				ENDHLSL
		}
	}
	Fallback "Sprites/Default"
}