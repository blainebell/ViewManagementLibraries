
#ifndef CPP_FACES3D_H
#define CPP_FACES3D_H

#include "Face.h"
#include "Cuboid.h"

class Faces3D {
 public:
    vector<Face *> *faces;
    Faces3D(vector<Face *> *faces = NULL);
    ~Faces3D();
    void clear();
    // static KBObject *createFaces3D(KBObject*);
//    virtual void valueSet(KBData &value);
//    virtual void valueObjectSet(KBObject *obj);
    virtual const string toString();
    virtual const string infoString();
    /*virtual KBObject *duplicate();
    virtual KBObject *facesDirectToStringObj();
    virtual KBObject *facesToStringObj();
    virtual KBObject *hasFacesObj();
    virtual KBObject *countObj();
    virtual KBObject *infoObj();*/
    const string facesToString();
    const string facesDirectToString();
    static string facesToString(vector<Face *> *faces);
    // virtual void valueObjectSet(KBObject *obj);
};


#endif
