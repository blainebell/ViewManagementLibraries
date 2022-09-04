package edu.columbia.cs.cgui.spam3d;

import java.util.*;
import java.awt.geom.*;

public class MinXComparator3d extends RectangleComparator3d {
    public int compare(FullRectangle3d o1, FullRectangle3d o2){
      int ret = compare(o1.getMinX(),o2.getMinX());
      if (ret==0){
	  return (o1.id.compareTo(o2.id));
      }
      return (ret);
  }
  public int compare(Rectangle3d o1, Rectangle3d o2){
      //      System.out.println("MinXComparator3d.compare(Rectangle3d o1, Rectangle3d o2)");
    return (compare (o1,o2,0));
  }
  int compare (Rectangle3d o1, Double o2){
//    System.out.println("MinXComparator3d.compare(Rectangle3d o1, Double o2)");
    int ret = compare(o1.getMinX(), o2.doubleValue());
    return(ret);
  }
  static int compare(Rectangle3d o1, Rectangle3d o2,int count){
//    System.out.println("MinXComparator3d.compare(Rectangle3d o1, Rectangle3d o2, int count)");
    int ret = compare(o1.getMinX(),o2.getMinX());
    if (ret==0 && count < 4){
      return(MinYComparator3d.compare(o1,o2,++count));
    }
    return (ret);
  }
}
