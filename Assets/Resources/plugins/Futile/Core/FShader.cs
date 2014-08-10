using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FShader
{
	static public FShader defaultShader;
	
	//shader types
	public static FShader Basic;
	public static FShader Additive;
	public static FShader AdditiveColor;
	public static FShader Solid;
	public static FShader SolidColored;

	public static FShader Basic_PixelSnap;
    public static FOverlayBlendShader OverlayBlend;

	public string name;
	public Shader shader;
	public bool needsApply = false;
	
	public FShader (string name, Shader shader) //only to be constructed inside this class using CreateShader()
	{
		this.name = name;
		this.shader = shader; 

		if(shader == null)
		{
			throw new FutileException("Couldn't find Futile shader '"+name+"'");
		}
	}

	virtual public void Apply(Material mat)
	{

	}
	
	public static void Init() //called by Futile
	{
		Basic = new FShader("Basic", Shader.Find("Futile/Basic"));	
		Additive = new FShader("Additive", Shader.Find("Futile/Additive"));	
		AdditiveColor = new FShader("AdditiveColor", Shader.Find("Futile/AdditiveColor"));	
		Solid = new FShader("Solid", Shader.Find("Futile/Solid"));	
		SolidColored = new FShader("SolidColored", Shader.Find("Futile/SolidColored"));	

		Basic_PixelSnap = new FShader("Basic_PixelSnap", Shader.Find("Futile/Basic_PixelSnap"));
        OverlayBlend = new FOverlayBlendShader();

		defaultShader = OverlayBlend;
	}
}


public class FBlurShader : FShader
{
	private float _blurAmount;
	
	public FBlurShader(float blurAmount) : base("BlurShader", Shader.Find("Futile/Blur"))
	{
		_blurAmount = blurAmount;
		needsApply = true;
	}
	
	override public void Apply(Material mat)
	{
		mat.SetFloat("_BlurForce",_blurAmount);
	}
	
	public float blurAmount
	{
		get {return _blurAmount;}
		set {if(_blurAmount != value) {_blurAmount = value; needsApply = true;}}
	}
}

public class FOverlayBlendShader : FShader
{
    private Color _overlayColor;

    public FOverlayBlendShader() : base("OverlayBlendShader", Shader.Find("Custom/OverlayBlend"))
    {
        _overlayColor = new Color(0.6549f, 0.505882f, 0.25f);
        needsApply = true;
    }
    public FOverlayBlendShader(Color overlayColor)
        : base("OverlayBlendShader", Shader.Find("Custom/OverlayBlend"))
    {
        _overlayColor = overlayColor;
        needsApply = true;
    }

    override public void Apply(Material mat)
    {
        mat.SetColor("_Color", _overlayColor);
    }

    public Color overlayColor
    {
        get { return _overlayColor; }
        set { if (_overlayColor != value) { _overlayColor = value; needsApply = true; } }
    }
}



