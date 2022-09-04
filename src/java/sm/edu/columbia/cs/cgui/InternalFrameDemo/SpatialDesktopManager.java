
/**
 * SpatialDesktopManager.java
 *
 *
 * Created: Fri Dec 24 13:05:33 1999
 *
 * @author 
 * @version
 */
package edu.columbia.cs.cgui.InternalFrameDemo;

import javax.swing.*;
import java.awt.*;
import java.awt.event.*;
import java.util.Hashtable;
import java.util.Vector;
import java.util.Enumeration;
import java.util.Iterator;
import java.io.*;
import java.lang.*;
import edu.columbia.cs.cgui.InternalFrameDemo.*;
import edu.columbia.cs.cgui.spam2d.*;

public class SpatialDesktopManager extends DefaultDesktopManager   {
    JDesktopPane desktoppane;
    public SpaceManager spacemanager;
    Hashtable framehashtable;
    boolean dragging=false;
    Vector goalList;
    Vector frameList;
    int mode=0;
    public static final int MOVE_WINDOWS_AWAY=0;
    public static final int AFTER_DRAG_MOVE_TO_FREE=1;
    public static final int MOVE_WINDOWS_AWAY_ONLY_DROP=2;
    public void setMode(int m){
	mode=m;
    }
    public class ObjectRectangle2d extends FullRectangle2d implements Serializable {// implements Cloneable,Serializable {
	transient public JComponent jcomp=null;
	ObjectRectangle2d(Rectangle2d r){
	    super(0,r);
	}
	ObjectRectangle2d(JComponent jc, double minx, double miny, double maxx, double maxy){
	    super(0,minx,miny,maxx,maxy);
	    jcomp=jc;
	}
	
	public Object writeReplace() throws ObjectStreamException{
	    return (new FullRectangle2d((FullRectangle2d)this));
	}
	public boolean equals(Object obj){
	    return(super.equals(obj));
	}
    }
    public SpatialDesktopManager(JDesktopPane dp) {
	desktoppane=dp;
	framehashtable=new Hashtable();
	goalList = new Vector();
	frameList = new Vector();
	Rectangle rect = dp.getBounds(null);//getVisibleRect();//dp.getSize(null);
	//	System.out.println("rect=" + rect);
	Rectangle2d screenRectangle = new Rectangle2d(rect.getX(),rect.getY(),
						      rect.getX()+rect.getWidth(),rect.getY()+rect.getHeight());
	//	System.out.println("screenRectangle=" + screenRectangle);
	try {
	    spacemanager = new SpaceManager(screenRectangle);
	    spacemanager.setDebug(false);
	} catch (Exception e){
	    e.printStackTrace();
	}
	
	JInternalFrame[] jframes = dp.getAllFrames();
	FullRectangle2d tmpRect;
	//	System.out.println("SpatialDesktopManager created jframes.length=" + jframes.length);
	for (int i=0; i<jframes.length;i++){
	    rect = jframes[i].getBounds();
	    //	    System.out.println("adding rect=" + rect);
	    tmpRect = new ObjectRectangle2d(jframes[i],rect.getX(),rect.getY(),rect.getX() + rect.getWidth(),rect.getY()+rect.getHeight());
	    framehashtable.put(jframes[i],tmpRect);
	    //	    System.out.println("adding tmpRect=" + tmpRect);
	    spacemanager.addRectangle(tmpRect,-1);
	}
	//	System.out.println("ContainerEvent.COMPONENT_ADDED=" + ContainerEvent.COMPONENT_ADDED);
	//	System.out.println("ContainerEvent.COMPONENT_REMOVED=" + ContainerEvent.COMPONENT_REMOVED);
	//	System.out.println("ContainerEvent.CONTAINER_FIRST=" + ContainerEvent.CONTAINER_FIRST);
	//	System.out.println("ContainerEvent.CONTAINER_LAST=" + ContainerEvent.CONTAINER_LAST);

	desktoppane.addContainerListener(new ContainerAdapter(){
		public void componentRemoved(ContainerEvent e){
		    if (e.getID()==ContainerEvent.COMPONENT_REMOVED){
			if (((SpatialDesktopManager)((JDesktopPane)e.getSource()).getDesktopManager()).dragging){
			    return;
			}
			JComponent jif=(JComponent)e.getChild();
			FullRectangle2d rect2=(FullRectangle2d)framehashtable.remove(jif);
			if (rect2==null){
			  //			    System.out.println("Warning: jif=" + jif + "is not in hashtable");
			}
			//			System.out.println("ComponentRemoved rect=" + rect2);
			spacemanager.deleteRectangle(rect2);
		    }
		}
		public void componentAdded(ContainerEvent e){
		  //		    System.out.println("componentAdded e.getID()=" + e.getID());// + " e.getChild()=" + e.getChild());
		    /*		    if (!(e.getSource() instanceof JInternalFrame)){
				    return;
				    }*/
		    if (e.getID()==ContainerEvent.COMPONENT_ADDED){
			JComponent jif=(JComponent)e.getChild();
			//			Thread.currentThread().dumpStack();
			if(framehashtable.containsKey(jif)){
			    // weird, start over..
			  //			    System.out.println("weird");
			    Rectangle rect = desktoppane.getBounds(null);//getVisibleRect();//dp.getSize(null);
			    Rectangle2d screenRectangle = new Rectangle2d(rect.getX(),rect.getY(),
									  rect.getX()+rect.getWidth(),rect.getY()+rect.getHeight());
			    //	System.out.println("screenRectangle=" + screenRectangle);
			    try {
				spacemanager = new SpaceManager(screenRectangle);
				spacemanager.setDebug(false);
			    } catch (Exception ex){
				ex.printStackTrace();
			    }
			    JInternalFrame[] jframes = desktoppane.getAllFrames();
			    FullRectangle2d tmpRect;
			    //			    System.out.println("SpatialDesktopManager created jframes.length=" + jframes.length);
			    framehashtable=new Hashtable();
			    for (int i=0; i<jframes.length;i++){
				rect = jframes[i].getBounds();
				//	    System.out.println("adding rect=" + rect);
				tmpRect = new ObjectRectangle2d(jframes[i],rect.getX(),rect.getY(),rect.getX() + rect.getWidth(),rect.getY()+rect.getHeight());
				framehashtable.put(jframes[i],tmpRect);
				//	    System.out.println("adding tmpRect=" + tmpRect);
				spacemanager.addRectangle(tmpRect,-1);
			    }
			    
			    //			    System.out.println("already has if=" + jif); 
			} else {
			    setComponentPlacement(jif);
			    /*			    Rectangle r=jif.getBounds(null);
						    Rectangle2d rect=new ObjectRectangle2d(jif,r.getX(),r.getY(),r.getX()+r.getWidth(),r.getY()+r.getHeight());
						    framehashtable.put(jif,rect);
						    //			    System.out.println("rect=" + rect);
						    spacemanager.addRectangle(rect);
			    */
			}
		    } else if (e.getID()==ContainerEvent.COMPONENT_REMOVED){
		    }
		}
	    });
    }
    void setComponentPlacement(JComponent jif){
	Dimension d=jif.getPreferredSize();//jif.getPreferredSize();
	Rectangle2d tmpRect=null,rect=null;
	for (Iterator i=spacemanager.getSpacesOrderedByArea();i.hasNext();){
	    tmpRect=(Rectangle2d) i.next();
	    if (tmpRect.getWidth()>=d.getWidth() && tmpRect.getHeight()>=d.getHeight()){
		rect=tmpRect;
		//		break;
	    } else if (tmpRect.area()<d.getWidth()*d.getHeight()){
		break;
	    }
	}
	if (rect==null){
	    double areacovered=0.,w=0,h=0;
	    for (Iterator i=spacemanager.getSpacesOrderedByArea();i.hasNext();){
		tmpRect=(Rectangle2d) i.next();
		if (tmpRect.getWidth()<d.getWidth()){
		    w=tmpRect.getWidth();
		} else {
		    w=d.getWidth();
		}
		if (tmpRect.getHeight()<d.getHeight()){
		    h=tmpRect.getHeight();
		} else {
		    h=d.getHeight();
		}
		//		System.out.println("tmpRect=" + tmpRect + "\tw=" + w + " h=" + h + " areacovered=" + areacovered);
		if (h*w>areacovered){
		    rect=tmpRect;
		    areacovered=h*w;
		} else if (tmpRect.area()<areacovered){
		    break;
		}
	    }
	}
	int mx,my;
	if (rect==null){
	    mx=0; my=0;
	} else {
	    mx=(int)Math.round(rect.getMinX());
	    my=(int)Math.round(rect.getMinY());
	}
	//	System.out.println("setComponentPlacement - (" + mx + "," + my + ") with preferredSize=" + d);
	setBoundsForFrame(jif,mx,my,d.width,d.height);
    }
    private void addToFrameList(ObjectRectangle2d or){
	if (or==null){
	    return ;
	}
	if (!frameList.contains(or)){
	    frameList.add(or);
	    Rectangle2d newDest = spacemanager.getClosestWithExactDimension(or,or.getWidth(),or.getHeight());
	    goalList.add(or);
	} else {
	    int ind=frameList.indexOf(or);
	    Rectangle2d newDest = spacemanager.getClosestWithExactDimension(or,or.getWidth(),or.getHeight());
	    goalList.setElementAt(newDest,ind);
	}
    }
    public void dragFrame(JComponent f,int newX,int newY){
	//	System.out.println("dragFrame");
	//	    System.out.println("moveRectangle rect=" + rect + "\tnewX=" + newX + "\tnewY=" + newY);
	FullRectangle2d rect=(FullRectangle2d)framehashtable.remove(f);
	spacemanager.deleteRectangle(rect);
	//	super.dragFrame(f,newX,newY);
	//	framehashtable.put(f,spacemanager.moveRectangle(rect,(double)(newX-rect.getMinX()),(double)(newY-rect.getMinY())));
	setBoundsForFrame(f,newX,newY,f.getWidth(),f.getHeight());
	if (mode==MOVE_WINDOWS_AWAY){
	    rect = (FullRectangle2d)framehashtable.get(f);
	    
	    Vector v=spacemanager.getAllOverlapObjects(rect);
	    Rectangle2d newPos;
	    ObjectRectangle2d tmpRect;
	    //	System.out.println("v.size=" + v.size());
	    if (v.size()==1 && ((Rectangle2d)v.firstElement())==rect){
		return;
	    }
	    v.remove(rect);
	    for (Enumeration e = v.elements() ; e.hasMoreElements() ;) {
		tmpRect=(ObjectRectangle2d) e.nextElement();
		if (!frameList.contains(tmpRect.jcomp)){
		    frameList.add(tmpRect.jcomp);
		    //		addToFrameList(tmpRect.jcomp);
		} else {
		    continue;
		}
	    }
	    boolean dontdelete=false;
	    JComponent tmpcomp;
	    for (Enumeration e = frameList.elements() ; e.hasMoreElements() ;) {
		tmpcomp=(JComponent) e.nextElement();
		tmpRect=(ObjectRectangle2d)framehashtable.remove(tmpcomp);
		//	    System.out.println("jcomp=" + jc);
		spacemanager.deleteRectangle(tmpRect);
		newPos = spacemanager.getClosestWithExactDimension(tmpRect,tmpRect.getWidth(),tmpRect.getHeight());
		//	    System.out.println("newPos=" + newPos);
		//	    System.out.println("newPos=" + newPos + "(jc==null)=" + (jc==null));
		if (newPos==null){
		    spacemanager.addRectangle(tmpRect,-1);
		    framehashtable.put(tmpcomp,tmpRect);
		    continue;
		}
		//	    System.out.println("dragFrame newPos=" + newPos + "\ttmpRect=" + tmpRect + "\tRectangle2d.getCenterDistanceBetween(newPos,tmpRect)=" + Rectangle2d.getCenterDistanceBetween(newPos,tmpRect));
		dontdelete=false;
		if (Rectangle2d.getCenterDistanceBetween(newPos,tmpRect) > 10.){
		    double diffx=newPos.getMinX()-tmpRect.getMinX(),
			diffy=newPos.getMinY()-tmpRect.getMinY(),scale;
		    //		System.out.println("diffx=" + diffx + "\tdiffy=" + diffy);
		    if (Math.abs(diffx)>Math.abs(diffy)){
			scale=Math.abs(diffx)*.1;
		    } else {
			scale=Math.abs(diffy)*.1;
		    }
		    double width=tmpRect.getWidth(),height=tmpRect.getHeight();
		    newPos.setMinX(tmpRect.getMinX()+diffx/scale);
		    newPos.setMinY(tmpRect.getMinY()+diffy/scale);
		    newPos.setMaxX(newPos.getMinX()+width);
		    newPos.setMaxY(newPos.getMinY()+height);
		    //		System.out.println("scale=" +scale + "\tnewPos=" + newPos);
		    dontdelete=true;
		}
		if (newPos!=null && tmpcomp!=null){
		    //		framehashtable.remove(jc);
		    setBoundsForFrame(tmpcomp,(int)newPos.getMinX(),(int)newPos.getMinY(),
				      (int)tmpRect.getWidth(),(int)tmpRect.getHeight());
		    //				  (int) newPos.getWidth(), (int) newPos.getHeight());
		    if (!dontdelete){
			frameList.remove(tmpcomp);
		    }
		} else {
		    //		System.out.println("Warning: getClosestWithExactDimension returned nothing");
		    //		spacemanager.addRectangle(tmpRect);
		}
	    }
	}
    }
    public void openFrame(JInternalFrame f){
	//	System.out.println("SpatialDesktopManager.openFrame f=" + f);
    }
    public void beginDraggingFrame(JComponent f){
	dragging=true;
	if (f instanceof JInternalFrame){
	    ((JInternalFrame)f).moveToFront();
	}
	//	System.out.println("beginDraggingFrame");

	//	if (f instanceof JInternalFrame)
	    //	    ((JInternalFrame)f).toFront();
    }
    Timer timer=new Timer(10,new ActionListener(){
	    public void actionPerformed(ActionEvent e){
		timer.stop();
		calcNextFrame();
	    }
	});
    public void endDraggingFrame(JComponent f){
	dragging=false;
	if (mode==MOVE_WINDOWS_AWAY_ONLY_DROP){
	    Point p=f.getLocation();
	    mode=MOVE_WINDOWS_AWAY;
	    dragFrame(f,p.x,p.y);
	    mode=MOVE_WINDOWS_AWAY_ONLY_DROP;
	}
	if (mode==AFTER_DRAG_MOVE_TO_FREE){
	    FullRectangle2d rect=(FullRectangle2d) framehashtable.get(f);
	    Rectangle2d newPos;
	    ObjectRectangle2d tmpRect;
	    if (!spacemanager.getAllOverlapObjects(rect).isEmpty()){
		tmpRect = (ObjectRectangle2d)framehashtable.remove(f);
		spacemanager.deleteRectangle(tmpRect);
		newPos = spacemanager.getClosestWithExactDimension(tmpRect,tmpRect.getWidth(),tmpRect.getHeight());
		double dist;
		if (newPos==null || (dist=Rectangle2d.getCenterDistanceBetween(newPos,tmpRect)) > 200.){
		    spacemanager.addRectangle(tmpRect,-1);
		    framehashtable.put(f,tmpRect);
		} else {
		    frameList.add(tmpRect.jcomp);
		    //		    setBoundsForFrame(f,(int)newPos.getMinX(),(int)newPos.getMinY(),
		    setBoundsForFrame(f,(int)tmpRect.getMinX(),(int)tmpRect.getMinY(),
				      (int)tmpRect.getWidth(),(int)tmpRect.getHeight());
		}
	    }
	}
	if (!frameList.isEmpty()){
	    timer.start();
	}
	//	System.out.println("endDragginFrame frameList.size()=" + frameList.size());
    }
    public void calcNextFrame(){
	JComponent tmpcomp;
	Rectangle2d newPos;
	ObjectRectangle2d tmpRect;
	boolean dontdelete=false,movedSomething=false;
	//	System.out.println("calcNextFrame()");
	for (Enumeration e = frameList.elements() ; e.hasMoreElements() ;) {
	    tmpcomp=(JComponent) e.nextElement();
	    tmpRect=(ObjectRectangle2d)framehashtable.remove(tmpcomp);
	    //	    System.out.println("jcomp=" + jc);
	    spacemanager.deleteRectangle(tmpRect);
	    newPos = spacemanager.getClosestWithExactDimension(tmpRect,tmpRect.getWidth(),tmpRect.getHeight());
	    //	    System.out.println("newPos=" + newPos);
	    //	    System.out.println("newPos=" + newPos + "(jc==null)=" + (jc==null));
	    if (newPos==null){
		spacemanager.addRectangle(tmpRect,-1);
		framehashtable.put(tmpcomp,tmpRect);
		continue;
	    }
	    //	    System.out.println("dragFrame newPos=" + newPos + "\ttmpRect=" + tmpRect + "\tRectangle2d.getCenterDistanceBetween(newPos,tmpRect)=" + Rectangle2d.getCenterDistanceBetween(newPos,tmpRect));
	    dontdelete=false;
	    if (Rectangle2d.getCenterDistanceBetween(newPos,tmpRect) > 10.){
		double diffx=newPos.getMinX()-tmpRect.getMinX(),
		    diffy=newPos.getMinY()-tmpRect.getMinY(),scale;
		//		System.out.println("diffx=" + diffx + "\tdiffy=" + diffy);
		if (Math.abs(diffx)>Math.abs(diffy)){
		    scale=Math.abs(diffx)*.1;
		} else {
		    scale=Math.abs(diffy)*.1;
		}
		double width=tmpRect.getWidth(),height=tmpRect.getHeight();
		newPos.setMinX(tmpRect.getMinX()+diffx/scale);
		newPos.setMinY(tmpRect.getMinY()+diffy/scale);
		newPos.setMaxX(newPos.getMinX()+width);
		newPos.setMaxY(newPos.getMinY()+height);
		//		System.out.println("scale=" +scale + "\tnewPos=" + newPos);
		movedSomething=true;
		dontdelete=true;
	    }
	    if (newPos!=null && tmpcomp!=null){
		//		framehashtable.remove(jc);
		setBoundsForFrame(tmpcomp,(int)newPos.getMinX(),(int)newPos.getMinY(),
				  (int)tmpRect.getWidth(),(int)tmpRect.getHeight());
		//				  (int) newPos.getWidth(), (int) newPos.getHeight());
		movedSomething=true;
		if (!dontdelete){
		    frameList.remove(tmpcomp);
		}
	    } else {
		//		System.out.println("Warning: getClosestWithExactDimension returned nothing");
		//		spacemanager.addRectangle(tmpRect);
	    }
	}
	if (frameList.isEmpty() || !movedSomething){
	    timer.stop();
	} else {
	    timer.start();
	}
	//	System.out.println("endDraggingFrame movedSomething=" + movedSomething + "\t frameList.size=" + frameList.size());
    }
    public void activateFrame(JInternalFrame f){
	//	System.out.println("SpatialDesktopManager.activateFrame");
    }
    public void maximizeFrame(JInternalFrame f){
	Rectangle rect = f.getBounds(null);
	//	System.out.println("SpatialDesktopManager.maximizeFrame rect=" + rect + "\t f.isMaximum=" + f.isMaximum());
	
	setPreviousBounds(f,rect);
	FullRectangle2d tmpRect=(FullRectangle2d)framehashtable.remove(f);
	//	System.out.println("SpatialDesktopManager.maximizeFrame tmpRect=" + tmpRect);
	spacemanager.deleteRectangle(tmpRect);
	//	spacemanager.printAllRectangles();
	Rectangle2d largerect=spacemanager.getLargestSpace();
	//	System.out.println("getLargestSpace returns largerect=" + largerect);
	if (largerect.area()>=.5*spacemanager.screenSpace.area()){
	    setBoundsForFrame(f,(int)largerect.getMinX(),(int)largerect.getMinY(),
			      (int)largerect.getWidth(), (int)largerect.getHeight());
	    /*	    spacemanager.addRectangle(largerect);
		    System.out.println("in maximizeFrame area>.5 largerect=" + largerect);
		    framehashtable.put(f,largerect);
	    */
	} else {
	    super.maximizeFrame(f);
	    /*	    
	    Rectangle newRect=f.getBounds();
	    largerect.setRect(newRect.getX(),newRect.getY(),
			      newRect.getX()+newRect.getWidth(),
			      newRect.getY()+newRect.getHeight());
			      System.out.println("in maximizeFrame largerect=" + largerect);
			      spacemanager.addRectangle(largerect);
			      framehashtable.put(f,largerect);
	    */
	}
	//	System.out.println("end SpatialDesktopManager.maximizeFrame rect=" + rect + "\t f.isMaximum=" + f.isMaximum());
    }
    public void minimizeFrame(JInternalFrame f){
	//	System.out.println("SpatialDesktopManager.minimizeFrame f.isMaximum=" + f.isMaximum());
	FullRectangle2d tmpRect=(FullRectangle2d)framehashtable.remove(f);
	//	System.out.println("SpatialDesktopManager.maximizeFrame tmpRect=" + tmpRect);
	spacemanager.deleteRectangle(tmpRect);
  	Rectangle rect=getPreviousBounds(f);
  	setBoundsForFrame(f,rect.x,rect.y,rect.width,rect.height);
    }
    public void resizeFrame(JComponent f,
			    int newX,
			    int newY,
			    int newWidth,
			    int newHeight){
	FullRectangle2d tmpRect=(FullRectangle2d)framehashtable.remove(f);
	//	System.out.println("SpatialDesktopManager.maximizeFrame tmpRect=" + tmpRect);
	spacemanager.deleteRectangle(tmpRect);
  	setBoundsForFrame(f,newX,newY, newWidth,newHeight);
	
    }
    public void setBoundsForFrame(JComponent f,
				  int newX,
				  int newY,
				  int newWidth,
				  int newHeight){
	//	System.out.println("setBoundsForFrame");
	Rectangle rect=f.getBounds(null);
	//	Rectangle2d tmpRect=(Rectangle2d)framehashtable.remove(f);
	//	spacemanager.deleteRectangle(tmpRect);
	super.setBoundsForFrame(f,newX,newY,newWidth,newHeight);
	ObjectRectangle2d tmpRect=new ObjectRectangle2d(f,(double)newX,(double)newY,(double)(newX+newWidth),(double)(newY+newHeight));
	//	System.out.println("setBoundsForFrame new added Rect=" + tmpRect);

	//	spacemanager.printAllRectangles();
	spacemanager.addRectangle(tmpRect,-1);
	framehashtable.put(f,tmpRect);
    } 
} // SpatialDesktopManager
