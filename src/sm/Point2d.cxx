

#include "Point2d.h"
#include "stlTools.h"
#define _USE_MATH_DEFINES
#include <math.h>

Point2d::Point2d(){
    
}

Point2d::Point2d(float *pt) : x(pt[0]), y(pt[1]) {
}
Point2d::Point2d(double *pt) : x(pt[0]), y(pt[1]) {
}

Point2d::Point2d(double _x, double _y) : x(_x), y(_y) {
}

bool Point2d::equals(const Point2d &pt){
    return doubleIsEqual(pt.x, x ) &&
            doubleIsEqual(pt.y, y );
}


void Point2d::add(Point2d &pt){
    x += pt.x;
    y += pt.y;
}

double Point2d::length(){
    return sqrt(x*x + y*y);
}

void Point2d::writeTo(float *fptr) const {
    fptr[0] = x;
    fptr[1] = y;
}
void Point2d::normalize(){
    double len = sqrt(x*x + y*y);
    x = x/len;
    y = y/len;
}

ostream& operator <<(ostream &os, Point2d &obj){
    os << "( " << obj.x << ", " << obj.y << " ) " ;
    return os;
}
ostream& operator <<(ostream &os, const Point2d &obj){
    os << "( " << obj.x << ", " << obj.y << " ) " ;
    return os;
}

Point2d operator+(const Point2d &p1, const Point2d &p2){
    return Point2d(p1.x+p2.x, p1.y+p2.y );
}

Point2d operator-(const Point2d &p1, const Point2d &p2){
    return Point2d(p1.x-p2.x, p1.y-p2.y );
}

Point2d operator*(const Point2d &p1, const Point2d &p2){
    return Point2d(p1.x*p2.x, p1.y*p2.y );
}

Point2d operator/(const Point2d &p1, const double num){
    return Point2d(p1.x/num, p1.y/num);
}
Point2d operator*(const int &i1, const Point2d &p2){
    return Point2d(i1 * p2.x, i1 * p2.y);
}

Point2d& Point2d::operator+=(const Point2d& rhs){
    x += rhs.x;
    y += rhs.y;
    return *this;
}

float dot_product(const Point2d &p1, const Point2d &p2){
    return p1.x * p2.x + p1.y * p2.y;
}
float dot_product(Point2d &p1, Point2d &p2){
    return p1.x * p2.x + p1.y * p2.y;
}

float angle(Point2d &p1, Point2d &p2){
    double vDot = dot_product(p1, p2) / (p1.length() * p2.length());
    if( vDot < -1.0) vDot = -1.0;
    if( vDot >  1.0) vDot =  1.0;
    return((double) (acos( vDot )));
}

float angle_deg(Point2d &p1, Point2d &p2){
    float rad = angle(p1, p2);
    return rad * 180.f / M_PI;
}

void clipToUnit(Point2d &tmpProj, const Point2d &tmpProj2) {
    if (tmpProj.x < -1.f){
        tmpProj.y = tmpProj.y + (tmpProj2.y-tmpProj.y) * (-1.f - tmpProj.x) / (tmpProj2.x-tmpProj.x); //y = y0 + (y1 - y0) * (xmin - x0) / (x1 - x0);
        tmpProj.x = -1.f;                  //x = xmin;
    } else if (tmpProj.x > 1.f){
        tmpProj.y = tmpProj.y + (tmpProj2.y-tmpProj.y) * (1.f - tmpProj.x) / (tmpProj2.x-tmpProj.x);  //y = y0 + (y1 - y0) * (xmax - x0) / (x1 - x0);
        tmpProj.x = 1.f; //x = xmax;
    }
    if (tmpProj.y < -1.f){
        tmpProj.x = tmpProj.x + (tmpProj2.x-tmpProj.x) * (-1.f - tmpProj.y) / (tmpProj2.y-tmpProj.y);  //x = x0 + (x1 - x0) * (ymin - y0) / (y1 - y0);
        tmpProj.y = -1.f; //y = ymin;
    } else if (tmpProj.y > 1.f){
        tmpProj.x = tmpProj.x + (tmpProj2.x-tmpProj.x) * (1.f - tmpProj.y) / (tmpProj2.y-tmpProj.y);  //x = x0 + (x1 - x0) * (ymax - y0) / (y1 - y0);
        tmpProj.y = 1.f; //y = ymax;
    }
}

bool clipToUnit0(Point2d &tmpProj, const Point2d &tmpProj2){
    bool clipped = false, inside = false;
    if (tmpProj.x < 0.f){
        tmpProj.y = tmpProj.y + (tmpProj2.y-tmpProj.y) * (0.f - tmpProj.x) / (tmpProj2.x-tmpProj.x); //y = y0 + (y1 - y0) * (xmin - x0) / (x1 - x0);
        tmpProj.x = 0.f;                  //x = xmin;
        clipped = tmpProj.y >= 0.f && tmpProj.y <= 1.f;
    } else if (tmpProj.x > 1.f){
        tmpProj.y = tmpProj.y + (tmpProj2.y-tmpProj.y) * (1.f - tmpProj.x) / (tmpProj2.x-tmpProj.x);  //y = y0 + (y1 - y0) * (xmax - x0) / (x1 - x0);
        tmpProj.x = 1.f; //x = xmax;
        clipped = tmpProj.y >= 0.f && tmpProj.y <= 1.f;
    } else {
        inside = true;
    }
    if (tmpProj.y < 0.f){
        tmpProj.x = tmpProj.x + (tmpProj2.x-tmpProj.x) * (0.f - tmpProj.y) / (tmpProj2.y-tmpProj.y);  //x = x0 + (x1 - x0) * (ymin - y0) / (y1 - y0);
        tmpProj.y = 0.f; //y = ymin;
        clipped = tmpProj.x >= 0.f && tmpProj.x <= 1.f;
    } else if (tmpProj.y > 1.f){
        tmpProj.x = tmpProj.x + (tmpProj2.x-tmpProj.x) * (1.f - tmpProj.y) / (tmpProj2.y-tmpProj.y);  //x = x0 + (x1 - x0) * (ymax - y0) / (y1 - y0);
        tmpProj.y = 1.f; //y = ymax;
        clipped = tmpProj.x >= 0.f && tmpProj.x <= 1.f;
    } else {
        clipped = clipped || inside;
    }
    return clipped;
}

int relativeCCW(double x1, double y1,
                double x2, double y2,
                double px, double py);

int relativeCCW(double x1, double y1,
                double x2, double y2,
                double px, double py)
{
    x2 -= x1;
    y2 -= y1;
    px -= x1;
    py -= y1;
    double ccw = px * y2 - py * x2;
    if (ccw == 0.0) {
        // The point is colinear, classify based on which side of
        // the segment the point falls on.  We can calculate a
        // relative value using the projection of px,py onto the
        // segment - a negative value indicates the point projects
        // outside of the segment in the direction of the particular
        // endpoint used as the origin for the projection.
        ccw = px * x2 + py * y2;
        if (ccw > 0.0) {
            // Reverse the projection to be relative to the original x2,y2
            // x2 and y2 are simply negated.
            // px and py need to have (x2 - x1) or (y2 - y1) subtracted
            //    from them (based on the original values)
            // Since we really want to get a positive answer when the
            //    point is "beyond (x2,y2)", then we want to calculate
            //    the inverse anyway - thus we leave x2 & y2 negated.
            px -= x2;
            py -= y2;
            ccw = px * x2 + py * y2;
            if (ccw < 0.0) {
                ccw = 0.0;
            }
        }
    }
    return (ccw < 0.0) ? -1 : ((ccw > 0.0) ? 1 : 0);
}

bool linesIntersect(double x1, double y1,
                    double x2, double y2,
                    double x3, double y3,
                    double x4, double y4)
{
    return ((relativeCCW(x1, y1, x2, y2, x3, y3) *
             relativeCCW(x1, y1, x2, y2, x4, y4) <= 0)
            && (relativeCCW(x3, y3, x4, y4, x1, y1) *
                relativeCCW(x3, y3, x4, y4, x2, y2) <= 0));
}

bool linesIntersect(Point2d &pt1, Point2d &pt2, Point2d &pt3, Point2d &pt4){
    return linesIntersect(pt1.x, pt1.y, pt2.x, pt2.y,
                          pt3.x, pt3.y, pt4.x, pt4.y);
}
