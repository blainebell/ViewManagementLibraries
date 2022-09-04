package edu.columbia.cs.cgui.spam3d;

import java.util.*;
import java.io.*;

class YSecondaryStructure3d extends SecondaryStructure implements Cloneable {
    private static final long serialVersionUID=1902831750981723512L;
    RectangleTree3d minY, maxY;
    YSecondaryStructure3d(){
	minY = new RectangleTree3d(new MinYComparator3d());
	maxY = new RectangleTree3d(new MaxYComparator3d());
    }
    boolean add ( Object ob){
	minY.add(ob);
	maxY.add(ob);
	return (true);
    }
    boolean isEmpty(){
	return (minY.isEmpty());
    };
    Vector enclosedBy(Object r, NeededCriteria nC,boolean justGetOne){
	return (getNeededCriteria(nC));
    }
    Vector windowQuery(Object r, NeededCriteria nC,boolean inclusive){
	//	System.out.println("YSecondaryStructure3d.windowQuery");
	return (getNeededCriteria(nC));
    }
    boolean remove(Object ob){
	minY.remove(ob);
	maxY.remove(ob);
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
	//	System.out.println("minCons=" + minCons + " maxCons=" + maxCons + " minV=" + minV + " maxV=" + maxV);
	if (!minCons && !maxCons){
	    ret.addAll(minY);
	} else if (minCons && maxCons){
	    ret.addAll(minY.headSet(minInc,minV));
	    ret.retainAll(maxY.tailSet(maxInc,maxV));
	} else if (minCons){
	    //	    System.out.println("minY.headSet(minInc=" + minInc + " minV=" + minV);
	    ret.addAll(minY.headSet(minInc,minV));
	} else if (maxCons){
	    ret.addAll(maxY.tailSet(maxInc,maxV));
	}
	//	System.out.println("minY=");
	//	Rectangle2d.printCollection(minY);
	//	System.out.println("YSecondaryStructure3d returns");
	//	Rectangle2d.printCollection(ret);
	//	System.out.println("YSecondaryStructure3d end");
	return (ret);
    }
    Vector getAll(Object ob){
	return (new Vector(minY));
    }
    void print(PrintStream ps){
	Iterator it = minY.iterator();
	while (it.hasNext()){
	    Rectangle2d rs=(Rectangle2d) it.next();
	    ps.println(rs);
	}
	ps.println("");
    }
    protected Object clone(){
	YSecondaryStructure3d clone = null;
	clone = (YSecondaryStructure3d) super.clone();
	clone.minY = (RectangleTree3d) minY.clone();
	clone.maxY = (RectangleTree3d) maxY.clone();
	return (clone);
    }
}
