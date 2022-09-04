package edu.columbia.cs.cgui.spam2i;

import java.io.*;

public class FullRectangle2i extends Rectangle2i implements Serializable {
    private static final long serialVersionUID = 123947193816571283L;
    Long id;
  public void setID(long i){
    id = new Long(i);
  }
    public FullRectangle2i(){
	super();
	id=new Long(0l);
    }
    public FullRectangle2i(long i,int mx, int my, int xx, int xy){
	super(mx,my,xx,xy);
	id=new Long(i);
    }
    public FullRectangle2i(FullRectangle2i fr){
	this(fr.id.longValue(),(Rectangle2i) fr);
    }
    public FullRectangle2i(long i,Rectangle2i r){
	super(r);
	id=new Long(i);
    }
    public boolean equals(Object obj){
	if (!(obj instanceof FullRectangle2i)){
	    return false;
	}
	return (id.equals(((FullRectangle2i)obj).id));
    }
    public String toString(){
	return ("id=" + id + " " + super.toString());
    }
}
