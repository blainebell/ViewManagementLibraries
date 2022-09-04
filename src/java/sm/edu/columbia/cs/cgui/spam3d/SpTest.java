package edu.columbia.cs.cgui.spam3d;

import java.io.*;
import java.lang.reflect.*;
import java.lang.*;
import java.util.*;

class SpTest {
    public static void main(String[] args) {
	SpaceManager3d sm=null;
	Rectangle3d rect = new Rectangle3d(.0,.0,.0,1.,1.,1.);
	try {
	    sm = new SpaceManager3d(rect);
	} catch (Exception e){
	    e.printStackTrace();
	}
	FullRectangle3d frect = new FullRectangle3d(1,.25,.25,.25,.75,.75,.75);
	FullRectangle3d frect2 = new FullRectangle3d(2,.5,.5,.5,.85,.85,.85);
	
	sm.addRectangle(frect,0);

	sm.addRectangle(frect2,0);
	Iterator it=sm.spaceIterator();
	while (it.hasNext()){
	    System.out.println(it.next());
	}
    }
}

