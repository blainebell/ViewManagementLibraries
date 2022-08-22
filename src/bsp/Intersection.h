
#ifndef CPP_INTERSECTION_H
#define CPP_INTERSECTION_H

#include "Vertex.h"

class Intersection {
public:
    int sign;
    Vertex *vertex1, *vertex2, *intersectionVertex;
    Intersection(int, Vertex*, Vertex*, Vertex*);
    ~Intersection();
    friend ostream& operator <<(ostream &os,Intersection &obj);
};

#endif
