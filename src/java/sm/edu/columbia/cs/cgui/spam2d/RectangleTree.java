package edu.columbia.cs.cgui.spam2d;

import java.util.*;

public class RectangleTree extends TreeSet {
    public static final boolean X=false;
    public static final boolean Y=true;
    private boolean direction=false;
    public static final boolean MIN=true; 
    public static final boolean MAX=false; 
    private boolean min=false;
    private Rectangle2d pos1,neg1,pos2,neg2;
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
	    pos1 = new Rectangle2d(0.,Double.POSITIVE_INFINITY,0.,Double.POSITIVE_INFINITY);
	    neg1 = new Rectangle2d(0.,Double.NEGATIVE_INFINITY,0.,Double.NEGATIVE_INFINITY);
	} else {
	    pos1 = new Rectangle2d(Double.POSITIVE_INFINITY,0.,Double.POSITIVE_INFINITY,0.);
	    neg1 = new Rectangle2d(Double.NEGATIVE_INFINITY,0.,Double.NEGATIVE_INFINITY,0.);
	}
	pos2 = new Rectangle2d(pos1);
	neg2 = new Rectangle2d(neg1);
    }
    public Rectangle2d setRect(Rectangle2d r,double n){
	if (direction==X){
	    r.setMinX(n);
	    r.setMaxX(n);
	} else {
	    r.setMinY(n);
	    r.setMaxY(n);
	}
	return(r);
    }
    public SortedSet headSet(boolean inclusive,double to){
	Rectangle2d xr;
	if (inclusive){
	    xr=setRect(pos1,to);
	} else {
	    xr=setRect(neg1,to);
	}
	return (headSet(xr));
    }
    public SortedSet tailSet(boolean inclusive,double from){
	Rectangle2d xr;
	if (inclusive){
	    xr=setRect(neg2,from);
	} else {
	    xr=setRect(pos2,from);
	}
	return (tailSet(xr));
    }
    public SortedSet subSet(boolean mininclusive, boolean maxinclusive,double min, double max){
	Rectangle2d nr,xr;
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




