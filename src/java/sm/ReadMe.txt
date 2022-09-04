Space Management Software Package (Copyright (C) 2001 by Columbia University)
Authored by Blaine Bell

This package includes software that implements space management algorithms as documented in the UIST 2000 paper 'Dynamic Space Management for User Interfaces'. It is implemented in Java (version 1.3). Packages include:

     edu.columbia.cs.cgui.spam2d - 2 dimensional Space Manager using double floating point precision
     edu.columbia.cs.cgui.spam2i - 2 dimensional Space Manager using integer precision
     edu.columbia.cs.cgui.spam3d - 3 dimensional Space Manager using double floating point precision
     edu.columbia.cs.cgui.InternalFrameDemo - Window Manager Demonstration Program

WARNING!!:: Use at your own risk!!! The double floating point precision algorithms might have a floating point precision bug (depending on how you use it). It is included in this implementation specifically because the Window Manager example uses it.

Documentation:

All of the documentation can be created using JavaDoc. Most of the comments are in the Integer precision directory (spam2i). Please see javadoc for instructions on how to use it.


Reporting Bugs

Please email me at blaine@cs.columbia.edu for any questions, comments, or suggestions on the implementation.  I am all ears!

Demonstration Programs

I have provided batch files to run the two demonstration programs.

InternalFrameDemo.bat - Window Manager Demonstration Program
    This demonstration program shows how we have used the space manager to implement a window manager that tries to avoid window overlapping when possible. It has 4 functions that are interesting:
    1) It creates a new window in a place where it doesn't overlap any existing windows. If it can't find a place, then it places it in a place that takes up the most empty space, so there is minimal overlapping.
 In order to choose these next functions, you need to check the control box's checkbox accordingly:
    2) 'After Drag Move...' - If this check box is checked, the window manager moves a dropped window (i.e. after it has been dragged) immediately, if the dragged window is overlapping any other windows.  It moves it to the closest place on the screen that avoids the overlapping.
    3) 'Move on Drop' - If this check box is checked, the window manager moves any windows that are overlapped by a dropped window immediately. It moves all of these windows to the closest place on the screen that avoids overlapping.
    4) 'Continuous Move' - This does the same as #3, but is does it continuously.

    5) (No Checkbox necessary) Maximize window Button - If a user clicks the maximize button on one of the windows, and the screen has at least 50 percent of it empty, then the window maximizes into the largest area that doesn't overlap any other windows. This is contrary to regular window managers that maximize the window to take up the entire screen.
    
SpaceManagerTest.bat - Space Manager Demonstration Program
    This program demonstrates the Space Management Representation. You can add, remove, and move full space rectangles, and the program shows the empty space representation which consists of a list of rectangles. To observe the list of rectangles, click on the number list on the right hand side of the program. 
