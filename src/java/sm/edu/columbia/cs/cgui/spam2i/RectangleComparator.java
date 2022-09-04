package edu.columbia.cs.cgui.spam2i;

import java.util.*;
import java.awt.geom.*;
import java.io.*;

abstract class RectangleComparator implements Comparator,Serializable {
    private void writeObject(java.io.ObjectOutputStream out)
	throws IOException {
	//	out.writeObject(getClass());
    } 
    private void readObject(java.io.ObjectInputStream in)
	throws IOException, ClassNotFoundException {
	//	this = (RectangleComparator) ((Class)in.readObject()).newInstance();
    }
  static int compare (int o1, int o2){
    //    System.out.println("RectangularComparator.compare(double,double)");
    if (o1==o2){
      return (0);
    } else if (o1 < o2){
      return(-1);
    } else {
      return(1);
    }
  }
  static int compare(Rectangle2i o1, Rectangle2i o2, int count){
    //    System.out.println("RectangleComparator.compare(Rectangle2d o1, Rectangle2d o2, int count)");
    return (0);
  }
  abstract int compare(FullRectangle2i o1, FullRectangle2i o2);
  abstract int compare(Rectangle2i o1, Rectangle2i o2);
  abstract int compare(Rectangle2i o1, Integer o2);
  public int compare(Object o1, Object o2){
    Integer c1=null,c2=null;
    //    System.out.println("RectangleComparator.compare(Object o1, Object o2)\to1=" + o1 + " o2=" + o2);
    if (o1 instanceof FullRectangle2i && o2 instanceof FullRectangle2i){
	return(compare((FullRectangle2i)o1,(FullRectangle2i)o2));
    } else if (o1 instanceof Rectangle2i && o2 instanceof Rectangle2i){
      return(compare((Rectangle2i)o1,(Rectangle2i)o2));
    } else if (o1 instanceof Integer && o2 instanceof Integer){
      return(((Integer)o1).compareTo((Integer)o2));
    } else if (o1 instanceof Rectangle2i && o2 instanceof Integer){
      return(compare((Rectangle2i)o1, (Integer) o2));
    } else if (o1 instanceof Integer && o2 instanceof Rectangle2i){
      return (-compare((Rectangle2i)o2, (Integer) o1));
    }
    return (0);
  }
}

