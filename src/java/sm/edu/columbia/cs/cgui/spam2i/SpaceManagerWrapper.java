/* 
 * @(#)SpaceManagerWrapper.java
 *
 */   

package edu.columbia.cs.cgui.spam2i;

import java.awt.geom.*;
import java.util.*;
import java.text.*;
import java.awt.*;
import java.lang.reflect.*;
import java.io.*;

/**
 * This class adds additional functionality including queries, datastructures 
 * to improve performance, undoable-adds, etc. Specifically, the addition of 
 * a Sorted list of empty spaces (binary tree) for better querying:
 * 1) larger area usually (not always, of course)  means more important (queries starting with highest area)
 * 2) after a certain area, queries can stop. (won't need an exhaustive search)
 * 3) logish operations (add/delete)
 * <p>
 * A Hashtable for the full-space objects has also been added.  This is used to
 * make sure each full-space rectangle has a unique id.  If a rectangle is being added
 * with an existing id, then it changes the id so that it is unque (1+maximum id).
 * <p>
 * @author Blaine Bell
 * @since JDK1.1
 *
 */
public class SpaceManagerWrapper extends SpaceManager implements SpaceManager.SpaceManagerListener, Cloneable, Serializable {
  private static final long serialVersionUID = 182019237174517273L;

  /** 
   * spaceTrees: Spaces that are sorted by area 
   */
  TreeSet spaceTrees;
  transient private Hashtable objectHash;
  private long maxLong=-1;

  /**
   * This class keeps track of the changes to the empty-space representation 
   * during an add operation of a full-space object.  This is used to do 
   * "undoable" adds, so that the delete operation does not need to be executed
   * for full-space rectangles that are deleted either right after it has been
   * added, or in an "undo/redo" stack of successive addition/deletions of 
   * full-space rectangles.
   *
   */
  public class RedoAction {
    /**
     * Rectangle that has been added, and will be deleted if the RedoAction occurs
     */
    public FullRectangle2i rectangle;
    /**
     * List of empty-spaces that has been added to the representation during
     * the operation.
     */
    public Vector addedSpaces;
    /**
     * List of empty-spaces that has been deleted to the representation during
     * the operation.
     */
    public Vector removedSpaces;
    /**
     * Constructor for RedoAction. 
     * @param fr the full-space rectangle that has been added.
     */
    RedoAction(FullRectangle2i fr){
      rectangle=fr;
      addedSpaces=new Vector();
      removedSpaces=new Vector();
    }
    /**
     * Constuctor for RedoAction.
     * @param fr the full-space rectangle that has been added.
     * @param added the list of empty-spaces that were added during operation.
     * @param deleted the list of empty-spaces that were deleted during operation.
     */
    RedoAction(FullRectangle2i fr, Collection added, Collection deleted){
      rectangle=fr;
      addedSpaces=new Vector(added);
      removedSpaces=new Vector(deleted);
    }
  }
  /**
   * This action "undos" an add operation to the representation.  It takes the 
   * added empty-spaces and removes them, and adds the empty-spaces that were 
   * deleted. It also removes the full-space.  Warning: If this function gets called
   * and the RedoAction is not "undone" correctly in order, the empty-space 
   * representation will not be correct.  Only if the add operation was the last 
   * operation that occurred, or if a sequence of addition operations have been
   * "undone" in the opposite order they were added.
   *
   * @param ra RedoAction that represents an addition operation that is being undone.
   */
  public int doRedoAction(RedoAction ra){
    return(deleteObjectRectangleDirect(ra.rectangle,  ra.removedSpaces, ra.addedSpaces));
  }
  /**
   * Function provides a callback for the SpaceManager each time empty-spaces are added
   * and deleted from the representation.  The empty-space data-structure gets maintained.
   *
   */
  public void addDeleteSpaceRectangles(FullRectangle2i fr, Collection added, Collection deleted){
    spaceTrees.addAll(added);
    spaceTrees.removeAll(deleted);
  }
  /**
   * Creates a copy of the space manager.  
   *
   * @return the copy
   */
  protected Object clone(){
    return (super.clone());
  }

  void printObjectRectangles(){
    super.printObjectRectangles();
  }
  void printSpaceRectangles(){
    super.printSpaceRectangles();
  }
  /**
   * Constructs a SpaceManagerWrapper that represents a specified area.
   *
   * @param  rs the area in which the Space Manager manages.
   */
  public SpaceManagerWrapper(Rectangle2i rs) throws NoSuchMethodException, ClassNotFoundException {
    super(rs); //spacemanager = new SpaceManager(rs);
    objectHash = new Hashtable();
    spaceTrees = new TreeSet(new AreaComparator());
    spaceTrees.add(new Rectangle2i(rs));
    super.addSpaceManagerListener(this);
  }

  /**
   * Adds a full-space to the Representation. When this method gets
   * called, the empty-space representation gets updated by having a 
   * number of largest empty-space rectangles added and deleted.
   *
   * @param  rs the full-space rectangle specified.
   *
   */
  public FullRectangle2i addRectangle(FullRectangle2i rs){
    if (objectHash.containsKey(rs.id)){
      // same id
      rs.id = new Long (maxLong+1l);
      maxLong++;
    } else if (rs.id.longValue() > maxLong){
      maxLong = rs.id.longValue();
    } 
    objectHash.put(rs.id,rs);
    return (super.addRectangle(rs, rs.id.intValue()));
  }
  /**
   * Delete a full-space from the representation.  When this method gets 
   * called, the empty-space representation gets updated by adding and
   * deleting a number of largest empty-space rectangles to and from it.
   *
   * @param rs the Full-space rectangle being deleted
   *
   */
  public int deleteRectangle(FullRectangle2i rs){
    if (!objectHash.containsKey(rs.id)){
      // rectangle is not in manager
      return (-1);
    } else {
      objectHash.remove(rs.id);
    }
    return (super.deleteRectangle(rs));
  }

  /**
   * Gets number of largest-empty spaces.  
   * 
   * @param justinside  specifies false-All true-ones that do not touch the edges of the area of the SpaceManager.
   */
  public int getNumberSpaces(boolean justinside){
    if (!justinside){
      return (spaceTrees.size());
    } else {
      int numInside=0;
      Iterator i = spaceTrees.iterator();
      Rectangle2i ss = super.getScreenSpace();
      while (i.hasNext()){
	Rectangle2i rect = (Rectangle2i)i.next();
	if (rect.getMinX()==ss.getMinX() || rect.getMinY()==ss.getMinY() ||
	    rect.getMaxX()==ss.getMaxX() || rect.getMaxY()==ss.getMaxY()){
	  continue;
	}
	numInside++;
      }
      return (numInside);
    }
  }
  // --- Serialization ------------------------------------
  
  /** 
   * 
   * Serializes the SpaceManagerWrapper.
   * 
   */
  private void writeObject(java.io.ObjectOutputStream out) throws IOException {
    out.defaultWriteObject();
    out.writeObject(objectHash);
  }
  /** 
   * 
   * Reads a serialized SpaceManagerWrapper.
   * 
   */
  private void readObject(java.io.ObjectInputStream in)
    throws IOException, ClassNotFoundException {
    in.defaultReadObject();
    objectHash = (Hashtable) in.readObject();
    for (Enumeration enum=objectHash.elements(); enum.hasMoreElements();){
      FullRectangle2i fs=(FullRectangle2i)enum.nextElement();
      if (fs.id.longValue()>maxLong){
	maxLong=fs.id.longValue();
      }
    }
  }

  /*
   * QUERIES
   *
   */
  /**
   * Returns all of the empty-space rectangles sorted by Area.
   *
   */
  public Iterator getSpacesOrderedByArea(){
    return (spaceTrees.iterator());
  }
  /**
   * Returns the Largest-Empty Space with the largest area.
   *
   */
  public Rectangle2i getLargestSpace(){
    return(new Rectangle2i((Rectangle2i)spaceTrees.first()));
  }

  /**
   *
   * returns the closest empty-space from specified rectangle.
   * @param rect rectangle specified
   */
  public Rectangle2i getClosestSpace(Rectangle2i rect){
    Iterator i=getSpacesOrderedByArea();
    Rectangle2i closest=null,tmpRect=null;
    double currentDistance=Integer.MAX_VALUE,tmpCD=0.;
    if (i.hasNext()){
      tmpRect = (Rectangle2i)i.next();
      currentDistance=Rectangle2i.getDistanceBetween(tmpRect,rect);
      closest=tmpRect;
    }
    while (i.hasNext()){
      tmpRect = (Rectangle2i)i.next();
      if ((tmpCD=Rectangle2i.getDistanceBetween(tmpRect,rect))<currentDistance ||
	  (currentDistance==tmpCD && closest.area()<tmpRect.area())){
	currentDistance=tmpCD;
	closest = tmpRect;
      }
    }
    return (new Rectangle2i(closest));
  }
  
  /**
   * Returns the largest rectangle that lies within the empty-space with
   * a given proportions.
   *  @param prop specified dimensions
   */
  public Rectangle2i getLargestProportion(Dimension prop){
    Iterator i=getSpacesOrderedByArea();
    Rectangle2i largestprop=null,tmpRect=null;
    double currentDistance=Integer.MAX_VALUE,tmpCD=0.;
    double currentScale=0.;
    while (i.hasNext()){
      tmpRect = (Rectangle2i)i.next();
      if (largestprop !=null && tmpRect.area()<largestprop.area()){
	break;
      }
      if ((largestprop==null) ||
	  (tmpRect.getWidth()>=prop.getWidth()*currentScale && 
	   tmpRect.getHeight()>=prop.getHeight()*currentScale)){
	double xscale,yscale;
	xscale=tmpRect.getWidth()/prop.getWidth();
	yscale=tmpRect.getHeight()/prop.getHeight();
	currentScale=Math.min(xscale,yscale);
	largestprop=tmpRect;
      }
    }
    return (largestprop);
  }

  /** 
   * This query is used for finding the closest rectangle for a popup object to a given:
   * screen object: screenRect and its last position: prevDest given its minimum and maximum
   * width and height it can have (minx, miny, maxx, maxy), its aspect ratio (ratioxtoy)
   * and the buffer between the screen object (buffer)
   */
  public Rectangle2i getClosestWithAtleastSizeAspectRatioClosestToBoth(Rectangle2i screenRect, Rectangle2i prevDest, int minx, int miny, int maxx, int maxy, double ratioxtoy,int buffer){
    Iterator i=getSpacesOrderedByArea();
    Rectangle2i closest=null,tmpRect=null;
    double minarea=minx * miny;
    double currentDistance=Integer.MAX_VALUE,tmpCD=0.,currentOverlap=0.;
    Vector closestV=new Vector();
    Rectangle2i result=new Rectangle2i();
    while (i.hasNext()){
      tmpRect = new Rectangle2i ((Rectangle2i)i.next());

      tmpRect.setMinX(tmpRect.getMinX()+buffer);
      tmpRect.setMaxX(tmpRect.getMaxX()-buffer);
      tmpRect.setMinY(tmpRect.getMinY()+buffer);
      tmpRect.setMaxY(tmpRect.getMaxY()-buffer);
      if (tmpRect.getWidth()>=minx && tmpRect.getHeight()>=miny){
	if ((tmpCD=Rectangle2i.getDistanceBetween(tmpRect,prevDest))<currentDistance){
	  currentDistance=tmpCD;
	  closest = tmpRect;
	  closestV.removeAllElements();
	  closestV.add(tmpRect);
	  if (tmpCD==0.){
	    if (Rectangle2i.intersect(prevDest,tmpRect,result)){
	      currentOverlap = result.area();
	    }
	  }
	} else if (currentDistance==0. && tmpCD==0.){
	  double compIntersection = 0.;
	  if (Rectangle2i.intersect(prevDest,tmpRect,result)){
	    compIntersection = result.area();
	    /* If area overlapped is greater than rectangle found before */
	    if (compIntersection>currentOverlap){
	      closest = tmpRect;
	      closestV.removeAllElements();
	      closestV.add(tmpRect);
	      currentOverlap=compIntersection;
	    }
	  }
	} else if (tmpCD==currentDistance){
	  closestV.add(tmpRect);
	}
      }
    }
    Rectangle2i returnRect = null;
    for (Enumeration en=closestV.elements();en.hasMoreElements();){
      Rectangle2i recRes=(Rectangle2i)en.nextElement();
      int ratiox, ratioy; // what ratio output should be
      ratiox = (int)Math.min(recRes.getWidth(),maxx);
      ratioy = (int)Math.min(recRes.getHeight(),maxy);
      if (ratioxtoy * ratioy > ratiox){
	// ratiox is used to calc ratioy
	ratioy = (int)Math.round(ratiox / ratioxtoy);
      } else {
	// ratioy is used to calc ratiox
	ratiox = (int)Math.round(ratioy * ratioxtoy);
      }
      if (returnRect==null || ratiox*ratioy>returnRect.area()){
	returnRect = recRes;
      }

      if (recRes.getMaxX()<=prevDest.getMinX()){ // left of rectangle
	recRes.setMinX(recRes.getMaxX()-ratiox);
      } else if (recRes.getMinX()>=prevDest.getMaxX()){ // right of rectangle
	recRes.setMaxX(recRes.getMinX()+ratiox);
      } else { // overlap in the X direction
	int pL = ((int)prevDest.getCenterX())-(ratiox/2);
	int pR = pL+ratiox;
	if (recRes.getMinX()<= pL && recRes.getMaxX() >= pR){
	  /* if the rectangle is completely within space in this dimension */
	  recRes.setMinX(((int)prevDest.getCenterX())-(ratiox/2));
	  recRes.setMaxX(recRes.getMinX()+ratiox);
	} else {
	  if (recRes.getMinX()<= (prevDest.getCenterX()-ratiox/2)){
	    recRes.setMinX(recRes.getMaxX()-ratiox); // right of rectangle, but overlap
	  } else {
	    recRes.setMaxX(recRes.getMinX()+ratiox);  // left of rectangle, but overlap
	  }
	}
      }
      
      if (recRes.getMaxY()<=prevDest.getMinY()){ // bottom of rectangle
	recRes.setMinY(recRes.getMaxY()-ratioy);
      } else if (recRes.getMinY()>=prevDest.getMaxY()){
	recRes.setMaxY(recRes.getMinY()+ratioy);
      } else { // overlap in the Y direction
	int pB = ((int)prevDest.getCenterY())-(ratioy/2);
	int pT = pB+ratioy;
	if (recRes.getMinY()<= pB && recRes.getMaxY() >= pT){
	  recRes.setMinY(((int)prevDest.getCenterY())-(ratioy/2));
	  recRes.setMaxY(recRes.getMinY()+ratioy);
	} else {
	  if (recRes.getMinY()<= (prevDest.getCenterY()-ratioy/2)){
	    recRes.setMinY(recRes.getMaxY()-ratioy); // bottom of rectangle
	  } else {
	    recRes.setMaxY(recRes.getMinY()+ratioy);  // top of rectangle
	  }
	}
      }
      
    }
    return (returnRect);
  }

  /**
   * Returns the closest rectangle from specified rectangle with given dimensions. 
   * Note: There can be a tie fore closest rectangle, in this case we take the first
   * one we come across, meaning the one that is enclosed with the empty-space that 
   * has the largest area. In this case, distance is measured by computing the distance
   * between the closest points that lie within the two rectangles.
   * @param rect specified rectangle
   * @param dw specified height of returned rectangle
   * @param dw specified width of returned rectangle
   */
  public Rectangle2i getClosestWithExactDimension(Rectangle2i rect,int dw, int dh){
    return (getClosestWithExactDimension(rect,dw,dh,0)); // buffer = 0 ;; default
  }
  public Rectangle2i getClosestWithExactDimension(Rectangle2i rect,int dw, int dh,int buffer){
    Vector listOfRects=getClosestWithAtLeastDimension(rect,dw+2*buffer,dh+2*buffer);
    Rectangle2i resCWALD;
    Rectangle2i retRect=null;
    double tmpD=0.;
    if (listOfRects.isEmpty()){
      return (null);
    }
    double distanceBetCenters=Integer.MAX_VALUE;
    for (Enumeration e=listOfRects.elements(); e.hasMoreElements();){
      resCWALD=(Rectangle2i)e.nextElement();
      Rectangle2i recRes=new Rectangle2i(resCWALD);
      recRes.setMinX(recRes.getMinX()+buffer);
      recRes.setMaxX(recRes.getMaxX()-buffer);
      recRes.setMinY(recRes.getMinY()+buffer);
      recRes.setMaxY(recRes.getMaxY()-buffer);
      if (recRes.getMaxX()<=rect.getMinX()){ // left of rectangle
	recRes.setMinX(recRes.getMaxX()-dw);
      } else if (recRes.getMinX()>=rect.getMaxX()){ // right of rectangle
	recRes.setMaxX(recRes.getMinX()+dw);
      } else { // overlap in the X direction
	int pL = ((int)rect.getCenterX())-(dw/2);
	int pR = pL+dw;
	if (recRes.getMinX()<= pL && recRes.getMaxX() >= pR){
	  /* if the rectangle is completely within space in this dimension */
	  recRes.setMinX(((int)rect.getCenterX())-(dw/2));
	  recRes.setMaxX(recRes.getMinX()+dw);
	} else {
	  if (recRes.getMinX()<= (rect.getCenterX()-dw/2)){
	    recRes.setMinX(recRes.getMaxX()-dw); // right of rectangle, but overlap
	  } else {
	    recRes.setMaxX(recRes.getMinX()+dw);  // left of rectangle, but overlap
	  }
	}
      }
      
      if (recRes.getMaxY()<=rect.getMinY()){ // bottom of rectangle
	recRes.setMinY(recRes.getMaxY()-dh);
      } else if (recRes.getMinY()>=rect.getMaxY()){
	recRes.setMaxY(recRes.getMinY()+dh);
      } else { // overlap in the Y direction
	int pB = ((int)rect.getCenterY())-(dh/2);
	int pT = pB+dh;
	if (recRes.getMinY()<= pB && recRes.getMaxY() >= pT){
	  recRes.setMinY(((int)rect.getCenterY())-(dh/2));
	  recRes.setMaxY(recRes.getMinY()+dh);
	} else {
	  if (recRes.getMinY()<= (rect.getCenterY()-dh/2)){
	    recRes.setMinY(recRes.getMaxY()-dh); // bottom of rectangle
	  } else {
	    recRes.setMaxY(recRes.getMinY()+dh);  // top of rectangle
	  }
	}
      }
      tmpD=Rectangle2i.getCenterDistanceBetween(recRes,rect);
      if (tmpD<distanceBetCenters){
	distanceBetCenters=tmpD;
	retRect=recRes;
      }
    }
    return (retRect);
  }

  public Rectangle2i getClosestWithExactDimensionWithTieBreak(Rectangle2i rect, Rectangle2i tie, int dw, int dh){
    return (getClosestWithExactDimensionTieBreak(rect,tie,dw,dh,0)); // buffer = 0 ;; default
  }

  public Rectangle2i getClosestWithExactDimensionTieBreak(Rectangle2i rect,Rectangle2i tie, int dw, int dh,int buffer){
    Vector listOfRects=getClosestWithAtLeastDimension(rect,dw+2*buffer,dh+2*buffer);
    Rectangle2i resCWALD;
    Rectangle2i retRect=null;
    double tmpD=0.;
    if (listOfRects.isEmpty()){
      return (null);
    }
    double distanceBetCenters=Integer.MAX_VALUE;
    for (Enumeration e=listOfRects.elements(); e.hasMoreElements();){
      resCWALD=(Rectangle2i)e.nextElement();
      Rectangle2i recRes=new Rectangle2i(resCWALD);
      recRes.setMinX(recRes.getMinX()+buffer);
      recRes.setMaxX(recRes.getMaxX()-buffer);
      recRes.setMinY(recRes.getMinY()+buffer);
      recRes.setMaxY(recRes.getMaxY()-buffer);
      if (recRes.getMaxX()<=rect.getMinX()){ // left of rectangle
	recRes.setMinX(recRes.getMaxX()-dw);
      } else if (recRes.getMinX()>=rect.getMaxX()){ // right of rectangle
	recRes.setMaxX(recRes.getMinX()+dw);
      } else { // overlap in the X direction
	int pL = ((int)rect.getCenterX())-(dw/2);
	int pR = pL+dw;
	if (recRes.getMinX()<= pL && recRes.getMaxX() >= pR){
	  /* if the rectangle is completely within space in this dimension */
	  recRes.setMinX(((int)rect.getCenterX())-(dw/2));
	  recRes.setMaxX(recRes.getMinX()+dw);
	} else {
	  if (recRes.getMinX()<= (rect.getCenterX()-dw/2)){
	    recRes.setMinX(recRes.getMaxX()-dw); // right of rectangle, but overlap
	  } else {
	    recRes.setMaxX(recRes.getMinX()+dw);  // left of rectangle, but overlap
	  }
	}
      }
      
      if (recRes.getMaxY()<=rect.getMinY()){ // bottom of rectangle
	recRes.setMinY(recRes.getMaxY()-dh);
      } else if (recRes.getMinY()>=rect.getMaxY()){
	recRes.setMaxY(recRes.getMinY()+dh);
      } else { // overlap in the Y direction
	int pB = ((int)rect.getCenterY())-(dh/2);
	int pT = pB+dh;
	if (recRes.getMinY()<= pB && recRes.getMaxY() >= pT){
	  recRes.setMinY(((int)rect.getCenterY())-(dh/2));
	  recRes.setMaxY(recRes.getMinY()+dh);
	} else {
	  if (recRes.getMinY()<= (rect.getCenterY()-dh/2)){
	    recRes.setMinY(recRes.getMaxY()-dh); // bottom of rectangle
	  } else {
	    recRes.setMaxY(recRes.getMinY()+dh);  // top of rectangle
	  }
	}
      }
      tmpD=Rectangle2i.getCenterDistanceBetween(recRes,rect);
      if (tmpD<distanceBetCenters || (tmpD==distanceBetCenters && tmpD < Rectangle2i.getCenterDistanceBetween(tie,rect))){
	distanceBetCenters=tmpD;
	retRect=recRes;
      }
    }
    return (retRect);
  }
  /**
   * Returns the closest rectangle from specified rectangle with at least a given
   * dimensions. 
   * Note: There can be a tie for closest rectangle, in this case we take the first
   * one we come across, meaning the one that is enclosed with the empty-space that 
   * has the largest area.  Also, if any of the rectangles overlap the specified rectangle,
   * then we take the rectangle that overlaps the most area of the specified rectangle.
   * @param rect specified rectangle
   * @param dw specified height of returned rectangle
   * @param dw specified width of returned rectangle
   */
  public Vector getClosestWithAtLeastDimension(Rectangle2i rect,double dw,double dh){
    Iterator i=getSpacesOrderedByArea();
    Rectangle2i closest=null,tmpRect=null;
    double minarea=dw * dh;
    double currentDistance=Integer.MAX_VALUE,tmpCD=0.,currentOverlap=0.;
    Vector closestV=new Vector();
    Rectangle2i result=new Rectangle2i();
    boolean pointQuery=rect.area()==0;
    while (i.hasNext()){
      tmpRect = (Rectangle2i)i.next();
      if (tmpRect.getWidth()>=dw && tmpRect.getHeight()>=dh){
	if ((tmpCD=Rectangle2i.getDistanceBetween(tmpRect,rect))<currentDistance){
	  currentDistance=tmpCD;
	  closest = tmpRect;
	  closestV.removeAllElements();
	  closestV.add(tmpRect);
	  if (tmpCD==0.){
	    if (Rectangle2i.intersect(rect,tmpRect,result)){
	      currentOverlap = result.area();
	    }
	  }
	} else if (currentDistance==0. && tmpCD==0.){
	  double compIntersection = 0.;
	  if (Rectangle2i.intersect(rect,tmpRect,result)){
	    compIntersection = result.area();
	    /* If area overlapped is greater than rectangle found before */
	    if (compIntersection>currentOverlap){
	      closest = tmpRect;
	      closestV.removeAllElements();
	      closestV.add(tmpRect);
	      currentOverlap=compIntersection;
	    } else if (pointQuery){
	      closestV.add(tmpRect);
	    }
	  }
	} else if (tmpCD==currentDistance){
	  closestV.add(tmpRect);
	}
      } else if (tmpRect.area()<dw*dh){
	break;
      }
    }
    return (closestV);
  }
  public Rectangle2i getLargestAreaInsideRectangle(Rectangle2i rect){
    Vector listOfRects = getAllOverlapSpaces(rect);
    Rectangle2i recRes = new Rectangle2i();
    double currentArea = Integer.MIN_VALUE,tmpArea;
    Rectangle2i retRect=new Rectangle2i();
    for (Enumeration e=listOfRects.elements(); e.hasMoreElements();){
      Rectangle2i.intersect(((Rectangle2i)e.nextElement()),rect,recRes); //.createIntersection(rect);
      if ((tmpArea=recRes.area())>currentArea){
	currentArea=tmpArea;
	retRect.setRect(recRes);
      }
    }
    return (retRect);
  }
  public boolean isCompletelyEnclosedBySpace(Rectangle2i rect){
    return (spaceRectangles.isEnclosedBy(rect));
  }
}





