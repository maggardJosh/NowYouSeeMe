using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Decoration : FSprite
{
    public Decoration(int type)
        : base(String.Format("Decoration/decoration{0:00}", type))
    {

    }
}
