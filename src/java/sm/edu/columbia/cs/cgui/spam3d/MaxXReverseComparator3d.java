package edu.columbia.cs.cgui.spam3d;

import java.util.*;
import java.awt.geom.*;

public class MaxXReverseComparator3d extends RectangleComparator3d {
    public int compare(FullRectangle3d o1, FullRectangle3d o2){
      int ret = compare(o2.getMaxX(),o1.getMaxX());
      if (ret==0){
	  return (o2.id.compareTo(o1.id));
      }
      return (ret);
    }
  public int compare(Rectangle3d o1, Rectangle3d o2){
    return (compare (o1,o2,0));
  }
  int compare (Rectangle3d o1, Double o2){
//  System.out.println("MaxXReverseComparator.compare(Rectangle3d o1, Double o2)");  	
    int ret = compare(o1.getMaxX(), o2.doubleValue());
    return(ret);
  }
  static int compare(Rectangle3d o1, Rectangle3d o2,int count){
    int ret = compare(o2.getMaxX(),o1.getMaxX());
    if (ret==0 && count < 4){
      return(MaxYComparator3d.compare(o2,o1,++count));
    }
    return (ret);
  }
}
