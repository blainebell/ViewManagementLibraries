package edu.columbia.cs.cgui.spam2d;

import java.io.*;
import java.lang.reflect.*;
import java.lang.*;
import java.util.*;
import javax.swing.Timer;
import java.awt.event.*;

class PerfTest {
    static int frames=0,lastFrames=0;;

    public static void main(String[] args){
	SpaceManager sm=null;
	FirstSecondaryStructure fss;
	SecondSecondaryStructure sss ;

	try {
	    fss = new FirstSecondaryStructure();
	    sss = new SecondSecondaryStructure();
	} catch (Exception e){
	    e.printStackTrace();
	}
	
	if (args.length != 6){
	    System.err.println("Usage:  PerfTest <file.smf> <sizeofmovingrectangle> <t/f=isMoveDeleteAdd>  <howmanymovingrectangles 1/2/3/4> <t/f=undo/redo> <t=fps or f=spf>");
	    System.exit(-1);
	}

	System.out.println("args[0]='" + args[0] + "'\targs[1]='"+ args[1] + "'\targs[2]='" + args[2] + "'");
	FileInputStream istream = null;
	ObjectInputStream q =null;

	try {
	    istream = new FileInputStream(new File (args[0]));
	    q = new ObjectInputStream(istream);
	    sm = (SpaceManager) q.readObject();
	    q.close();
	} catch (FileNotFoundException fnf){
	    System.err.println("File "+ args[0] + " not found");
	    return;
	} catch (Exception e){
	    System.err.println("file f=" + args[0]);
	    e.printStackTrace();
	    return;
	}
	sm.setDebug(false);

	boolean redo=args[4].equals("t");
	boolean fps=args[5].equals("t");
	int numRectangles=Integer.parseInt(args[3]);
	if (numRectangles!=1 && numRectangles!=2 && numRectangles!=3 && numRectangles!=4 && 
	    numRectangles!=5 && numRectangles!=6 && numRectangles!=7 && numRectangles!=8){
	    numRectangles=1;
	}
	System.out.println("numRectangles=" + numRectangles);
	Rectangle2d screenSpace = new Rectangle2d(sm.screenSpace);
	double sizeOfRect = Double.valueOf(args[1]).doubleValue();
	System.out.println("sizeOfRect=" + sizeOfRect);
	double startminY = (screenSpace.getHeight()/2.)-(sizeOfRect/2.);
	double startminX = (screenSpace.getWidth()/2.)-(sizeOfRect/2.);
	System.out.println("screenSpace=" + screenSpace);
	System.out.println("startminY=" + startminY);
	FullRectangle2d movingRect[] = new FullRectangle2d[8];
	
	movingRect[0] = new FullRectangle2d(999999l,0.,startminY, sizeOfRect, startminY + sizeOfRect);
	if (numRectangles>=2){
	    movingRect[1] = new FullRectangle2d(9999999l,screenSpace.getMaxX()-sizeOfRect,startminY, screenSpace.getMaxX(), startminY + sizeOfRect);
	}
	if (numRectangles>=3){
	    movingRect[2] = new FullRectangle2d(99999999l,startminX, 0., startminX+sizeOfRect, sizeOfRect);
	}
	if (numRectangles>=4){
	    movingRect[3] = new FullRectangle2d(999999999l,startminX, screenSpace.getMaxY()-sizeOfRect, startminX+sizeOfRect, screenSpace.getMaxY());
	}
	if (numRectangles>=5){
	    movingRect[4] = new FullRectangle2d(9999998l,
						(screenSpace.getMaxX()/4.)-(sizeOfRect/2.),
						(screenSpace.getMaxY()/4.)-(sizeOfRect/2.),
						(screenSpace.getMaxX()/4.)+(sizeOfRect/2.),
						(screenSpace.getMaxY()/4.)+(sizeOfRect/2.));
	}
	if (numRectangles>=6){
	    movingRect[5] = new FullRectangle2d(99999998l,
						(3*screenSpace.getMaxX()/4.)-(sizeOfRect/2.),
						(3*screenSpace.getMaxY()/4.)-(sizeOfRect/2.),
						(3*screenSpace.getMaxX()/4.)+(sizeOfRect/2.),
						(3*screenSpace.getMaxY()/4.)+(sizeOfRect/2.));
	}
	if (numRectangles>=7){
	    movingRect[6] = new FullRectangle2d(999999998l,
						(screenSpace.getMaxX()/4.)-(sizeOfRect/2.),
						(3*screenSpace.getMaxY()/4.)-(sizeOfRect/2.),
						(screenSpace.getMaxX()/4.)+(sizeOfRect/2.),
						(3*screenSpace.getMaxY()/4.)+(sizeOfRect/2.));
	}
	if (numRectangles>=8){
	    movingRect[7] = new FullRectangle2d(999999997l,
						(3*screenSpace.getMaxX()/4.)-(sizeOfRect/2.),
						(screenSpace.getMaxY()/4.)-(sizeOfRect/2.),
						(3*screenSpace.getMaxX()/4.)+(sizeOfRect/2.),
						(screenSpace.getMaxY()/4.)+(sizeOfRect/2.));
	}
	
	System.out.println("screenSpace=" + screenSpace);
	for (int zxc=0; zxc<numRectangles;zxc++){
	    System.out.println("movingRect[" + zxc + "]=" + movingRect[zxc]);
	}
	double dx=0.,dy=1.,incr=Math.PI/100.,scalex = .5*(1.-sizeOfRect),scaley=1./3.,dx2=.5,dy2=.5,dx3,dy3,dx4,dy4;
	Vector redovector=new Vector();
	if (redo){
	    for (int qer=0; qer<numRectangles; qer++){
		redovector.add(movingRect[qer]);
	    }
	    sm.addRectanglesCalcRedoList(redovector);
	} else {
	    for (int qer=0; qer<numRectangles; qer++){
		sm.addRectangle(movingRect[qer],-1);
	    }
	}
	if (fps){
	    Timer timer = new Timer (1000, new ActionListener(){
		    int numSec=0;
		    public void actionPerformed(ActionEvent e){
			System.out.println("frames per Second=" + frames);
			frames=0;
			if (++numSec==20){
			    System.exit(-1);
			}
		    }
		});
	    timer.start();
	}
	boolean a=true;
	boolean isMoveDeleteAdd=args[2].equals("t");
	System.out.println("isMoveDeleteAdd=" + isMoveDeleteAdd);
	long startTime=System.currentTimeMillis(),lastTime,currentTime;
	lastTime=startTime;
	boolean tmpb=true;
	for (double z=incr; z<= Math.PI/4./*Math.PI*50*/; z+= incr){
	    tmpb=false;
	    dx=scalex*incr*Math.sin(z)/screenSpace.getWidth();
	    dy=scaley*incr*Math.cos(z)/screenSpace.getHeight();	
	    dx2=scaley*incr*Math.sin(z-(Math.PI/2.))/screenSpace.getHeight();//Width();
	    dy2=scalex*incr*Math.cos(z-(Math.PI/2.))/screenSpace.getWidth();//Height();
	    dx3=scaley*incr*Math.sin(z-(Math.PI/4.))/screenSpace.getHeight();//Width();
	    dy3=scalex*incr*Math.cos(z-(Math.PI/4.))/screenSpace.getWidth();//Height();
	    dx4=scaley*incr*Math.sin(z-(3*Math.PI/4.))/screenSpace.getHeight();//Width();
	    dy4=scalex*incr*Math.cos(z-(3*Math.PI/4.))/screenSpace.getWidth();//Height();
	    System.out.println("movingRect=" + movingRect + "\tdx=" +dx + "\tdy=" + dy + "\tdx2=" +dx2 + "\tdy2=" + dy2 + "\tdx3=" +dx3 + "\tdy3=" + dy3 + "\tdx4=" +dx4 + "\tdy4=" + dy4 );

	    if (redo){
		sm.undoAllRedoList();
		movingRect[0].move(dx,dy);
		if (numRectangles>=2){ movingRect[1].move(-dx,-dy); }
		if (numRectangles>=3){ movingRect[2].move(dx2,dy2); }
		if (numRectangles>=4){ movingRect[3].move(-dx2,-dy2); }
		if (numRectangles>=5){ movingRect[4].move(dx3,dy3); }
		if (numRectangles>=6){ movingRect[5].move(-dx3,-dy3); }
		if (numRectangles>=7){ movingRect[6].move(dx4,dy4); }
		if (numRectangles>=8){ movingRect[7].move(-dx4,-dy4); }
		sm.addRectanglesCalcRedoList(redovector);
	    } else if (isMoveDeleteAdd){
		movingRect[0]=sm.moveRectangle(movingRect[0],dx,dy);
		if (numRectangles>=2){ movingRect[1]=sm.moveRectangle(movingRect[1],-dx,-dy); }
		if (numRectangles>=3){ movingRect[2]=sm.moveRectangle(movingRect[2],dx2,dy2); }
		if (numRectangles>=4){ movingRect[3]=sm.moveRectangle(movingRect[3],-dx2,-dy2); }
		if (numRectangles>=5){ movingRect[4]=sm.moveRectangle(movingRect[4],dx3,dy3); }
		if (numRectangles>=6){ movingRect[5]=sm.moveRectangle(movingRect[5],-dx3,-dy3); }
		if (numRectangles>=7){ movingRect[6]=sm.moveRectangle(movingRect[6], dx4, dy4); }
		if (numRectangles>=8){ movingRect[7]=sm.moveRectangle(movingRect[7],-dx4,-dy4); }
	    } else {
		SpaceManager spacemanager=sm;
		try {
		    sm = new SpaceManager(screenSpace);
		    //	      System.out.println("created new SpaceManager");
		} catch (Exception ex){
		    ex.printStackTrace();
		}
		boolean ch=true;
		for (Iterator i=spacemanager.objectIterator(); i.hasNext();){
		    FullRectangle2d r=(FullRectangle2d)i.next();
		    //	      System.out.println("adding new rectangle r=" + r + "to new SpaceManager");
		    ch=false;
		    for (int ert=0; ert<numRectangles;ert++){
			if (r.id==movingRect[ert].id){
			    if (ert==0){ 
				movingRect[0].move(dx,dy); 
				movingRect[0] = sm.addRectangle(movingRect[0],-1);
				ch=true; break;
			    } 
			    if (ert==1){
				movingRect[1].move(-dx,-dy);
				movingRect[1] = sm.addRectangle(movingRect[1],-1);
				ch=true; break;
			    } 
			    if (ert==2){ 
				movingRect[2].move(dx2,dy2); 
				movingRect[2] = sm.addRectangle(movingRect[2],-1);
				ch=true; break;
			    } 
			    if (ert==3){ 
				movingRect[3].move(-dx2,-dy2);
				movingRect[3] = sm.addRectangle(movingRect[3],-1);
				ch=true; break;
			    } 
			    if (ert==4){ 
				movingRect[4].move(dx3,dy3);
				movingRect[4] = sm.addRectangle(movingRect[4],-1);
				ch=true; break;
			    } 
			    if (ert==5){ 
				movingRect[5].move(-dx3,-dy3);
				movingRect[5] = sm.addRectangle(movingRect[5],-1);
				ch=true; break;
			    } 
			    if (ert==6){ 
				movingRect[6].move(dx4,dy4);
				movingRect[6] = sm.addRectangle(movingRect[6],-1);
				ch=true; break;
			    } 
			    if (ert==7){ 
				movingRect[7].move(-dx4,-dy4);
				movingRect[7] = sm.addRectangle(movingRect[7],-1);
				ch=true; break;
			    } 
			}
		    }
		    if (!ch){
			sm.addRectangle(r,-1);
		    }
		}
	    }

	    frames++;
	    if (!fps && frames % 100==0){
		currentTime=System.currentTimeMillis();
		System.out.println("100 frames in " + (((double)(currentTime-lastTime))/1000.) + "\taverage after " + frames +" frames fps=" + (double) ((double)frames*1000)/((double)currentTime-startTime));
		lastTime=currentTime;
	    }
	}

	FileOutputStream ostream=null;
	ObjectOutputStream p=null ;
	try {
	    ostream = new FileOutputStream(new File("saved.smf"));
	    p = new ObjectOutputStream(ostream);
	    p.writeObject(sm);
	    p.flush();
	    ostream.close();
	} catch (Exception e){
	    e.printStackTrace();
	}
	
    }
}
