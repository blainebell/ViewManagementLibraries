package edu.columbia.cs.cgui.spam3d;

import java.lang.*;
import java.util.*;
import java.lang.reflect.*;
import java.io.*;

class YZSecondaryStructure3d extends SecondaryStructure implements Cloneable {
    private static final long serialVersionUID=1237409092348129395L;
    private IntervalTree3d secondaryIntervalTree;
    static private Method minzMeth,maxzMeth;
    static private Class ss;
    static private Class nodeClass;
    static private IntervalDimension iD;
    static {
	try {
	    nodeClass=Class.forName("edu.columbia.cs.cgui.spam3d.Rectangle3d");
	    minzMeth=nodeClass.getDeclaredMethod("getMinZ",null); 
	    maxzMeth=nodeClass.getDeclaredMethod("getMaxZ",null);
	    ss = Class.forName("edu.columbia.cs.cgui.spam3d.SecondYZSecondaryStructure3d");
	} catch (ClassNotFoundException cnf){
	    cnf.printStackTrace();
	} catch (NoSuchMethodException nsm){
	    nsm.printStackTrace();
	}
	iD = new IntervalDimension(2,minzMeth,maxzMeth);
    }
    //    private TreeSet minTree,maxTree;
    YZSecondaryStructure3d() throws NoSuchMethodException, ClassNotFoundException {
	secondaryIntervalTree = new IntervalTree3d(nodeClass,minzMeth,maxzMeth,ss,iD);
    }
    protected Object clone(){
	YZSecondaryStructure3d clone = null;
	clone = (YZSecondaryStructure3d) super.clone();
	clone.secondaryIntervalTree = (IntervalTree3d)secondaryIntervalTree.clone();
	return (clone);
    }
    private void writeObject(java.io.ObjectOutputStream out)
	throws IOException {
	out.writeObject(secondaryIntervalTree);
	//	    out.writeObject(minTree);
	//	    out.writeObject(maxTree);
    }
    private void readObject(java.io.ObjectInputStream in)
	throws IOException, ClassNotFoundException {
	secondaryIntervalTree = (IntervalTree3d)in.readObject();
	//	    minTree = (TreeSet) in.readObject();
	//	    maxTree = (TreeSet) in.readObject();
    }
    Vector enclosedBy(Object r, NeededCriteria nC,boolean justGetOne){
	return(secondaryIntervalTree.enclosedBy(r,nC,justGetOne));
    }
    Vector windowQuery(Object r,NeededCriteria nC,boolean inclusive){
	//	System.out.println("YZSecondaryStructure3d.windowQuery\t r=" + r + "\tnC=");
	//	nC.print(System.out);
	return(secondaryIntervalTree.windowQuery(r,nC,inclusive));
    }
    Vector getAll(Object ob){
	return (secondaryIntervalTree.getAll(ob));
    }
    boolean add ( Object ob){
	secondaryIntervalTree.addRectangle((Rectangle3d)ob);
	return (true);
    }
    boolean isEmpty(){
	return(secondaryIntervalTree.isEmpty());
    }
    boolean remove(Object ob){
	return (secondaryIntervalTree.deleteRectangle((Rectangle3d)ob));
    }
    void print(PrintStream ps){
	System.out.println("YZSecondaryStructure3d.print()");
	secondaryIntervalTree.print(ps);
    }
}
