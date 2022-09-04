package edu.columbia.cs.cgui.spam3d;

import java.util.*;
import java.awt.geom.*;
import java.io.*;

abstract class RectangleComparator3d implements Comparator,Serializable {
    private void writeObject(java.io.ObjectOutputStream out)
	throws IOException {
	//	out.writeObject(getClass());
    } 
    private void readObject(java.io.ObjectInputStream in)
	throws IOException, ClassNotFoundException {
	//	this = (RectangleComparator3d) ((Class)in.readObject()).newInstance();
    }
  static int compare (double o1, double o2){
    //    System.out.println("RectangularComparator.compare(double,double)");
    if (o1==o2){
      return (0);
    } else if (o1 < o2){
      return(-1);
    } else {
      return(1);
    }
  }
  static int compare(Rectangle3d o1, Rectangle3d o2, int count){
    //    System.out.println("RectangleComparator3d.compare(Rectangle3d o1, Rectangle3d o2, int count)");
    return (0);
  }
  abstract int compare(FullRectangle3d o1, FullRectangle3d o2);
  abstract int compare(Rectangle3d o1, Rectangle3d o2);
  abstract int compare(Rectangle3d o1, Double o2);
  public int compare(Object o1, Object o2){
    Double c1=null,c2=null;
    //    System.out.println("RectangleComparator3d.compare(Object o1, Object o2)\to1=" + o1 + " o2=" + o2);
    if (o1 instanceof FullRectangle3d && o2 instanceof FullRectangle3d){
	return(compare((FullRectangle3d)o1,(FullRectangle3d)o2));
    } else if (o1 instanceof Rectangle3d && o2 instanceof Rectangle3d){
      return(compare((Rectangle3d)o1,(Rectangle3d)o2));
    } else if (o1 instanceof Double && o2 instanceof Double){
      return(((Double)o1).compareTo((Double)o2));
    } else if (o1 instanceof Rectangle3d && o2 instanceof Double){
      return(compare((Rectangle3d)o1, (Double) o2));
    } else if (o1 instanceof Double && o2 instanceof Rectangle3d){
      return (-compare((Rectangle3d)o2, (Double) o1));
    }
    return (0);
  }
}

