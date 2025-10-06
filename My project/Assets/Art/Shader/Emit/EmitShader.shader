Shader "SubstanceP/Emit"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_Shadow("Shadow",Color) = (1,1,1,0.5)
		_Process("Process",Range(0,1)) = 0.5
		_CenterX("CenterX",Range(0,1)) = 0.5
		_CenterY("CenterY",Range(0,1)) = 0.5
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
 
			float4 _Shadow;
			float _Process;
			float _CenterX;
			float _CenterY;
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
				float2 center=float2(_CenterX,_CenterY); 
				float k=(center.y-i.uv.y)/(center.x-i.uv.x);
				float d=center.y-k*center.x;
				float2 edge;
				if(i.uv.x<center.x){
					if(i.uv.y<center.y){
						if(d<0) edge=float2(-d/k,0);
						else edge=float2(0,d);
					}
					else{
						if(d>1) edge=float2((1-d)/k,1);
						else edge=float2(0,d);
					}
				}
				else{
					float y=k+d;
					if(i.uv.y<center.y){
						if(y<0) edge=float2(-d/k,0);
						else edge=float2(1,y);
					}
					else{
						if(y>1) edge=float2((1-d)/k,1);
						else edge=float2(1,y);
					}
				}
				float process=length(i.uv-center)/length(edge-center);
				if(process<_Process*_Process) return _Shadow;
				else if(process<_Process) return _Shadow*float4(1,1,1,(1-(process-_Process*_Process)/(_Process-_Process*_Process)));
				else return half4(0,0,0,0);
			}
			ENDHLSL
		}
	}
	Fallback "Sprites/Default"
}