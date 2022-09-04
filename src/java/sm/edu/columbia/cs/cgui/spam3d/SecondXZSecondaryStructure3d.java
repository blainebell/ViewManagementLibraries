package edu.columbia.cs.cgui.spam3d;

import java.lang.*;
import java.util.*;
import java.lang.reflect.*;
import java.io.*;
import java.text.*;

class SecondXZSecondaryStructure3d extends SecondaryStructure implements Cloneable {
    private static final long serialVersionUID=2385712039912385923L;
    private RectangleTree3d rt[] = new RectangleTree3d[4];
    
    SecondXZSecondaryStructure3d() throws NoSuchMethodException, ClassNotFoundException {
	rt[0] = new RectangleTree3d(new MinXComparator3d());
	rt[1] = new RectangleTree3d(new MaxXComparator3d());
	rt[2] = new RectangleTree3d(new MinZComparator3d());
	rt[3] = new RectangleTree3d(new MaxZComparator3d());
    }
    protected Object clone(){
	SecondXZSecondaryStructure3d clone = null;
	clone = (SecondXZSecondaryStructure3d) super.clone();
	for (int i=0; i<4; i++){
	    clone.rt[i] = (RectangleTree3d) rt[i].clone();
	}
	return (clone);
    }
    private void writeObject(java.io.ObjectOutputStream out)
	throws IOException {
	for (int i=0; i<4; i++){
	    out.writeObject(rt[i]);
	}
    }
    private void readObject(java.io.ObjectInputStream in)
	throws IOException, ClassNotFoundException {
	rt = new RectangleTree3d[4];
	for (int i=0; i<4; i++){
	    rt[i] = (RectangleTree3d) in.readObject();
	}
    }
    Vector enclosedBy(Object r, NeededCriteria nC,boolean justGetOne){
	return (getNeededCriteria(nC));
    }
    Vector getAll(Object ob){
	return(getNeededCriteria(new NeededCriteria()));
    }
    Vector getNeededCriteria(NeededCriteria nC){
	//	System.out.println("getNeededCriteria nC.size()=" + nC.size());
	/*	for (Enumeration enum = nC.getAllCriteria();enum.hasMoreElements();){
		System.out.print("\t");
		((NeededCriteria.NeededCondition)enum.nextElement()).print(System.out);
		System.out.println("");
		}
		System.out.println("");
	*/
	Vector ret = new Vector();
	NeededCriteria.NeededCondition nCond;
	Enumeration e = nC.getAllCriteria();
	TreeSet tmpTree=null;
	int tmpTreeNum=0;
	Rectangle2d tmpRect=null;
	if (!e.hasMoreElements()){
	    ret.addAll(new Vector(rt[0]));
	} else {
	    int nCnum=0;
	    while(e.hasMoreElements()){
		tmpTreeNum=0;
		nCond = (NeededCriteria.NeededCondition) e.nextElement();
		tmpTreeNum=nCond.direction; // Odd if greater-than (meaning compare with max, otherwise even
		if (nCond.dimension.getID()==1){
		} else if (nCond.dimension.getID()==2){
		    tmpTreeNum+=2;
		}
		
		if (nCond.direction==0){
		    tmpTree=new TreeSet(rt[tmpTreeNum].headSet(nCond.clusive,nCond.value));
		}else {
		    tmpTree=new TreeSet(rt[tmpTreeNum].tailSet(nCond.clusive,nCond.value));
		}
		int retnum=0;

		retnum=0;
		if (tmpTree.isEmpty()){
		    ret.clear();
		    return(ret);
		}
		if (ret.isEmpty()){
		    ret.addAll(tmpTree);
		} else {
		    ret.retainAll(tmpTree);
		    if (ret.isEmpty()){
			return(ret);
		    }
		}
		retnum=0;
	    }
	}
	//	System.out.println("ret.size()=" + ret.size());
	int contnum=0;
	/*	for (Enumeration enum = ret.elements(); enum.hasMoreElements();){
		Rectangle2d clrs = (Rectangle2d)enum.nextElement();
		System.out.println("ret[" + (contnum++) + "]=" + clrs);
		}
	*/
	return(ret);
    }
    Vector windowQuery(Object r,NeededCriteria nC,boolean inclusive){
	return (getNeededCriteria(nC));
    }
    boolean add ( Object ob){
	for (int i=0; i<4; i++){
	    rt[i].add(ob);
	}
	return (true);
    }
    boolean isEmpty(){
	return(rt[0].isEmpty());
    }
    boolean remove(Object ob){
	for (int i=0; i<4; i++){
	    rt[i].remove(ob);
	}
	return (true);
    }
    void print(PrintStream ps){
	System.out.println("SecondSecondaryStructure.print()");
	Iterator it = rt[0].iterator();
	DecimalFormat df = new DecimalFormat("##.###");
	while (it.hasNext()){
	    Rectangle2d rs = (Rectangle2d)it.next();
	    ps.print( "\t" + rs + "\t");
	}
	ps.println("");
	
    }
}
