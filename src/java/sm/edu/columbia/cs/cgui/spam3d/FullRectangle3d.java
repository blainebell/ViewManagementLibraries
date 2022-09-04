package edu.columbia.cs.cgui.spam3d;

import java.io.*;

public class FullRectangle3d extends Rectangle3d implements Serializable {
    Long id;
    FullRectangle3d(){
	super();
	id=new Long(0l);
    }
    FullRectangle3d(long i,double mx, double my, double mz, double xx, double xy, double xz){
	super(mx,my,mz,xx,xy,xz);
	id=new Long(i);
    }
    FullRectangle3d(FullRectangle3d fr){
	this(fr.id.longValue(),(Rectangle3d) fr);
    }
    FullRectangle3d(long i,Rectangle3d r){
	super(r);
	id=new Long(i);
    }
    public boolean equals(Object obj){
	if (!(obj instanceof FullRectangle3d)){
	    return false;
	}
	return (id.equals(((FullRectangle3d)obj).id));
    }
    public String toString(){
	return ("id=" + id + " " + super.toString());
    }
}
