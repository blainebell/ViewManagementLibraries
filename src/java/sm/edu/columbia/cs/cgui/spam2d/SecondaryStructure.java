package edu.columbia.cs.cgui.spam2d;

import java.lang.*;
import java.util.*;
import java.lang.reflect.*;
import java.io.*;

public abstract class SecondaryStructure implements Cloneable, Serializable {
    //    private Class nodeClass;
    public SecondaryStructure(){
	//	nodeClass = this.nodeClass;
    }
    public abstract boolean add ( Object ob);
    public abstract boolean isEmpty();
    public abstract Vector enclosedBy(Object r, NeededCriteria nC,boolean justGetOne);
    public abstract Vector windowQuery(Object r, NeededCriteria nC,boolean inclusive);
    public abstract boolean remove(Object ob);
    public abstract Vector getAll(Object ob);
    public abstract void print(PrintStream ps);
    public Object clone(){
	//	    throws CloneNotSupportedException {
	try { 
	    return (super.clone());
	} catch (CloneNotSupportedException e){
	    throw new InternalError();		
	}
    }
}
