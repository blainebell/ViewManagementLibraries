
#include "BSPObjectTree.h"

#include "IntervalTree.h"

#define CLASSNAME "BSPObjectTree"

void BSPObjectTree::print(){
    stringstream ss;
    ss << *this;
    cerr << ss.str().c_str() << endl;
}

BSPObjectTree::BSPObjectTree() : root(NULL) {
}

BSPObjectTree::~BSPObjectTree(){
    clear();
    for (auto it=allBSPObjects.begin(); it!=allBSPObjects.end(); it++){
        delete it->second;
    }
    allBSPObjects.clear();
}

void BSPObjectTree::clear(){
    if (root)
        delete root;
    root = NULL;
}

int BSPObjectTree::add(BSPObject *obj) {
  allBSPObjects[obj->_id] = obj;
}

int BSPObjectTree::removeObjectID(int idval){
    int numNodesRemoved = -1;
    if (root){
        numNodesRemoved = root->removeID(idval);
        if (!root->hasParts()){
            delete root;
            root = NULL;
        }
    }
    for (auto it = allBSPObjects.begin(); it!= allBSPObjects.end(); ){
        auto bspid = it->first;
        auto bspo = it->second;
        if (idval == bspid){
            it = allBSPObjects.erase(it);
            delete bspo;  // deleting BSPObject
        } else {
            it++;
        }
    }
    return numNodesRemoved;
}

ostream& operator <<(ostream &os, const BSPObjectTree &obj){
    if (obj.root)
        obj.root->printSubTree(os, 0);
    return os;
}
void BSPObjectTree::printTree(ostream &os){
    if (root){
        root->printSubTree(os);
    }
}

#define ALWAYS_CHOOSE_FACE_FROM_OBJECT

Plane *forceChoosingPlane(Rectangle3d *rect, vector <BSPObjectPart *> *objectparts, IntervalTree3d &intervalTree3d){
    // for each part, find the edge of a bbx that is furthest away from the bounding box
    // of all parts (rect).  if all too close, then choose plane from object
    Rectangle3d tmpRect, tmpRect2;
    Plane retPlane;
#ifndef ALWAYS_CHOOSE_FACE_FROM_OBJECT
    static float minSizeToSplit = 2.f;
    double distance = 0;
    bool chooseFaceFromObject = true;
    if (rect->getWidth()<minSizeToSplit || rect->getHeight()<minSizeToSplit || rect->getDepth()<minSizeToSplit){
        chooseFaceFromObject=true;
    }
    if (!chooseFaceFromObject){
        for (auto it=objectparts->begin(); it!=objectparts->end(); it++){
            auto overV = intervalTree3d.windowQuery((*it)->bbx);
            for (auto it2=overV->begin(); it2!=overV->end(); it2++){
                tmpRect.set(*it2);
                Rectangle3d *rect = &(*it)->bbx;
                intersect(tmpRect, *rect, tmpRect2);
                if (tmpRect2.minx-rect->minx> distance){
                    distance = tmpRect2.minx - rect->minx;
                    retPlane.x = 1; retPlane.y = 0.; retPlane.z = 0.; retPlane.w = -tmpRect2.minx+PLANE_SPLIT;
                }
                if (rect->maxx-tmpRect2.maxx > distance){
                    distance = rect->maxx-tmpRect2.maxx;
                    retPlane.x = 1; retPlane.y = 0.; retPlane.z = 0.; retPlane.w = -tmpRect2.maxx-PLANE_SPLIT;
                }
                if (tmpRect2.miny-rect->miny > distance){
                    distance = tmpRect2.miny-rect->miny;
                    retPlane.x = 0; retPlane.y = 1.; retPlane.z = 0.; retPlane.w = -tmpRect2.miny+PLANE_SPLIT;
                }
                if (rect->maxy-tmpRect2.maxy > distance){
                    distance = rect->maxy-tmpRect2.maxy;
                    retPlane.x = 0.; retPlane.y = 1.; retPlane.z = 0.; retPlane.w = -tmpRect2.maxy-PLANE_SPLIT;
                }
                if (tmpRect2.minz-rect->minz > distance){
                    distance = tmpRect2.minz-rect->minz;
                    retPlane.x = 0.; retPlane.y = 0.; retPlane.z = 1.; retPlane.w = -tmpRect2.minz+PLANE_SPLIT;
                }
                if (rect->maxz-tmpRect2.maxz > distance){
                    distance = rect->maxz-tmpRect2.maxz;
                    retPlane.x = 0.; retPlane.y = 0.; retPlane.z = 1.; retPlane.w = -tmpRect2.maxz-PLANE_SPLIT;
                }
            }
            delete overV;
        }
    }
    
    if (chooseFaceFromObject || distance < TOLER){
#else
    {
#endif
        /** This means that all object part's bounding boxes are the same
         Hence, we need to pick other planes, and break them up
         find part with the least number of faces, and pick the first plane */
        int minfaces = INT_MAX;
        vector<Face*> *retAllFaces = NULL;
        int objid = 0;
        for (auto it=objectparts->begin(); it!=objectparts->end(); it++){
            auto allFaces = (*it)->allFaces;
            if (allFaces->size() < minfaces){
                retAllFaces = allFaces;
                minfaces = (int)allFaces->size();
                objid = (*it)->bspObject->_id;
            }
        }
        if (retAllFaces && minfaces > 0){
            return new Plane(*retAllFaces->at(0)->plane);
        }
        // something went wrong
        return NULL;
    }
    
    return new Plane(retPlane);
}
Plane *choosePlane(vector <BSPObjectPart *> *objectparts, Rectangle3d *rect, bool *isSplitPlane, bool debug = false){
    Rectangles3D rects_x(Rectangle3d::minx_compare);
    Rectangles3D rects_y(Rectangle3d::miny_compare);
    Rectangles3D rects_z(Rectangle3d::minz_compare);
    Rectangles3D *mintree[] = { (Rectangles3D *)&rects_x, (Rectangles3D *)&rects_y, (Rectangles3D *)&rects_z };
    IntervalTree3d intervalTree3d;
    bool isAllOverlap = true, first = true;
    rect->set(MAXFLOAT,MAXFLOAT,MAXFLOAT,-MAXFLOAT,-MAXFLOAT,-MAXFLOAT);
    for (auto it=objectparts->begin(); it!=objectparts->end(); it++){
        mintree[0]->insert((*it)->bbx);
        mintree[1]->insert((*it)->bbx);
        mintree[2]->insert((*it)->bbx);
        rect->add((*it)->bbx);
        if (!first && isAllOverlap && !intervalTree3d.isOverlapped((*it)->bbx)){
            isAllOverlap = false;
        }
        intervalTree3d.addData((*it)->bbx);
        first = false;
    }
    
    if (objectparts->size() == 1){
        BSPObjectPart *bspop = (*objectparts)[0];
        BSPObject *bspo = bspop->bspObject;
        if (bspo && bspo->hasMoreSplitPlanes()){
            Plane splitPlane = bspo->nextSplitPlane();
            int side;
            bool oneside = splitPlane.isOnOneSide(bspop->bbx, side);
            if (debug){
                stringstream ss;
                ss << "choosePlane: 1 object part, considering splitPlane=" << splitPlane << " oneside=" << oneside << " side=" << side << endl;
                cerr << ss.str();
            }
            if (!oneside){
                bspo->useSplitPlane();
                isSplitPlane[0] = true;
                return new Plane(splitPlane);
            } else {
                return NULL;
            }
        }
    }

    if (isAllOverlap){
        return forceChoosingPlane(rect, objectparts, intervalTree3d);
    }
    
    int numberOfTotalObjects = (int)objectparts->size();
    int bestplanedim=0, bestNumOverlappedObjects = 0;
    double bestplaneposition=0, bestplanevalue=0;
    bool hasbestplane=false;
    
    stringstream ssout ;
    if (debug){
        ssout << "choosePlane: numberOfTotalObjects=" << numberOfTotalObjects << endl;
    }
    for (int dimension=0; dimension<3; dimension++){
        int numberOfPassedObjects = 0;
        double nextmax = 0.; // next value that is for rectangle's end interval
        double rectmax = 0.;
        auto it=mintree[dimension]->begin();
        Rectangle3d *currentRect = (Rectangle3d *)&(*it); it++;
        Rectangles3D *maxtree = NULL;
        double min = 0., max = 0.;
        switch (dimension){
            case 0:
                maxtree = (Rectangles3D*) new Rectangles3D(Rectangle3d::maxx_compare);
                min = rect->minx;
                max = currentRect->minx;
                nextmax = currentRect->maxx;
                rectmax = rect->maxx;
                break;
            case 1:
                maxtree = (Rectangles3D*) new Rectangles3D(Rectangle3d::maxy_compare);
                min = rect->miny;
                max = currentRect->miny;
                nextmax = currentRect->maxy;
                rectmax = rect->maxy;
                break;
            case 2:
                maxtree = (Rectangles3D*) new Rectangles3D(Rectangle3d::maxz_compare);
                min = rect->minz;
                max = currentRect->minz;
                nextmax = currentRect->maxz;
                rectmax = rect->maxz;
                break;
        }
        maxtree->insert(*currentRect);
        bool isnextmax=true;
        bool beginRect=true, // Last value was set by beginning of rectangle
        fromMaxTree=true;
        bool first=true;
        int numberOverlappedObjects=1;
        double curRectMin=-MAXFLOAT;
        while (it != mintree[dimension]->end() || max < rectmax){
            if (beginRect){
                if (it != mintree[dimension]->end()){ // hasNext
                    // need to get next rectangle
                    currentRect = (Rectangle3d *)&(*it); it++;
                    if (first){
                        double tmpV=0;
                        switch (dimension){
                            case 0: tmpV = currentRect->maxx; break;
                            case 1: tmpV = currentRect->maxy; break;
                            case 2: tmpV = currentRect->maxz; break;
                        }
                        if (tmpV < nextmax){
                            nextmax = tmpV;
                        }
                        first = false;
                    }
                } else {
                    currentRect = NULL;
                }
                beginRect=false;
            }
            if (currentRect!=NULL){
                switch (dimension){
                    case 0: curRectMin = currentRect->minx; break;
                    case 1: curRectMin = currentRect->miny; break;
                    case 2: curRectMin = currentRect->minz; break;
                }
            }
            if (isnextmax && (currentRect==NULL || curRectMin > nextmax)){
                // this interval ends with the max value of first rectangle
                // in the maxxtree. This rectangle (as well as all of them
                // in maxxtree should be included in fpi (and area if calculating)
                const Rectangle3d tmpRect = *maxtree->begin();
                min = max;
                switch (dimension){
                    case 0: max = tmpRect.maxx; break;
                    case 1: max = tmpRect.maxy; break;
                    case 2: max = tmpRect.maxz; break;
                }
                maxtree->erase(maxtree->begin());
                if (!maxtree->empty()){
                    const Rectangle3d tmpRect2 = *maxtree->begin();
                    switch (dimension){
                        case 0: nextmax = tmpRect2.maxx; break;
                        case 1: nextmax = tmpRect2.maxy; break;
                        case 2: nextmax = tmpRect2.maxz; break;
                    }
                    isnextmax=true;
                } else {
                    isnextmax=false;
                }
                fromMaxTree=true;
            } else {
                // this interval ends with the next rectangle: currentRect, it
                // also includes all rectangles in maxxtree in fpi (and area if calculating)
                min = max;
                if (!currentRect){
                    break;
                }
                maxtree->insert((Rectangle3d)*currentRect);
                const Rectangle3d tmpRect = *maxtree->begin();
                switch (dimension){
                    case 0:
                        max = currentRect->minx;
                        nextmax = tmpRect.maxx;
                        break;
                    case 1:
                        max = currentRect->miny;
                        nextmax = tmpRect.maxy;
                        break;
                    case 2:
                        max = currentRect->minz;
                        nextmax = tmpRect.maxz;
                        break;
                }
                isnextmax = true;
                beginRect = true;
                fromMaxTree = false;
            }
            // compute value for plane inside this area
            int oneSide = numberOfTotalObjects - numberOfPassedObjects - numberOverlappedObjects;
            double tmpplanevalue = numberOverlappedObjects + fmax(oneSide, numberOfPassedObjects)
            / (fmin( oneSide, numberOfPassedObjects ) + 1.);
            if ((numberOfPassedObjects!=0) &&
                (oneSide!=0) &&
                (!hasbestplane ||
                 (numberOverlappedObjects==0 && bestNumOverlappedObjects!=0) ||
                 ( !(numberOverlappedObjects!=0 && bestNumOverlappedObjects==0) && bestplanevalue > tmpplanevalue))){
                    bestplanedim = dimension;
                    bestplanevalue = tmpplanevalue;
                    bestNumOverlappedObjects = numberOverlappedObjects;
                    bestplaneposition = (max+min)/2.;
                    hasbestplane = true;
            }
            if (fromMaxTree){
                numberOfPassedObjects++;
                numberOverlappedObjects--;
            } else {
                numberOverlappedObjects++;
            }
        }
        if (debug){
            ssout << "      dimension=" << dimension << " hasbestplane=" << hasbestplane << " bestplanedim=" << bestplanedim << " bestplanevalue=" << bestplanevalue << " bestplaneposition=" << bestplaneposition << " bestNumOverlappedObjects=" << bestNumOverlappedObjects << endl;
            
        }
        delete maxtree;
    }
    
    if (!hasbestplane){
        stringstream ss;
        ss << "\tchoosePlane: WARNING: could not find best plane: #objectparts=" << IntToString((int)objectparts->size()) << ", forcing" << endl;
        for (auto it=objectparts->begin(); it!=objectparts->end(); it++){
            ss << "    : " << *(*it) << endl;
        }
        cerr << ss.str().c_str();
        return forceChoosingPlane(rect, objectparts, intervalTree3d);
    }
    Plane *bestPlane = NULL;
    switch (bestplanedim){
        case 0:
            bestPlane = new Plane(1.,0.,0.,-bestplaneposition);
            break;
        case 1:
            bestPlane = new Plane(0.,1.,0.,-bestplaneposition);
            break;
        case 2:
            bestPlane = new Plane(0.,0.,1.,-bestplaneposition);
            break;
    }
    if (debug){
      cerr << ssout.str().c_str();
    }
    return(bestPlane);
}

void partitionObjectsWithPlane(Plane *plane, vector<BSPObjectPart*> *objectparts,
                               vector<BSPObjectPart*> *objectsNeg, vector<BSPObjectPart*> *objectsPos,
                               vector<BSPObjectPart*> *piecesSameDir, vector<BSPObjectPart*> *piecesOppDir){
    for (auto it = objectparts->begin(); it!=objectparts->end(); it++){
        BSPObjectPart *part = (*it);
        part->split(plane, objectsNeg, objectsPos, piecesSameDir, piecesOppDir);
        if (part->allFaces->size()==0)
            delete part;
        else
	  cerr << "WARNING: partitionObjectsWithPlane part split but some faces are still lingering";
    }
    objectparts->clear();
}


void BSPObjectTree::generate(){
  vector<BSPObjectPart*> allParts;
  cout << "generate() called" << endl;
  for (auto it=allBSPObjects.begin(); it!=allBSPObjects.end(); it++){
    auto bspid = it->first;
    auto bspobj = it->second;
    for (auto it=bspobj->allParts.begin(); it!=bspobj->allParts.end(); it++){
      allParts.push_back(*it);
    }
  }

  root = generateBSPObjectSubTree(allParts);

  allParts.clear();
}

BSPObjectNode *generateBSPObjectSubTree(vector<BSPObjectPart*> &allParts, bool debug){
    // lets construct the BSPObjectTree
    vector< vector<BSPObjectPart*> * > nodequeue;
    vector<BSPObjectPart*> *allPartsCopy = new vector<BSPObjectPart*>();
    push_back_all(*allPartsCopy, allParts);
    nodequeue.push_back(allPartsCopy);

    vector< pair<BSPObjectNode*,bool> > returnBSPInfo; // queue of returnBSPNodes and returnBSPSide
    BSPObjectNode *bspnodereturn = NULL;
    
    while (!nodequeue.empty()){
        vector <BSPObjectPart *> *objectparts = *nodequeue.begin();
        nodequeue.erase(nodequeue.begin());
        Rectangle3d rect;
        bool isSplitPlane[1] = { false };
        Plane *plane = choosePlane(objectparts, &rect, isSplitPlane, debug);
        if (debug){
            stringstream ss;
            ss << " generateBSPObjectSubTree: debug : choosePlane: plane=";
            ss << ((plane != NULL) ? plane->toString() : "NULL");
            ss << " rect=" << rect << endl;
            cerr << ss.str().c_str();
        }
        if (plane==NULL){
            if (objectparts->size() == 1){
                auto retinfo = returnBSPInfo.begin();
                BSPObjectNode *parentBspObjectNode = retinfo->first;
                bool parentSide = retinfo->second;
                if (debug){
                    stringstream ss;
                    ss << "plane is NULL, setting parent node's child to this one part: parentBspObjectNode=" << parentBspObjectNode << " parentSide=" << parentSide << endl;
                    cerr << ss.str();
                }
                BSPObjectNode *newBspObjectNode = new BSPObjectNode((*objectparts)[0]);
                newBspObjectNode->setToBounds(rect);
                if (parentSide){
                    parentBspObjectNode->_positiveSide = newBspObjectNode;
                } else {
                    parentBspObjectNode->_negativeSide = newBspObjectNode;
                }
                returnBSPInfo.erase(returnBSPInfo.begin());
            }
        } else {
            vector<BSPObjectPart*> *objectsNeg = new vector<BSPObjectPart*>(),
            *objectsPos = new vector<BSPObjectPart*>(),
            *piecesSameDir = new vector<BSPObjectPart*>(),
            *piecesOppDir = new vector<BSPObjectPart*>();
            
            partitionObjectsWithPlane(plane, objectparts, objectsNeg, objectsPos, piecesSameDir, piecesOppDir);
            if (debug){
                stringstream ss;
                ss << "partitioned Objects: #parts=" << objectparts->size() << " : #neg=" << objectsNeg->size() << " #pos=" << objectsPos->size() << " #same=" << piecesSameDir->size() << " #opp=" << piecesOppDir->size() << " returnBSPInfo.empty()=" << returnBSPInfo.empty() << endl;
                ss << "objectsPos:" << endl;
                int pl = 0;
                for (auto part = objectsPos->begin(); part != objectsPos->end(); part++){
                    ss << "\t#" << pl << " : " << faceListToStringDirect(*(*part)->allFaces) << endl;
                    pl++;
                }
                ss << "objectsNeg:" << endl;
                pl = 0;
                for (auto part = objectsNeg->begin(); part != objectsNeg->end(); part++){
                    ss << "\t#" << pl << " : " << faceListToStringDirect(*(*part)->allFaces) << endl;
                    pl++;
                }
                cerr << ss.str();
            }
            BSPObjectNode *newBspObjectNode = new BSPObjectNode(plane, piecesSameDir, piecesOppDir);
            newBspObjectNode->isSplitPlane = isSplitPlane[0];
            newBspObjectNode->setToBounds(rect);
            
            if (!returnBSPInfo.empty()){
                auto retinfo = returnBSPInfo.begin();
                BSPObjectNode *parentBspObjectNode = retinfo->first;
                bool parentSide = retinfo->second;
                if (parentSide){
                    parentBspObjectNode->_positiveSide = newBspObjectNode;
                } else {
                    parentBspObjectNode->_negativeSide = newBspObjectNode;
                }
                returnBSPInfo.erase(returnBSPInfo.begin());
            } else {
                bspnodereturn = newBspObjectNode;
            }
            
            /* Construct Tree's "-" branch */
            if (objectsNeg->size()==1){
                BSPObjectPart *part = *objectsNeg->begin();
                if (!part->bspObject->hasMoreSplitPlanes()){
                    newBspObjectNode->_negativeSide = new BSPObjectNode(part);
                    delete objectsNeg;
                } else {
                    returnBSPInfo.insert(returnBSPInfo.begin(), pair<BSPObjectNode*, bool>(newBspObjectNode, false));
                    nodequeue.insert(nodequeue.begin(), objectsNeg);
                }
            } else if (objectsNeg->size()==0){
                newBspObjectNode->_negativeSide = new BSPObjectNode(BSPObjectNode::IN_NODE);
                delete objectsNeg;
            } else {
                returnBSPInfo.push_back(pair<BSPObjectNode*, bool>(newBspObjectNode, false));
                nodequeue.push_back(objectsNeg);
            }
            
            /* Construct Tree's "+" branch */
            if (objectsPos->size()==1){
                BSPObjectPart *part = *objectsPos->begin();
                if (!part->bspObject->hasMoreSplitPlanes()){
                    newBspObjectNode->_positiveSide = new BSPObjectNode(part);
                    delete objectsPos;
                } else {
                    returnBSPInfo.insert(returnBSPInfo.begin(), pair<BSPObjectNode*, bool>(newBspObjectNode, true));
                    nodequeue.insert(nodequeue.begin(), objectsPos);
                }
            } else if (objectsPos->size()==0){
                newBspObjectNode->_positiveSide = new BSPObjectNode(BSPObjectNode::OUT_NODE);
                delete objectsPos;
            } else {
                returnBSPInfo.push_back(pair<BSPObjectNode*, bool>(newBspObjectNode, true));
                nodequeue.push_back(objectsPos);
            }
        }
        delete objectparts;
    }
    return bspnodereturn;
}

int BSPObjectTree::getNumberOfParts(int objid){
    if (allBSPObjects.find(objid) != allBSPObjects.end()){
        BSPObject *bspo = allBSPObjects[objid];
        return (int)bspo->allParts.size();
    }
    return -1;
}
int BSPObjectTree::getNumberOfTotalParts(){
    int nparts = 0;
    if (root){
        nparts = root->getNumberOfTotalParts();
    }
    return nparts;
}
int BSPObjectTree::getNumberOfTotalObjects(){
   return (int)allBSPObjects.size();
}
