package edu.columbia.cs.cgui.spam3d;

import java.util.*;

public class RectangleTree3d extends TreeSet {
    public static final short X=0;
    public static final short Y=1;
    public static final short Z=2;
    private int direction=0;
    public static final boolean MIN=true; 
    public static final boolean MAX=false; 
    private boolean min=false;
    private Rectangle3d pos1,neg1,pos2,neg2;
    public RectangleTree3d(Comparator c){
	super(c);
	if (c instanceof MinXComparator3d){
	    direction=X;
	    min=true;
	} else if (c instanceof MaxXComparator3d){
	    direction=X;
	    min=false;
	} else if (c instanceof MinYComparator3d){
	    direction=Y;
	    min=true;
	} else if (c instanceof MaxYComparator3d){
	    direction=Y;
	    min=false;
	} else if (c instanceof MinZComparator3d){
	    direction=Z;
	    min=true;
	} else if (c instanceof MaxZComparator3d){
	    direction=Z;
	    min=false;
	}
	if (direction==X){
	    pos1 = new Rectangle3d(0.,Double.POSITIVE_INFINITY,Double.POSITIVE_INFINITY,
				   0.,Double.POSITIVE_INFINITY,Double.POSITIVE_INFINITY);
	    neg1 = new Rectangle3d(0.,Double.NEGATIVE_INFINITY,Double.NEGATIVE_INFINITY,
				   0.,Double.NEGATIVE_INFINITY,Double.NEGATIVE_INFINITY);
	} else if (direction==Y) {
	    pos1 = new Rectangle3d(Double.POSITIVE_INFINITY,0.,Double.POSITIVE_INFINITY,
				   Double.POSITIVE_INFINITY,0.,Double.POSITIVE_INFINITY);
	    neg1 = new Rectangle3d(Double.NEGATIVE_INFINITY,0.,Double.NEGATIVE_INFINITY,
				   Double.NEGATIVE_INFINITY,0.,Double.NEGATIVE_INFINITY);
	} else if (direction==Z) {
	    pos1 = new Rectangle3d(Double.POSITIVE_INFINITY,Double.POSITIVE_INFINITY,0.,
				   Double.POSITIVE_INFINITY,Double.POSITIVE_INFINITY,0.);
	    neg1 = new Rectangle3d(Double.NEGATIVE_INFINITY,Double.NEGATIVE_INFINITY,0.,
				   Double.NEGATIVE_INFINITY,Double.NEGATIVE_INFINITY,0.);
	}
	pos2 = new Rectangle3d(pos1);
	neg2 = new Rectangle3d(neg1);
    }
    public Rectangle3d setRect(Rectangle3d r,double n){
	if (direction==X){
	    r.setMinX(n);
	    r.setMaxX(n);
	} else if (direction==Y){
	    r.setMinY(n);
	    r.setMaxY(n);
	} else if (direction==Z){
	    r.setMinZ(n);
	    r.setMaxZ(n);
	}
	return(r);
    }
    public SortedSet headSet(boolean inclusive,double to){
	Rectangle3d xr;
	if (inclusive){
	    xr=setRect(pos1,to);
	} else {
	    xr=setRect(neg1,to);
	}
	return (headSet(xr));
    }
    public SortedSet tailSet(boolean inclusive,double from){
	Rectangle3d xr;
	if (inclusive){
	    xr=setRect(neg2,from);
	} else {
	    xr=setRect(pos2,from);
	}
	return (tailSet(xr));
    }
    public SortedSet subSet(boolean mininclusive, boolean maxinclusive,double min, double max){
	Rectangle3d nr,xr;
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




