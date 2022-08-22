
#include "Intersection.h"



Intersection::Intersection(int s, Vertex *v1, Vertex *v2, Vertex *i) : sign(s), vertex1(v1), vertex2(v2), intersectionVertex(i){
}

Intersection::~Intersection(){
//    if (intersectionVertex != vertex2) delete intersectionVertex;
}
ostream& operator <<(ostream &os,Intersection &obj){
    os << "\n\t\tvertex1=" << obj.vertex1 << "\n\t\tvertex2=" << obj.vertex2 << "\n\t\tintersectionVertex=" << obj.intersectionVertex;
    return os;
}
