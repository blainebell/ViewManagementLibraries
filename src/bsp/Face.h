#ifndef CPP_FACE_H
#define CPP_FACE_H

#include "Vertex.h"
#include "Intersection.h"
#include <vector>

using namespace std;

class Plane;

class Face {
public:
    Vertex *vhead;
    Plane *plane;
    Face(Vertex *v=NULL);
    Face(const vector<float> &arr);
    ~Face();
    int numVertices();
    void writeLines(float *, int&);
    Vertex *getHead() const { return vhead; }
    void setVertices(Vertex *v);
    Plane *getPlane() const { return plane; }
    bool computePlane(){ return computePlane(vhead, vhead); };
    bool computePlane(Vertex *vh, Vertex *end);
    bool straddlesPlane(Plane *);
    vector<Intersection*> *findIntersectionsOfPlane(Plane*);
    Face *createOtherFace(Intersection *i1, Intersection *i2);
    friend ostream& operator <<(ostream &os, Face &obj);
    const string toString();
    Face *copy();
    float area();
    bool has_area();
    bool pointIsInside(const Point3d&);
    double pointIsInsideVal(const Point3d&);
    const string commaDelString();
};

string faceListToString(vector<Face*> &obj);
string faceListToStringDirect(vector<Face*> &obj);
string faceListToPlanesStringDirect(vector<Face*> &obj);

vector<Face*> *duplicateFaceList(vector<Face*> *);

#endif
