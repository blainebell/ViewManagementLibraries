

#include "Face.h"
#include "stlTools.h"
#include "Plane.h"
#define _USE_MATH_DEFINES
#include <math.h>

Face::Face(Vertex *v) : vhead(v), plane(NULL) {
    if (vhead)
        computePlane();
}
Face::Face(const vector<float> &arr) : vhead(NULL), plane(NULL) {
    auto sz = arr.size();
    if (sz > 0 && sz % 3){
        cerr << "Face created with incorrect # of floats arr.len=" << arr.size() << endl;
        return;
    }
    auto it = arr.begin();
    vhead = new Vertex(*(it++), *(it++), *(it++));
    Vertex *vtemp = vhead;
    while ( it!=arr.end()){
        vtemp->next = new Vertex(*(it++), *(it++), *(it++));
        vtemp = vtemp->next;
    }
    vtemp->next = vhead;
    computePlane();
}

Face::~Face(){
    if (plane) delete plane;
    if (vhead){
        Vertex *origvhead = vhead, *vtempnext = vhead, *vtemp;
        do {
            vtemp = vtempnext;
            vtempnext = vtemp->next;
            delete vtemp;
        } while (vtempnext != origvhead);
    }
}

int Face::numVertices(){
    Vertex *head = vhead;
    int ret=0;
    if (head==NULL){
        return (ret);
    }
    Vertex *tmp = head;
    do {
        ret++;
        tmp = tmp->getNext();
    } while (tmp!=head);
    return (ret);
}

void Face::writeLines(float *fdata, int &off){
    Vertex *head = vhead;
    if (head==NULL){
        return;
    }
    Vertex *tmp = head, *prev = head;
    do {
        fdata[off++] = tmp->x; fdata[off++] = tmp->y; fdata[off++] = tmp->z;
        tmp = tmp->getNext();
//        fdata[off++] = prev->x; fdata[off++] = prev->y; fdata[off++] = prev->z;
//        fdata[off++] = tmp->x; fdata[off++] = tmp->y; fdata[off++] = tmp->z;
        prev = tmp;
    } while (tmp!=head);
    fdata[off++] = tmp->x; fdata[off++] = tmp->y; fdata[off++] = tmp->z;
    fdata[off++] = tmp->x; fdata[off++] = tmp->y; fdata[off++] = tmp->z;   // IS THIS RIGHT?
    return;
}

void Face::setVertices(Vertex *v){
    vhead = v;
    computePlane();
}

bool Face::computePlane(Vertex *vh, Vertex *end){
    Vertex *v1, *v2, *v3;
    v1 = vh;
    if (v1==NULL) return false;
    v2 = v1->next;
    if (v2==NULL || v2==v1) return false;
    v3 = v2->next;
    if (v3==NULL || v3==v1) return false;

    Point3d vect1, vect2, result;
    vect1.set(v2->x-v1->x, v2->y-v1->y, v2->z-v1->z);
    vect2.set(v3->x-v1->x, v3->y-v1->y, v3->z-v1->z);
    result.cross(vect1, vect2);
    result.normalize();
    double w = result.x * v1->x + result.y * v1->y + result.z * v1->z;
    if (vh->next != end &&
        (::isNaN(result.x) ||
         ::isNaN(result.y) ||
         ::isNaN(result.z))){
            return (computePlane(vh->next,end));
        }
    plane = new Plane(result.x,result.y,result.z,-w);
    return (true);
}

bool Face::straddlesPlane(Plane *planeInQuestion){
    bool anyNegative = false, anyPositive = false;
    Vertex *vtrav = NULL;
    double value;
    /* for all vertices... */
    vtrav = vhead;
    do {
        value = planeInQuestion->x * vtrav->x +
                planeInQuestion->y * vtrav->y +
                planeInQuestion->z * vtrav->z + planeInQuestion->w;
        /* check which side vertex is on relative to plane */
        if (::fabs(value) < TOLER){
            vtrav = vtrav->next;
            continue;
        } else if (value<0.f){
            anyNegative = true;
        } else if (value > 0.f){
            anyPositive = true;
        }
        /* if vertices on both sides of plane then face straddles else it no */
        if (anyNegative && anyPositive) return true;
        vtrav = vtrav->next;
    } while (vtrav != vhead);
    return false;
}

vector<Intersection*> *Face::findIntersectionsOfPlane(Plane *planeInQuestion){
    vector<Intersection*> *ret = new vector<Intersection*>();
    if (vhead==NULL){
        return (ret);
    }
    Vertex *v1, *v2;
    Intersection *intersection=NULL;
    v1 = vhead;
    v2 = vhead->next;
    do {
        intersection = planeInQuestion->anyEdgeIntersectWithPlane( *v1, *v2);
        if (intersection!=NULL){
            ret->push_back(intersection);
        }
        if (ret->size()==2){
            return (ret);
        }
        v1 = v2;
        v2 = v2->next;
    } while (v1!=vhead);
    return (ret);
}

Face *Face::createOtherFace(Intersection *i1, Intersection *i2){
    /** This object is the original face which needs to change
     * its vertices as well by adding the 2 new intersected ones,
     * and deleting ones that are on the other side of the plane */
    
    Vertex *v1 = i1->vertex1; /* first vertices of both intersections */
    Vertex *v2 = i2->vertex1;
    Vertex *i1p1, *i2p1; /* new vertices for original face */
    Vertex *i1p2, *i2p2; /* new vertices for new face */
    
    Vertex *vtemp;    /* place holders */
    Vertex *beforeV2; /* place holders */
    Face *newFace;
    
    i1p1 = i1->intersectionVertex;
    i2p1 = i2->intersectionVertex;
    i1p2 = new Vertex(i1p1);
    i2p2 = new Vertex(i2p1);

    vtemp = v1->next;
    if (vtemp->equals(i1p1)){
        vtemp = vtemp->next;
    }
    /* merge both intersection vertices i1p1 & i2p1 into 1st list */
    if (i2p1->equals(v2->next)){ /* intersection vertex coincident? */
        /* if it is, delete it */
        i1p1->next = v2->next;
    } else {
        i2p1->next = v2->next; /* attach intersection list onto 1st list */
        i1p1->next = i2p1; /* attach both intersection vertices */
    }
    v1->next = i1p1; /* attach front of 1st list to intersection vertices */
    
    /* merge intersection vertices i1p2, i2p2 & p2end into second list */
    i2p2->next = i1p2;		/* attach both intersection vertices */
    v2->next = i2p2;		/* attach 2nd list to interection vertices */
    if (vtemp==v2) {
        i1p2->next = v2;	/* close up 2nd list */
    } else {
        if (i1p2->equals(vtemp)) { /* intersection vertex coincident? */
            i2p2->next = vtemp;	/* attach intersection vertex to 2nd list */
        } else {
            i1p2->next = vtemp;	/* attach intersection list to 2nd list */
        }
        /* find previous vertex to v2 */
        for (beforeV2 = vtemp; beforeV2->next != v2; beforeV2 = beforeV2->next)
            ;
        beforeV2->next = v2;	/* and attach it to complete the 2nd list */
    }

    /* copy original face info but with new vertex list */
    newFace = new Face(v2);
    if (!newFace->getPlane()->equals(*getPlane())){
        cout << "Warning:: createOtherFace: newFace.plane=" << newFace->getPlane() << " this.plane=" << getPlane() << endl;
    }
    return(newFace);
}
Face *Face::copy(){
    Face *newFace = new Face();
    if (vhead){
        Vertex *vnewtemp = newFace->vhead = new Vertex(vhead);
        Vertex *vtemp = vhead;
        while (vtemp->next!=vhead) {
            vnewtemp->next = new Vertex(vtemp->next);
            vtemp = vtemp->next;
            vnewtemp = vnewtemp->next;
        }
        vnewtemp->next = newFace->vhead;
    }
    if (plane)
        newFace->plane = new Plane(*plane);
    return newFace;
}

ostream& operator <<(ostream &os, Face &obj){
    os << obj.toString();
    return os;
}
const string Face::commaDelString(){
    stringstream ss;
    Vertex *h=getHead(), *n;
    n=h;
    do {
        ss << n->toString();
        n = n->getNext();
        if (n!=h)
            ss << ",";
    } while (n!=h);
    return ss.str();
}
const string Face::toString(){
    stringstream ss;
    Vertex *h=getHead(), *n;
    n=h;
    do {
        ss << " " << n->toString() << " ";
        n = n->getNext();
    } while (n!=h);
    return ss.str();
}

float Face::area(){
    if (numVertices() < 3){
        return 0.f;
    }
    Point3d total, prod;
    Vertex *h=getHead(), *vi2, *vi1;
    vi2 = h;
    do {
        vi1 = vi2;
        vi2 = vi2->getNext();
        prod.cross(*vi1,*vi2);
        total += prod;
    } while (vi2!=h);
    return dot_product(total, *plane) / 2.f;
}

bool Face::has_area(){
    int numv = numVertices();
    if (numv < 3){
        return false;
    }
    int nedges_have_length = 0;
    Vertex *h=getHead(), *vi2, *vi1;
    vi2 = h;
    Point3d dist;
    do {
        vi1 = vi2;
        vi2 = vi2->getNext();
        dist = (*vi2) - (*vi1);
        if (((float)dist.length()) > 2 * PLANE_SPLIT){
            nedges_have_length++;
        }
    } while (vi2!=h);
    return nedges_have_length > 2;
}

bool Face::pointIsInside(const Point3d &pt){
//    cout << " Face::pointIsInside called pt=" << pt << endl;
    Vertex *vi1, *vi2;
    double m1, m2, m1x2;
    Vertex *h=getHead();

//    int edgenum = 0;
    double anglesum = 0, costheta = 0;
    //Point3d tmpPT = *h - pt;
    Point3d tmpPt1, tmpPt2 = *h - pt;
    m2 = sqrt(tmpPt2.x*tmpPt2.x + tmpPt2.y*tmpPt2.y + tmpPt2.z*tmpPt2.z);
    vi2 = h;
    do {
        vi1 = vi2;
        vi2 = vi2->getNext();
        tmpPt1 = tmpPt2;
        tmpPt2 = *vi2 - pt;
        m1 = m2;
        m2 = sqrt(tmpPt2.x*tmpPt2.x + tmpPt2.y*tmpPt2.y + tmpPt2.z*tmpPt2.z);
        m1x2 = m1 * m2;
        if (m1x2 < TOLER)
            return true; // point is on vertex and therefore inside
        costheta = (tmpPt1.x*tmpPt2.x + tmpPt1.y*tmpPt2.y + tmpPt1.z*tmpPt2.z)/m1x2;
        anglesum += acos(costheta);
//        cout << " edge#" << edgenum << " vi1=" << *vi1 << " vi2=" << *vi2 << " costheta=" << costheta << " acos=" << acos(costheta) << " anglesum=" << anglesum << endl;
//        edgenum++;
    } while (vi2!=h);
//    cout << "anglesum=" << anglesum << " (anglesum - 2 * M_PI)=" << (anglesum - 2 * M_PI) << " returning " << (fabs(anglesum - 2 * M_PI) < TOLER) << endl;
    return fabs(anglesum - 2 * M_PI) < TOLER;
}

double Face::pointIsInsideVal(const Point3d &pt){
    //    cout << " Face::pointIsInside called pt=" << pt << endl;
    Vertex *vi1, *vi2;
    double m1, m2, m1x2;
    Vertex *h=getHead();
    
    int edgenum = 0;
    double anglesum = 0, costheta = 0;
    //Point3d tmpPT = *h - pt;
    Point3d tmpPt1, tmpPt2 = *h - pt;
    m2 = sqrt(tmpPt2.x*tmpPt2.x + tmpPt2.y*tmpPt2.y + tmpPt2.z*tmpPt2.z);
    vi2 = h;
    do {
        vi1 = vi2;
        vi2 = vi2->getNext();
        tmpPt1 = tmpPt2;
        tmpPt2 = *vi2 - pt;
        m1 = m2;
        m2 = sqrt(tmpPt2.x*tmpPt2.x + tmpPt2.y*tmpPt2.y + tmpPt2.z*tmpPt2.z);
        m1x2 = m1 * m2;
        if (m1x2 < TOLER)
            return true; // point is on vertex and therefore inside
        costheta = (tmpPt1.x*tmpPt2.x + tmpPt1.y*tmpPt2.y + tmpPt1.z*tmpPt2.z)/m1x2;
        anglesum += acos(costheta);
        //        cout << " edge#" << edgenum << " vi1=" << *vi1 << " vi2=" << *vi2 << " costheta=" << costheta << " acos=" << acos(costheta) << " anglesum=" << anglesum << endl;
        edgenum++;
    } while (vi2!=h);
    //    cout << "anglesum=" << anglesum << " (anglesum - 2 * M_PI)=" << (anglesum - 2 * M_PI) << " returning " << (fabs(anglesum - 2 * M_PI) < TOLER) << endl;
    return fabs(anglesum - 2 * M_PI);
}

vector<Face*> *duplicateFaceList(vector<Face*> *origList){
    vector<Face*> *newList = new vector<Face*>();
    if (origList){
        for (auto it=origList->begin(); it!=origList->end(); it++){
            newList->push_back((*it)->copy());
        }
    }
    return newList;
}
ostream& operator <<(ostream &os, vector<Face*> &obj){
    int fn = 0;
    for (auto it=obj.begin(); it!=obj.end(); it++){
        os << "  face#" << fn << " : plane: " << *(*it)->plane << " : area=" << (*it)->area() << " : " << *(*it) << endl;
//        os << "  face#" << fn << " : " << *(*it) << endl;
        fn++;
    }
    return os;
}

string faceListToString(vector<Face*> &obj){
    stringstream ss;
    int fn = 0;
    for (auto it=obj.begin(); it!=obj.end(); it++){
        ss << "  face#" << fn << " : plane: " <<  *(*it)->plane << " : area=" << (*it)->area() << " : " << *(*it) << endl;
//        os << "  face#" << fn << " : " << *(*it) << endl;
//        ss << "  face#" << fn << " : plane: " << *(*it)->plane << *(*it) << endl;
        fn++;
    }
    return ss.str();
}
string faceListToStringDirect(vector<Face*> &obj){
    stringstream ss;
    int fn = 0;
    auto it=obj.begin();
    bool cont = it!=obj.end();
    while ( cont){
        ss << "[" << *(*it) << "]";
        fn++;
        it++;
        cont = it!=obj.end();
        if (cont) ss << " ";
    }
    return ss.str();
}

string faceListToPlanesStringDirect(vector<Face*> &obj){
    stringstream ss;
    int fn = 0;
    auto it=obj.begin();
    bool cont = it!=obj.end();
    while ( cont){
        ss << "(" << *(*it)->plane << ")";
        fn++;
        it++;
        cont = it!=obj.end();
        if (cont) ss << " ";
    }
    return ss.str();
}
