package edu.columbia.cs.cgui.spam3d;

import javax.swing.*;
import java.awt.event.*;
import java.awt.*;
import java.util.*;
import java.awt.geom.*;
import java.lang.*;
import javax.swing.plaf.basic.*;
import java.awt.image.*;
import java.io.*;
import javax.swing.text.*;
import javax.swing.event.*;

public class SpaceManagerPanel3d extends JPanel implements MouseListener, ActionListener, MouseMotionListener {
    private boolean debug=true,shownumberrect=true,showoverlaprect=false,shownextnumberrect=false,showinsiderect=false;
    SpaceManager3d spacemanager;
    Point startingPoint;
    Point2D mousePlace;
    Color transparent = new Color(1.0f,1.0f,1.0f,.0f);
    Color blacktrans = new Color(0.f,0.f,0.f,.5f),whitetrans = new Color(1.f,1.f,1.f,.5f),blacktrans2 = new Color(0.f,0.f,0.f,.75f);
    float fl[]=new float[] { 2.f};
    BasicStroke dash=new BasicStroke(1.f,BasicStroke.CAP_BUTT,BasicStroke.JOIN_MITER,
				     10.f,fl,2.f), normal=new BasicStroke();
    Dimension size= new Dimension();
    double height,width;
    static Random rand = new Random();
    
    boolean drawing=false,deleting=false;
    Rectangle addingRectangle1=null,addingRectangle2=null,movingRectangleOutline=null;
    double addRectDepth=0.0;
    FullRectangle3d movingRectangle=null;
    Rectangle3d screenSpace=null;
    DefaultListModel defaultlistmodel;
    public static int ADDING=0x01;
    public static int DELETING=0x02;
    public static int MOVING=0x03;
    public static int MOVINGFAST=0x04;
    private int mode=ADDING;
    private JList jlist;
    private JComboBox showoverlap,shownext;
    private Vector overlapRectangles=new Vector();
    private int numberOfOverlapRect=0,numoverlap,maxNumOverlap, numnext, maxNumNext;
    private JTextComponent maxRectangleDisplay;
    private boolean isMoveDeleteAdd=true,showClosest=false,gridon=false;
    private double drawScaleX=1.;
    private double drawScaleY=1.;
    private double drawScaleZ=1.;
    private int addingStage=INITSTAGE;
    private static int INITSTAGE=0x00;
    private static int DEPTHSTAGE=0x01;
    private static int MOVESTAGE=0x02;
    void clearComboBox(JComboBox cb){
	while (cb.getItemCount()>0){
	    cb.removeItemAt(0);
	}
    }
    void setShowClosest(boolean b){
	showClosest=b;
	repaint();
    }
    void setShowGrid(boolean b){
	gridon=b;
	repaint();
    }
    void setShowOverlap(boolean b){
	showoverlaprect = b;
	if (!showoverlaprect){
	    showoverlap.removeItemListener(iloverlap);
	    clearComboBox(showoverlap);
	    showoverlap.addItemListener(iloverlap);
	}
	showOverlapRectangles(-1);
    }
    void setShowInsideRect(boolean b){
	showinsiderect=b;
	setListModelSize(spacemanager.getNumberSpaces(showinsiderect));
    }
    void setShowNextNumberRect(boolean b){
	//	System.out.println("setShowNextNumberRect b=" +b);
	shownextnumberrect = b;
	if (!shownextnumberrect){
	    shownext.removeItemListener(ilnext);
	    clearComboBox(shownext);
	    shownext.addItemListener(ilnext);
	}
    }
    void setMaxRectangleDisplay(JTextComponent tc){
	maxRectangleDisplay = tc;
    }
    ItemListener iloverlap,ilnext;
    void setNumberOverlapChooser(JComboBox cb){
	showoverlap = cb;
	showoverlap.addItemListener(iloverlap=new ItemListener(){
		public void itemStateChanged(ItemEvent e){
		    if (e.getStateChange()==ItemEvent.SELECTED){
			showoverlap.removeItemListener(iloverlap);
			Integer i = new Integer((String)e.getItem());
			showOverlapRectangles(i.intValue());
			showoverlap.setSelectedItem(e.getItem());
			showoverlap.addItemListener(iloverlap);
			repaint();
		    }
		}
	    });
    }
    void setNumberNextChooser(JComboBox cb){
	shownext = cb;
	shownext.addItemListener(ilnext = new ItemListener(){
		public void itemStateChanged(ItemEvent e){
		    if (e.getStateChange()==ItemEvent.SELECTED){
			Integer i = new Integer((String)e.getItem());
			showNextNumberRectangles(i.intValue());
			//			shownext.setSelectedItem(e.getItem());
			repaint();
		    }
		}
	    });
    }
    public void showNumberRectangles(boolean t){
	shownumberrect=t;
	if (t){
	    maxRectangleDisplay.setText(Integer.toString(jlist.getModel().getSize()));
	} else {
	    maxRectangleDisplay.setText("");
	}
    }
    void setNumberEntries(int num){
	//	System.out.println("setNumberEntries: num="+ num + "\tcb.getItemCount()=" + showoverlap.getItemCount());
	/*	for (int adsf=0; adsf<cb.getItemCount(); adsf++){
	    System.out.println("item #" + adsf + ": " + cb.getItemAt(adsf));
	    }*/

	if (showoverlap.getItemCount()<num){
	    //	    for (int i=cb.getItemCount()+1; cb.getItemCount()+1<=num;i++){
	    while (showoverlap.getItemCount()+1<=num){
		//		System.out.println("ItemCount<num: cb.addItem(" + cb.getItemCount()+1+ ")");
		showoverlap.addItem(Integer.toString(showoverlap.getItemCount()+1));
	    }
	} else if (showoverlap.getItemCount()>num){
	    //	    for (int i=cb.getItemCount(); i>num;i--){
	    showoverlap.removeItemListener(iloverlap);
	    clearComboBox(showoverlap);
	    showoverlap.addItemListener(iloverlap);
	    /*	    while (showoverlap.getItemCount()>num){
		//		System.out.println("ItemCount<num: cb.removeItem(" + cb.getItemCount()+ ")");
		showoverlap.removeItemAt(showoverlap.getItemCount()-1);
		}
	    */
	}
	/*	for (int adsf=0; adsf<cb.getItemCount(); adsf++){
	    System.out.println("end: item #" + adsf + ": " + cb.getItemAt(adsf));
	    } */
	//	System.out.println("setNumberEntries end");
    }
    static boolean inside = false;
    public void showNextNumberRectangles(int i){
	//	System.out.println("showNextNumberRectangles: i=" + i + " inside=" + inside);
	if (inside){
	    return;
	}
	shownext.removeItemListener(ilnext);
	int beforeSelected;
	if (shownext.getSelectedItem()==null){
	    beforeSelected=-1;
	} else {
	    beforeSelected=Integer.parseInt((String)shownext.getSelectedItem());
	}
	inside=true;
	numnext = i;
	if (shownext!=null){
	    boolean b=calcNextRectangles(numnext,beforeSelected);
	    //		setNumberEntries(showoverlap, maxNumOverlap);
	    if (numnext<0 || !b){
		shownext.setSelectedIndex(shownext.getItemCount()-1);
		//		    System.out.println("showoverlap.setSelectedIndex(" + maxNumOverlap + ")");
	    }
	}
	inside = false;	
	//	System.out.println("end: showNextNumberRectangles: i=" + i + " inside=" + inside + "\toverlapRectangles.size=" + overlapRectangles.size());
	shownext.addItemListener(ilnext);
    }
    public void showOverlapRectangles(int i){
	if (inside){
	    return;
	}
	inside=true;
	//	showoverlap = tc;
	//	System.out.println("showOverlapRectangles(" + i + ")");
	//	showoverlaprect=t;
	//	if (t){
	numoverlap = i;
	calcOverlapRectangles(numoverlap);
	if (showoverlap!=null){
	    setNumberEntries(maxNumOverlap);
	    if (numoverlap<0){
		showoverlap.setSelectedIndex(maxNumOverlap-1);
		//		    System.out.println("showoverlap.setSelectedIndex(" + maxNumOverlap + ")");
	    }
	}
	inside = false;
    }
    private boolean calcNextRectangles(int ov,int selected){
	boolean ret=false;
	//	System.out.println("calcNextRectangles ov=" + ov + " shownextnumberrect=" + shownextnumberrect);
	overlapRectangles.removeAllElements();

	//	shownext.removeItemListener(ilnext);
	if (shownext!=null &&  shownext.getItemCount()>0){
	    shownext.removeAllItems();
	}
	//	maxNumOverlap=0;
	if (!shownextnumberrect){
	    return false;
	}
	//	shownext.removeItemListener(ilnext);

	//	System.out.println("continuing");
	TreeSet nnext = new TreeSet();
	Iterator oi = spacemanager.objectIterator();
	TreeSet tsx=new TreeSet(), tsy=new TreeSet();
	Rectangle3d rect2;
	tsx.add(new Double(spacemanager.screenSpace.getMinX()));
	tsx.add(new Double(spacemanager.screenSpace.getMaxX()));
	tsy.add(new Double(spacemanager.screenSpace.getMinY()));
	tsy.add(new Double(spacemanager.screenSpace.getMaxY()));
	while (oi.hasNext()){
	    rect2 = (Rectangle3d)oi.next();
	    if (rect2.getMinX() > spacemanager.screenSpace.getMinX() && rect2.getMinX() < spacemanager.screenSpace.getMaxX())
		tsx.add(new Double(rect2.getMinX()));
	    if (rect2.getMaxX() > spacemanager.screenSpace.getMinX() && rect2.getMaxX() < spacemanager.screenSpace.getMaxX())
		tsx.add(new Double(rect2.getMaxX()));
	    if (rect2.getMinY() > spacemanager.screenSpace.getMinX() && rect2.getMinY() < spacemanager.screenSpace.getMaxX())
		tsy.add(new Double(rect2.getMinY()));
	    if (rect2.getMaxY() > spacemanager.screenSpace.getMinX() && rect2.getMaxY() < spacemanager.screenSpace.getMaxX())
		tsy.add(new Double(rect2.getMaxY()));
	}
	double minx, miny, maxx, maxy,px,py;
	Iterator keepy = tsy.iterator();
	maxy = ((Double)keepy.next()).doubleValue();
	int tmpnext,curnum;
	maxNumNext=-1;
	curnum = spacemanager.spaceTrees.size();
	while (keepy.hasNext()){
	    miny = maxy;
	    maxy = ((Double)keepy.next()).doubleValue();
	    
	    Iterator keepx = tsx.iterator();
	    maxx = ((Double)keepx.next()).doubleValue();
	    
	    while (keepx.hasNext()){
		minx = maxx;
		maxx = ((Double)keepx.next()).doubleValue();
		double smx = (.25*(maxx-minx));
		double smy = (.25*(maxy-miny));
		FullRectangle3d rect3 = new FullRectangle3d(1L,minx+smx,miny+smy,0.,maxx-smx,maxy-smy,1.);
		//		tmpoverlap = spacemanager.spaceRectangles.windowQueryWithoutEdges(rect3).size();
		spacemanager.addRectangle(rect3,-1);
		tmpnext = spacemanager.spaceTrees.size();
		spacemanager.deleteRectangle(rect3);
		if (ov <= 0){
		    if (tmpnext>maxNumNext){ //numberOfOverlapRect){
			overlapRectangles.removeAllElements();
			overlapRectangles.add(new Rectangle3d(minx,miny,0.,maxx,maxy,1.));
			maxNumNext = tmpnext;
			//			numberOfOverlapRect = tmpoverlap;
			//			maxNumOverlap = tmpoverlap;
		    } else if (tmpnext==maxNumNext){ // numberOfOverlapRect){
			overlapRectangles.add(new Rectangle3d(minx,miny,.0,maxx,maxy,1.));
		    }
		    if (tmpnext!=curnum && !nnext.contains(new Integer(tmpnext))){
			nnext.add(new Integer(tmpnext));
		    }
		} else {
		    if (tmpnext>maxNumNext){ //numberOfOverlapRect){
			maxNumNext = tmpnext;
		    }
		    if (tmpnext!=curnum && !nnext.contains(new Integer(tmpnext))){
			nnext.add(new Integer(tmpnext));
		    }
		    if (tmpnext==ov){
			overlapRectangles.add(new Rectangle3d(minx,miny,.0,maxx,maxy,1.));
			//			System.out.println("added Rectangle " + rect3 + " for total:" + overlapRectangles.size());
		    }
		}
	    }
	}
	Iterator menuitem = nnext.iterator();
	String str;
	//	System.out.println("curnum=" + curnum);
	Integer integer;
	while (menuitem.hasNext()){
	    integer = (Integer)menuitem.next();
	    shownext.addItem(integer.toString());
	    if (integer.intValue()==selected){
		shownext.setSelectedIndex(shownext.getItemCount()-1);
		ret=true;
	    }
	    //	    System.out.println("adding: "+str);
	}
	//	shownext.addItemListener(ilnext);
	//	System.out.println("end of calcOverlapRectangles(" + ov + ")\tmaxNumOverlap=" + maxNumOverlap);
	return (ret);
    }
    private void calcOverlapRectangles(int ov){
	//	System.out.println("calcOverlapRectangles: ov=" + ov + "\tshowoverlaprect=" + showoverlaprect);
	//	if (!showoverlaprect){
	    overlapRectangles.removeAllElements();
	    //	}
	    //	numberOfOverlapRect=0;
	    maxNumOverlap=0;
	    if (!showoverlaprect){
		return;
	    }
	Iterator oi = spacemanager.objectIterator();
	TreeSet tsx=new TreeSet(), tsy=new TreeSet();
	Rectangle3d rect2;
	tsx.add(new Double(spacemanager.screenSpace.getMinX()));
	tsx.add(new Double(spacemanager.screenSpace.getMaxX()));
	tsy.add(new Double(spacemanager.screenSpace.getMinY()));
	tsy.add(new Double(spacemanager.screenSpace.getMaxY()));
	while (oi.hasNext()){
	    rect2 = (Rectangle3d)oi.next();
	    if (rect2.getMinX() > spacemanager.screenSpace.getMinX() && rect2.getMinX() < spacemanager.screenSpace.getMaxX())
		tsx.add(new Double(rect2.getMinX()));
	    if (rect2.getMaxX() > spacemanager.screenSpace.getMinX() && rect2.getMaxX() < spacemanager.screenSpace.getMaxX())
		tsx.add(new Double(rect2.getMaxX()));
	    if (rect2.getMinY() > spacemanager.screenSpace.getMinX() && rect2.getMinY() < spacemanager.screenSpace.getMaxX())
		tsy.add(new Double(rect2.getMinY()));
	    if (rect2.getMaxY() > spacemanager.screenSpace.getMinX() && rect2.getMaxY() < spacemanager.screenSpace.getMaxX())
		tsy.add(new Double(rect2.getMaxY()));
	}
	double minx, miny, maxx, maxy,px,py;
	Iterator keepy = tsy.iterator();
	maxy = ((Double)keepy.next()).doubleValue();
	int tmpoverlap;
	while (keepy.hasNext()){
	    miny = maxy;
	    maxy = ((Double)keepy.next()).doubleValue();
	    
	    Iterator keepx = tsx.iterator();
	    maxx = ((Double)keepx.next()).doubleValue();
	    
	    while (keepx.hasNext()){
		minx = maxx;
		maxx = ((Double)keepx.next()).doubleValue();
		Rectangle3d rect3 = new Rectangle3d(minx,miny,0.,maxx,maxy,1.);
		tmpoverlap = spacemanager.spaceRectangles.windowQueryWithoutEdges(rect3).size();
		if (ov <= 0){
		    if (tmpoverlap>maxNumOverlap){ //numberOfOverlapRect){
			overlapRectangles.removeAllElements();
			overlapRectangles.add(new Rectangle3d(rect3));
			numberOfOverlapRect = tmpoverlap;
			maxNumOverlap = tmpoverlap;
		    } else if (tmpoverlap==maxNumOverlap){ // numberOfOverlapRect){
			overlapRectangles.add(new Rectangle3d(rect3));
		    }
		} else {
		    if (tmpoverlap>maxNumOverlap){ //numberOfOverlapRect){
			maxNumOverlap = tmpoverlap;
		    }
		    if (tmpoverlap==ov){
			overlapRectangles.add(new Rectangle3d(rect3));
			//			System.out.println("added Rectangle " + rect3 + " for total:" + overlapRectangles.size());
		    }
		}
	    }
	}
	//	System.out.println("end of calcOverlapRectangles(" + ov + ")\tmaxNumOverlap=" + maxNumOverlap);
	setNumberEntries(maxNumOverlap);
    }

    public void loadSpaceManager(File f){
	FileInputStream istream = null;
	ObjectInputStream q =null;
	try {
	    istream = new FileInputStream(f);
	    q = new ObjectInputStream(istream);
	    spacemanager = (SpaceManager3d) q.readObject();
	    frontViewSpaces.clear();
	    sideViewSpaces.clear();
	    sideViewObjects.clear();
	    frontViewObjects.clear();
	    frontViewSpaces.addAll(spacemanager.spaceTrees);
	    sideViewSpaces.addAll(spacemanager.spaceTrees);
	    sideViewObjects.addAll(spacemanager.objectVector);
	    frontViewObjects.addAll(spacemanager.objectVector);	    
	    spacemanager.addSpaceListener(spacelistener);
	    q.close();
	} catch (FileNotFoundException fnf){
	    System.out.println("File "+ f + " not found");
	    return;
	} catch (Exception e){
	    System.out.println("file f=" + f);
	    e.printStackTrace();
	}
	setListModelSize(spacemanager.getNumberSpaces(showinsiderect));
	if (debug){
	    spacemanager.debugSpaceManager3d();
	}
	drawScaleX = spacemanager.screenSpace.getWidth();
	drawScaleY = spacemanager.screenSpace.getHeight();
	drawScaleZ = spacemanager.screenSpace.getWidth();

	repaint();
    }
    public void saveSpaceManager(File f){
	FileOutputStream ostream=null;
	ObjectOutputStream p=null ;
	try {
	    ostream = new FileOutputStream(f);
	    p = new ObjectOutputStream(ostream);
	    p.writeObject(spacemanager);
	    p.flush();
	    ostream.close();
	} catch (Exception e){
	    e.printStackTrace();
	}
    }
    public void printAllRectangles(){
	if (spacemanager!=null)
	    spacemanager.printAllRectangles();
    }
    public void printSpaceTree(){
	spacemanager.printSpaceRectangles();
    }
    public void printObjectTree(){
	spacemanager.printObjectRectangles();
    }
    public void Debug(boolean b){
	debug=b;
	spacemanager.setDebug(debug);
    }
    int setMode(int m){
	if (m !=MOVING || m!=MOVINGFAST){
	    movingRectangle=null;
	}
	if (m !=DELETING){
	    deleting=false;
	}
	if (m !=ADDING){
	    drawing = false;
	}
	if (m==ADDING){
	    addingStage=INITSTAGE;
	}
	return(mode=m);
    }
    public void setList(JList jl){
	jlist = jl;
    }
    private boolean zero=false;
    private void setListModelSize(int newSize){
    int dlms = jlist.getModel().getSize();
    //    System.out.println("setListModelSize called: newSize=" + newSize + " dlms=" + dlms + "\t zero=" + zero);
    if (newSize==dlms){
      return;
    }
    /*    if (zero){
	defaultlistmodel.removeAllElements();
	zero=false;
	}*/
    if (dlms>newSize){
	jlist.clearSelection();
	defaultlistmodel.removeRange(newSize,defaultlistmodel.size()-1);
    } else {
	for (; dlms<newSize; dlms++){
	    defaultlistmodel.addElement(String.valueOf(dlms+1));
	}
    }
    /*    if (newSize==0){
	defaultlistmodel.addElement("0");
	zero=true;
	}*/
    
  }
  public void setSizeVars(){ 
    getSize(size);
    width = (double)size.width/2.;
    height = (double)size.height;
  }
    void testClone(){
	SpaceManager3d sm = (SpaceManager3d)spacemanager.clone();
	spacemanager = sm;
	repaint();
    }
    void resetSpaceManager(){
      try {
	  spacemanager = new SpaceManager3d(screenSpace=new Rectangle3d(0.,0.,0.,1.,1.,1.));
	  drawScaleX=1.;
	  drawScaleY=1.;
	  drawScaleZ=1.;
	  setListModelSize(spacemanager.getNumberSpaces(showinsiderect));
	  spacemanager.setDebug(false);
	  frontViewSpaces.clear();
	  frontViewObjects.clear();
	  sideViewSpaces.clear();
	  sideViewObjects.clear();
	  frontViewSpaces.add(screenSpace);
	  sideViewSpaces.add(screenSpace);
	  spacemanager.addSpaceListener(spacelistener);
      } catch (Exception e){
	  e.printStackTrace();
      }	
    }
    JSplitPane splitpane=null;

    TreeSet frontViewSpaces, sideViewSpaces,frontViewObjects, sideViewObjects;

    private void addViewObject(Rectangle3d r){
	frontViewObjects.add(r);
	sideViewObjects.add(r);
    }
    private void removeViewObject(Rectangle3d r){
	frontViewObjects.remove(r);
	sideViewObjects.remove(r);
    }
    private SpaceManager3d.SpaceListener spacelistener;
    SpaceManagerPanel3d(DefaultListModel dlm,JList jl){
      setLayout (new BorderLayout());
      //      splitpane=new JSplitPane(JSplitPane.HORIZONTAL_SPLIT);
      //      add("Center",splitpane);
      //      splitpane.setDividerLocation(.5);
      //      splitpane.setRightComponent();
      //      splitpane.setLeftComponent();
      try {
	  spacemanager = new SpaceManager3d(screenSpace=new Rectangle3d(0.,0.,0.,1.,1.,1.));
      } catch (Exception e){
	  e.printStackTrace();
      } 
      frontViewSpaces = new TreeSet(new MinZReverseComparator3d());
      sideViewSpaces = new TreeSet(new MaxXComparator3d());
      frontViewSpaces.add(screenSpace);
      sideViewSpaces.add(screenSpace);
      frontViewObjects = new TreeSet(new MinZReverseComparator3d());
      sideViewObjects = new TreeSet(new MaxXComparator3d());

      spacemanager.addSpaceListener(spacelistener=new SpaceManager3d.SpaceListener(){
	      public void addSpace(Rectangle3d rect){
		  frontViewSpaces.add(rect);
		  sideViewSpaces.add(rect);
	      }
	      public boolean deleteSpace(Rectangle3d rect){
		  return (frontViewSpaces.remove(rect) && sideViewSpaces.remove(rect));
	      }
	  });
      drawScaleX=1.;
      drawScaleY=1.;
      drawScaleZ=1.;
    spacemanager.setDebug(debug);
    defaultlistmodel = dlm;
    
    setList(jl);
    setListModelSize(spacemanager.getNumberSpaces(showinsiderect));
    //	  float dash[] = new float[1];
    //	  dash[0] = 2.f;
    //	  slist = new Stroke[1];
    //	  slist[0] = new BasicStroke(4,BasicStroke.CAP_BUTT,BasicStroke.JOIN_BEVEL,1.f,dash,2.f);
    int width=5;
    tplist = new TexturePaint[clist.length*width];
    
    for (int i=0; i<tplist.length; i++){
      BufferedImage bi = new BufferedImage(width, width, BufferedImage.TYPE_INT_ARGB);
      Graphics2D gi = bi.createGraphics();
      gi.setColor(transparent);
      gi.fillRect(0,0,width,width);
      gi.setColor(clist[i%clist.length]);
      gi.drawLine(i%width,0,i%width,width);
      gi.drawLine(0,i%width,width,i%width); 
      //      gi.draw(new Rectangle2D.Float(0,0,5,5));
      tplist[i] = new TexturePaint(bi,new Rectangle(0,0,width,width));
    }
    addMouseListener(this);
    addMouseMotionListener(this);
    addComponentListener( new ComponentAdapter() {
      public void componentResized(ComponentEvent e){
	firstTime=true;
	setSizeVars();
      }
    });
  }
  public void mouseClicked(MouseEvent e){
  }
    private boolean movef;
  public void mousePressed(MouseEvent e){
    if (mode==ADDING){
	if (addingStage==INITSTAGE && !drawing){
	    if (gridon){
		Point p=e.getPoint();
		int incY=(int)height/20, incX=(int)width/20;
		startingPoint = new Point(incX*(p.x/incX),incY*(p.y/incY));
		//		System.out.println("incX=" + incX + " incY=" + incY + " p="+ p + " startingPoint=" + startingPoint);
	    } else {
		startingPoint = e.getPoint();
	    }
	    drawing = true;
	    mousePlace = startingPoint;
	} else if (addingStage==DEPTHSTAGE){
	    addRectDepth=addingRectangle2.getWidth();
	    //	    System.out.println("addRectDepth=" + addRectDepth);
	    addingStage=MOVESTAGE;
	} else if (addingStage==MOVESTAGE){
	    FullRectangle3d rect = new FullRectangle3d(0L,
						       drawScaleX*addingRectangle1.getX()/width,
						       drawScaleY*addingRectangle1.getY()/height,
						       drawScaleZ*(addingRectangle2.getX()-width)/width,
						       drawScaleX*addingRectangle1.getMaxX()/width,
						       drawScaleY*addingRectangle1.getMaxY()/height,
						       drawScaleZ*(addingRectangle2.getMaxX()-width)/width);
	    //	    System.out.println("rect=" + rect);
	    spacemanager.addRectangle(rect,-1);
	    addViewObject(rect);
	    int nS = spacemanager.getNumberSpaces(showinsiderect);
	    setListModelSize(nS);
	    
	    addingRectangle1 = addingRectangle2 = null;
	    addingStage = INITSTAGE;
	    repaint();
	    drawing=false;
	}
    } else if (mode==DELETING){
      
    } else if (mode==MOVING || mode==MOVINGFAST){
	double mx,my;
	mx = drawScaleX*e.getPoint().getX()/width;
	my = drawScaleY*e.getPoint().getY()/height;

	if (mx>1.0){
	    mx -=1.0;
	    movingRectangle = getClosestRectangleX(new Rectangle3d(0,my,mx,1.,my,mx));
	    movef=false;
	} else {
	    movingRectangle = getClosestRectangle(new Rectangle3d(mx,my,.0,mx,my,1.));
	    movef=true;
	}
	mousePlace = e.getPoint();
    }
  }
    public FullRectangle3d getClosestRectangleX(Rectangle3d rect){
	Vector v=spacemanager.getAllOverlapObjects(rect);
	double closVal=Double.MIN_VALUE;
	FullRectangle3d tmpRect=null;
	FullRectangle3d retRect=null;
	for (Enumeration e=v.elements();e.hasMoreElements();){
	    tmpRect=(FullRectangle3d)e.nextElement();
	    if (tmpRect.getMaxX()>closVal){
		retRect=tmpRect;
	    }
	}
	return(retRect);
    }
    public FullRectangle3d getClosestRectangle(Rectangle3d rect){
	Vector v=spacemanager.getAllOverlapObjects(rect);
	double closVal=Double.MAX_VALUE;
	FullRectangle3d tmpRect=null;
	FullRectangle3d retRect=null;
	for (Enumeration e=v.elements();e.hasMoreElements();){
	    tmpRect=(FullRectangle3d)e.nextElement();
	    if (tmpRect.getMinX()<closVal){
		retRect=tmpRect;
	    }
	}
	return(retRect);
    }
    public void addRandomRectangles(int num){
	double sx, sy, sz;
	for (int i=0; i<num; i++){
	    FullRectangle3d rect = new FullRectangle3d();
	    rect.setFrameFromDiagonal(sx=rand.nextDouble(),sy=rand.nextDouble(),sz=rand.nextDouble(),sx+rand.nextDouble()*.065,sy+rand.nextDouble()*.065,sz+rand.nextDouble()*.065);
	    spacemanager.addRectangle(rect,-1);
	    addViewObject(rect);
	    if (debug && spacemanager.debugSpaceManager3d()){
		System.err.println("SpaceManagerPanel3d::addRandomRectangles Quit after adding " + (i+1) + " rectangles");
		break;
	    };
	}
	setListModelSize(spacemanager.getNumberSpaces(showinsiderect));
    }
  public void mouseReleased(MouseEvent e){
    if (drawing && mode==ADDING){
	if (addingStage==INITSTAGE){
	    addingRectangle1 = new Rectangle(startingPoint);
	    if (gridon){
		Point p=e.getPoint();
		int incY=(int)height/20, incX=(int)width/20;
		addingRectangle1.add(new Point(incX*(p.x/incX),incY*(p.y/incY)));	  
	    } else {
		addingRectangle1.add(e.getPoint());
	    }
	    repaint();
	    addingStage=DEPTHSTAGE;
	    addingRectangle2 = new Rectangle(e.getPoint().x,addingRectangle1.y,Math.round((float)(2.f*((1.5f*width)-e.getPoint().x))),addingRectangle1.height);
	    //	    System.out.println("addingStage=DEPTHSTAGE");
	}
	/*
	  FullRectangle3d rect = new FullRectangle3d(0L,drawScaleX*addingRectangle1.getX()/width,
	  drawScaleY*addingRectangle1.getY()/height,
	  0.,
	  drawScaleX*addingRectangle1.getMaxX()/width,
	  drawScaleY*addingRectangle1.getMaxY()/height,
	  1.);
	  
	  spacemanager.addRectangle(rect,-1);
	  if (debug){
	  spacemanager.debugSpaceManager3d();
	  }
      int nS = spacemanager.getNumberSpaces(showinsiderect);
      setListModelSize(nS);
      if (shownumberrect && maxRectangleDisplay!=null){
	  maxRectangleDisplay.setText(Integer.toString(nS));
      }
      if (showoverlaprect && showoverlap!=null){
	  setNumberEntries( maxNumOverlap);//numberOfOverlapRect);
	  calcOverlapRectangles(numoverlap);
      }
      if (shownextnumberrect){
	  showNextNumberRectangles(-1);
	  //	  shownext.setSelectedIndex(shownext.getItemCount()-1);
      }
	  
      addingRectangle1 = null;
	*/
	//	drawing = false;
    } else if (mode==DELETING){
	double drwx, drwy;
	drwx = drawScaleX*e.getPoint().getX()/width;
	drwy = drawScaleY*e.getPoint().getY()/height;
	lastDeletedRectangle=new FullRectangle3d(0l,drwx,drwy,0.,drwx,drwy,1.);
	lastDeletedRectangle=getClosestRectangle(lastDeletedRectangle);
	if (lastDeletedRectangle!=null){
	    spacemanager.deleteRectangle(lastDeletedRectangle);
	    removeViewObject(lastDeletedRectangle);
	} else {
	    return;
	}
	if (debug){
	    spacemanager.debugSpaceManager3d();
	}
	int nS = spacemanager.getNumberSpaces(showinsiderect);
	setListModelSize(nS);
	if (shownumberrect && maxRectangleDisplay!=null){
	    maxRectangleDisplay.setText(Integer.toString(nS));
	}
	if (showoverlaprect && showoverlap!=null){
	    calcOverlapRectangles(numoverlap);
	    setNumberEntries(maxNumOverlap);//numberOfOverlapRect);
	}
	if (shownextnumberrect){
	    showNextNumberRectangles(-1);
	    //	    shownext.setSelectedIndex(shownext.getItemCount()-1);
	}
	repaint();
    } else if (mode==MOVING && movingRectangle!=null){
	setListModelSize(spacemanager.getNumberSpaces(showinsiderect));
	movingRectangle = null;
    } else if (mode==MOVINGFAST && movingRectangle!=null){
	mode=MOVING;
	mouseDragged(e);
	setListModelSize(spacemanager.getNumberSpaces(showinsiderect));
	movingRectangle = null;
	mode=MOVINGFAST;
    }
  }
  public void mouseEntered(MouseEvent e){
  }
  public void mouseExited(MouseEvent e){
  }
  public void mouseDragged(MouseEvent e){
      //	System.out.println("drawing=" + drawing);
    if (drawing){
	if (addingStage==INITSTAGE){
	    addingRectangle1 = new Rectangle(startingPoint);
	    addingRectangle1.add(e.getPoint());
	    Rectangle tmpRect = new Rectangle(addingRectangle1);
	    tmpRect.add(mousePlace);
	    mousePlace = e.getPoint();
	    repaint(tmpRect.x-1, tmpRect.y-1, tmpRect.width+2, tmpRect.height+2);
	}
    } else if (mode==MOVING && movingRectangle!=null){
      Point2D newMousePlace = e.getPoint();
      if (gridon){
	  int incY=(int)height/20, incX=(int)width/20;
	  int moveX=(int)(newMousePlace.getX()-mousePlace.getX()),// /width,
	      moveY=(int)(newMousePlace.getY()-mousePlace.getY());// /height;
	  if (Math.abs(moveX)<incX && Math.abs(moveY)<incY){
	      return;
	  } else {
	      moveX=(int)incX*(moveX/incX);
	      moveY=(int)incY*(moveY/incY);
	      //	      System.out.println("moveX=" + moveX + " moveY=" + moveY + " drawScaleX=" + drawScaleX + " drawScaleY=" + drawScaleY);
	      if (movef){
		  spacemanager.moveRectangle(movingRectangle,drawScaleX*(double)moveX/width,drawScaleY*(double)moveY/height,.0);
	      } else {
		  spacemanager.moveRectangle(movingRectangle,0,drawScaleY*(double)moveY/height,drawScaleX*(double)moveX/width);
	      }
	  }
      } else if (isMoveDeleteAdd){
	  if (movef){
	      spacemanager.moveRectangle(movingRectangle,drawScaleX*(newMousePlace.getX()-mousePlace.getX())/width, drawScaleY*(newMousePlace.getY()-mousePlace.getY())/height,.0);
	  } else {
	      spacemanager.moveRectangle(movingRectangle,0.,drawScaleY*(newMousePlace.getY()-mousePlace.getY())/height, drawScaleX*(newMousePlace.getX()-mousePlace.getX())/width);//drawScaleY*(newMousePlace.getY()-mousePlace.getY())/height,.0);
	  }
      } else {
	  SpaceManager3d sm=spacemanager;
	  try {
	      spacemanager = new SpaceManager3d(screenSpace);
	      //	      System.out.println("created new SpaceManager");
	  } catch (Exception ex){
	      ex.printStackTrace();
	  }
	  for (Iterator i=sm.objectIterator(); i.hasNext();){
	      FullRectangle3d r=(FullRectangle3d)i.next();
	      //	      System.out.println("adding new rectangle r=" + r + "to new SpaceManager");
	      if (r==movingRectangle){
		  double changex, changey;
		  changex=drawScaleX*(newMousePlace.getX()-mousePlace.getX())/width;
		  changey=drawScaleY*(newMousePlace.getY()-mousePlace.getY())/height;
		  movingRectangle.setRect(r.getMinX()+changex, r.getMinY()+changey,.0, r.getMaxX()+changex, r.getMaxY()+changey,.0);
		  movingRectangle=spacemanager.addRectangle(movingRectangle,-1);
		  addViewObject(movingRectangle);
	      } else {
		  spacemanager.addRectangle(r,-1);
		  addViewObject(r);
	      }
	  }
      }
      
      if (debug && spacemanager.debugSpaceManager3d()){
	  movingRectangle=null;
      } else {
	  int nS = spacemanager.getNumberSpaces(showinsiderect);
	  if (shownumberrect && maxRectangleDisplay!=null){
	      maxRectangleDisplay.setText(Integer.toString(nS));
	  }
	  if (showoverlaprect && showoverlap!=null){
	      calcOverlapRectangles(numoverlap);
	      setNumberEntries(maxNumOverlap);//numberOfOverlapRect);
	  }
	  if (shownextnumberrect){
	      showNextNumberRectangles(-1);
	      //	      shownext.setSelectedIndex(shownext.getItemCount()-1);
	  }
      }
      mousePlace = newMousePlace;
      repaint();
    } else if (mode==MOVINGFAST && movingRectangle!=null){
	Point2D newMousePlace = e.getPoint();
	movingRectangleOutline=new Rectangle((int)(drawScaleX*movingRectangle.getMinX()),(int)(drawScaleY*movingRectangle.getMinY()),(int)(drawScaleX*movingRectangle.getWidth()),(int)(drawScaleY*movingRectangle.getHeight()));
	repaint(movingRectangleOutline.x-1,movingRectangleOutline.y-1,movingRectangleOutline.width+2,movingRectangleOutline.height+2);
    }
  }
  public void mouseMoved(MouseEvent e){
      if (addingStage==DEPTHSTAGE){
	  Rectangle tmpRect = new Rectangle(addingRectangle2);
	  int startP ;
	  if (e.getPoint().x > (1.5f*width)){
	      startP = (3*(int)width)-(int)e.getPoint().x;
	  } else {
	      startP = e.getPoint().x;
	  }
	  addingRectangle2 = new Rectangle(startP,addingRectangle1.y,Math.round((float)(2.f*(Math.abs((1.5f*width)-e.getPoint().x)))),addingRectangle1.height);
	  tmpRect.add(addingRectangle2);
	  repaint(tmpRect.x-1,tmpRect.y-1,tmpRect.width+2,tmpRect.height+2);
      } else if (addingStage==MOVESTAGE){
	  int mid,lef,rig;
	  Rectangle tmpRect = new Rectangle(addingRectangle2);
	  mid = e.getPoint().x;
	  lef = mid - (int)(addRectDepth/2.);
	  addingRectangle2 = new Rectangle(lef,addingRectangle2.y,
					   (int)addRectDepth, addingRectangle2.height);
	  tmpRect.add(addingRectangle2);
	  repaint(tmpRect.x-1,tmpRect.y-1,tmpRect.width+2,tmpRect.height+2);
      }
  }
  public void actionPerformed(ActionEvent e){
  }
  Color clist[] = {
    Color.red,
    Color.blue,
    new Color(119,51,131), // purple
    new Color(103,151,236), // carolina blue
    new Color(247,140,13), // orange
    new Color(35,147,23), // green
    Color.yellow,
    Color.magenta
  };

  Stroke slist[] ;
  TexturePaint tplist[];
  boolean firstTime = true;
  BufferedImage bi;
  Graphics2D big;
    FullRectangle3d lastDeletedRectangle=null;
  public void paint(Graphics g){
      Graphics2D g2 = (Graphics2D) g;
      
      if (firstTime){
	  bi = (BufferedImage) createImage(size.width,size.height);
	  big = bi.createGraphics();
	  firstTime = false;
      }
      big.setColor(getBackground());
      big.clearRect(0,0,size.width,size.height);
      
      if (gridon){
	  big.setColor(blacktrans2);
	  for (int d=0; d<width; d+=width/20){
	      big.drawLine(d,0,d,(int)height);
	  }
	  for (int d=0; d<height; d+=height/20){
	      big.drawLine(0,d,(int)width,d);
	  }
      }

      Hashtable spacehash = new Hashtable();
      int a=0;
      //      System.out.println("start printing spaces");
      for (Iterator i=spacemanager.spaceIterator(); i.hasNext();){
	  Rectangle3d tmpRect=(Rectangle3d)i.next();
	  //	  System.out.println("tmpRect=" + tmpRect);
	  spacehash.put( tmpRect,new Integer(a++));
      }
      
      Rectangle3d tmpRect=null;
      Rectangle3d rs = new Rectangle3d();
      Iterator i=frontViewSpaces.iterator();
      Iterator i2=frontViewObjects.iterator();
      Rectangle3d nextSpace=null;
      if (i.hasNext()){
	  nextSpace = (Rectangle3d)i.next();
      }
      FullRectangle3d nextObject=null;
      if (i2.hasNext()){
	  nextObject = (FullRectangle3d)i2.next();
      }
      boolean cont=(nextObject!=null || nextSpace!=null);
      boolean isspace=false;
      Rectangle3d currSpace=null;
      FullRectangle3d currObject=null;
      a=-1;
      int spacecount=0;
      while(cont){
	  if (nextObject==null){
	      currSpace = nextSpace;
	      if (i.hasNext()){
		  nextSpace = (Rectangle3d)i.next();
	      } else {
		  nextSpace = null;
		  cont = false;
	      }
	      isspace=true;
	  } else if (nextSpace==null){
	      currObject = (FullRectangle3d)nextObject;
	      if (i2.hasNext()){
		  nextObject = (FullRectangle3d)i2.next();
	      } else {
		  cont = false;
		  nextObject =null;
	      }
	      isspace=false;
	  } else {
	      if (nextSpace.getMinZ()<nextObject.getMinZ()){
		  currObject = nextObject;
		  if (i2.hasNext()){
		      nextObject = (FullRectangle3d)i2.next();
		  } else {
		      nextObject = null;
		  }
		  isspace=false;
	      } else {
		  currSpace = nextSpace;
		  if (i.hasNext()){
		      nextSpace = (Rectangle3d)i.next();
		  } else {
		      nextSpace = null;
		  }
		  isspace=true;
	      }
	      if (nextSpace==null && nextObject==null){
		  cont=false;
	      }
	  }
	  //i.hasNext();){
	  //	  System.out.println("isspace=" + isspace + "\tcurrSpace=" + currSpace + "\tcurrObject=" +currObject);
	  if (isspace){
	      // space it is!!
	      tmpRect = currSpace;
	      rs.setRect(tmpRect.getMinX()/drawScaleX,
			 tmpRect.getMinY()/drawScaleY,
			 tmpRect.getMinZ()/drawScaleZ,
			 tmpRect.getMaxX()/drawScaleX,
			 tmpRect.getMaxY()/drawScaleY,
			 tmpRect.getMaxZ()/drawScaleZ);
	      if (showinsiderect &&
		  (rs.getMinX()==screenSpace.getMinX() || 
		   rs.getMinY()==screenSpace.getMinY() || 
		   rs.getMinZ()==screenSpace.getMinZ() || 
		   rs.getMaxX()==screenSpace.getMaxX() ||
		   rs.getMaxY()==screenSpace.getMaxY() ||
		   rs.getMaxZ()==screenSpace.getMaxZ())){
		  continue;
	      }
	      //	      System.out.println("getting tmpRect=" + tmpRect + "\thash=" + spacehash.get(tmpRect));
	      int spacenum = ((Integer)spacehash.get(tmpRect)).intValue();
	      if (jlist.isSelectedIndex(spacenum)){
		  big.setPaint(blacktrans);
		  big.fillRect(Math.round((float)(rs.getMinX()*width)),
			       Math.round((float)(rs.getMinY()*height)),
			       Math.round((float)(rs.getWidth()*width)),
			       Math.round((float)(rs.getHeight()*height)));
		  big.setPaint(Color.white);
	      } else {
		  big.setPaint(Color.black);
		  //	big.setPaint(Color.black);
		  big.drawRect(Math.round((float)(rs.getMinX()*width)),
			       Math.round((float)(rs.getMinY()*height)),
			       Math.round((float)(rs.getWidth()*width)-1),
			       Math.round((float)(rs.getHeight()*height-1)));
		  big.setPaint(Color.gray);
		  //	big.setPaint(tplist[++a%tplist.length]);
	      }
	      //      System.out.println("paint: Rectangle #" + a + " X=" + rs.getX() + " Y=" + rs.getY() + " maxX=" + rs.getMaxX() + " maxY=" + rs.getMaxY());
	      big.drawString(String.valueOf(spacenum+1),Math.round((float)rs.getCenterX()*width), Math.round((float)rs.getCenterY()*height));
	      spacecount++;
	  } else {
	      // it is an object
	      tmpRect = currObject;
	      rs.setRect(tmpRect.getMinX()/drawScaleX,
			 tmpRect.getMinY()/drawScaleY,
			 tmpRect.getMinZ()/drawScaleZ,
			 tmpRect.getMaxX()/drawScaleX,
			 tmpRect.getMaxY()/drawScaleY,
			 tmpRect.getMaxZ()/drawScaleZ);	  
	      if (showinsiderect &&
		  (rs.getMinX()==screenSpace.getMinX() || rs.getMinY()==screenSpace.getMinY() || rs.getMinZ()==screenSpace.getMinZ() ||
		   rs.getMaxX()==screenSpace.getMaxX() || rs.getMaxY()==screenSpace.getMaxY() || rs.getMaxZ()==screenSpace.getMaxZ())){
		  continue;
	      }
	      big.setColor(clist[currObject.id.intValue()%clist.length]);
	      if (rs.getMinX()>1.){
		  continue;
	      } else if (rs.getMaxX()>1.){
		  big.clearRect(Math.round((float)(rs.getMinX()*width)),
			       Math.round((float)(rs.getMinY()*height)),
			       Math.round((float)(rs.getWidth()*width)),
			       Math.round((float)(rs.getHeight()*height)));
		  big.fillRect(Math.round((float)(rs.getMinX()*width)),
			       Math.round((float)(rs.getMinY()*height)),
			       Math.round((float)(rs.getWidth()*width)),
			       Math.round((float)(rs.getHeight()*height)));
	      } else {
		  big.clearRect(Math.round((float)(rs.getMinX()*width)),
			       Math.round((float)(rs.getMinY()*height)),
			       Math.round((float)(rs.getWidth()*width)),
			       Math.round((float)(rs.getHeight()*height)));
		  big.fillRect(Math.round((float)(rs.getMinX()*width)),
			       Math.round((float)(rs.getMinY()*height)),
			       Math.round((float)(rs.getWidth()*width)),
			       Math.round((float)(rs.getHeight()*height)));
	      }
	      if (rs.getCenterX()<1.){
		  big.setColor(Color.black);
		  big.drawString(String.valueOf(currObject.id.intValue()+1),Math.round((float)rs.getCenterX()*width), Math.round((float)rs.getCenterY()*height));
		  //		  big.drawString(String.valueOf(a+1),Math.round((float)(1.+rs.getCenterZ())*width), Math.round((float)rs.getCenterY()*height));
	      }
	  }
      }

      tmpRect=null;
      i=sideViewSpaces.iterator();
      i2=sideViewObjects.iterator();
      nextSpace=null;
      if (i.hasNext()){
	  nextSpace = (Rectangle3d)i.next();
      }
      nextObject=null;
      if (i2.hasNext()){
	  nextObject = (FullRectangle3d)i2.next();
      }
      cont=(nextObject!=null || nextSpace!=null);
      isspace=false;
      currSpace=null;
      currObject=null;
      a=-1;
      spacecount=0;
      while(cont){
	  if (nextObject==null){
	      currSpace = nextSpace;
	      if (i.hasNext()){
		  nextSpace = (Rectangle3d)i.next();
	      } else {
		  nextSpace = null;
		  cont = false;
	      }
	      isspace=true;
	  } else if (nextSpace==null){
	      currObject = (FullRectangle3d)nextObject;
	      if (i2.hasNext()){
		  nextObject = (FullRectangle3d)i2.next();
	      } else {
		  cont = false;
		  nextObject =null;
	      }
	      isspace=false;
	  } else {
	      if (nextSpace.getMaxX()>nextObject.getMaxX()){
		  currObject = nextObject;
		  if (i2.hasNext()){
		      nextObject = (FullRectangle3d)i2.next();
		  } else {
		      nextObject = null;
		  }
		  isspace=false;
	      } else {
		  currSpace = nextSpace;
		  if (i.hasNext()){
		      nextSpace = (Rectangle3d)i.next();
		  } else {
		      nextSpace = null;
		  }
		  isspace=true;
	      }
	      if (nextSpace==null && nextObject==null){
		  cont=false;
	      }
	  }
	  //i.hasNext();){
	  //	  System.out.println("isspace=" + isspace + "\tcurrSpace=" + currSpace + "\tcurrObject=" +currObject);
	  if (isspace){
	      // space it is!!
	      tmpRect = currSpace;
	      rs.setRect(tmpRect.getMinX()/drawScaleX,
			 tmpRect.getMinY()/drawScaleY,
			 tmpRect.getMinZ()/drawScaleZ,
			 tmpRect.getMaxX()/drawScaleX,
			 tmpRect.getMaxY()/drawScaleY,
			 tmpRect.getMaxZ()/drawScaleZ);
	      //	      System.out.println("space rs=" + rs);
	      if (showinsiderect &&
		  (rs.getMinX()==screenSpace.getMinX() || 
		   rs.getMinY()==screenSpace.getMinY() || 
		   rs.getMinZ()==screenSpace.getMinZ() || 
		   rs.getMaxX()==screenSpace.getMaxX() ||
		   rs.getMaxY()==screenSpace.getMaxY() ||
		   rs.getMaxZ()==screenSpace.getMaxZ())){
		  continue;
	      }
	      //	      System.out.println("getting tmpRect=" + tmpRect + "\thash=" + spacehash.get(tmpRect));
	      int spacenum = ((Integer)spacehash.get(tmpRect)).intValue();
	      if (jlist.isSelectedIndex(spacenum)){
		  big.setPaint(blacktrans);
		  big.fillRect(Math.round((float)((1.+rs.getMinZ())*width)),
			       Math.round((float)(rs.getMinY()*height)),
			       Math.round((float)(rs.getDepth()*width)),
			       Math.round((float)(rs.getHeight()*height)));
		  big.setPaint(Color.white);
	      } else {
		  big.setPaint(Color.black);
		  //	big.setPaint(Color.black);
		  big.drawRect(Math.round((float)((1.+rs.getMinZ())*width)),
			       Math.round((float)(rs.getMinY()*height)),
			       Math.round((float)(rs.getDepth()*width)-1),
			       Math.round((float)(rs.getHeight()*height-1)));
		  big.setPaint(Color.gray);
		  //	big.setPaint(tplist[++a%tplist.length]);
	      }
	      //      System.out.println("paint: Rectangle #" + a + " X=" + rs.getX() + " Y=" + rs.getY() + " maxX=" + rs.getMaxX() + " maxY=" + rs.getMaxY());
	      big.drawString(String.valueOf(spacenum+1),Math.round((float)(1.+rs.getCenterZ())*width), Math.round((float)rs.getCenterY()*height));
	      spacecount++;
	  } else {
	      tmpRect = currObject;
	      // it is an object
	      rs.setRect(tmpRect.getMinX()/drawScaleX,
			 tmpRect.getMinY()/drawScaleY,
			 tmpRect.getMinZ()/drawScaleZ,
			 tmpRect.getMaxX()/drawScaleX,
			 tmpRect.getMaxY()/drawScaleY,
			 tmpRect.getMaxZ()/drawScaleZ);	  
	      if (showinsiderect &&
		  (rs.getMinX()==screenSpace.getMinX() || rs.getMinY()==screenSpace.getMinY() || rs.getMinZ()==screenSpace.getMinZ() ||
		   rs.getMaxX()==screenSpace.getMaxX() || rs.getMaxY()==screenSpace.getMaxY() || rs.getMaxZ()==screenSpace.getMaxZ())){
		  continue;
	      }
	      big.setColor(clist[currObject.id.intValue()%clist.length]);
	      if (rs.getMinX()>1.){
		  continue;
	      } else if (rs.getMaxX()>1.){
		  big.fillRect(Math.round((float)((1.+rs.getMinZ())*width)),
			       Math.round((float)(rs.getMinY()*height)),
			       Math.round((float)(rs.getDepth()*width)),
			       Math.round((float)(rs.getHeight()*height)));
	      } else {
		  big.fillRect(Math.round((float)((1.+rs.getMinZ())*width)),
			       Math.round((float)(rs.getMinY()*height)),
			       Math.round((float)(rs.getDepth()*width)),
			       Math.round((float)(rs.getHeight()*height)));
	      }
	      if (rs.getCenterX()<1.){
		  big.setColor(Color.black);
		  //		  big.drawString(String.valueOf(a+1),Math.round((float)rs.getCenterX()*width), Math.round((float)rs.getCenterY()*height));
		  big.drawString(String.valueOf(currObject.id.intValue()+1),Math.round((float)(1.+rs.getCenterZ())*width), Math.round((float)rs.getCenterY()*height));
	      }
	  }
      }

	  /*	  int a=-1;
		  big.setPaintMode();
		  while (i.hasNext()){

		  if (jlist.isSelectedIndex(a+1)){
		  big.setPaint(blacktrans);
		  big.fillRect(Math.round((float)(rs.getMinX()*width)),
		  Math.round((float)(rs.getMinY()*height)),
		  Math.round((float)(rs.getWidth()*width)),
		  Math.round((float)(rs.getHeight()*height)));
		  big.fillRect(Math.round((float)((1.+rs.getMinZ())*width)),
		  Math.round((float)(rs.getMinY()*height)),
		  Math.round((float)(rs.getDepth()*width)),
		  Math.round((float)(rs.getHeight()*height)));
		  big.setPaint(Color.white);
		  } else {
		  big.setPaint(Color.black);
		  //	big.setPaint(Color.black);
		  big.drawRect(Math.round((float)(rs.getMinX()*width)),
		  Math.round((float)(rs.getMinY()*height)),
		  Math.round((float)(rs.getWidth()*width)-1),
		  Math.round((float)(rs.getHeight()*height-1)));
		  big.drawRect(Math.round((float)((1+rs.getMinZ())*width)),
		  Math.round((float)(rs.getMinY()*height)),
		  Math.round((float)(rs.getDepth()*width)-1),
		  Math.round((float)(rs.getHeight()*height-1)));
		  big.setPaint(Color.gray);
		  //	big.setPaint(tplist[++a%tplist.length]);
		  }
		  //      System.out.println("paint: Rectangle #" + a + " X=" + rs.getX() + " Y=" + rs.getY() + " maxX=" + rs.getMaxX() + " maxY=" + rs.getMaxY());
		  a++;
		  big.drawString(String.valueOf(a+1),Math.round((float)rs.getCenterX()*width), Math.round((float)rs.getCenterY()*height));
		  big.drawString(String.valueOf(a+1),Math.round((float)(1.+rs.getCenterZ())*width), Math.round((float)rs.getCenterY()*height));
		  }
	  */

	  /*      Iterator i2 = spacemanager.objectIterator();
		  a=-1;
		  //      i2.next();i2.next();i2.next();i2.next(); // All 4 sides of the screen
		  tmpRect=null;
		  while (i2.hasNext()){
		  tmpRect = (Rectangle3d)i2.next();
		  rs.setRect(tmpRect.getMinX()/drawScaleX,
		  tmpRect.getMinY()/drawScaleY,
		  tmpRect.getMinZ()/drawScaleZ,
		  tmpRect.getMaxX()/drawScaleX,
		  tmpRect.getMaxY()/drawScaleY,
		  tmpRect.getMaxZ()/drawScaleZ);	  
		  if (showinsiderect &&
		  (rs.getMinX()==screenSpace.getMinX() || rs.getMinY()==screenSpace.getMinY() ||
		  rs.getMaxX()==screenSpace.getMaxX() || rs.getMaxY()==screenSpace.getMaxY())){
		  continue;
		  }
		  big.setColor(clist[++a%clist.length]);
		  if (rs.getMinX()>1.){
		  continue;
		  } else if (rs.getMaxX()>1.){
		  big.fillRect(Math.round((float)(rs.getMinX()*width)),
		  Math.round((float)(rs.getMinY()*height)),
		  Math.round((float)((1-rs.getMinX())*width)),
		  Math.round((float)(rs.getHeight()*height)));
		  big.fillRect(Math.round((float)((1.+rs.getMinZ())*width)),
		  Math.round((float)(rs.getMinY()*height)),
		  Math.round((float)((1-rs.getMinZ())*width)),
		  Math.round((float)(rs.getHeight()*height)));
		  } else {
		  big.fillRect(Math.round((float)(rs.getMinX()*width)),
		  Math.round((float)(rs.getMinY()*height)),
		  Math.round((float)(rs.getWidth()*width)),
		  Math.round((float)(rs.getHeight()*height)));
		  big.fillRect(Math.round((float)((1.+rs.getMinZ())*width)),
		  Math.round((float)(rs.getMinY()*height)),
		  Math.round((float)(rs.getDepth()*width)),
		  Math.round((float)(rs.getHeight()*height)));
		  }
		  if (rs.getCenterX()<1.){
		  big.setColor(Color.black);
		  big.drawString(String.valueOf(a+1),Math.round((float)rs.getCenterX()*width), Math.round((float)rs.getCenterY()*height));
		  big.drawString(String.valueOf(a+1),Math.round((float)(1.+rs.getCenterZ())*width), Math.round((float)rs.getCenterY()*height));
		  }
		  }
	  */
	  Rectangle3d lastObject=tmpRect;
	  
	  big.setColor(Color.black);
	  big.setStroke(dash);
	  if (addingRectangle1!=null){
	      big.draw(addingRectangle1);
	  }
	  if (addingRectangle2!=null){
	      big.draw(addingRectangle2);
	  }
	  big.setStroke(normal);
	  
	  if (mode==MOVINGFAST && movingRectangle!=null){
	      big.setColor(Color.black);
	      big.draw(movingRectangleOutline);
	  }
	  if (showoverlaprect || shownextnumberrect){
	      for (Enumeration e=overlapRectangles.elements();e.hasMoreElements();){
		  tmpRect = (Rectangle3d)e.nextElement();
		  rs.setRect(tmpRect.getMinX()/drawScaleX,
			     tmpRect.getMinY()/drawScaleY,
			     .0,
			     tmpRect.getMaxX()/drawScaleX,
			     tmpRect.getMaxY()/drawScaleY,
			     1.);	  
		  big.setPaint(blacktrans);
		  big.fillRect(Math.round((float)(rs.getMinX()*width)),
			       Math.round((float)(rs.getMinY()*height)),
			       Math.round((float)(rs.getWidth()*width)),
			       Math.round((float)(rs.getHeight()*height)));
		  big.fillRect(Math.round((float)((1.+rs.getMinZ())*width)),
			       Math.round((float)(rs.getMinY()*height)),
			       Math.round((float)(rs.getDepth()*width)),
			       Math.round((float)(rs.getHeight()*height)));
		  big.setColor(Color.black);
	      }
	  } else {
	      
	      //	  Rectangle3d closest=spacemanager.getClosestWithExactDimension(lastObject,lastObject.getWidth(),lastObject.getHeight());
	      
	      
	      if (showClosest && lastDeletedRectangle !=null){
		  Rectangle3d closest=spacemanager.getClosestWithExactDimension(lastDeletedRectangle,lastDeletedRectangle.getWidth(),lastDeletedRectangle.getHeight());
		  
		  big.setPaint(blacktrans);
		  big.fillRect(Math.round((float)(closest.getMinX()*width)),
			       Math.round((float)(closest.getMinY()*height)),
			       Math.round((float)(closest.getWidth()*width)),
			       Math.round((float)(closest.getHeight()*height)));
		  Vector v=spacemanager.getClosestWithAtLeastDimension(lastDeletedRectangle,lastDeletedRectangle.getWidth(),lastDeletedRectangle.getHeight());
		  if (!v.isEmpty()){
		      closest=(Rectangle3d)v.firstElement();
		  } else {
		      closest=null;
		  }
		  //	      closest=spacemanager.getClosestWithAtLeastDimension(lastObject,lastObject.getWidth(),lastObject.getHeight());
		  big.setPaint(whitetrans);
		  /*	      big.fillRect(Math.round((float)(closest.getMinX()*width)),
			      Math.round((float)(closest.getMinY()*height)),
			      Math.round((float)(closest.getWidth()*width)),
			      Math.round((float)(closest.getHeight()*height)));
		  */
		  Rectangle lastD=new Rectangle((int)Math.round(lastDeletedRectangle.getX()*width),
						(int)Math.round(lastDeletedRectangle.getY()*height),
						(int)Math.round(lastDeletedRectangle.getWidth()*width),
						(int)Math.round(lastDeletedRectangle.getHeight()*height));
		  
		  big.setPaint(Color.black);
		  
		  big.draw(lastD);
	      }
	  }
	  
	  g2.drawImage(bi,0,0,this);
      }
  }

/*


    Graphics2D g2 = (Graphics2D) g;
    System.out.println("drawScaleX=" + drawScaleX + "\tdrawScaleY=" + drawScaleY);
    if (firstTime){
      bi = (BufferedImage) createImage(size.width,size.height);
      big = bi.createGraphics();
      firstTime = false;
    }
    big.setColor(getBackground());
    big.clearRect(0,0,size.width,size.height);


    Iterator i = spacemanager.spaceIterator();
    Rectangle2d rs;
    int a=-1;
    big.setPaintMode();
    while (i.hasNext()){
      rs = (Rectangle2d)i.next();
      if (showinsiderect &&
	  (rs.getMinX()==screenSpace.getMinX() || rs.getMinY()==screenSpace.getMinY() ||
	   rs.getMaxX()==screenSpace.getMaxX() || rs.getMaxY()==screenSpace.getMaxY())){
	  continue;
      }
      if (jlist.isSelectedIndex(a+1)){
	big.setPaint(blacktrans);
	big.fillRect(Math.round((float)(rs.getMinX()*width)),
		     Math.round((float)(rs.getMinY()*height)),
		     Math.round((float)(rs.getWidth()*width)),
		     Math.round((float)(rs.getHeight()*height)));
	big.setPaint(Color.white);
      } else {
	big.setPaint(Color.black);
	big.drawRect(Math.round((float)(rs.getMinX()*width)),
		     Math.round((float)(rs.getMinY()*height)),
		     Math.round((float)(rs.getWidth()*width)-1),
		     Math.round((float)(rs.getHeight()*height)-1));
	big.setPaint(Color.gray);
      }
      a++;
      big.drawString(String.valueOf(a+1),Math.round((float)rs.getCenterX()*width), Math.round((float)rs.getCenterY()*height));
    }
    Iterator i2 = spacemanager.objectIterator();
    a=-1;
    while (i2.hasNext()){
      rs = (Rectangle2d)i2.next();
      if (showinsiderect &&
	  (rs.getMinX()==screenSpace.getMinX() || rs.getMinY()==screenSpace.getMinY() ||
	   rs.getMaxX()==screenSpace.getMaxX() || rs.getMaxY()==screenSpace.getMaxY())){
	  continue;
      }
      big.setColor(clist[++a%clist.length]);
      big.fillRect(Math.round((float)(rs.getMinX()*width)),
		  Math.round((float)(rs.getMinY()*height)),
		  Math.round((float)(rs.getWidth()*width)),
		  Math.round((float)(rs.getHeight()*height)));
      big.setColor(Color.black);
      big.drawString(String.valueOf(a+1),Math.round((float)rs.getCenterX()*width), Math.round((float)rs.getCenterY()*height));
    }
    
    if (addingRectangle!=null){
      big.setColor(Color.black);
      big.draw(addingRectangle);
    }
    if (showoverlaprect || shownextnumberrect){
	Rectangle2d tmprect;
	for (Enumeration e=overlapRectangles.elements();e.hasMoreElements();){
	    tmprect = (Rectangle2d)e.nextElement();
	    big.setPaint(blacktrans);
	    big.fillRect(Math.round((float)(tmprect.getMinX()*width)),
			 Math.round((float)(tmprect.getMinY()*height)),
			 Math.round((float)(tmprect.getWidth()*width)),
			 Math.round((float)(tmprect.getHeight()*height)));
	}
    }
  }
}
*/
