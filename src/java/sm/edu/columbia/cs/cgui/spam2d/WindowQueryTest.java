package edu.columbia.cs.cgui.spam2d;


/**
 * WindowQueryTest.java
 *
 *
 * Created: Mon Dec 27 20:38:09 1999
 *
 * @author 
 * @version
 */
import java.lang.reflect.*;
import java.lang.*;
import java.util.*;

public class WindowQueryTest  {
    
    public WindowQueryTest() {
	
    }
    public static void main(String args[]){
	try {
	    Class rectClass = Class.forName("Rectangle2d");
	    Method minxMeth, maxxMeth;
	    minxMeth = rectClass.getDeclaredMethod("getMinX",null);
	    maxxMeth = rectClass.getDeclaredMethod("getMaxX",null);
	    Class ss = Class.forName("FirstSecondaryStructure");
	    IntervalDimension iD = new IntervalDimension(1,minxMeth,maxxMeth);
	    IntervalTree it;
	    it = new IntervalTree(rectClass,minxMeth,maxxMeth,ss,iD);
	    Rectangle2d rect=new Rectangle2d(0.,0.,1.,1.);
	    it.addRectangle(rect);
	    Vector v=it.windowQueryWithoutEdges(rect);
	    for (Enumeration e=v.elements(); e.hasMoreElements();){
		Rectangle2d clrs=(Rectangle2d) e.nextElement();
		System.out.println("clrs=" + clrs);
	    }
	} catch (Exception e){
	    e.printStackTrace();
	}
    }
    
} // WindowQueryTest
