

#include "Point3d.h"
#include "stlTools.h"
#define _USE_MATH_DEFINES
#include <math.h>

Point3d::Point3d(){
    
}

Point3d::Point3d(float *pt) : x(pt[0]), y(pt[1]), z(pt[2]) {
}
Point3d::Point3d(double *pt) : x(pt[0]), y(pt[1]), z(pt[2]) {
}

Point3d::Point3d(double _x, double _y, double _z) : x(_x), y(_y), z(_z) {
}
Point3d::Point3d(Point3d *pt) : x(pt->x), y(pt->y), z(pt->z) {
}

bool Point3d::equals(const Point3d &pt){
    return doubleIsEqual(pt.x, x ) &&
            doubleIsEqual(pt.y, y ) &&
            doubleIsEqual(pt.z, z );
}

void Point3d::cross(Point3d &v1, Point3d &v2){
    x = v1.y * v2.z - v1.z * v2.y;
    y = v1.z * v2.x - v1.x * v2.z;
    z = v1.x * v2.y - v1.y * v2.x;
}
void Point3d::add(Point3d &pt){
    x += pt.x;
    y += pt.y;
    z += pt.z;
}

double Point3d::length(){
    return sqrt(x*x + y*y + z*z);
}

void Point3d::writeTo(float *fptr) const {
    fptr[0] = x;
    fptr[1] = y;
    fptr[2] = z;
}
void Point3d::normalize(){
    double len = sqrt(x*x + y*y + z*z);
    x = x/len;
    y = y/len;
    z = z/len;
}

ostream& operator <<(ostream &os, Point3d &obj){
    os << "( " << obj.x << ", " << obj.y << ", " << obj.z << " ) " ;
    return os;
}
ostream& operator <<(ostream &os, const Point3d &obj){
    os << "( " << obj.x << ", " << obj.y << ", " << obj.z << " ) " ;
    return os;
}

Point3d operator+(const Point3d &p1, const Point3d &p2){
    return Point3d(p1.x+p2.x, p1.y+p2.y, p1.z+p2.z );
}

Point3d operator-(const Point3d &p1, const Point3d &p2){
    return Point3d(p1.x-p2.x, p1.y-p2.y, p1.z-p2.z );
}

Point3d operator*(const Point3d &p1, const Point3d &p2){
    return Point3d(p1.x*p2.x, p1.y*p2.y, p1.z*p2.z );
}

Point3d operator/(const Point3d &p1, const double num){
    return Point3d(p1.x/num, p1.y/num, p1.z/num);
}
Point3d operator*(const Point3d &p1, const double num){
    return Point3d(p1.x*num, p1.y*num, p1.z*num);
}

Point3d& Point3d::operator+=(const Point3d& rhs){
    x += rhs.x;
    y += rhs.y;
    z += rhs.z;
    return *this;
}

float dot_product(const Point3d &p1, const Point3d &p2){
    return p1.x * p2.x + p1.y * p2.y + p1.z * p2.z;
}
float dot_product(Point3d &p1, Point3d &p2){
    return p1.x * p2.x + p1.y * p2.y + p1.z * p2.z;
}

float angle(Point3d &p1, Point3d &p2){
    double vDot = dot_product(p1, p2) / (p1.length() * p2.length());
    if( vDot < -1.0) vDot = -1.0;
    if( vDot >  1.0) vDot =  1.0;
    return((double) (acos( vDot )));
}

float angle_deg(Point3d &p1, Point3d &p2){
    float rad = angle(p1, p2);
    return rad * 180.f / M_PI;
}
