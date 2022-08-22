


#ifndef CPP_PLANE_H
#define CPP_PLANE_H

#include <iostream>

using namespace std;

#include "Intersection.h"
#include "Point3d.h"
#include "Rectangle3.h"

class Face;

class Plane : public Point3d {
public:
    double w;
    Plane();
    Plane(const Plane &);
    Plane(double, double, double, double);
    Plane(Point3d *vect1, Point3d *vect2, Point3d *pt);
    bool isNaN();
    Intersection* anyEdgeIntersectWithPlane(Vertex &v1, Vertex &v2);
    Point3d* anyLineIntersectWithPlane(Point3d &v1, Point3d &line);
    double findIntersection(const Point3d &, const Point3d &, Point3d &);
    double distanceFromPoint(const Point3d &point);
    int whichSideIsPoint(const Point3d &point);
    int whichSideIsPoint(const double x, const double y, const double z);
    bool whichSideIsPointOrZero(int &side, const double _x, const double _y, const double _z);
    int whichSideIsFace(const Face &face);
    bool isPointOnFrontSide(const Point3d &point);
    bool isOnOneSide(Rectangle3d &, int &side);
    bool equals(const Plane &plane);
    const string toString();
    friend ostream& operator <<(ostream &os, Plane &obj);    
};

ostream& operator <<(ostream &os, Plane &obj);

void computePlane(Plane &p, Point3d *vect1, Point3d *vect2, Point3d *pt);

#endif
