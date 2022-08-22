
#ifndef CPP_BSPOBJECTTREE_H
#define CPP_BSPOBJECTTREE_H

#include "BSPObjectNode.h"

class BSPObjectTree {
public:
    map<int, BSPObject*> allBSPObjects;
    BSPObjectNode *root;
    BSPObjectTree();
    ~BSPObjectTree();
    int add(BSPObject *);
    int removeObjectID(int);
    int getNumberOfParts(int);
    int getNumberOfTotalParts();
    int getNumberOfTotalObjects();
    void print();
    void clear();
    virtual const string toString(){return ( "-BSPObjectTree-" );}
    friend ostream& operator <<(ostream &os, const BSPObjectTree &obj);
    void printTree(ostream &os);
    void generate();
};

BSPObjectNode *generateBSPObjectSubTree(vector<BSPObjectPart*> &, bool debug=false);

#endif
