package edu.columbia.cs.cgui.spam2i;

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

public class SpaceManagerPanel extends JPanel implements MouseListener, ActionListener, MouseMotionListener {
    private boolean debug=true,shownumberrect=true,showoverlaprect=false,shownextnumberrect=false,showinsiderect=false;
    SpaceManagerDisplayWrapper spacemanager;
    Point startingPoint;
    Point mousePlace;
    Color transparent = new Color(1.0f,1.0f,1.0f,.0f);
    Color blacktrans = new Color(0.f,0.f,0.f,.5f),whitetrans = new Color(1.f,1.f,1.f,.5f),blacktrans2 = new Color(0.f,0.f,0.f,.75f);
    Dimension size= new Dimension();
    double height,width;
    static Random rand = new Random();
    
    boolean drawing=false,deleting=false;
    Rectangle addingRectangle=null,movingRectangleOutline=null;
    FullRectangle2i movingRectangle=null;
    Rectangle2i screenSpace=null;
    Dimension screenSize = new Dimension();
    DefaultListModel defaultlistmodel;
    public static int ADDING=0x01;
    public static int DELETING=0x02;
    public static int MOVING=0x03;
    public static int MOVINGFAST=0x04;
    public static int MOVEOBJECTS=0x05;
    public static int SHOWENCLOSED=0x06;
    private int mode=ADDING;
    private int number_of_objects_to_move=0;
    private Vector objects_moving=null;
    private JList jlist;
    private JComboBox showoverlap,shownext;
    private Vector overlapRectangles=new Vector();
    private int numberOfOverlapRect=0,numoverlap,maxNumOverlap, numnext, maxNumNext;
    private JTextComponent maxRectangleDisplay;
    private boolean isMoveDeleteAdd=true,showClosest=false,showAllClosest=false,gridon=false;
    private double drawScaleX=1.;
    private double drawScaleY=1.;
    private Rectangle2i enclosedRectangle=null;
    private Rectangle enclosedRect =null;
    
    void clearComboBox(JComboBox cb){
	
	while (cb.getItemCount()>0){
	    cb.removeItemAt(0);
	}
    }
    public void setShowClosest(boolean b){
	showClosest=b;
	repaint();
    }
    public void setShowGrid(boolean b){
	gridon=b;
	repaint();
    }
    public void setShowOverlap(boolean b){
	showoverlaprect = b;
	if (!showoverlaprect){
	    showoverlap.removeItemListener(iloverlap);
	    clearComboBox(showoverlap);
	    showoverlap.addItemListener(iloverlap);
	}
	showOverlapRectangles(-1);
    }
    public void setShowInsideRect(boolean b){
	showinsiderect=b;
	setListModelSize(spacemanager.getNumberSpaces(showinsiderect));
    }
    public void setShowNextNumberRect(boolean b){
	//	System.out.println("setShowNextNumberRect b=" +b);
	shownextnumberrect = b;
	if (!shownextnumberrect){
	    shownext.removeItemListener(ilnext);
	    clearComboBox(shownext);
	    shownext.addItemListener(ilnext);
	}
    }
    public void setMaxRectangleDisplay(JTextComponent tc){
	maxRectangleDisplay = tc;
    }
    ItemListener iloverlap,ilnext;
    public void setNumberOverlapChooser(JComboBox cb){
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
    public void setNumberNextChooser(JComboBox cb){
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
	Rectangle2i rect2;
	Rectangle2i ss = spacemanager.getScreenSpace();
	tsx.add(new Integer(ss.getMinX()));
	tsx.add(new Integer(ss.getMaxX()));
	tsy.add(new Integer(ss.getMinY()));
	tsy.add(new Integer(ss.getMaxY()));
	while (oi.hasNext()){
	    rect2 = (Rectangle2i)oi.next();
	    if (rect2.getMinX() > ss.getMinX() && rect2.getMinX() < ss.getMaxX())
		tsx.add(new Integer(rect2.getMinX()));
	    if (rect2.getMaxX() > ss.getMinX() && rect2.getMaxX() < ss.getMaxX())
		tsx.add(new Integer(rect2.getMaxX()));
	    if (rect2.getMinY() > ss.getMinX() && rect2.getMinY() < ss.getMaxX())
		tsy.add(new Integer(rect2.getMinY()));
	    if (rect2.getMaxY() > ss.getMinX() && rect2.getMaxY() < ss.getMaxX())
		tsy.add(new Integer(rect2.getMaxY()));
	}
	int minx, miny, maxx, maxy,px,py;
	Iterator keepy = tsy.iterator();
	maxy = ((Integer)keepy.next()).intValue();
	int tmpnext,curnum;
	maxNumNext=-1;
	curnum = spacemanager.spaceTrees.size();
	while (keepy.hasNext()){
	    miny = maxy;
	    maxy = ((Integer)keepy.next()).intValue();
	    
	    Iterator keepx = tsx.iterator();
	    maxx = ((Integer)keepx.next()).intValue();
	    
	    while (keepx.hasNext()){
		minx = maxx;
		maxx = ((Integer)keepx.next()).intValue();
		int smx = (int)Math.ceil(.25*(maxx-minx));
		int smy = (int)Math.ceil(.25*(maxy-miny));
		FullRectangle2i rect3 = new FullRectangle2i(1L,minx+smx,miny+smy,maxx-smx,maxy-smy);
		//		tmpoverlap = spacemanager.spaceRectangles.windowQueryWithoutEdges(rect3).size();
		spacemanager.addRectangle(rect3,-1);
		tmpnext = spacemanager.spaceTrees.size();
		spacemanager.deleteRectangle(rect3);
		if (ov <= 0){
		    if (tmpnext>maxNumNext){ //numberOfOverlapRect){
			overlapRectangles.removeAllElements();
			overlapRectangles.add(new Rectangle2i(minx,miny,maxx,maxy));
			maxNumNext = tmpnext;
			//			numberOfOverlapRect = tmpoverlap;
			//			maxNumOverlap = tmpoverlap;
		    } else if (tmpnext==maxNumNext){ // numberOfOverlapRect){
			overlapRectangles.add(new Rectangle2i(minx,miny,maxx,maxy));
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
			overlapRectangles.add(new Rectangle2i(minx,miny,maxx,maxy));
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
	Rectangle2i rect2;
	Rectangle2i ss = spacemanager.getScreenSpace();
	tsx.add(new Double(ss.getMinX()));
	tsx.add(new Double(ss.getMaxX()));
	tsy.add(new Double(ss.getMinY()));
	tsy.add(new Double(ss.getMaxY()));
	while (oi.hasNext()){
	    rect2 = (Rectangle2i)oi.next();
	    if (rect2.getMinX() > ss.getMinX() && rect2.getMinX() < ss.getMaxX())
		tsx.add(new Double(rect2.getMinX()));
	    if (rect2.getMaxX() > ss.getMinX() && rect2.getMaxX() < ss.getMaxX())
		tsx.add(new Double(rect2.getMaxX()));
	    if (rect2.getMinY() > ss.getMinX() && rect2.getMinY() < ss.getMaxX())
		tsy.add(new Double(rect2.getMinY()));
	    if (rect2.getMaxY() > ss.getMinX() && rect2.getMaxY() < ss.getMaxX())
		tsy.add(new Double(rect2.getMaxY()));
	}
	int minx, miny, maxx, maxy,px,py;
	Iterator keepy = tsy.iterator();
	maxy = ((Integer)keepy.next()).intValue();
	int tmpoverlap;
	while (keepy.hasNext()){
	    miny = maxy;
	    maxy = ((Integer)keepy.next()).intValue();
	    
	    Iterator keepx = tsx.iterator();
	    maxx = ((Integer)keepx.next()).intValue();
	    
	    while (keepx.hasNext()){
		minx = maxx;
		maxx = ((Integer)keepx.next()).intValue();
		Rectangle2i rect3 = new Rectangle2i(minx,miny,maxx,maxy);
		tmpoverlap = spacemanager.getAllOverlapSpaces(rect3).size();
		if (ov <= 0){
		    if (tmpoverlap>maxNumOverlap){ //numberOfOverlapRect){
			overlapRectangles.removeAllElements();
			overlapRectangles.add(new Rectangle2i(rect3));
			numberOfOverlapRect = tmpoverlap;
			maxNumOverlap = tmpoverlap;
		    } else if (tmpoverlap==maxNumOverlap){ // numberOfOverlapRect){
			overlapRectangles.add(new Rectangle2i(rect3));
		    }
		} else {
		    if (tmpoverlap>maxNumOverlap){ //numberOfOverlapRect){
			maxNumOverlap = tmpoverlap;
		    }
		    if (tmpoverlap==ov){
			overlapRectangles.add(new Rectangle2i(rect3));
			//			System.out.println("added Rectangle " + rect3 + " for total:" + overlapRectangles.size());
		    }
		}
	    }
	}
	//	System.out.println("end of calcOverlapRectangles(" + ov + ")\tmaxNumOverlap=" + maxNumOverlap);
	setNumberEntries(maxNumOverlap);
    }

  public void setSpaceManager(SpaceManagerDisplayWrapper spm){
    spacemanager = spm;
    setListModelSize(spacemanager.getNumberSpaces(showinsiderect));
    Rectangle2i ss = spacemanager.getScreenSpace();
    drawScaleX = ss.getWidth();
    drawScaleY = ss.getHeight();
    
    repaint();
  }
    public void loadSpaceManager(File f){
	FileInputStream istream = null;
	ObjectInputStream q =null;
	try {
	    istream = new FileInputStream(f);
	    q = new ObjectInputStream(istream);
	    spacemanager = (SpaceManagerDisplayWrapper) q.readObject();
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
	  //	    spacemanager.debugSpaceManager();
	}
	Rectangle2i ss = spacemanager.getScreenSpace();
	drawScaleX = ss.getWidth();
	drawScaleY = ss.getHeight();

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
	//	spacemanager.setDebug(debug);
    }
    public int setMode(int m){
	if (m !=MOVING || m!=MOVINGFAST){
	    movingRectangle=null;
	}
	if (m !=DELETING){
	    deleting=false;
	}
	if (m !=ADDING){
	    drawing = false;
	}
	if (m == ADDING || m==DELETING){
	  objects_moving = null;
	  number_of_objects_to_move = 0;
	}
	if (m!=SHOWENCLOSED){
	    drawing = false;
	    enclosedRectangle=null;
	    enclosedRect=null;
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
    width = (double)size.width;
    height = (double)size.height;
    resetSpaceManager();
  }
  public void testClone(){
    SpaceManagerDisplayWrapper sm = (SpaceManagerDisplayWrapper)spacemanager.clone();
    spacemanager = sm;
    repaint();
  }
  public void resetSpaceManager(){
      try {
	  getSize(screenSize);
	  spacemanager = new SpaceManagerDisplayWrapper(screenSpace=new Rectangle2i(0,0,screenSize.width,screenSize.height));
	  drawScaleX=1.;
	  drawScaleY=1.;
	  setListModelSize(spacemanager.getNumberSpaces(showinsiderect));
	  //	  spacemanager.setDebug(false);
      } catch (Exception e){
	  e.printStackTrace();
      }	
    }
    Frame owner = null;
  public SpaceManagerPanel(Frame own, DefaultListModel dlm,JList jl){
      setLayout (new BorderLayout());
      this.owner = own;
      try {
	  getSize(screenSize);
	  spacemanager = new SpaceManagerDisplayWrapper(screenSpace=new Rectangle2i(0,0,screenSize.width,screenSize.height));
      } catch (Exception e){
	  e.printStackTrace();
      } 
      drawScaleX=1.;
      drawScaleY=1.;
      //    spacemanager.setDebug(debug);
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
  public void mousePressed(MouseEvent e){
    if (!drawing && (mode==ADDING || (enclosedRectangle==null && mode==SHOWENCLOSED))){
	if (gridon){
	    Point p=e.getPoint();
	    int incY=(int)height/20, incX=(int)width/20;
	    startingPoint = new Point(incX*(p.x/incX),incY*(p.y/incY));
	    //	    System.out.println("incX=" + incX + " incY=" + incY + " p="+ p + " startingPoint=" + startingPoint);
	} else {
	    startingPoint = e.getPoint();
	}
      drawing = true;
      mousePlace = startingPoint;
    } else if (mode==DELETING){
      
    } else if (mode==MOVING || mode==MOVINGFAST){
	int mx,my;
	mx = e.getPoint().x;
	my = e.getPoint().y;
	movingRectangle = spacemanager.getClosestRectangle(new Rectangle2i(mx,my,mx,my));
	//	if (movingRectangle!=null){
	//	    System.out.println("movingRectangle set id=" + movingRectangle.id + "\t" + movingRectangle);
	//	}
	mousePlace = e.getPoint();
    }
  }
    public void addRandomRectangles(int num){
	int sx, sy;
	for (int i=0; i<num; i++){
	    FullRectangle2i rect = new FullRectangle2i();
	    rect.setFrameFromDiagonal(sx=rand.nextInt(screenSize.width),sy=rand.nextInt(screenSize.height),sx+rand.nextInt(screenSize.width/10),sy+rand.nextInt(screenSize.height/10));
	    spacemanager.addRectangle(rect,0);
	    /*	    if (debug && spacemanager.debugSpaceManager()){
		System.err.println("SpaceManagerPanel::addRandomRectangles Quit after adding " + (i+1) + " rectangles");
		break;
	    };
	    */
	}
	setListModelSize(spacemanager.getNumberSpaces(showinsiderect));
    }
  public void mouseReleased(MouseEvent e){
    if (drawing && mode==ADDING){
      addingRectangle = new Rectangle(startingPoint);
      if (gridon){
	    Point p=e.getPoint();
	    int incY=(int)height/20, incX=(int)width/20;
	    addingRectangle.add(new Point(incX*(p.x/incX),incY*(p.y/incY)));	  
      } else {
	  addingRectangle.add(e.getPoint());
      }
      FullRectangle2i rect = new FullRectangle2i(0L,addingRectangle.x,
						    addingRectangle.y,
						    addingRectangle.x + addingRectangle.width,
						    addingRectangle.y + addingRectangle.height);
      
      spacemanager.addRectangle(rect,0);
      if (debug){
	//	  spacemanager.debugSpaceManager();
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
	  
      addingRectangle = null;
      repaint();
      drawing = false;
    } else if (mode==DELETING){
	int drwx, drwy;
	drwx = e.getPoint().x;
	drwy = e.getPoint().y;
	lastDeletedRectangle=new FullRectangle2i(0l,drwx,drwy,drwx,drwy);
	lastDeletedRectangle=spacemanager.getClosestRectangle(lastDeletedRectangle);
	if (lastDeletedRectangle!=null){
	    spacemanager.deleteRectangle(lastDeletedRectangle);
	} else {
	    return;
	}
	if (debug){
	  //	    spacemanager.debugSpaceManager();
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
    } else if (mode==MOVEOBJECTS && objects_moving.size()<number_of_objects_to_move){
	int drwx, drwy;
	drwx = e.getPoint().x;
	drwy = e.getPoint().y;
	FullRectangle2i fr;
	fr=new FullRectangle2i(0l,drwx,drwy,drwx,drwy);
	fr=spacemanager.getClosestRectangle(fr);
	System.out.println("adding rect=" + fr + " to moving");
	if (fr!=null){
	  objects_moving.add(fr);
	  if (objects_moving.size()==number_of_objects_to_move){
	    showAllClosest = true;
	    setMode(MOVING);
	    repaint();
	  }
	}
    } else if (enclosedRectangle==null && drawing && mode == SHOWENCLOSED){
      enclosedRect = new Rectangle(startingPoint);
      if (gridon){
	    Point p=e.getPoint();
	    int incY=(int)height/20, incX=(int)width/20;
	    enclosedRect.add(new Point(incX*(p.x/incX),incY*(p.y/incY)));	  
      } else {
	  enclosedRect.add(e.getPoint());
      }
      enclosedRectangle = new Rectangle2i(enclosedRect.x,enclosedRect.y, enclosedRect.x + enclosedRect.width, enclosedRect.y + enclosedRect.height);
      drawing = false;
      startingPoint = null;
      addingRectangle = null;
      repaint();
    }
  }
  public void mouseEntered(MouseEvent e){
  }
  public void mouseExited(MouseEvent e){
  }
  public void mouseDragged(MouseEvent e){
    if (drawing){
      addingRectangle = new Rectangle(startingPoint);
      addingRectangle.add(e.getPoint());
      Rectangle tmpRect = new Rectangle(addingRectangle);
      tmpRect.add(mousePlace);
      mousePlace = e.getPoint();
      repaint(tmpRect.x, tmpRect.y, tmpRect.width+1, tmpRect.height+1);
    } else if (mode==MOVING && movingRectangle!=null){
      Point newMousePlace = e.getPoint();
      if (gridon){
	  int incY=(int)height/20, incX=(int)width/20;
	  int moveX=newMousePlace.x-mousePlace.x,// /width,
	      moveY=newMousePlace.y-mousePlace.y;// /height;
	  if (Math.abs(moveX)<incX && Math.abs(moveY)<incY){
	      return;
	  } else {
	      moveX=(int)incX*(moveX/incX);
	      moveY=(int)incY*(moveY/incY);
	      //	      System.out.println("moveX=" + moveX + " moveY=" + moveY + " drawScaleX=" + drawScaleX + " drawScaleY=" + drawScaleY);
	      spacemanager.moveRectangle(movingRectangle,moveX,moveY);
	  }
      } else if (isMoveDeleteAdd){
	  spacemanager.moveRectangle(movingRectangle,newMousePlace.x-mousePlace.x, newMousePlace.y-mousePlace.y);
      } else {
	  SpaceManagerDisplayWrapper sm=spacemanager;
	  try {
	      spacemanager = new SpaceManagerDisplayWrapper(screenSpace);
	      //	      System.out.println("created new SpaceManager");
	  } catch (Exception ex){
	      ex.printStackTrace();
	  }
	  for (Iterator i=sm.objectIterator(); i.hasNext();){
	      FullRectangle2i r=(FullRectangle2i)i.next();
	      //	      System.out.println("adding new rectangle r=" + r + "to new SpaceManager");
	      if (r==movingRectangle){
		  int changex, changey;
		  changex=newMousePlace.x-mousePlace.x;
		  changey=newMousePlace.y-mousePlace.y;
		  movingRectangle.setRect(r.getMinX()+changex, r.getMinY()+changey, r.getMaxX()+changex, r.getMaxY()+changey);
		  movingRectangle=spacemanager.addRectangle(movingRectangle,0);
	      } else {
		  spacemanager.addRectangle(r,0);
	      }
	  }
      }
      
      /*      if (debug && spacemanager.debugSpaceManager()){
	  movingRectangle=null;
      } else {
      */
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
	  //      }
      mousePlace = newMousePlace;
      repaint();
    } else if (mode==MOVINGFAST && movingRectangle!=null){
	Point newMousePlace = e.getPoint();
	movingRectangleOutline=new Rectangle((int)(drawScaleX*movingRectangle.getMinX()),(int)(drawScaleY*movingRectangle.getMinY()),(int)(drawScaleX*movingRectangle.getWidth()),(int)(drawScaleY*movingRectangle.getHeight()));
	repaint(movingRectangleOutline.x,movingRectangleOutline.y,movingRectangleOutline.width+1,movingRectangleOutline.height+1);
    }
  }
  public void mouseMoved(MouseEvent e){
    if (mode==SHOWENCLOSED && enclosedRectangle!=null){
      Point nMP = e.getPoint();
      int w=enclosedRectangle.getWidth(),hw;
      int h=enclosedRectangle.getHeight(),hh;
      hw = w >> 1; hh = h >> 1;
      enclosedRectangle.setRect(nMP.x - hw, nMP.y - hh, 
				nMP.x + hw, nMP.y + hh);
      enclosedRect.setRect(nMP.x-hw, nMP.y-hh, w, h);
      repaint();
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
    Color.gray,
    Color.magenta
  };

  Stroke slist[] ;
  TexturePaint tplist[];
  boolean firstTime = true;
  BufferedImage bi;
  Graphics2D big;
    FullRectangle2i lastDeletedRectangle=null;
  boolean displayReverse=false;
  public void setDisplayReverse(boolean dr){
    displayReverse=dr;
  }
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
      Iterator i = spacemanager.getSpacesOrderedByArea();
      Rectangle2i rs = new Rectangle2i();
      int a=-1;
      big.setPaintMode();
      Rectangle2i tmpRect=null;
      while (i.hasNext()){
	  tmpRect = (Rectangle2i)i.next();
	  if (displayReverse){
	    rs.setRect(tmpRect.getMinX(),
		       screenSpace.getMaxY()-tmpRect.getMaxY(),
		       tmpRect.getMaxX(),
		       screenSpace.getMaxY()-tmpRect.getMinY());
	    if (showinsiderect &&
		(rs.getMinX()==screenSpace.getMinX() || rs.getMinY()==screenSpace.getMaxY() ||
		 rs.getMaxX()==screenSpace.getMaxX() || rs.getMaxY()==screenSpace.getMinY())){
	      continue;
	    }
	  } else {
	    rs.setRect(tmpRect.getMinX(),
		       tmpRect.getMinY(),
		       tmpRect.getMaxX(),
		       tmpRect.getMaxY());
	    if (showinsiderect &&
		(rs.getMinX()==screenSpace.getMinX() || rs.getMinY()==screenSpace.getMinY() ||
		 rs.getMaxX()==screenSpace.getMaxX() || rs.getMaxY()==screenSpace.getMaxY())){
	      continue;
	    }
	  }
	  
	  if (jlist.isSelectedIndex(a+1) || (enclosedRectangle!=null && enclosedRectangle.enclosedBy(rs))){
	      big.setPaint(blacktrans);
	      big.fillRect(Math.round((float)rs.getMinX()),
			   Math.round((float)rs.getMinY()),
			   Math.round((float)rs.getWidth()),
			   Math.round((float)rs.getHeight()));
	      big.setPaint(Color.white);
	  } else {
	      big.setPaint(Color.black);
	      big.drawRect(Math.round((float)(rs.getMinX())),
			   Math.round((float)(rs.getMinY())),
			   Math.round((float)(rs.getWidth())-1),
			   Math.round((float)(rs.getHeight()-1)));
	      big.setPaint(Color.gray);
	  }
	  a++;
	  big.drawString(String.valueOf(a+1),Math.round((float)rs.getCenterX()), Math.round((float)rs.getCenterY()));
      }
      Iterator i2 = spacemanager.objectIterator();
      a=-1;
      Rectangle2i firstObject=null;
      tmpRect=null;
      while (i2.hasNext()){
	  tmpRect = (Rectangle2i)i2.next();
	  if (a==-1){
	      firstObject=tmpRect;
	  }
	  if (displayReverse){
	    rs.setRect(tmpRect.getMinX(),
		       screenSpace.getMaxY()-tmpRect.getMaxY(),
		       tmpRect.getMaxX(),
		       screenSpace.getMaxY()-tmpRect.getMinY());
	    if (showinsiderect &&
		(rs.getMinX()==screenSpace.getMinX() || rs.getMinY()==screenSpace.getMaxY() ||
		 rs.getMaxX()==screenSpace.getMaxX() || rs.getMaxY()==screenSpace.getMinY())){
	      continue;
	    }
	  } else {	  
	    rs.setRect(tmpRect.getMinX(),
		       tmpRect.getMinY(),
		       tmpRect.getMaxX(),
		       tmpRect.getMaxY());
	    if (showinsiderect &&
		(rs.getMinX()==screenSpace.getMinX() || rs.getMinY()==screenSpace.getMinY() ||
		 rs.getMaxX()==screenSpace.getMaxX() || rs.getMaxY()==screenSpace.getMaxY())){
	      continue;
	    }
	  }
	  big.setColor(clist[++a%clist.length]);
	  big.fillRect(rs.getMinX(),
		       rs.getMinY(),
		       rs.getWidth(),
		       rs.getHeight());
	  big.setColor(Color.black);
	  big.drawString(String.valueOf(a+1),Math.round((float)rs.getCenterX()), Math.round((float)rs.getCenterY()));
      }
      Rectangle2i lastObject=tmpRect;
      
      if (addingRectangle!=null){
	  big.setColor(Color.black);
	  big.draw(addingRectangle);
      }
      if (enclosedRectangle!=null){
	  big.setColor(Color.white);
	  big.draw(enclosedRect);
      }

      if (mode==MOVINGFAST && movingRectangle!=null){
	  big.setColor(Color.black);
	  big.draw(movingRectangleOutline);
      }
      if (showoverlaprect || shownextnumberrect){
	  for (Enumeration e=overlapRectangles.elements();e.hasMoreElements();){
	      tmpRect = (Rectangle2i)e.nextElement();
	      rs.setRect(tmpRect.getMinX(),
			 tmpRect.getMinY(),
			 tmpRect.getMaxX(),
			 tmpRect.getMaxY());
	      big.setPaint(blacktrans);
	      big.fillRect(rs.getMinX(),
			   rs.getMinY(),
			   rs.getWidth(),
			   rs.getHeight());
	      big.setColor(Color.black);
	  }
      } else {
	  if (showClosest && lastDeletedRectangle !=null){
	    /*	    int mx, my;
		    mx = lastDeletedRectangle.getX() + lastDeletedRectangle.getWidth()/2;
		    my = lastDeletedRectangle.getY() + lastDeletedRectangle.getHeight()/2;
		    System.out.println("mx="+ mx +" my=" + my);
		    Rectangle2i closest=spacemanager.getClosestWithExactDimension(new Rectangle2i(mx,my,mx,my),lastDeletedRectangle.getWidth(),lastDeletedRectangle.getHeight());
	    */
	    Rectangle2i closest=spacemanager.getClosestWithExactDimension(lastDeletedRectangle,lastDeletedRectangle.getWidth(),lastDeletedRectangle.getHeight());
	      
	      big.setPaint(blacktrans);
	      big.fillRect(Math.round((float)(closest.getMinX())),
			   Math.round((float)(closest.getMinY())),
			   Math.round((float)(closest.getWidth())),
			   Math.round((float)(closest.getHeight())));
	      /*	      Vector v=spacemanager.getClosestWithAtLeastDimension(lastDeletedRectangle,lastDeletedRectangle.getWidth(),lastDeletedRectangle.getHeight());
	      if (!v.isEmpty()){
		  closest=(Rectangle2i)v.firstElement();
	      } else {
		  closest=null;
		  }*/
	      big.setPaint(whitetrans);
	      Rectangle lastD=new Rectangle((int)Math.round(lastDeletedRectangle.getX()),
					    (int)Math.round(lastDeletedRectangle.getY()),
					    (int)Math.round(lastDeletedRectangle.getWidth()),
					    (int)Math.round(lastDeletedRectangle.getHeight()));
	      
	      big.setPaint(Color.black);
	      
	      big.draw(lastD);
	  } else if (showAllClosest){
	    for (int q=0; q<number_of_objects_to_move; q++){
	      FullRectangle2i fr = (FullRectangle2i)objects_moving.get(q);
	      Rectangle2i close=spacemanager.getClosestWithExactDimension(fr,fr.getWidth(),fr.getHeight());
	      big.setPaint(blacktrans);
	      big.fillRect(Math.round((float)(close.getMinX())),
			   Math.round((float)(close.getMinY())),
			   Math.round((float)(close.getWidth())),
			   Math.round((float)(close.getHeight())));
	    }
	  }
      }
      
      g2.drawImage(bi,0,0,this);
  }
  public void getClosestRects(){
      final JDialog getNumber = new JDialog (owner, true);
      getNumber.getContentPane().setLayout(new FlowLayout());
      JTextField text = new JTextField("Number of Objects want to Move:");
      text.setEditable(false);
      getNumber.getContentPane().add(text);
      final JTextField field = new JTextField(10);
      getNumber.getContentPane().add(field);
      field.addKeyListener(new KeyAdapter(){
	      public void keyTyped(KeyEvent e) {
		  if (e.getKeyChar()=='\n'){
		    int numobs=0;
		    try {
		      number_of_objects_to_move = Integer.parseInt(field.getText());
		      setMode(MOVEOBJECTS);
		      objects_moving = new Vector();
		    } catch (NumberFormatException nfe){
		      nfe.printStackTrace();
		    }
		    getNumber.dispose();
		  }
	      }
	  });
      getNumber.pack();
      getNumber.show();
  }
}
