package edu.columbia.cs.cgui.spam2i;

import java.io.*;

public class Rectangle2i implements Cloneable,Serializable {
    private int minx, miny, maxx, maxy;
    private static final long serialVersionUID=-1524053421293129401L;
    static public Rectangle2i consensus(Rectangle2i r1, Rectangle2i r2, int dimension ){
	/* if dimension = 1, rectangles are adjacent on top and bottom,
	   if dimension = 0, rectangles are adjacent on left and right */
	Rectangle2i ret=null;
	if (dimension==1){
	    ret=new Rectangle2i(Math.max(r1.getMinX(),r2.getMinX()),
				Math.min(r1.getMinY(),r2.getMinY()),
				Math.min(r1.getMaxX(),r2.getMaxX()),
				Math.max(r1.getMaxY(),r2.getMaxY()));
	} else if (dimension==0){
	    ret=new Rectangle2i(Math.min(r1.getMinX(),r2.getMinX()),
				Math.max(r1.getMinY(),r2.getMinY()),
				Math.max(r1.getMaxX(),r2.getMaxX()),
				Math.min(r1.getMaxY(),r2.getMaxY()));
	}
	if (ret.area()<=0){
	    System.out.println("consensus: r1=" + r1 + "\tr2=" + r2 + "\tret=" + ret);
	    Thread.currentThread().dumpStack();
	    System.exit(-1);
	}
	return (ret);
    }
    public boolean equals(Object obj){
	if (!(obj instanceof Rectangle2i)){
	    return false;
	}
	Rectangle2i comp=(Rectangle2i)obj;
	if (comp.getMinX()==minx && comp.getMaxX()==maxx && 
	    comp.getMinY()==miny && comp.getMaxY()==maxy){
	    return (true);
	}
	return(false);
    }
  public void grow(int x, int y){
    setRect(getMinX()-x, getMinY()-y, getMaxX()+x, getMaxY()+y);
  }
  public void clip(Rectangle2i crect){
    setRect(Math.max(getMinX(), crect.getMinX()),
	    Math.max(getMinY(), crect.getMinY()),
	    Math.min(getMaxX(), crect.getMaxX()),
	    Math.min(getMaxY(), crect.getMaxY()));
  }

  /** set size around center in both x and y */
  public void setSize(int w, int h){
    double cx = getCenterX(), cy = getCenterY();
    int hw = w/2, hh = h/2, mx, my;
    mx =(int)Math.round(cx-hw);
    my =(int)Math.round(cy-hh);
    setRect(mx, my, mx+w, my+h);
  }
  /** scales around center in both x and y */
  public void scale(double sclx, double scly){
    int w = getWidth(), h = getHeight(),x,y;
    double cx = getCenterX(), cy = getCenterY();
    w = (int)Math.round(w*sclx);
    h = (int)Math.round(h*scly);
    x = (int)(cx - w/2);
    y = (int)(cy - h/2);
    setRect(x,y,x+w, y+h);
  }

    public void move(int x,int y){
	setRect(getMinX()+x,getMinY()+y, getMaxX()+x, getMaxY()+y);
    }
    public boolean enclosesPoint(int x, int y){
      return (minx <= x && maxx >=x && miny <= y && maxy >= y);
    }
    public boolean enclosedBy(Rectangle2i r){
	if (r.getMinX()<=minx && r.getMaxX()>=maxx && r.getMinY()<=miny && r.getMaxY()>=maxy){
	    return (true);
	} else {
	    return (false);
	}
    }
    protected Object clone(){
	Rectangle2i clone = null;
	try {
	    clone = (Rectangle2i) super.clone();
	} catch (CloneNotSupportedException e) { 
	    throw new InternalError();
	}
	clone.minx = minx;
	clone.maxx = maxx;
	clone.miny = miny;
	clone.maxy = maxy;
	return (clone);
    }
    public Rectangle2i(int mx, int my, int xx, int xy){
	minx = (int)mx; miny =(int) my; maxx = (int)xx; maxy =(int) xy;
    }
    public String toString(){
	return ("(" + minx + "," + miny + ")-(" + maxx + "," + maxy + ")");
    }
    public void print(PrintStream ps){
	ps.print(toString());
    }
    public void setFrameFromDiagonal(int x1, int y1,
				     int x2, int y2) {
	if (x2 < x1) { int t = x1; x1 = x2; x2 = t; }
	if (y2 < y1) { int t = y1; y1 = y2; y2 = t; }
	setRect(x1, y1, x2, y2);
    }
    
    public int area(){
	int x,y;
	x = maxx-minx;
	y = maxy-miny;
	if (x < 0.0 || y < 0.0){
	    return (-Math.abs(x*y));
	} else {
	    return(Math.abs(x*y));
	}
    }
    public Rectangle2i createIntersection(Rectangle2i r) {
	Rectangle2i retRect = new Rectangle2i(Math.max(r.minx,minx),Math.max(r.miny,miny),
					      Math.min(r.maxx,maxx),Math.min(r.maxy,maxy));
	return retRect;
    }
    public void add(int newx, int newy) {
	int x1 = Math.min(getMinX(), newx);
	int x2 = Math.max(getMaxX(), newx);
	int y1 = Math.min(getMinY(), newy);
	int y2 = Math.max(getMaxY(), newy);
	setRect(x1, y1, x2, y2);
    }
    public void setRect(int mx, int my, int xx, int xy){
	minx = (int)mx; miny = (int)my; maxx = (int)xx; maxy = (int)xy;
    }
    public Rectangle2i(){
	minx = miny = maxx = maxy = 0;
    }
    public void setRect(Rectangle2i r){ 
	minx = r.minx; miny = r.miny; 
	maxx = r.maxx; maxy = r.maxy;
    }
    public Rectangle2i(Rectangle2i r){ 
	minx = r.minx; miny = r.miny; 
	maxx = r.maxx; maxy = r.maxy;
    }

    public static double getCenterDistanceBetween(Rectangle2i rect1, Rectangle2i rect2){
	double diffx=rect1.getCenterX()-rect2.getCenterX();
	double diffy=rect1.getCenterY()-rect2.getCenterY();
	return (Math.sqrt(diffx*diffx+diffy*diffy));
    }
    public static double getDistanceBetween(Rectangle2i rect1, Rectangle2i rect2){
	return (getDistanceBetween(rect1,rect2,new Double(0.)));
    }
    public static double getDistanceBetween(Rectangle2i rect1, Rectangle2i rect2,Double offset){
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
    public static boolean intersect(Rectangle2i src1,
				 Rectangle2i src2,
				 Rectangle2i dest) {
      int x1 = Math.max(src1.getMinX(), src2.getMinX());
      int y1 = Math.max(src1.getMinY(), src2.getMinY());
      int x2 = Math.min(src1.getMaxX(), src2.getMaxX());
      int y2 = Math.min(src1.getMaxY(), src2.getMaxY());
      dest.setRect(x1,y1,x2,y2);
      boolean ret = dest.getWidth()>0. && dest.getHeight()>0.;
      if (ret){
	return (true);
      } else if (src1.area()==0){
	return (src1.enclosedBy(dest));
      } else if (src2.area()==0){
	return (src2.enclosedBy(dest));
      }
      return (false);
    }
    
    public int getX(){ return minx; }
    public int getY(){ return miny; }
    public int getWidth(){ return maxx-minx; }
    public int getHeight(){ return maxy-miny; }
    public double getCenterX(){ return getX() + (getWidth()/2.); }
    public double getCenterY(){ return getY() + (getHeight()/2.); }
    public int getMinX(){ return minx; }
    public int getMinY(){ return miny; }
    public int getMaxX(){ return maxx; }
    public int getMaxY(){ return maxy; }
    public void setMinX(int v){ minx = (int)v; }
    public void setMinY(int v){ miny = (int)v; }
    public void setMaxX(int v){ maxx = (int)v; }
    public void setMaxY(int v){ maxy = (int)v; }
}
