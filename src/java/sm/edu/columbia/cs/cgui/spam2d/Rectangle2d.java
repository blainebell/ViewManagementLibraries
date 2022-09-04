package edu.columbia.cs.cgui.spam2d;

import java.io.*;

public class Rectangle2d implements Cloneable,Serializable {
    private double minx, miny, maxx, maxy;
    private static final long serialVersionUID=-1524058536629658856L;
    static public Rectangle2d consensus(Rectangle2d r1, Rectangle2d r2, int dimension ){
	/* if dimension = 1, rectangles are adjacent on top and bottom,
	   if dimension = 0, rectangles are adjacent on left and right */
	Rectangle2d ret=null;
	if (dimension==1){
	    ret=new Rectangle2d(Math.max(r1.getMinX(),r2.getMinX()),
				Math.min(r1.getMinY(),r2.getMinY()),
				Math.min(r1.getMaxX(),r2.getMaxX()),
				Math.max(r1.getMaxY(),r2.getMaxY()));
	} else if (dimension==0){
	    ret=new Rectangle2d(Math.min(r1.getMinX(),r2.getMinX()),
				Math.max(r1.getMinY(),r2.getMinY()),
				Math.max(r1.getMaxX(),r2.getMaxX()),
				Math.min(r1.getMaxY(),r2.getMaxY()));
	}
	if (ret.area()<=0.){
	    System.out.println("consensus: r1=" + r1 + "\tr2=" + r2 + "\tret=" + ret);
	    Thread.currentThread().dumpStack();
	    System.exit(-1);
	}
	return (ret);
    }
    public boolean equals(Object obj){
	if (!(obj instanceof Rectangle2d)){
	    return false;
	}
	Rectangle2d comp=(Rectangle2d)obj;
	if (comp.getMinX()==minx && comp.getMaxX()==maxx && 
	    comp.getMinY()==miny && comp.getMaxY()==maxy){
	    return (true);
	}
	return(false);
    }
    public void move(double x,double y){
	setRect(getMinX()+x,getMinY()+y, getMaxX()+x, getMaxY()+y);
    }
    public boolean enclosedBy(Rectangle2d r){
	if (r.getMinX()<=minx && r.getMaxX()>=maxx && r.getMinY()<=miny && r.getMaxY()>=maxy){
	    return (true);
	} else {
	    return (false);
	}
    }
    protected Object clone(){
	Rectangle2d clone = null;
	try {
	    clone = (Rectangle2d) super.clone();
	} catch (CloneNotSupportedException e) { 
	    throw new InternalError();
	}
	clone.minx = minx;
	clone.maxx = maxx;
	clone.miny = miny;
	clone.maxy = maxy;
	return (clone);
    }
    public Rectangle2d(double mx, double my, double xx, double xy){
	minx = (double)mx; miny =(double) my; maxx = (double)xx; maxy =(double) xy;
    }
    public String toString(){
	return ("(" + minx + "," + miny + ")-(" + maxx + "," + maxy + ")");
    }
    public void print(PrintStream ps){
	ps.print(toString());
    }
    public void setFrameFromDiagonal(double x1, double y1,
				     double x2, double y2) {
	if (x2 < x1) { double t = x1; x1 = x2; x2 = t; }
	if (y2 < y1) { double t = y1; y1 = y2; y2 = t; }
	setRect(x1, y1, x2, y2);
    }
    
    public double area(){
	double x,y;
	x = maxx-minx;
	y = maxy-miny;
	if (x < 0.0 || y < 0.0){
	    return (-Math.abs(x*y));
	} else {
	    return(Math.abs(x*y));
	}
    }
    public Rectangle2d createIntersection(Rectangle2d r) {
	Rectangle2d retRect = new Rectangle2d(Math.max(r.minx,minx),Math.max(r.miny,miny),
					      Math.min(r.maxx,maxx),Math.min(r.maxy,maxy));
	return retRect;
    }
    public void add(double newx, double newy) {
	double x1 = Math.min(getMinX(), newx);
	double x2 = Math.max(getMaxX(), newx);
	double y1 = Math.min(getMinY(), newy);
	double y2 = Math.max(getMaxY(), newy);
	setRect(x1, y1, x2, y2);
    }
    public void setRect(double mx, double my, double xx, double xy){
	minx = (double)mx; miny = (double)my; maxx = (double)xx; maxy = (double)xy;
    }
    public Rectangle2d(){
	minx = miny = maxx = maxy = 0.0f;
    }
    public void setRect(Rectangle2d r){ 
	minx = r.minx; miny = r.miny; 
	maxx = r.maxx; maxy = r.maxy;
    }
    public Rectangle2d(Rectangle2d r){ 
	minx = r.minx; miny = r.miny; 
	maxx = r.maxx; maxy = r.maxy;
    }

    public static double getCenterDistanceBetween(Rectangle2d rect1, Rectangle2d rect2){
	double diffx=rect1.getCenterX()-rect2.getCenterX();
	double diffy=rect1.getCenterY()-rect2.getCenterY();
	return (Math.sqrt(diffx*diffx+diffy*diffy));
    }
    public static double getDistanceBetween(Rectangle2d rect1, Rectangle2d rect2){
	return (getDistanceBetween(rect1,rect2,new Double(0.)));
    }
    public static double getDistanceBetween(Rectangle2d rect1, Rectangle2d rect2,Double offset){
	boolean left,right,top,bottom;
	left = rect1.getMaxX()<rect2.getMinX();
	right = rect1.getMinX()>rect2.getMaxX();
	bottom = rect1.getMaxY()<rect2.getMinY();
	top = rect1.getMinY()>rect2.getMaxY();
	if (!left && !right && !top && !bottom){
	    return (0.);
	}
	if (!left && !right){
	    if (top){
		return (rect1.getMinY()-rect2.getMaxY());
	    } else {
		return (rect2.getMinY()-rect1.getMaxY());
	    }
	}
	if (!bottom && !top){
	    if (right){
		return (rect1.getMinX()-rect2.getMaxX());
	    } else {
		return (rect2.getMinX()-rect1.getMaxX());
	    }
	}
	double bv[][]=new double[2][2];
	if (top){
	    bv[0][1]=rect1.getMinY();
	    bv[1][1]=rect2.getMaxY();
	} else {
	    bv[0][1]=rect1.getMaxY();
	    bv[1][1]=rect2.getMinY();
	}
	if (right){
	    bv[0][0]=rect1.getMinX();
	    bv[1][0]=rect2.getMaxX();
	}else {
	    bv[0][0]=rect1.getMaxX();
	    bv[1][0]=rect2.getMinX();
	}
	return (Math.pow(Math.pow(bv[1][0]-bv[0][0],2.) + Math.pow(bv[1][1]-bv[0][1],2.),.5));
    }
    public static boolean intersect(Rectangle2d src1,
				 Rectangle2d src2,
				 Rectangle2d dest) {
	double x1 = Math.max(src1.getMinX(), src2.getMinX());
	double y1 = Math.max(src1.getMinY(), src2.getMinY());
	double x2 = Math.min(src1.getMaxX(), src2.getMaxX());
	double y2 = Math.min(src1.getMaxY(), src2.getMaxY());
	dest.setRect(x1,y1,x2,y2);
	return (dest.getWidth()>0. && dest.getHeight()>0.);
    }
    
    public double getX(){ return minx; }
    public double getY(){ return miny; }
    public double getWidth(){ return maxx-minx; }
    public double getHeight(){ return maxy-miny; }
    public double getCenterX(){ return getX() + (getWidth()/2.); }
    public double getCenterY(){ return getY() + (getHeight()/2.); }
    public double getMinX(){ return minx; }
    public double getMinY(){ return miny; }
    public double getMaxX(){ return maxx; }
    public double getMaxY(){ return maxy; }
    public void setMinX(double v){ minx = (double)v; }
    public void setMinY(double v){ miny = (double)v; }
    public void setMaxX(double v){ maxx = (double)v; }
    public void setMaxY(double v){ maxy = (double)v; }
}
