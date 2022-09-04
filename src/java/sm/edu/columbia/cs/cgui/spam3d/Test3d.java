package edu.columbia.cs.cgui.spam3d;

import java.io.*;
import java.lang.reflect.*;
import java.lang.*;
import java.util.*;

class Test3d {
    public static void main(String[] args) {
	//	SpaceManager sm,sm2;
	IntervalTree3d it=null;
	try {
	    Class rectClass = Class.forName("Rectangle3d");
	    Method minxMeth, maxxMeth;
	    minxMeth = rectClass.getDeclaredMethod("getMinX",null);
	    maxxMeth = rectClass.getDeclaredMethod("getMaxX",null);
	    Class ss = Class.forName("FirstSecondaryStructure3d");
	    IntervalDimension iD = new IntervalDimension(1,minxMeth,maxxMeth);
	    it = new IntervalTree3d(rectClass,minxMeth,maxxMeth,ss,iD);
	} catch (Exception e){
	    e.printStackTrace();
	}
	Rectangle3d rect = new Rectangle3d();
	rect.setRect(0.0,0.0,0.,1.,1.0,1.);//left
	//	it.addRectangle(rect);

	//	rect.setRect(0.67,0.0,0.,1.0,1.0,1.);//right
	//	it.addRectangle(rect);
	Rectangle3d qrect = new Rectangle3d(.9,.9,.9,1.1,1.1,1.1);
	it.addRectangle(rect);
	Vector v=it.windowQuery(qrect);
	System.out.println("v.size=" + v.size());
	for (Enumeration e=v.elements(); e.hasMoreElements();){
	    Rectangle3d r=(Rectangle3d)e.nextElement();
	    System.out.println("r=" + r);
	}
	//	rect.setRect(0.67,0.648,1.,1.0);
	//	System.out.println("it.isEnclosedBy(rect)=" +it.isEnclosedBy(rect));
    }
}

