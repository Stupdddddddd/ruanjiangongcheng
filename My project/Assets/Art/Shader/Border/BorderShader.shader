Shader "SubstanceP/Border"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_Outer("Outer",Color) = (0,0,0,1)
		_Inner("Inner",Color) = (0,0,0,0.5)
		_BoundaryX("BoundaryX",Range(0,0.5)) = 0.1
		_BoundaryY("BoundaryY",Range(0,0.5)) = 0.1
	}
	SubShader
	{
		//去掉遮挡和深度缓冲
		Cull Off
		ZWrite Off
		//开启深度测试
		ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

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
 
			float4 _Outer;
			float4 _Inner;
			float _BoundaryX;
			float _BoundaryY;
            TEXTURE2D(_MainTex);
            half4 _MainTex_ST;
 
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
				float4 boundary=float4(_BoundaryX,_BoundaryY,1-_BoundaryX,1-_BoundaryY);
				if(i.uv.x<boundary.x||i.uv.x>boundary.z||i.uv.y<boundary.y||i.uv.y>boundary.w) return _Outer;
				return _Inner;
			}
			ENDHLSL
		}
	}
	Fallback "Sprites/Default"
}