
#include "Plane.h"

#include "stlTools.h"
#include <cmath>
#include <cstdlib>
#include "Face.h"


Plane::Plane() : Point3d(), w(0.) {

}

Plane::Plane(double _x, double _y, double _z, double _w) : Point3d(_x, _y, _z), w(_w) {
    
}
Plane::Plane(const Plane &p) : Point3d(p.x, p.y, p.z), w(p.w) {
    
}

Plane::Plane(Point3d *vect1, Point3d *vect2, Point3d *pt){
    Point3d result;
    result.cross(*vect1, *vect2);
    result.normalize();
    double w = result.x * pt->x + result.y * pt->y + result.z * pt->z;
    if (::isNaN(result.x) ||
         ::isNaN(result.y) ||
         ::isNaN(result.z)){
            return ;
    }
    x = result.x;
    y = result.y;
    z = result.z;
    w = -w;
}

void computePlane(Plane &p, Point3d *vect1, Point3d *vect2, Point3d *pt){
    Point3d result;
    result.cross(*vect1, *vect2);
    result.normalize();
    double w = result.x * pt->x + result.y * pt->y + result.z * pt->z;
    if (::isNaN(result.x) ||
        ::isNaN(result.y) ||
        ::isNaN(result.z)){
        return ;
    }
    p.x = result.x;
    p.y = result.y;
    p.z = result.z;
    p.w = -w;
}

bool Plane::isNaN(){
    return ::isNaN(x) || ::isNaN(y) || ::isNaN(z) || ::isNaN(w);
}
double Plane::findIntersection(const Point3d &pt, const Point3d &vect, Point3d &result){
    double denom = x*vect.x + y*vect.y + z*vect.z ;
    double tt = - ((x*pt.x) + (y*pt.y) + (z*pt.z) + w) / denom ;
    result.set(pt.x + (tt*vect.x),
               pt.y + (tt*vect.y),
               pt.z + (tt*vect.z));
    return tt;
}

Intersection* Plane::anyEdgeIntersectWithPlane(Vertex &v1, Vertex &v2){
    double temp1, temp2;
    int sign1, sign2;
    temp1 = ((v1.x*x) + (v1.y*y) + (v1.z*z) + w);
    sign1 = computeSign(temp1);
    if (sign1==ZERO){
        return NULL;
    }
    temp2 = ((v2.x*x) + (v2.y*y) + (v2.z*z) + w);
    sign2 = computeSign(temp2);
    if (sign2==ZERO){
        return(new Intersection(((sign1 == NEGATIVE)? NEGATIVE : POSITIVE), &v1, &v2, &v2));
    }
    if (sign1 != sign2){
        double dx, dy, dz, denom, tt;
        dx = v2.x - v1.x;
        dy = v2.y - v1.y;
        dz = v2.z - v1.z;
        denom = (x*dx) + (y*dy) + (z*dz);
        tt = - ((x*v1.x) + (y*v1.y) + (z*v1.z) + w)/denom;
        Vertex *newIntersection = new Vertex((double)(v1.x + (tt*dx)),
                                            (double)(v1.y + (tt*dy)),
                                            (double)(v1.z + (tt*dz)));
        return (new Intersection(((sign1 == NEGATIVE)? NEGATIVE : POSITIVE), &v1, &v2, newIntersection));
    }
    return NULL;
}


Point3d *Plane::anyLineIntersectWithPlane(Point3d &v1, Point3d &line){
    double denom, tt; // dx, dy, dz is the line
    denom = (x*line.x) + (y*line.y) + (z*line.z);
    if (::isNaN(denom) || ::isInfinity(denom) || ::fabs(denom) < TOLER){
        return (NULL);
    }
    tt = - ((x*v1.x) + (y*v1.y) + (z*v1.z) + w)/denom;
    Point3d *newIntersection = new Point3d((double)(v1.x + (tt*line.x)),
                                          (double)(v1.y + (tt*line.y)),
                                          (double)(v1.z + (tt*line.z)));
    return (newIntersection);
}

double Plane::distanceFromPoint(const Point3d &point){
    return(( (x*point.x) + (y*point.y) + (z*point.z) + w )/ ::sqrt(x*x + y*y + z*z));
}

int Plane::whichSideIsPoint(const Point3d &point){
    double value = (x*point.x) + (y*point.y) + (z*point.z) + w;
    if (value < -TOLER){
        return (NEGATIVE);
    } else if (value > TOLER){
        return (POSITIVE);
    }
    return (ZERO);
}

int Plane::whichSideIsPoint(const double _x, const double _y, const double _z){
    double value = (x*_x) + (y*_y) + (z*_z) + w;
    if (value < -TOLER){
        return (NEGATIVE);
    } else if (value > TOLER){
        return (POSITIVE);
    }
    return (ZERO);
}

bool Plane::whichSideIsPointOrZero(int &side, const double _x, const double _y, const double _z){
    int wside = whichSideIsPoint(_x, _y, _z);
    if (!side){
        side = wside;
        return true;
    } else {
        return !wside || wside == side;
    }
}

bool Plane::isOnOneSide(Rectangle3d &rect, int &side){
    side = 0;
    return whichSideIsPointOrZero(side, rect.minx, rect.miny, rect.minz) &&
           whichSideIsPointOrZero(side, rect.minx, rect.miny, rect.maxz) &&
           whichSideIsPointOrZero(side, rect.minx, rect.maxy, rect.minz) &&
           whichSideIsPointOrZero(side, rect.minx, rect.maxy, rect.maxz) &&
           whichSideIsPointOrZero(side, rect.maxx, rect.miny, rect.minz) &&
           whichSideIsPointOrZero(side, rect.maxx, rect.miny, rect.maxz) &&
           whichSideIsPointOrZero(side, rect.maxx, rect.maxy, rect.minz) &&
           whichSideIsPointOrZero(side, rect.maxx, rect.maxy, rect.maxz);
}
/*
bool Plane::isOnOneSide(Rectangle3d &rect, int &side){
    side = 0;
    return whichSideIsPointOrZero(side, rect.minx, rect.miny, rect.minz) &&
    whichSideIsPointOrZero(side, rect.minx, rect.miny, rect.maxz) &&
    whichSideIsPointOrZero(side, rect.minx, rect.maxy, rect.minz) &&
    whichSideIsPointOrZero(side, rect.minx, rect.maxy, rect.maxz) &&
    whichSideIsPointOrZero(side, rect.maxx, rect.miny, rect.minz) &&
    whichSideIsPointOrZero(side, rect.maxx, rect.miny, rect.maxz) &&
    whichSideIsPointOrZero(side, rect.maxx, rect.maxy, rect.minz) &&
    whichSideIsPointOrZero(side, rect.maxx, rect.maxy, rect.maxz);
}*/

int Plane::whichSideIsFace(const Face &face){
    Vertex *vtrav;
    double value=0;
    bool isNeg=false, isPos=false;

    if (face.getPlane()!=NULL && face.getPlane()->equals(*this)){
        return(ZERO);
    }
    vtrav = face.getHead();
    do {
        value= (x * vtrav->x) + (y * vtrav->y) + (z * vtrav->z) + w;
        if (value < -TOLER)
            isNeg= true;
        else if (value > TOLER)
            isPos= true;
        vtrav= vtrav->next;
    } while (vtrav != face.getHead());
    
    /* in the very rare case that some vertices slipped thru to other side of
     * plane due to round-off errors, execute the above again but count the
     * vertices on each side instead and pick the maximum.
     */
    if (isNeg && isPos) {	/* yes so handle this numerical problem */
        int countNeg, countPos;
        /* count how many vertices are on either side */
        countNeg= countPos= 0;
        for (vtrav= face.getHead(); vtrav->next != face.getHead();
             vtrav= vtrav->next) {
            value= (x*vtrav->x) + (y*vtrav->y) + (z*vtrav->z) + w;
            if (value < -TOLER)
                countNeg++;
            else if (value > TOLER)
                countPos++;
        } /* for */
        /* return the side corresponding to the maximum */
        if (countNeg > countPos) return(NEGATIVE);
        else if (countPos > countNeg) return(POSITIVE);
        else return(ZERO);
    }
    else {			/* this is the usual case */
        if (isNeg) return(NEGATIVE);
        else if (isPos) return(POSITIVE);
        return(ZERO);
    }
}

bool Plane::isPointOnFrontSide(const Point3d &point){
    double dp = x*point.x + y*point.y + z*point.z + w;
    return (dp>0.0);
}

bool Plane::equals(const Plane &plane){
    return doubleIsEqual(plane.x, x ) &&
            doubleIsEqual(plane.y, y ) &&
            doubleIsEqual(plane.z, z ) &&
            doubleIsEqual(plane.w, w );
}



const string Plane::toString(){
    stringstream ss;
    ss << "( " << x << ", " << y << ", " << z << ", " << w << " )";;
    return ss.str();
}

ostream& operator <<(ostream &os, Plane &obj){
    os << "( " << obj.x << ", " << obj.y << ", " << obj.z << ", " << obj.w << " )";;
    return os;
}
