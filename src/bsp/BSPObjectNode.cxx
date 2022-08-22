
#include "BSPObjectNode.h"
//#include "KBFaces3D.h"
//#include "KBCuboidD.h"
#include "BSPObjectTree.h"

static int bspobjectnodeCounter=1;

char BSPObjectNode::PARTITION_NODE='p';
char BSPObjectNode::IN_NODE='i';
char BSPObjectNode::OUT_NODE='o';
char BSPObjectNode::CHILD_NODE='c';

BSPObjectNode::BSPObjectNode(Plane *p, vector<BSPObjectPart*> *sameDir, vector<BSPObjectPart*> *oppDir) : _negativeSide(NULL), _positiveSide(NULL), _part(NULL), bounds(NULL) {
    bspObjectNodeID = bspobjectnodeCounter++;
    _plane = p;
    _type = PARTITION_NODE;
    _sameDir = sameDir;
    _oppDir = oppDir;
    for (auto obj = sameDir->begin(); obj!=sameDir->end(); obj++)
        addToBounds((*obj)->bbx);
    for (auto obj = oppDir->begin(); obj!=oppDir->end(); obj++)
        addToBounds((*obj)->bbx);
/*
    cout << "plane=" << *_plane << endl;
    cout << "\tsameDir: planes:\n";
    for (auto obj = sameDir->begin(); obj!=sameDir->end(); obj++){
        for (auto faceit = (*obj)->allFaces->begin(); faceit!=(*obj)->allFaces->end(); faceit++){
            cout << "\t\t" << *(*faceit)->plane << endl;
        }
    }
    cout << "\toppDir: normals: " << endl;
    for (auto obj = oppDir->begin(); obj!=oppDir->end(); obj++){
        for (auto faceit = (*obj)->allFaces->begin(); faceit!=(*obj)->allFaces->end(); faceit++){
            cout << "\t\t" << *(*faceit)->plane << endl;
        }
    }
 */
    
}

BSPObjectNode::BSPObjectNode(BSPObjectPart *part) : _negativeSide(NULL), _positiveSide(NULL), _plane(NULL), _sameDir(NULL), _oppDir(NULL), bounds(NULL) {
    bspObjectNodeID = bspobjectnodeCounter++;
    _type = CHILD_NODE;
    _part = part;
    addToBounds(part->bbx);
}

void BSPObjectNode::setToChildNode(BSPObjectPart *part){
    _type = CHILD_NODE;
    _part = part;
    setToBounds(part->bbx);
}
BSPObjectNode::BSPObjectNode(Plane *p) : _negativeSide(NULL), _positiveSide(NULL), _part(NULL), _sameDir(NULL), _oppDir(NULL), bounds(NULL) {
    bspObjectNodeID = bspobjectnodeCounter++;
    _plane = p;
    _type = PARTITION_NODE;
}

BSPObjectNode::BSPObjectNode(char t) : _negativeSide(NULL), _positiveSide(NULL), _part(NULL), _sameDir(NULL), _oppDir(NULL), _plane(NULL), bounds(NULL) {
    _type = t;
    if (t!=IN_NODE && t!=OUT_NODE){
	stringstream ss;
        ss << "ERROR: BSPObjectNode: _type=" << t << endl;
        cerr << ss.str().c_str();
    }
}

BSPObjectNode::~BSPObjectNode(){
    if (_part) delete _part;
    if (_sameDir){
        for (auto it=_sameDir->begin(); it!=_sameDir->end(); it++){
            delete *it;
        }
        delete _sameDir;
    }
    if (_oppDir){
        for (auto it=_oppDir->begin(); it!=_oppDir->end(); it++){
            delete *it;
        }
        delete _oppDir;
    }
    if (_negativeSide) delete _negativeSide;
    if (_positiveSide) delete _positiveSide;
    if (_plane) delete _plane;
    if (bounds){
        delete bounds;
    }
}

void BSPObjectNode::printSubTree(ostream &os, int tabs){
    if (_type==CHILD_NODE){
        for (int i=0; i<tabs;i++){
            os << "\t";
        }
        os << "Node: ";
        if (bounds){
            os << "bounds: " << *bounds;
        }

        os << " CHILD: bspObjectNodeID=" << bspObjectNodeID ;
        if (_part!=NULL){
            os << " part: " << *_part << " : #faces=" << _part->allFaces->size() << endl;
        }
    } else if (_type==PARTITION_NODE){
        if (_negativeSide!=NULL){
            _negativeSide->printSubTree(os, tabs+1);
        }
        for (int i=0; i<tabs;i++){
            os << "\t";
        }
        os << "Node: ";
        if (bounds){
            os << "bounds: " << *bounds;
        }

        os << " plane=" << *_plane << " isSplitPlane=" << isSplitPlane << " bspObjectNodeID=" << bspObjectNodeID << " sameDir: ";
        if(_sameDir==NULL){
            os << "NULL ";
        } else {
            os << "size=" << _sameDir->size() << " ";
            for (auto it=_sameDir->begin(); it!= _sameDir->end(); it++){
                os << "\t" << **it;
            }
        }
        os << " _oppDir: " ;
        if (_oppDir==NULL){
            os << "NULL ";
        } else {
            os << "size=" << _oppDir->size() << " ";
            for (auto it=_oppDir->begin(); it!= _oppDir->end(); it++){
                os << "\t" << **it;
            }
        }
        os << endl;
        if (_positiveSide!=NULL){
            _positiveSide->printSubTree(os, tabs+1);
        }
    } else {
        string typestr = (_type==IN_NODE) ? "IN_NODE" : "OUT_NODE";
        for (int i=0; i<tabs;i++){
            os << "\t";
        }
        if (_part==NULL){
            os << "BSPObjectNode: type=" << typestr << "  _part=null bspObjectNodeID=" << bspObjectNodeID << endl;
        } else {
            os << "BSPObjectNode: type=" << typestr << ": ";
            if (bounds){
                os << "bounds: " << *bounds;
            }
            os << " _part.faces.length=" << _part->allFaces->size() << " bspObjectNodeID=" << bspObjectNodeID << " part Object ID=" << _part->bspObject->_id << endl;
        }
    }
}

void callDMFunctionForPart(string &func_name, BSPObjectPart *part, bool plusPartNumber){
    int nargs = 4;
    if (plusPartNumber) nargs++;
    /* TODO: NEED TO ADD CALLBACK
    DataTypeInstanceStack args(nargs);
    KBFaces3D *faces3D;
    args.slotSet(0, part->bspObject->_id);
    int argnum = 1;
    if (plusPartNumber){
        args.slotSet(argnum++, new KBInteger(part->getNumber()));
    }
    args.slotSet(argnum++, faces3D = new KBFaces3D(part->allFaces));
    args.slotSet(argnum++, new KBCuboidD(part->bbx));
    string funcname(func_name);
    KBObject *ret = staticDataManagerInstance->createInstanceFromTableMapping(func_name, &args, 0);
    faces3D->faces = NULL; // NEED TO CLEAR FACES SINCE WE ARE NOT COPYING THEM, PART OWNS THEM AND THEY SHOULD NOT CHANGE
    CHECK_REF(ret);
    */
}
void callDMFunctionForPartList(string &func_name, vector<BSPObjectPart*> *partlist, bool plusPartNumber){
    for (auto it = partlist->begin(); it!=partlist->end(); it++){
        callDMFunctionForPart(func_name, *it, plusPartNumber);
    }
}

void BSPObjectNode::traverseFrontToBackFromPoint(string &func_name, Point3d &pos, Point3d &dir, bool cull, bool plusPartNumber, bool debug){
    int whichSide = ZERO;
    if (_plane){
        whichSide = _plane->whichSideIsPoint(pos);
    }
    if (debug){
        stringstream ss;
        ss << "traverse: Node#" << bspObjectNodeID << " whichSide=" << whichSide ;
        ss << " pos=" << pos << " dir=" << dir;
        if (_plane)
            ss << " _plane=" << *_plane ;
        ss << endl;
	cerr << ss.str().c_str();
    }
    if (whichSide == NEGATIVE){
        if (_negativeSide) _negativeSide->traverseFrontToBackFromPoint(func_name, pos, dir, cull, plusPartNumber, debug);
        if (!cull && _sameDir)
            callDMFunctionForPartList(func_name, _sameDir, plusPartNumber);
        if (_oppDir)
            callDMFunctionForPartList(func_name, _oppDir, plusPartNumber);
        if (_positiveSide) _positiveSide->traverseFrontToBackFromPoint(func_name, pos, dir, cull, plusPartNumber, debug);
    } else if (whichSide == POSITIVE){
        if (_positiveSide) _positiveSide->traverseFrontToBackFromPoint(func_name, pos, dir, cull, plusPartNumber, debug);
        if (!cull && _oppDir)
            callDMFunctionForPartList(func_name, _oppDir, plusPartNumber);
        if (_sameDir)
            callDMFunctionForPartList(func_name, _sameDir, plusPartNumber);
        if (_negativeSide) _negativeSide->traverseFrontToBackFromPoint(func_name, pos, dir, cull, plusPartNumber, debug);
    } else {
        if (debug && !_plane){
            stringstream ss;
            ss << "\t" ;
            if (_sameDir)
                ss << "_sameDir.len=" << _sameDir->size() << " ";
            if (_oppDir)
                ss << "_oppDir.len=" << _oppDir->size() << " ";
            if (_sameDir || _oppDir){
                ss << endl;
                cerr << ss.str().c_str();
            }
        }
        // doesn't matter which order, but don't need to look at same/opp objects on plane
        if (_positiveSide) _positiveSide->traverseFrontToBackFromPoint(func_name, pos, dir, cull, plusPartNumber, debug);
        if (_part)
            callDMFunctionForPart(func_name, _part, plusPartNumber);
        if (_negativeSide) _negativeSide->traverseFrontToBackFromPoint(func_name, pos, dir, cull, plusPartNumber, debug);
    }
}
int BSPObjectNode::removeID(int idval){
    int removedNum = 0;
    if (_part && _part->bspObject){
        if (_part->bspObject->_id == idval){
            delete _part;
            _part = NULL;
            removedNum++;
        }
    }
    if (_sameDir){
        for (auto it = _sameDir->begin(); it!=_sameDir->end(); ){
            if ((*it)->bspObject && (*it)->bspObject->_id == idval){
                auto part = *it;
                it = _sameDir->erase(it);
                delete part;
                removedNum++;
            } else {
                it++;
            }
        }
    }
    if (_oppDir){
        for (auto it = _oppDir->begin(); it!=_oppDir->end(); ){
            if ((*it)->bspObject && (*it)->bspObject->_id == idval){
                auto part = *it;
                it = _oppDir->erase(it);
                delete part;
                removedNum++;
            } else {
                it++;
            }
        }
    }
    if (_negativeSide){
        removedNum += _negativeSide->removeID(idval);
        if (!_negativeSide->hasParts()){
            delete _negativeSide;
            _negativeSide = NULL;
        }
    }
    if (_positiveSide){
        removedNum += _positiveSide->removeID(idval);
        if (!_positiveSide->hasParts()){
            delete _positiveSide;
            _positiveSide = NULL;
        }
    }
    
    return removedNum;
}
bool BSPObjectNode::hasParts(){
    if (_part && _part->bspObject) return true;
    if (_negativeSide && _negativeSide->hasParts()) return true;
    if (_positiveSide && _positiveSide->hasParts()) return true;
    if (_sameDir && !_sameDir->empty()) return true;
    if (_oppDir && !_oppDir->empty()) return true;
    return false;
}
int BSPObjectNode::getNumberOfTotalParts(){
    int nparts = 0;
    if (_part) nparts++;
    if (_negativeSide) nparts += _negativeSide->getNumberOfTotalParts();
    if (_positiveSide) nparts += _positiveSide->getNumberOfTotalParts();
    if (_sameDir) nparts += _sameDir->size();
    if (_oppDir) nparts += _oppDir->size();
    return nparts;
}
void BSPObjectNode::addBSPObjectPart(BSPObjectPart *part){
    addToBounds(part->bbx);
    if (_plane){
        int side;
        bool isOnOneSide = _plane->isOnOneSide(part->bbx, side);
        if (isOnOneSide){
            if (side>0){
                if (_positiveSide){
                    _positiveSide->addBSPObjectPart(part);
                } else {
                    _positiveSide = new BSPObjectNode(part);
                }
            } else if (side<0){
                if (_negativeSide){
                    _negativeSide->addBSPObjectPart(part);
                } else {
                    _negativeSide = new BSPObjectNode(part);
                }
            } else {
	      cerr << "WARNING: BSPObjectPart::addBSPObjectPart isOnOneSide=" << IntToString(isOnOneSide) << " but side=" << IntToString(side) << endl;
            }
        } else {
            // plane splits the part, need to split
            stringstream ss;
            bool debug = false;
            if (debug){
                ss << "SPLITTING: before split: _plane=" << *_plane << " part=" << *part << endl;
            }
            vector<BSPObjectPart*> *objectsNeg = new vector<BSPObjectPart*>(),
                                   *objectsPos = new vector<BSPObjectPart*>();
            part->split(_plane, objectsNeg, objectsPos, _sameDir, _oppDir);
            
            if (!objectsPos->empty()){
                BSPObjectPart *posPart = *(objectsPos->begin());
                if (debug){
                    ss << "     after split: posPart=" << *posPart << endl;
                }
                if (_positiveSide){
                    _positiveSide->addBSPObjectPart(posPart);
                } else {
                    _positiveSide = new BSPObjectNode(posPart);  // should only be one in objectsPos
                }
                if (debug){
                    ss << "posPart->bbx=" << posPart->bbx << endl;
                    if (_positiveSide->bounds){
                        ss << " pos bounds: " << *_positiveSide->bounds << endl;
                    } else {
                        ss << " pos bounds: NULL" << endl;
                    }
                }
                if (objectsPos->size() > 1)
		  cerr << "WARNING: BSPObjectNode::addBSPObjectPart: objectsPos size=" << IntToString((int)objectsPos->size()) << "should only have 1 object" << endl;
            }
            if (!objectsNeg->empty()){
                BSPObjectPart *negPart = *(objectsNeg->begin());
                if (debug){
                    ss << "     after split: negPart=" << *negPart << endl;
                }
                if (_negativeSide){
                    _negativeSide->addBSPObjectPart(negPart);
                } else {
                    _negativeSide = new BSPObjectNode(negPart);  // should only be one in objectsNeg
                }
                if (debug){
                    ss << "negPart->bbx=" << negPart->bbx << endl;
                    if (_negativeSide->bounds){
                        ss << " neg bounds: " << *_negativeSide->bounds << endl;
                    } else {
                        ss << " neg bounds: NULL" << endl;
                    }
                }
                if (objectsNeg->size() > 1)
		  cerr << "WARNING: BSPObjectNode::addBSPObjectPart: objectsNeg size=" << IntToString((int)objectsNeg->size()) << "should only have 1 object" << endl;
            }
            delete objectsPos;
            delete objectsNeg;
            BSPObject *bobj = part->bspObject;
            if (debug){
                ss << "addBSPObjectPart: id=" << IntToString(bobj->_id) << " SPLITTING #parts=" << bobj->allParts.size() << endl;
                for (auto pit = bobj->allParts.begin(); pit != bobj->allParts.end(); pit++){
                    auto p = (*pit);
                    ss << "\tPart: #faces="  << p->allFaces->size() << endl;
                    for (auto faceit = p->allFaces->begin(); faceit != p->allFaces->end(); faceit++){
                        auto face = (*faceit);
                        ss << "\t\t" << face->toString() << endl;
                    }
                    ss << endl;
                }
                cerr << ss.str().c_str();
            }
            delete part;  // BSPObjectPart->split() should always require deleting the part after
        }
    } else {
        if (_part){
            // no plane, we should generate a new BSPObjectTree subtree inside this node from the new part and existing part
            bool debug = false;
            vector<BSPObjectPart*> allParts;
            allParts.push_back(part);
            allParts.push_back(_part);
            stringstream ss;
            if (debug){
                ss << "Creating subtree for 2 parts: part=" << *part << " _part=" << *_part << endl;
                cerr << ss.str().c_str();
            }
            BSPObjectNode *newNode = generateBSPObjectSubTree(allParts, debug);
            if (newNode){
                if (bounds){ delete bounds; bounds = NULL; }
                isSplitPlane = newNode->isSplitPlane;
                _type = newNode->_type;
                _plane = newNode->_plane;
                _sameDir = newNode->_sameDir;
                _oppDir = newNode->_oppDir;
                bounds = newNode->bounds;
                newNode->bounds = NULL;
                _negativeSide = newNode->_negativeSide;
                _positiveSide = newNode->_positiveSide;
                _part = newNode->_part;
                delete newNode;
            } else {
	      cerr << "WARNING: BSPObjectNode::addBSPObjectPart: generateBSPObjectSubTree returned NULL, should never happen" << endl;
              delete part;
            }
        } else {
            // only this part, just change to child node
            _type = CHILD_NODE;
            _part = part;
        }
    }
}
ostream& operator <<(ostream &os,BSPObjectNode &obj){
    obj.printSubTree(os);
    return os;
}
string BSPObjectNode::toString(){
    stringstream ss;
    ss << *this;
    return ss.str();
}
