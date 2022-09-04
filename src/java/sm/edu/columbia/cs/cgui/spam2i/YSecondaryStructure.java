package edu.columbia.cs.cgui.spam2i;

import java.util.*;
import java.io.*;

public class YSecondaryStructure extends SecondaryStructure implements Cloneable {
    private static final long serialVersionUID=4821928361278636193L;
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
    }
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
	return (ret);
    }
    public Vector getAll(Object ob){
	return (new Vector(minY));
    }
  public void printObjects(PrintStream ps){
	Iterator it = minY.iterator();
	while (it.hasNext()){
	    Rectangle2i rs=(Rectangle2i) it.next();
	    ps.println(rs);
	}
  }
    public void print(PrintStream ps){
	Iterator it = minY.iterator();
	while (it.hasNext()){
	    Rectangle2i rs=(Rectangle2i) it.next();
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
  public Iterator iterator(){
    return (minY.iterator());
  }
}
