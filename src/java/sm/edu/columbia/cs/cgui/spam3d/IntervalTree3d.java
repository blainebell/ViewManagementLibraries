package edu.columbia.cs.cgui.spam3d;

import java.awt.geom.*;
import java.util.*;
import java.text.*;
import java.io.*;
import java.lang.*;
import java.lang.reflect.*;

public class IntervalTree3d implements Cloneable, Serializable {
    private static final long serialVersionUID = 981173612879519272L;
    static short MINX=0;
    static short MINY=1;
    static short MAXX=2;
    static short MAXY=3;
    static Class nodeClass;
    Class internalNodeClass;
    Method nodeMinMethod, nodeMaxMethod;
    IntervalDimension intervalDimension;
    IntervalTreeNode rootNode=null;

    public boolean isEmpty(){
	//	System.out.println("IntervalTree.isEmpty(): rootNode.secondaryStructure.isEmpty()=" + rootNode.secondaryStructure.isEmpty());
	//	print(System.out);
	
	if ((rootNode==null) || 
	    (rootNode.secondaryStructure.isEmpty() && 
	     rootNode.tertiaryStructure[0]==null &&
	     rootNode.tertiaryStructure[1]==null)){
	    //	    System.out.println("IntervalTree.isEmpty() returned true");
	    return (true);
	}
	//	System.out.println("IntervalTree.isEmpty() returned false");
	return (false);
    }
    protected Object clone(){
	//	System.out.println("IntervalTree.clone()");
	IntervalTree3d clone = null;
	try {
	    clone = (IntervalTree3d) super.clone();
	} catch (CloneNotSupportedException e) { 
	    throw new InternalError();
	}
	//	clone.nodeClass = nodeClass;
	clone.internalNodeClass = internalNodeClass;
	clone.nodeMinMethod = nodeMinMethod;
	clone.nodeMaxMethod = nodeMaxMethod;
	clone.intervalDimension = intervalDimension;
	clone.rootNode = (IntervalTreeNode)rootNode.clone();
	return (clone);
    }
    private void writeObject(java.io.ObjectOutputStream out)
	throws IOException {
	//	out.defaultWriteObject();
	//	out.writeObject(nodeClass);
	out.writeObject(internalNodeClass);
	out.writeObject(nodeMinMethod.getName());
	out.writeObject(nodeMaxMethod.getName());
	out.writeObject(intervalDimension);
	if (rootNode!=null){
	    out.writeBoolean(true);
	    out.writeObject(rootNode);
	} else {
	    out.writeBoolean(false);
	}
	//	out.writeBoolean(true);
	
    }
    private void readObject(java.io.ObjectInputStream in)
	throws IOException, ClassNotFoundException {
	//	in.defaultReadObject();
	try {
	    //	    nodeClass = (Class) in.readObject();
	    internalNodeClass = (Class) in.readObject();
	    nodeMinMethod = nodeClass.getDeclaredMethod((String) in.readObject(),null);
	    nodeMaxMethod = nodeClass.getDeclaredMethod((String) in.readObject(),null);
	} catch (NoSuchMethodException e){
	    e.printStackTrace();
	} catch (Exception e){
	    e.printStackTrace();
	}
	intervalDimension = (IntervalDimension)in.readObject();
	//	System.out.println("reading intervalDimension=" + intervalDimension);
	boolean b=in.readBoolean();
	if (b){
	    rootNode = (IntervalTreeNode) in.readObject();
	    rootNode.setOwner(this);
	}
	/*	if (!in.readBoolean()){
		System.err.println("readObject: did not work");
		};
	*/
    }

    public IntervalTree3d(Class nC, Method minMeth, Method maxMeth) throws ClassNotFoundException {
	this(nC,minMeth,maxMeth,Class.forName("edu.columbia.cs.cgui.spam3d.FirstSecondaryStructure"), new IntervalDimension(1,minMeth,maxMeth));
    }
    public IntervalTree3d(Class nC, Method minMeth, Method maxMeth,Class iC, IntervalDimension iD) throws ClassNotFoundException {

	//	System.out.println("nC=" + nC + "\tminMeth=" + minMeth + "\tmaxMeth=" +maxMeth + "\tReturn Type=" + minMeth.getReturnType().toString() + "\tminMeth.getReturnType().toString().compareTo(\"double\")=" + minMeth.getReturnType().toString().compareTo("double"));
	if (nC==null || minMeth== null || maxMeth==null || minMeth.getReturnType().toString().compareTo("double")!=0){
	    //  || Class.forName("Double").isInstance(minMeth.getReturnType())){
	    throw new IllegalArgumentException();
	}
	nodeClass = nC;
	internalNodeClass = iC;
	intervalDimension = iD;
	nodeMinMethod = minMeth;
	nodeMaxMethod = maxMeth;
    }
    public void print(PrintStream ps){
	ps.println("Printing IntervalTree3d");
	ps.println("~~~~~~~~~~~~~~~~~~~~~~~");
	if (rootNode!=null)
	    rootNode.printSubTree(ps,0);
    }

    public void debugIntervalTree(){
	IntervalTreeNode currentNode=rootNode, parrentNode=null, activeNode = null;
	//    System.out.println("spaceRectangles.debugIntervalTree");
	Stack searchStack = new Stack();
	if (currentNode!=null){
	    searchStack.push(currentNode);
	    while (!searchStack.empty()){
		currentNode = (IntervalTreeNode) searchStack.pop();
		// Checking primary Structures
		if (currentNode.primaryStructure[0]!=null){
		    if (currentNode.value <= currentNode.primaryStructure[0].value){
			System.out.println("debugIntervalTree::primaryStructure[0].value="+currentNode.primaryStructure[0].value + " >= parent currentNode.value=" + currentNode.value);
			System.err.println("debugIntervalTree::primaryStructure[0].value="+currentNode.primaryStructure[0].value + " >= parent currentNode.value=" + currentNode.value);
			print(System.out);
			print(System.err);
		    }
		}
		if (currentNode.primaryStructure[1]!=null){
		    if (currentNode.value >= currentNode.primaryStructure[1].value){
			System.out.println("debugIntervalTree::primaryStructure[1].value="+ currentNode.primaryStructure[1].value + " >= parent currentNode.value=" + currentNode.value);
			System.err.println("debugIntervalTree::primaryStructure[1].value="+ currentNode.primaryStructure[1].value + " >= parent currentNode.value=" + currentNode.value);
			print(System.out);
			print(System.err);
		    }
		}
		// Checking tertiary Structures
		if (currentNode.active){
		    if (currentNode != rootNode && currentNode.secondaryStructure.isEmpty() && ((currentNode.tertiaryStructure[0]==null) || (currentNode.tertiaryStructure[1]==null))){
			System.out.println("debugIntervalTree::currentNode.value=" + currentNode.value + " is active, but has no elements, and not both tertiaryStructures are populated");
			System.err.println("debugIntervalTree::currentNode.value=" + currentNode.value + " is active, but has no elements, and not both tertiaryStructures are populated");
			print(System.out);
			print(System.err);
		    }
		}
		if (currentNode.tertiaryStructure[0]!=null && !currentNode.tertiaryStructure[0].active){
		    System.out.println("debugIntervalTree::currentNode.value=" + currentNode.value + " currentNode.tertiaryStructure[0].value=" + currentNode.tertiaryStructure[0].value + " is set to a node that is not active");
		    System.err.println("debugIntervalTree::currentNode.value=" + currentNode.value + " currentNode.tertiaryStructure[0].value=" + currentNode.tertiaryStructure[0].value + " is set to a node that is not active");
		    print(System.out);
		    print(System.err);
		}
		if (currentNode.tertiaryStructure[0]!=null && currentNode.tertiaryStructure[0].value > currentNode.value){
		    System.out.println("debugIntervalTree::currentNode.value=" + currentNode.value + " currentNode.tertiaryStructure[0].value=" + currentNode.tertiaryStructure[0].value + " childs value is screwed up");
		    System.err.println("debugIntervalTree::currentNode.value=" + currentNode.value + " currentNode.tertiaryStructure[0].value=" + currentNode.tertiaryStructure[0].value + " childs value is screwed up");
		    print(System.out);
		    print(System.err);
		}
		if (currentNode.tertiaryStructure[0]!=null && !currentNode.isChild(currentNode.tertiaryStructure[0])){
		    System.out.println("debugIntervalTree::currentNode.value=" + currentNode.value + " currentNode.tertiaryStructure[0].value=" + currentNode.tertiaryStructure[0].value + " tertiaryStruct is not child of currentnode");
		    System.err.println("debugIntervalTree::currentNode.value=" + currentNode.value + " currentNode.tertiaryStructure[0].value=" + currentNode.tertiaryStructure[0].value + " tertiaryStruct is not child of currentnode");
		    print(System.out);
		    print(System.err);
		}
		IntervalTreeNode tmptn;
		if (currentNode.tertiaryStructure[0]!=null && (tmptn=currentNode.isActiveChildBefore(currentNode.tertiaryStructure[0]))!=null){
		    System.out.println("debugIntervalTree::currentNode.value=" + currentNode.value + " currentNode.tertiaryStructure[0].value=" + currentNode.tertiaryStructure[0].value + " has active node tmptn.value" + tmptn.value + " between them");
		    System.err.println("debugIntervalTree::currentNode.value=" + currentNode.value + " currentNode.tertiaryStructure[0].value=" + currentNode.tertiaryStructure[0].value + " has active node tmptn.value" + tmptn.value + " between them");
		    print(System.out);
		    print(System.err);
		}
		if (currentNode.tertiaryStructure[1]!=null && (tmptn=currentNode.isActiveChildBefore(currentNode.tertiaryStructure[1]))!=null){
		    System.out.println("debugIntervalTree::currentNode.value=" + currentNode.value + " currentNode.tertiaryStructure[1].value=" + currentNode.tertiaryStructure[1].value + " has active node tmptn.value" + tmptn.value + " between them");
		    System.err.println("debugIntervalTree::currentNode.value=" + currentNode.value + " currentNode.tertiaryStructure[1].value=" + currentNode.tertiaryStructure[1].value + " has active node tmptn.value" + tmptn.value + " between them");
		    print(System.out);
		    print(System.err);
		}
		
		if (currentNode.tertiaryStructure[1]!=null && !currentNode.isChild(currentNode.tertiaryStructure[1])){
		    System.out.println("debugIntervalTree::currentNode.value=" + currentNode.value + " currentNode.tertiaryStructure[0].value=" + currentNode.tertiaryStructure[1].value + " tertiaryStruct is not child of currentnode");
		    System.err.println("debugIntervalTree::currentNode.value=" + currentNode.value + " currentNode.tertiaryStructure[0].value=" + currentNode.tertiaryStructure[1].value + " tertiaryStruct is not child of currentnode");
		    print(System.out);
		    print(System.err);
		}
		
		if (currentNode.tertiaryStructure[1]!=null && currentNode.tertiaryStructure[1].value < currentNode.value){
		    System.out.println("debugIntervalTree::currentNode.value=" + currentNode.value + " currentNode.tertiaryStructure[1].value=" + currentNode.tertiaryStructure[1].value + " childs value is screwed up");
		    System.err.println("debugIntervalTree::currentNode.value=" + currentNode.value + " currentNode.tertiaryStructure[1].value=" + currentNode.tertiaryStructure[1].value + " childs value is screwed up");
		    print(System.out);
		    print(System.err);
		}
		
		if (currentNode.tertiaryStructure[1]!=null && !currentNode.tertiaryStructure[1].active){
		    System.out.println("debugIntervalTree::currentNode.value=" + currentNode.value + " currentNode.tertiaryStructure[1].value=" + currentNode.tertiaryStructure[1].value + " is set to a node that is not active");
		    System.err.println("debugIntervalTree::currentNode.value=" + currentNode.value + " currentNode.tertiaryStructure[1].value=" + currentNode.tertiaryStructure[1].value + " is set to a node that is not active");
		    print(System.out);
		    print(System.err);
		}
		
		if (currentNode.primaryStructure[0]!=null){
		    searchStack.push(currentNode.primaryStructure[0]);
		}
		if (currentNode.primaryStructure[1]!=null){
		    searchStack.push(currentNode.primaryStructure[1]);
		}
	    }
	}
    }
    public Vector getAll(Object ob){
	IntervalTreeNode currentNode = rootNode;
	Vector rectList = new Vector();
	Stack searchStack = new Stack();
	searchStack.push(currentNode);
	while (!searchStack.empty()){
	    currentNode = (IntervalTreeNode) searchStack.pop();	    
	    rectList.addAll(currentNode.secondaryStructure.getAll(ob));
	    if (currentNode.tertiaryStructure[0]!=null){
		searchStack.push(currentNode.tertiaryStructure[0]);
	    }
	    if (currentNode.tertiaryStructure[1]!=null){
		searchStack.push(currentNode.tertiaryStructure[1]);
	    }
	}
	return(rectList);
    }
    public boolean isEnclosedBy(Rectangle3d r){
	boolean b=isEnclosedBy(r,new NeededCriteria());
	//	System.out.println("isEnclosedBy returned=" + b);
	return(b);
    }
    public boolean isEnclosedBy(Rectangle3d r, NeededCriteria neededCriteria){
	return (enclosedBy(r,neededCriteria,true).size()>0);
    }
    public Vector enclosedBy(Object r){
	return (enclosedBy(r,false));
    }
    public Vector enclosedBy(Object r,boolean justGetOne){
	return (enclosedBy(r, new NeededCriteria(),justGetOne));
    }
    public Vector enclosedBy(Object r, NeededCriteria neededCriteria){
	return (enclosedBy(r,neededCriteria,false));
    }
    public Vector enclosedBy(Object r, NeededCriteria neededCriteria,boolean justGetOne){
	if (!nodeClass.isInstance(r)){
	    throw new IllegalArgumentException();
	}
	double rmin=0., rmax=0.;
	try {
	    rmin = ((Double)nodeMinMethod.invoke(r,null)).doubleValue();
	    rmax = ((Double)nodeMaxMethod.invoke(r,null)).doubleValue();
	} catch (InvocationTargetException e){
	    e.printStackTrace();
	    return (null);
	} catch (IllegalAccessException e){
	    e.printStackTrace();
	    return (null);
	}
	IntervalTreeNode currentNode = rootNode, parentNode=null,activeNode = null;
	boolean cont=false,mincomp,maxcomp;
	Vector rectList = new Vector();
	Stack searchStack = new Stack();
	
	if (currentNode!=null){
	    searchStack.push(currentNode);

	    while (!searchStack.empty()){
		currentNode = (IntervalTreeNode) searchStack.pop();
		mincomp = currentNode.value >= rmin;
		maxcomp = currentNode.value <= rmax;
		if (mincomp && maxcomp){
		    Rectangle3d rect = new Rectangle3d();
		    if (!currentNode.secondaryStructure.isEmpty()){
			neededCriteria.pushCriteria(intervalDimension,0 /* less-than */,true /* inclusive */, rmin);
			neededCriteria.pushCriteria(intervalDimension,1 /* greater-than */,true /* inclusive */, rmax);
			rectList.addAll(currentNode.secondaryStructure.enclosedBy(r,neededCriteria,justGetOne));
			neededCriteria.popCriteria();
			neededCriteria.popCriteria();
			if (justGetOne && rectList.size()>0){
			    return (rectList);
			}
		    }
		} else if (mincomp){
		    if (!currentNode.secondaryStructure.isEmpty()){
			neededCriteria.pushCriteria(intervalDimension,0 /* less-than */,true /* inclusive */, rmin);
			rectList.addAll(currentNode.secondaryStructure.enclosedBy(r,neededCriteria,justGetOne));
			neededCriteria.popCriteria();
			if (justGetOne && rectList.size()>0){
			    return (rectList);
			}
		    }
		    if (currentNode.tertiaryStructure[0]!=null){
			searchStack.push(currentNode.tertiaryStructure[0]);
		    }
		} else if (maxcomp){
		    if (!currentNode.secondaryStructure.isEmpty()){
			neededCriteria.pushCriteria(intervalDimension,1 /* greater-than */,true /* inclusive */, rmax);    
			rectList.addAll(currentNode.secondaryStructure.enclosedBy(r,neededCriteria,justGetOne));
			neededCriteria.popCriteria();
			
			if (justGetOne && rectList.size()>0){
			    return (rectList);
			}
		    }
		    if (currentNode.tertiaryStructure[1]!=null){
			searchStack.push(currentNode.tertiaryStructure[1]);
		    }
		}
	    }
	}
	return (rectList);
    }
    
    public Vector windowQueryWithoutEdges(Object r){
	return (windowQuery(r,new NeededCriteria(),false));
    }
    public Vector windowQueryWithoutEdges(Object r,NeededCriteria nC){
	return (windowQuery(r,nC,false));
    }
    public Vector windowQuery(Object r){
	return (windowQuery(r,new NeededCriteria(),true));
    }
    public Vector windowQuery(Object r,NeededCriteria nC){
	return (windowQuery(r,nC,true));
    }
    public Vector windowQuery(Object r, NeededCriteria neededCriteria, boolean inclusive){
	if (!nodeClass.isInstance(r)){
	    throw new IllegalArgumentException();
	}
	double rmin=0., rmax=0.;
	try {
	    rmin = ((Double)nodeMinMethod.invoke(r,null)).doubleValue();
	    rmax = ((Double)nodeMaxMethod.invoke(r,null)).doubleValue();
	} catch (InvocationTargetException e){
	    e.printStackTrace();
	    return (null);
	} catch (IllegalAccessException e){
	    e.printStackTrace();
	    return (null);
	}
	IntervalTreeNode currentNode = rootNode, parentNode=null,activeNode = null;
	Vector rectList = new Vector();
	Stack searchStack = new Stack();
	if (currentNode!=null){
	    searchStack.push(currentNode);
	}
	while (!searchStack.empty()){
	    currentNode = (IntervalTreeNode) searchStack.pop();

	    if (rmax <= currentNode.value){
		// If   : the point (currentNode.value) is to the right of the query rectangle,
		// Then : get all rectangles in secondary structure in which their minimum value is less than
		//        the maximum value of the query rectangle
		if (!currentNode.secondaryStructure.isEmpty()){
		    Rectangle3d rect = new Rectangle3d();
		    rect.setRect(rmax,Double.POSITIVE_INFINITY,Double.POSITIVE_INFINITY,
				 rmax,Double.POSITIVE_INFINITY,Double.POSITIVE_INFINITY);
		    neededCriteria.pushCriteria(intervalDimension,0 /* less-than */,inclusive, rmax);
		    rectList.addAll(currentNode.secondaryStructure.windowQuery(r,neededCriteria,inclusive));
		    neededCriteria.popCriteria();
		}
		if (currentNode.tertiaryStructure[0]!=null){
		    searchStack.push(currentNode.tertiaryStructure[0]);
		}
	    } else if (rmin >= currentNode.value){
		// If   : the point (currentNode.value) is to the left/adjacent of the query rectangle,
		// Then : get all rectangles in secondary structure in which their maximum value is greater than
		//        the minimum value of the query rectangle
		if (!currentNode.secondaryStructure.isEmpty()){
		    neededCriteria.pushCriteria(intervalDimension,1 /* greater-than */,inclusive, rmin);    
		    rectList.addAll(currentNode.secondaryStructure.windowQuery(r,neededCriteria,inclusive));
		    neededCriteria.popCriteria();
		}
		if (currentNode.tertiaryStructure[1]!=null){
		    searchStack.push(currentNode.tertiaryStructure[1]);
		}
	    } else {
		// If   : the point (currentNode.value) is inside the query rectangle, 
		// then : add all rectangles in secondary structure 
		if (!currentNode.secondaryStructure.isEmpty()){
		    rectList.addAll(currentNode.secondaryStructure.windowQuery(r,neededCriteria,inclusive));
		}
		if (currentNode.tertiaryStructure[0]!=null)
		    searchStack.push(currentNode.tertiaryStructure[0]);
		if (currentNode.tertiaryStructure[1]!=null)
		    searchStack.push(currentNode.tertiaryStructure[1]);
	    }
	}
	return(rectList);
    }

    public void addRectangle(Rectangle3d rs){
	IntervalTreeNode currentNode = rootNode, parentNode=null,activeNode = null, nonActiveNode = null;
	int whichWayTertiary=0, whichWayNoneActive=0, whichWayPrimary=0;
	double rmin=0., rmax=0.,rcenter=0.;
	try {
	    rmin = ((Double)nodeMinMethod.invoke(rs,null)).doubleValue();
	    rmax = ((Double)nodeMaxMethod.invoke(rs,null)).doubleValue();
	    rcenter = rmin+ (.5*(rmax-rmin));
	} catch (InvocationTargetException e){
	    e.printStackTrace();
	    return ;
	} catch (IllegalAccessException e){
	    e.printStackTrace();
	    return ;
	}
	if (rootNode == null){
	    rootNode = new IntervalTreeNode(rcenter,this);
	    currentNode = rootNode;
	}
	boolean contentWithNonActiveNode = false;
	while ( currentNode != null){
	    if (rmax < currentNode.value){
		if (currentNode.primaryStructure[0]==null){
		    currentNode.primaryStructure[0] =  new IntervalTreeNode(rcenter,this);
		}
		if (currentNode.active){
		    activeNode = currentNode;
		    whichWayTertiary = 0;
		    nonActiveNode = null;
		} else {
		    IntervalTreeNode adsf = activeNode.tertiaryStructure[whichWayTertiary];
		    if (nonActiveNode==null){
			nonActiveNode = currentNode;
			whichWayNoneActive = 0;
			contentWithNonActiveNode=false;
		    } 
		    if (!contentWithNonActiveNode && adsf!=null && (currentNode.value < adsf.value)){
			contentWithNonActiveNode = true;
			nonActiveNode = currentNode;
			whichWayNoneActive = 0;
		    }
		}
		currentNode = currentNode.primaryStructure[0];
		whichWayPrimary = 0;
	    } else if (rmin > currentNode.value){
		if (currentNode.primaryStructure[1]==null){
		    currentNode.primaryStructure[1] =  new IntervalTreeNode(rcenter,this);
		}
		if (currentNode.active){
		    activeNode = currentNode;
		    whichWayTertiary = 1;
		    nonActiveNode = null;
		} else {
		    IntervalTreeNode adsf = activeNode.tertiaryStructure[whichWayTertiary];
		    if (nonActiveNode==null){
			nonActiveNode = currentNode;
			whichWayNoneActive = 1;
			contentWithNonActiveNode=false;
		    } 
		    if (!contentWithNonActiveNode && adsf!=null && (currentNode.value > adsf.value)){
			contentWithNonActiveNode = true;
			nonActiveNode = currentNode;
			whichWayNoneActive = 1;
		    }
		}
		currentNode = currentNode.primaryStructure[1];
		whichWayPrimary = 1;
	    } else {  // Inserting rectangle to a node
		if (!currentNode.active){
		    IntervalTreeNode activeNodePtr =null;
		    if (activeNode!=null){
			activeNodePtr = activeNode.tertiaryStructure[whichWayTertiary];
		    }
		    if (activeNode==rootNode && activeNodePtr==null){
			activeNode.tertiaryStructure[whichWayTertiary] = currentNode;
		    } else if (nonActiveNode!=null && activeNodePtr!=null && ((activeNodePtr.value>nonActiveNode.value) != (currentNode.value > nonActiveNode.value))){ // we know a node has to be active b/c the root is always active
			nonActiveNode.setActive(true);
			nonActiveNode.tertiaryStructure[(whichWayNoneActive+1)%2] = activeNode.tertiaryStructure[whichWayTertiary];
			activeNode.tertiaryStructure[whichWayTertiary] = nonActiveNode;
			nonActiveNode.tertiaryStructure[whichWayNoneActive] = currentNode;
		    } else if (activeNode!=null){
			if (activeNode.tertiaryStructure[whichWayTertiary]!=null){
			    int asdf = activeNode.tertiaryStructure[whichWayTertiary].value > currentNode.value ? 1 : 0; 
			    currentNode.tertiaryStructure[asdf] = activeNode.tertiaryStructure[whichWayTertiary];
			}
			activeNode.tertiaryStructure[whichWayTertiary] = currentNode;
		    }
		} 
		currentNode.addRectangle(rs);
		return;
	    }
	}
    }
    public boolean deleteRectangle(Object r){
	if (!nodeClass.isInstance(r)){
	    throw new IllegalArgumentException();
	}
	double rmin=0., rmax=0.;
	try {
	    rmin = ((Double)nodeMinMethod.invoke(r,null)).doubleValue();
	    rmax = ((Double)nodeMaxMethod.invoke(r,null)).doubleValue();
	} catch (InvocationTargetException e){
	    e.printStackTrace();
	    return (false);
	} catch (IllegalAccessException e){
	    e.printStackTrace();
	    return (false);
	}
	IntervalTreeNode itn = rootNode, parent=null, grandparent = null;
	boolean deleted = false;
	int whichWay=0,whichWayGrand=0;
	Stack stack = new Stack();
	while (itn!= null && !deleted){
	    stack.push(itn);
	    if (rmax < itn.value){
		grandparent = parent;
		parent = itn;
		itn = itn.tertiaryStructure[0];
		whichWayGrand=whichWay;
		whichWay=0;
	    } else if (rmin > itn.value){
		grandparent = parent;
		parent = itn;
		itn = itn.tertiaryStructure[1];
		whichWayGrand=whichWay;
		whichWay=1;
	    } else {
		deleted = itn.secondaryStructure.remove(r);
		if (!deleted){
		    itn.secondaryStructure.print(System.out);
		    System.out.println("IntervalTree.deleteRectangle didn't delete");
		    System.exit(-1);
		}
	    }
	}
	if (deleted){
	    boolean ts0, ts1;
	    if (itn.secondaryStructure.isEmpty() && itn != rootNode){
		ts0 = itn.tertiaryStructure[0]==null;
		ts1 = itn.tertiaryStructure[1]==null;
		
		// When the tertiary node is a leaf, make it inactive and check parent to make sure 
		// it totally doesn't depend on this node (if it is active with no rectangles)
		if (ts0 && ts1){
		    itn.setActive(false);
		    parent.tertiaryStructure[whichWay] = null;
		    
		    IntervalTreeNode itn2 = parent.tertiaryStructure[(whichWay +1)%2];
		    itn = parent;
		    if (parent.secondaryStructure.isEmpty() && parent!=rootNode){ // && itn2!=null){
			parent.setActive(false);
			grandparent.tertiaryStructure[whichWayGrand] = itn2;
		    }
		    // if node that is becoming inactive has one tertiary child,
		    // set the parent 
		} else if (ts0 ^ ts1){ 
		    if (ts0){
			parent.tertiaryStructure[whichWay] = itn.tertiaryStructure[1];
			itn.setActive(false);
		    } else {
			parent.tertiaryStructure[whichWay] = itn.tertiaryStructure[0];
			itn.setActive(false);
		    }
		} 
	    }
	} else {
	}
	return deleted;
    }
    private class IntervalTreeNode implements Cloneable, Serializable {
	private static final long serialVersionUID = -1140132654438177644L;
	boolean active;
	double value;
	IntervalTreeNode primaryStructure[];
	SecondaryStructure secondaryStructure;
	IntervalTreeNode tertiaryStructure[];
	IntervalTree3d owner;
	//	IntervalTreeNode parent;
	public void setOwner(IntervalTree3d own){
	    owner = own;
	    if (primaryStructure[0]!=null){
		primaryStructure[0].setOwner(owner);
	    }
	    if (primaryStructure[1]!=null){
		primaryStructure[1].setOwner(owner);
	    }
	}
	private void writeObject(java.io.ObjectOutputStream out)
	    throws IOException {
	    out.writeBoolean(active);
	    out.writeDouble(value);
	    if (tertiaryStructure[0]!=null){
		out.writeBoolean(true);
		out.writeDouble(tertiaryStructure[0].value);
	    } else {
		out.writeBoolean(false);
	    }
	    if (tertiaryStructure[1]!=null){
		out.writeBoolean(true);
		out.writeDouble(tertiaryStructure[1].value);
	    } else {
		out.writeBoolean(false);
	    }
	    out.writeObject(secondaryStructure);
	    if (primaryStructure[0]==null){
		out.writeBoolean(false);
	    } else {
		out.writeBoolean(true);
	    }
	    if (primaryStructure[1]==null){
		out.writeBoolean(false);
	    } else {
		out.writeBoolean(true);
	    }
	    if (primaryStructure[0]!=null){
		out.writeObject(primaryStructure[0]);
	    }
	    if (primaryStructure[1]!=null){
		out.writeObject(primaryStructure[1]);
	    }
	}
	private void readObject(java.io.ObjectInputStream in)
	    throws IOException, ClassNotFoundException {
	    active = in.readBoolean();
	    value = in.readDouble();
	    boolean tmp1,tmp2;
	    double tert1=0.,tert2=0.;
	    tmp1 = in.readBoolean();
	    if (tmp1){
		tert1 = in.readDouble();
	    }
	    tmp2 = in.readBoolean();
	    if (tmp2){
		tert2 = in.readDouble();
	    }
	    //	    System.out.println("owner=" + this);
	    secondaryStructure = (SecondaryStructure)in.readObject();
	    primaryStructure = new IntervalTreeNode[2];
	    tertiaryStructure = new IntervalTreeNode[2];
	    primaryStructure[0] = primaryStructure[1] = null;
	    boolean p1,p2;
	    p1 = in.readBoolean();
	    p2 = in.readBoolean();
	    if (p1){
		try {
		    primaryStructure[0] = (IntervalTreeNode) in.readObject();
		} catch (OptionalDataException ode){
		}
	    }
	    if (p2){
		try {
		    primaryStructure[1] = (IntervalTreeNode) in.readObject();
		} catch (OptionalDataException ode){
		}
	    }
	    if (tmp1){
		tertiaryStructure[0] = getNode(tert1);
	    }
	    if (tmp2){
		tertiaryStructure[1] = getNode(tert2);
	    }
	}

	protected Object clone(){
	    IntervalTreeNode clone = null;
	    try {
		clone = (IntervalTreeNode) super.clone();
	    } catch (CloneNotSupportedException e) { 
		throw new InternalError();
	    }
	    clone.active = active;
	    clone.value = value;
	    clone.primaryStructure = new IntervalTreeNode[2];
	    clone.tertiaryStructure = new IntervalTreeNode[2];
	    
	    if (primaryStructure[0]!=null){
		clone.primaryStructure[0] = (IntervalTreeNode) primaryStructure[0].clone();
	    }
	    if (primaryStructure[1]!=null){
		clone.primaryStructure[1] = (IntervalTreeNode) primaryStructure[1].clone();
	    }
	    clone.secondaryStructure = (SecondaryStructure)secondaryStructure.clone();
	    clone.tertiaryStructure[0] = null;
	    clone.tertiaryStructure[1] = null;
	    if (tertiaryStructure[0]!=null){
		clone.tertiaryStructure[0] = clone.primaryStructure[0].getNode(tertiaryStructure[0]);
	    }
	    if (tertiaryStructure[1]!=null){
		clone.tertiaryStructure[1] = clone.primaryStructure[1].getNode(tertiaryStructure[1]);
	    }
	    return(clone);
	}
	IntervalTreeNode getNode(double num){
	    if (num==value){
		return(this);
	    }
	    if (num>value){
		if (primaryStructure[1]==null){
		    return (null);
		}
		return(primaryStructure[1].getNode(num));
	    } else {
		if (primaryStructure[0]==null){
		    return (null);
		}
		return(primaryStructure[0].getNode(num));
	    }
	}
	IntervalTreeNode getNode(IntervalTreeNode itn){
	    if (itn==null){
		return(null);
	    }
	    if (itn.value==value){
		return(this);
	    }
	    if (itn.value>value){
		return(primaryStructure[1].getNode(itn));
	    } else {
		return(primaryStructure[0].getNode(itn));
	    }
	}
	IntervalTreeNode(double x,IntervalTree3d it) {
	    value = x;
	    owner = it;
	    primaryStructure = new IntervalTreeNode[2];
	    primaryStructure[0] = null;
	    primaryStructure[1] = null;
	    try {
		secondaryStructure = (SecondaryStructure)owner.internalNodeClass.newInstance();
	    } catch (IllegalAccessException e){
		e.printStackTrace();
	    } catch (InstantiationException e){
		e.printStackTrace();
	    }
	    tertiaryStructure = new IntervalTreeNode[2];
	    setActive(false);
	}
	void setActive(boolean t){
	    active = t;
	    if (!active){
		tertiaryStructure[0] = null;
		tertiaryStructure[1] = null;
	    }
	}
	public IntervalTreeNode isActiveChildBefore(IntervalTreeNode itn){
	    IntervalTreeNode ret;
	    if (itn==null){
		return (null);
	    } 
	    if (itn.value < this.value){
		if (primaryStructure[0]==itn){
		    return(null);
		} else if (primaryStructure[0]==null){
		    return(null);
		} else if (primaryStructure[0].active){
		    return (primaryStructure[0]);
		} else {
		    return (primaryStructure[0].isActiveChildBefore(itn));
		}
	    } else {
		if (primaryStructure[1]==itn){
		    return(null);
		} else if (primaryStructure[1]==null){
		    return(null);
		} else if (primaryStructure[1].active){
		    return (primaryStructure[1]);
		} else {
		    return (primaryStructure[1].isActiveChildBefore(itn));
		}
	    }
	}
	
	public boolean isChild(IntervalTreeNode itn){
	    boolean ret=false;
	    if (itn==null){
		return (false);
	    } else if (itn==primaryStructure[0] || itn==primaryStructure[1]){
		return (true);
	    }
	    if (primaryStructure[0]!=null){
		ret = primaryStructure[0].isChild(itn);
	    }
	    if (primaryStructure[1]!=null){
		ret |= primaryStructure[1].isChild(itn);
	    }
	    return (ret);
	}
	
	public void printNode(PrintStream ps){
	    ps.print(value);
	    
	    if (active){
		ps.print(" *");
		if (tertiaryStructure[0]!=null){
		    ps.print(" 0: " + tertiaryStructure[0].value);
		}
		if (tertiaryStructure[1]!=null){
		    ps.print(" 1: " + tertiaryStructure[1].value);
		}
		ps.print(" - \n");
		System.out.println("secondaryStructure intervalDimension="+ owner.intervalDimension +"\tvalue=" + value + ":");
		secondaryStructure.print(ps);
	    }
	    ps.println("");
	}
	public void printSubTree(PrintStream ps, int depth){
	    if (primaryStructure[0]!=null)
		primaryStructure[0].printSubTree(ps,depth+1);
	    for (int i=0; i<depth; i++){
		ps.print("\t");
	    }
	    printNode(ps);
	    
	    if (primaryStructure[1]!=null)
		primaryStructure[1].printSubTree(ps,depth+1);
	}
	public void addRectangle (Rectangle3d r){
	    secondaryStructure.add(r);
	    active = true;
	}
	public IntervalTreeNode getFirstActiveNode(){
	    Vector queue = new Vector();
	    queue.add(0,this);
	    IntervalTreeNode itn;
	    while (!queue.isEmpty()){
		itn = (IntervalTreeNode) queue.firstElement();
		if (itn.active)
		    return (itn);
		if (itn.primaryStructure[0]!=null)
		    queue.add(0,itn.primaryStructure[0]);
		if (itn.primaryStructure[1]!=null)
		    queue.add(0,itn.primaryStructure[1]);
	    }
	    return (null);
	}
    }
}
