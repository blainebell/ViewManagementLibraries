package edu.columbia.cs.cgui.spam2d;

import java.io.*;
import java.lang.reflect.*;
import java.lang.*;
import java.util.*;

class NormalGenerator {
    public static void main(String[] args) {
	SpaceManager sm = null;
	try {
	    sm=new SpaceManager(new Rectangle2d(0.,0.,1.,1.));
	} catch (Exception e){
	    e.printStackTrace();
	}
	Random rand=new Random();
	for (int i=0; i<50; i++){
	    double nx,ny,nw,nh;
	    nx = rand.nextDouble();
	    ny = rand.nextDouble();

	    nw = (rand.nextDouble()*.1);
	    nh = (rand.nextDouble()*.1);
	    if ((nx+nw)>1.){ nx=nx-nw; } 
	    nw = nx+nw;
	    if ((ny+nh)>1.){ ny=ny-nh; }
	    nh = ny+nh;
	    
	    FullRectangle2d fr;
	    fr=new FullRectangle2d(0l,nx,ny,nw,nh);
	    //	    System.out.println("fr="+ fr);
	    sm.addRectangle(fr,-1);
	    //	    if (i%100==99){
		System.out.println("numRectangles=" + (i+1) + "\t#spaces=" + sm.numberSpaces());
		FileOutputStream ostream=null;
		ObjectOutputStream p=null ;
		try {
		    ostream = new FileOutputStream(new File("normal" + (i+1) + ".smf"));
		    p = new ObjectOutputStream(ostream);
		    p.writeObject(sm);
		    p.flush();
		    ostream.close();
		    //		    System.out.println("wrote normal" + (i+1) + ".smf");
		    System.out.flush();
		} catch (Exception e){
		    e.printStackTrace();
		}
		//	    }
	}
    }
}
