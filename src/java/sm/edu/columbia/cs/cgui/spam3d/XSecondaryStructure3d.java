package edu.columbia.cs.cgui.spam3d;

import java.util.*;
import java.io.*;

class XSecondaryStructure3d extends SecondaryStructure implements Cloneable {
    private static final long serialVersionUID=4842213246126124389L;
    RectangleTree3d minX, maxX;
    XSecondaryStructure3d(){
	minX = new RectangleTree3d(new MinXComparator3d());
	maxX = new RectangleTree3d(new MaxXComparator3d());
    }
    boolean add ( Object ob){
	minX.add(ob);
	maxX.add(ob);
	return (true);
    }
    boolean isEmpty(){
	return (minX.isEmpty());
    };
    Vector enclosedBy(Object r, NeededCriteria nC,boolean justGetOne){
	return (getNeededCriteria(nC));
    }
    Vector windowQuery(Object r, NeededCriteria nC,boolean inclusive){
	return (getNeededCriteria(nC));
    }
    boolean remove(Object ob){
	minX.remove(ob);
	maxX.remove(ob);
	return (true);
    }
    
    Vector getNeededCriteria(NeededCriteria nC){
	Enumeration e=nC.getAllCriteria();
	Vector ret = new Vector();
	double minV=0., maxV=0.;
	boolean minInc=false, maxInc=false;
	boolean minCons=false, maxCons=false;
	if (e.hasMoreElements()){
	    NeededCriteria.NeededCondition nc=(NeededCriteria.NeededCondition)e.nextElement();
	    if (nc.direction==0){
		minCons=true;
		minInc=nc.clusive;
		minV=nc.value;
	    } else if (nc.direction==1){
		maxCons=true;
		maxInc=nc.clusive;
		maxV=nc.value;
	    }
	}
	//	System.out.println("minCo
	if (!minCons && !maxCons){
	    ret.addAll(minX);
	} else if (minCons && maxCons){
	    ret.addAll(minX.headSet(minInc,minV));
	    ret.retainAll(maxX.tailSet(maxInc,maxV));
	} else if (minCons){
	    ret.addAll(minX.headSet(minInc,minV));
	} else if (maxCons){
	    //	    System.out.println("maxX maxInc=" + maxInc + " maxV=" + maxV);
	    ret.addAll(maxX.tailSet(maxInc,maxV));
	}
	/*	if (minCons && maxCons){
		System.out.println("getNeededCriteria: greater than minV=" + minV + "\tand less than maxV=" + maxV);
		} else if (minCons){
		System.out.println("getNeededCriteria: greater than minV=" + minV);
		} else if (maxCons){
		System.out.println("getNeededCriteria: less than maxV=" + maxV);
		} else {
		System.out.println("getNeededCriteria: no constraints, get all");
		}
		for (Enumeration e2=ret.elements(); e2.hasMoreElements();){
		System.out.println("\t" + ((Rectangle2d)e2.nextElement()));
		}
	*/
	return (ret);
    }
    Vector getAll(Object ob){
	return (new Vector(minX));
    }
    void print(PrintStream ps){
	Iterator it = minX.iterator();
	while (it.hasNext()){
	    Rectangle3d rs=(Rectangle3d) it.next();
	    ps.println(rs);
	}
	ps.println("");
    }
    protected Object clone(){
	XSecondaryStructure3d clone = null;
	clone = (XSecondaryStructure3d) super.clone();
	clone.minX = (RectangleTree3d) minX.clone();
	clone.maxX = (RectangleTree3d) maxX.clone();
	return (clone);
    }
}
