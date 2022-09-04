package edu.columbia.cs.cgui.spam2i;

import java.util.*;
import java.awt.geom.*;
import java.io.*;
/**
 * This class compares 
 *
 */
public class AreaComparator implements Comparator, Serializable {
    private static final long serialVersionUID = -813005466287772766L;
    public int compare(Rectangle2i o1, Rectangle2i o2){
	int o1a=o1.area(),o2a=o2.area();
	if (o1a>o2a){
	    return(-1);
	} else if (o1a==o2a){
	    int ret=MinXComparator.compare(o1,o2,0);
	    return (ret);
	} else {
	    return (1);
	}
    }
    public int compare(Object o1, Object o2){
	return (compare((Rectangle2i) o1, (Rectangle2i) o2));
    }
}
