package edu.columbia.cs.cgui.spam2i;

import java.lang.*;
import java.util.*;
import java.lang.reflect.*;
import java.io.*;

class FirstSecondaryStructure extends SecondaryStructure implements Cloneable {
    private static final long serialVersionUID=4842266493197182389L;
    private IntervalTree secondaryIntervalTree;
    static private Method minyMeth,maxyMeth;
    static private Class ss;
    static private Class nodeClass;
    static private IntervalDimension iD;
    static {
	try {
	    nodeClass=Class.forName("edu.columbia.cs.cgui.spam2i.Rectangle2i");
	    minyMeth=nodeClass.getDeclaredMethod("getMinY",null); 
	    maxyMeth=nodeClass.getDeclaredMethod("getMaxY",null);
	    ss = Class.forName("edu.columbia.cs.cgui.spam2i.SecondSecondaryStructure");
	} catch (ClassNotFoundException cnf){
	    cnf.printStackTrace();
	} catch (NoSuchMethodException nsm){
	    nsm.printStackTrace();
	}
	iD = new IntervalDimension(2,minyMeth,maxyMeth);
    }
    //    private TreeSet minTree,maxTree;
    FirstSecondaryStructure() throws NoSuchMethodException, ClassNotFoundException {
	secondaryIntervalTree = new IntervalTree(nodeClass,minyMeth,maxyMeth,ss,iD);
    }
    public Object clone(){
	FirstSecondaryStructure clone = null;
	clone = (FirstSecondaryStructure) super.clone();
	clone.secondaryIntervalTree = (IntervalTree)secondaryIntervalTree.clone();
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
	secondaryIntervalTree = (IntervalTree)in.readObject();
	//	    minTree = (TreeSet) in.readObject();
	//	    maxTree = (TreeSet) in.readObject();
    }
    public Vector enclosedBy(Object r, NeededCriteria nC,boolean justGetOne){
	return(secondaryIntervalTree.enclosedBy(r,nC,justGetOne));
    }
    public Vector windowQuery(Object r,NeededCriteria nC,boolean inclusive){
	//	System.out.println("FirstSecondaryStructure.windowQuery\t r=" + r + "\tnC=");
	//	nC.print(System.out);
	return(secondaryIntervalTree.windowQuery(r,nC,inclusive));
    }
    public Vector getAll(Object ob){
	return (secondaryIntervalTree.getAll(ob));
    }
    public boolean add ( Object ob){
	secondaryIntervalTree.addRectangle((Rectangle2i)ob);
	return (true);
    }
    public boolean isEmpty(){
	return(secondaryIntervalTree.isEmpty());
    }
    public boolean remove(Object ob){
	return (secondaryIntervalTree.deleteRectangle((Rectangle2i)ob));
    }
    public void printObjects(PrintStream ps){
      secondaryIntervalTree.printObjects(ps);
    }
    public void print(PrintStream ps){
	System.out.println("FirstSecondaryStructure.print()");
	secondaryIntervalTree.print(ps);
    }
  public Iterator iterator(){
    return (secondaryIntervalTree.iterator());
  }
}
