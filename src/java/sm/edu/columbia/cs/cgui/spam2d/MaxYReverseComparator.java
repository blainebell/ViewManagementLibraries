package edu.columbia.cs.cgui.spam2d;

import java.util.*;
import java.awt.geom.*;

public class MaxYReverseComparator extends RectangleComparator {
    public int compare(FullRectangle2d o1, FullRectangle2d o2){
      int ret = compare(o2.getMaxY(),o1.getMaxY());
      if (ret==0){
	  return (o2.id.compareTo(o1.id));
      }
      return (ret);
    }
  public int compare(Rectangle2d o1, Rectangle2d o2){
    return (compare (o1,o2,0));
  }
  int compare (Rectangle2d o1, Double o2){
//  System.out.println("MaxYReverseComparator.compare(Rectangle2d o1, Double o2)");  	
    int ret = compare(o1.getMaxY(), o2.doubleValue());
    return(ret);
  }
  static int compare(Rectangle2d o1, Rectangle2d o2,int count){
    int ret = compare(o2.getMaxY(),o1.getMaxY());
    if (ret==0 && count < 4){
      return(MinXComparator.compare(o2,o1,++count));
    }
    return (ret);
  }
}

