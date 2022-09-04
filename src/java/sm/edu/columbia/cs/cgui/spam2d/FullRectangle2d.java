package edu.columbia.cs.cgui.spam2d;

import java.io.*;

public class FullRectangle2d extends Rectangle2d implements Serializable {
    Long id;
    public FullRectangle2d(){
	super();
	id=new Long(0l);
    }
    public FullRectangle2d(long i,double mx, double my, double xx, double xy){
	super(mx,my,xx,xy);
	id=new Long(i);
    }
    public FullRectangle2d(FullRectangle2d fr){
	this(fr.id.longValue(),(Rectangle2d) fr);
    }
    public FullRectangle2d(long i,Rectangle2d r){
	super(r);
	id=new Long(i);
    }
    public boolean equals(Object obj){
	if (!(obj instanceof FullRectangle2d)){
	    return false;
	}
	return (id.equals(((FullRectangle2d)obj).id));
    }
    public String toString(){
	return ("id=" + id + " " + super.toString());
    }
}
