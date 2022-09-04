package edu.columbia.cs.cgui.spam2i;

import java.lang.*;
import java.util.*;
import java.lang.reflect.*;
import java.io.*;
import java.text.*;

class SecondSecondaryStructure extends SecondaryStructure implements Cloneable {
  private static final long serialVersionUID=2269192438058723953L;
  private TreeSet ts[] = new TreeSet[4];
  static Rectangle2i tmpNegRectangle[],tmpPosRectangle[];
  static {
    tmpNegRectangle = new Rectangle2i [2];
    tmpPosRectangle = new Rectangle2i [2];
    tmpNegRectangle[0] = new Rectangle2i(0,Integer.MIN_VALUE,0,Integer.MIN_VALUE);
    tmpNegRectangle[1] = new Rectangle2i(Integer.MIN_VALUE,0,Integer.MIN_VALUE,0); 
    tmpPosRectangle[0] = new Rectangle2i(0,Integer.MAX_VALUE,0,Integer.MAX_VALUE);
    tmpPosRectangle[1] = new Rectangle2i(Integer.MAX_VALUE,0,Integer.MAX_VALUE,0);
  }
  
  SecondSecondaryStructure() throws NoSuchMethodException, ClassNotFoundException {
    ts[0] = new TreeSet(new MinXComparator());
    ts[1] = new TreeSet(new MaxXComparator());
    ts[2] = new TreeSet(new MinYComparator());
    ts[3] = new TreeSet(new MaxYComparator());
  }
  public Object clone(){
    SecondSecondaryStructure clone = null;
    clone = (SecondSecondaryStructure) super.clone();
    for (int i=0; i<4; i++){
      clone.ts[i] = (TreeSet) ts[i].clone();
    }
    return (clone);
  }
  private void writeObject(java.io.ObjectOutputStream out)
    throws IOException {
    for (int i=0; i<4; i++){
      out.writeObject(ts[i]);
    }
  }
  private void readObject(java.io.ObjectInputStream in)
    throws IOException, ClassNotFoundException {
    ts = new TreeSet[4];
    for (int i=0; i<4; i++){
      ts[i] = (TreeSet) in.readObject();
    }
  }
  public Vector enclosedBy(Object r, NeededCriteria nC,boolean justGetOne){
    return (getNeededCriteria(nC));
  }
  public Vector getAll(Object ob){
    return(getNeededCriteria(new NeededCriteria()));
  }
  public Vector getNeededCriteria(NeededCriteria nC){
    Vector ret = new Vector();
    NeededCriteria.NeededCondition nCond;
    Enumeration e = nC.getAllCriteria();
    TreeSet tmpTree=null;
    int tmpTreeNum=0;
    Rectangle2i tmpRect=null;
    if (!e.hasMoreElements()){
      ret.addAll(new TreeSet(ts[0]));
    } else {
      int nCnum=0;
      while(e.hasMoreElements()){
	tmpTreeNum=0;
	nCond = (NeededCriteria.NeededCondition) e.nextElement();
	if (!(nCond.direction==0 ^ nCond.clusive)){ // less-than
	  if (nCond.dimension.getID()==1){ // x direction
	    tmpPosRectangle[0].setMinX(nCond.value);
	    tmpPosRectangle[0].setMaxX(nCond.value);
	    tmpRect=tmpPosRectangle[0];
	  } else if (nCond.dimension.getID()==2){ // y direction
	    tmpPosRectangle[1].setMinY(nCond.value);
	    tmpPosRectangle[1].setMaxY(nCond.value);
	    tmpRect=tmpPosRectangle[1];
	    tmpTreeNum+=2;
	  }
	} else {
	  if (nCond.dimension.getID()==1){ // x direction
	    tmpNegRectangle[0].setMinX(nCond.value);
	    tmpNegRectangle[0].setMaxX(nCond.value);
	    tmpRect=tmpNegRectangle[0];
	  } else if (nCond.dimension.getID()==2){ // y direction
	    tmpNegRectangle[1].setMinY(nCond.value);
	    tmpNegRectangle[1].setMaxY(nCond.value);
	    tmpRect=tmpNegRectangle[1];
	    tmpTreeNum+=2;
	  }
	}
	if (nCond.direction==0){
	  tmpTree=new TreeSet(ts[tmpTreeNum].headSet(tmpRect));
	}else {
	  tmpTreeNum++;
	  tmpTree=new TreeSet(ts[tmpTreeNum].tailSet(tmpRect));
	}
	int retnum=0;
	retnum=0;
	if (tmpTree.isEmpty()){
	  ret.clear();
	  return(ret);
	}
	if (ret.isEmpty()){
	  ret.addAll(tmpTree);
	} else {
	  ret.retainAll(tmpTree);
	  if (ret.isEmpty()){
	    return(ret);
	  }
	}
	retnum=0;
      }
    }
    int contnum=0;
    return(ret);
  }
  public Vector windowQuery(Object r,NeededCriteria nC,boolean inclusive){
    return (getNeededCriteria(nC));
  }
  public boolean add ( Object ob){
    for (int i=0; i<4; i++){
      ts[i].add(ob);
    }
    return (true);
  }
  public boolean isEmpty(){
    return(ts[0].isEmpty());
  }
  public boolean remove(Object ob){
    for (int i=0; i<4; i++){
      ts[i].remove(ob);
    }
    return (true);
  }
  public void printObjects(PrintStream ps){
    Iterator it = ts[0].iterator();
    while (it.hasNext()){
      Rectangle2i rs = (Rectangle2i)it.next();
      ps.print( "EmptySpace: " + rs + "\n");
    }
  }
  public void print(PrintStream ps){
    System.out.println("SecondSecondaryStructure.print()");
    Iterator it = ts[0].iterator();
    DecimalFormat df = new DecimalFormat("##.###");
    while (it.hasNext()){
      Rectangle2i rs = (Rectangle2i)it.next();
      ps.print( "\t" + rs + "\t");
    }
    ps.println("");
  }
  public Iterator iterator(){
    return (ts[0].iterator());
  }
}

