/* 
 * @(#)SpaceManager.java
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
 * This class is responsible for computing and maintaining a list of rectangles
 * that represent the empty-space.  The list of rectangles are considered to be
 * the largest rectangles that lie completely within the empty-space and cannot 
 * get any larger without overlaping any full-space.
 * <p>
 * @author Blaine Bell
 * @since JDK1.1
 *
 */

public class SpaceManager implements Cloneable, Serializable {
  private static final long serialVersionUID = 823073100499990844L;
  
  protected IntervalTree spaceRectangles;
  protected IntervalTree objectRectangles;
  private Rectangle2i screenSpace;
  private long maxLong=-1;
  int processing;
  /**
   * This is the list of SpaceManagerListeners listening for changes
   * to the representation.
   *
   */
  protected Vector spacemanagerlisteners;
  /** 
   * The callbacks for listeners that want to be notified each time
   * a empty-space gets added or deleted from the space manager.  Since
   * each operation (add and delete) adds and deletes a list of empty 
   * spaces, it notifies the callbacks when lists get added and deleted 
   * as well. */
  public interface SpaceManagerListener {
    /** 
     * Function is called when a list of empty-space rectangles are added and
     * deleted to SpaceManager representation when listening.
     *
     * @param fr Full-space rectangle added or deleted
     * @param added The list of empty-space rectangles added.
     * @param deleted The list of empty-space rectangles deleted.
     */
    public void addDeleteSpaceRectangles(FullRectangle2i fr, Collection added, Collection deleted);
  }
    /**
     * Adds a <code>SpaceManagerListener</code> to the SpaceManager. Multiple
     * listeners can be added to a SpaceManager.
     * @param sml the <code>SpaceManagerListener</code> to be added
     */
  public void addSpaceManagerListener(SpaceManagerListener sml){
    spacemanagerlisteners.add(sml);
  }
    /**
     * Removes a <code>SpaceManagerListener</code> from the SpaceManager.
     * If the listener is the currently Listening, then it is removed
     * so it no longer listens.
     *
     * @param sml the listener to be removed
     */
  public void removeSpaceManagerListener(SpaceManagerListener sml){
    spacemanagerlisteners.remove(sml);
  }
  /**
   * Removes all full-space rectangles specified from representation
   *
   * @param v list of all full-space rectangles deleted
   *
   */
  public void deleteAllFullRectangles(Vector v){
    FullRectangle2i fr=null;
    for (Enumeration e=v.elements(); e.hasMoreElements();){
      fr = (FullRectangle2i)e.nextElement();
      deleteRectangle(fr);
    }
  }
  /**
   * Creates a copy of the space manager.  
   *
   * @return the copy
   */
  protected Object clone(){
    SpaceManager clone = null;
    try {
      clone = (SpaceManager) super.clone();
    } catch (CloneNotSupportedException e) { 
      throw new InternalError();
    }
    clone.spaceRectangles = (IntervalTree)spaceRectangles.clone();
    clone.objectRectangles = (IntervalTree)objectRectangles.clone();
    clone.screenSpace = (Rectangle2i)screenSpace.clone();
    clone.processing = processing;
    clone.rectClass = rectClass;
    clone.minxMeth = minxMeth;
    clone.maxxMeth = maxxMeth;
    clone.ss = ss;
    clone.iD = iD;
    clone.spacemanagerlisteners = new Vector();
    
    return (clone);
  }
  
  // --- Serialization ------------------------------------
  
  /** 
   * 
   * Serializes the SpaceManager.
   * 
   */
  private void writeObject(java.io.ObjectOutputStream out)
    throws IOException {
    out.writeInt(processing);
    out.writeObject(screenSpace);
    out.writeObject(spaceRectangles);
    out.writeObject(objectRectangles);
  }
  
  /** 
   * 
   * Reads a serialized SpaceManager.
   * 
   */
  private void readObject(java.io.ObjectInputStream in)
    throws IOException, ClassNotFoundException {
    try {
      rectClass = Class.forName("edu.columbia.cs.cgui.spam2i.Rectangle2i");
      minxMeth = rectClass.getDeclaredMethod("getMinX",null);
      maxxMeth = rectClass.getDeclaredMethod("getMaxX",null);
      ss = Class.forName("edu.columbia.cs.cgui.spam2i.FirstSecondaryStructure");
      iD = new IntervalDimension(1,minxMeth,maxxMeth);
    } catch (Exception e){
      e.printStackTrace();
    }
    processing = in.readInt();
    screenSpace = (Rectangle2i) in.readObject();
    spaceRectangles = (IntervalTree) in.readObject();
    objectRectangles = (IntervalTree) in.readObject();
  }
  void printObjectRectangles(){
    objectRectangles.print(System.err);
  }
  void printSpaceRectangles(){
    spaceRectangles.printObjects(System.err);
  }
  Class rectClass=null;
  Method minxMeth=null,maxxMeth=null;
  Class ss = null;
  IntervalDimension iD = null;
  
  /**
   * Constructs a SpaceManager that represents a specified area.
   *
   * @param  rs the area in which the Space Manager manages.
   *
   */
  public SpaceManager(Rectangle2i rs) throws NoSuchMethodException, ClassNotFoundException {
    rectClass = Class.forName("edu.columbia.cs.cgui.spam2i.Rectangle2i");
    minxMeth = rectClass.getDeclaredMethod("getMinX",null);
    maxxMeth = rectClass.getDeclaredMethod("getMaxX",null);
    ss = Class.forName("edu.columbia.cs.cgui.spam2i.FirstSecondaryStructure");
    iD = new IntervalDimension(1,minxMeth,maxxMeth);
    
    spaceRectangles = new IntervalTree(rectClass,minxMeth,maxxMeth,ss,iD);
    objectRectangles = new IntervalTree(rectClass,minxMeth,maxxMeth,ss,iD);
    Rectangle2i init = new Rectangle2i(rs);
    spaceRectangles.addRectangle(init);
    screenSpace = new Rectangle2i(init);
    
    spacemanagerlisteners = new Vector();
    processing=0;
  }
  
  /**
   * This function adds an empty-space rectangle to the representation and
   * is called from the algorithms each time space rectangle is added
   * to representation. If this function is called from outside
   * the SpaceManager, representation could become inconsistant if 
   * not properly used.
   *
   * @param r empty-space rectangle added.
   */
  protected boolean addSpaceRectangleDirect(Rectangle2i r){
    spaceRectangles.addRectangle(r);
    return(true);
  }
  /**
   * This function deletes an full-space rectangle to the representation.
   * If this function is called from outside the SpaceManager, 
   * representation could become inconsistant if not properly used.
   *
   * @param r empty-space rectangle added.
   */
  protected int deleteObjectRectangleDirect(FullRectangle2i rs){
    int ret=rs.id.intValue();
    objectRectangles.deleteRectangle(rs);
    return (ret);
  }

  /**
   * This function deletes an empty-space rectangle to the representation and
   * is called from the algorithms each time space rectangle is added
   * to representation. If this function is called from outside
   * the SpaceManager, representation could become inconsistant if 
   * not properly used.
   */
  protected boolean deleteSpaceRectangleDirect(Rectangle2i rs){
    boolean ret=true;
    if (!spaceRectangles.deleteRectangle(rs)){
      System.err.println("spaceRectangles.deleteRectangle returned false");
      ret=false;
    }
    return (ret);
  }
  /**
   * This function deletes a full-space object directly from the
   * representation.  The changes of empty-spaces are specified as 
   * arguements to this function so that there is no need for any algorithms,
   * these empty-spaces get directly added/deleted to representation.
   * Listeners are notified accordingly.
   *
   * @param added list of empty-space rectangles added to representation
   * @param deleted list of empty-space rectangles deleted to representation
   *
   */
  protected int deleteObjectRectangleDirect(FullRectangle2i rs, Vector added, Vector deleted){
    Rectangle2i r;
    for (Enumeration e=deleted.elements();e.hasMoreElements();){
      r=(Rectangle2i)e.nextElement();
      deleteSpaceRectangleDirect(r);
    }
    
    for (Enumeration e=added.elements();e.hasMoreElements();){
      r=(Rectangle2i)e.nextElement();
      addSpaceRectangleDirect(r);
    }
    for (Enumeration en=spacemanagerlisteners.elements(); en.hasMoreElements();){
      SpaceManagerListener sml = (SpaceManagerListener) en.nextElement();
      sml.addDeleteSpaceRectangles(rs, added, deleted);
    }
    return(deleteObjectRectangleDirect(rs));
  }

  /**
   * This function adds an full-space rectangle to the representation.
   * If this function is called from outside the SpaceManager, 
   * representation could become inconsistant if not properly used.
   *
   * @param r empty-space rectangle added.
   *
   */
  protected FullRectangle2i addObjectRectangle(FullRectangle2i rs,int place){
    FullRectangle2i r = rs ;
    objectRectangles.addRectangle(r);
    return (r);
  }
  /**
   * Moves a full-space rectangle in the direction specified in the x and y directions.
   * This implementation deletes and then re-adds the full-space rectangle using
   * the functions deleteRectangle and addRectangle.  It provides no optimizations.
   *
   * @param rect full-space rectangle moved
   * @param x distance full-space moved in the x direction
   * @param y distance full-space moved in the y direction
   * 
   */
  public FullRectangle2i moveRectangle(FullRectangle2i rect,int x, int y){
    int place;
    place=deleteRectangle(rect);
    rect.setRect(rect.getMinX()+x, rect.getMinY()+y, rect.getMaxX()+x, rect.getMaxY()+y);
    addRectangle(rect,place);
    return (rect);
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
    return (addRectangle(rs,-1));
  }
  /**
   * Adds a full-space to the Representation. When this method gets
   * called, the empty-space representation gets updated by having a 
   * number of largest empty-space rectangles added and deleted.
   *
   * @param  r the full-space rectangle specified.
   * @param  place the place of the full-space rectangle in ordered list that is returned
   *
   */
  public FullRectangle2i addRectangle(FullRectangle2i r,int place){
    if (processing>0){
      System.out.println("addRectangle: another thread is processing=" + processing);
    }
    processing++;
    if (r.area()<=0){
      System.err.println("Warning: area of rectangle passed in addRectangle less than or equal to 0 r=" + r);
      processing--;
      return null;
    }
    FullRectangle2i rs=new FullRectangle2i(r);
    FullRectangle2i ret=addObjectRectangle(rs,place);
    if (ret==null){
      System.err.println("Warning: addObjectRectangle returned null");
      processing--;
      return (null);
    }
    Vector v[],va[];
    Vector smlvadding = new Vector(),smlvdeleting = new Vector();
    
    Vector containmentList= spaceRectangles.windowQuery(rs);
    
    v=new Vector[4];
    va=new Vector[4];
    for (int f=0; f<4; f++){
      v[f] = new Vector();
      va[f] = new Vector();
    }
    
    int contnum=0;
    
    int i=0;
    Rectangle2i tmpRect,tmpRect2;
    Enumeration e;
    boolean removed;
    for (e = containmentList.elements(); e.hasMoreElements();){
      Rectangle2i clrs = (Rectangle2i)e.nextElement();
      i++;
      if (rs.getMinX()==clrs.getMaxX()){  // Exactly adjacent to left, only add to list for containment query
	va[0].add(clrs);
	continue;
      }
      if (rs.getMaxX()==clrs.getMinX()){  // Exactly adjacent to right, only add to list for containment query
	va[1].add(clrs);
	continue;
      }
      if (rs.getMaxY()==clrs.getMinY()){  // Exactly adjacent to top, only add to list for containment query
	va[2].add(clrs);
	continue;
      }
      if (rs.getMinY()==clrs.getMaxY()){  // Exactly adjacent to bottom, only add to list for containment query
	va[3].add(clrs);
	continue;
      }
      if (!spacemanagerlisteners.isEmpty()){
	smlvdeleting.add(clrs);	
      }
      if (!deleteSpaceRectangleDirect(clrs)){
	System.out.println("in addRectangle");
      };
      if (rs.getMinX()>clrs.getMinX()){ // Left
	v[0].add(new Rectangle2i(clrs.getMinX(),clrs.getMinY(),rs.getMinX(),clrs.getMaxY()));
      }
      if (clrs.getMaxX()>rs.getMaxX()){ // right
	v[1].add(new Rectangle2i(rs.getMaxX(),clrs.getMinY(),clrs.getMaxX(),clrs.getMaxY()));
      }
      if (clrs.getMaxY()>rs.getMaxY()){ // top
	v[2].add(new Rectangle2i(clrs.getMinX(),rs.getMaxY(),clrs.getMaxX(),clrs.getMaxY()));
      }
      if (rs.getMinY()>clrs.getMinY()){ // bottom
	v[3].add(new Rectangle2i(clrs.getMinX(),clrs.getMinY(),clrs.getMaxX(),rs.getMinY()));
      }
    }

    for (int doeach=0; doeach<4; doeach++){
      for (Iterator it=v[doeach].iterator();it.hasNext();){
	tmpRect = (Rectangle2i)it.next();
	removed=false;
	for (e=va[doeach].elements();e.hasMoreElements();){
	  tmpRect2 = (Rectangle2i)e.nextElement();
	  if (tmpRect.enclosedBy(tmpRect2)){
	    it.remove();
	    removed=true;
	    break;
	  }
	}
	if (!removed){
	  for (e=v[doeach].elements();e.hasMoreElements();){
	    tmpRect2 = (Rectangle2i)e.nextElement();
	    if (tmpRect2==tmpRect){
	      continue;
	    }
	    if (tmpRect.enclosedBy(tmpRect2)){
	      it.remove();
	      break;
	    }
	  }
	}
      }
      for (e=v[doeach].elements();e.hasMoreElements();){
	tmpRect = (Rectangle2i)e.nextElement();
	addSpaceRectangleDirect(tmpRect);
      }
      if (!spacemanagerlisteners.isEmpty()){
	smlvadding.addAll(v[doeach]);
      }
    }
    if (!spacemanagerlisteners.isEmpty()){
      for (Enumeration en=spacemanagerlisteners.elements(); en.hasMoreElements();){
	SpaceManagerListener sml = (SpaceManagerListener) en.nextElement();
	sml.addDeleteSpaceRectangles(rs, smlvadding, smlvdeleting);
      }
    }
    processing--;
    return (ret);
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
    if (processing>0){
      System.out.println("deleteRectangle: another thread is processing=" + processing);
    }
    processing++;
    int ret;
    ret=deleteObjectRectangleDirect(rs);
    Rectangle2i inter = rs.createIntersection(screenSpace);
    
    if (inter.area() <= 0.0){
      processing--;
      return(ret);
    }
    Vector adjacentContainmentList = spaceRectangles.windowQuery(rs);
    
    IntervalTree tmpIntervals[] = new IntervalTree[4];
    Class spaceRectClass = null;
    Method minMeth[] = new Method[2], maxMeth[] = new Method[2];
    
    IntervalTree bottomInterval=null,
      topInterval=null,
      leftInterval=null,
      rightInterval=null;
    
    try {
      Class rectClass = Class.forName("edu.columbia.cs.cgui.spam2i.Rectangle2i");
      spaceRectClass = Class.forName("edu.columbia.cs.cgui.spam2i.Rectangle2i");
      minMeth[0] = rectClass.getDeclaredMethod("getMinX",null);
      maxMeth[0] = rectClass.getDeclaredMethod("getMaxX",null);
      minMeth[1] = rectClass.getDeclaredMethod("getMinY",null);
      maxMeth[1] = rectClass.getDeclaredMethod("getMaxY",null);
      tmpIntervals[0] = new IntervalTree(spaceRectClass,minMeth[1],maxMeth[1],Class.forName("edu.columbia.cs.cgui.spam2i.YSecondaryStructure"), new IntervalDimension(1,minMeth[1],maxMeth[1]));
      tmpIntervals[1] = new IntervalTree(spaceRectClass,minMeth[1],maxMeth[1],Class.forName("edu.columbia.cs.cgui.spam2i.YSecondaryStructure"), new IntervalDimension(1,minMeth[1],maxMeth[1]));
      tmpIntervals[2] = new IntervalTree(spaceRectClass,minMeth[0],maxMeth[0],Class.forName("edu.columbia.cs.cgui.spam2i.XSecondaryStructure"), new IntervalDimension(1,minMeth[0],maxMeth[0]));
      tmpIntervals[3] = new IntervalTree(spaceRectClass,minMeth[0],maxMeth[0],Class.forName("edu.columbia.cs.cgui.spam2i.XSecondaryStructure"), new IntervalDimension(1,minMeth[0],maxMeth[0]));
    } catch (Exception e){
      e.printStackTrace();
    }
    
    Vector completeResultList = new Vector();
    for (Iterator i=adjacentContainmentList.iterator(); i.hasNext();){
      Rectangle2i r = (Rectangle2i)i.next();
      if (r.getMaxX()==rs.getMinX()){ // left space rectangles
	tmpIntervals[0].addRectangle(r);
      } else if (r.getMinX()==rs.getMaxX()){ // right space rectangles
	tmpIntervals[1].addRectangle(r);
      } else if (r.getMaxY()==rs.getMinY()){ // bottom space rectangles
	tmpIntervals[2].addRectangle(r);
      } else if (r.getMinY()==rs.getMaxY()){ // top space rectangles
	tmpIntervals[3].addRectangle(r);
      }
      completeResultList.add(r);
      deleteSpaceRectangleDirect(r);
    }
    
    SpaceManagerWrapper deletetmpmanager=null;
	
    Vector containmentList = objectRectangles.windowQueryWithoutEdges(rs);
    Vector listOfInnerSpaces = new Vector();
    
    if (containmentList.size()==0){
      listOfInnerSpaces.add(inter);
    } else {
      try {
	deletetmpmanager = new SpaceManagerWrapper(inter);
      } catch (Exception e){
	e.printStackTrace();
      }
      for (Enumeration e=containmentList.elements(); e.hasMoreElements();){
	deletetmpmanager.addRectangle(new FullRectangle2i((FullRectangle2i)e.nextElement()),-1);
      }
      listOfInnerSpaces.addAll(deletetmpmanager.spaceTrees);
    }
    Vector listOfBits = new Vector();
    for (Iterator iter=listOfInnerSpaces.iterator(); iter.hasNext();){
      Rectangle2i currRect = (Rectangle2i) iter.next();
      BitSet bs=new BitSet(4); // left=0, right=1, bottom=2, top=3
      int count=0;
      if (rs.getMinX()==currRect.getMinX()){ bs.set(0); }
      if (rs.getMaxX()==currRect.getMaxX()){ bs.set(1); }
      if (rs.getMinY()==currRect.getMinY()){ bs.set(2); }
      if (rs.getMaxY()==currRect.getMaxY()){ bs.set(3); }
      if (bs.length()==0){
	addSpaceRectangleDirect(currRect);
	iter.remove();
	continue;
      } else {
	listOfBits.add(bs);
      }
    }

    for (int whichDim=0,dim=0; whichDim<4; whichDim++,dim=whichDim/2){
      Vector newListOfInnerSpaces = new Vector();
      Vector newListOfBits = new Vector();
      for (Enumeration e=listOfInnerSpaces.elements(), e2=listOfBits.elements();
	   e.hasMoreElements();){
	Rectangle2i currRect = (Rectangle2i) e.nextElement();
	BitSet bs = (BitSet) e2.nextElement();
	BitSet currbs = (BitSet)bs.clone();
	if (!bs.get(whichDim)){
	  newListOfInnerSpaces.add(currRect);
	  newListOfBits.add(bs.clone());
	  continue;
	}

	boolean enclosed=false;
	Rectangle2i longest=null;
	BitSet longestbs=null;
	
	for (Enumeration e3=tmpIntervals[whichDim].windowQueryWithoutEdges(currRect).elements();e3.hasMoreElements();){
	  Rectangle2i tmpRect=(Rectangle2i)e3.nextElement();
	  Rectangle2i tmpR = Rectangle2i.consensus(tmpRect,currRect,dim);
	  // Taking into account spaces that are the same height as the tmpRect before the consensus
	  boolean enc=true;
	  if (dim!=0){
	    if (enc && tmpRect.getMinX()<=currRect.getMinX() && 
		tmpRect.getMaxX()>=currRect.getMaxX()){
	      enc=true;
	    } else {
	      enc=false;
	    }
	  }
	  if (dim!=1){
	    if (enc && tmpRect.getMinY()<=currRect.getMinY() && 
		tmpRect.getMaxY()>=currRect.getMaxY()){
	      enc=true;
	    } else {
	      enc=false;
	    }
	  }
	  if (enc){
	    enclosed=true;
	    if (longest==null){
	      longest=tmpR;
	      longestbs=(BitSet)currbs.clone();
	    } else {
	      boolean setlong=false;
	      try {
		if (whichDim%2==0){
		  if (((Integer)minMeth[dim].invoke(longest,null)).doubleValue() > 
		      ((Integer)minMeth[dim].invoke(tmpR,null)).doubleValue()){
		    setlong=true;
		  }
		} else {
		  if (((Integer)maxMeth[dim].invoke(longest,null)).doubleValue() < 
		      ((Integer)maxMeth[dim].invoke(tmpR,null)).doubleValue()){
		    setlong=true;
		  }
		}
	      } catch (Exception ex){
		ex.printStackTrace();
	      }
	      if (setlong){
		longest=tmpR;
		longestbs=(BitSet)currbs.clone();
	      }
	    }
	  } else {
	    newListOfInnerSpaces.add(tmpR);
	    BitSet clbs = (BitSet)bs.clone();
	    if (dim!=0){
	      if (clbs.get(0) && tmpR.getMinX()>inter.getMinX()) { clbs.clear(0); }
	      if (clbs.get(1) && tmpR.getMaxX()<inter.getMaxX()) { clbs.clear(1); }
	    }
	    if (dim!=1){
	      if (clbs.get(2) && tmpR.getMinY()>inter.getMinY()) { clbs.clear(2); }
	      if (clbs.get(3) && tmpR.getMaxY()<inter.getMaxY()) { clbs.clear(3); }
	    }
	    newListOfBits.add(clbs);
	  }
	}
	if (!enclosed){
	  newListOfInnerSpaces.add(currRect);
	  newListOfBits.add(bs);		    
	} else {
	  newListOfInnerSpaces.add(longest);
	  newListOfBits.add(longestbs);
	}
      }
      listOfInnerSpaces = newListOfInnerSpaces;
      listOfBits = newListOfBits;
    }
    listOfInnerSpaces.addAll(adjacentContainmentList);
    for (Iterator it2=listOfInnerSpaces.iterator(); it2.hasNext();){
      Rectangle2i rect1=(Rectangle2i)it2.next();
      for (Enumeration e=listOfInnerSpaces.elements(); e.hasMoreElements();){
	Rectangle2i rect2=(Rectangle2i)e.nextElement();
	if (rect1==rect2){
	  continue;
	}
	if (rect1.enclosedBy(rect2)){
	  it2.remove();
	  break;
	}
      }
    }
    int asdf=0;
    for (Enumeration e=listOfInnerSpaces.elements(); e.hasMoreElements();){
      Rectangle2i tmpRect=(Rectangle2i)e.nextElement();
      addSpaceRectangleDirect(tmpRect);
    }
    if (!spacemanagerlisteners.isEmpty()){
      completeResultList.removeAll(listOfInnerSpaces);
      for (Enumeration en=spacemanagerlisteners.elements(); en.hasMoreElements();){
	SpaceManagerListener sml = (SpaceManagerListener) en.nextElement();
	sml.addDeleteSpaceRectangles(rs, listOfInnerSpaces, completeResultList);
      }
    }
    processing--;
    return (ret);
  }
  /**
   * Get all full-space objects that overlap a given area specified.
   *
   * @param rect area specified.
   *
   */
  public Vector getAllOverlapObjects(Rectangle2i rect){
    return (objectRectangles.windowQueryWithoutEdges(rect));
  }
  /**
   * Get all empty-space objects that overlap a given area specified.
   *
   * @param rect area specified.
   *
   */
  public Vector getAllOverlapSpaces(Rectangle2i rect){
    return (spaceRectangles.windowQueryWithoutEdges(rect));
  }
  /**
   * Gets the rectangle the area in which the spacemanager considers to 
   * be available space where empty spaces can exist. Full spaces can be
   * defined outside this area, but all empty-spaces are defined inside. 
   */
  public Rectangle2i getScreenSpace(){
    return (screenSpace);
  }
}

