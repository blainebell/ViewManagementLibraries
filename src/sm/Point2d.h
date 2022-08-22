

#ifndef CPP_POINT2D_H
#define CPP_POINT2D_H

#include <iostream>

using namespace std;

class Point2d {
public:
    double x, y;
    Point2d();
    Point2d(float *);
    Point2d(double *);
    Point2d(double,double);
    void set(double _x, double _y){ x=_x; y=_y; }
    friend ostream& operator <<(ostream &os, Point2d &obj);
    friend ostream& operator <<(ostream &os, const Point2d &obj);
    bool equals(const Point2d &);
    void add(Point2d&);
    void normalize();
    double length();
    void writeTo(float*) const;
    Point2d& operator+=(const Point2d& rhs);
    friend Point2d operator+(const Point2d &p1, const Point2d &p2);
    friend Point2d operator-(const Point2d &p1, const Point2d &p2);
    friend Point2d operator*(const Point2d &p1, const Point2d &p2);
    friend Point2d operator*(const int &i1, const Point2d &p2);
    friend Point2d operator/(const Point2d &p1, const double num);
    friend float dot_product(const Point2d&,const Point2d&);
    friend float dot_product(Point2d&,Point2d&);
    friend float angle(Point2d &p1, Point2d &p2);
    friend float angle_deg(Point2d &p1, Point2d &p2);
};

void clipToUnit(Point2d &tmpProj, const Point2d &tmpProj2);

bool clipToUnit0(Point2d &tmpProj, const Point2d &tmpProj2);

bool linesIntersect(double x1, double y1,
                    double x2, double y2,
                    double x3, double y3,
                    double x4, double y4);

bool linesIntersect(Point2d &pt1, Point2d &pt2, Point2d &pt3, Point2d &pt4);

#endif
