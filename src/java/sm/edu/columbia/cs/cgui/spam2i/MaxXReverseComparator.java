package edu.columbia.cs.cgui.spam2i;

import java.util.*;
import java.awt.geom.*;

public class MaxXReverseComparator extends RectangleComparator {
    public int compare(FullRectangle2i o1, FullRectangle2i o2){
      int ret = compare(o2.getMaxX(),o1.getMaxX());
      if (ret==0){
	  return (o2.id.compareTo(o1.id));
      }
      return (ret);
    }
  public int compare(Rectangle2i o1, Rectangle2i o2){
    return (compare (o1,o2,0));
  }
  int compare (Rectangle2i o1, Integer o2){
//  System.out.println("MaxXReverseComparator.compare(Rectangle2i o1, Integer o2)");  	
    int ret = compare(o1.getMaxX(), o2.intValue());
    return(ret);
  }
  static int compare(Rectangle2i o1, Rectangle2i o2,int count){
    int ret = compare(o2.getMaxX(),o1.getMaxX());
    if (ret==0 && count < 4){
      return(MaxYComparator.compare(o2,o1,++count));
    }
    return (ret);
  }
}
