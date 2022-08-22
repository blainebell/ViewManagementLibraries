
#ifndef CPP_BSPOBJECT_H
#define CPP_BSPOBJECT_H

#include "Plane.h"
#include "stlTools.h"

class BSPObjectPart;

class BSPObject {
public:
    int _id;
    set<BSPObjectPart*> allParts;
    vector<Plane> splitPlanes;
    int splitPlanesUsed;
    bool hasSplitPlane(){ return !splitPlanes.empty(); };
    bool hasMoreSplitPlanes(){ return splitPlanesUsed < (int)splitPlanes.size(); };
    Plane nextSplitPlane() { return splitPlanes[splitPlanesUsed]; };
    void useSplitPlane() { splitPlanesUsed++; };
    BSPObject(int);
};


#endif
