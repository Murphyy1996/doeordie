// Upgrade NOTE: replaced 'SeperateSpecular' with 'SeparateSpecular'

Shader "Unlit/ThisMeansNothing"
{
	Properties
	{
		_Color("Main Color", Color) = (1, 0.5, 0.5, 1) //R G B Alpha //This line creates a new Colour called main colour and assigns the RGBA value given. 1 = 100% and 0.5 = 50%
		_Emission ("Emission Color", Color) = (1, 1, 1, 1) //adds the options for the emission colour to be changed?
		_Shininess ("Amount of shine", Range (0, 5)) = 0
		_SpecColor ("Spec Color", Color) = (1, 1, 1, 1)
		_Glossiness("Smoothness", Range(0,1)) = 0.5
	}
	SubShader //used for running on other hardware - i.e have seperate ones for high, medium and low graphics and one that can run on anything
	{
			Pass
		{
			Material
	{
		//Emission [_Emission]
		//Specular [_SpecColor]
	}
		//Lighting On
		//SeparateSpecular On
		
	{
	Diffuse [_Color]
	}
		Lighting On
		}
	}
}
