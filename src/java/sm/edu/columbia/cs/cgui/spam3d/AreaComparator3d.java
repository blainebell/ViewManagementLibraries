package edu.columbia.cs.cgui.spam3d;

import java.util.*;
import java.awt.geom.*;
import java.io.*;

public class AreaComparator3d implements Comparator, Serializable { // extends RectangleComparator {
    private static final long serialVersionUID = -813005466287772766L;
    public int compare(Rectangle3d o1, Rectangle3d o2){
	double o1a=o1.area(),o2a=o2.area();
	if (o1a>o2a){
	    return(-1);
	} else if (o1a==o2a){
	    int ret=MinXComparator3d.compare(o1,o2,0);
	    return (ret);
	} else {
	    return (1);
	}
    }
    public int compare(Object o1, Object o2){
	return (compare((Rectangle3d) o1, (Rectangle3d) o2));
    }
    /*    int compare (Rectangle2d o1, Double o2){
	  //    System.out.println("MinXComparator.compare(Rectangle2d o1, Double o2)");
	  int ret = compare(o1.area(), o2.doubleValue());
	  return(ret);
	  }
    */
}
