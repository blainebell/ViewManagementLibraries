package edu.columbia.cs.cgui.spam2d;

import java.io.*;
import java.util.*;

class IntervalTreeNode implements Cloneable, Serializable {
    private static final long serialVersionUID = -1140132654438177644L;
    boolean active;
    double value;
    IntervalTreeNode primaryStructure[];
    SecondaryStructure secondaryStructure;
    IntervalTreeNode tertiaryStructure[];
    IntervalTree owner;
    //	IntervalTreeNode parent;
    public void setOwner(IntervalTree own){
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
    IntervalTreeNode(double x,IntervalTree it) {
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
    public void addRectangle (Rectangle2d r){
	secondaryStructure.add(r);
	active = true;
    }
    public IntervalTreeNode getFirstActiveNode(){
	Vector queue = new Vector();
	queue.insertElementAt(this,0);
	IntervalTreeNode itn;
	while (!queue.isEmpty()){
	    itn = (IntervalTreeNode) queue.firstElement();
	    if (itn.active)
		return (itn);
	    if (itn.primaryStructure[0]!=null)
		queue.insertElementAt(itn.primaryStructure[0],0);
	    if (itn.primaryStructure[1]!=null)
		queue.insertElementAt(itn.primaryStructure[1],0);
	}
	return (null);
    }
}
