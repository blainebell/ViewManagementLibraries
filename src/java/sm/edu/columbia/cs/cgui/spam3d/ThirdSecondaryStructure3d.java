package edu.columbia.cs.cgui.spam3d;

import java.lang.*;
import java.util.*;
import java.lang.reflect.*;
import java.io.*;
import java.text.*;

class ThirdSecondaryStructure3d extends SecondaryStructure implements Cloneable {
    private static final long serialVersionUID=2269192438058723953L;
    private RectangleTree3d rs[] = new RectangleTree3d[6];
    
    ThirdSecondaryStructure3d() throws NoSuchMethodException, ClassNotFoundException {
	rs[0] = new RectangleTree3d(new MinXComparator3d());
	rs[1] = new RectangleTree3d(new MaxXComparator3d());
	rs[2] = new RectangleTree3d(new MinYComparator3d());
	rs[3] = new RectangleTree3d(new MaxYComparator3d());
	rs[4] = new RectangleTree3d(new MinZComparator3d());
	rs[5] = new RectangleTree3d(new MaxZComparator3d());
    }
    protected Object clone(){
	ThirdSecondaryStructure3d clone = null;
	clone = (ThirdSecondaryStructure3d) super.clone();
	for (int i=0; i<6; i++){
	    clone.rs[i] = (RectangleTree3d) rs[i].clone();
	}
	return (clone);
    }
    private void writeObject(java.io.ObjectOutputStream out)
	throws IOException {
	for (int i=0; i<6; i++){
	    out.writeObject(rs[i]);
	}
    }
    private void readObject(java.io.ObjectInputStream in)
	throws IOException, ClassNotFoundException {
	rs = new RectangleTree3d[6];
	for (int i=0; i<6; i++){
	    rs[i] = (RectangleTree3d) in.readObject();
	}
    }
    Vector enclosedBy(Object r, NeededCriteria nC,boolean justGetOne){
	return (getNeededCriteria(nC));
    }
    Vector getAll(Object ob){
	return(getNeededCriteria(new NeededCriteria()));
    }
    Vector getNeededCriteria(NeededCriteria nC){
	Vector ret = new Vector();
	NeededCriteria.NeededCondition nCond;
	Enumeration e = nC.getAllCriteria();
	TreeSet tmpTree=null;
	int tmpTreeNum=0;
	Rectangle3d tmpRect=null;
	if (!e.hasMoreElements()){
	    ret.addAll(new Vector(rs[0]));
	} else {
	    int nCnum=0;
	    while(e.hasMoreElements()){
		tmpTreeNum=0;
		nCond = (NeededCriteria.NeededCondition) e.nextElement();
		tmpTreeNum=nCond.direction; // Odd if greater-than (meaning compare with max, otherwise even
		if (nCond.dimension.getID()==1){
		} else if (nCond.dimension.getID()==2){
		    tmpTreeNum+=2;
		} else if (nCond.dimension.getID()==3){
		    tmpTreeNum+=4;
		}
		
		if (nCond.direction==0){
		    tmpTree=new TreeSet(rs[tmpTreeNum].headSet(nCond.clusive,nCond.value));
		}else {
		    tmpTree=new TreeSet(rs[tmpTreeNum].tailSet(nCond.clusive,nCond.value));
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
	int contnum=0;
	return(ret);
    }
    Vector windowQuery(Object r,NeededCriteria nC,boolean inclusive){
	return (getNeededCriteria(nC));
    }
    boolean add ( Object ob){
	for (int i=0; i<6; i++){
	    rs[i].add(ob);
	}
	return (true);
    }
    boolean isEmpty(){
	return(rs[0].isEmpty());
    }
    boolean remove(Object ob){
	for (int i=0; i<6; i++){
	    rs[i].remove(ob);
	}
	return (true);
    }
    void print(PrintStream ps){
	System.out.println("ThirdSecondaryStructure3d.print()");
	Iterator it = rs[0].iterator();
	DecimalFormat df = new DecimalFormat("##.###");
	while (it.hasNext()){
	    Rectangle3d rs3 = (Rectangle3d)it.next();
	    ps.print( "\t" + rs3 + "\t");
	}
	ps.println("");
	
    }
}
