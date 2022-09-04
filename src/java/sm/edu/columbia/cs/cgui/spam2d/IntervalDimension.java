package edu.columbia.cs.cgui.spam2d;

import java.lang.reflect.*;
import java.lang.*;
import java.io.*;

public class IntervalDimension implements Cloneable, Serializable {
    Method minMethod, maxMethod;
    int intervalID;
    public IntervalDimension(int id,Method min, Method max){
	intervalID=id;
	minMethod=min;
	maxMethod=max;
    }
    public int getID(){
	return (intervalID);
    }
    public String toString(){
	return (" intervalID=" + intervalID);
    }
    public Object clone(){
	IntervalDimension clone = null;
	try {
	    clone = (IntervalDimension) super.clone();
	} catch (CloneNotSupportedException e) { 
	    throw new InternalError();
	}
	clone.minMethod = minMethod;
	clone.maxMethod = maxMethod;
	clone.intervalID = intervalID;
	return (clone);
    }
    private void writeObject(java.io.ObjectOutputStream out)
	throws IOException {
	out.writeObject(minMethod.getDeclaringClass());
	out.writeObject(minMethod.getName());
	out.writeObject(maxMethod.getName());
	out.writeInt(intervalID);
    }
    private void readObject(java.io.ObjectInputStream in)
	throws IOException, ClassNotFoundException {
	Class nodeClass;
	try {
	    nodeClass = (Class) in.readObject();
	    minMethod = nodeClass.getDeclaredMethod((String) in.readObject(),null);
	    maxMethod = nodeClass.getDeclaredMethod((String) in.readObject(),null);
	    intervalID = in.readInt();
	} catch (NoSuchMethodException e){
	    e.printStackTrace();
	} catch (Exception e){
	    e.printStackTrace();
	}
	
    }
}

