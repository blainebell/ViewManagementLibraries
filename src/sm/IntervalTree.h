#ifndef CPP_INTERVALTREE_H
#define CPP_INTERVALTREE_H

#include "Rectangle2.h"
#include "Rectangle3.h"
#include "DataListInterface.h"
#include "stlTools.h"

template <typename numericalValueType, typename nodeClass> class IntervalDimension {
public:
    numericalValueType (*minMethod)(const nodeClass&);
    numericalValueType (*maxMethod)(const nodeClass&);
    int intervalID;
    IntervalDimension(int iD, numericalValueType (*min)(const nodeClass&), numericalValueType (*max)(const nodeClass&)){
        intervalID = iD;
        minMethod = min;
        maxMethod = max;
    }
    friend ostream& operator <<(ostream &os, const IntervalDimension &obj){
        os << " intervalID=" << obj.intervalID;
        return os;
    }
};

template <typename numericalValueType, typename nodeClass> class NeededCondition {
public:
    IntervalDimension<numericalValueType,nodeClass> *dimension;
    numericalValueType value;
    unsigned char direction; // 0=less-than 1=greater-than 2=all
    bool clusive;            // true=inclusive false=exclusive
    NeededCondition(IntervalDimension<numericalValueType,nodeClass> *dim, int dir, bool cl, numericalValueType val){
        dimension = dim;
        direction = dir;
        clusive = cl;
        value = val;
    }
    friend ostream& operator <<(ostream &os, const NeededCondition &obj){
        os << " dimension=" << *obj.dimension << " direction=" << (int)obj.direction << " clusive=" << obj.clusive << " value=" << obj.value;
        return os;
    }
};

template <typename numericalValueType, typename nodeClass> class NeededCriteria {
public:
    vector< NeededCondition<numericalValueType,nodeClass> > neededCriteria;
    void pushCriteria(IntervalDimension<numericalValueType,nodeClass> *iD, unsigned char dir, bool cl, numericalValueType value){
        neededCriteria.push_back( { iD, dir, cl, value } );
    }
    void popCriteria(){
        neededCriteria.pop_back();
    }
    bool empty(){
        return neededCriteria.empty();
    }
};

template <typename numericalValueType, typename nodeClass> class SecondaryStructure {
public:
    virtual bool add(nodeClass *ob) = 0;
    virtual bool isEmpty() = 0;
    virtual vector<nodeClass> *enclosedBy(const nodeClass &r, NeededCriteria<numericalValueType, nodeClass> &nC, bool justGetOne, vector<nodeClass> *resultList) = 0;
    virtual vector<nodeClass> *windowQuery(const nodeClass &rs, vector<nodeClass> *resultList, bool justGetOne){
        NeededCriteria<numericalValueType, nodeClass> nc;
        return windowQuery(rs,nc, true, resultList, justGetOne);
    }
    virtual vector<nodeClass> *windowQuery(const nodeClass &rs, NeededCriteria<numericalValueType, nodeClass> &neededCriteria, bool inclusive, vector<nodeClass> *resultList, bool justGetOne) = 0;
    virtual bool remove(nodeClass &r) = 0;
    virtual vector<nodeClass> *getAll(vector<nodeClass> *) = 0;
    virtual void print(ostream &os) = 0;
    virtual ~SecondaryStructure(){};
};

template <typename numericalValueType, typename nodeClass> class IntervalTreeDefaults {
public:
    IntervalTreeDefaults<numericalValueType, nodeClass>() {}
    static IntervalDimension<numericalValueType, nodeClass> default_interval_dimension;
    static IntervalDimension<numericalValueType, nodeClass> secondary_interval_dimension;
    static IntervalDimension<numericalValueType, nodeClass> tertiary_interval_dimension;
};

template <typename numericalValueType, typename nodeClass, typename internalNodeClass> class IntervalTreeSubclass : public IntervalTreeDefaults<numericalValueType, nodeClass> {
public:
    IntervalDimension<numericalValueType,nodeClass> *intervalDimension;
    virtual internalNodeClass *createInternalNodeInstance() = 0;
    IntervalTreeSubclass(IntervalDimension<numericalValueType,nodeClass> *iD) : IntervalTreeDefaults<numericalValueType, nodeClass>(), intervalDimension(iD){
        if (!iD)
            intervalDimension = &IntervalTreeDefaults<numericalValueType, nodeClass>::default_interval_dimension;
    }
    ~IntervalTreeSubclass(){
    }
};

template <typename numericalValueType, typename nodeClass, typename internalNodeClass> class IntervalTreeNode {
public:
    bool active;
    double value;
    IntervalTreeNode<numericalValueType, nodeClass, internalNodeClass> *primaryStructure[2];
    SecondaryStructure<numericalValueType, nodeClass> *secondaryStructure;
    IntervalTreeNode<numericalValueType, nodeClass, internalNodeClass> *tertiaryStructure[2];
    IntervalTreeSubclass<numericalValueType, nodeClass, internalNodeClass> *owner;
    void setActive(bool t){
        active = t;
        if (!active){
            if (tertiaryStructure[0]){ /*delete tertiaryStructure[0];*/ tertiaryStructure[0] = NULL; }
            if (tertiaryStructure[1]){ /*delete tertiaryStructure[1];*/ tertiaryStructure[1] = NULL; }
        }
    }
    void add (nodeClass *r){
        secondaryStructure->add(r);
        active = true;
    }
    IntervalTreeNode(double val,IntervalTreeSubclass<numericalValueType, nodeClass, internalNodeClass> *it) : value(val), owner(it), active(false) {
        primaryStructure[0] = primaryStructure[1] = NULL;
        tertiaryStructure[0] = tertiaryStructure[1] = NULL;
        secondaryStructure = dynamic_cast<SecondaryStructure<numericalValueType, nodeClass> *>(owner->createInternalNodeInstance());
    }
    ~IntervalTreeNode(){
        if (secondaryStructure){ delete secondaryStructure; secondaryStructure = NULL; }
        if (primaryStructure[0]){ delete primaryStructure[0]; primaryStructure[0] = NULL; }
        if (primaryStructure[1]){ delete primaryStructure[1]; primaryStructure[1] = NULL; }
    }
    void printSubTree(ostream &os, int depth = 0) {
        if (primaryStructure[0])
            primaryStructure[0]->printSubTree(os,depth+1);
        for (int i=0; i<depth; i++){
            os << "\t";
        }
        printNode(os);
        
        if (primaryStructure[1])
            primaryStructure[1]->printSubTree(os,depth+1);
    }
    void printNode(ostream &os){
        os << value;
        
        if (active){
            os << " *";
            if (tertiaryStructure[0]){
                os << " 0: " << tertiaryStructure[0]->value;
            }
            if (tertiaryStructure[1]){
                os << " 1: " << tertiaryStructure[1]->value;
            }
            os << " - \n";
            os << "secondaryStructure intervalDimension=" << *(owner->intervalDimension) << "\tvalue=" << value << ":" << endl;
            secondaryStructure->print(os);
        }
        os << endl;
    }
};

template <typename numericalValueType, typename nodeClass, typename internalNodeClass > class IntervalTree : public IntervalTreeSubclass<numericalValueType, nodeClass, internalNodeClass>, public DataListInterface<nodeClass> {
public:
    int numRectangles;
    IntervalTreeNode<numericalValueType, nodeClass, internalNodeClass> *rootNode;
    IntervalTree(IntervalDimension<numericalValueType, nodeClass> *iD = NULL) : IntervalTreeSubclass<numericalValueType, nodeClass, internalNodeClass>(iD), DataListInterface<nodeClass>(), rootNode(NULL), numRectangles(0) {
    };
    //for vector<nodeClass>
    virtual void push_back(const nodeClass &nc){  // to work with vector
        nodeClass newI(nc);
        add(&newI);
    }
    virtual void addData(const nodeClass &nc){
        add(nc);
    }
    virtual void addDataAt(nodeClass &nc, int at){
        add(&nc);
    }
    virtual void deleteData(nodeClass &nc){
        remove(&nc);
    }
    virtual void clearAllData(){
        clear();
    }
    ~IntervalTree(){
        clear();
    }
    void clear(){
        if (rootNode){
            delete rootNode;
            rootNode = NULL;
        }
    }
    int size(){
        vector<nodeClass> *all = getAll();
        int s = (int)all->size();
        delete all;
        return s;
    }
    bool isEmpty() {
        return (rootNode==NULL || (rootNode->secondaryStructure->isEmpty() &&
                                                rootNode->tertiaryStructure[0] == NULL &&
                                                rootNode->tertiaryStructure[1] == NULL));
    }
    vector<nodeClass> *getAll(vector<nodeClass> *allList = NULL){
        if (!allList) allList = new vector<nodeClass>();
        
        auto currentNode = rootNode;
        stack<IntervalTreeNode<numericalValueType, nodeClass, internalNodeClass> *> searchStack;
        if (currentNode)
            searchStack.push(currentNode);
        while (!searchStack.empty()){
            currentNode = searchStack.top(); searchStack.pop();
            currentNode->secondaryStructure->getAll(allList);
            if (currentNode->tertiaryStructure[0]) searchStack.push(currentNode->tertiaryStructure[0]);
            if (currentNode->tertiaryStructure[1]) searchStack.push(currentNode->tertiaryStructure[1]);
        }
        return allList;
    }
    virtual bool isEnclosedBy(const nodeClass &rs){
      NeededCriteria<numericalValueType, nodeClass> nc;
      vector<nodeClass> resV;
      enclosedBy(rs, nc, true, &resV);
      return !resV.empty();
    }
    virtual vector<nodeClass> *enclosedBy(const nodeClass &rs){
      NeededCriteria<numericalValueType, nodeClass> nc;
      return enclosedBy(rs, nc, false, NULL);
    }
    virtual vector<nodeClass> *enclosedBy(const nodeClass &rs, NeededCriteria<numericalValueType, nodeClass> &neededCriteria, bool justGetOne = false, vector<nodeClass> *resultList = NULL){
        if (!resultList)
            resultList = new vector<nodeClass>();
        numericalValueType rmin = IntervalTreeSubclass<numericalValueType, nodeClass, internalNodeClass>::intervalDimension->minMethod(rs),
        rmax = IntervalTreeSubclass<numericalValueType, nodeClass, internalNodeClass>::intervalDimension->maxMethod(rs);
        
        bool mincomp,maxcomp;
        auto *currentNode = rootNode;
        stack<IntervalTreeNode<numericalValueType, nodeClass, internalNodeClass> *> searchStack;
        if (currentNode)
            searchStack.push(currentNode);
        while (!searchStack.empty()){
            currentNode = searchStack.top(); searchStack.pop();
            if (currentNode->secondaryStructure->isEmpty()){
                continue;
            }
            mincomp = currentNode->value >= rmin;
            maxcomp = currentNode->value <= rmax;
            if (mincomp && maxcomp){
                neededCriteria.pushCriteria(IntervalTreeSubclass<numericalValueType, nodeClass, internalNodeClass>::intervalDimension, 0 /* less-than */, true, rmin);
                neededCriteria.pushCriteria(IntervalTreeSubclass<numericalValueType, nodeClass, internalNodeClass>::intervalDimension, 1 /* greater-than */, true, rmax);
                currentNode->secondaryStructure->enclosedBy(rs, neededCriteria, justGetOne, resultList);
                neededCriteria.popCriteria();
                neededCriteria.popCriteria();
                if (justGetOne && !resultList->empty())
                    return resultList;
            } else if (mincomp){
                neededCriteria.pushCriteria(IntervalTreeSubclass<numericalValueType, nodeClass, internalNodeClass>::intervalDimension, 0 /* less-than */, true, rmin);
                currentNode->secondaryStructure->enclosedBy(rs, neededCriteria, justGetOne, resultList);
                neededCriteria.popCriteria();
                if (justGetOne && !resultList->empty())
                    return resultList;
                if (currentNode->tertiaryStructure[0])
                    searchStack.push(currentNode->tertiaryStructure[0]);
            } else if (maxcomp){
                neededCriteria.pushCriteria(IntervalTreeSubclass<numericalValueType, nodeClass, internalNodeClass>::intervalDimension, 1 /* greater-than */, true, rmax);
                currentNode->secondaryStructure->enclosedBy(rs, neededCriteria, justGetOne, resultList);
                neededCriteria.popCriteria();
                if (justGetOne && !resultList->empty())
                    return resultList;
                if (currentNode->tertiaryStructure[1])
                    searchStack.push(currentNode->tertiaryStructure[1]);
            }
            
        }
        return resultList;
    }
    virtual vector<nodeClass> *windowQuery(const nodeClass &rs, vector<nodeClass> *resultList = NULL){
        NeededCriteria<numericalValueType, nodeClass> nc;
        return windowQuery(rs,nc, true, resultList);
    }
    virtual vector<nodeClass> *windowQueryWithoutEdges(const nodeClass &rs){
        NeededCriteria<numericalValueType, nodeClass> nc;
        return windowQuery(rs, nc, false);
    }
    virtual bool isOverlapped(const nodeClass &rs){
        NeededCriteria<numericalValueType, nodeClass> nc;
        vector<nodeClass> resL;
        windowQuery(rs, nc, false, &resL, true);
        return !resL.empty();
    }
    virtual bool isOverlappedOrAdjacent(const nodeClass &rs){
        NeededCriteria<numericalValueType, nodeClass> nc;
        vector<nodeClass> resL;
        windowQuery(rs, nc, true, &resL, true);
        return !resL.empty();
    }
    vector<nodeClass> *windowQuery(const nodeClass &rs, NeededCriteria<numericalValueType, nodeClass> &neededCriteria, bool inclusive = true, vector<nodeClass> *resultList = NULL, bool justGetOne = false){
        if (!resultList)
            resultList = new vector<nodeClass>();

        numericalValueType rmin = IntervalTreeSubclass<numericalValueType, nodeClass, internalNodeClass>::intervalDimension->minMethod(rs),
        rmax = IntervalTreeSubclass<numericalValueType, nodeClass, internalNodeClass>::intervalDimension->maxMethod(rs);
        
        auto *currentNode = rootNode;
        
        stack<IntervalTreeNode<numericalValueType, nodeClass, internalNodeClass> *> searchStack;

        if (currentNode)
            searchStack.push(currentNode);
        
        while (!searchStack.empty()){
            currentNode = searchStack.top(); searchStack.pop();
            if (rmax <= currentNode->value){
                if (!currentNode->secondaryStructure->isEmpty()){
                    neededCriteria.pushCriteria(IntervalTreeSubclass<numericalValueType, nodeClass, internalNodeClass>::intervalDimension, 0 /* less-than */,inclusive, rmax);
                    currentNode->secondaryStructure->windowQuery(rs, neededCriteria, inclusive, resultList, justGetOne);
                    neededCriteria.popCriteria();
                    if (justGetOne && !resultList->empty()){
                        return resultList;
                    }
                }
                if (currentNode->tertiaryStructure[0]) searchStack.push(currentNode->tertiaryStructure[0]);
            } else if (rmin >= currentNode->value){
                if (!currentNode->secondaryStructure->isEmpty()){
                    neededCriteria.pushCriteria(IntervalTreeSubclass<numericalValueType, nodeClass, internalNodeClass>::intervalDimension, 1 /* less-than */,inclusive, rmin);
                    currentNode->secondaryStructure->windowQuery(rs, neededCriteria, inclusive, resultList, justGetOne);
                    neededCriteria.popCriteria();
                    if (justGetOne && !resultList->empty()){
                        return resultList;
                    }
                }
                if (currentNode->tertiaryStructure[1]) searchStack.push(currentNode->tertiaryStructure[1]);
            } else {
                if (!currentNode->secondaryStructure->isEmpty()){
                    currentNode->secondaryStructure->windowQuery(rs, neededCriteria, inclusive, resultList, justGetOne);
                    if (justGetOne && !resultList->empty()){
                        return resultList;
                    }
                }
                if (currentNode->tertiaryStructure[0]) searchStack.push(currentNode->tertiaryStructure[0]);
                if (currentNode->tertiaryStructure[1]) searchStack.push(currentNode->tertiaryStructure[1]);
            }
        }

        return resultList;
    }
    friend ostream& operator <<(ostream &os, const IntervalTree &obj){
        os << "Printing IntervalTree" << endl;
        os << "~~~~~~~~~~~~~~~~~~~~~" << endl;
        if (obj.rootNode)
            obj.rootNode->printSubTree(os,0);
        return os;
    }
    void add(nodeClass rs) {
        add(&rs);
    }
    void add(nodeClass &rs) {
        add(&rs);
    }
    void add(nodeClass *rs) {
        IntervalTreeNode<numericalValueType, nodeClass, internalNodeClass> *currentNode = rootNode, *activeNode = NULL, *nonActiveNode = NULL;
        int whichWayTertiary=0, whichWayNoneActive=0;
        numericalValueType rmin = IntervalTreeSubclass<numericalValueType, nodeClass, internalNodeClass>::intervalDimension->minMethod(*rs),
                           rmax = IntervalTreeSubclass<numericalValueType, nodeClass, internalNodeClass>::intervalDimension->maxMethod(*rs);
        double rcenter = (rmin+rmax)/ 2.;
        if (rootNode == NULL){
            rootNode = new IntervalTreeNode<numericalValueType, nodeClass, internalNodeClass>(rcenter,this);
            currentNode = rootNode;
        }
        bool contentWithNonActiveNode = false;
        numRectangles++;
        while ( currentNode != NULL){
            if (rmax < currentNode->value){
                if (currentNode->primaryStructure[0]==NULL){
                    currentNode->primaryStructure[0] =  new IntervalTreeNode<numericalValueType, nodeClass, internalNodeClass>(rcenter,this);
                }
                if (currentNode->active){
                    activeNode = currentNode;
                    whichWayTertiary = 0;
                    nonActiveNode = NULL;
                } else {
                    IntervalTreeNode<numericalValueType, nodeClass, internalNodeClass> *adsf = activeNode->tertiaryStructure[whichWayTertiary];
                    if (nonActiveNode==NULL){
                        nonActiveNode = currentNode;
                        whichWayNoneActive = 0;
                        contentWithNonActiveNode=false;
                    }
                    if (!contentWithNonActiveNode && adsf!=NULL && (currentNode->value < adsf->value)){
                        contentWithNonActiveNode = true;
                        nonActiveNode = currentNode;
                        whichWayNoneActive = 0;
                    }
                }
                currentNode = currentNode->primaryStructure[0];
            } else if (rmin > currentNode->value){
                if (currentNode->primaryStructure[1]==NULL){
                    currentNode->primaryStructure[1] =  new IntervalTreeNode<numericalValueType, nodeClass, internalNodeClass>(rcenter,this);
                }
                if (currentNode->active){
                    activeNode = currentNode;
                    whichWayTertiary = 1;
                    nonActiveNode = NULL;
                } else {
                    IntervalTreeNode<numericalValueType, nodeClass, internalNodeClass> *adsf = activeNode->tertiaryStructure[whichWayTertiary];
                    if (nonActiveNode==NULL){
                        nonActiveNode = currentNode;
                        whichWayNoneActive = 1;
                        contentWithNonActiveNode=false;
                    }
                    if (!contentWithNonActiveNode && adsf!=NULL && (currentNode->value > adsf->value)){
                        contentWithNonActiveNode = true;
                        nonActiveNode = currentNode;
                        whichWayNoneActive = 1;
                    }
                }
                currentNode = currentNode->primaryStructure[1];
            } else {  // Inserting rectangle to a node
                if (!currentNode->active){
                    IntervalTreeNode<numericalValueType, nodeClass, internalNodeClass> *activeNodePtr = NULL;
                    if (activeNode!=NULL){
                        activeNodePtr = activeNode->tertiaryStructure[whichWayTertiary];
                    }
                    if (activeNode==rootNode && activeNodePtr==NULL){
                        activeNode->tertiaryStructure[whichWayTertiary] = currentNode;
                    } else if (nonActiveNode!=NULL && activeNodePtr!=NULL && ((activeNodePtr->value > nonActiveNode->value) != (currentNode->value > nonActiveNode->value))){ // we know a node has to be active b/c the root is always active
                        nonActiveNode->setActive(true);
                        nonActiveNode->tertiaryStructure[(whichWayNoneActive+1)%2] = activeNode->tertiaryStructure[whichWayTertiary];
                        activeNode->tertiaryStructure[whichWayTertiary] = nonActiveNode;
                        nonActiveNode->tertiaryStructure[whichWayNoneActive] = currentNode;
                    } else if (activeNode!=NULL){
                        if (activeNode->tertiaryStructure[whichWayTertiary]!=NULL){
                            int asdf = activeNode->tertiaryStructure[whichWayTertiary]->value > currentNode->value ? 1 : 0;
                            currentNode->tertiaryStructure[asdf] = activeNode->tertiaryStructure[whichWayTertiary];
                        }
                        activeNode->tertiaryStructure[whichWayTertiary] = currentNode;
                    }
                } 
                currentNode->add(rs);
                return;
            }
        }
    }
    bool remove(nodeClass rs) {
        remove(&rs);
    }
    bool remove(nodeClass &rs) {
        remove(&rs);
    }
    bool remove(nodeClass *rs) {
        numericalValueType rmin = IntervalTreeSubclass<numericalValueType, nodeClass, internalNodeClass>::intervalDimension->minMethod(*rs),
                           rmax = IntervalTreeSubclass<numericalValueType, nodeClass, internalNodeClass>::intervalDimension->maxMethod(*rs);

        auto *itn = rootNode;
        IntervalTreeNode<numericalValueType, nodeClass, internalNodeClass> *parent=NULL, *grandparent = NULL;

        bool deleted = false;
        int whichWay=0,whichWayGrand=0;
        while (itn!=NULL && !deleted){
            if (rmax < itn->value){
                grandparent = parent;
                parent = itn;
                itn = itn->tertiaryStructure[0];
                whichWayGrand=whichWay;
                whichWay=0;
            } else if (rmin > itn->value){
                grandparent = parent;
                parent = itn;
                itn = itn->tertiaryStructure[1];
                whichWayGrand=whichWay;
                whichWay=1;
            } else {
                deleted = itn->secondaryStructure->remove(*rs);
                if (!deleted){
                    return (false);
                }
            }
        }
        if (deleted){
            numRectangles--;
            if (itn->secondaryStructure->isEmpty() && itn != rootNode){
                bool ts0 = itn->tertiaryStructure[0]==NULL;
                bool ts1 = itn->tertiaryStructure[1]==NULL;
                
                // When the tertiary node is a leaf, make it inactive and check parent to make sure
                // it totally doesn't depend on this node (if it is active with no rectangles)
                if (ts0 && ts1){
                    itn->setActive(false);
                    parent->tertiaryStructure[whichWay] = NULL;
                    
                    auto *itn2 = parent->tertiaryStructure[(whichWay +1)%2];
                    itn = parent;
                    if (parent->secondaryStructure->isEmpty() && parent!=rootNode){ // && itn2!=null){
                        parent->setActive(false);
                        grandparent->tertiaryStructure[whichWayGrand] = itn2;
                    }
                    // if node that is becoming inactive has one tertiary child,
                    // set the parent 
                } else if (ts0 ^ ts1){ 
                    if (ts0){
                        parent->tertiaryStructure[whichWay] = itn->tertiaryStructure[1];
                        itn->setActive(false);
                    } else {
                        parent->tertiaryStructure[whichWay] = itn->tertiaryStructure[0];
                        itn->setActive(false);
                    }
                } 
            }
        } else {
        }
        return deleted;
    }
    virtual internalNodeClass *createInternalNodeInstance(){
        return new internalNodeClass();
    }
    
    virtual bool getSpacesInsideAndAddFullRectangleInEmptySpace(const nodeClass rs,
                                                                DataListInterface<nodeClass> *listController,
                                                                DataListInterface<nodeClass> *insideController,
                                                                IntervalTree<numericalValueType, nodeClass, internalNodeClass> *insideIntervalTree, numericalValueType minDimension) = 0;
    virtual bool addOnePiece(const nodeClass rs, DataListInterface<nodeClass> *listController, numericalValueType minDim = 0) = 0;
};

// USE_INTERVAL_TREE : uses an IntervalTree to support exhaustive isEnclosedBy() search towards the end of addOnePiece()
#define USE_INTERVAL_TREE

template <typename numericalValueType, typename nodeClass, typename internalNodeClass > class IntervalTree2 : public IntervalTree<numericalValueType, nodeClass, internalNodeClass> {
public:
    IntervalTree2<numericalValueType, nodeClass, internalNodeClass>(IntervalDimension<numericalValueType, nodeClass> *iD=NULL) : IntervalTree<numericalValueType, nodeClass, internalNodeClass>(iD) {}
    virtual bool addOnePiece(const nodeClass rs, DataListInterface<nodeClass> *listController, numericalValueType minDim = 0){
#ifdef USE_INTERVAL_TREE
        IntervalTree2<numericalValueType, nodeClass, internalNodeClass> addingAll;
#else
        vector<nodeClass> addingAll;  // this could be replaced by an IntervalTree
#endif
        addingAll.push_back(rs);
        list< pair<int,nodeClass> > allRectsToQuery;
        allRectsToQuery.push_back(pair<int,nodeClass>(7,rs));
        while (!allRectsToQuery.empty()){
            auto rsp = allRectsToQuery.front();
            auto checkBits = rsp.first;
            auto rsq = rsp.second;
            allRectsToQuery.pop_front();
            auto rectsIn = this->windowQuery(rsq);
            for (auto it=rectsIn->begin(); it!=rectsIn->end(); it++){
                auto rect = (*it);
                if (rect.enclosedBy(rsq)){
                    listController->deleteData(rect);
                    continue;
                }
                if ( (checkBits & 1) && isAdjacentX(rect,rsq)){
                    auto tmprect = consensus(rsq,rect,0);
                    addingAll.push_back(tmprect);
                    if (!(checkBits & 16))
                        allRectsToQuery.push_back(pair<int,nodeClass>(2 | 8, tmprect));
                } else if ( (checkBits & 2) && isAdjacentY(rect,rsq)){
                    auto tmprect = consensus(rsq,rect,1);
                    addingAll.push_back(tmprect);
                    if (!(checkBits & 8))
                        allRectsToQuery.push_back(pair<int,nodeClass>(1 | 16, tmprect));
                } else if ( (checkBits & 4) && has_intersection(rect,rsq) && !rect.enclosedBy(rsq)){
                    auto tmprect = consensus(rsq,rect,0);
                    if (!tmprect.enclosedBy(rsq) && !tmprect.enclosedBy(rect)){
                        addingAll.push_back(tmprect);
                    }
                    tmprect=consensus(rsq,rect,1);
                    if (!tmprect.enclosedBy(rsq) && !tmprect.enclosedBy(rect)){
                        addingAll.push_back(tmprect);
                    }
                }
            }
            delete rectsIn; // no longer needed
        }
        vector<nodeClass> addingAll2;

#ifdef USE_INTERVAL_TREE
        vector<nodeClass> *addingAllTmp = addingAll.getAll();
        for (auto it3=addingAllTmp->begin();it3!=addingAllTmp->end();it3++){
            auto rect = (*it3);
            addingAll.remove(&rect);
            if (!addingAll.isEnclosedBy(rect)) {
                addingAll2.push_back(rect);
                addingAll.add(&rect); // only need to re-add it if it isn't enclosed by any other
            }
        }
        delete addingAllTmp;
#else
        // add all rectangles into addingAll2 that are not enclosedBy others in addingAll
        // this could probably be enhanced by using an IntervalTree for addingAll, to speedup
        // isEnclosedBy(), instead of exhaustively going through all in addingAll
        for (auto it3=addingAll.begin();it3!=addingAll.end();it3++){
            auto tmpR=(*it3);
            bool shouldAdd = true;
            for (auto it4=addingAll.begin();it4!=addingAll.end();it4++){
                auto tmpR2 = (*it4);
                if (tmpR2 == tmpR){
                    continue;
                }
                if (tmpR.enclosedBy(tmpR2)){
                    shouldAdd = false;
                    break;
                }
            }
            if (shouldAdd)
                addingAll2.push_back(tmpR);
        }
#endif
        bool hasAny = false;
        // add all rectangles in addingAll2 that aren't already enclosed by existing rectangles
        for (auto it = addingAll2.begin(); it!=addingAll2.end(); it++){
            if (minDim > 0){
                auto rect = *it;
                if (rect.getWidth()>=minDim && rect.getHeight()>=minDim && !IntervalTree<numericalValueType, nodeClass, internalNodeClass>::isEnclosedBy(*it)){
                    listController->addData(*it);
                    hasAny = true;
                }
            } else {
                if ((*it).area()>0 && !IntervalTree<numericalValueType, nodeClass, internalNodeClass>::isEnclosedBy(*it)){
                    listController->addData(*it);
                    hasAny = true;
                }
            }
            auto wq = IntervalTree<numericalValueType, nodeClass, internalNodeClass>::windowQuery(*it);
            for (auto it2 = wq->begin(); it2 != wq->end(); it2++){
                if ((*it)!=(*it2) && it2->enclosedBy(*it)){
                    listController->deleteData(*it2);
                }
            }
            delete wq;
        }
        return hasAny;
    }
    float getPercentageSpaceOverlapped(const nodeClass queryRect){
        auto resList = this->windowQuery(queryRect);
        int totalArea = queryRect.area();
        int accumArea = 0;
        stringstream ss;
        if (resList){
            IntervalTree2<numericalValueType, nodeClass, internalNodeClass > spaceIT;
            spaceIT.addData(queryRect);
            for (auto resRectIT = resList->begin(); resRectIT!=resList->end(); resRectIT++){
                auto resRect = (*resRectIT);
                auto spaceResRectList = spaceIT.windowQuery(resRect);
                if (spaceResRectList){
                    for (auto spaceResRectIT = spaceResRectList->begin(); spaceResRectIT!=spaceResRectList->end(); spaceResRectIT++){
                        Rectangle2i inters;
                        auto spaceResRect = *spaceResRectIT;
                        if (intersect(resRect, spaceResRect, inters)){
                            accumArea += inters.area();
                            spaceIT.addFullRectangleInEmptySpace(inters, &spaceIT);
                        }
                    }
                    delete spaceResRectList;
                }
            }
            delete resList;
        }
        return accumArea/(float)totalArea; // percentage
    }

    virtual bool addFullRectangleInEmptySpace(const nodeClass rs, DataListInterface<nodeClass> *listController){
        return getSpacesInsideAndAddFullRectangleInEmptySpace(rs, listController);
    }
    virtual bool getSpacesInsideAndAddFullRectangleInEmptySpace(const nodeClass rs,
                                                                DataListInterface<nodeClass> *listController,
                                                                DataListInterface<nodeClass> *insideController = NULL,
                                                                IntervalTree<numericalValueType, nodeClass, internalNodeClass> *insideIntervalTree = NULL, numericalValueType minDimension = 0){
        if (rs.area()<=0){
            stringstream ss;
            ss << "Warning: area of rectangle passed in addRectangle less than or equal to 0 rect=" << rs << endl;
            cout << ss.str();
        }
        list<nodeClass> v[4],va[4];
        auto containmentList = IntervalTree<numericalValueType, nodeClass, internalNodeClass>::windowQuery(rs);
        bool hasAny = false, hasAnyBeenSet = false;
        if (insideController && insideIntervalTree){
            // if space inside is needed, take all rectangles in containmentList, find largest, and add addOnePiece for each
            hasAnyBeenSet = true;
            IntervalTree2<numericalValueType, nodeClass, internalNodeClass> addingAll;
            bool entireSpace = false;
            for (auto it = containmentList->begin(); it!=containmentList->end();it++){
                auto clrs = (*it);
                nodeClass rect;
                intersect(clrs, rs, rect);
                entireSpace |= rs.enclosedBy(rect);
                if (rect.area()>0){
                    addingAll.push_back(rect);
                }
            }
            
            // add all from addingAll that are enclosed by another rectangle to addingAll2
            auto *addingAllTmp = addingAll.getAll();
            vector<nodeClass> addingAll2;
            for (auto it3=addingAllTmp->begin();it3!=addingAllTmp->end();it3++){
                auto rect = (*it3);
                addingAll.remove(&rect);
                if (!addingAll.isEnclosedBy(rect)) {
                    addingAll2.push_back(rect);
                    addingAll.add(&rect); // only need to re-add it if it isn't enclosed by any other
                }
            }
            delete addingAllTmp;
            
            if (insideIntervalTree->isEmpty()){
                for (auto it = addingAll2.begin(); it!=addingAll2.end(); it++){
                    auto rect = *it;
                    if (rect.getWidth()>= minDimension && rect.getHeight() >= minDimension){
                    //if (entireSpace || (rect.getWidth()>= minDimension && rect.getHeight() >= minDimension)){
                        insideController->addData(rect);
                        hasAny = true;
                    }
                }
            } else {
                bool debug = false;
                stringstream ss;
                if (debug){
                    auto allV = insideIntervalTree->getAll();
                    ss << "getSpacesInsideAndAddFullRectangleInEmptySpace: adding to insideIntervalTree when not empty: #rects=" << allV->size() << endl;
                    for (auto it = allV->begin(); it!=allV->end(); it++){
                        ss << "\t" << (*it) << endl;
                    }
                    delete allV;
                    ss << "ADDING #rects=" << addingAll2.size() << ":" << endl;
                }
                
                for (auto it = addingAll2.begin(); it!=addingAll2.end(); it++){
                    if (debug){
                        ss << "\t" << *it << endl;
                    }
                    bool ret = insideIntervalTree->addOnePiece(*it, insideController, minDimension);//entireSpace ? 0 : minDimension);
                    hasAny |= ret;
                }
                if (debug){
                    auto allV = insideIntervalTree->getAll();
                    ss << "RESULTS:" << " #rects=" << allV->size() << endl;
                    for (auto it = allV->begin(); it!=allV->end(); it++){
                        ss << "\t" << (*it) << endl;
                    }
                    delete allV;
                    cout << ss.str().c_str();
                }
            }
        }
        for (auto it = containmentList->begin(); it!=containmentList->end();it++){
            auto clrs = (*it);
            if (nodeClass::type_equals(rs.minx,clrs.maxx)){  // Exactly adjacent to left, only add to list for containment query
                va[0].push_back(clrs);
                continue;
            }
            if (nodeClass::type_equals(rs.maxx,clrs.minx)){  // Exactly adjacent to right, only add to list for containment query
                va[1].push_back(clrs);
                continue;
            }
            if (nodeClass::type_equals(rs.maxy,clrs.miny)){  // Exactly adjacent to top, only add to list for containment query
                va[2].push_back(clrs);
                continue;
            }
            if (nodeClass::type_equals(rs.miny,clrs.maxy)){  // Exactly adjacent to bottom, only add to list for containment query
                va[3].push_back(clrs);
                continue;
            }
            listController->deleteData(clrs);
            if (hasAnyBeenSet || minDimension == 0 || hasAny){
                if (nodeClass::type_compare(rs.minx, clrs.minx) > 0){ // Left
                    v[0].push_back(nodeClass(clrs.minx,clrs.miny,rs.minx,clrs.maxy));
                }
                if (nodeClass::type_compare(clrs.maxx,rs.maxx) > 0){ // right
                    v[1].push_back(nodeClass(rs.maxx,clrs.miny,clrs.maxx,clrs.maxy));
                }
                if (nodeClass::type_compare(clrs.maxy,rs.maxy) > 0){ // top
                    v[2].push_back(nodeClass(clrs.minx,rs.maxy,clrs.maxx,clrs.maxy));
                }
                if (nodeClass::type_compare(rs.miny, clrs.miny) > 0){ // bottom
                    v[3].push_back(nodeClass(clrs.minx,clrs.miny,clrs.maxx,rs.miny));
                }
            } else {
                // if hasAnyBeenSet not set, and minDimension > 0, need to figure out if visible, i.e., moved gt minDim in both dimensions
                if (nodeClass::type_compare(rs.minx, clrs.minx) > 0){ // Left
                    if (clrs.getHeight() >= minDimension && (rs.minx - clrs.minx) >= minDimension)
                        hasAny = true;
                    v[0].push_back(nodeClass(clrs.minx,clrs.miny,rs.minx,clrs.maxy));
                }
                if (nodeClass::type_compare(clrs.maxx,rs.maxx) > 0){ // right
                    if (clrs.getHeight() >= minDimension && (clrs.maxx - rs.maxx) >= minDimension)
                        hasAny = true;
                    v[1].push_back(nodeClass(rs.maxx,clrs.miny,clrs.maxx,clrs.maxy));
                }
                if (nodeClass::type_compare(clrs.maxy,rs.maxy) > 0){ // top
                    if (clrs.getWidth() >= minDimension && (clrs.maxy - rs.maxy) >= minDimension)
                        hasAny = true;
                    v[2].push_back(nodeClass(clrs.minx,rs.maxy,clrs.maxx,clrs.maxy));
                }
                if (nodeClass::type_compare(rs.miny, clrs.miny) > 0){ // bottom
                    if (clrs.getWidth() >= minDimension && (rs.miny - clrs.miny) >= minDimension)
                        hasAny = true;
                    v[3].push_back(nodeClass(clrs.minx,clrs.miny,clrs.maxx,rs.miny));
                }
            }
        }
        delete containmentList;
        for (int doeach=0; doeach<4; doeach++){
            for (auto it=v[doeach].begin();it!=v[doeach].end(); ){
                auto tmpRect = *it;
                bool removed=false;
                for (auto it2=va[doeach].begin();it2!=va[doeach].end();it2++){
                    auto tmpRect2 = (*it2);
                    if (tmpRect.enclosedBy(&tmpRect2)){
                        removed=true;
                        break;
                    }
                }
                if (!removed){
                    for (auto it2=v[doeach].begin();it2!=v[doeach].end();it2++){
                        auto tmpRect2 = (*it2);
                        if (tmpRect2==tmpRect){
                            continue;
                        }
                        if (tmpRect.enclosedBy(&tmpRect2)){
                            removed = true;
                            break;
                        }
                    }
                }
                if (removed)
                    it = v[doeach].erase(it);
                else
                    it++;
            }
            bool hasAdded = false;
            for (auto it=v[doeach].begin();it!=v[doeach].end();it++){
                auto tmpRect = (*it);
                listController->addData(tmpRect);
                hasAdded = true;
            }
            if (minDimension<=0 && hasAdded)
                hasAny = true;  // only when empty space representation changes, is the projection visible
                                // this is done here to support the situation when the inside visible space is
                                // not needed (and its cheap to do it here)
        }
        return hasAny;
    }
};

template <typename numericalValueType, typename nodeClass, typename nodeClassSet> class SecondSecondaryStructure2 : public SecondaryStructure<numericalValueType, nodeClass> {
public:
    nodeClassSet **ts;
    virtual bool add(nodeClass *ob) {
        //cout << "SecondSecondaryStructure2.add: ob=" << *ob << endl;
        for (int i=0; i<4; i++)
            ts[i]->insert(*ob);
        return true;
    }
    virtual bool isEmpty(){
        return ts[0]->empty();
    }
    virtual vector<nodeClass> *getNeededCriteria(NeededCriteria<numericalValueType, nodeClass> &neededCriteria, vector<nodeClass> *resultList){
        if (neededCriteria.empty()){
            for (auto it=ts[0]->begin(); it!=ts[0]->end(); it++) resultList->push_back(*it);
        } else {
            nodeClassSet retList(nodeClass::default_compare);
            nodeClass tmpRect;
            for (auto it=neededCriteria.neededCriteria.begin(); it!= neededCriteria.neededCriteria.end(); it++){
                auto &nCond = *it;
                int tmpTreeNum = 0;
                if (!(nCond.direction==0 ^ nCond.clusive)){ // less-than
                    switch (nCond.dimension->intervalID){
                        case 1: // x direction
                            tmpRect.set(nCond.value, nodeClass::MAX_VAL, nCond.value, nodeClass::MAX_VAL);
                            break;
                        case 2:
                            tmpRect.set(nodeClass::MAX_VAL, nCond.value, nodeClass::MAX_VAL, nCond.value);
                            tmpTreeNum+=2;
                            break;
                        default:
                            cout << "WARNING: intervalID=" << nCond.dimension->intervalID << endl;
                    }
                } else {
                    switch (nCond.dimension->intervalID){
                        case 1: // x direction
                            tmpRect.set(nCond.value, nodeClass::MIN_VAL, nCond.value, nodeClass::MIN_VAL);
                            break;
                        case 2: // y direction
                            tmpRect.set(nodeClass::MIN_VAL, nCond.value, nodeClass::MIN_VAL, nCond.value);
                            tmpTreeNum+=2;
                            break;
                        default:
                            cout << "WARNING: intervalID=" << nCond.dimension->intervalID << endl;
                    }
                }
                nodeClassSet tmpList(nodeClass::default_compare);
                if (nCond.direction==0){
                    // front of list (before value)
                    auto comp = ts[tmpTreeNum]->key_comp();
                    auto it = ts[tmpTreeNum]->begin();
                    auto end = ts[tmpTreeNum]->end();
                    while (it!=end && comp(*it, tmpRect)){
                        tmpList.insert(*it);
                        it++;
                    }
                } else {
                    // end of list (after value)
                    tmpTreeNum++;
                    if (!ts[tmpTreeNum]->empty()){
                        auto comp = ts[tmpTreeNum]->key_comp();
                        auto it = ts[tmpTreeNum]->end();
                        auto beg = ts[tmpTreeNum]->begin();
                        bool cont = true, cmpv, atbeg = (it == beg);
                        while (cont){
                            if (!atbeg)
                                it--;
                            cmpv=comp(tmpRect, *it);
                            cont = !atbeg && cmpv;
                            if (cmpv)
                                tmpList.insert(*it);
                            if (!atbeg)
                                atbeg = (it == beg);
                        }
                    }
                }
                if (tmpList.empty()){
                    return resultList;
                }
                if (retList.empty()){
                    for (auto it=tmpList.begin();it!=tmpList.end();it++){
                        retList.insert(*it);
                    }
                } else {
                    // union

                    vector<nodeClass> er;
                    for (auto it=retList.begin();it!=retList.end();it++){
                        nodeClass r1 = (*it);
                        if (tmpList.find(*it)==tmpList.end()){
                            er.push_back(*it);
                        }
                    }
                    for (auto it=er.begin(); it!=er.end(); it++){
                        retList.erase(*it);
                    }
                    if (retList.empty()){
                        return resultList;
                    }
                }
                
            }
            for (auto it=retList.begin(); it!=retList.end(); it++){
                resultList->push_back(*it);
            }
        }
        return resultList;
    }
    virtual vector<nodeClass> *enclosedBy(const nodeClass &r, NeededCriteria<numericalValueType, nodeClass> &neededCriteria, bool justGetOne, vector<nodeClass> *resultList){
        return getNeededCriteria(neededCriteria, resultList);
    }
    virtual vector<nodeClass> *windowQuery(const nodeClass &rs, NeededCriteria<numericalValueType, nodeClass> &neededCriteria, bool inclusive, vector<nodeClass> *resultList, bool justGetOne){
        return getNeededCriteria(neededCriteria, resultList);
    };
    virtual bool remove(nodeClass &r){
        bool ret = true;
        for (int i=0; i<4; i++){
            if (ts[i]->find(r)==ts[i]->end())
                ret = false;
            ts[i]->erase(r);
        }
        return ret;
    }
    virtual vector<nodeClass> *getAll(vector<nodeClass> *allList){
        for (auto it = ts[0]->begin(); it!= ts[0]->end(); it++){
            allList->push_back(*it);
        }
        return allList;
    }
    virtual void print(ostream &os){
        os << "SecondSecondaryStructure.print()" << endl;

        for (auto it = ts[0]->begin(); it!= ts[0]->end(); it++){
            os << "\t" << (*it) << "\t";
        }
        os << endl;
    }
    SecondSecondaryStructure2(){
        ts = (nodeClassSet **)malloc ( sizeof(nodeClassSet*)* 4 );
        ts[0] = new nodeClassSet(&nodeClass::minx_compare);
        ts[1] = new nodeClassSet(&nodeClass::maxx_compare);
        ts[2] = new nodeClassSet(&nodeClass::miny_compare);
        ts[3] = new nodeClassSet(&nodeClass::maxy_compare);
    }
    ~SecondSecondaryStructure2(){
        for (int i=0; i<4; i++) delete ts[i];
        free(ts);
    }
};

template <typename numericalValueType, typename nodeClass, typename nodeClassSet> class FirstSecondaryStructure2 : public SecondaryStructure<numericalValueType, nodeClass> {
public:
    IntervalTree2<numericalValueType, nodeClass, SecondSecondaryStructure2<numericalValueType, nodeClass, nodeClassSet> > secondaryIntervalTree;
    virtual bool add(nodeClass *ob){
        secondaryIntervalTree.add(ob);
        return true;
    };
    virtual bool isEmpty(){
        return secondaryIntervalTree.isEmpty();
    }
    virtual vector<nodeClass> *enclosedBy(const nodeClass &r, NeededCriteria<numericalValueType, nodeClass> &nC, bool justGetOne, vector<nodeClass> *resultList) {
        return secondaryIntervalTree.enclosedBy(r, nC, justGetOne, resultList);
    }
    virtual vector<nodeClass> *windowQuery(const nodeClass &rs, NeededCriteria<numericalValueType, nodeClass> &neededCriteria, bool inclusive, vector<nodeClass> *resultList, bool justGetOne){
        return secondaryIntervalTree.windowQuery(rs, neededCriteria, inclusive, resultList, justGetOne);
    };
    virtual bool remove(nodeClass &r){
        return secondaryIntervalTree.remove(&r);
    }
    virtual vector<nodeClass> *getAll(vector<nodeClass> *allList){
        return secondaryIntervalTree.getAll(allList);
    }
    virtual void print(ostream &os){
        os << "FirstSecondaryStructure.print()" << endl;
        os << secondaryIntervalTree;
    }
    FirstSecondaryStructure2() : secondaryIntervalTree(&IntervalTree2<numericalValueType, nodeClass, SecondSecondaryStructure2<numericalValueType, nodeClass, nodeClassSet> >::secondary_interval_dimension){
    }
    ~FirstSecondaryStructure2(){
    }
};

template <typename numericalValueType, typename nodeClass, typename internalNodeClass > class IntervalTree3 : public IntervalTree<numericalValueType, nodeClass, internalNodeClass> {
public:
    IntervalTree3<numericalValueType, nodeClass, internalNodeClass>(IntervalDimension<numericalValueType, nodeClass> *iD=NULL) : IntervalTree<numericalValueType, nodeClass, internalNodeClass>(iD) {}
    virtual bool addOnePiece(const nodeClass rs, DataListInterface<nodeClass> *listController, numericalValueType minDim = 0){
#ifdef USE_INTERVAL_TREE
        IntervalTree3<numericalValueType, nodeClass, internalNodeClass> addingAll;
#else
        vector<nodeClass> addingAll;  // this could be replaced by an IntervalTree
#endif
        addingAll.push_back(rs);
        auto rectsIn = this->windowQuery(rs);
        //        cout << "addOnePiece: rs=" << rs << " rectsIn.size=" << rectsIn->size() << endl;
        // Create all rectangles addingAll that can be merged with existing overlapping or adjacent ones
        for (auto it=rectsIn->begin(); it!=rectsIn->end(); it++){
            auto rect = (*it);
            //            cout << "\trect=" << rect << endl;
            if (rect.enclosedBy(rs)){
                listController->deleteData(rect);
                continue;
            }
            if (isAdjacentX(rect,rs)){
                addingAll.push_back(consensus(rs,rect,0));
            } else if (isAdjacentY(rect,rs)){
                addingAll.push_back(consensus(rs,rect,1));
            } else if (has_intersection(rect,rs) && !rect.enclosedBy(rs)){
                auto tmprect = consensus(rs,rect,0);
                if (!tmprect.enclosedBy(rs) && !tmprect.enclosedBy(rect)){
                    addingAll.push_back(tmprect);
                }
                tmprect=consensus(rs,rect,1);
                if (!tmprect.enclosedBy(rs) && !tmprect.enclosedBy(rect)){
                    addingAll.push_back(tmprect);
                }
            }
        }
        delete rectsIn; // no longer needed
        
        vector<nodeClass> addingAll2;
        
#ifdef USE_INTERVAL_TREE
        vector<nodeClass> *addingAllTmp = addingAll.getAll();
        for (auto it3=addingAllTmp->begin();it3!=addingAllTmp->end();it3++){
            auto rect = (*it3);
            addingAll.remove(&rect);
            if (!addingAll.isEnclosedBy(rect)) {
                addingAll2.push_back(rect);
                addingAll.add(&rect); // only need to re-add it if it isn't enclosed by any other
            }
        }
        delete addingAllTmp;
#else
        // add all rectangles into addingAll2 that are not enclosedBy others in addingAll
        // this could probably be enhanced by using an IntervalTree for addingAll, to speedup
        // isEnclosedBy(), instead of exhaustively going through all in addingAll
        for (auto it3=addingAll.begin();it3!=addingAll.end();it3++){
            auto tmpR=(*it3);
            bool shouldAdd = true;
            for (auto it4=addingAll.begin();it4!=addingAll.end();it4++){
                auto tmpR2 = (*it4);
                if (tmpR2 == tmpR){
                    continue;
                }
                if (tmpR.enclosedBy(tmpR2)){
                    shouldAdd = false;
                    break;
                }
            }
            if (shouldAdd)
                addingAll2.push_back(tmpR);
        }
#endif
        bool hasAny = false;
        // add all rectangles in addingAll2 that aren't already enclosed by existing rectangles
        for (auto it = addingAll2.begin(); it!=addingAll2.end(); it++){
            if (minDim > 0){
		auto rect = *it;
	        if (rect.getWidth()>=minDim && rect.getHeight()>=minDim && !IntervalTree<numericalValueType, nodeClass, internalNodeClass>::isEnclosedBy(*it)){
        	    listController->addData(*it);
                    hasAny = true;
            	}
	    } else {
	        if (!IntervalTree<numericalValueType, nodeClass, internalNodeClass>::isEnclosedBy(*it)){
        	    listController->addData(*it);
                    hasAny = true;
            	}
	    }
            auto wq = IntervalTree<numericalValueType, nodeClass, internalNodeClass>::windowQuery(*it);
            for (auto it2 = wq->begin(); it2 != wq->end(); it2++){
                if ((*it)!=(*it2) && it2->enclosedBy(*it)){
                    listController->deleteData(*it2);
                }
            }
            delete wq;
        }
	return hasAny;
    }
    virtual bool getSpacesInsideAndAddFullRectangleInEmptySpace(const nodeClass rs,
                                                                DataListInterface<nodeClass> *listController,
                                                                DataListInterface<nodeClass> *insideController = NULL,
                                                                IntervalTree<numericalValueType, nodeClass, internalNodeClass> *insideIntervalTree = NULL, numericalValueType minDimension = 0){
        if (rs.area()<=0){
            stringstream ss;
            ss << "Warning: area of rectangle passed in addRectangle less than or equal to 0 rect=" << rs << endl;
            cout << ss.str();
        }
        //        cout << " getSpacesInsideAndAddFullRectangleInEmptySpace rs=" << rs << endl;
        list<nodeClass> v[6],va[6];
        auto containmentList = IntervalTree<numericalValueType, nodeClass, internalNodeClass>::windowQuery(rs);
        
        if (insideController && insideIntervalTree){
            // if space inside is needed, take all rectangles in containmentList, find largest, and add addOnePiece for each
            
            IntervalTree3<numericalValueType, nodeClass, internalNodeClass> addingAll;
            for (auto it = containmentList->begin(); it!=containmentList->end();it++){
                auto clrs = (*it);
                nodeClass rect;
                intersect(clrs, rs, rect);
                if (rect.area()>0){
                    addingAll.push_back(rect);
                }
            }
            
            // add all from addingAll that are enclosed by another rectangle to addingAll2
            auto *addingAllTmp = addingAll.getAll();
            vector<nodeClass> addingAll2;
            for (auto it3=addingAllTmp->begin();it3!=addingAllTmp->end();it3++){
                auto rect = (*it3);
                addingAll.remove(&rect);
                if (!addingAll.isEnclosedBy(rect)) {
                    addingAll2.push_back(rect);
                    addingAll.add(&rect); // only need to re-add it if it isn't enclosed by any other
                }
            }
            delete addingAllTmp;
            
            if (insideIntervalTree->isEmpty()){
                for (auto it = addingAll2.begin(); it!=addingAll2.end(); it++){
                    insideController->addData(*it);
                }
            } else {
                bool debug = false;
                stringstream ss;
                if (debug){
                    auto allV = insideIntervalTree->getAll();
                    ss << "getSpacesInsideAndAddFullRectangleInEmptySpace: adding to insideIntervalTree when not empty: #rects=" << allV->size() << endl;
                    for (auto it = allV->begin(); it!=allV->end(); it++){
                        ss << "\t" << (*it) << endl;
                    }
                    delete allV;
                    ss << "ADDING #rects=" << addingAll2.size() << ":" << endl;
                }
                
                for (auto it = addingAll2.begin(); it!=addingAll2.end(); it++){
                    if (debug){
                        ss << "\t" << *it << endl;
                    }
                    insideIntervalTree->addOnePiece(*it, insideController);
                }
                if (debug){
                    auto allV = insideIntervalTree->getAll();
                    ss << "RESULTS:" << " #rects=" << allV->size() << endl;
                    for (auto it = allV->begin(); it!=allV->end(); it++){
                        ss << "\t" << (*it) << endl;
                    }
                    delete allV;
                    cout << ss.str().c_str();
                }
            }
        }
        for (auto it = containmentList->begin(); it!=containmentList->end();it++){
            auto clrs = (*it);
            if (nodeClass::type_equals(rs.minx,clrs.maxx)){  // Exactly adjacent to left, only add to list for containment query
                va[0].push_back(clrs);
                continue;
            }
            if (nodeClass::type_equals(rs.maxx,clrs.minx)){  // Exactly adjacent to right, only add to list for containment query
                va[1].push_back(clrs);
                continue;
            }
            if (nodeClass::type_equals(rs.maxy,clrs.miny)){  // Exactly adjacent to top, only add to list for containment query
                va[2].push_back(clrs);
                continue;
            }
            if (nodeClass::type_equals(rs.miny,clrs.maxy)){  // Exactly adjacent to bottom, only add to list for containment query
                va[3].push_back(clrs);
                continue;
            }
            if (nodeClass::type_equals(rs.minz,clrs.maxz)){  // Exactly adjacent to bottom in z, only add to list for containment query
                va[4].push_back(clrs);
                continue;
            }
            if (nodeClass::type_equals(rs.maxz,clrs.minz)){  // Exactly adjacent to top in z, only add to list for containment query
                va[5].push_back(clrs);
                continue;
            }
            listController->deleteData(clrs);
            if (nodeClass::type_compare(rs.minx, clrs.minx) > 0){ // Left
                v[0].push_back(nodeClass(clrs.minx,clrs.miny,clrs.minz,rs.minx,clrs.maxy,clrs.maxz));
            }
            if (nodeClass::type_compare(clrs.maxx,rs.maxx) > 0){ // right
                v[1].push_back(nodeClass(rs.maxx,clrs.miny,clrs.minz,clrs.maxx,clrs.maxy,clrs.maxz));
            }
            if (nodeClass::type_compare(clrs.maxy,rs.maxy) > 0){ // top
                v[2].push_back(nodeClass(clrs.minx,rs.maxy,clrs.minz,clrs.maxx,clrs.maxy,clrs.maxz));
            }
            if (nodeClass::type_compare(rs.miny, clrs.miny) > 0){ // bottom
                v[3].push_back(nodeClass(clrs.minx,clrs.miny,clrs.minz,clrs.maxx,rs.miny,clrs.maxz));
            }
            if (nodeClass::type_compare(rs.minz, clrs.minz) > 0){ // bottom
                v[4].push_back(nodeClass(clrs.minx,clrs.miny,clrs.minz,clrs.maxx,clrs.maxy,rs.minz));
            }
            if (nodeClass::type_compare(clrs.maxz,rs.maxz) > 0){ // top
                v[5].push_back(nodeClass(clrs.minx,clrs.miny,rs.maxz,clrs.maxx,clrs.maxy,clrs.maxz));
            }
        }
        bool hasAny = false; // !containmentList->empty();
        delete containmentList;
        for (int doeach=0; doeach<4; doeach++){
            for (auto it=v[doeach].begin();it!=v[doeach].end(); ){
                auto tmpRect = *it;
                bool removed=false;
                for (auto it2=va[doeach].begin();it2!=va[doeach].end();it2++){
                    auto tmpRect2 = (*it2);
                    if (tmpRect.enclosedBy(&tmpRect2)){
                        removed=true;
                        break;
                    }
                }
                if (!removed){
                    for (auto it2=v[doeach].begin();it2!=v[doeach].end();it2++){
                        auto tmpRect2 = (*it2);
                        if (tmpRect2==tmpRect){
                            continue;
                        }
                        if (tmpRect.enclosedBy(&tmpRect2)){
                            removed = true;
                            break;
                        }
                    }
                }
                if (removed)
                    it = v[doeach].erase(it);
                else
                    it++;
            }
            for (auto it=v[doeach].begin();it!=v[doeach].end();it++){
                auto tmpRect = (*it);
                listController->addData(tmpRect);
                hasAny = true;  // only when empty space representation changes, is the projection visible
                // this is done here to support the situation when the inside visible space is
                // not needed (and its cheap to do it here)
            }
        }
        return hasAny;
    }
};

template <typename numericalValueType, typename nodeClass, typename nodeClassSet> class TertiarySecondaryStructure3 : public SecondaryStructure<numericalValueType, nodeClass> {
public:
    nodeClassSet **ts;
    virtual bool add(nodeClass *ob) {
        //cout << "SecondSecondaryStructure2.add: ob=" << *ob << endl;
        for (int i=0; i<6; i++)
            ts[i]->insert(*ob);
        return true;
    }
    virtual bool isEmpty(){
        return ts[0]->empty();
    }
    virtual vector<nodeClass> *getNeededCriteria(NeededCriteria<numericalValueType, nodeClass> &neededCriteria, vector<nodeClass> *resultList){
        if (neededCriteria.empty()){
            for (auto it=ts[0]->begin(); it!=ts[0]->end(); it++) resultList->push_back(*it);
        } else {
            nodeClassSet retList(nodeClass::default_compare);
            nodeClass tmpRect;
            for (auto it=neededCriteria.neededCriteria.begin(); it!= neededCriteria.neededCriteria.end(); it++){
                auto &nCond = *it;
                int tmpTreeNum = 0;
                if (!(nCond.direction==0 ^ nCond.clusive)){ // less-than
                    switch (nCond.dimension->intervalID){
                        case 1: // x direction
                            tmpRect.set(nCond.value, nodeClass::MAX_VAL, nodeClass::MAX_VAL, nCond.value, nodeClass::MAX_VAL, nodeClass::MAX_VAL);
                            break;
                        case 2: // y direction
                            tmpRect.set(nodeClass::MAX_VAL, nCond.value, nodeClass::MAX_VAL, nodeClass::MAX_VAL, nCond.value, nodeClass::MAX_VAL);
                            tmpTreeNum+=2;
                            break;
                        case 3: // z direction
                            tmpRect.set(nodeClass::MAX_VAL, nodeClass::MAX_VAL, nCond.value, nodeClass::MAX_VAL, nodeClass::MAX_VAL, nCond.value);
                            tmpTreeNum+=4;
                            break;
                        default:
                            cout << "WARNING: intervalID=" << nCond.dimension->intervalID << endl;
                    }
                } else {
                    switch (nCond.dimension->intervalID){
                        case 1: // x direction
                            tmpRect.set(nCond.value, nodeClass::MIN_VAL, nodeClass::MIN_VAL, nCond.value, nodeClass::MIN_VAL, nodeClass::MIN_VAL);
                            break;
                        case 2: // y direction
                            tmpRect.set(nodeClass::MIN_VAL, nCond.value, nodeClass::MIN_VAL, nodeClass::MIN_VAL, nCond.value, nodeClass::MIN_VAL);
                            tmpTreeNum+=2;
                            break;
                        case 3: // z direction
                            tmpRect.set(nodeClass::MIN_VAL, nodeClass::MIN_VAL, nCond.value, nodeClass::MIN_VAL, nodeClass::MIN_VAL, nCond.value);
                            tmpTreeNum+=4;
                            break;
                        default:
                            cout << "WARNING: intervalID=" << nCond.dimension->intervalID << endl;
                    }
                }
                nodeClassSet tmpList(nodeClass::default_compare);
                if (nCond.direction==0){
                    // front of list (before value)
                    auto comp = ts[tmpTreeNum]->key_comp();
                    auto it = ts[tmpTreeNum]->begin();
                    auto end = ts[tmpTreeNum]->end();
                    while (it!=end && comp(*it, tmpRect)){
                        tmpList.insert(*it);
                        it++;
                    }
                } else {
                    // end of list (after value)
                    tmpTreeNum++;
                    auto comp = ts[tmpTreeNum]->key_comp();
                    auto it = ts[tmpTreeNum]->end();
                    auto beg = ts[tmpTreeNum]->begin();
                    bool cont = true, cmpv, atbeg = (it == beg);
                    while (cont){
                        if (!atbeg)
                            it--;
                        cmpv=comp(tmpRect, *it);
                        cont = !atbeg && cmpv;
                        if (cmpv)
                            tmpList.insert(*it);
                        if (!atbeg)
                            atbeg = (it == beg);
                    }
                }
                if (tmpList.empty()){
                    return resultList;
                }
                if (retList.empty()){
                    for (auto it=tmpList.begin();it!=tmpList.end();it++){
                        retList.insert(*it);
                    }
                } else {
                    // union
                    
                    vector<nodeClass> er;
                    for (auto it=retList.begin();it!=retList.end();it++){
                        nodeClass r1 = (*it);
                        if (tmpList.find(*it)==tmpList.end()){
                            er.push_back(*it);
                        }
                    }
                    for (auto it=er.begin(); it!=er.end(); it++){
                        retList.erase(*it);
                    }
                    if (retList.empty()){
                        return resultList;
                    }
                }
                
            }
            for (auto it=retList.begin(); it!=retList.end(); it++){
                resultList->push_back(*it);
            }
        }
        return resultList;
    }
    virtual vector<nodeClass> *enclosedBy(const nodeClass &r, NeededCriteria<numericalValueType, nodeClass> &neededCriteria, bool justGetOne, vector<nodeClass> *resultList){
        return getNeededCriteria(neededCriteria, resultList);
    }
    virtual vector<nodeClass> *windowQuery(const nodeClass &rs, NeededCriteria<numericalValueType, nodeClass> &neededCriteria, bool inclusive, vector<nodeClass> *resultList, bool justGetOne){
        return getNeededCriteria(neededCriteria, resultList);
    };
    virtual bool remove(nodeClass &r){
        bool ret = true;
        for (int i=0; i<6; i++){
            if (ts[i]->find(r)==ts[i]->end())
                ret = false;
            ts[i]->erase(r);
        }
        return ret;
    }
    virtual vector<nodeClass> *getAll(vector<nodeClass> *allList){
        for (auto it = ts[0]->begin(); it!= ts[0]->end(); it++){
            allList->push_back(*it);
        }
        return allList;
    }
    virtual void print(ostream &os){
        os << "SecondSecondaryStructure.print()" << endl;
        
        for (auto it = ts[0]->begin(); it!= ts[0]->end(); it++){
            os << "\t" << (*it) << "\t";
        }
        os << endl;
    }
    TertiarySecondaryStructure3(){
        ts = (nodeClassSet **)malloc ( sizeof(nodeClassSet*)* 6 );
        ts[0] = new nodeClassSet(&nodeClass::minx_compare);
        ts[1] = new nodeClassSet(&nodeClass::maxx_compare);
        ts[2] = new nodeClassSet(&nodeClass::miny_compare);
        ts[3] = new nodeClassSet(&nodeClass::maxy_compare);
        ts[4] = new nodeClassSet(&nodeClass::minz_compare);
        ts[5] = new nodeClassSet(&nodeClass::maxz_compare);
    }
    ~TertiarySecondaryStructure3(){
        for (int i=0; i<6; i++) delete ts[i];
        free(ts);
    }
};


template <typename numericalValueType, typename nodeClass, typename nodeClassSet> class SecondSecondaryStructure3 : public SecondaryStructure<numericalValueType, nodeClass> {
public:
    IntervalTree3<numericalValueType, nodeClass, TertiarySecondaryStructure3<numericalValueType, nodeClass, nodeClassSet> > tertiaryIntervalTree;
    virtual bool add(nodeClass *ob){
        tertiaryIntervalTree.add(ob);
        return true;
    };
    virtual bool isEmpty(){
        return tertiaryIntervalTree.isEmpty();
    }
    virtual vector<nodeClass> *enclosedBy(const nodeClass &r, NeededCriteria<numericalValueType, nodeClass> &nC, bool justGetOne, vector<nodeClass> *resultList) {
        return tertiaryIntervalTree.enclosedBy(r, nC, justGetOne, resultList);
    }
    virtual vector<nodeClass> *windowQuery(const nodeClass &rs, NeededCriteria<numericalValueType, nodeClass> &neededCriteria, bool inclusive, vector<nodeClass> *resultList, bool justGetOne){
        return tertiaryIntervalTree.windowQuery(rs, neededCriteria, inclusive, resultList);
    };
    virtual bool remove(nodeClass &r){
        return tertiaryIntervalTree.remove(&r);
    }
    virtual vector<nodeClass> *getAll(vector<nodeClass> *allList){
        return tertiaryIntervalTree.getAll(allList);
    }
    virtual void print(ostream &os){
        os << "SecondSecondaryStructure.print()" << endl;
        os << tertiaryIntervalTree;
    }
    SecondSecondaryStructure3() : tertiaryIntervalTree(&IntervalTree3<numericalValueType, nodeClass, SecondSecondaryStructure3<numericalValueType, nodeClass, nodeClassSet> >::tertiary_interval_dimension){
    }
    ~SecondSecondaryStructure3(){
    }
};

template <typename numericalValueType, typename nodeClass, typename nodeClassSet> class FirstSecondaryStructure3 : public SecondaryStructure<numericalValueType, nodeClass> {
public:
    IntervalTree3<numericalValueType, nodeClass, SecondSecondaryStructure3<numericalValueType, nodeClass, nodeClassSet> > secondaryIntervalTree;
    virtual bool add(nodeClass *ob){
        secondaryIntervalTree.add(ob);
        return true;
    };
    virtual bool isEmpty(){
        return secondaryIntervalTree.isEmpty();
    }
    virtual vector<nodeClass> *enclosedBy(const nodeClass &r, NeededCriteria<numericalValueType, nodeClass> &nC, bool justGetOne, vector<nodeClass> *resultList) {
        return secondaryIntervalTree.enclosedBy(r, nC, justGetOne, resultList);
    }
    virtual vector<nodeClass> *windowQuery(const nodeClass &rs, NeededCriteria<numericalValueType, nodeClass> &neededCriteria, bool inclusive, vector<nodeClass> *resultList, bool justGetOne){
        return secondaryIntervalTree.windowQuery(rs, neededCriteria, inclusive, resultList, justGetOne);
    };
    virtual bool remove(nodeClass &r){
        return secondaryIntervalTree.remove(&r);
    }
    virtual vector<nodeClass> *getAll(vector<nodeClass> *allList){
        return secondaryIntervalTree.getAll(allList);
    }
    virtual void print(ostream &os){
        os << "FirstSecondaryStructure.print()" << endl;
        os << secondaryIntervalTree;
    }
    FirstSecondaryStructure3() : secondaryIntervalTree(&IntervalTree3<numericalValueType, nodeClass, SecondSecondaryStructure3<numericalValueType, nodeClass, nodeClassSet> >::secondary_interval_dimension){
    }
    ~FirstSecondaryStructure3(){
    }
};




//, &Rectangle2d::minx_get, &Rectangle2d::maxx_get
//, &Rectangle2i::minx_get, &Rectangle2i::maxx_get
typedef FirstSecondaryStructure2<double, Rectangle2d, Rectangles2D> FirstSecondaryStructure2D;
typedef FirstSecondaryStructure2<int, Rectangle2i, Rectangles2I> FirstSecondaryStructure2I;
typedef FirstSecondaryStructure2<int, FullRectangle2i, FullRectangles2I> FirstFullSecondaryStructure2I;

typedef IntervalTree2<double, Rectangle2d, FirstSecondaryStructure2D> IntervalTree2d;
typedef IntervalTree2<int, Rectangle2i, FirstSecondaryStructure2I> IntervalTree2i;
typedef IntervalTree2<int, FullRectangle2i, FirstFullSecondaryStructure2I> IntervalTree2if;

typedef FirstSecondaryStructure3<double, Rectangle3d, Rectangles3D> FirstSecondaryStructure3D;
typedef IntervalTree3<double, Rectangle3d, FirstSecondaryStructure3D> IntervalTree3d;

typedef FirstSecondaryStructure3<double, FullRectangle3d, FullRectangles3D> FirstSecondaryStructure3DFull;
typedef IntervalTree3<double, FullRectangle3d, FirstSecondaryStructure3DFull> IntervalTree3dFull;

#endif
