package edu.columbia.cs.cgui.spam2d;

import javax.swing.*;
import java.awt.event.*;
import java.awt.*;
import javax.swing.text.*;

public class ToolTipDemo extends JFrame {
  public static void main(String[] args) {
      ToolTipDemo tooltipdemo = new ToolTipDemo();
      tooltipdemo.addWindowListener(new WindowAdapter() {
	      public void windowClosing(WindowEvent e) {
		  System.exit(0);
	      }
	  });
      tooltipdemo.setSize(500,500);
      JPanel jpanel = new JPanel();
      jpanel.setLayout(new BoxLayout(jpanel,BoxLayout.X_AXIS));
      jpanel.add(new JButton("Test"));
      tooltipdemo.setContentPane(jpanel);
      tooltipdemo.setVisible(true);
      SpaceManager sm ;
      Dimension dim=tooltipdemo.getSize();
      try {
	  sm = new SpaceManager(new Rectangle2d(0.,0.,dim.getWidth(),dim.getHeight()));
      } catch (Exception e){
	  e.printStackTrace();
      }
  }
}
