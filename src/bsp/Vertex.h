#ifndef CPP_VERTEX_H
#define CPP_VERTEX_H

#include <iostream>
#include "Point3d.h"

using namespace std;

class Vertex : public Point3d {
public:
//    void *userData;
    Vertex *next;
    Vertex* getNext() const { return next; }
    Vertex();
    Vertex(double x, double y, double z);
    Vertex(Vertex *);
    Vertex( string );
    bool equals(const Vertex*);
    double length();
    string toString();
    friend ostream& operator <<(ostream &os,Vertex &obj);
};


#endif
