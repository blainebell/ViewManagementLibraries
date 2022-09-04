package edu.columbia.cs.cgui.spam2d;

//import java.awt.geom.*;
import java.util.*;
import java.text.*;
import java.io.*;
import java.lang.*;
import java.lang.reflect.*;

public class IntervalTree implements Cloneable, Serializable {
    private static final long serialVersionUID = 586384211934148777L;
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
    public Object clone(){
	//	System.out.println("IntervalTree.clone()");
	IntervalTree clone = null;
	try {
	    clone = (IntervalTree) super.clone();
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
	    nodeMinMethod = nodeClass.getMethod((String) in.readObject(),null);
	    nodeMaxMethod = nodeClass.getMethod((String) in.readObject(),null);
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

    public IntervalTree(Class nC, Method minMeth, Method maxMeth) throws ClassNotFoundException {
	this(nC,minMeth,maxMeth,Class.forName("edu.columbia.cs.cgui.spam2d.FirstSecondaryStructure"), new IntervalDimension(1,minMeth,maxMeth));
    }
    public IntervalTree(Class nC, Method minMeth, Method maxMeth,Class iC, IntervalDimension iD) throws ClassNotFoundException {

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
	ps.println("Printing IntervalTree");
	ps.println("~~~~~~~~~~~~~~~~~~~~~");
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
    public boolean isEnclosedBy(Rectangle2d r){
	boolean b=isEnclosedBy(r,new NeededCriteria());
	//	System.out.println("isEnclosedBy returned=" + b);
	return(b);
    }
    public boolean isEnclosedBy(Rectangle2d r, NeededCriteria neededCriteria){
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
		    Rectangle2d rect = new Rectangle2d();
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
		    Rectangle2d rect = new Rectangle2d();
		    rect.setRect(rmax,Double.POSITIVE_INFINITY,rmax,Double.POSITIVE_INFINITY);
		    neededCriteria.pushCriteria(intervalDimension,0 /* less-than */,inclusive, rmax);
		    //		    System.out.println("rmax=" + rmax + " correntNode.value=" + currentNode.value);
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
		    //		    System.out.println("rmin=" + rmin + " correntNode.value=" + currentNode.value);
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
		  //		    System.out.println("rmin=" + rmin + " rmax=" + rmax + " correntNode.value=" + currentNode.value);
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

    public void addRectangle(Rectangle2d rs){
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
}
