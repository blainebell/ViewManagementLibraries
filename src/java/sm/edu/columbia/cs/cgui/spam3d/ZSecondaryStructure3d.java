package edu.columbia.cs.cgui.spam3d;

import java.util.*;
import java.io.*;

class ZSecondaryStructure3d extends SecondaryStructure implements Cloneable {
    private static final long serialVersionUID=4843249190348161239L;
    RectangleTree3d minZ, maxZ;
    ZSecondaryStructure3d(){
	minZ = new RectangleTree3d(new MinZComparator3d());
	maxZ = new RectangleTree3d(new MaxZComparator3d());
    }
    boolean add ( Object ob){
	minZ.add(ob);
	maxZ.add(ob);
	return (true);
    }
    boolean isEmpty(){
	return (minZ.isEmpty());
    };
    Vector enclosedBy(Object r, NeededCriteria nC,boolean justGetOne){
	return (getNeededCriteria(nC));
    }
    Vector windowQuery(Object r, NeededCriteria nC,boolean inclusive){
	//	System.out.println("ZSecondaryStructure3d.windowQuery");
	return (getNeededCriteria(nC));
    }
    boolean remove(Object ob){
	minZ.remove(ob);
	maxZ.remove(ob);
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
	    ret.addAll(minZ);
	} else if (minCons && maxCons){
	    ret.addAll(minZ.headSet(minInc,minV));
	    ret.retainAll(maxZ.tailSet(maxInc,maxV));
	} else if (minCons){
	    //	    System.out.println("minZ.headSet(minInc=" + minInc + " minV=" + minV);
	    ret.addAll(minZ.headSet(minInc,minV));
	} else if (maxCons){
	    ret.addAll(maxZ.tailSet(maxInc,maxV));
	}
	return (ret);
    }
    Vector getAll(Object ob){
	return (new Vector(minZ));
    }
    void print(PrintStream ps){
	Iterator it = minZ.iterator();
	while (it.hasNext()){
	    Rectangle2d rs=(Rectangle2d) it.next();
	    ps.println(rs);
	}
	ps.println("");
    }
    protected Object clone(){
	ZSecondaryStructure3d clone = null;
	clone = (ZSecondaryStructure3d) super.clone();
	clone.minZ = (RectangleTree3d) minZ.clone();
	clone.maxZ = (RectangleTree3d) maxZ.clone();
	return (clone);
    }
}
