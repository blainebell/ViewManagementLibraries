package edu.columbia.cs.cgui.spam2d;

import java.util.*;
import java.io.*;

public class NeededCriteria {
    Stack neededCriteria = new Stack();
    static final int LESS_THAN=0;
    static final int GREATER_THAN=1;
    static final int INCLUSIVE=0;
    static final int EXCLUSIVE=2;
    static final int ALL=4;
    public int size(){
	return (neededCriteria.size());
    }
    public class NeededCondition {
	public IntervalDimension dimension;
	public int direction; // 0=less-than 1=greater-than 2=all 
	public boolean clusive; // true=inclusive false=exclusive
	public double value;
	NeededCondition(IntervalDimension id, int dir, boolean cl , double v){
	    dimension=id;
	    direction=dir;
	    clusive=cl;
	    value=v;
	}
	public String toString(){
	    return ("dimension="+dimension + "\tdirection=" + direction + "\tclusive=" + clusive + "\tvalue="+value);
	}
	void print(PrintStream ps){
	    ps.println(toString());
	}
    } 
    public NeededCriteria(){
    }
    public String toString(){
	String str = "";
	for (Enumeration e=neededCriteria.elements();e.hasMoreElements();){
	    str = str + ((NeededCondition)e.nextElement());
	}
	return (str);
    }
    public void print(PrintStream ps){
	ps.print(toString());
    }
    public void pushCriteria(IntervalDimension id, int dir, boolean cl, double value){
	neededCriteria.push(new NeededCondition(id,dir,cl,value));
    }
    public NeededCondition popCriteria(){
	return ((NeededCondition)neededCriteria.pop());
    }
    public Enumeration getAllCriteria(){
	return (neededCriteria.elements());
    }
}
