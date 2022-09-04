package edu.columbia.cs.cgui.spam2i;

import java.util.*;

public class RectangleTree extends TreeSet {
    public static final boolean X=false;
    public static final boolean Y=true;
    private boolean direction=false;
    public static final boolean MIN=true; 
    public static final boolean MAX=false; 
    private boolean min=false;
    private Rectangle2i pos1,neg1,pos2,neg2;
    public RectangleTree(Comparator c){
	super(c);
	if (c instanceof MinXComparator){
	    direction=X;
	    min=true;
	} else if (c instanceof MaxXComparator){
	    direction=X;
	    min=false;
	} else if (c instanceof MinYComparator){
	    direction=Y;
	    min=true;
	} else if (c instanceof MaxYComparator){
	    direction=Y;
	    min=false;
	}
	if (direction==X){
	    pos1 = new Rectangle2i(0,Integer.MAX_VALUE,0,Integer.MAX_VALUE);
	    neg1 = new Rectangle2i(0,Integer.MIN_VALUE,0,Integer.MIN_VALUE);
	} else {
	    pos1 = new Rectangle2i(Integer.MAX_VALUE,0,Integer.MAX_VALUE,0);
	    neg1 = new Rectangle2i(Integer.MIN_VALUE,0,Integer.MIN_VALUE,0);
	}
	pos2 = new Rectangle2i(pos1);
	neg2 = new Rectangle2i(neg1);
    }
    public Rectangle2i setRect(Rectangle2i r,int n){
	if (direction==X){
	    r.setMinX(n);
	    r.setMaxX(n);
	} else {
	    r.setMinY(n);
	    r.setMaxY(n);
	}
	return(r);
    }
    public SortedSet headSet(boolean inclusive,int to){
	Rectangle2i xr;
	if (inclusive){
	    xr=setRect(pos1,to);
	} else {
	    xr=setRect(neg1,to);
	}
	return (headSet(xr));
    }
    public SortedSet tailSet(boolean inclusive,int from){
	Rectangle2i xr;
	if (inclusive){
	    xr=setRect(neg2,from);
	} else {
	    xr=setRect(pos2,from);
	}
	return (tailSet(xr));
    }
    public SortedSet subSet(boolean mininclusive, boolean maxinclusive,int min, int max){
	Rectangle2i nr,xr;
	if (mininclusive){
	    nr=setRect(neg1,min);
	}  else {
	    nr=setRect(pos1,min);
	}
	if (maxinclusive){
	    xr=setRect(pos2,max);
	} else {
	    xr=setRect(neg2,max);
	}
	return (subSet(nr,xr));
    }
}




