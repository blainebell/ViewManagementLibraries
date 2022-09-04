package edu.columbia.cs.cgui.spam2i;

import java.util.*;
import java.awt.geom.*;

public class MinYComparator extends RectangleComparator {
    public int compare(FullRectangle2i o1, FullRectangle2i o2){
      int ret = compare(o1.getMinY(),o2.getMinY());
      if (ret==0){
	  return (o1.id.compareTo(o2.id));
      }
      return (ret);
  }
  public int compare(Rectangle2i o1, Rectangle2i o2){
    return (compare (o1,o2,0));
  }
  int compare (Rectangle2i o1, Integer o2){
    int ret = compare(o1.getMinY(), o2.intValue());
    return(ret);
  }
  static int compare(Rectangle2i o1, Rectangle2i o2,int count){
    int ret = compare(o1.getMinY(),o2.getMinY());
    if (ret==0 && count < 4){
      return(MaxXComparator.compare(o1,o2,++count));
    }
    return (ret);
  }
}
