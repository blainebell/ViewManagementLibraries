
#include "Plane.h"
#include "BSPObjectPart.h"
#include <cmath>
#include "stlTools.h"

int BSPObjectPart::getNumber(){
    int num = 0;
    for (auto it = bspObject->allParts.begin(); it!=bspObject->allParts.end(); it++){
        if ((*it) == this){
            break;
        }
        num++;
    }
    return num;
}

int BSPObjectPart::countNumberOfLines(){
    int nlines = 0;
    for (auto it = allFaces->begin(); it!=allFaces->end(); it++){
        auto face = *it;
        int nvert = face->numVertices();
        switch (nvert){
            case 1: break;
            case 2: nlines += 3; break;
            default:
                if (nvert > 0)
                    nlines += nvert + 2;
                break;
        }
    }
    return nlines;
}

void BSPObjectPart::writeLines(float *fptr, int &off){
    for (auto it = allFaces->begin(); it!=allFaces->end(); it++){
        auto face = *it;
        face->writeLines(fptr, off);
    }
    return;
}

BSPObjectPart::BSPObjectPart(BSPObject *obj, vector<Face*> *faces) : bspObject(obj), allFaces(faces), bbx() {
    computeBBX();
    obj->allParts.insert(this);
}

BSPObjectPart::~BSPObjectPart(){
    bspObject->allParts.erase(this);
    for (auto it=allFaces->begin(); it!=allFaces->end();it++){
        delete *it;
    }
    delete allFaces;
}

void BSPObjectPart::computeBBX(){
    bbx.set(MAXFLOAT, MAXFLOAT, MAXFLOAT, -MAXFLOAT, -MAXFLOAT, -MAXFLOAT);
    for (auto it=allFaces->begin();it!=allFaces->end(); it++){
        Face *face = *it;
        Vertex *vhead = face->vhead, *tmp = face->vhead;
        do {
            bbx.add(tmp->x,tmp->y, tmp->z);
            tmp = tmp->next;
        } while (tmp && vhead!=tmp);
    }
}

void partitionFacesWithPlane(Plane *plane, vector<Face *> *faceList, vector<Face *> *facePos, vector<Face *> *faceNeg,
                             vector<Face *> *faceSameDir, vector<Face *> *faceOppDir){
    Face *ftrav = NULL;
//    cout << "partitionFacesWithPlane: faceList.size=" << faceList->size() << endl;
    auto it = faceList->begin();
//    int i=0;
    while (it!=faceList->end()){
        ftrav=(Face*)(*it);
        it++;
//        i++;
//        cout << "   i=" << i << " : ftrav: plane=" << *ftrav->plane << " : " << *ftrav << endl;
        // find first intersection
        //	    System.out.println("findIntersectionsOfPlane: ftrav=" + ftrav + " plane=" + plane);
        vector<Intersection*> *intersections = ftrav->findIntersectionsOfPlane(plane);
        
        Intersection *i1=NULL, *i2=NULL;
        if (intersections->size()==2){
            i1 = (Intersection*)intersections->at(0);
            i2 = (Intersection*)intersections->at(1);
        }
        delete intersections;
        bool nointersection = false;
        if (i1!=NULL && i2 != NULL){
            Face *newOtherFace;
            newOtherFace = ftrav->createOtherFace(i1,i2);
            if (newOtherFace->numVertices()<3 || !newOtherFace->has_area()){
                delete newOtherFace;
                if (ftrav->numVertices()<3){
                    nointersection = true;
                }
            } else if (ftrav->numVertices()<3){// || !ftrav->has_area()){
                //delete ftrav;  should ftrav be deleted from faceList?
                //faceList->erase(it-1);
                ftrav = newOtherFace;
                nointersection = true;
            } else {
                if (i1->sign == NEGATIVE){
                    faceNeg->insert(faceNeg->begin(), ftrav);
                    facePos->insert(facePos->begin(), newOtherFace);
                } else {
                    faceNeg->insert(faceNeg->begin(), newOtherFace);
                    facePos->insert(facePos->begin(), ftrav);
                }
            }
            //		System.out.println("inserted faces into faceNeg and facePos");
        } else {
            nointersection = true;
        }
        if (i1) delete i1;
        if (i2) delete i2;
        if (nointersection){
            // no intersection
            int side = plane->whichSideIsFace(*ftrav);
            if (side==NEGATIVE){
                faceNeg->insert(faceNeg->begin(), ftrav);
            } else if (side==POSITIVE){
                facePos->insert(facePos->begin(), ftrav);
            } else {
                if (doubleIsEqual(ftrav->plane->x, plane->x) &&
                    doubleIsEqual(ftrav->plane->y, plane->y) &&
                    doubleIsEqual(ftrav->plane->z, plane->z)){
                    faceSameDir->insert(faceSameDir->begin(), ftrav);
                } else {
                    faceOppDir->insert(faceOppDir->begin(), ftrav);
                }
            }
        }
    }
}

bool BSPObjectPart::split(Plane *plane,
                          vector<BSPObjectPart*> *objectpartNeg, vector<BSPObjectPart*> *objectpartPos,
                          vector<BSPObjectPart*> *objectpartSameDir, vector<BSPObjectPart*> *objectpartOppDir){
    {
        // part spans the plane, need to split
        vector<Face*> *frontFaces = new vector<Face*>(),
        *backFaces = new vector<Face*>(),
        *sameFaces = new vector<Face*>(),
        *oppFaces = new vector<Face*>();
        partitionFacesWithPlane(plane, allFaces, frontFaces, backFaces, sameFaces, oppFaces);
        bool ssdebug = false;
        stringstream ss;
        if (ssdebug)
            ss << "partitionFacesWithPlane: plane=" << *plane << " allFaces.size=" << allFaces->size() << " front=" << frontFaces->size() << " back=" << backFaces->size() << " sameFaces=" << sameFaces->size() << " oppFaces=" << oppFaces->size() << endl;
        allFaces->clear();
        if (!backFaces->empty()){
            if (ssdebug){
                ss << "back: " << faceListToStringDirect(*backFaces) << endl;
                ss << "back planes: " << faceListToPlanesStringDirect(*backFaces) << endl;
                
            }
            objectpartNeg->push_back(new BSPObjectPart(bspObject, backFaces));
        } else {
            delete backFaces;
        }
        if (!frontFaces->empty()){
            if (ssdebug){
                ss << "front: " << faceListToStringDirect(*frontFaces) << endl;
                ss << "front planes: " << faceListToPlanesStringDirect(*frontFaces) << endl;
            }
            objectpartPos->push_back(new BSPObjectPart(bspObject, frontFaces));
        } else {
            delete frontFaces;
        }
        if (!sameFaces->empty()){
            if (ssdebug){
                ss << "same: " << faceListToStringDirect(*sameFaces) << endl;
                ss << "same planes: " << faceListToPlanesStringDirect(*sameFaces) << endl;
            }
            objectpartSameDir->push_back(new BSPObjectPart(bspObject, sameFaces));
        } else {
            delete sameFaces;
        }
        if (!oppFaces->empty()){
            if (ssdebug){
                ss << "opp: " << faceListToStringDirect(*oppFaces) << endl;;
                ss << "opp planes: " << faceListToPlanesStringDirect(*oppFaces) << endl;;
            }
            objectpartOppDir->push_back(new BSPObjectPart(bspObject, oppFaces));
        } else {
            delete oppFaces;
        }
        allFaces->clear(); // all faces have been transfered to these new BSPObjectParts
        if (ssdebug)
	  cout << ss.str().c_str();
    }
    return false;
}

ostream& operator <<(ostream &os,BSPObjectPart &obj){
    os << "bspid=" << obj.bspObject->_id << " bbx=" << obj.bbx << " #faces=" << obj.allFaces->size() << " ";
    return os;
}
