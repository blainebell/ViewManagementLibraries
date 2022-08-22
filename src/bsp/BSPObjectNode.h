

#ifndef CPP_BSPOBJECTNODE_H
#define CPP_BSPOBJECTNODE_H



#include <iostream>
#include "BSPObjectPart.h"
#include "Plane.h"

using namespace std;

class BSPObjectNode {
public:
    static char PARTITION_NODE;
    static char IN_NODE;
    static char OUT_NODE;
    static char CHILD_NODE;
    
    int bspObjectNodeID;
    bool isSplitPlane;
    char _type;
    Plane *_plane;
    vector<BSPObjectPart *> *_sameDir, *_oppDir;
    Rectangle3d *bounds;
    
    BSPObjectNode *_negativeSide, *_positiveSide;
    BSPObjectPart *_part;

    void addToBounds(Rectangle3d &ab){
        if (bounds)
            bounds->add(ab);
        else
            bounds = new Rectangle3d(ab);
    }
    void setToBounds(Rectangle3d &ab){
        if (bounds)
            bounds->set(ab);
        else
            bounds = new Rectangle3d(ab);
    }

    int getNumberOfTotalParts();
    BSPObjectNode();
    BSPObjectNode(char t);
    BSPObjectNode(Plane *p);
    BSPObjectNode(BSPObjectPart *part);
    BSPObjectNode(Plane *p, vector<BSPObjectPart*> *sameDir, vector<BSPObjectPart*> *oppDir);
    void setToChildNode(BSPObjectPart *part);
    ~BSPObjectNode();
    string toString();
    friend ostream& operator <<(ostream &os,BSPObjectNode &obj);
    void printSubTree(ostream &os, int tabs = 0);
    void traverseFrontToBackFromPoint(string &, Point3d &, Point3d &, bool, bool plusPartNumber = false, bool debug = false);
    void addBSPObjectPart(BSPObjectPart *);
    int removeID(int idval);
    bool hasParts();
};

#endif
