/* 
 * @(#)SpaceManagerDisplayWrapper.java
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
 * This class uses a datastructure to help the implementation of the demonstration 
 * application (SpaceManagerTest).  Specifically, it adds a list of full-space
 * rectangle to keep track of drawing order of the full-space objects.
 *
 * <p>
 * @author Blaine Bell
 * @since JDK1.1
 *
 */
public class SpaceManagerDisplayWrapper extends SpaceManagerWrapper {
  private static final long serialVersionUID = 163821947126472832L;

  private Vector objectVector;
  private long maxLong;
  /**
   * Constructs a SpaceManagerDisplayWrapper that represents a specified area
   * 
   * @param  rs the area in which the Space Manager manages.
   */
  public SpaceManagerDisplayWrapper(Rectangle2i rs)  throws NoSuchMethodException, ClassNotFoundException {
    super(rs);
    objectVector = new Vector();
  }
  /**
   * Gets all full-space objects in display order.
   */
  public Iterator objectIterator(){
    return (objectVector.iterator());
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
   * @param  rs the full-space rectangle specified.
   * @param  place the place of the full-space rectangle in ordered list that is returned
   *
   */
  public FullRectangle2i addRectangle(FullRectangle2i rs,int place){
    FullRectangle2i ret;
    ret=super.addRectangle(rs);
    if (place<=0){
      objectVector.add(ret);
    } else {
      objectVector.insertElementAt(ret,place);
    }
    return (ret);
  }
  /**
   * Delete a full-space from the representation.  When this method gets 
   * called, the empty-space representation gets updated by adding and
   * deleting a number of largest empty-space rectangles to and from it.
   *
   * @param rs the Full-space rectangle being deleted
   */
  public int deleteRectangle(FullRectangle2i rs){
    super.deleteRectangle(rs);
    int ret;
    ret = objectVector.indexOf(rs);
    objectVector.remove(ret);
    return (ret);
  }

  /**
   * This is an Enumeration to allow us to traverse the full-space objects 
   * in backwards order.
   *
   */
  private class BackwardsEnumeration implements Enumeration {
    int count = 0;
    Vector vect;
    public BackwardsEnumeration(Vector v){
      vect=v;
      count = vect.size();
    }
    public boolean hasMoreElements() {
      return count > 0;
    }
    
    public Object nextElement() {
      synchronized (vect) {
	if (count > 0) {
	  return vect.elementAt(--count);
	}
      }
      throw new NoSuchElementException("Vector Enumeration");
    }
  }

  /**
   * Returns the closest full-space rectangle (i.e. rectangle drawn last)
   * for display purposes. 
   *
   * @param rectarg rectangle specified
   *
   */
  public FullRectangle2i getClosestRectangle(Rectangle2i rectarg){
    Vector v = getAllOverlapObjects(rectarg);//objectRectangles.windowQuery(rectarg);
    int i=0;
    for (Enumeration e = (Enumeration) new BackwardsEnumeration(objectVector); e.hasMoreElements() ;){
      FullRectangle2i rs=(FullRectangle2i)e.nextElement();
      if(v.contains(rs)){
	return(rs);
      }
    }
    return null;
  }
  /**
   * Deletes the full-space rectangle that is drawn last and overlaps the rectangle
   * specified. This is used to find the full-space rectangle to remove from the
   * representation when a user clicks on the screen in delete mode.
   *
   * @param rectarg rectangle specified to check for overlapped full-spaces
   */
  public int deleteRectangleWindow(Rectangle2i rectarg){
    //	System.out.println("deleteRectangleWindow:: rectarg=(" + rectarg.getMinX() + "," + rectarg.getMinY() + ")-(" + rectarg.getMaxX() + "," + rectarg.getMaxY() + ")");
    FullRectangle2i rect = getClosestRectangle(rectarg);
    if (rect!=null){
      return (deleteRectangle(rect));
    } else {
      return 0;
    }
  }
  /**
   * Prints all full-space objects in display order.
   */
  void printObjectRectangles(){
    for (Enumeration en=objectVector.elements(); en.hasMoreElements();){
      System.out.println("Full-Space rectangle: " + en.nextElement());
    }
  }
  public void printAllRectangles(){
    printObjectRectangles();
    printSpaceRectangles();
  }
}





