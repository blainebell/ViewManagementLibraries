package edu.columbia.cs.cgui.spam3d;

import java.util.*;
import java.awt.geom.*;

public class MaxZReverseComparator3d extends RectangleComparator3d {
    public int compare(FullRectangle3d o1, FullRectangle3d o2){
      int ret = compare(o2.getMaxZ(),o1.getMaxZ());
      if (ret==0){
	  return (o2.id.compareTo(o1.id));
      }
      return (ret);
    }
  public int compare(Rectangle3d o1, Rectangle3d o2){
    return (compare (o1,o2,0));
  }
  int compare (Rectangle3d o1, Double o2){
//  System.out.println("MaxZReverseComparator.compare(Rectangle3d o1, Double o2)");  	
    int ret = compare(o1.getMaxZ(), o2.doubleValue());
    return(ret);
  }
  static int compare(Rectangle3d o1, Rectangle3d o2,int count){
    int ret = compare(o2.getMaxZ(),o1.getMaxZ());
    if (ret==0 && count < 4){
      return(MinXComparator3d.compare(o2,o1,++count));
    }
    return (ret);
  }
}

