
#include "Faces3D.h"
#include "stlTools.h"
#include <sstream>

#define CLASSNAME "Faces3D"

Faces3D::Faces3D(vector<Face *> *f) : faces(f) {
}

Faces3D::~Faces3D(){
    clear();
    if (faces)
        delete faces;
}
void Faces3D::clear(){
    if (faces){
        for (auto it=faces->begin(); it!=faces->end();it++){
            delete *it;
        }
        faces->clear();
    }
}
/*
KBObject *Faces3D::facesToStringObj(){
    return new KBString(facesToString());
}
KBObject *Faces3D::facesDirectToStringObj(){
    return new KBString(facesDirectToString());
}

KBObject *Faces3D::hasFacesObj(){
    return new KBBoolean(faces && !faces->empty());
}
KBObject *Faces3D::duplicate(){
    Faces3D *newFaces = new Faces3D();
    if (faces){
        for (auto it=faces->begin(); it!=faces->end(); it++){
            newFaces->faces->push_back((*it)->copy());
        }
    }
    return newFaces;
}
*/
const string Faces3D::facesToString(){
    stringstream ss;
    ss << "Faces3D " ;
    if (faces){
      ss << Faces3D::facesToString(faces);
    }
    return ss.str();
}

string Faces3D::facesToString(vector<Face *> *faces){
  stringstream ss;
  if (faces){
      ss << faces->size() << " faces : " << endl;
      for (auto it=faces->begin(); it!=faces->end(); it++){
          ss << "   " << (*it)->toString() << endl;
      }
  } else {
      ss << "0 faces" << endl;
  }
  return ss.str();

}

const string Faces3D::facesDirectToString(){
    stringstream ss;
    if (faces){
        for (auto it=faces->begin(); it!=faces->end(); ){
            ss << "[" << (*it)->toString() << "] ";
            it++;
        }
    }
    return ss.str();
}

const string Faces3D::toString(){
    return facesDirectToString();
}
const string Faces3D::infoString(){
    stringstream ss;
    ss << "Faces3D: ";
    if (faces)
        ss << faces->size() << " faces";
    else
        ss << "NULL";
    return ss.str();
}
/*
KBObject *Faces3D::infoObj(){
    return (new KBString(infoString()));
}
void Faces3D::valueObjectSet(KBObject *obj){
    Faces3D *facesa = dynamic_cast<Faces3D*>(resolveKBObject(obj));
    if (facesa){
        faces = facesa->faces;
    } else {
        KBString *facesstr = dynamic_cast<KBString*>(resolveKBObject(obj));
        if (facesstr){
            if (!faces) faces = new vector<Face*>();
            clear();
            vector<string> allfaces;
            string fstr = facesstr->toString();
            Tokenize(fstr, allfaces, "[]", false, true);
            int fn = 0;
            vector<float> facevals;
            for (auto facesit = allfaces.begin(); facesit != allfaces.end(); facesit++){
                vector<string> allvals;
                Tokenize(*facesit, allvals, "() ,", false, true);
                
                facevals.clear();
                for (auto it=allvals.begin(); it!=allvals.end(); it++){
                    float fval = (float)StringToDouble(*it);
                    facevals.push_back(fval);
                }
                faces->push_back(new Face(facevals));
                fn++;
            }
        } else {
            faces = NULL;
        }
    }
}
KBObject *Faces3D::countObj(){
    return new KBInteger(faces ? (int)faces->size() : 0);
}

KBObject *Faces3D::createFaces3D(KBObject *argobj){
    Faces3D *faces = new Faces3D();
    if (argobj){
        faces->valueObjectSet(argobj);
    }
    return faces;
}
*/
