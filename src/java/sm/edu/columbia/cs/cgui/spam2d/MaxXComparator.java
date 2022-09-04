package edu.columbia.cs.cgui.spam2d;

import java.util.*;
import java.awt.geom.*;

public class MaxXComparator extends RectangleComparator {
    public int compare(FullRectangle2d o1, FullRectangle2d o2){
      int ret = compare(o1.getMaxX(),o2.getMaxX());
      if (ret==0){
	  return (o1.id.compareTo(o2.id));
      }
      return (ret);
  }
  public int compare(Rectangle2d o1, Rectangle2d o2){
    return (compare (o1,o2,0));
  }
  int compare (Rectangle2d o1, Double o2){
//  System.out.println("MaxXComparator.compare(Rectangle2d o1, Double o2)");  	
    int ret = compare(o1.getMaxX(), o2.doubleValue());
    return(ret);
  }
  static int compare(Rectangle2d o1, Rectangle2d o2,int count){
    int ret = compare(o1.getMaxX(),o2.getMaxX());
    if (ret==0 && count < 4){
      return(MaxYComparator.compare(o1,o2,++count));
    }
    return (ret);
  }
}
