package edu.columbia.cs.cgui.spam2d;


import java.io.*;
import java.lang.reflect.*;
import java.lang.*;
import java.util.*;

class Test {
    public static void main(String[] args) {
	//	SpaceManager sm,sm2;
	IntervalTree it=null;
	try {
	    Class rectClass = Class.forName("Rectangle2d");
	    Method minxMeth, maxxMeth;
	    minxMeth = rectClass.getDeclaredMethod("getMinX",null);
	    maxxMeth = rectClass.getDeclaredMethod("getMaxX",null);
	    Class ss = Class.forName("FirstSecondaryStructure");
	    IntervalDimension iD = new IntervalDimension(1,minxMeth,maxxMeth);
	    it = new IntervalTree(rectClass,minxMeth,maxxMeth,ss,iD);
	} catch (Exception e){
	    e.printStackTrace();
	}
	Rectangle2d rect = new Rectangle2d();
	rect.setRect(0.0,0.0,0.306,1.0);//left
	//	it.addRectangle(rect);
	rect.setRect(0.67,0.0,1.0,1.0);//right
	it.addRectangle(rect);

	rect.setRect(0.67,0.648,1.,1.0);
	System.out.println("it.isEnclosedBy(rect)=" +it.isEnclosedBy(rect));
    }
}

