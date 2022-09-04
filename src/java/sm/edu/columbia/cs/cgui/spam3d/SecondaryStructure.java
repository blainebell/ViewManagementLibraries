package edu.columbia.cs.cgui.spam3d;

import java.lang.*;
import java.util.*;
import java.lang.reflect.*;
import java.io.*;

public abstract class SecondaryStructure implements Cloneable, Serializable {
    //    private Class nodeClass;
    SecondaryStructure(){
	//	nodeClass = this.nodeClass;
    }
    abstract boolean add ( Object ob);
    abstract boolean isEmpty();
    abstract Vector enclosedBy(Object r, NeededCriteria nC,boolean justGetOne);
    abstract Vector windowQuery(Object r, NeededCriteria nC,boolean inclusive);
    abstract boolean remove(Object ob);
    abstract Vector getAll(Object ob);
    abstract void print(PrintStream ps);
    protected Object clone(){
	//	    throws CloneNotSupportedException {
	try { 
	    return (super.clone());
	} catch (CloneNotSupportedException e){
	    throw new InternalError();		
	}
    }
}
