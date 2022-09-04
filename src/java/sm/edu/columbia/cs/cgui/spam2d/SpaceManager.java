package edu.columbia.cs.cgui.spam2d;

import java.awt.geom.*;
import java.util.*;
import java.text.*;
import java.awt.*;
import java.lang.reflect.*;
import java.io.*;

public class SpaceManager implements Cloneable, Serializable {
    private static final long serialVersionUID = 823073100499990844L;
    public IntervalTree spaceRectangles;
    private IntervalTree objectRectangles;
    private TreeSet objectTrees[] = new TreeSet[4];
    public TreeSet spaceTrees;
    private Vector objectVector;
    transient private Hashtable objectHash;
    public Rectangle2d screenSpace;
    private static int TOP=3;
    private static int BOTTOM=1;
    private static int RIGHT=0;
    private static int LEFT=2;
    private boolean debug=true;
    private boolean useObjectVector=true;
    private long maxLong=-1;
    int processing;
    private Stack redoList=null;
    private boolean calcRedoList=false;
    private RedoAction currentRedoAction=null;
    public class RedoAction {
	public FullRectangle2d rectangle;
	public Vector addedSpaces;
	public Vector removedSpaces;
	RedoAction(FullRectangle2d fr){
	    rectangle=fr;
	    addedSpaces=new Vector();
	    removedSpaces=new Vector();
	}
    }
    private int doRedoAction(RedoAction ra){
	Rectangle2d r=null;
	for (Enumeration e=ra.removedSpaces.elements();e.hasMoreElements();){
	    r=(Rectangle2d)e.nextElement();
	    deleteSpaceRectangle(r);
	    //	    System.out.println("doRedoAction: deleted r=" + r);
	}
	for (Enumeration e=ra.addedSpaces.elements();e.hasMoreElements();){
	    r=(Rectangle2d)e.nextElement();
	    addSpaceRectangleWithoutCheck(r);
	    //	    System.out.println("doRedoAction: added r=" + r);
	}
	return(deleteObjectRectangle(ra.rectangle));
    }
    public int undoTopRedoList(){
	if (redoList==null){
	    return (-1);
	}
	RedoAction ra=null;
	if (!redoList.empty()){
	    ra = (RedoAction) redoList.pop();
	    return(doRedoAction(ra));
	}
	return (-1);
    }
    public Vector undoAllRedoList(){ // returns vector of FullRectangles
	if (redoList==null){
	    return null;
	}
	Vector ret=new Vector();
	RedoAction ra=null;
	Rectangle2d r=null;
	while (!redoList.empty()){
	    ra = (RedoAction) redoList.pop();
	    doRedoAction(ra);
	    ret.add(ra.rectangle);
	}
	redoList=null;
	return (ret);
    }
    public void setRedoList(long l[]){ // Vector of FullRectangle IDs
	Vector v = new Vector ();
	FullRectangle2d fr=null;
	for (int i=0; i<l.length; i++){
	    fr=(FullRectangle2d) objectHash.get(new Long(l[i]));
	    v.add(fr);
	    deleteRectangle(fr);
	}
	addRectanglesCalcRedoList(v);
    }
    public void deleteAllRectangles(Vector v){
	FullRectangle2d fr=null;
	for (Enumeration e=v.elements(); e.hasMoreElements();){
	    fr = (FullRectangle2d)e.nextElement();
	    deleteRectangle(fr);
	}
    }
    public void addRectanglesCalcRedoList(Vector v){
	redoList = new Stack();
	FullRectangle2d fr=null;
	for (Enumeration e=v.elements(); e.hasMoreElements();){
	    fr = (FullRectangle2d)e.nextElement();
	    currentRedoAction = new RedoAction(fr);
	    calcRedoList=true;
	    addRectangle(fr,-1);
	    calcRedoList=false;
	    redoList.push(currentRedoAction);
	}
	currentRedoAction = null;
    }
    public void setDebug(boolean d){
	if (d && !debug){ // changing to debug mode, need to build object trees 
	    for (Enumeration e=objectVector.elements(); e.hasMoreElements();){
		Rectangle2d tmpRect = (Rectangle2d)e.nextElement();
		objectTrees[0].add(tmpRect);
		objectTrees[1].add(tmpRect);
		objectTrees[2].add(tmpRect);
		objectTrees[3].add(tmpRect);
	    }
	} else if (!d && debug){ // turn debug mode off, delete everything in object trees
	    objectTrees[0] = new TreeSet(new MinXComparator());
	    objectTrees[1] = new TreeSet(new MinYComparator());
	    objectTrees[2] = new TreeSet(new MaxXReverseComparator());
	    objectTrees[3] = new TreeSet(new MaxYReverseComparator());
	}
	debug = d;
    }
    public void setUseObjectVector(boolean t){
	useObjectVector=t;
    }
    protected Object clone(){
	SpaceManager clone = null;
	try {
	    clone = (SpaceManager) super.clone();
	} catch (CloneNotSupportedException e) { 
	    throw new InternalError();
	}
	clone.spaceRectangles = (IntervalTree)spaceRectangles.clone();
	clone.objectRectangles = (IntervalTree)objectRectangles.clone();
	for (int i=0; i<4; i++){
	    clone.objectTrees[i] = objectTrees[i];
	}
	clone.spaceTrees = (TreeSet)spaceTrees.clone();
	clone.objectVector = (Vector)objectVector.clone();
	clone.screenSpace = (Rectangle2d)screenSpace.clone();
	clone.debug = debug;
	clone.processing = processing;
	clone.rectClass = rectClass;
	clone.minxMeth = minxMeth;
	clone.maxxMeth = maxxMeth;
	clone.ss = ss;
	clone.iD = iD;

	return (clone);
    }
    
    private void writeObject(java.io.ObjectOutputStream out)
	throws IOException {
	out.writeInt(processing);
	out.writeBoolean(debug);
	out.writeObject(screenSpace);
	out.writeObject(spaceRectangles);
	out.writeObject(objectRectangles);
	out.writeObject(objectHash);
	for (int i=0; i<4; i++){
	    out.writeObject(objectTrees[i]);
	}
	out.writeObject(spaceTrees);
	out.writeObject(objectVector);
    }
    
    private void readObject(java.io.ObjectInputStream in)
	throws IOException, ClassNotFoundException {
	try {
	    rectClass = Class.forName("edu.columbia.cs.cgui.spam2d.Rectangle2d");
	    minxMeth = rectClass.getDeclaredMethod("getMinX",null);
	    maxxMeth = rectClass.getDeclaredMethod("getMaxX",null);
	    ss = Class.forName("edu.columbia.cs.cgui.spam2d.FirstSecondaryStructure");
	    iD = new IntervalDimension(1,minxMeth,maxxMeth);
	} catch (Exception e){
	    e.printStackTrace();
	}
	processing = in.readInt();
	if (processing>0) {
	    System.err.println("Warning readObject: processing=" + processing);
	    processing=0;
	}
	debug = in.readBoolean();
	screenSpace = (Rectangle2d) in.readObject();
	spaceRectangles = (IntervalTree) in.readObject();
	objectRectangles = (IntervalTree) in.readObject();
	objectHash = (Hashtable) in.readObject();
	objectTrees = new TreeSet[4];
	for (int i=0; i<4; i++){
	    objectTrees[i] = (TreeSet) in.readObject();
	}
	spaceTrees = (TreeSet) in.readObject();
	useObjectVector=true;
	objectVector = (Vector) in.readObject();
	for (Enumeration enum=objectVector.elements(); enum.hasMoreElements();){
	    FullRectangle2d fs=(FullRectangle2d)enum.nextElement();
	    if (fs.id.longValue()>maxLong){
		maxLong=fs.id.longValue();
	    }
	}
	debug=false;
    }
    int numberSpaces(){
	return (spaceTrees.size());
    }
    void printObjectRectangles(){
	objectRectangles.print(System.err);
    }
    void printSpaceRectangles(){
	spaceRectangles.print(System.err);
    }
    Class rectClass=null;
    Method minxMeth=null,maxxMeth=null;
    Class ss = null;
    IntervalDimension iD = null;
    
    public SpaceManager(Rectangle2d rs) throws NoSuchMethodException, ClassNotFoundException {
	rectClass = Class.forName("edu.columbia.cs.cgui.spam2d.Rectangle2d");
	minxMeth = rectClass.getDeclaredMethod("getMinX",null);
	maxxMeth = rectClass.getDeclaredMethod("getMaxX",null);
	ss = Class.forName("edu.columbia.cs.cgui.spam2d.FirstSecondaryStructure");
	iD = new IntervalDimension(1,minxMeth,maxxMeth);
	
	spaceRectangles = new IntervalTree(rectClass,minxMeth,maxxMeth,ss,iD);
	objectRectangles = new IntervalTree(rectClass,minxMeth,maxxMeth,ss,iD);
	spaceTrees = new TreeSet(new AreaComparator());
	objectTrees[0] = new TreeSet(new MinXComparator());
	objectTrees[1] = new TreeSet(new MinYComparator());
	objectTrees[2] = new TreeSet(new MaxXReverseComparator());
	objectTrees[3] = new TreeSet(new MaxYReverseComparator());
	objectVector = new Vector();
	objectHash = new Hashtable();
	Rectangle2d init = new Rectangle2d(rs);
	spaceRectangles.addRectangle(init);
	screenSpace = new Rectangle2d(init);
	spaceTrees.add(init);
	processing=0;
    }
    public boolean debugSpaceManager(){
	Iterator i = spaceTrees.iterator();
	boolean failed=false;
	int rectNum = 1;
	Rectangle2d rect;
	Rectangle2d rect1;
	objectRectangles.debugIntervalTree();
	spaceRectangles.debugIntervalTree();
	while (i.hasNext()){
	    boolean Xmin,Ymin, Xmax, Ymax;
	    Xmin = Ymin = Xmax = Ymax = false;
	    rect = (Rectangle2d)i.next();
	    Iterator i1 = objectRectangles.windowQuery(rect).iterator();
	    Vector enclosed = spaceRectangles.enclosedBy(rect);
	    if (enclosed.size()==0 || enclosed.size()>1 || !((Rectangle2d)enclosed.firstElement()).equals(rect)){
		Iterator e1 = enclosed.iterator();
		System.out.println("debugSpaceManager::Rectangle #" + rectNum + ": enclosed.size()=" + enclosed.size());
		System.err.println("debugSpaceManager::Rectangle #" + rectNum + ": enclosed.size()=" + enclosed.size());
		int a=1;
		while(e1.hasNext()){
		    Rectangle2d q=(Rectangle2d) e1.next();
		    System.out.println("debugSpaceManager::\t#" + a + ": "+ q);
		    a++;
		}
		failed =true;
	    }
	    while (i1.hasNext() && (!Xmin || !Ymin || !Xmax || !Ymax)){
		rect1 = (Rectangle2d)i1.next();
		if (!Xmin && rect.getMinX()==rect1.getMaxX()){
		    Xmin = true;
		}
		if(!Ymin && rect.getMinY()==rect1.getMaxY()){
		    Ymin = true;
		}
		if (!Xmax && rect.getMaxX()==rect1.getMinX()){
		    Xmax = true;
		}
		if (!Ymax && rect.getMaxY()==rect1.getMinY()){
		    Ymax = true;
		}
	    }
	    if (!Xmin && rect.getMinX()==screenSpace.getMinX()){
		Xmin = true;
	    }
	    if(!Ymin && rect.getMinY()==screenSpace.getMinY()){
		Ymin = true;
	    }
	    if (!Xmax && rect.getMaxX()==screenSpace.getMaxX()){
		Xmax = true;
	    }
	    if (!Ymax && rect.getMaxY()==screenSpace.getMaxY()){
		Ymax = true;
	    }
	    if (!Xmin || !Ymin || !Xmax || !Ymax){
		System.out.println("debugSpaceManager:: rectangle #" + rectNum + " does not have any shared sides and does not touch the edges of the screen Xmin=" + Xmin + "\tXmax=" + Xmax + "\tYmin=" + Ymin + "\tYmax=" + Ymax);
		System.err.println("debugSpaceManager:: rectangle #" + rectNum + " does not have any shared sides and does not touch the edges of the screen Xmin=" + Xmin + "\tXmax=" + Xmax + "\tYmin=" + Ymin + "\tYmax=" + Ymax);
		failed = true;
	    }
	    rectNum++;
	}
	
	Iterator oi = objectTrees[0].iterator();
	TreeSet tsx=new TreeSet(), tsy=new TreeSet();
	Rectangle2d rect2;
	tsx.add(new Double(screenSpace.getMinX()));
	tsx.add(new Double(screenSpace.getMaxX()));
	tsy.add(new Double(screenSpace.getMinY()));
	tsy.add(new Double(screenSpace.getMaxY()));
	while (oi.hasNext()){
	    rect2 = (Rectangle2d)oi.next();
	    if (rect2.getMinX() > screenSpace.getMinX() && rect2.getMinX() < screenSpace.getMaxX())
		tsx.add(new Double(rect2.getMinX()));
	    if (rect2.getMaxX() > screenSpace.getMinX() && rect2.getMaxX() < screenSpace.getMaxX())
		tsx.add(new Double(rect2.getMaxX()));
	    if (rect2.getMinY() > screenSpace.getMinY() && rect2.getMinY() < screenSpace.getMaxY())
		tsy.add(new Double(rect2.getMinY()));
	    if (rect2.getMaxY() > screenSpace.getMinY() && rect2.getMaxY() < screenSpace.getMaxY())
		tsy.add(new Double(rect2.getMaxY()));
	}

	double minx, miny, maxx, maxy,px,py;
	Iterator keepy = tsy.iterator();
	maxy = ((Double)keepy.next()).doubleValue();

	while (keepy.hasNext()){
	    miny = maxy;
	    maxy = ((Double)keepy.next()).doubleValue();
	    
	    Iterator keepx = tsx.iterator();
	    maxx = ((Double)keepx.next()).doubleValue();
	    
	    while (keepx.hasNext()){
		minx = maxx;
		maxx = ((Double)keepx.next()).doubleValue();
		px = minx + ((maxx-minx)/2.);
		py = miny + ((maxy-miny)/2.);
		Rectangle2d rect3 = new Rectangle2d(px,py,px,py);
		if (spaceRectangles.windowQuery(rect3).size()==0 && objectRectangles.windowQuery(rect3).size()==0){
		    System.out.println("debugSpaceManager::position (" + px + "," + py + ") is not covered by a space in the list");
		    System.err.println("debugSpaceManager::position (" + px + "," + py + ") is not covered by a space in the list");
		    failed = true;
		}
	    }
	}
	return(failed);
    }
    public void printAllRectangles(){
	Iterator i1 = objectVector.iterator();
	Rectangle2d rect1;
	int objectNum=1;
	while (i1.hasNext()){
	    rect1 = (Rectangle2d)i1.next();
	    System.out.println("Object Rectangle #" + objectNum + ": " + rect1);
	    System.err.println("Object Rectangle #" + objectNum + ": " + rect1);
	    objectNum++;
	}
	i1 = spaceTrees.iterator();
	int spaceNum=1;
	while (i1.hasNext()){
	    rect1 = (Rectangle2d)i1.next();
	    System.out.println("Space Rectangle #" + spaceNum + ": " + rect1 + " area=" + rect1.area());
	    System.err.println("Space Rectangle #" + spaceNum + ": " + rect1 + " area=" + rect1.area());
	    spaceNum++;
	}
    }
    /* Either a solid is on the side of the rectangle, or it is on the edge of the Query try (screenSpace */
    private boolean isSolidAdjacent(int direction, Rectangle2d rs){
	TreeSet ts;
	int rangeDirection=0;
	Rectangle2d r; 
	if (direction==RIGHT){
	    ts = objectTrees[0];
	    rangeDirection=1;
	    r = new Rectangle2d(rs.getMaxX(),Double.NEGATIVE_INFINITY,rs.getMaxX(),Double.NEGATIVE_INFINITY);
	    SortedSet ss=null;
	    ss = ts.tailSet(r);
	    //	  ss = ts.headSet(r);
	    
	    Iterator i = ss.iterator();
	    while (i.hasNext()){
		Rectangle2d no=(Rectangle2d)i.next();
		if (no.getMinX()==rs.getMaxX()){
		    if (no.getMinY()<rs.getMaxY() && no.getMaxY()>rs.getMinY()){
			return (true);
		    } else {
			continue;
		    }
		}
		return (false);
	    }
	} else if (direction==TOP){
	    ts = objectTrees[1];
	    rangeDirection=0;
	    r = new Rectangle2d(Double.NEGATIVE_INFINITY,rs.getMaxY(),Double.NEGATIVE_INFINITY,rs.getMaxY());
	    SortedSet ss=null;
	    ss = ts.tailSet(r);
	    //	  ss = ts.headSet(r);
	    
	    Iterator i = ss.iterator();
	    while (i.hasNext()){
		Rectangle2d no=(Rectangle2d)i.next();
		if (no.getMinY()==rs.getMaxY()){
		    if (no.getMinX()<rs.getMaxX() && no.getMaxX()>rs.getMinX()){
			return (true);
		    } else {
			continue;
		    }
		}
		return (false);
	    }
	} else if (direction==BOTTOM){
	    ts = objectTrees[3];
	    rangeDirection=0;
	    r = new Rectangle2d(Double.POSITIVE_INFINITY,rs.getMinY(),Double.POSITIVE_INFINITY,rs.getMinY());
	    SortedSet ss=null;
	    ss = ts.tailSet(r);
	    //	  ss = ts.headSet(r);
	    
	    Iterator i = ss.iterator();
	    while (i.hasNext()){
		Rectangle2d no=(Rectangle2d)i.next();
		if (no.getMaxY()==rs.getMinY()){
		    if (no.getMinX()<rs.getMaxX() && no.getMaxX()>rs.getMinX()){
			return (true);
		    } else {
			continue;
		    }
		}
		return (false);
	    }
	} else if (direction==LEFT){
	    ts = objectTrees[2];
	    rangeDirection=1;
	    r = new Rectangle2d(rs.getMinX(),Double.POSITIVE_INFINITY,rs.getMinX(),Double.POSITIVE_INFINITY);
	    SortedSet ss=null;
	    ss = ts.tailSet(r);
	    //	  ss = ts.headSet(r);
	    
	    Iterator i = ss.iterator();
	    while (i.hasNext()){
		Rectangle2d no=(Rectangle2d)i.next();
		//		System.out.println("LEFT no=" + no);
		if (no.getMaxX()==rs.getMinX()){
		    if (no.getMinY()<rs.getMaxY() && no.getMaxY()>rs.getMinY()){
			return (true);
		    } else {
			continue;
		    }
		}
		//		System.out.println("return false");
		return (false);
	    }
	}
	return (false);
    }
    private boolean addSpaceRectangle(Rectangle2d rs){
	return(addSpaceRectangle(rs.getMinX(),rs.getMinY(),rs.getMaxX(), rs.getMaxY()));
    }
    private boolean addSpaceRectangle(double minx, double miny, double maxx, double maxy){
	Rectangle2d r = new Rectangle2d(minx,miny,maxx,maxy);
	int a=-1;
	if (r.area()<=0.0 || spaceRectangles.isEnclosedBy(r)){
	    return(false);
	}
	return (addSpaceRectangleWithoutCheck(r));
    }
    private boolean addSpaceRectangleWithoutCheck(Rectangle2d r){
	spaceTrees.add(r);
	spaceRectangles.addRectangle(r);
	return(true);
    }
    private int deleteObjectRectangle(FullRectangle2d rs){
	int ret=0;
	if (debug){
	    for (int i=0; i<4; i++){
		objectTrees[i].remove(rs);
	    }
	}
	if (!objectHash.containsKey(rs.id)){
	    // rectangle is not in manager
	    return (-1);
	} else {
	    objectHash.remove(rs.id);
	}
	objectRectangles.deleteRectangle(rs);
	if (useObjectVector){
	    ret=objectVector.indexOf(rs);
	    objectVector.remove(ret);
	}
	return (ret);
    }
    private boolean deleteSpaceRectangle(Rectangle2d rs){
	boolean ret=true;
	if (!spaceTrees.remove(rs)){
	    System.err.println("spaceTrees.remove returned false rs=" + rs + " and SpaceRectangles=");
	    Thread.currentThread().dumpStack();
	    printAllRectangles();
	    ret=false;
	}
	if (!spaceRectangles.deleteRectangle(rs)){
	    System.err.println("spaceRectangles.deleteRectangle returned false");
	    ret=false;
	}
	if (calcRedoList && ret){
	    currentRedoAction.addedSpaces.add(rs);
	}
	return (ret);
    }
    private FullRectangle2d addObjectRectangle(FullRectangle2d rs,int place){
	FullRectangle2d r = rs ;
	
	if (objectHash.containsKey(rs.id)){
	    // same id
	    rs.id = new Long (maxLong+1l);
	    maxLong++;
	} else if (rs.id.longValue() > maxLong){
	    maxLong = rs.id.longValue();
	} 

	objectHash.put(rs.id,rs);
	if (debug){
	    for (int i=0; i<4; i++){
		objectTrees[i].add(r);
	    }
	}
	objectRectangles.addRectangle(r);

	if (useObjectVector){
	    if (place<0){
		objectVector.add(r);
	    } else {
		objectVector.insertElementAt(r,place);
	    }
	}
	return (r);
    }
    public FullRectangle2d moveRectangle(FullRectangle2d rect,double x, double y){
	int place;
	if (redoList != null && redoList.empty()){
	    System.out.println("moveRectangle redoList is empty and not null rect=" + rect);
	}
	if (redoList != null && !redoList.empty() && ((RedoAction)redoList.peek()).rectangle.id==rect.id){
	    place=undoTopRedoList();
	} else {
	    place=deleteRectangle(rect);
	    if (debug && debugSpaceManager()){
		return (rect);
	    }
	}
	redoList = new Stack();
	calcRedoList=true;
	rect.setRect(rect.getMinX()+x, rect.getMinY()+y, rect.getMaxX()+x, rect.getMaxY()+y);
	currentRedoAction = new RedoAction(rect);
	addRectangle(rect,place);
	calcRedoList=false;
	redoList.push(currentRedoAction);
	currentRedoAction=null;
	return (rect);
    }
    // Function addAllRectangles:: Does not work yet.
    public void addAllRectangles (Vector rectangles){
	class SpaceInterval {
	    double min, max, depth;
	    SpaceInterval left,right;
	    SpaceInterval (double mn,double mx,double d){
		min=mn;
		max=mx;
		depth=d;
		left=null;
		right=null;
	    }
	    SpaceInterval (double mn,double mx,double d,SpaceInterval l,SpaceInterval r){
		this(mn,mx,d);
		left=l;
		right=r;
	    }
	    void setLeft(SpaceInterval l){
		left=l;
	    }
	    void setRight(SpaceInterval r){
		right=r;
	    }
	};
	TreeSet ts=new TreeSet(new MinXComparator());
	ts.addAll(rectangles);
	SpaceInterval si = new SpaceInterval(screenSpace.getMinY(),screenSpace.getMaxY(),0.);
	Rectangle2d rect,newRect,tmpRect;
	SpaceInterval cursi;
	TreeSet rectList = new TreeSet(new MaxXComparator());
	Vector siList = new Vector();
	double tmpvar;
	for (Iterator i=ts.iterator(); i.hasNext();){
	    rect = (Rectangle2d) i.next();
	    siList.removeAllElements();
	    siList.add(si);
	    SortedSet ss=rectList.headSet(new Double(rect.getMinX()));
	    Iterator it=ss.iterator();
	    while (it.hasNext()){
		tmpRect = (Rectangle2d)it.next();
		siList.add(si);
		while (!siList.isEmpty()){
		    cursi = (SpaceInterval)siList.firstElement();
		    siList.remove(0);
		    if (cursi.min>rect.getMaxY()){
			siList.add(cursi.left);
			continue;
		    } else if (si.max<rect.getMinY()){ // space is to left of rectangle
			siList.add(cursi.right);
			continue;
		    } else { // grow some rectangles
			if (cursi.max <rect.getMaxY()){
			    tmpvar = cursi.max;
			    cursi.max=rect.getMinY();
			    cursi.right = new SpaceInterval(cursi.max,tmpvar,rect.getMaxX(),cursi,cursi.right);
			} 
		    }
		}
	    }
	    siList.removeAllElements();
	    siList.add(si);
	    while (!siList.isEmpty()){
		cursi = (SpaceInterval)siList.firstElement();
		siList.remove(0);
		if (cursi.min>rect.getMaxY()){ // space is to right of rectangle
		    siList.add(cursi.left);
		    continue;
		} else if (si.max<rect.getMinY()){ // space is to left of rectangle
		    siList.add(cursi.right);
		    continue;
		} else { // overlap, lets make some rectangles
		    boolean right=false,left=false;
		    //		    addSpaceRectanglesWithoutCheck(cursi.depth,si.min,rect.getMinX(),si.max);
		    rectList.add(rect);
		    left=cursi.min<rect.getMinY();
		    right=cursi.max>rect.getMaxY();
		    if (left&&right){
			tmpvar=cursi.max;
			cursi.max=rect.getMinY();
			//new SpaceInterval(cursi.min,rect.getMinY(), cursi.depth, cursi.left,cursi);
			cursi.right=new SpaceInterval(rect.getMaxY(), tmpvar, cursi.depth, cursi,cursi.right);
		    }else if (left){
			cursi.max=rect.getMinY();
			siList.add(cursi.right);
		    }else if (right){
			cursi.min=rect.getMaxY();
			siList.add(cursi.left);
		    }

		}
	    }
	}
    }
    public FullRectangle2d addRectangle(FullRectangle2d rs,int place){
	if (processing>0){
	    System.out.println("addRectangle: another thread is processing=" + processing);
	}
	processing++;
	if (rs.area()==0){
	    return null;
	}
	FullRectangle2d ret=addObjectRectangle(rs,place);
	//	System.out.println("ret=" + ret);
	if (ret==null){
	    return (null);
	}
	Vector v[],va[];
	
	//	Vector containmentList = spaceRectangles.windowQueryWithoutEdges(rs);
	Vector containmentList= spaceRectangles.windowQuery(rs);

	v=new Vector[4];
	va=new Vector[4];
	for (int f=0; f<4; f++){
	    v[f] = new Vector();
	    va[f] = new Vector();
	}

	int contnum=0;
	
	int i=0;
	Rectangle2d tmpRect,tmpRect2;
	Enumeration e;
	boolean removed;
	for (e = containmentList.elements(); e.hasMoreElements();){
	    Rectangle2d clrs = (Rectangle2d)e.nextElement();
	    i++;
	    if (rs.getMinX()==clrs.getMaxX()){  // Exactly adjacent to left, only add to list for containment query
		va[0].add(clrs);
		continue;
	    }
	    if (rs.getMaxX()==clrs.getMinX()){  // Exactly adjacent to right, only add to list for containment query
		va[1].add(clrs);
		continue;
	    }
	    if (rs.getMaxY()==clrs.getMinY()){  // Exactly adjacent to top, only add to list for containment query
		va[2].add(clrs);
		continue;
	    }
	    if (rs.getMinY()==clrs.getMaxY()){  // Exactly adjacent to bottom, only add to list for containment query
		va[3].add(clrs);
		continue;
	    }
	    if (!deleteSpaceRectangle(clrs)){
		System.out.println("in addRectangle");
	    };
	    if (rs.getMinX()>clrs.getMinX()){ // Left
		v[0].add(new Rectangle2d(clrs.getMinX(),clrs.getMinY(),rs.getMinX(),clrs.getMaxY()));
	    }
	    if (clrs.getMaxX()>rs.getMaxX()){ // right
		v[1].add(new Rectangle2d(rs.getMaxX(),clrs.getMinY(),clrs.getMaxX(),clrs.getMaxY()));
	    }
	    if (clrs.getMaxY()>rs.getMaxY()){ // top
		v[2].add(new Rectangle2d(clrs.getMinX(),rs.getMaxY(),clrs.getMaxX(),clrs.getMaxY()));
	    }
	    if (rs.getMinY()>clrs.getMinY()){ // bottom
		v[3].add(new Rectangle2d(clrs.getMinX(),clrs.getMinY(),clrs.getMaxX(),rs.getMinY()));
	    }
	}
	
	for (int doeach=0; doeach<4; doeach++){
	    for (Iterator it=v[doeach].iterator();it.hasNext();){
		tmpRect = (Rectangle2d)it.next();
		removed=false;
		for (e=va[doeach].elements();e.hasMoreElements();){
		    tmpRect2 = (Rectangle2d)e.nextElement();
		    if (tmpRect.enclosedBy(tmpRect2)){
			it.remove();
			removed=true;
			break;
		    }
		}
		if (!removed){
		    for (e=v[doeach].elements();e.hasMoreElements();){
			tmpRect2 = (Rectangle2d)e.nextElement();
			if (tmpRect2==tmpRect){
			    continue;
			}
			if (tmpRect.enclosedBy(tmpRect2)){
			    it.remove();
			    break;
			}
		    }
		}
	    }
	    for (e=v[doeach].elements();e.hasMoreElements();){
		tmpRect = (Rectangle2d)e.nextElement();
		addSpaceRectangleWithoutCheck(tmpRect);
	    }
	    if (calcRedoList){
		currentRedoAction.removedSpaces.addAll(v[doeach]);
	    }
	}
	processing--;
	return (ret);
    }

    class BackwardsEnumeration implements Enumeration {
	int count = 0;
	Vector vect;
	public BackwardsEnumeration(Vector v){
	    vect=v;
	    count = vect.size();
	}
	public boolean hasMoreElements() {
	    return count > 0;
	}
    
	public Object nextElement() {
	    synchronized (vect) {
		if (count > 0) {
		    return vect.elementAt(--count);
		}
	    }
	    throw new NoSuchElementException("Vector Enumeration");
	}
    };
  
    public FullRectangle2d getClosestRectangle(Rectangle2d rectarg){
	Vector v = objectRectangles.windowQuery(rectarg);
	int i=0;
	for (Enumeration e = (Enumeration) new BackwardsEnumeration(objectVector); e.hasMoreElements() ;){
	    FullRectangle2d rs=(FullRectangle2d)e.nextElement();
	    if(v.contains(rs)){
		return(rs);
	    }
	}
	return null;
    }
    public int deleteRectangleWindow(Rectangle2d rectarg){
	//	System.out.println("deleteRectangleWindow:: rectarg=(" + rectarg.getMinX() + "," + rectarg.getMinY() + ")-(" + rectarg.getMaxX() + "," + rectarg.getMaxY() + ")");
	FullRectangle2d rect = getClosestRectangle(rectarg);
	if (rect!=null){
	    return (deleteRectangle(rect));
	} else {
	    return 0;
	}
    }
    public int deleteRectangle(long rsnum){
	Long rsnumob = new Long (rsnum);
	if (!objectHash.containsKey(rsnumob)){
	    // rectangle is not in manager
	    return (-1);
	} else {
	    return(deleteRectangle((FullRectangle2d)objectHash.get(rsnumob)));
	}
    }
    public int deleteRectangle(FullRectangle2d rs){
	if (processing>0){
	    System.out.println("deleteRectangle: another thread is processing=" + processing);
	}
	processing++;
	int ret;
	ret=deleteObjectRectangle(rs);
	Rectangle2d inter = rs.createIntersection(screenSpace);
	
	if (inter.area() <= 0.0){
	    processing--;
	    return(ret);
	}
	Vector adjacentContainmentList = spaceRectangles.windowQuery(rs);

	IntervalTree tmpIntervals[] = new IntervalTree[4];
	Class spaceRectClass = null;
	Method minMeth[] = new Method[2], maxMeth[] = new Method[2];

	IntervalTree bottomInterval=null,
	    topInterval=null,
	    leftInterval=null,
	    rightInterval=null;
	
	try {
	    Class rectClass = Class.forName("edu.columbia.cs.cgui.spam2d.Rectangle2d");
	    spaceRectClass = Class.forName("edu.columbia.cs.cgui.spam2d.Rectangle2d");
	    minMeth[0] = rectClass.getDeclaredMethod("getMinX",null);
	    maxMeth[0] = rectClass.getDeclaredMethod("getMaxX",null);
	    minMeth[1] = rectClass.getDeclaredMethod("getMinY",null);
	    maxMeth[1] = rectClass.getDeclaredMethod("getMaxY",null);
	    tmpIntervals[0] = new IntervalTree(spaceRectClass,minMeth[1],maxMeth[1],Class.forName("edu.columbia.cs.cgui.spam2d.YSecondaryStructure"), new IntervalDimension(1,minMeth[1],maxMeth[1]));
	    tmpIntervals[1] = new IntervalTree(spaceRectClass,minMeth[1],maxMeth[1],Class.forName("edu.columbia.cs.cgui.spam2d.YSecondaryStructure"), new IntervalDimension(1,minMeth[1],maxMeth[1]));
	    tmpIntervals[2] = new IntervalTree(spaceRectClass,minMeth[0],maxMeth[0],Class.forName("edu.columbia.cs.cgui.spam2d.XSecondaryStructure"), new IntervalDimension(1,minMeth[0],maxMeth[0]));
	    tmpIntervals[3] = new IntervalTree(spaceRectClass,minMeth[0],maxMeth[0],Class.forName("edu.columbia.cs.cgui.spam2d.XSecondaryStructure"), new IntervalDimension(1,minMeth[0],maxMeth[0]));
	} catch (Exception e){
	    e.printStackTrace();
	}

	Vector completeResultList = new Vector();
	for (Iterator i=adjacentContainmentList.iterator(); i.hasNext();){
	    Rectangle2d r = (Rectangle2d)i.next();
	    if (r.getMaxX()==rs.getMinX()){ // left space rectangles
		tmpIntervals[0].addRectangle(r);
	    } else if (r.getMinX()==rs.getMaxX()){ // right space rectangles
		tmpIntervals[1].addRectangle(r);
	    } else if (r.getMaxY()==rs.getMinY()){ // bottom space rectangles
		tmpIntervals[2].addRectangle(r);
	    } else if (r.getMinY()==rs.getMaxY()){ // top space rectangles
		tmpIntervals[3].addRectangle(r);
	    }
	    completeResultList.add(r);
	    deleteSpaceRectangle(r);
	}

	SpaceManager deletetmpmanager=null;
	/*	boolean spacesLeft=!leftInterval.isEmpty(),
		spacesRight=!rightInterval.isEmpty(),
		spacesTop=!topInterval.isEmpty(),
		spacesBottom=!bottomInterval.isEmpty();
	*/
	
	Vector containmentList = objectRectangles.windowQueryWithoutEdges(rs);
	Vector listOfInnerSpaces = new Vector();

	if (containmentList.size()==0){
	    listOfInnerSpaces.add(inter);
	} else {
	    try {
		deletetmpmanager = new SpaceManager(inter);
	    } catch (Exception e){
		e.printStackTrace();
	    }
	    for (Enumeration e=containmentList.elements(); e.hasMoreElements();){
		deletetmpmanager.addRectangle((FullRectangle2d)e.nextElement(),-1);
	    }
	    listOfInnerSpaces.addAll(deletetmpmanager.spaceTrees);
	}
	Vector listOfBits = new Vector();
	for (Iterator iter=listOfInnerSpaces.iterator(); iter.hasNext();){
	    Rectangle2d currRect = (Rectangle2d) iter.next();
	    BitSet bs=new BitSet(4); // left=0, right=1, bottom=2, top=3
	    int count=0;
	    if (rs.getMinX()==currRect.getMinX()){ bs.set(0); }
	    if (rs.getMaxX()==currRect.getMaxX()){ bs.set(1); }
	    if (rs.getMinY()==currRect.getMinY()){ bs.set(2); }
	    if (rs.getMaxY()==currRect.getMaxY()){ bs.set(3); }
	    //	    System.out.println("currRect=" + currRect + "\tbs=" + bs);
	    if (bs.length()==0){
		addSpaceRectangleWithoutCheck(currRect);
		iter.remove();
		continue;
	    } else {
		listOfBits.add(bs);
	    }
	}
	//	System.out.println("inter=" + inter);
	for (int whichDim=0,dim=0; whichDim<4; whichDim++,dim=whichDim/2){
	    Vector newListOfInnerSpaces = new Vector();
	    Vector newListOfBits = new Vector();
	    //	    System.out.println("whichDim=" + whichDim);
	    for (Enumeration e=listOfInnerSpaces.elements(), e2=listOfBits.elements();
		 e.hasMoreElements();){
		Rectangle2d currRect = (Rectangle2d) e.nextElement();
		BitSet bs = (BitSet) e2.nextElement();
		BitSet currbs = (BitSet)bs.clone();
		//		System.out.print("\tcurrRect=" + currRect + "\tbs=" + bs); 
		if (!bs.get(whichDim)){
		    //		    System.out.println(" not processed");
		    newListOfInnerSpaces.add(currRect);
		    newListOfBits.add(bs.clone());
		    continue;
		}
		//		System.out.println("");
		boolean enclosed=false;
		Rectangle2d longest=null;
		BitSet longestbs=null;

		for (Enumeration e3=tmpIntervals[whichDim].windowQueryWithoutEdges(currRect).elements();e3.hasMoreElements();){
		    Rectangle2d tmpRect=(Rectangle2d)e3.nextElement();
		    Rectangle2d tmpR = Rectangle2d.consensus(tmpRect,currRect,dim);
		    // Taking into account spaces that are the same height as the tmpRect before the consensus
		    boolean enc=true;
		    if (dim!=0){
			if (enc && tmpRect.getMinX()<=currRect.getMinX() && 
			    tmpRect.getMaxX()>=currRect.getMaxX()){
			    enc=true;
			} else {
			    enc=false;
			}
		    }
		    if (dim!=1){
			if (enc && tmpRect.getMinY()<=currRect.getMinY() && 
			    tmpRect.getMaxY()>=currRect.getMaxY()){
			    enc=true;
			} else {
			    enc=false;
			}
		    }
		    if (enc){
			enclosed=true;
			if (longest==null){
			    longest=tmpR;
			    longestbs=(BitSet)currbs.clone();
			} else {
			    boolean setlong=false;
			    try {
				if (whichDim%2==0){
				    if (((Double)minMeth[dim].invoke(longest,null)).doubleValue() > 
					((Double)minMeth[dim].invoke(tmpR,null)).doubleValue()){
					setlong=true;
				    }
				} else {
				    if (((Double)maxMeth[dim].invoke(longest,null)).doubleValue() < 
					((Double)maxMeth[dim].invoke(tmpR,null)).doubleValue()){
					setlong=true;
				    }
				}
			    } catch (Exception ex){
				ex.printStackTrace();
			    }
			    if (setlong){
				longest=tmpR;
				longestbs=(BitSet)currbs.clone();
			    }
			}
		    } else {
			newListOfInnerSpaces.add(tmpR);
			BitSet clbs = (BitSet)bs.clone();
			if (dim!=0){
			    if (clbs.get(0) && tmpR.getMinX()>inter.getMinX()) { clbs.clear(0); }
			    if (clbs.get(1) && tmpR.getMaxX()<inter.getMaxX()) { clbs.clear(1); }
			}
			if (dim!=1){
			    if (clbs.get(2) && tmpR.getMinY()>inter.getMinY()) { clbs.clear(2); }
			    if (clbs.get(3) && tmpR.getMaxY()<inter.getMaxY()) { clbs.clear(3); }
			}
			newListOfBits.add(clbs);
			//			System.out.println("\t\t\t\ttmpR=" + tmpR + "\tbs=" + bs);
		    }
		}
		if (!enclosed){
		    //		    System.out.println("!enclosed:\t\tcurrRect=" + currRect + "\tbs=" + bs);
		    newListOfInnerSpaces.add(currRect);
		    newListOfBits.add(bs);		    
		} else {
		    //		    System.out.println("enclosed:\t\tlongest=" + longest + "\tlongestbs=" + longestbs);
		    newListOfInnerSpaces.add(longest);
		    newListOfBits.add(longestbs);
		}
	    }
	    listOfInnerSpaces = newListOfInnerSpaces;
	    listOfBits = newListOfBits;
	}
	listOfInnerSpaces.addAll(adjacentContainmentList);
	for (Iterator it2=listOfInnerSpaces.iterator(); it2.hasNext();){
	    Rectangle2d rect1=(Rectangle2d)it2.next();
	    for (Enumeration e=listOfInnerSpaces.elements(); e.hasMoreElements();){
		Rectangle2d rect2=(Rectangle2d)e.nextElement();
		if (rect1==rect2){
		    continue;
		}
		if (rect1.enclosedBy(rect2)){
		    it2.remove();
		    break;
		}
	    }
	}
	int asdf=0;
	for (Enumeration e=listOfInnerSpaces.elements(); e.hasMoreElements();){
	    Rectangle2d tmpRect=(Rectangle2d)e.nextElement();
	    //	    System.out.println("listOfInnerSpaces rect[" + (asdf++) + "]=" + tmpRect);
	    addSpaceRectangleWithoutCheck(tmpRect);
	}
	processing--;
	return (ret);
    }
    public int getNumberSpaces(boolean justinside){
	if (!justinside){
	    return (spaceTrees.size());
	} else {
	    int numInside=0;
	    Iterator i = spaceTrees.iterator();
	    while (i.hasNext()){
		Rectangle2d rect = (Rectangle2d)i.next();
		if (rect.getMinX()==screenSpace.getMinX() || rect.getMinY()==screenSpace.getMinY() ||
		    rect.getMaxX()==screenSpace.getMaxX() || rect.getMaxY()==screenSpace.getMaxY()){
		    continue;
		}
		numInside++;
	    }
	    return (numInside);
	}
    }
    public Iterator spaceIterator(){
	return (spaceTrees.iterator());
    }
    public Iterator getSpacesOrderedByArea(){
	return (spaceTrees.iterator());
    }
    public Rectangle2d getLargestSpace(){
	return(new Rectangle2d((Rectangle2d)spaceTrees.first()));
    }
    public Rectangle2d getClosestSpace(Rectangle2d rect){
	Iterator i=spaceTrees.iterator();
	Rectangle2d closest=null,tmpRect=null;
	double currentDistance=Double.POSITIVE_INFINITY,tmpCD=0.;
	if (i.hasNext()){
	    tmpRect = (Rectangle2d)i.next();
	    currentDistance=Rectangle2d.getDistanceBetween(tmpRect,rect);
	    closest=tmpRect;
	}
	while (i.hasNext()){
	    tmpRect = (Rectangle2d)i.next();
	    if ((tmpCD=Rectangle2d.getDistanceBetween(tmpRect,rect))<currentDistance ||
		(currentDistance==tmpCD && closest.area()<tmpRect.area())){
		currentDistance=tmpCD;
		closest = tmpRect;
	    }
	}
	return (new Rectangle2d(closest));
    }

    public Rectangle2d getLargestProportion(Rectangle2d rect, Dimension prop){
	Iterator i=spaceTrees.iterator();
	Rectangle2d largestprop=null,tmpRect=null;
	double currentDistance=Double.POSITIVE_INFINITY,tmpCD=0.;
	double currentScale=0.;
	while (i.hasNext()){
	    tmpRect = (Rectangle2d)i.next();
	    if (largestprop !=null && tmpRect.area()<largestprop.area()){
		break;
	    }
	    if ((largestprop==null) ||
		(tmpRect.getWidth()>=prop.getWidth()*currentScale && 
		 tmpRect.getHeight()>=prop.getHeight()*currentScale)){
		double xscale,yscale;
		xscale=tmpRect.getWidth()/prop.getWidth();
		yscale=tmpRect.getHeight()/prop.getHeight();
		currentScale=Math.min(xscale,yscale);
		largestprop=tmpRect;
	    }
	}
	return (largestprop);
    }
    public Rectangle2d getClosestWithExactDimension(Rectangle2d rect,double dw, double dh){
	Vector listOfRects=getClosestWithAtLeastDimension(rect,dw,dh);
	Rectangle2d resCWALD;
	Rectangle2d retRect=null;
	double tmpD=0.;
	if (listOfRects.isEmpty()){
	    return (null);
	}
	double distanceBetCenters=Double.POSITIVE_INFINITY;
	for (Enumeration e=listOfRects.elements(); e.hasMoreElements();){
	    resCWALD=(Rectangle2d)e.nextElement();
	    Rectangle2d recRes=new Rectangle2d(resCWALD);
	    boolean left,right,top,bottom;
	    if (recRes.getMaxX()<=rect.getMinX()){ // left of rectangle
		recRes.setMinX(recRes.getMaxX()-dw);
	    } else if (recRes.getMinX()>=rect.getMaxX()){ // right of rectangle
		recRes.setMaxX(recRes.getMinX()+dw);
	    } else { // overlap in the X direction
		if (recRes.getMinX() > rect.getMinX()){ 
		    recRes.setMaxX(recRes.getMinX()+dw); // should be placed to far left of space
		} else if (recRes.getMaxX() < rect.getMaxX()){
		    recRes.setMinX(recRes.getMaxX()-dw);
		} else {
		    double cx=rect.getCenterX(),hw=dw/2.;
		    recRes.setMinX(cx-hw);
		    recRes.setMaxX(cx+hw);
		}
	    }
	    
	    if (recRes.getMaxY()<=rect.getMinY()){ // bottom of rectangle
		recRes.setMinY(recRes.getMaxY()-dh);
	    } else if (recRes.getMinY()>=rect.getMaxY()){
		recRes.setMaxY(recRes.getMinY()+dh);
	    } else { // overlap in the Y direction
		if (recRes.getMinY() > rect.getMinY()){
		    recRes.setMaxY(recRes.getMinY()+dh);
		} else if (recRes.getMaxY() < rect.getMaxY()){
		    recRes.setMinY(recRes.getMaxY()-dh);
		} else {
		    double cy=rect.getCenterY(),hh=dh/2.;
		    recRes.setMinY(cy-hh);
		    recRes.setMaxY(cy+hh);
		}
	    }
	    tmpD=Rectangle2d.getCenterDistanceBetween(recRes,rect);
	    if (tmpD<distanceBetCenters){
		distanceBetCenters=tmpD;
		retRect=recRes;
	    }
	}
	return (retRect);
    }

    public Vector getClosestWithAtLeastDimension(Rectangle2d rect,double dw,double dh){
	Iterator i=spaceTrees.iterator();
	Rectangle2d closest=null,tmpRect=null;
	double minarea=dw * dh;
	double currentDistance=Double.POSITIVE_INFINITY,tmpCD=0.,currentOverlap=0.;
	Vector closestV=new Vector();
	Rectangle2d result=new Rectangle2d();
	while (i.hasNext()){
	    tmpRect = (Rectangle2d)i.next();
	    if (tmpRect.getWidth()>=dw && tmpRect.getHeight()>=dh){
		if ((tmpCD=Rectangle2d.getDistanceBetween(tmpRect,rect))<currentDistance){
		    currentDistance=tmpCD;
		    closest = tmpRect;
		    closestV.removeAllElements();
		    closestV.add(tmpRect);
		    if (tmpCD==0.){
			if (Rectangle2d.intersect(rect,tmpRect,result)){
			    currentOverlap = result.area();
			}
		    }
		} else if (currentDistance==0. && tmpCD==0.){
		    double compIntersection = 0.;
		    if (Rectangle2d.intersect(rect,tmpRect,result)){
			compIntersection = result.area();
			if (compIntersection>currentOverlap){
			    closest = tmpRect;
			    closestV.removeAllElements();
			    closestV.add(tmpRect);
			    currentOverlap=compIntersection;
			}
		    }
		} else if (tmpCD==currentDistance){
		    closestV.add(tmpRect);
		}
	    }
	}
	return (closestV);
    }
    
    public Iterator objectIterator(){
	return (objectVector.iterator());
    }
    public Vector getAllOverlapObjects(Rectangle2d rect){
	return (objectRectangles.windowQueryWithoutEdges(rect));
    }
}

