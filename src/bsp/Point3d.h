

#ifndef CPP_POINT3D_H
#define CPP_POINT3D_H

#include <iostream>

using namespace std;

class Point3d {
public:
    double x, y, z;
    Point3d();
    Point3d(float *);
    Point3d(double *);
    Point3d(double,double,double);
    Point3d(Point3d*);
    void set(double _x, double _y, double _z){ x=_x; y=_y; z=_z; }
    friend ostream& operator <<(ostream &os, Point3d &obj);
    friend ostream& operator <<(ostream &os, const Point3d &obj);
    bool equals(const Point3d &);
    void cross(Point3d&, Point3d&);
    void add(Point3d&);
    void normalize();
    double length();
    void writeTo(float*) const;
    Point3d& operator+=(const Point3d& rhs);
    void setDimension(int dim,double val){ 
      switch(dim){
      case 0:
	x = val; break;
      case 1:
	y = val; break;
      case 2:
	z = val; break;
      default:
        cerr << "WARNING: Point3d setDimension: dim=" << dim << endl;
      }
    }
    friend Point3d operator+(const Point3d &p1, const Point3d &p2);
    friend Point3d operator-(const Point3d &p1, const Point3d &p2);
    friend Point3d operator*(const Point3d &p1, const Point3d &p2);
    friend Point3d operator*(const Point3d &p1, const double num);
    friend Point3d operator/(const Point3d &p1, const double num);
    friend float dot_product(const Point3d&,const Point3d&);
    friend float dot_product(Point3d&,Point3d&);
    friend float angle(Point3d &p1, Point3d &p2);
    friend float angle_deg(Point3d &p1, Point3d &p2);
};

#endif
