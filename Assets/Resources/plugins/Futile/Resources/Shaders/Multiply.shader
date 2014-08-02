Shader "Custom/Multiply" {

	Properties
	{
		_Color ("Color", Color) = (1,1,1)
	}

    

	SubShader
	{

		Tags {Queue=Transparent}
		Blend Zero DstColor
		Pass {Color [_Color]}

	}

}