package edu.columbia.cs.cgui.spam2i;

import javax.swing.*;
import java.awt.event.*;
import java.awt.*;
import javax.swing.event.*;
import java.io.*;
import javax.swing.text.*;

public class SpaceManagerTest extends JApplet {
    static public SpaceManagerPanel spacemanagerpanel;
    JMenuBar menubar;
    JMenu filemenu,rectanglemenu,viewmenu,debugmenu;
  static JCheckBoxMenuItem addrectitem, deleterectitem,moverectitem,movefastrectitem,debugitem,shownumberrectitem,shownumberoverlapitem,shownextnumberrectitem,showjustinsiderectitem,showclosestitem,showgriditem,drawenclosedspacesitem;
  JMenuItem printrectanglesitem,addrectanglesitem,printObjectTreeitem, printSpaceTreeitem,testcloneitem,loaditem,saveitem,newitem,getclosestrectsitem;
    DefaultListModel defaultlistmodel;
    JList jlist;
    javax.swing.filechooser.FileFilter filefilter = (javax.swing.filechooser.FileFilter) new ExampleFileFilter( new String("smf"), "Space Managers");
    JFileChooser jfilechooser = new JFileChooser(new File (System.getProperty("user.dir")));

  public void init(){
    createPanel(true);
    setJMenuBar(menubar);
    setContentPane(spacemanagerpanel);
  }
    static int sizx=500, sizy=500;
    static public void setWindowSize(int x, int y){
	sizx=x; sizy=y;
    }
  public void createPanel(boolean applet){
    spacemanagerpanel = new SpaceManagerPanel(frame,defaultlistmodel,jlist);

    spacemanagerpanel.setPreferredSize(new Dimension(sizx,sizy));
    spacemanagerpanel.resetSpaceManager();
    menubar = new JMenuBar();
    menubar.add(filemenu = new JMenu("File"));    
    filemenu.setMnemonic(KeyEvent.VK_F);
    menubar.add(rectanglemenu = new JMenu("Rectangles"));
    rectanglemenu.setMnemonic(KeyEvent.VK_R);
    //    menubar.add(viewmenu = new JMenu("View"));
    viewmenu = new JMenu("View");
    viewmenu.setMnemonic(KeyEvent.VK_V);

    viewmenu.add(getclosestrectsitem = new JMenuItem("Set # Closest"));
    getclosestrectsitem.setMnemonic('G');
    getclosestrectsitem.addActionListener(new ActionListener(){
	    public void actionPerformed(ActionEvent e){
		spacemanagerpanel.getClosestRects();
	    }
	});
    //        menubar.add(debugmenu = new JMenu("Debug"));
    debugmenu = new JMenu("Debug");
    viewmenu.setMnemonic(KeyEvent.VK_D);
    rectanglemenu.add(addrectitem = new JCheckBoxMenuItem("Add",true));
    rectanglemenu.add(deleterectitem = new JCheckBoxMenuItem("Delete",false));
    rectanglemenu.add(moverectitem = new JCheckBoxMenuItem("Move",false));
    rectanglemenu.add(movefastrectitem = new JCheckBoxMenuItem("Move Fast",false));
    rectanglemenu.add(drawenclosedspacesitem = new JCheckBoxMenuItem("Highlight Enclosed", false));
    debugmenu.add(printrectanglesitem = new JMenuItem("Print Rectangles"));
    //    debugmenu.add(addrectanglesitem = new JMenuItem("Add Rectangles"));
    debugmenu.add(debugitem = new JCheckBoxMenuItem("Debug Data Structures",false));
    spacemanagerpanel.Debug(false);
    debugmenu.add(shownumberrectitem = new JCheckBoxMenuItem("Show Number of Rectangles",true));
    debugmenu.add(shownumberoverlapitem = new JCheckBoxMenuItem("Show Most Overlap",false));
    debugmenu.add(shownextnumberrectitem = new JCheckBoxMenuItem("Show Next # Spaces",false));
    debugmenu.add(showjustinsiderectitem = new JCheckBoxMenuItem("Show Just Inside Spaces",false));
    debugmenu.add(showclosestitem = new JCheckBoxMenuItem("Show Closest",false));
    debugmenu.add(showgriditem = new JCheckBoxMenuItem("Show Grid",false));
    debugmenu.add(printObjectTreeitem = new JMenuItem("Print Object Tree"));
    debugmenu.add(printSpaceTreeitem = new JMenuItem("Print Space Tree"));
    debugmenu.add(testcloneitem = new JMenuItem("Test Clone"));
    printObjectTreeitem.setMnemonic('o');
    printObjectTreeitem.addActionListener(new ActionListener(){
	    public void actionPerformed(ActionEvent e){
		spacemanagerpanel.printObjectTree();
	    }
	});
    testcloneitem.setMnemonic('t');
    testcloneitem.addActionListener(new ActionListener(){
	    public void actionPerformed(ActionEvent e){
		spacemanagerpanel.testClone();
	    }
	});
    printSpaceTreeitem.setMnemonic('s');
    printSpaceTreeitem.addActionListener(new ActionListener(){
	    public void actionPerformed(ActionEvent e){
		spacemanagerpanel.printSpaceTree();
	    }
	});
    printrectanglesitem.setMnemonic('p');
    printrectanglesitem.addActionListener(new ActionListener(){
	    public void actionPerformed(ActionEvent e){
		spacemanagerpanel.printAllRectangles();
	    }
	});
    /*    addrectanglesitem.setMnemonic('a');
    addrectanglesitem.addActionListener(new ActionListener(){
	    public void actionPerformed(ActionEvent e){
		spacemanagerpanel.addRandomRectangles(100);
	    }
	});
    */
    debugitem.setMnemonic('d');
    debugitem.addActionListener(new ActionListener() {
	    public void actionPerformed(ActionEvent e){
		spacemanagerpanel.Debug(debugitem.getState());
	    }
	});
    shownumberrectitem.setMnemonic('d');
    shownumberrectitem.addActionListener(new ActionListener() {
	    public void actionPerformed(ActionEvent e){
		spacemanagerpanel.showNumberRectangles(shownumberrectitem.getState());
	    }
	});
    shownumberoverlapitem.setMnemonic('o');
    shownumberoverlapitem.addActionListener(new ActionListener() {
	    public void actionPerformed(ActionEvent e){
		shownextnumberrectitem.setState(false);
		spacemanagerpanel.setShowNextNumberRect(false);
		spacemanagerpanel.setShowOverlap(shownumberoverlapitem.getState());
		spacemanagerpanel.repaint();
	    }
	});

    showjustinsiderectitem.setMnemonic('j');
    showjustinsiderectitem.addActionListener(new ActionListener() {
	    public void actionPerformed(ActionEvent e){
		spacemanagerpanel.setShowInsideRect(showjustinsiderectitem.getState());
		spacemanagerpanel.repaint();
	    }
	});
    showclosestitem.setMnemonic('c');
    showclosestitem.addActionListener(new ActionListener() {
	    public void actionPerformed(ActionEvent e){
		spacemanagerpanel.setShowClosest(showclosestitem.getState());
		spacemanagerpanel.repaint();
	    }
	});
    showgriditem.setMnemonic('g');
    showgriditem.addActionListener(new ActionListener() {
	    public void actionPerformed(ActionEvent e){
		spacemanagerpanel.setShowGrid(showgriditem.getState());
		spacemanagerpanel.repaint();
	    }
	});
    shownextnumberrectitem.setMnemonic('o');
    shownextnumberrectitem.addActionListener(new ActionListener() {
	    public void actionPerformed(ActionEvent e){
		shownumberoverlapitem.setState(false);
		spacemanagerpanel.setShowOverlap(false);
		spacemanagerpanel.setShowNextNumberRect(shownextnumberrectitem.getState());
		spacemanagerpanel.showNextNumberRectangles(-1);
		spacemanagerpanel.repaint();
	    }
	});
    
    drawenclosedspacesitem.setMnemonic('h');
    drawenclosedspacesitem.addActionListener(new ActionListener(){
	public void actionPerformed(ActionEvent e){
	  spacemanagerpanel.setMode(SpaceManagerPanel.SHOWENCLOSED);
	  addrectitem.setState(false);
	  deleterectitem.setState(false);
	  moverectitem.setState(false);
	  movefastrectitem.setState(false);
	  drawenclosedspacesitem.setState(true);
	}
      });
    addrectitem.setMnemonic('a');
    addrectitem.addActionListener(new ActionListener() {
      public void actionPerformed(ActionEvent e){
	spacemanagerpanel.setMode(SpaceManagerPanel.ADDING);
	addrectitem.setState(true);
	deleterectitem.setState(false);
	moverectitem.setState(false);
	movefastrectitem.setState(false);
	drawenclosedspacesitem.setState(false);
      }
    });
    deleterectitem.setMnemonic('d');
    deleterectitem.addActionListener(new ActionListener() {
      public void actionPerformed(ActionEvent e){
	spacemanagerpanel.setMode(SpaceManagerPanel.DELETING);
	addrectitem.setState(false);
	deleterectitem.setState(true);
	moverectitem.setState(false);
	movefastrectitem.setState(false);
	drawenclosedspacesitem.setState(false);
      }
    });
    moverectitem.setMnemonic('m');
    moverectitem.addActionListener(new ActionListener() {
	    public void actionPerformed(ActionEvent e){
		spacemanagerpanel.setMode(SpaceManagerPanel.MOVING);
		addrectitem.setState(false);
		deleterectitem.setState(false);
		moverectitem.setState(true);
		movefastrectitem.setState(false);
		drawenclosedspacesitem.setState(false);
	    }
	});
    
    movefastrectitem.setMnemonic('f');
    movefastrectitem.addActionListener(new ActionListener() {
	    public void actionPerformed(ActionEvent e){
		spacemanagerpanel.setMode(SpaceManagerPanel.MOVINGFAST);
		addrectitem.setState(false);
		deleterectitem.setState(false);
		moverectitem.setState(false);
		movefastrectitem.setState(true);
		drawenclosedspacesitem.setState(false);
	    }
	});

    
    JMenuItem exitmenuitem;
    filemenu.add(newitem = new JMenuItem("New"));
    newitem.setMnemonic('n');
    filemenu.add(loaditem = new JMenuItem("Open"));
    loaditem.setMnemonic('o');
    filemenu.add(saveitem = new JMenuItem("Save"));
    saveitem.setMnemonic('s');
    jfilechooser.addChoosableFileFilter(filefilter);

    newitem.addActionListener(new ActionListener() {
	    public void actionPerformed(ActionEvent e){
		spacemanagerpanel.resetSpaceManager();
	    }
	});
    loaditem.addActionListener(new ActionListener() {
	    public void actionPerformed(ActionEvent e){
		//		JFrame frame = new JFrame();
		System.out.println("user.dir=" + System.getProperty("user.dir"));
		jfilechooser.setFileFilter(filefilter);
                jfilechooser.setMultiSelectionEnabled(false);
		jfilechooser.setFileSelectionMode(JFileChooser.FILES_ONLY);
		jfilechooser.rescanCurrentDirectory();
		//		jfilechooser.setSelectedFile(new File(""));
		
		if (jfilechooser.showOpenDialog(frame)==JFileChooser.APPROVE_OPTION){
		    System.out.println("jfilechooser approved file: " + jfilechooser.getSelectedFile().toString());
		    
		    shownumberoverlapitem.setState(false);
		    spacemanagerpanel.setShowOverlap(false);
		    shownextnumberrectitem.setState(false);
		    spacemanagerpanel.setShowNextNumberRect(false);
		    spacemanagerpanel.loadSpaceManager(jfilechooser.getSelectedFile());
		    //		    jfilechooser.setSelectedFile(new File(""));
		    spacemanagerpanel.showNumberRectangles(shownumberrectitem.getState());
		}
	    }
	});
    saveitem.addActionListener(new ActionListener() {
	    public void actionPerformed(ActionEvent e){
		jfilechooser.setFileFilter(filefilter);
                jfilechooser.setMultiSelectionEnabled(false);
		jfilechooser.setFileSelectionMode(JFileChooser.FILES_ONLY);
		if (jfilechooser.showSaveDialog(frame)==JFileChooser.APPROVE_OPTION){
		    System.out.println("jfilechooser approved file: " + jfilechooser.getSelectedFile().toString());
		    spacemanagerpanel.saveSpaceManager(jfilechooser.getSelectedFile());
		}
		//		System.exit(0);
	    }
	});
    
    filemenu.add(exitmenuitem = new JMenuItem("Exit"));
    exitmenuitem.setMnemonic('x');
    if (applet){
      exitmenuitem.addActionListener(new ActionListener() {
	public void actionPerformed(ActionEvent e){
	  spacemanagerpanel.setVisible(false);
	  menubar.setVisible(false);
	  filemenu.setVisible(false);
	  repaint(0,0, getSize().width, getSize().height);
	}
      });
    } else {
      exitmenuitem.addActionListener(new ActionListener() {
	public void actionPerformed(ActionEvent e){
	  System.exit(0);
	}
      });
    }
  } 
  public SpaceManagerTest() {
  }
  public SpaceManagerTest(String args[]) {
  }
  static SpaceManagerTest smt;
  static JFrame frame;
    static JTextComponent numrectfield;
    static JComboBox numoverlap,numnext;
    static ItemListener il;
    static ListDataListener ldl;
  public static void main(String[] args) {
    frame = new JFrame("SpaceManager");
    frame.addWindowListener(new WindowAdapter() {
      public void windowClosing(WindowEvent e) {
	System.exit(0);
      }
    });
    smt = new SpaceManagerTest(args);
    smt.jlist = new JList((ListModel)(smt.defaultlistmodel = new DefaultListModel()));
    smt.jlist.setFixedCellWidth(40);
    smt.defaultlistmodel.addListDataListener( ldl = new ListDataListener(){
	    public void intervalAdded(ListDataEvent e){
		if (e.getType()==ListDataEvent.INTERVAL_ADDED){
		}
	    }
	    public void intervalRemoved(ListDataEvent e){
	    }
	    public void contentsChanged(ListDataEvent e){
	    }
	});
    
    smt.createPanel(false);
    JPanel jpanel = new JPanel (new BorderLayout());
    jpanel.add("Center",smt.spacemanagerpanel);
    String[] data = {"one", "two", "free", "four"};
    smt.jlist.addListSelectionListener(new ListSelectionListener() {
      public void valueChanged(ListSelectionEvent e){
	spacemanagerpanel.repaint();
      }
    });
    MouseListener mouseListener = new MouseAdapter() {
      public void mouseClicked(MouseEvent e) {
	int index = smt.jlist.locationToIndex(e.getPoint());
	if (index<0 || (e.getModifiers() & MouseEvent.BUTTON3_MASK)>0){
	  smt.jlist.removeSelectionInterval(0,smt.jlist.getModel().getSize());
	}
      }
    };
    smt.jlist.addMouseListener(mouseListener);
 
    JPanel statusbar;
    JScrollPane sp;
    jpanel.add("East", sp=new JScrollPane(smt.jlist));
    sp.setHorizontalScrollBarPolicy(JScrollPane.HORIZONTAL_SCROLLBAR_AS_NEEDED);
    GridBagLayout gridbag = new GridBagLayout();
    GridBagConstraints c = new GridBagConstraints();
    c.fill = GridBagConstraints.BOTH;
    c.weightx = 1.0;
    jpanel.add("South", statusbar = new JPanel(gridbag));
    JTextField maxnumsp;
    statusbar.add(maxnumsp=new JTextField("Max # of Spaces:"));
    
    maxnumsp.setEditable(false);
    statusbar.add(numrectfield=new JTextField(10));
    numrectfield.setEditable(false);
    spacemanagerpanel.setMaxRectangleDisplay(numrectfield);
    spacemanagerpanel.showNumberRectangles(true);
    statusbar.add(maxnumsp=new JTextField("# overlap spaces:"));
    maxnumsp.setEditable(false);

    statusbar.add(numoverlap = new JComboBox());
    Dimension d=numoverlap.getPreferredSize();
    numoverlap.setPreferredSize(new Dimension(70,d.height));
    numoverlap.setMinimumSize(new Dimension(70,d.height));
    numoverlap.setMaximumSize(new Dimension(70,d.height));
    spacemanagerpanel.setNumberOverlapChooser(numoverlap);

    statusbar.add(maxnumsp=new JTextField("Next Spaces:"));
    maxnumsp.setEditable(false);

    statusbar.add(numnext = new JComboBox());
    spacemanagerpanel.setNumberNextChooser(numnext);

    numnext.setPreferredSize(new Dimension(70,d.height));
    numnext.setMinimumSize(new Dimension(70,d.height));
    numnext.setMaximumSize(new Dimension(70,d.height));

    numoverlap.addItemListener(il=new ItemListener(){
	    public void itemStateChanged(ItemEvent e){
		if (e.getStateChange()==ItemEvent.SELECTED){
		    numoverlap.removeItemListener(il);
		    Integer i = new Integer((String)e.getItem());
		    if (shownextnumberrectitem.getState()){
			spacemanagerpanel.showNextNumberRectangles(i.intValue());
		    } else if (shownumberoverlapitem.getState()){
			spacemanagerpanel.showOverlapRectangles(i.intValue());
		    }

		    numoverlap.setSelectedItem(e.getItem());
		    numoverlap.addItemListener(il);
		    spacemanagerpanel.repaint();
		}
	    }
	});
    frame.setContentPane(jpanel); 
    frame.setJMenuBar(smt.menubar);
    
    frame.pack();
    frame.setVisible(true);
  }
  public String getAppletInfo() {
    return "A Space Manager";
  }

}
