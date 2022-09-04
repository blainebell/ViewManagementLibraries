package edu.columbia.cs.cgui.spam2d;

import java.io.*;
import java.lang.reflect.*;
import java.lang.*;
import java.util.*;

class Test2 {
    public static void main(String[] args) {
	SpaceManager sm=null;
	try {
	    sm = new SpaceManager(new Rectangle2d(.0,.0,1.,1.));
	} catch (Exception e){
	    e.printStackTrace();
	}
	FullRectangle2d fr1 = new FullRectangle2d(1l,.25,0.25,.75,.75);
	FullRectangle2d fr2 = new FullRectangle2d(2l,0.25,0.25,.75,.75);
	sm.addRectangle(fr1,-1);
	sm.addRectangle(fr2,-1);

	Iterator it=sm.spaceIterator();
	int in=1;
	while (it.hasNext()){
	    System.out.println("space #" + (in++) + " "+ it.next());
	}
	
	it = sm.objectIterator();
	in=1;
	while (it.hasNext()){
	    System.out.println("object #" + (in++) + " "+ it.next());
	}
	File f = new File ("test.smf");
	FileOutputStream ostream=null;
	ObjectOutputStream p=null ;
	try {
	    ostream = new FileOutputStream(f);
	    p = new ObjectOutputStream(ostream);
	    p.writeObject(sm);
	    p.flush();
	    ostream.close();
	} catch (Exception e){
	    e.printStackTrace();
	}
    }
}
