package edu.columbia.cs.cgui.spam3d;

import java.io.*;

public class Rectangle3d implements Cloneable,Serializable {
    private double minx, miny, minz, maxx, maxy,maxz;
    private static final long serialVersionUID=-1524058923486558856L;
    public int hashCode(){
	return (new Double(minx + (2*maxx) + (4*miny) + (8*maxy) + (16*minz) + (32*maxz)).hashCode());
    }
    static public Rectangle3d consensus(Rectangle3d r1, Rectangle3d r2, int dimension ){
	/* if dimension = 2, rectangles are adjacent on back and front
	   if dimension = 1, rectangles are adjacent on top and bottom,
	   if dimension = 0, rectangles are adjacent on left and right */
	Rectangle3d ret=null;
	if (dimension==2){
	    ret=new Rectangle3d(Math.max(r1.getMinX(),r2.getMinX()),
				Math.max(r1.getMinY(),r2.getMinY()),
				Math.min(r1.getMinZ(),r2.getMinZ()),
				Math.min(r1.getMaxX(),r2.getMaxX()),
				Math.min(r1.getMaxY(),r2.getMaxY()),
				Math.max(r1.getMaxZ(),r2.getMaxZ()));
	} else if (dimension==1){
	    ret=new Rectangle3d(Math.max(r1.getMinX(),r2.getMinX()),
				Math.min(r1.getMinY(),r2.getMinY()),
				Math.max(r1.getMinZ(),r2.getMinZ()),
				Math.min(r1.getMaxX(),r2.getMaxX()),
				Math.max(r1.getMaxY(),r2.getMaxY()),
				Math.min(r1.getMaxZ(),r2.getMaxZ()));
	} else if (dimension==0){
	    ret=new Rectangle3d(Math.min(r1.getMinX(),r2.getMinX()),
				Math.max(r1.getMinY(),r2.getMinY()),
				Math.max(r1.getMinZ(),r2.getMinZ()),
				Math.max(r1.getMaxX(),r2.getMaxX()),
				Math.min(r1.getMaxY(),r2.getMaxY()),
				Math.min(r1.getMaxZ(),r2.getMaxZ()));
	}
	if (ret.area()<=0.){
	    System.out.println("consensus: r1=" + r1 + "\tr2=" + r2 + "\tret=" + ret);
	    Thread.currentThread().dumpStack();
	    System.exit(-1);
	}
	return (ret);
    }
    public boolean equals(Object obj){
	if (!(obj instanceof Rectangle3d)){
	    return false;
	}
	Rectangle3d comp=(Rectangle3d)obj;
	if (comp.getMinX()==minx && comp.getMaxX()==maxx && 
	    comp.getMinY()==miny && comp.getMaxY()==maxy &&
	    comp.getMinZ()==minz && comp.getMaxZ()==maxz){
	    return (true);
	}
	return(false);
    }
    public boolean enclosedBy(Rectangle3d r){
	if (r.getMinX()<=minx && r.getMaxX()>=maxx && r.getMinY()<=miny && r.getMaxY()>=maxy
	    && r.getMinZ()<=minz && r.getMaxZ()>=maxz){
	    return (true);
	} else {
	    return (false);
	}
    }
    protected Object clone(){
	Rectangle3d clone = null;
	try {
	    clone = (Rectangle3d) super.clone();
	} catch (CloneNotSupportedException e) { 
	    throw new InternalError();
	}
	clone.minx = minx;
	clone.maxx = maxx;
	clone.miny = miny;
	clone.maxy = maxy;
	clone.minz = minz;
	clone.maxz = maxz;
	return (clone);
    }
    public Rectangle3d(double mx, double my, double mz, double xx, double xy, double xz){
	minx = (double)mx; miny =(double) my; minz =(double) mz; 
	maxx = (double)xx; maxy =(double) xy; maxz =(double) xz; 
    }
    public String toString(){
	return ("(" + minx + "," + miny + "," + minz + ")-(" + maxx + "," + maxy + "," + maxz + ")" );
    }
    public void print(PrintStream ps){
	ps.print(toString());
    }
    public void setFrameFromDiagonal(double x1, double y1, double z1,
				     double x2, double y2, double z2) {
	if (x2 < x1) { double t = x1; x1 = x2; x2 = t; }
	if (y2 < y1) { double t = y1; y1 = y2; y2 = t; }
	if (z2 < z1) { double t = z1; z1 = z2; z2 = t; }
	setRect(x1, y1, z1, x2, y2, z2);
    }
    
    public double area(){
	double x,y,z;
	x = maxx-minx;
	y = maxy-miny;
	z = maxz-minz;
	if (x < 0.0 || y < 0.0 || z<0.0){
	    return (-Math.abs(x*y));
	} else {
	    return(Math.abs(x*y));
	}
    }
    public Rectangle3d createIntersection(Rectangle3d r) {
	Rectangle3d retRect = new Rectangle3d(Math.max(r.minx,minx),Math.max(r.miny,miny),Math.max(r.minz,minz),
					      Math.min(r.maxx,maxx),Math.min(r.maxy,maxy),Math.min(r.maxz,maxz));
	return retRect;
    }
    public void add(double newx, double newy, double newz) {
	double x1 = Math.min(getMinX(), newx);
	double x2 = Math.max(getMaxX(), newx);
	double y1 = Math.min(getMinY(), newy);
	double y2 = Math.max(getMaxY(), newy);
	double z1 = Math.min(getMinZ(), newz);
	double z2 = Math.max(getMaxZ(), newz);
	setRect(x1, y1, z1, x2, y2, z2);
    }
    public void setRect(double mx, double my, double mz, double xx, double xy, double xz){
	minx = (double)mx; miny = (double)my; minz = (double) mz;
	maxx = (double)xx; maxy = (double)xy; maxz = (double) xz;
    }
    public Rectangle3d(){
	minx = miny = minz = maxx = maxy = maxz = 0.0f;
    }
    public void setRect(Rectangle3d r){ 
	minx = r.minx; miny = r.miny; minz = r.minz; 
	maxx = r.maxx; maxy = r.maxy; maxz = r.maxz;
    }
    public Rectangle3d(Rectangle3d r){ 
	minx = r.minx; miny = r.miny; minz = r.minz;
	maxx = r.maxx; maxy = r.maxy; maxz = r.maxz;
    }

    public static double getCenterDistanceBetween(Rectangle3d rect1, Rectangle3d rect2){
	double diffx=rect1.getCenterX()-rect2.getCenterX();
	double diffy=rect1.getCenterY()-rect2.getCenterY();
	double diffz=rect1.getCenterZ()-rect2.getCenterZ();
	return (Math.sqrt(diffx*diffx+diffy*diffy+diffz*diffz));
    }
    public static double getDistanceBetween(Rectangle3d rect1, Rectangle3d rect2){
	return (getDistanceBetween(rect1,rect2,new Double(0.)));
    }
    public static double getDistanceBetween(Rectangle3d rect1, Rectangle3d rect2,Double offset){
	boolean left,right,top,bottom,front,back;
	left = rect1.getMaxX()<rect2.getMinX();
	right = rect1.getMinX()>rect2.getMaxX();
	bottom = rect1.getMaxY()<rect2.getMinY();
	top = rect1.getMinY()>rect2.getMaxY();
	front = rect1.getMaxZ()<rect2.getMinZ();
	back = rect1.getMinZ()>rect2.getMaxZ();
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
    public static boolean intersect(Rectangle3d src1,
				 Rectangle3d src2,
				 Rectangle3d dest) {
	double x1 = Math.max(src1.getMinX(), src2.getMinX());
	double y1 = Math.max(src1.getMinY(), src2.getMinY());
	double z1 = Math.max(src1.getMinZ(), src2.getMinZ());
	double x2 = Math.min(src1.getMaxX(), src2.getMaxX());
	double y2 = Math.min(src1.getMaxY(), src2.getMaxY());
	double z2 = Math.min(src1.getMaxZ(), src2.getMaxZ());
	dest.setRect(x1,y1,z1,x2,y2,z2);
	return (dest.getWidth()>0. && dest.getHeight()>0. && dest.getDepth()>0.);
    }
    
    public double getX(){ return minx; }
    public double getY(){ return miny; }
    public double getZ(){ return minz; }
    public double getWidth(){ return maxx-minx; }
    public double getHeight(){ return maxy-miny; }
    public double getDepth(){ return maxz-minz; }
    public double getCenterX(){ return getX() + (getWidth()/2.); }
    public double getCenterY(){ return getY() + (getHeight()/2.); }
    public double getCenterZ(){ return getZ() + (getDepth()/2.); }
    public double getMinX(){ return minx; }
    public double getMinY(){ return miny; }
    public double getMinZ(){ return minz; }
    public double getMaxX(){ return maxx; }
    public double getMaxY(){ return maxy; }
    public double getMaxZ(){ return maxz; }
    public void setMinX(double v){ minx = (double)v; }
    public void setMinY(double v){ miny = (double)v; }
    public void setMinZ(double v){ minz = (double)v; }
    public void setMaxX(double v){ maxx = (double)v; }
    public void setMaxY(double v){ maxy = (double)v; }
    public void setMaxZ(double v){ maxz = (double)v; }
}
