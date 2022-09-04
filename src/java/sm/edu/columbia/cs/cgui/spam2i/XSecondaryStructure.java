package edu.columbia.cs.cgui.spam2i;

import java.util.*;
import java.io.*;

public class XSecondaryStructure extends SecondaryStructure implements Cloneable {
    private static final long serialVersionUID=4842266493143282389L;
    RectangleTree minX, maxX;
    public XSecondaryStructure(){
	minX = new RectangleTree(new MinXComparator());
	maxX = new RectangleTree(new MaxXComparator());
    }
    public boolean add ( Object ob){
	minX.add(ob);
	maxX.add(ob);
	return (true);
    }
    public boolean isEmpty(){
	return (minX.isEmpty());
    }
    public Vector enclosedBy(Object r, NeededCriteria nC,boolean justGetOne){
	return (getNeededCriteria(nC));
    }
    public Vector windowQuery(Object r, NeededCriteria nC,boolean inclusive){
	return (getNeededCriteria(nC));
    }
    public boolean remove(Object ob){
	minX.remove(ob);
	maxX.remove(ob);
	return (true);
    }
    
    public Vector getNeededCriteria(NeededCriteria nC){
	Enumeration e=nC.getAllCriteria();
	Vector ret = new Vector();
	int minV=0, maxV=0;
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
		System.out.println("\t" + ((Rectangle2i)e2.nextElement()));
		}
	*/
	return (ret);
    }
    public Vector getAll(Object ob){
	return (new Vector(minX));
    }
  public void printObjects(PrintStream ps){
	Iterator it = minX.iterator();
	while (it.hasNext()){
	    Rectangle2i rs=(Rectangle2i) it.next();
	    ps.println(rs);
	}
  }
    public void print(PrintStream ps){
	Iterator it = minX.iterator();
	while (it.hasNext()){
	    Rectangle2i rs=(Rectangle2i) it.next();
	    ps.println(rs);
	}
	ps.println("");
    }
    public Object clone(){
	XSecondaryStructure clone = null;
	clone = (XSecondaryStructure) super.clone();
	clone.minX = (RectangleTree) minX.clone();
	clone.maxX = (RectangleTree) maxX.clone();
	return (clone);
    }
  public Iterator iterator(){
    return (minX.iterator());
  }
}
