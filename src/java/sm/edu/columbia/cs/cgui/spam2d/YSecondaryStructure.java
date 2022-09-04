package edu.columbia.cs.cgui.spam2d;

import java.util.*;
import java.io.*;

public class YSecondaryStructure extends SecondaryStructure implements Cloneable {
    private static final long serialVersionUID=4842266493143282389L;
    RectangleTree minY, maxY;
    public YSecondaryStructure(){
	minY = new RectangleTree(new MinYComparator());
	maxY = new RectangleTree(new MaxYComparator());
    }
    public boolean add ( Object ob){
	minY.add(ob);
	maxY.add(ob);
	return (true);
    }
    public boolean isEmpty(){
	return (minY.isEmpty());
    };
    public Vector enclosedBy(Object r, NeededCriteria nC,boolean justGetOne){
	return (getNeededCriteria(nC));
    }
    public Vector windowQuery(Object r, NeededCriteria nC,boolean inclusive){
	//	System.out.println("YSecondaryStructure.windowQuery");
	return (getNeededCriteria(nC));
    }
    public boolean remove(Object ob){
	minY.remove(ob);
	maxY.remove(ob);
	return (true);
    }
    
    public Vector getNeededCriteria(NeededCriteria nC){
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
	//	System.out.println("YSecondaryStructure returns");
	//	Rectangle2d.printCollection(ret);
	//	System.out.println("YSecondaryStructure end");
	return (ret);
    }
    public Vector getAll(Object ob){
	return (new Vector(minY));
    }
    public void print(PrintStream ps){
	Iterator it = minY.iterator();
	while (it.hasNext()){
	    Rectangle2d rs=(Rectangle2d) it.next();
	    ps.println(rs);
	}
	ps.println("");
    }
    public Object clone(){
	YSecondaryStructure clone = null;
	clone = (YSecondaryStructure) super.clone();
	clone.minY = (RectangleTree) minY.clone();
	clone.maxY = (RectangleTree) maxY.clone();
	return (clone);
    }
}
